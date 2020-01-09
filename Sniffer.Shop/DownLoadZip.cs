using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Sniffer.Core.Helpers;
using Sniffer.Shop.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace Sniffer.Shop
{
    public class DownLoadZip
    {
        ChromeDriver driver = null;
        CookieContainer cookieContainer = new CookieContainer();
        ShopConfig ShopConfig { get; set; }
       
        public DownLoadZip(ChromeDriver chromeDriver)
        {
            driver = chromeDriver;
            ShopConfig = ShopConfig.LoadConfig();
        }
        public void Execute(List<String> urls)
        {
            if (Login())
            {
                //请求Url获取 zip 包路径
                foreach (var url in urls)
                {
                    String html = HttpHelper.DoGet(url, cookieContainer);
                    var urlModel = JsonConvert.DeserializeObject<UrlModel>(html);
                    if (urlModel.code == 0 && urlModel.data != null && !String.IsNullOrWhiteSpace(urlModel.data.url))
                    {
                        Console.WriteLine($"文件准备下载 {urlModel.data.url}");
                        //下载 zip 包
                        HttpHelper.DownloadFile(urlModel.data.url, ShopConfig.SavePath, true);
                        Console.WriteLine($"文件下载完毕 {urlModel.data.url}");
                    }
                    else
                    {
                        Console.WriteLine($"{url} 返回:\r\n {html}");
                    }
                }
            }
        }

        private bool Login()
        {
            Console.WriteLine("打开首页");
            driver.Navigate().GoToUrl("https://pp.17zwd.com/");
            Console.WriteLine("设置窗体大小");
            driver.Manage().Window.Maximize();
            String closeTipJs = "document.getElementsByClassName('common-navbar-daifa-tip')[0].innerHTML='';";
            Console.WriteLine("关闭提示层");
            driver.ExecuteScript(closeTipJs);
            try
            {
                Console.WriteLine("模拟登录提交");
                driver.FindElements(By.Name("username"))[1].SendKeys(ShopConfig.Account);
                driver.FindElements(By.Name("password"))[2].SendKeys(ShopConfig.PassWord);
                driver.FindElementByClassName("login-btn").Click();
                Thread.Sleep(5000);
                foreach (var item in driver.Manage().Cookies.AllCookies)
                {
                    Console.WriteLine($"Name:{item.Name}");
                    Console.WriteLine($"Value:{item.Value}");
                    Console.WriteLine($"Domain:{item.Domain}");
                    Console.WriteLine($"Path:{item.Path}");
                    Console.WriteLine($"Expiry:{item.Expiry}");

                    cookieContainer.Add(new System.Net.Cookie(item.Name, item.Value, item.Path, item.Domain));
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}
