using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Text;

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
        public void ChangeEventWithoutRouteToHaveRoute()
        {

            driver.Navigate().GoToUrl("https://localhost:7111/");
            driver.Manage().Window.Maximize();
            driver.FindElement(By.LinkText("Sign in")).Click();
            driver.FindElement(By.Id("Input_Email")).Clear();
            driver.FindElement(By.Id("Input_Email")).SendKeys("user@scmanagement.me");
            driver.FindElement(By.Id("Input_Password")).Clear();
            driver.FindElement(By.Id("Input_Password")).SendKeys("User123!");
            driver.FindElement(By.Id("login-submit")).Click();
            //Choose Club
            driver.FindElement(By.XPath("//*[@id=\"dropdownMenuButton1\"]/img")).Click();
            driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[7]/form/div/select")).Click();
            new SelectElement(driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[7]/form/div/select"))).SelectByText("CreateEvent (Club Administrator)");

            //Create Event
            driver.FindElement(By.LinkText("Events")).Click();
            driver.FindElement(By.LinkText("Create New")).Click();
            driver.FindElement(By.Id("EventTranslationsName_0__Value")).Click();
            driver.FindElement(By.Id("EventTranslationsName_0__Value")).Clear();
            driver.FindElement(By.Id("EventTranslationsName_0__Value")).SendKeys("0 - Simple Event");
            driver.FindElement(By.Id("ckbIsPublic")).Click();
            driver.FindElement(By.Id("startDateTime")).SendKeys(DateTime.Now.AddDays(5).ToString("dd/MM/yyyy"));
            driver.FindElement(By.Id("endDateTime")).SendKeys(DateTime.Now.AddDays(6).ToString("dd/MM/yyyy"));
            driver.FindElement(By.Id("EnrollLimitDate")).SendKeys(DateTime.Now.AddDays(3).ToString("dd/MM/yyyy"));
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Logout'])[2]/following::main[1]")).Click();
            driver.FindElement(By.Id("txtMaxEventEnrolls")).Clear();
            driver.FindElement(By.Id("txtMaxEventEnrolls")).SendKeys("15");
            driver.FindElement(By.Id("selectEventResultType")).Click();
            new SelectElement(driver.FindElement(By.Id("selectEventResultType"))).SelectByText("Position");
            //Slide to the element
            if (driver.FindElement(By.XPath("//input[@type='text']")).Displayed)
            {
                var button = driver.FindElement(By.XPath("//input[@type='text']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", button);
                Thread.Sleep(500);
                ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollBy(0,-200);");
                Thread.Sleep(500);
                button.Click();
            }
            driver.FindElement(By.XPath("//input[@type='text']")).Clear();
            driver.FindElement(By.XPath("//input[@type='text']")).SendKeys("mar");
            driver.FindElement(By.XPath("//input[@type='text']")).Clear();
            driver.FindElement(By.XPath("//input[@type='text']")).SendKeys("Rua Maria Adelaide Rosado Pinto, 2900-693 Setúbal, Setúbal, Portugal");
            Thread.Sleep(500);
            driver.FindElement(By.XPath("//*[@id=\"map\"]/div[3]/div[1]/div/div[1]/ul/li[1]/a/div/div[2]")).Click();
            IWebElement canvas = driver.FindElement(By.XPath("//div[@id='map']/div[2]/canvas"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", canvas);
            Thread.Sleep(500);
            Actions builder = new Actions(driver);
            builder.MoveToElement(canvas, 100, 100).Click().Build().Perform();
            Thread.Sleep(500);
            driver.FindElement(By.XPath("//button[@id='save-button']/i")).Click();

            driver.FindElement(By.XPath("//input[@value='Create']")).Click();


            //Edit Event
            driver.FindElement(By.LinkText("Events")).Click();
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Futures'])[1]/following::label[1]")).Click();
            driver.FindElement(By.LinkText("Details")).Click();
            driver.FindElement(By.LinkText("Edit")).Click();
            
            if (driver.FindElement(By.Id("ckbHaveRoute")).Displayed)
            {
                var button = driver.FindElement(By.Id("ckbHaveRoute"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", button);
                Thread.Sleep(500);
                button.Click();
                Thread.Sleep(500);
            }

            //Insert Route
            canvas = driver.FindElement(By.XPath("//div[@id='map']/div[2]/canvas"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", canvas);
            Thread.Sleep(500);
            builder = new Actions(driver);
            builder.MoveToElement(canvas, -200, 100).Click().Build().Perform();
            builder.MoveToElement(canvas, 100, 100).Click().Build().Perform();
            builder.MoveToElement(canvas, 200, 100).Click().Build().Perform();
            builder.MoveToElement(canvas, 200, 100).Click().Build().Perform();
            driver.FindElement(By.XPath("//button[@id='save-button']/i")).Click();
            //Slide to Save Element
            if (driver.FindElement(By.XPath("//input[@value='Save']")).Displayed)
            {
                var button = driver.FindElement(By.XPath("//input[@value='Save']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", button);
                Thread.Sleep(200);
                button.Click();
            }

            //Delete Event
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Futures'])[1]/following::label[1]")).Click();
            driver.FindElement(By.LinkText("Details")).Click();
            driver.FindElement(By.XPath("/html/body/div/main/div[2]/div[1]/div[2]/button")).Click();
            driver.FindElement(By.XPath("//*[@id=\"modal1\"]/div/div/form/input[2]")).Click();



        }

        [Test]
        public void ChangeEventWithRouteToRemoveRoute()
        {
            driver.Navigate().GoToUrl("https://localhost:7111/");
            driver.Manage().Window.Maximize();
            driver.FindElement(By.LinkText("Sign in")).Click();
            driver.FindElement(By.Id("Input_Email")).Clear();
            driver.FindElement(By.Id("Input_Email")).SendKeys("user@scmanagement.me");
            driver.FindElement(By.Id("Input_Password")).Clear();
            driver.FindElement(By.Id("Input_Password")).SendKeys("User123!");
            driver.FindElement(By.Id("login-submit")).Click();
            //Choose Club
            driver.FindElement(By.XPath("//*[@id=\"dropdownMenuButton1\"]/img")).Click();
            driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[7]/form/div/select")).Click();
            new SelectElement(driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[7]/form/div/select"))).SelectByText("CreateEvent (Club Administrator)");

            //Create Event
            driver.FindElement(By.LinkText("Events")).Click();
            driver.FindElement(By.LinkText("Create New")).Click();
            driver.FindElement(By.Id("EventTranslationsName_0__Value")).Click();
            driver.FindElement(By.Id("EventTranslationsName_0__Value")).Clear();
            driver.FindElement(By.Id("EventTranslationsName_0__Value")).SendKeys("0 - Route Event");
            driver.FindElement(By.Id("ckbIsPublic")).Click();
            driver.FindElement(By.Id("startDateTime")).SendKeys(DateTime.Now.AddDays(5).ToString("dd/MM/yyyy"));
            driver.FindElement(By.Id("endDateTime")).SendKeys(DateTime.Now.AddDays(6).ToString("dd/MM/yyyy"));
            driver.FindElement(By.Id("EnrollLimitDate")).SendKeys(DateTime.Now.AddDays(3).ToString("dd/MM/yyyy"));
            if (driver.FindElement(By.Id("ckbHaveRoute")).Displayed)
            {
                var button = driver.FindElement(By.Id("ckbHaveRoute"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", button);
                ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollBy(0,100);");
                Thread.Sleep(500);
                button.Click();
                Thread.Sleep(500);
            }
            //Insert Route
            IWebElement canvas = driver.FindElement(By.XPath("//div[@id='map']/div[2]/canvas"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", canvas);
            Thread.Sleep(500);
            Actions builder = new Actions(driver);
            builder.MoveToElement(canvas, -200, 100).Click().Build().Perform();
            builder.MoveToElement(canvas, 100, 100).Click().Build().Perform();
            builder.MoveToElement(canvas, 200, 100).Click().Build().Perform();
            builder.MoveToElement(canvas, 200, 100).Click().Build().Perform();
            driver.FindElement(By.XPath("//button[@id='save-button']/i")).Click();
            driver.FindElement(By.XPath("//input[@value='Create']")).Click();


            //Edit Event
            driver.FindElement(By.LinkText("Events")).Click();
            driver.FindElement(By.LinkText("Details")).Click();
            Thread.Sleep(1000);
            driver.FindElement(By.LinkText("Edit")).Click();
            //Disable have route checkbox
            if (driver.FindElement(By.Id("ckbHaveRoute")).Displayed)
            {
                var button = driver.FindElement(By.Id("ckbHaveRoute"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(false);", button);
                Thread.Sleep(500);
                button.Click();
                Thread.Sleep(500);
            }

            driver.FindElement(By.Id("btnAddLocation")).Click();

            //Insert Location
            //Slide to the element
            if (driver.FindElement(By.XPath("//div[@id='map']/div[3]/div/div/input")).Displayed)
            {
                var button = driver.FindElement(By.XPath("//div[@id='map']/div[3]/div/div/input"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", button);
                Thread.Sleep(500);
                ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollBy(0,-200);");
                Thread.Sleep(500);
                button.Click();
            }


            driver.FindElement(By.XPath("//div[@id='map']/div[3]/div/div/input")).Clear();
            driver.FindElement(By.XPath("//div[@id='map']/div[3]/div/div/input")).SendKeys("Mar");
            driver.FindElement(By.XPath("//div[@id='map']/div[3]/div/div/input")).Clear();
            driver.FindElement(By.XPath("//div[@id='map']/div[3]/div/div/input")).SendKeys("Rua Maria Adelaide Rosado Pinto, 2900-693 Setúbal, Setúbal, Portugal");
            Thread.Sleep(500);
            driver.FindElement(By.XPath("//*[@id='map']/div[3]/div[1]/div/div[1]/ul/li[1]/a/div/div[1]")).Click();
            
            canvas = driver.FindElement(By.XPath("//div[@id='map']/div[2]/canvas"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", canvas);
            Thread.Sleep(500);
            builder = new Actions(driver);
            builder.MoveToElement(canvas, 100, 100).Click().Build().Perform();
            Thread.Sleep(500);


            driver.FindElement(By.XPath("//button[@id='save-button']/i")).Click();
            //Slide to Save Element
            if (driver.FindElement(By.XPath("//input[@value='Save']")).Displayed)
            {
                var button = driver.FindElement(By.XPath("//input[@value='Save']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", button);
                Thread.Sleep(500);
                button.Click();
            }

            Thread.Sleep(200);
            
            //Delete Event
            driver.FindElement(By.LinkText("Details")).Click();
            driver.FindElement(By.XPath("/html/body/div/main/div[2]/div[1]/div[2]/button")).Click();
            driver.FindElement(By.XPath("//*[@id=\"modal1\"]/div/div/form/input[2]")).Click();

        }


        [Test]
        public void TheEventEnrollAndResultTest()
        {
            driver.Navigate().GoToUrl("https://localhost:7111/");
            driver.Manage().Window.Maximize();
            driver.FindElement(By.LinkText("Sign in")).Click();
            driver.FindElement(By.Id("Input_Email")).Clear();
            driver.FindElement(By.Id("Input_Email")).SendKeys("user@scmanagement.me");
            driver.FindElement(By.Id("Input_Password")).Clear();
            driver.FindElement(By.Id("Input_Password")).SendKeys("User123!");
            driver.FindElement(By.Id("login-submit")).Click();
            //Choose Club
            driver.FindElement(By.XPath("//*[@id=\"dropdownMenuButton1\"]/img")).Click();
            driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[7]/form/div/select")).Click();
            new SelectElement(driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[7]/form/div/select"))).SelectByText("CreateEvent (Club Administrator)");
            
            driver.FindElement(By.LinkText("Events")).Click();
            driver.FindElement(By.LinkText("Create New")).Click();
            driver.FindElement(By.Id("EventTranslationsName_0__Value")).Click();
            driver.FindElement(By.Id("EventTranslationsName_0__Value")).Click();
            driver.FindElement(By.Id("EventTranslationsName_0__Value")).Clear();
            driver.FindElement(By.Id("EventTranslationsName_0__Value")).SendKeys("0 - Enroll and Result");
            driver.FindElement(By.Id("ckbIsPublic")).Click();
            driver.FindElement(By.Id("startDateTime")).SendKeys(DateTime.Now.AddDays(5).ToString("dd/MM/yyyy"));
            driver.FindElement(By.Id("endDateTime")).SendKeys(DateTime.Now.AddDays(6).ToString("dd/MM/yyyy"));
            driver.FindElement(By.Id("EnrollLimitDate")).SendKeys(DateTime.Now.AddDays(3).ToString("dd/MM/yyyy"));

            //Slide to the element
            if (driver.FindElement(By.XPath("//input[@type='text']")).Displayed)
            {
                var button = driver.FindElement(By.XPath("//input[@type='text']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", button);
                Thread.Sleep(500);
                ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollBy(0,-200);");
                Thread.Sleep(500);
                button.Click();
            }
            driver.FindElement(By.XPath("//input[@type='text']")).Clear();
            driver.FindElement(By.XPath("//input[@type='text']")).SendKeys("mar");
            driver.FindElement(By.XPath("//input[@type='text']")).Clear();
            driver.FindElement(By.XPath("//input[@type='text']")).SendKeys("Rua Maria Adelaide Rosado Pinto, 2900-693 Setúbal, Setúbal, Portugal");
            Thread.Sleep(500);
            driver.FindElement(By.XPath("//*[@id=\"map\"]/div[3]/div[1]/div/div[1]/ul/li[1]/a/div/div[2]")).Click();

            IWebElement canvas = driver.FindElement(By.XPath("//div[@id='map']/div[2]/canvas"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", canvas);
            Thread.Sleep(500);
            Actions builder = new Actions(driver);
            builder.MoveToElement(canvas, 100, 100).Click().Build().Perform();
            Thread.Sleep(500);

            driver.FindElement(By.XPath("//button[@id='save-button']/i")).Click();
            driver.FindElement(By.XPath("//input[@value='Create']")).Click();

            Thread.Sleep(200);
            driver.FindElement(By.XPath("//*[@id=\"dropdownMenuButton1\"]/img")).Click();
            driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[9]/form/button")).Click();
            driver.FindElement(By.LinkText("Sign in")).Click();
            driver.FindElement(By.Id("Input_Email")).Clear();
            driver.FindElement(By.Id("Input_Email")).SendKeys("admin@scmanagement.me");
            driver.FindElement(By.Id("Input_Password")).Clear();
            driver.FindElement(By.Id("Input_Password")).SendKeys("Admin123!");
            driver.FindElement(By.Id("login-submit")).Click();
            
            driver.FindElement(By.LinkText("Events")).Click();
            driver.FindElement(By.LinkText("Details")).Click();
            driver.FindElement(By.XPath("/html/body/div/main/div[2]/div/div[1]/form/button")).Click();
            driver.FindElement(By.Id("dropdownMenuButton1")).Click();
            driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[9]/form/button")).Click();
            driver.FindElement(By.LinkText("Sign in")).Click();
            driver.FindElement(By.Id("Input_Email")).Click();
            driver.FindElement(By.Id("Input_Email")).Clear();
            driver.FindElement(By.Id("Input_Email")).SendKeys("user@scmanagement.me");
            driver.FindElement(By.Id("Input_Password")).Clear();
            driver.FindElement(By.Id("Input_Password")).SendKeys("User123!");
            driver.FindElement(By.Id("login-submit")).Click();
            //Choose Club
            driver.FindElement(By.XPath("//*[@id=\"dropdownMenuButton1\"]/img")).Click();
            driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[7]/form/div/select")).Click();
            new SelectElement(driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[7]/form/div/select"))).SelectByText("CreateEvent (Club Administrator)");
            
            driver.FindElement(By.LinkText("Events")).Click();
            driver.FindElement(By.LinkText("Details")).Click();
            driver.FindElement(By.XPath("/html/body/div/main/div[2]/div[1]/div[5]/form/button")).Click();
            driver.FindElement(By.LinkText("Add Result")).Click();
            driver.FindElement(By.Id("UserId")).Click();
            new SelectElement(driver.FindElement(By.Id("UserId"))).SelectByText("Admin User");
            driver.FindElement(By.Id("Result")).Click();
            driver.FindElement(By.Id("Result")).Clear();
            driver.FindElement(By.Id("Result")).SendKeys("5");
            driver.FindElement(By.Id("btnAdd")).Click();
            driver.FindElement(By.LinkText("Events")).Click();
            driver.FindElement(By.LinkText("Details")).Click();
            driver.FindElement(By.XPath("/html/body/div/main/div[2]/div[1]/div[2]/button")).Click();
            driver.FindElement(By.XPath("//*[@id=\"modal1\"]/div/div/form/input[2]")).Click();
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
