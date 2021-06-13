using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EC = SeleniumExtras.WaitHelpers.ExpectedConditions;
using NUnit;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;


namespace Test
{
    abstract class BaseTest
    {
        protected IWebDriver driver;
        protected WebDriverWait wait;

        protected void GoToURL(string url)
        {
            Logger.log("INFO", $"Opening URL: {url}");
            this.driver.Navigate().GoToUrl(url);
        }

        protected IWebElement MyFindElement(By Selector)
        {
            IWebElement ReturnElement = null;
            Logger.log("INFO", $"Looking for element: <{Selector}>");
            try
            {
                ReturnElement = this.driver.FindElement(Selector);
            }
            catch (NoSuchElementException)
            {
                Logger.log("ERROR", $"Can't find element: <{Selector}>");
            }

            if (ReturnElement != null)
            {
                Logger.log("INFO", $"Element: <{Selector}> found.");
            }

            return ReturnElement;
        }

        protected IWebElement WaitForElement(Func<IWebDriver, IWebElement> ExpectedConditions)
        {
            IWebElement ReturnElement = null;
            Logger.log("INFO", $"Waiting for element.");
            try
            {
                ReturnElement = this.wait.Until(ExpectedConditions);

            }
            catch (WebDriverTimeoutException)
            {
                Logger.log("ERROR", $"Can't wait for element.");
            }

            if (ReturnElement != null)
            {
                Logger.log("INFO", $"Element found.");
            }

            return ReturnElement;
        }

        protected bool ElementExists(By Selector)
        {
            IWebElement ReturnElement = null;
            try
            {
                ReturnElement = this.wait.Until(EC.ElementExists(Selector));
            }
            catch (WebDriverTimeoutException)
            {
                Logger.log("ERROR", $"Can't wait for element.");
            }

            return ReturnElement != null;
        }

        public void PopulateInput(By Selector, String TextToType)
        {
            Logger.log("INFO", $"Populate input element: <{Selector}> = '{TextToType}'");

            // this.MyFindElement(Selector).SendKeys(TextToType);
            IWebElement inputElement = this.MyFindElement(Selector);
            inputElement.SendKeys(TextToType);
        }

        protected void ExplicitWait(int waitTime)
        {
            Logger.log("INFO", $"Sleeping: {waitTime}ms");
            System.Threading.Thread.Sleep(waitTime);
        }
    }
}

