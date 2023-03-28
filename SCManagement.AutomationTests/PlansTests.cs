
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
    internal class PlansTests
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
        public void TheCreatePlansTest()
        {

            //Create Atlhete Code and Use it
            #region Create Athlete
            driver.Navigate().GoToUrl("https://localhost:7111/");
            driver.Manage().Window.Maximize();
            driver.FindElement(By.Id("signin")).Click();
            driver.FindElement(By.Id("Input_Email")).Clear();
            driver.FindElement(By.Id("Input_Email")).SendKeys("user@scmanagement.me");
            driver.FindElement(By.Id("Input_Password")).Clear();
            driver.FindElement(By.Id("Input_Password")).SendKeys("User123!");
            driver.FindElement(By.Id("login-submit")).Click();
            driver.FindElement(By.XPath("//*[@id=\"dropdownMenuButton1\"]/img")).Click();
            driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[7]/form/div/select")).Click();
            new SelectElement(driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[7]/form/div/select"))).SelectByText("PlansClub (Club Administrator)");
            driver.FindElement(By.LinkText("Clubs")).Click();
            driver.FindElement(By.LinkText("My Club")).Click();
            driver.FindElement(By.XPath("//div[6]/a/div/div/img")).Click();
            driver.FindElement(By.XPath("//button[@onclick='openCreate()']")).Click();
            driver.FindElement(By.Id("ExpireDate")).Clear();
            driver.FindElement(By.Id("ExpireDate")).SendKeys("2023-10-28");
            driver.FindElement(By.XPath("//input[@value='Create']")).Click();
            driver.FindElement(By.XPath("//div[@id='modal-inner-content']/div/div/dl/dd")).Click();
            String code = driver.FindElement(By.XPath("//div[@id='modal-inner-content']/div/div/dl/dd")).Text;
            driver.FindElement(By.XPath("//span[@onclick=\"$('#modal').hide()\"]")).Click();
            driver.FindElement(By.Id("dropdownMenuButton1")).Click();
            driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[9]/form/button")).Click();
            driver.FindElement(By.Id("signin")).Click();
            driver.FindElement(By.Id("Input_Email")).Click();
            driver.FindElement(By.Id("Input_Email")).Clear();
            driver.FindElement(By.Id("Input_Email")).SendKeys("admin@scmanagement.me");
            driver.FindElement(By.Id("Input_Password")).Clear();
            driver.FindElement(By.Id("Input_Password")).SendKeys("Admin123!");
            driver.FindElement(By.Id("login-submit")).Click();
            driver.FindElement(By.LinkText("Clubs")).Click();
            driver.FindElement(By.LinkText("Join with code")).Click();
            driver.FindElement(By.Id("Code")).Click();
            driver.FindElement(By.Id("Code")).Clear();
            driver.FindElement(By.Id("Code")).SendKeys(code);
            driver.FindElement(By.XPath("//input[@value='Join']")).Click();
            driver.FindElement(By.XPath("//*[@id=\"dropdownMenuButton1\"]/img")).Click();
            driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[9]/form/button")).Click();
            driver.FindElement(By.Id("signin")).Click();
            driver.FindElement(By.Id("Input_Email")).Click();
            driver.FindElement(By.Id("Input_Email")).Clear();
            driver.FindElement(By.Id("Input_Email")).SendKeys("user@scmanagement.me");
            driver.FindElement(By.Id("Input_Password")).Clear();
            driver.FindElement(By.Id("Input_Password")).SendKeys("User123!");
            driver.FindElement(By.Id("login-submit")).Click();
            driver.FindElement(By.LinkText("Clubs")).Click();
            driver.FindElement(By.LinkText("My Club")).Click();
            driver.FindElement(By.XPath("//div[10]/a/div/div/img")).Click();
            driver.FindElement(By.LinkText("Create Team")).Click();
            driver.FindElement(By.Id("Name")).Click();
            driver.FindElement(By.Id("Name")).Clear();
            driver.FindElement(By.Id("Name")).SendKeys("TeamTEst");
            driver.FindElement(By.XPath("//input[@value='Create']")).Click();
            driver.FindElement(By.LinkText("Edit")).Click();
            driver.FindElement(By.LinkText("Add Athlete")).Click();
            driver.FindElement(By.Name("selectedAthletes")).Click();
            driver.FindElement(By.XPath("//div[@id='modal-inner-content']/form/button")).Click();
            driver.FindElement(By.XPath("//input[@value='Save']")).Click();
            #endregion


            //Create Meal Plan
            //driver.FindElement(By.LinkText("Trainer zone")).Click();
            //driver.FindElement(By.LinkText("Training zone")).Click();
            //driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Teams'])[1]/following::label[1]")).Click();
            //driver.FindElement(By.XPath("//a[@onclick=\"$('#AthletesMeal').show();\"]")).Click();
            //driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='×'])[5]/following::strong[1]")).Click();
            //driver.FindElement(By.Id("Name")).Click();
            //driver.FindElement(By.Id("Name")).Clear();
            //driver.FindElement(By.Id("Name")).SendKeys("0-SimplePLan");
            //driver.FindElement(By.Id("Description")).Click();
            //driver.FindElement(By.Id("Description")).Clear();
            //driver.FindElement(By.Id("Description")).SendKeys("This is a description");
            //driver.FindElement(By.Id("StartDate")).Click();
            //driver.FindElement(By.Id("StartDate")).Clear();
            //driver.FindElement(By.Id("StartDate")).SendKeys("2023-10-27");
            //driver.FindElement(By.Id("EndDate")).Click();
            //driver.FindElement(By.Id("EndDate")).Clear();
            //driver.FindElement(By.Id("EndDate")).SendKeys("2023-10-30");
            //driver.FindElement(By.Id("MealPlanSessions_0__MealName")).Click();
            //driver.FindElement(By.Id("MealPlanSessions_0__MealName")).Clear();
            //driver.FindElement(By.Id("MealPlanSessions_0__MealName")).SendKeys("Bread");
            //driver.FindElement(By.Id("MealPlanSessions_0__MealDescription")).Click();
            //driver.FindElement(By.Id("MealPlanSessions_0__MealDescription")).Clear();
            //driver.FindElement(By.Id("MealPlanSessions_0__MealDescription")).SendKeys("Eat");
            //driver.FindElement(By.Id("MealPlanSessions_0__Time")).Click();
            //driver.FindElement(By.Id("MealPlanSessions_0__Time")).Clear();
            //driver.FindElement(By.Id("MealPlanSessions_0__Time")).SendKeys("05:00");
            //driver.FindElement(By.Name("action")).Click();
            //driver.FindElement(By.Id("MealPlanSessions_1__MealName")).Click();
            //driver.FindElement(By.Id("MealPlanSessions_1__MealName")).Clear();
            //driver.FindElement(By.Id("MealPlanSessions_1__MealName")).SendKeys("asd");
            //driver.FindElement(By.XPath("//button[@onclick='removeElement(\"b1becf69-9c06-499c-b1f9-7f959d24c6fc\")']")).Click();
            //driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Cancel'])[1]/following::button[1]")).Click();
            //driver.Navigate().GoToUrl("https://localhost:7111/MyClub/TrainingZone");
            //driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Teams'])[1]/following::label[1]")).Click();
            //driver.FindElement(By.XPath("//tbody[@id='athletesBody']/tr/td[5]/a[2]")).Click();
            //driver.Navigate().GoToUrl("https://localhost:7111/Plans/AthleteMealPlans/00bafd60-2c17-4a47-8b20-afb7f232dc70");
            //driver.FindElement(By.LinkText("Details")).Click();
            //driver.Navigate().GoToUrl("https://localhost:7111/Plans/MealDetails/2");
            //driver.FindElement(By.LinkText("Edit")).Click();
            //driver.Navigate().GoToUrl("https://localhost:7111/Plans/EditMealPlan/2");
            //driver.FindElement(By.Name("action")).Click();
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

