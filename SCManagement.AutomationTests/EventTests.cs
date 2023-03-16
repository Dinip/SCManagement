using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests
{
    [TestFixture]
    public class EventTests
    {
        private IWebDriver driver;
        private StringBuilder verificationErrors;
        private string baseURL;
        private bool acceptNextAlert = true;
        
        [SetUp]
        public void SetupTest()
        {
            driver = new ChromeDriver();
            baseURL = "https://www.google.com/";
            verificationErrors = new StringBuilder();
        }
        
        [TearDown]
        public void TeardownTest()
        {
            try
            {
                driver.Quit();
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
            Assert.AreEqual("", verificationErrors.ToString());
        }
        
        [Test]
        public void TheCreateEventWithoutRouteTest()
        {
            driver.Navigate().GoToUrl("https://localhost:7111/");
            driver.Manage().Window.Maximize();
            driver.FindElement(By.LinkText("Sign in")).Click();
            driver.FindElement(By.Id("Input_Email")).Click();
            driver.FindElement(By.Id("Input_Email")).Clear();
            driver.FindElement(By.Id("Input_Email")).SendKeys("user@scmanagement.me");
            driver.FindElement(By.Id("Input_Password")).Clear();
            driver.FindElement(By.Id("Input_Password")).SendKeys("User123!");
            driver.FindElement(By.Id("login-submit")).Click();
            driver.FindElement(By.LinkText("Events")).Click();
            driver.FindElement(By.LinkText("Create New")).Click();
            driver.FindElement(By.Id("EventTranslationsName_0__Value")).Click();
            driver.FindElement(By.Id("EventTranslationsName_0__Value")).Clear();
            driver.FindElement(By.Id("EventTranslationsName_0__Value")).SendKeys("Simple Event");
            driver.FindElement(By.Id("ckbIsPublic")).Click();
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Logout'])[2]/following::main[1]")).Click();
            driver.FindElement(By.Id("txtMaxEventEnrolls")).Clear();
            driver.FindElement(By.Id("txtMaxEventEnrolls")).SendKeys("15");
            driver.FindElement(By.Id("selectEventResultType")).Click();
            new SelectElement(driver.FindElement(By.Id("selectEventResultType"))).SelectByText("Position");
            if (driver.FindElement(By.XPath("//input[@type='text']")).Displayed)
            {
                var button = driver.FindElement(By.XPath("//input[@type='text']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", button);
                Thread.Sleep(2000);
                button.Click();
            }
            driver.FindElement(By.XPath("//input[@type='text']")).Clear();
            driver.FindElement(By.XPath("//input[@type='text']")).SendKeys("mar");
            driver.FindElement(By.XPath("//input[@type='text']")).Clear();
            driver.FindElement(By.XPath("//input[@type='text']")).SendKeys("Rua Maria Adelaide Rosado Pinto, 2900-693 Setúbal, Setúbal, Portugal");
            Thread.Sleep(500);
            driver.FindElement(By.XPath("//*[@id=\"map\"]/div[3]/div[1]/div/div[1]/ul/li[1]/a/div/div[2]")).Click();
            
            driver.FindElement(By.XPath("//button[@id='save-button']/i")).Click();
            driver.FindElement(By.XPath("//input[@value='Create']")).Click();
            String CreatedEvent = driver.FindElement(By.XPath("//table[@id='table_id']/tbody/tr[3]/td")).Text;
            for (int second = 0;; second++) {
                if (second >= 60) Assert.Fail("timeout");
                try
                {
                    if (IsElementPresent(By.XPath("//table[@id='table_id']/tbody/tr[3]/td"))) break;
                }
                catch (Exception)
                {}
                Thread.Sleep(1000);
            }
        }
        private bool IsElementPresent(By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
        
        private bool IsAlertPresent()
        {
            try
            {
                driver.SwitchTo().Alert();
                return true;
            }
            catch (NoAlertPresentException)
            {
                return false;
            }
        }
        
        private string CloseAlertAndGetItsText() {
            try {
                IAlert alert = driver.SwitchTo().Alert();
                string alertText = alert.Text;
                if (acceptNextAlert) {
                    alert.Accept();
                } else {
                    alert.Dismiss();
                }
                return alertText;
            } finally {
                acceptNextAlert = true;
            }
        }
    }
}
