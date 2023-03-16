using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SCManagement.Tests.Browser.Core
{
    public class BrowserTestsSetUp : DSL
    {
        private void OpenChrome()
        {
            var headlessMode = new ChromeOptions();
            headlessMode.AddArgument("window-size=1366x768");
            headlessMode.AddArgument("disk-cache-size=0");
            headlessMode.AddArgument("headless");

            var devMode = new ChromeOptions();
            devMode.AddArgument("start-maximized");
            devMode.AddArgument("disk-cache-size=0");

            if (headlessTest) _driver = new ChromeDriver(headlessMode);
            else _driver = new ChromeDriver(devMode);

            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        [SetUp]
        public void Setup()
        {
            OpenChrome();
            _driver.Navigate().GoToUrl("https://localhost:7111/");
            _quit = false;
        }

        [TearDown]
        public void End()
        {
            if (_quit) _driver.Quit();
        }

    }
}
