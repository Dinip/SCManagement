using OpenQA.Selenium;
using SCManagement.Tests.Browser.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCManagement.AutomationTests.Browser.Tests
{
    public class BrowserTest : BrowserTestsSetUp
    {
        [Test]
        public void Login_Success()
        {
            //Click on the login button
            _driver.FindElement(By.XPath("/html/body/header/nav/div/div/ul/a[2]")).Click();

            //Enter the login credentials
            _driver.FindElement(By.Id("Input_Email")).SendKeys("admin@scmanagement.me");
            _driver.FindElement(By.Id("Input_Password")).SendKeys("Admin123!");

            //Click on the log in  button
            _driver.FindElement(By.Id("login-submit")).Click();

            //Assert that the user is logged in
            Assert.That("https://localhost:7111/", Is.EqualTo(_driver.Url));
            _quit = true;
        }

        [Test]
        public void Login_Fail()
        {
            //Click on the login button
            _driver.FindElement(By.XPath("/html/body/header/nav/div/div/ul/a[2]")).Click();

            //Enter the login credentials
            _driver.FindElement(By.Id("Input_Email")).SendKeys("admin@asdfasdf.me");
            _driver.FindElement(By.Id("Input_Password")).SendKeys("Admin123!");

            //Click on the log in  button
            _driver.FindElement(By.Id("login-submit")).Click();

            //Assert that the user is logged in
            Assert.That("Invalid login attempt.", Is.EqualTo(_driver.FindElement(By.XPath("/html/body/section/div[2]/div/div/div[2]/div/form/div[1]/ul/li")).Text));
            _quit = true;
        }

        [Test]
        public void Perfil_Change_PhoneNumber_Success()
        {
            //Click on the login button
            _driver.FindElement(By.XPath("/html/body/header/nav/div/div/ul/a[2]")).Click();

            //Enter the login credentials
            _driver.FindElement(By.Id("Input_Email")).SendKeys("admin@scmanagement.me");
            _driver.FindElement(By.Id("Input_Password")).SendKeys("Admin123!");

            //Click on the log in  button
            _driver.FindElement(By.Id("login-submit")).Click();

            //Click on the profile button
            _driver.FindElement(By.XPath("/html/body/header/nav/div/div/ul/li/div/button")).Click();

            //Click on the edit profile button
            _driver.FindElement(By.XPath("/html/body/header/nav/div/div/ul/li/div/ul/li[1]/a")).Click();

            //Enter the new phone number
            _driver.FindElement(By.Id("Input_PhoneNumber")).Clear();
            _driver.FindElement(By.Id("Input_PhoneNumber")).SendKeys("123456789");

            //Click on the save button
            _driver.FindElement(By.Id("update-profile-button")).Click();

            //Assert that the user is logged in
            Assert.That("Your profile has been updated", Is.EqualTo(_driver.FindElement(By.XPath("/html/body/div/main/div/div[2]/div[2]/div/div")).Text));
            Assert.That("123456789", Is.EqualTo(_driver.FindElement(By.Id("Input_PhoneNumber")).GetAttribute("value")));
            _quit = true;
        }

        [Test]
        public void Perfil_Change_PhoneNumber_Fail()
        {
            //Click on the login button
            _driver.FindElement(By.XPath("/html/body/header/nav/div/div/ul/a[2]")).Click();

            //Enter the login credentials
            _driver.FindElement(By.Id("Input_Email")).SendKeys("admin@scmanagement.me");
            _driver.FindElement(By.Id("Input_Password")).SendKeys("Admin123!");

            //Click on the log in  button
            _driver.FindElement(By.Id("login-submit")).Click();

            //Click on the profile button
            _driver.FindElement(By.XPath("/html/body/header/nav/div/div/ul/li/div/button")).Click();

            //Click on the edit profile button
            _driver.FindElement(By.XPath("/html/body/header/nav/div/div/ul/li/div/ul/li[1]/a")).Click();

            //Enter the new phone number
            _driver.FindElement(By.Id("Input_PhoneNumber")).Clear();
            _driver.FindElement(By.Id("Input_PhoneNumber")).SendKeys("sdfghsdgfh");

            //Click on the save button
            _driver.FindElement(By.Id("update-profile-button")).Click();

            //Assert that the user is logged in
            Assert.That("The Phone number field is not a valid phone number.", Is.EqualTo(_driver.FindElement(By.XPath("/html/body/div/main/div/div[2]/div[2]/div/form/div[2]/span")).Text));
            Assert.That("sdfghsdgfh", Is.EqualTo(_driver.FindElement(By.Id("Input_PhoneNumber")).GetAttribute("value")));
            _quit = true;
        }

        
    }
}
