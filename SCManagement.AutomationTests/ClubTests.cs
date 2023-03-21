using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests
{
    [TestFixture]
    public class ClubTests
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
        public void TheCreateClubTest()
        {
            driver.Navigate().GoToUrl("https://localhost:7111/");
            driver.FindElement(By.LinkText("Sign in")).Click();
            driver.FindElement(By.Id("Input_Email")).Clear();
            driver.FindElement(By.Id("Input_Email")).SendKeys("user@scmanagement.me");
            driver.FindElement(By.Id("Input_Password")).Clear();
            driver.FindElement(By.Id("Input_Password")).SendKeys("User123!");
            driver.FindElement(By.Id("login-submit")).Click();
            driver.FindElement(By.LinkText("Club Plans")).Click();
            driver.FindElement(By.LinkText("Select and create club")).Click();
            driver.FindElement(By.Id("idName")).Click();
            driver.FindElement(By.Id("idName")).Clear();
            driver.FindElement(By.Id("idName")).SendKeys("MyClub");
            driver.FindElement(By.XPath("//*[@id=\"ModalitiesIds\"]/option[1]")).Click();
            driver.FindElement(By.XPath("//input[@value='Create']")).Click();
        }


        [Test]
        public void TheUpdateClubTest()
        {
            driver.Navigate().GoToUrl("https://localhost:7111/");
            driver.Manage().Window.Maximize();
            driver.FindElement(By.LinkText("Sign in")).Click();
            driver.FindElement(By.Id("Input_Email")).Clear();
            driver.FindElement(By.Id("Input_Email")).SendKeys("user@scmanagement.me");
            driver.FindElement(By.Id("Input_Password")).Clear();
            driver.FindElement(By.Id("Input_Password")).SendKeys("User123!");
            driver.FindElement(By.Id("login-submit")).Click();
            driver.FindElement(By.XPath("//*[@id=\"dropdownMenuButton1\"]/img")).Click();
            driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[6]/form/div/select")).Click();
            new SelectElement(driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[6]/form/div/select"))).SelectByText("ToEditClub (Club Administrator)");
            driver.FindElement(By.LinkText("Clubs")).Click();
            driver.FindElement(By.LinkText("My Club")).Click();
            driver.FindElement(By.LinkText("Edit")).Click();
            driver.FindElement(By.Id("Name")).Click();
            driver.FindElement(By.Id("Name")).Clear();
            driver.FindElement(By.Id("Name")).SendKeys("ToEditClubEdited");
            driver.FindElement(By.Id("Email")).Click();
            driver.FindElement(By.Id("Email")).Clear();
            driver.FindElement(By.Id("Email")).SendKeys("myClubTest@scmanagement.me");
            driver.FindElement(By.Id("PhoneNumber")).Click();
            driver.FindElement(By.Id("PhoneNumber")).Clear();
            driver.FindElement(By.Id("PhoneNumber")).SendKeys("939393939");
            driver.FindElement(By.Id("ClubTranslationsTerms_0__Value")).Click();
            driver.FindElement(By.Id("ClubTranslationsTerms_0__Value")).Clear();
            driver.FindElement(By.Id("ClubTranslationsTerms_0__Value")).SendKeys("Our Terms and Conditions resume");
            driver.FindElement(By.Id("ClubTranslationsAbout_0__Value")).Click();
            driver.FindElement(By.Id("ClubTranslationsAbout_0__Value")).Clear();
            driver.FindElement(By.Id("ClubTranslationsAbout_0__Value")).SendKeys("This is Club Test Edit");
            driver.FindElement(By.XPath("//input[@value='translate']")).Click();
            driver.FindElement(By.Id("btn-about-pt-PT")).Click();
            driver.FindElement(By.Id("ClubTranslationsAbout_1__Value")).Click();
            driver.FindElement(By.Id("ClubTranslationsAbout_1__Value")).Click();
            driver.FindElement(By.Id("ClubTranslationsAbout_1__Value")).Click();
            String AboutUS = driver.FindElement(By.Id("ClubTranslationsAbout_1__Value")).GetAttribute("value");
            driver.FindElement(By.Id("ClubTranslationsAbout_1__Value")).Click();


            if (driver.FindElement(By.XPath("//*[@id=\"myform\"]/i")).Displayed)
            {
                var button = driver.FindElement(By.XPath("//*[@id=\"myform\"]/i"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", button);
                Thread.Sleep(500);
                button.Click();
            }
            if (driver.FindElement(By.XPath("//div[@id='map']/div[3]/div/div/input")).Displayed)
            {
                var button = driver.FindElement(By.XPath("//div[@id='map']/div[3]/div/div/input"));
                ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, 0);");
                Thread.Sleep(500);
                button.Click();
                button.Clear();
                button.SendKeys("Rua Maria Adelaide Rosado");
            }

            Thread.Sleep(500);
            driver.FindElement(By.XPath("//*[@id=\"map\"]/div[3]/div[1]/div/div[1]/ul/li[1]/a/div/div[2]")).Click();

            IWebElement canvas = driver.FindElement(By.XPath("//div[@id='map']/div[2]/canvas"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", canvas);
            Thread.Sleep(500);
            Actions builder = new Actions(driver);
            builder.MoveToElement(canvas, 100, 100).Click().Build().Perform();

            if (driver.FindElement(By.XPath("//button[@id='save-button']/i")).Displayed)
            {
                var button = driver.FindElement(By.XPath("//button[@id='save-button']/i"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", button);
                Thread.Sleep(500);
                button.Click();

                //Agora é preciso clicar no mapa, ja n é escrever a rua
            }

            if (driver.FindElement(By.XPath("//*[@id=\"myform\"]/div[7]/input")).Displayed)
            {
                var button = driver.FindElement(By.Id("btnSaveForm"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", button);
                Thread.Sleep(500);
                button.Click();
            }
            driver.FindElement(By.LinkText("Public Page")).Click();
            driver.FindElement(By.XPath("//div[@id='colorthiefobj']/div/div[2]/h1")).Click();
            String ClubTitle = driver.FindElement(By.XPath("//div[@id='colorthiefobj']/div/div[2]/h1")).Text;
            Assert.AreEqual("ToEditClubEdited", driver.FindElement(By.XPath("//div[@id='colorthiefobj']/div/div[2]/h1")).Text);
            String AboutUs = driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='About Us:'])[1]/following::span[1]")).Text;
            Assert.AreEqual("This is Club Test Edit", driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='About Us:'])[1]/following::span[1]")).Text);
            String ClubEmail = driver.FindElement(By.LinkText("myClubTest@scmanagement.me")).Text;
            Assert.AreEqual("myClubTest@scmanagement.me", driver.FindElement(By.LinkText("myClubTest@scmanagement.me")).Text);
            String ClubContact = driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='myClubTest@scmanagement.me'])[1]/following::span[1]")).Text;
            Assert.AreEqual("939393939", driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='myClubTest@scmanagement.me'])[1]/following::span[1]")).Text);
            String TermsAndCondition = driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Terms and conditions:'])[1]/following::span[1]")).Text;
            Assert.AreEqual("Our Terms and Conditions resume", driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Terms and conditions:'])[1]/following::span[1]")).Text);


            driver.FindElement(By.LinkText("Clubs")).Click();
            driver.FindElement(By.LinkText("My Club")).Click();
            driver.FindElement(By.LinkText("Edit")).Click();
            driver.FindElement(By.Id("Name")).Click();
            driver.FindElement(By.Id("Name")).Clear();
            driver.FindElement(By.Id("Name")).SendKeys("ToEditClub");
            if (driver.FindElement(By.XPath("//*[@id=\"myform\"]/div[7]/input")).Displayed)
            {
                var button = driver.FindElement(By.Id("btnSaveForm"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", button);
                Thread.Sleep(500);
                button.Click();
            }
        }

        [Test]
        public void TheCreateCodeAndUseItTest()
        {
            driver.Navigate().GoToUrl("https://localhost:7111/");
            driver.Manage().Window.Maximize();
            driver.FindElement(By.LinkText("Sign in")).Click();
            driver.FindElement(By.Id("Input_Email")).Clear();
            driver.FindElement(By.Id("Input_Email")).SendKeys("user@scmanagement.me");
            driver.FindElement(By.Id("Input_Password")).Clear();
            driver.FindElement(By.Id("Input_Password")).SendKeys("User123!");
            driver.FindElement(By.Id("login-submit")).Click();
            driver.FindElement(By.XPath("//*[@id=\"dropdownMenuButton1\"]/img")).Click();
            driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[6]/form/div/select")).Click();
            new SelectElement(driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[6]/form/div/select"))).SelectByText("CreateCodeAndUseIt (Club Administrator)");
            driver.FindElement(By.LinkText("Clubs")).Click();
            driver.FindElement(By.LinkText("My Club")).Click();
            driver.FindElement(By.LinkText("Codes")).Click();
            driver.FindElement(By.XPath("//button[@onclick='openCreate()']")).Click();
            driver.FindElement(By.Id("RoleId")).Click();
            new SelectElement(driver.FindElement(By.Id("RoleId"))).SelectByText("Secretary");
            driver.FindElement(By.Id("ExpireDate")).Click();
            driver.FindElement(By.Id("ExpireDate")).Clear();
            driver.FindElement(By.Id("ExpireDate")).SendKeys("17-11-2023");
            driver.FindElement(By.XPath("//input[@value='Create']")).Click();
            Thread.Sleep(200);
            String CodeText = driver.FindElement(By.XPath("//*[@id=\"modal-inner-content\"]/div/div/dl/dd[1]")).Text;
            driver.FindElement(By.XPath("//span[@onclick=\"$('#modal').hide()\"]")).Click();
            driver.FindElement(By.XPath("//*[@id=\"dropdownMenuButton1\"]/img")).Click();
            driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[8]/form/button")).Click();
            driver.FindElement(By.LinkText("Sign in")).Click();
            driver.FindElement(By.Id("Input_Email")).Clear();
            driver.FindElement(By.Id("Input_Email")).SendKeys("admin@scmanagement.me");
            driver.FindElement(By.Id("Input_Password")).Clear();
            driver.FindElement(By.Id("Input_Password")).SendKeys("Admin123!");
            driver.FindElement(By.Id("login-submit")).Click();
            driver.FindElement(By.LinkText("Clubs")).Click();
            driver.FindElement(By.LinkText("Join with code")).Click();
            driver.FindElement(By.Id("Code")).Click();
            driver.FindElement(By.Id("Code")).Clear();
            driver.FindElement(By.Id("Code")).SendKeys(CodeText);
            driver.FindElement(By.XPath("//input[@value='Join']")).Click();
            driver.FindElement(By.LinkText("Staff")).Click();
            driver.FindElement(By.XPath("//*[@id=\"dropdownMenuButton1\"]/img")).Click();
            driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[6]/form/div/select")).Click();
            new SelectElement(driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[6]/form/div/select"))).SelectByText("CreateCodeAndUseIt (Secretary)");
            driver.FindElement(By.LinkText("Staff")).Click();
            String UserName = driver.FindElement(By.XPath("//table[@id='table_id']/tbody/tr/td")).Text;
            Assert.AreEqual("Admin User", driver.FindElement(By.XPath("//table[@id='table_id']/tbody/tr/td")).Text);
            driver.FindElement(By.XPath("//*[@id=\"dropdownMenuButton1\"]/img")).Click();
            driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[8]/form/button")).Click();
            driver.FindElement(By.LinkText("Sign in")).Click();
            driver.FindElement(By.Id("Input_Email")).Clear();
            driver.FindElement(By.Id("Input_Email")).SendKeys("user@scmanagement.me");
            driver.FindElement(By.Id("Input_Password")).Clear();
            driver.FindElement(By.Id("Input_Password")).SendKeys("User123!");
            driver.FindElement(By.Id("login-submit")).Click();
            driver.FindElement(By.XPath("//*[@id=\"dropdownMenuButton1\"]/img")).Click();
            driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[6]/form/div/select")).Click();
            new SelectElement(driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[6]/form/div/select"))).SelectByText("CreateCodeAndUseIt (Club Administrator)");
            driver.FindElement(By.LinkText("Clubs")).Click();
            driver.FindElement(By.LinkText("My Club")).Click();
            driver.FindElement(By.LinkText("Staff")).Click();
            driver.FindElement(By.XPath("//input[@value='Remove staff']")).Click();
        }

        [Test]
        public void TheCreateTeamTest()
        {
            driver.Navigate().GoToUrl("https://localhost:7111/");
            driver.Manage().Window.Maximize();
            driver.FindElement(By.LinkText("Sign in")).Click();
            driver.FindElement(By.Id("Input_Email")).Clear();
            driver.FindElement(By.Id("Input_Email")).SendKeys("user@scmanagement.me");
            driver.FindElement(By.Id("Input_Password")).Clear();
            driver.FindElement(By.Id("Input_Password")).SendKeys("User123!");
            driver.FindElement(By.Id("login-submit")).Click();
            driver.FindElement(By.XPath("//*[@id=\"dropdownMenuButton1\"]/img")).Click();
            driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[6]/form/div/select")).Click();
            new SelectElement(driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[6]/form/div/select"))).SelectByText("CreateTeam (Club Administrator)");
            driver.FindElement(By.LinkText("Clubs")).Click();
            driver.FindElement(By.LinkText("My Club")).Click();
            driver.FindElement(By.LinkText("Codes")).Click();
            driver.FindElement(By.XPath("//button[@onclick='openCreate()']")).Click();
            driver.FindElement(By.Id("ExpireDate")).Click();
            driver.FindElement(By.Id("ExpireDate")).Clear();
            driver.FindElement(By.Id("ExpireDate")).SendKeys("27-10-2023");
            driver.FindElement(By.Id("RoleId")).Click();
            new SelectElement(driver.FindElement(By.Id("RoleId"))).SelectByText("Athlete");
            driver.FindElement(By.XPath("//input[@value='Create']")).Click();
            String CodeToUse = driver.FindElement(By.XPath("//div[@id='modal-inner-content']/div/div/dl/dd")).Text;
            driver.FindElement(By.XPath("//span[@onclick=\"$('#modal').hide()\"]")).Click();
            driver.FindElement(By.XPath("//*[@id=\"dropdownMenuButton1\"]/img")).Click();
            driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[8]/form/button")).Click();
            driver.FindElement(By.LinkText("Sign in")).Click();
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
            driver.FindElement(By.Id("Code")).SendKeys(CodeToUse);
            driver.FindElement(By.XPath("//input[@value='Join']")).Click();
            driver.FindElement(By.LinkText("Athletes")).Click();
            driver.FindElement(By.XPath("//*[@id=\"dropdownMenuButton1\"]/img")).Click();
            driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[6]/form/div/select")).Click();
            new SelectElement(driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[6]/form/div/select"))).SelectByText("CreateTeam (Athlete)");
            driver.FindElement(By.XPath("//*[@id=\"dropdownMenuButton1\"]/img")).Click();
            driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[8]/form/button")).Click();
            driver.FindElement(By.LinkText("Sign in")).Click();
            driver.FindElement(By.Id("Input_Email")).Clear();
            driver.FindElement(By.Id("Input_Email")).SendKeys("user@scmanagement.me");
            driver.FindElement(By.Id("Input_Password")).Clear();
            driver.FindElement(By.Id("Input_Password")).SendKeys("User123!");
            driver.FindElement(By.Id("login-submit")).Click();
            driver.FindElement(By.LinkText("Clubs")).Click();
            driver.FindElement(By.LinkText("My Club")).Click();
            driver.FindElement(By.LinkText("Athletes")).Click();
            driver.FindElement(By.LinkText("Clubs")).Click();
            driver.FindElement(By.LinkText("My Club")).Click();
            driver.FindElement(By.LinkText("Teams")).Click();
            driver.FindElement(By.LinkText("Create Team")).Click();
            driver.FindElement(By.Id("Name")).Click();
            driver.FindElement(By.Id("Name")).Clear();
            driver.FindElement(By.Id("Name")).SendKeys("TeamTest");
            driver.FindElement(By.Id("TrainerId")).Click();
            driver.FindElement(By.Id("ModalityId")).Click();
            new SelectElement(driver.FindElement(By.Id("ModalityId"))).SelectByText("Hóquei em Patins");
            driver.FindElement(By.XPath("//input[@value='Create']")).Click();
            driver.FindElement(By.LinkText("Edit")).Click();
            driver.FindElement(By.Id("Name")).Click();
            driver.FindElement(By.Id("Name")).Clear();
            driver.FindElement(By.Id("Name")).SendKeys("TeamTestEdited");
            driver.FindElement(By.LinkText("Add Athlete")).Click();
            driver.FindElement(By.Name("selectedAthletes")).Click();
            driver.FindElement(By.XPath("//div[@id='modal-inner-content']/form/button")).Click();
            driver.FindElement(By.XPath("//input[@value='Save']")).Click();
            driver.FindElement(By.LinkText("Details")).Click();
            driver.FindElement(By.Id("btn-back")).Click();
            driver.FindElement(By.LinkText("Edit")).Click();
            driver.FindElement(By.XPath("//input[@value='Remove']")).Click();
            driver.FindElement(By.XPath("//input[@value='Save']")).Click();
            driver.FindElement(By.XPath("//table[@id='table_id']/tbody/tr/td[5]/form/button")).Click();
            driver.FindElement(By.LinkText("Clubs")).Click();
            driver.FindElement(By.LinkText("My Club")).Click();
            driver.FindElement(By.LinkText("Athletes")).Click();
            driver.FindElement(By.XPath("//input[@value='Remove athlete']")).Click();
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
