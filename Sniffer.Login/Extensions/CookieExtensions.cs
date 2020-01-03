using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sniffer.Login.Extensions
{
    public static class CookieExtensions
    {
        public static System.Net.Cookie ToNetCookie(this OpenQA.Selenium.Cookie seleniumCookie)
        {
            return new System.Net.Cookie() {
                Name = seleniumCookie.Name,
                Value = seleniumCookie.Value,
                Path = seleniumCookie.Path,
                Domain = seleniumCookie.Domain,
                Expires = seleniumCookie.Expiry.HasValue ? seleniumCookie.Expiry.Value : DateTime.Now.AddYears(1)//没有过期时间，默认1年
            };
        }

        public static OpenQA.Selenium.Cookie ToSeleniumCookie(this System.Net.Cookie netCookie)
        {
            return new OpenQA.Selenium.Cookie(netCookie.Name, netCookie.Value, netCookie.Domain, netCookie.Path, netCookie.Expires);
        }
    }

    [Serializable]
    public class PersistenceCookie
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Domain { get; set; }
        public virtual string Path { get; set; }
        public DateTime? Expiry { get; set; }
    }

}
