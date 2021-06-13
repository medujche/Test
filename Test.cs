using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using EC = SeleniumExtras.WaitHelpers.ExpectedConditions;
using Test.Lib;
using System.Collections.ObjectModel;



namespace Test.Lib
{
    class Test : BaseTest
    {

        [SetUp]
        public void SetUp()
        {
            Logger.setFileName(@"C:\Kurs\Tests.log");
            this.driver = new ChromeDriver();
            this.driver.Manage().Window.Maximize();
            this.wait = new WebDriverWait(this.driver, new TimeSpan(0, 0, 10));
        }

        [TearDown]
        public void TearDown()
        {
            this.driver.Close();
        }

        [Test]
        [Category("http://test.qa.rs/")]
        public void TestLogin()
        {
            Logger.beginTest("TestLogin");
            Logger.log("INFO", "Starting test.");

            this.GoToURL("http://test.qa.rs/");
            Assert.IsTrue(Login("jovana", "1234"));

            Logger.endTest();
        }
        [Test]
        [Category("http://test.qa.rs/")]
        public void TestCheckout()
        {
          


            Logger.beginTest("TestCheckout");
            Logger.log("INFO", "Starting test.");

            this.GoToURL("http://test.qa.rs/");
            Assert.IsTrue(Login("jovana", "1234"));

            string PackageName = ("Burger");
            string PackageQuantity = "1";

            IWebElement quantity = this.MyFindElement(
                By.XPath(
                "//div[@class='panel panel-danger']//input[@name='quantity']"
                ));
            quantity.SendKeys("1");

            IWebElement sideDish = this.MyFindElement(
                By.XPath(
                "//div[@class='panel panel-danger']//select[@name='side']//option[@value='or'"));

            IWebElement orderNowButton = this.MyFindElement(
                By.XPath("//div[@class='panel panel-danger']//input[@type='submit']")
            );
            orderNowButton.Click();
            this.WaitForElement(
               EC.ElementIsVisible(
                   By.XPath("//h1[contains(., 'Order #')]")
               ));




            Assert.AreEqual(this.driver.Url, "http://test.qa.rs/order");

            ReadOnlyCollection<IWebElement> redovi = this.driver.FindElements(
                By.XPath(
                    $"//tr//td[contains(text(), '{PackageName.ToUpper()}')]//parent::tr//td[contains(text(), '{PackageQuantity}')]//parent::tr"
                )
            );

            Assert.GreaterOrEqual(redovi.Count, 1);

            IWebElement totalColumn = this.MyFindElement(By.XPath("//td[contains(., 'Total:')]"));

            string cartTotal = totalColumn.Text.Substring(7);
            //string cartTotal = totalColumn.Text.Replace("Total: ", "");

            Logger.log("INFO", $"Cart price is: {cartTotal}");

            //

            IWebElement checkoutButton = this.MyFindElement(By.Name("checkout"));
            checkoutButton?.Click();

            IWebElement checkoutSuccess = this.WaitForElement(
                EC.ElementIsVisible(
                    By.XPath("//h2[contains(., 'Order #')]")
                )
            );

            Assert.IsTrue(checkoutSuccess.Displayed);

            int OrderNumberStart = checkoutSuccess.Text.IndexOf("#");
            int OrderNumberEnd = checkoutSuccess.Text.IndexOf(")", OrderNumberStart);
            int OrderNumberLength = OrderNumberEnd - OrderNumberStart;
            string OrderNumber = checkoutSuccess.Text.Substring(OrderNumberStart, OrderNumberLength);
            Logger.log("INFO", $"Order number = {OrderNumber}");

            IWebElement h3charged = this.MyFindElement(By.XPath("//h3[contains(., 'Your credit card has been charged')]"));

            string cardCharged = h3charged.Text.Substring(h3charged.Text.IndexOf("$"));
            Logger.log("INFO", $"Card charged: {cardCharged}");

            Assert.AreEqual(cartTotal, cardCharged);

            IWebElement historyLink = this.MyFindElement(By.LinkText("Order history"));

            Assert.IsTrue(historyLink.Displayed);
            historyLink.Click();

            string orderRowXpath = $"//tr[contains(., '{OrderNumber}')]";

            IWebElement HistoryOrderTotal = this.MyFindElement(
                By.XPath(orderRowXpath + "/td[@class='total']")
            );

            IWebElement HistoryOrderStatus = this.MyFindElement(
                By.XPath(orderRowXpath + "/td[@class='status']")
            );

            Logger.log("INFO", $"History order total = {HistoryOrderTotal.Text}");
            Logger.log("INFO", $"History order status = {HistoryOrderStatus.Text}");

            Assert.AreEqual("Ordered", HistoryOrderStatus.Text);

            double HistoryOrderTotalInt = Convert.ToDouble(HistoryOrderTotal.Text);
            string HistoryOrderTotalCompat = "$" + HistoryOrderTotalInt.ToString();
            Logger.log("INFO", $"History order total compatible = {HistoryOrderTotalCompat}");

            Assert.AreEqual(HistoryOrderTotalCompat, cardCharged);







            Logger.endTest();



            
           
        }
        [Test]
        [Category("http://test.qa.rs/")]

        public void TestLogout()
        {
            Logger.beginTest("TestLogout");
            Logger.log("INFO", "Starting test.");

            Assert.IsTrue(Login("jovana", "1234"));

            IWebElement logout = this.MyFindElement(By.XPath("//a[contains(., 'Logout')]"));

            Assert.IsTrue(logout.Displayed);
            logout.Click();

            IWebElement login = this.WaitForElement(
                EC.ElementIsVisible(
                    By.LinkText("Login")
                )
            );
        }
        public bool Login(string Username, string Password)
        {
            this.GoToURL("http://test.qa.rs/login");

            bool formExists = this.ElementExists(By.ClassName("form-signin"));
            Assert.IsTrue(formExists);

            IWebElement inputUsername = this.WaitForElement(EC.ElementIsVisible(By.Name("username")));
            inputUsername.SendKeys(Username);

            IWebElement inputPassword = this.MyFindElement(By.Name("password"));
            inputPassword.SendKeys(Password);

            IWebElement buttonLogin = this.MyFindElement(By.Name("login"));
            buttonLogin.Click();

            return this.ElementExists(By.XPath("//h2[contains(text(), 'Welcome back,')]"));
        }
    }
}









