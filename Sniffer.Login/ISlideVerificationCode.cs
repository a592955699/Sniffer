using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sniffer.Login
{
    public interface ISlideVerificationCode
    {
        bool Pass(RemoteWebDriver remoteWebDriver);
    }
}
