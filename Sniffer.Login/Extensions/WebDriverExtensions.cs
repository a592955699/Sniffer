using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Sniffer.Login.Extensions
{
    public static class WebDriverExtensions
    {
        /// <summary>
        /// 获取 IWebElement 元素
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="by"></param>
        /// <param name="timeoutInMilliseconds"></param>
        /// <returns></returns>
        public static IWebElement FindElementExt(this IWebDriver driver, By by, int timeoutInMilliseconds = 0)
        {
            try
            {
                if (timeoutInMilliseconds > 0)
                {
                    var wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(timeoutInMilliseconds));
                    return wait.Until(drv => drv.FindElement(by));
                }
                return driver.FindElement(by);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// 更新 IWebDriver 的 Cookie
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="cookieCollection"></param>
        public static void UpdateCookie(this IWebDriver driver, CookieCollection cookieCollection)
        {
            var cookieJar = driver.Manage().Cookies;
            var webDriverCookies = cookieJar.AllCookies;

            var removeCookies = new List<OpenQA.Selenium.Cookie>();

            var newCookies = new List<OpenQA.Selenium.Cookie>();
            foreach (System.Net.Cookie netCookie in cookieCollection)
            {
                var cookie = webDriverCookies.FirstOrDefault(x => x.Name == netCookie.Name && x.Domain == netCookie.Domain && x.Path == netCookie.Path);
                if (cookie != null)
                {
                    removeCookies.Add(cookie);
                }
                newCookies.Add(netCookie.ToSeleniumCookie());
            }

            foreach (var item in removeCookies)
            {
                cookieJar.DeleteCookie(item);
            }
            foreach (var item in newCookies)
            {
                cookieJar.AddCookie(item);
            }
        }
        /// <summary>
        /// 根据当身份获取 cookie 容器
        /// </summary>
        /// <param name="driver"></param>
        /// <returns></returns>
        public static CookieContainer GetCookieContainer(this IWebDriver driver)
        {
            CookieContainer cookieContainer = new CookieContainer();
            foreach (OpenQA.Selenium.Cookie webDriverCookie in driver.Manage().Cookies.AllCookies)
            {
                cookieContainer.Add(webDriverCookie.ToNetCookie());
            }
            return cookieContainer;
        }
        /// <summary>
        /// 将 Cookie 持久化到文档
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="filePath"></param>
        public static void SaveCookieToFile(this IWebDriver driver, string filePath)
        {
            var cookies = driver.Manage().Cookies.AllCookies.Select(x => new PersistenceCookie()
            {
                Name = x.Name,
                Value = x.Value,
                Path = x.Path,
                Domain = x.Domain,
                Expiry = x.Expiry
            });

            var jsonString = JsonConvert.SerializeObject(cookies);
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(jsonString);
                    sw.Flush();
                    sw.Dispose();
                    fs.Dispose();
                }
            }
        }
        /// <summary>
        /// 根据文档反序列化到cookie
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="filePath"></param>
        public static void LoadCookieByFile(this IWebDriver driver, string filePath)
        {
            if (!File.Exists(filePath))
            {
                return;
            }
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    var jsonString = sr.ReadToEnd();
                    var cokoies = JsonConvert.DeserializeObject<List<PersistenceCookie>>(jsonString);

                    CookieCollection cookieCollection = new CookieCollection();
                    foreach (var item in cokoies)
                    {
                        cookieCollection.Add(new System.Net.Cookie()
                        {
                            Name = item.Name,
                            Value = item.Value,
                            Path = item.Path,
                            Domain = item.Domain,
                            Expires = item.Expiry.HasValue ? item.Expiry.Value : DateTime.Now.AddYears(1)
                        });
                    }

                    driver.UpdateCookie(cookieCollection);
                }
            }
        }
    }
}
