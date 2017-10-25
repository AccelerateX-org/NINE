using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStick.TimeTable.UaTest.Base;
using NUnit.Framework;
using OpenQA.Selenium;

namespace MyStick.TimeTable.UaTest
{
    [TestFixture("Chrome", "61.0", "Windows 10", "1280x1024", "", "")]
    //[TestFixture("Firefox", "55.0", "Windows 10", "1280x1024", "", "")]
    public class UaTest : UaTestBaseClass
    {
        public UaTest(string browser, string version, string os, string screenResultion, string deviceName, string deviceOrientation)
            : base(browser, version, os, screenResultion, deviceName, deviceOrientation, applicationName: "MyStick.TimeTable.Web") { }

        [Test]
        public void DislcaimerExists()
        {
            Driver.Navigate().GoToUrl(BaseUrl);
            Assert.AreEqual("Impressum", Driver.FindElement(By.XPath("//a[contains(text(),'Impressum')]")).Text);
        }

        [Test]
        public void B_LoginUserTest()
        {
            Driver.Navigate().GoToUrl(BaseUrl);
            Assert.AreEqual("Herzlich Willkommen bei NINE", Driver.FindElement(By.XPath("//h2")).Text);
        }
    }
}
