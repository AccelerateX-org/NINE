using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace MyStick.TimeTable.UaTest.Base
{
    public abstract class UaTestBaseClass
    {
        private IWebDriver _driver;

        public IWebDriver Driver { get; private set; }

        public string BaseUrl { get; private set; }

        private readonly bool _isLocalMode;

        private readonly string _sauceLabsUserName;
        private readonly string _sauceLabsAccessKey;
        private readonly string _build;

        private readonly string _browser;
        private readonly string _version;
        private readonly string _os;
        private readonly string _deviceName;
        private readonly string _deviceOrientation;
        private readonly string _screenResultion;
        private readonly string _applicationName;

        const int iisPort = 2020;
        private Process _iisProcess;

        protected UaTestBaseClass(string browser, string version, string os, string screenResultion, string deviceName, string deviceOrientation, string applicationName)
        {
            _browser = browser;
            _version = version;
            _os = os;
            _screenResultion = screenResultion;
            _deviceName = deviceName;
            _deviceOrientation = deviceOrientation;
            _applicationName = applicationName;

            try
            {
                _sauceLabsUserName = Environment.GetEnvironmentVariable("SL_USERNAME");
                _sauceLabsAccessKey = Environment.GetEnvironmentVariable("SL_API_KEY");
                _build = Environment.GetEnvironmentVariable("SL_BUILD");

                if (string.IsNullOrEmpty(_sauceLabsUserName) || string.IsNullOrEmpty(_sauceLabsAccessKey))
                {
                    _isLocalMode = true;
                }

                if (!_isLocalMode)
                {
                    BaseUrl = Environment.GetEnvironmentVariable("SL_BASE_URL");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(@"Exception while obtaining SauceLabs credentials. Error while obtaining Environment Variables..." + e);
            }
        }

        [SetUp]
        public void Init()
        {
            var caps = new DesiredCapabilities();
            caps.SetCapability(CapabilityType.BrowserName, _browser);
            caps.SetCapability(CapabilityType.Version, _version);
            caps.SetCapability(CapabilityType.Platform, _os);
            caps.SetCapability("screenResolution", _screenResultion);
            caps.SetCapability("deviceName", _deviceName);
            caps.SetCapability("deviceOrientation", _deviceOrientation);
            caps.SetCapability("username", _sauceLabsUserName);
            caps.SetCapability("accessKey", _sauceLabsAccessKey);
            caps.SetCapability("name", TestContext.CurrentContext.Test.Name);

            if (_isLocalMode)
            {
                throw new NotImplementedException("Local UAT is not supported yet!");
            }

            try
            {
                _driver = !_isLocalMode ? new RemoteWebDriver(new Uri("http://ondemand.saucelabs.com:80/wd/hub"), caps, TimeSpan.FromSeconds(600)) : new ChromeDriver();
            }
            catch (Exception e)
            {
                Console.WriteLine(@"Exception while starting WebDriver..." + e);
            }

            Driver = _driver;
        }

        [TearDown]
        public void CleanUp()
        {
            if (_isLocalMode)
            {
                _driver.Quit();
                if (_iisProcess.HasExited == false)
                {
                    _iisProcess.Kill();
                }
                return;
            }

            var passed = TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed;

            try
            {
                ((IJavaScriptExecutor)_driver).ExecuteScript("sauce:job-result=" + (passed ? "passed" : "failed"));
                ((IJavaScriptExecutor)_driver).ExecuteScript("sauce:job-build=" + _build);
            }
            finally
            {
                _driver.Quit();
            }
        }

        private void StartIIS()
        {
            var applicationPath = GetApplicationPath(_applicationName);
            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            _iisProcess = new Process();
            _iisProcess.StartInfo.FileName = programFiles + @"\IIS Express\iisexpress.exe";
            _iisProcess.StartInfo.Arguments = string.Format("path:\"{0}\" /port:{1}", applicationPath, iisPort);
            _iisProcess.Start();
        }

        protected virtual string GetApplicationPath(string applicationName)
        {
            var solutionFolder = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)));
            return Path.Combine(solutionFolder ?? throw new InvalidOperationException(), applicationName);
        }

        public string GetAbsoluteUrl(string relativeUrl)
        {
            if (!relativeUrl.StartsWith("/"))
            {
                relativeUrl = "/" + relativeUrl;
            }
            return string.Format("http://localhost:{0}{1}", iisPort, relativeUrl);
        }
    }
}
