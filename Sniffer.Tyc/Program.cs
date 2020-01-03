using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Chrome;
using Sniffer.Core;
using Sniffer.Core.Enums;
using Sniffer.Core.Models.Pages;
using Sniffer.Tyc.Logins;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Text;

namespace Sniffer.Tyc
{
    class Program
    {
        public static CookieContainer CookieContainer = null;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            ChromeDriver driver = new ChromeDriver(AppContext.BaseDirectory);
            driver.Manage().Window.Maximize();//窗口最大化，便于脚本执行
            driver.Manage().Window.Size = new Size(800, 800);
     
            TianyanchaLogin tianyanchaLogin = new TianyanchaLogin(driver);
            bool login = tianyanchaLogin.Login();
            CookieContainer = tianyanchaLogin.CookieContainer;

            //注册编码（放在将要指定编码，进行文件解析前）
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            SnifferContext context = new SnifferContext();
            PageConfigManager pageConfigManager = new PageConfigManager();
            String path = AppContext.BaseDirectory + "PageConfigs\\ShopConfig.xml";
            var configs = pageConfigManager.LoadConfig(path);
            foreach (var item in configs)
            {
                if (item.PageType == PageType.DetailPage)
                {
                    DetailPage rootDetailPage = new DetailPage(null, item);
                    context.AddToWaitPages(rootDetailPage);
                }
                else
                {
                    ListPage rootListPage = new ListPage(null, item);
                    context.AddToWaitPages(rootListPage);
                }
            }

            SnifferManager sniffer = new SnifferManager(context);

            sniffer.OnPageDoneEventHandler = (page) =>
            {
                Console.WriteLine($"OnPageDoneEventHandler\t\t{page.Url}");
            };
            sniffer.OnRootPageDoneEventHandler = (page) =>
            {
                Console.WriteLine($"OnRootPageDoneEventHandler\t{page.Url}");
            };
            sniffer.OnListUrlPageDoneEventHandler = (page) =>
            {
                Console.WriteLine($"OnListUrlPageDoneEventHandler\t{page.Url}");
            };
            sniffer.OnListPageDoneEventHandler = (page) =>
            {
                Console.WriteLine($"OnListPageDoneEventHandler\t{page.Url}");
            };
            //详细页采集完毕，组合生成url待下载
            sniffer.OnDetailPageDoneEventHandler = (page) =>
            {
                Console.WriteLine($"OnDetailPageDoneEventHandler\t{page.Url}");
            };
            sniffer.Execute();
        }
    }
}
