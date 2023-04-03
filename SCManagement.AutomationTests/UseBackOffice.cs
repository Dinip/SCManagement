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
    public class UseBackOffice
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
        public void TheUseBackOfficeTest()
        {
            driver.Navigate().GoToUrl("https://localhost:7111/");
            driver.Manage().Window.Maximize();
            driver.FindElement(By.Id("signin")).Click();
            driver.FindElement(By.Id("Input_Email")).Clear();
            driver.FindElement(By.Id("Input_Email")).SendKeys("admin@scmanagement.me");
            driver.FindElement(By.Id("Input_Password")).Clear();
            driver.FindElement(By.Id("Input_Password")).SendKeys("Admin123!");
            driver.FindElement(By.Id("account")).Click();
            driver.FindElement(By.Id("login-submit")).Click();
            driver.FindElement(By.LinkText("Backoffice")).Click();
            driver.FindElement(By.LinkText("Operations")).Click();
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='User Access'])[1]/following::div[4]")).Click();
            driver.FindElement(By.LinkText("Create New")).Click();
            driver.FindElement(By.Id("ModalityTranslations_0__Value")).Click();
            driver.FindElement(By.Id("ModalityTranslations_0__Value")).Clear();
            driver.FindElement(By.Id("ModalityTranslations_0__Value")).SendKeys("Teste");
            driver.FindElement(By.Id("ModalityTranslations_1__Value")).Click();
            driver.FindElement(By.Id("ModalityTranslations_1__Value")).Clear();
            driver.FindElement(By.Id("ModalityTranslations_1__Value")).SendKeys("Teste");
            driver.FindElement(By.XPath("//input[@value='Create']")).Click();
            driver.FindElement(By.LinkText("Backoffice")).Click();
            driver.FindElement(By.LinkText("Operations")).Click();
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Modalities'])[1]/following::div[4]")).Click();
            driver.FindElement(By.LinkText("Create Plan")).Click();
            driver.FindElement(By.Id("Name")).Click();
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Confirmed'])[1]/following::main[1]")).Click();
            driver.FindElement(By.Id("Name")).Clear();
            driver.FindElement(By.Id("Name")).SendKeys("0- Simple");
            driver.FindElement(By.Id("Value")).Click();
            driver.FindElement(By.Id("Value")).Clear();
            driver.FindElement(By.Id("Value")).SendKeys("20");
            driver.FindElement(By.Id("Frequency")).Click();
            new SelectElement(driver.FindElement(By.Id("Frequency"))).SelectByText("Monthly");
            driver.FindElement(By.Id("AthleteSlots")).Click();
            driver.FindElement(By.Id("AthleteSlots")).Clear();
            driver.FindElement(By.Id("AthleteSlots")).SendKeys("10");
            driver.FindElement(By.XPath("//input[@value='Create']")).Click();
            driver.FindElement(By.XPath("//input[@value='Enable']")).Click();
            driver.FindElement(By.LinkText("Yes")).Click();
            driver.FindElement(By.XPath("//input[@value='Disable']")).Click();
            driver.FindElement(By.LinkText("Yes")).Click();
            driver.FindElement(By.XPath("//table[@id='table-plans']/tbody/tr[1]/td[6]/div/a")).Click();
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Athlete Slots'])[1]/following::label[1]")).Click();
            driver.FindElement(By.XPath("//input[@value='Save']")).Click();
            driver.FindElement(By.XPath("//table[@id='table-plans']/tbody/tr[1]/td[6]/div/a")).Click();
            driver.FindElement(By.XPath("//input[@value='Delete']")).Click();
            driver.FindElement(By.LinkText("Yes")).Click();
            driver.FindElement(By.LinkText("Backoffice")).Click();
            driver.FindElement(By.LinkText("Operations")).Click();
            acceptNextAlert = true;
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Operations'])[2]/following::div[5]")).Click();
            driver.FindElement(By.XPath("//table[@id='table-users']/tbody/tr[2]/td[3]/form/input[3]")).Click();
            acceptNextAlert = true;
            CloseAlertAndGetItsText();
            driver.FindElement(By.XPath("//table[@id='table-users']/tbody/tr[2]/td[3]/form/input[3]")).Click();
            CloseAlertAndGetItsText();
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
