using OpenQA.Selenium;

namespace SCManagement.Tests.Browser.Core
{
    public class GlobalVariables
    {
        // Declare the driver
        public IWebDriver _driver;
        
        // Declare the quit variable
        public bool _quit;

        // Declare the headless mode
        public bool headlessTest = true;
    }
}
