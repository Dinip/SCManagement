
using System;
using System.Numerics;
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
            driver.FindElement(By.Id("ExpireDate")).SendKeys(DateTime.Now.AddMonths(1).ToString("dd/MM/yyyy"));
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
            
            driver.FindElement(By.XPath("//*[@id=\"dropdownMenuButton1\"]/img")).Click();
            driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[7]/form/div/select")).Click();
            new SelectElement(driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[7]/form/div/select"))).SelectByText("PlansClub (Club Administrator)");

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
            #region Create Meal Plan
            driver.FindElement(By.LinkText("Trainer zone")).Click();
            driver.FindElement(By.LinkText("Training zone")).Click();
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Teams'])[1]/following::label[1]")).Click();
            driver.FindElement(By.XPath("//*[@id=\"athletesBody\"]/tr/td[5]/a[1]")).Click();
            Thread.Sleep(500);
            driver.FindElement(By.XPath("/html/body/div/main/div/div[2]/div/div[4]/div/div/div[2]/a[1]")).Click();
            driver.FindElement(By.Id("Name")).Click();
            driver.FindElement(By.Id("Name")).Clear();
            driver.FindElement(By.Id("Name")).SendKeys("Teste");
            driver.FindElement(By.Id("Description")).Click();
            driver.FindElement(By.Id("Description")).Clear();
            driver.FindElement(By.Id("Description")).SendKeys("Teste");
            driver.FindElement(By.Id("EndDate")).Click();
            driver.FindElement(By.Id("EndDate")).Clear();
            driver.FindElement(By.Id("EndDate")).SendKeys(DateTime.Now.AddDays(2).ToString("dd-MM-yyyy"));
            driver.FindElement(By.Id("MealPlanSessions_0__MealName")).Click();
            driver.FindElement(By.Id("MealPlanSessions_0__MealName")).Clear();
            driver.FindElement(By.Id("MealPlanSessions_0__MealName")).SendKeys("Dale");
            driver.FindElement(By.Id("MealPlanSessions_0__MealDescription")).Clear();
            driver.FindElement(By.Id("MealPlanSessions_0__MealDescription")).SendKeys("Dale");
            driver.FindElement(By.Id("MealPlanSessions_0__Time")).Clear();
            driver.FindElement(By.Id("MealPlanSessions_0__Time")).SendKeys("01:00");
            driver.FindElement(By.Name("action")).Click();
            driver.FindElement(By.Id("MealPlanSessions_1__MealName")).Click();
            driver.FindElement(By.Id("MealPlanSessions_1__MealName")).Clear();
            driver.FindElement(By.Id("MealPlanSessions_1__MealName")).SendKeys("Mim");

            driver.FindElement(By.XPath("//html/body/div/main/div/div/div/div/form/div[5]/div[2]/div/div[4]")).Click();
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Cancel'])[1]/following::button[1]")).Click();
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Teams'])[1]/following::label[1]")).Click();
            driver.FindElement(By.XPath("//tbody[@id='athletesBody']/tr/td[5]/a[2]")).Click();
            driver.FindElement(By.LinkText("Edit")).Click();
            driver.FindElement(By.Name("action")).Click();
            driver.FindElement(By.Id("MealPlanSessions_1__MealName")).Click();
            driver.FindElement(By.Id("MealPlanSessions_1__MealName")).Clear();
            driver.FindElement(By.Id("MealPlanSessions_1__MealName")).SendKeys("More");
            driver.FindElement(By.Id("MealPlanSessions_1__MealDescription")).Click();
            driver.FindElement(By.Id("MealPlanSessions_1__MealDescription")).Clear();
            driver.FindElement(By.Id("MealPlanSessions_1__MealDescription")).SendKeys("More");
            driver.FindElement(By.Id("MealPlanSessions_1__Time")).Click();
            driver.FindElement(By.Id("MealPlanSessions_1__Time")).Clear();
            driver.FindElement(By.Id("MealPlanSessions_1__Time")).SendKeys("01:00");
            driver.FindElement(By.Name("action")).Click();
            driver.FindElement(By.XPath("//html/body/div/main/div/div/div/div/form/div[5]/div[2]")).Click();
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Add Sessions'])[1]/following::button[1]")).Click();
            driver.FindElement(By.XPath("//html/body/div/main/div/div/div/div/form/div[5]/div[3]/div/div[4]")).Click();
            driver.FindElement(By.XPath("/html/body/div/main/div/div/div/div/form/div[6]/div[3]/button")).Click();
            #endregion

            //Create Training Plan
            #region Create Training Plan
            driver.FindElement(By.LinkText("Trainer zone")).Click();
            driver.FindElement(By.LinkText("Training zone")).Click();
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Teams'])[1]/following::label[1]")).Click();
            driver.FindElement(By.XPath("//*[@id=\"athletesBody\"]/tr/td[4]/a[1]")).Click();
            driver.FindElement(By.XPath("/html/body/div/main/div/div[2]/div/div[3]/div/div/div[2]/a[1]")).Click();
            driver.FindElement(By.Id("Name")).Click();
            driver.FindElement(By.Id("Name")).Clear();
            driver.FindElement(By.Id("Name")).SendKeys("Train");
            driver.FindElement(By.Id("Description")).Click();
            driver.FindElement(By.Id("Description")).Clear();
            driver.FindElement(By.Id("Description")).SendKeys("Train");
            driver.FindElement(By.Id("EndDate")).Click();
            driver.FindElement(By.Id("EndDate")).Clear();
            driver.FindElement(By.Id("EndDate")).SendKeys(DateTime.Now.AddDays(2).ToString("dd-MM-yyyy"));
            driver.FindElement(By.Id("TrainingPlanSessions_0__ExerciseName")).Click();
            driver.FindElement(By.Id("TrainingPlanSessions_0__ExerciseName")).Clear();
            driver.FindElement(By.Id("TrainingPlanSessions_0__ExerciseName")).SendKeys("Treino1");
            driver.FindElement(By.Id("TrainingPlanSessions_0__ExerciseDescription")).Click();
            driver.FindElement(By.Id("TrainingPlanSessions_0__ExerciseDescription")).Clear();
            driver.FindElement(By.Id("TrainingPlanSessions_0__ExerciseDescription")).SendKeys("Descirption");
            driver.FindElement(By.Id("TrainingPlanSessions_0__Repetitions")).Click();
            driver.FindElement(By.Id("TrainingPlanSessions_0__Repetitions")).Clear();
            driver.FindElement(By.Id("TrainingPlanSessions_0__Repetitions")).SendKeys("3");
            driver.FindElement(By.Id("TrainingPlanSessions_0__Duration")).Click();
            driver.FindElement(By.Id("TrainingPlanSessions_0__Duration")).Clear();
            driver.FindElement(By.Id("TrainingPlanSessions_0__Duration")).SendKeys("60");
            driver.FindElement(By.Name("action")).Click();
            driver.FindElement(By.Id("TrainingPlanSessions_1__ExerciseName")).Click();
            driver.FindElement(By.Id("TrainingPlanSessions_1__ExerciseName")).Clear();
            driver.FindElement(By.Id("TrainingPlanSessions_1__ExerciseName")).SendKeys("Teste");
            driver.FindElement(By.XPath("//html/body/div/main/div/div/div/div/form/div[6]/div[2]/div/div[5]/button")).Click();
            driver.FindElement(By.XPath("//form[@id='form']/div[7]/div[3]/button")).Click();
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Teams'])[1]/following::label[1]")).Click();
            driver.FindElement(By.XPath("//*[@id=\"athletesBody\"]/tr/td[4]/a[2]")).Click();
            driver.FindElement(By.LinkText("Edit")).Click();
            driver.FindElement(By.Name("action")).Click();
            driver.FindElement(By.Id("TrainingPlanSessions_1__ExerciseName")).Click();
            driver.FindElement(By.Id("TrainingPlanSessions_1__ExerciseName")).Clear();
            driver.FindElement(By.Id("TrainingPlanSessions_1__ExerciseName")).SendKeys("Mais 1");
            driver.FindElement(By.Id("TrainingPlanSessions_1__ExerciseDescription")).Click();
            driver.FindElement(By.Id("TrainingPlanSessions_1__ExerciseDescription")).Clear();
            driver.FindElement(By.Id("TrainingPlanSessions_1__ExerciseDescription")).SendKeys("Aguenta");
            driver.FindElement(By.Id("TrainingPlanSessions_1__Repetitions")).Click();
            driver.FindElement(By.Id("TrainingPlanSessions_1__Repetitions")).Clear();
            driver.FindElement(By.Id("TrainingPlanSessions_1__Repetitions")).SendKeys("2");
            driver.FindElement(By.Id("TrainingPlanSessions_1__Duration")).Click();
            driver.FindElement(By.Id("TrainingPlanSessions_1__Duration")).Clear();
            driver.FindElement(By.Id("TrainingPlanSessions_1__Duration")).SendKeys("80");
            driver.FindElement(By.Id("TrainingPlanSessions_0__ExerciseName")).Click();
            driver.FindElement(By.Id("TrainingPlanSessions_0__ExerciseName")).Clear();
            driver.FindElement(By.Id("TrainingPlanSessions_0__ExerciseName")).SendKeys("Treino12");
            driver.FindElement(By.Name("action")).Click();
            driver.FindElement(By.Id("TrainingPlanSessions_2__ExerciseName")).Click();
            driver.FindElement(By.Id("TrainingPlanSessions_2__ExerciseName")).Clear();
            driver.FindElement(By.Id("TrainingPlanSessions_2__ExerciseName")).SendKeys("asd");
            driver.FindElement(By.XPath("//html/body/div/main/div/div/div/div/form/div[6]/div[3]/div/div[5]/button")).Click();
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Add Sessions'])[1]/following::button[1]")).Click();
            #endregion

            //Create Goal
            #region Create Goal
            driver.FindElement(By.LinkText("Trainer zone")).Click();
            driver.FindElement(By.LinkText("Training zone")).Click();
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Teams'])[1]/following::label[1]")).Click();
            driver.FindElement(By.XPath("//*[@id=\"athletesBody\"]/tr/td[3]/a[1]")).Click();
            driver.FindElement(By.Id("Name")).Click();
            driver.FindElement(By.Id("Name")).Clear();
            driver.FindElement(By.Id("Name")).SendKeys("MyGoal");
            driver.FindElement(By.Id("Description")).Clear();
            driver.FindElement(By.Id("Description")).SendKeys("MyDescription");
            driver.FindElement(By.Id("EndDate")).Click();
            driver.FindElement(By.Id("EndDate")).Clear();
            driver.FindElement(By.Id("EndDate")).SendKeys(DateTime.Now.AddDays(1).ToString("dd/MM/yyyy"));
            driver.FindElement(By.XPath("//input[@value='Create']")).Click();
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Teams'])[1]/following::label[1]")).Click();
            driver.FindElement(By.LinkText("See")).Click();
            //Order By date
            driver.FindElement(By.XPath("/html/body/div/main/div/div/div/div/table/thead/tr/th[3]")).Click();
            driver.FindElement(By.LinkText("Edit")).Click();
            driver.FindElement(By.Id("EndDate")).Click();
            driver.FindElement(By.Id("EndDate")).Click();
            driver.FindElement(By.Id("EndDate")).Clear();
            driver.FindElement(By.Id("EndDate")).SendKeys("");
            driver.FindElement(By.Id("EndDate")).Clear();
            driver.FindElement(By.Id("EndDate")).SendKeys(DateTime.Now.AddMonths(1).ToString("dd/MM/yyyy"));
            driver.FindElement(By.XPath("/html/body/div/main/div/div/div/div/form/div[5]/div[2]/input")).Click();
            #endregion

            //MyZone
            #region MyZone
            driver.FindElement(By.XPath("//*[@id=\"dropdownMenuButton1\"]/img")).Click();
            driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[9]/form/button")).Click();
            driver.FindElement(By.Id("signin")).Click();
            driver.FindElement(By.Id("Input_Email")).Click();
            driver.FindElement(By.Id("Input_Email")).Clear();
            driver.FindElement(By.Id("Input_Email")).SendKeys("admin@scmanagement.me");
            driver.FindElement(By.Id("Input_Password")).Clear();
            driver.FindElement(By.Id("Input_Password")).SendKeys("Admin123!");
            driver.FindElement(By.Id("login-submit")).Click();
            driver.FindElement(By.XPath("//*[@id=\"dropdownMenuButton1\"]/img")).Click();
            driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[7]/form/div/select")).Click();
            new SelectElement(driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[7]/form/div/select"))).SelectByText("PlansClub (Athlete)");
            driver.FindElement(By.LinkText("MyZone")).Click();
            driver.FindElement(By.LinkText("Complete")).Click();
            //driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Complete'])[1]/following::label[1]")).Click();
            //driver.FindElement(By.XPath("//*[@id=\"table_id3\"]/tbody/tr/td[4]/a")).Click();
            driver.FindElement(By.LinkText("MyZone")).Click();

            //See training plan
            var button = driver.FindElement(By.XPath("//table[@id='table_id2']/tbody/tr/td[5]/a"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(false);", button);
            Thread.Sleep(1000);
            button.Click();

            driver.FindElement(By.LinkText("MyZone")).Click();

            button = driver.FindElement(By.LinkText("Insert Bioimpedance"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", button);
            Thread.Sleep(500);
            ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollBy(0,-200);");
            Thread.Sleep(500);
            button.Click();
            
            driver.FindElement(By.Id("Height")).Click();
            driver.FindElement(By.Id("Height")).Clear();
            driver.FindElement(By.Id("Height")).SendKeys("185cm");
            driver.FindElement(By.Id("fatMass")).Click();
            driver.FindElement(By.Id("fatMass")).Clear();
            driver.FindElement(By.Id("fatMass")).SendKeys("50");
            driver.FindElement(By.Id("leanMass")).Click();
            driver.FindElement(By.Id("leanMass")).Clear();
            driver.FindElement(By.Id("leanMass")).SendKeys("60");
            driver.FindElement(By.XPath("//*[@id=\"myForm\"]/div[9]/input")).Click();
            Assert.AreEqual("The sum of the values must be less than 100", CloseAlertAndGetItsText());
            driver.FindElement(By.Id("leanMass")).Click();
            driver.FindElement(By.Id("leanMass")).Clear();
            driver.FindElement(By.Id("leanMass")).SendKeys("20");
            driver.FindElement(By.XPath("//*[@id=\"myForm\"]/div[9]/input")).Click();


            button = driver.FindElement(By.LinkText("Insert Bioimpedance"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", button);
            Thread.Sleep(500);
            ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollBy(0,-200);");
            Thread.Sleep(500);
            button.Click();


            driver.FindElement(By.Id("muscleMass")).Click();
            driver.FindElement(By.Id("muscleMass")).Clear();
            driver.FindElement(By.Id("muscleMass")).SendKeys("12");
            driver.FindElement(By.Id("leanMass")).Click();
            driver.FindElement(By.Id("leanMass")).Clear();
            driver.FindElement(By.Id("leanMass")).SendKeys("20");
            driver.FindElement(By.XPath("//input[@value='Update']")).Click();

            #endregion

            //RemovePlans and athlete
            #region RemovePlans
            driver.FindElement(By.XPath("//*[@id=\"dropdownMenuButton1\"]/img")).Click();
            driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[9]/form/button")).Click();
            driver.FindElement(By.Id("signin")).Click();
            driver.FindElement(By.Id("Input_Email")).Clear();
            driver.FindElement(By.Id("Input_Email")).SendKeys("user@scmanagement.me");
            driver.FindElement(By.Id("Input_Password")).Clear();
            driver.FindElement(By.Id("Input_Password")).SendKeys("User123!");
            driver.FindElement(By.Id("login-submit")).Click();
            driver.FindElement(By.XPath("//*[@id=\"dropdownMenuButton1\"]/img")).Click();
            driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[7]/form/div/select")).Click();
            new SelectElement(driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[7]/form/div/select"))).SelectByText("PlansClub (Club Administrator)");

            //remove meal plan
            driver.FindElement(By.LinkText("Trainer zone")).Click();
            driver.FindElement(By.LinkText("Training zone")).Click();
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Teams'])[1]/following::label[1]")).Click();
            driver.FindElement(By.XPath("//tbody[@id='athletesBody']/tr/td[5]/a[2]")).Click();
            driver.FindElement(By.XPath("//table[@id='table_id']/tbody/tr/td[4]/div/form/button")).Click();
            //remove training plan
            driver.FindElement(By.LinkText("Trainer zone")).Click();
            driver.FindElement(By.LinkText("Training zone")).Click();
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Teams'])[1]/following::label[1]")).Click();
            driver.FindElement(By.XPath("//tbody[@id='athletesBody']/tr/td[4]/a[2]")).Click();
            driver.FindElement(By.XPath("//table[@id='table_id']/tbody/tr/td[5]/div/form/button")).Click();
            driver.FindElement(By.LinkText("Trainer zone")).Click();
            driver.FindElement(By.LinkText("Training zone")).Click();
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Teams'])[1]/following::label[1]")).Click();
            driver.FindElement(By.LinkText("See")).Click();

            //Remove athlete and Team
            driver.FindElement(By.LinkText("Clubs")).Click();
            driver.FindElement(By.LinkText("My Club")).Click();
            driver.FindElement(By.XPath("//div[10]/a/div/div/img")).Click();
            driver.FindElement(By.XPath("//table[@id='table_id']/tbody/tr/td[5]/div/form/button")).Click();
            driver.FindElement(By.LinkText("Clubs")).Click();
            driver.FindElement(By.LinkText("My Club")).Click();
            driver.FindElement(By.XPath("//div[9]/a/div/div/img")).Click();
            driver.FindElement(By.XPath("//input[@value='Remove']")).Click();
            #endregion

        }

        [Test]
        public void TheCreateTemplateTest()
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
            driver.FindElement(By.Id("ExpireDate")).SendKeys(DateTime.Now.AddMonths(1).ToString("dd/MM/yyyy"));
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
            driver.FindElement(By.XPath("//*[@id=\"dropdownMenuButton1\"]/img")).Click();
            driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[7]/form/div/select")).Click();
            new SelectElement(driver.FindElement(By.XPath("//ul[@id='Dropdown1']/li[7]/form/div/select"))).SelectByText("PlansClub (Club Administrator)");
            
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

            //Create Training Template
            #region Create Training Template
            driver.FindElement(By.LinkText("Trainer zone")).Click();
            driver.FindElement(By.LinkText("Templates")).Click();
            driver.FindElement(By.Id("createTraining")).Click();
            driver.FindElement(By.Id("Name")).Click();
            driver.FindElement(By.Id("Name")).Clear();
            driver.FindElement(By.Id("Name")).SendKeys("TreinaMais");
            driver.FindElement(By.Id("Description")).Clear();
            driver.FindElement(By.Id("Description")).SendKeys("MaisTreino");
            driver.FindElement(By.Id("TrainingPlanSessions_0__ExerciseName")).Click();
            driver.FindElement(By.Id("TrainingPlanSessions_0__ExerciseName")).Clear();
            driver.FindElement(By.Id("TrainingPlanSessions_0__ExerciseName")).SendKeys("asd");
            driver.FindElement(By.Id("TrainingPlanSessions_0__ExerciseDescription")).Click();
            driver.FindElement(By.Id("TrainingPlanSessions_0__ExerciseDescription")).Clear();
            driver.FindElement(By.Id("TrainingPlanSessions_0__ExerciseDescription")).SendKeys("asd");
            driver.FindElement(By.Id("TrainingPlanSessions_0__Repetitions")).Click();
            driver.FindElement(By.Id("TrainingPlanSessions_0__Repetitions")).Clear();
            driver.FindElement(By.Id("TrainingPlanSessions_0__Repetitions")).SendKeys("12");
            driver.FindElement(By.Id("TrainingPlanSessions_0__Duration")).Click();
            driver.FindElement(By.Id("TrainingPlanSessions_0__Duration")).Clear();
            driver.FindElement(By.Id("TrainingPlanSessions_0__Duration")).SendKeys("20");
            driver.FindElement(By.Name("action")).Click();
            driver.FindElement(By.Id("TrainingPlanSessions_1__ExerciseName")).Click();
            driver.FindElement(By.Id("TrainingPlanSessions_1__ExerciseName")).Clear();
            driver.FindElement(By.Id("TrainingPlanSessions_1__ExerciseName")).SendKeys("Simples");
            driver.FindElement(By.XPath("/html/body/div/main/div/div/div/div/form/div[4]/div[2]/div/div[5]/button")).Click();
            driver.FindElement(By.XPath("//*[@id=\"form\"]/div[5]/div[3]/button")).Click();
            driver.FindElement(By.LinkText("Edit")).Click();
            driver.FindElement(By.XPath("/html/body/div/main/div/div/div/div/form/div[5]/div[2]/button")).Click();
            driver.FindElement(By.Id("TrainingPlanSessions_1__ExerciseName")).Click();
            driver.FindElement(By.Id("TrainingPlanSessions_1__ExerciseName")).Clear();
            driver.FindElement(By.Id("TrainingPlanSessions_1__ExerciseName")).SendKeys("Vai");
            driver.FindElement(By.Id("TrainingPlanSessions_1__ExerciseDescription")).Click();
            driver.FindElement(By.Id("TrainingPlanSessions_1__ExerciseDescription")).Clear();
            driver.FindElement(By.Id("TrainingPlanSessions_1__ExerciseDescription")).SendKeys("Continua");
            driver.FindElement(By.Id("TrainingPlanSessions_1__Repetitions")).Click();
            driver.FindElement(By.Id("TrainingPlanSessions_1__Repetitions")).Clear();
            driver.FindElement(By.Id("TrainingPlanSessions_1__Repetitions")).SendKeys("2");
            driver.FindElement(By.Id("TrainingPlanSessions_1__Duration")).Click();
            driver.FindElement(By.Id("TrainingPlanSessions_1__Duration")).Clear();
            driver.FindElement(By.Id("TrainingPlanSessions_1__Duration")).SendKeys("10");
            driver.FindElement(By.Name("action")).Click();
            driver.FindElement(By.Id("TrainingPlanSessions_2__ExerciseName")).Click();
            driver.FindElement(By.Id("TrainingPlanSessions_2__ExerciseName")).Clear();
            driver.FindElement(By.Id("TrainingPlanSessions_2__ExerciseName")).SendKeys("Vais");
            driver.FindElement(By.XPath("/html/body/div/main/div/div/div/div/form/div[4]/div[3]/div/div[5]/button")).Click();
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Add Sessions'])[1]/following::button[1]")).Click();
            #endregion

            //Create Meal Template
            #region Create Meal Template
            driver.FindElement(By.LinkText("Trainer zone")).Click();
            driver.FindElement(By.LinkText("Templates")).Click();
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Training'])[1]/following::label[1]")).Click();
            driver.FindElement(By.Id("createMeal")).Click();
            driver.FindElement(By.Id("Name")).Click();
            driver.FindElement(By.Id("Name")).Clear();
            driver.FindElement(By.Id("Name")).SendKeys("Template");
            driver.FindElement(By.Id("Description")).Click();
            driver.FindElement(By.Id("Description")).Clear();
            driver.FindElement(By.Id("Description")).SendKeys("dale");
            driver.FindElement(By.Id("MealPlanSessions_0__MealName")).Click();
            driver.FindElement(By.Id("MealPlanSessions_0__MealName")).Clear();
            driver.FindElement(By.Id("MealPlanSessions_0__MealName")).SendKeys("Meal1");
            driver.FindElement(By.Id("MealPlanSessions_0__MealDescription")).Click();
            driver.FindElement(By.Id("MealPlanSessions_0__MealDescription")).Clear();
            driver.FindElement(By.Id("MealPlanSessions_0__MealDescription")).SendKeys("Descirptis");
            driver.FindElement(By.Id("MealPlanSessions_0__Time")).Click();
            driver.FindElement(By.Id("MealPlanSessions_0__Time")).Clear();
            driver.FindElement(By.Id("MealPlanSessions_0__Time")).SendKeys("01:00");
            driver.FindElement(By.Name("action")).Click();
            driver.FindElement(By.Id("MealPlanSessions_1__MealName")).Click();
            driver.FindElement(By.Id("MealPlanSessions_1__MealName")).Clear();
            driver.FindElement(By.Id("MealPlanSessions_1__MealName")).SendKeys("Teste");
            driver.FindElement(By.XPath("/html/body/div/main/div/div/div/div/form/div[3]/div[2]/div/div[4]/button")).Click();
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Cancel'])[1]/following::button[1]")).Click();
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Training'])[1]/following::label[1]")).Click();
            driver.FindElement(By.LinkText("Edit")).Click();
            driver.FindElement(By.Name("action")).Click();
            driver.FindElement(By.Id("MealPlanSessions_1__MealName")).Click();
            driver.FindElement(By.Id("MealPlanSessions_1__MealName")).Clear();
            driver.FindElement(By.Id("MealPlanSessions_1__MealName")).SendKeys("Meal2");
            driver.FindElement(By.Id("MealPlanSessions_1__MealDescription")).Clear();
            driver.FindElement(By.Id("MealPlanSessions_1__MealDescription")).SendKeys("Description2");
            driver.FindElement(By.Id("MealPlanSessions_1__Time")).Click();
            driver.FindElement(By.Id("MealPlanSessions_1__Time")).Clear();
            driver.FindElement(By.Id("MealPlanSessions_1__Time")).SendKeys("01:00");
            driver.FindElement(By.Name("action")).Click();
            driver.FindElement(By.Id("MealPlanSessions_2__MealName")).Click();
            driver.FindElement(By.Id("MealPlanSessions_2__MealName")).Clear();
            driver.FindElement(By.Id("MealPlanSessions_2__MealName")).SendKeys("Teste");
            driver.FindElement(By.XPath("/html/body/div/main/div/div/div/div/form/div[3]/div[3]/div/div[4]/button")).Click();
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Add Sessions'])[1]/following::button[1]")).Click();
#endregion
            
            //Use Templates
            #region ApplyTemplate
            driver.FindElement(By.LinkText("Trainer zone")).Click();
            driver.FindElement(By.LinkText("Training zone")).Click();
            driver.FindElement(By.LinkText("Add")).Click();
            driver.FindElement(By.XPath("//div[@id='modal-inner-content']/div[2]/a[2]/strong")).Click();
            driver.FindElement(By.LinkText("Apply Template")).Click();
            driver.FindElement(By.Id("EndDate")).Click();
            driver.FindElement(By.Id("EndDate")).Clear();
            driver.FindElement(By.Id("EndDate")).SendKeys(DateTime.Now.AddDays(10).ToString("dd/MM/yyyy"));
            driver.FindElement(By.XPath("//form[@id='form']/div[4]/div[3]/button")).Click();
            driver.FindElement(By.XPath("//*[@id=\"teamsBody\"]/tr[1]/td[4]/a")).Click();
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='New Plan'])[2]/following::strong[1]")).Click();
            driver.FindElement(By.LinkText("Apply Template")).Click();
            driver.FindElement(By.Id("EndDate")).Click();
            driver.FindElement(By.Id("EndDate")).Clear();
            driver.FindElement(By.Id("EndDate")).SendKeys(DateTime.Now.AddDays(10).ToString("dd/MM/yyyy"));
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Cancel'])[1]/following::button[1]")).Click();
            #endregion

            //Delete Templates
            #region DeleteTemplate
            driver.FindElement(By.LinkText("Trainer zone")).Click();
            driver.FindElement(By.LinkText("Templates")).Click();
            driver.FindElement(By.XPath("//tbody[@id='trainingBody']/tr/td[4]/div/form/button")).Click();
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Training'])[1]/following::label[1]")).Click();
            driver.FindElement(By.XPath("//tbody[@id='mealBody']/tr/td[3]/div/form/button")).Click();
            #endregion

            //RemovePlans and athlete
            #region RemovePlans
            driver.FindElement(By.LinkText("Trainer zone")).Click();
            driver.FindElement(By.LinkText("Training zone")).Click();
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Teams'])[1]/following::label[1]")).Click();
            driver.FindElement(By.XPath("//tbody[@id='athletesBody']/tr/td[4]/a[2]")).Click();
            driver.FindElement(By.XPath("//table[@id='table_id']/tbody/tr/td[5]/div/form/button")).Click();
            driver.FindElement(By.LinkText("Trainer zone")).Click();
            driver.FindElement(By.LinkText("Training zone")).Click();
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Teams'])[1]/following::label[1]")).Click();
            driver.FindElement(By.XPath("//*[@id=\"athletesBody\"]/tr/td[5]/a[2]")).Click();
            driver.FindElement(By.XPath("//*[@id=\"table_id\"]/tbody/tr/td[4]/div/form/button")).Click();

            driver.FindElement(By.LinkText("Clubs")).Click();
            driver.FindElement(By.LinkText("My Club")).Click();
            driver.FindElement(By.XPath("//div[10]/a/div/div/img")).Click();
            driver.FindElement(By.XPath("//table[@id='table_id']/tbody/tr/td[5]/div/form/button")).Click();
            driver.FindElement(By.LinkText("Clubs")).Click();
            driver.FindElement(By.LinkText("My Club")).Click();
            driver.FindElement(By.XPath("//div[9]/a/div/div/img")).Click();
            driver.FindElement(By.XPath("//input[@value='Remove']")).Click();
            #endregion

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

