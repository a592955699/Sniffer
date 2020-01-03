using OpenQA.Selenium.Remote;
using Sniffer.Login;
using Sniffer.Login.SiteLogins;
using Sniffer.Login.VerificationCodes.Slide;
using System;
using System.Threading;

namespace Sniffer.Tyc.Logins
{
    public class TianyanchaLogin : AbstractLogin
    {
        private GeetestSlideVerificationCode _geetestSlideVerificationCode = new GeetestSlideVerificationCode();
        public TianyanchaLogin(RemoteWebDriver remoteWebDriver) : base(remoteWebDriver)
        {
            CookieFilePath = AppContext.BaseDirectory + "tianyangcha\\TianyanchaLogin.txt";
        }
        protected override void AfterInput()
        {
            //点击登录按钮
            RemoteWebDriver.ExecuteScript("loginObj.loginByPhone(event);");
            Console.WriteLine("点击《登录》按钮");
            Thread.Sleep(500);
        }

        protected override void BeforeInput()
        {
            RemoteWebDriver.ExecuteScript("header.loginLink(event)");
            Console.WriteLine("点击《登录/注册》按钮");
            Thread.Sleep(500);

            //点击 《密码登录》
            RemoteWebDriver.ExecuteScript("loginObj.changeCurrent(1);");
            Console.WriteLine("点击 《密码登录》按钮");
            Thread.Sleep(500);
        }

        protected override CheckCodeType GetCheckCodeType()
        {
            return CheckCodeType.Slide;
        }

        protected override void HomePage()
        {
            RemoteWebDriver.Navigate().GoToUrl("https://www.tianyancha.com/");
        }

        protected override void Input()
        {
            RemoteWebDriver.ExecuteScript("$('.contactphone').val('******');$('.contactword').val('******')");
            Thread.Sleep(500);
        }


        protected override bool PassDragCheckCode()
        {
            return _geetestSlideVerificationCode.Pass(RemoteWebDriver);
        }

        protected override bool PassImageCheckCode()
        {
            return false;
        }

        protected override bool SubmitInput()
        {
            return IsLogin();
        }

        protected override bool IsLogin()
        {
            RemoteWebDriver.Navigate().GoToUrl("https://www.tianyancha.com/login");
            return RemoteWebDriver.PageSource.Contains("退出登录");
        }
    }
}
