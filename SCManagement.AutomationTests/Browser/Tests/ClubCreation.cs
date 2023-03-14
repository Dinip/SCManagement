using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace SCManagement.AutomationTests.Browser.Tests
{
    [TestFixture]
    internal class ClubCreation
    {
        private IWebDriver driver;
        private StringBuilder verificationErrors;
        private string baseURL;
        private bool acceptNextAlert = true;

        [SetUp]
        public void SetupTest()
        {
            driver = new FirefoxDriver();
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
        public void TheUntitledTestCaseTest()
        {
            driver.Navigate().GoToUrl("https://localhost:7111/");
            driver.FindElement(By.LinkText("Iniciar Sessão")).Click();
            driver.FindElement(By.Id("Input_Email")).Clear();
            driver.FindElement(By.Id("Input_Email")).SendKeys("user@scmanagement.me");
            driver.FindElement(By.Id("Input_Password")).Clear();
            driver.FindElement(By.Id("Input_Password")).SendKeys("User123!");
            driver.FindElement(By.Id("login-submit")).Click();
            driver.FindElement(By.LinkText("Planos de clube")).Click();
            driver.FindElement(By.LinkText("Selecionar e criar clube")).Click();
            driver.FindElement(By.Id("idName")).Click();
            driver.FindElement(By.Id("idName")).Clear();
            driver.FindElement(By.Id("idName")).SendKeys("Simba");
            driver.FindElement(By.XPath("//div[@id='ModalitiesIds_chosen']/ul")).Click();
            driver.FindElement(By.XPath("//div[@id='ModalitiesIds_chosen']/div/ul/li")).Click();
            driver.FindElement(By.XPath("//div[@id='ModalitiesIds_chosen']/ul")).Click();
            driver.FindElement(By.XPath("//div[@id='ModalitiesIds_chosen']/div/ul/li[3]")).Click();
            driver.FindElement(By.XPath("//input[@value='Criar']")).Click();
            try
            {
                Assert.AreEqual("Club Plan XS (Simba)", driver.FindElement(By.XPath("//table[@id='table_id']/tbody/tr[2]/td[6]")).Text);
            }
            catch (AssertionException e)
            {
                verificationErrors.Append(e.Message);
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

        private string CloseAlertAndGetItsText()
        {
            try
            {
                IAlert alert = driver.SwitchTo().Alert();
                string alertText = alert.Text;
                if (acceptNextAlert)
                {
                    alert.Accept();
                }
                else
                {
                    alert.Dismiss();
                }
                return alertText;
            }
            finally
            {
                acceptNextAlert = true;
            }
        }
    }

}