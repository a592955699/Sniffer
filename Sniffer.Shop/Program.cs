using Sniffer.Core;
using Sniffer.Core.Enums;
using Sniffer.Core.Models.Configs;
using Sniffer.Core.Models.Pages;
using Sniffer.Core.Models.Sniffer;
using Sniffer.Login.Baidu;
using Sniffer.Shop.Plugs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Sniffer.Shop
{
    class Program
    {
        static String shopUrl = "https://gz.17zwd.com/shop/21675.htm";
        static int pageCount = 1;
        static void Main(string[] args)
        {

            BaiduTest.Test(@"F:\files\test.png");

            Console.WriteLine("Hello World!");

            //注册编码（放在将要指定编码，进行文件解析前）
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            SnifferContext context = new SnifferContext();

            PageConfigManager pageConfigManager = new PageConfigManager();
            String path = AppContext.BaseDirectory + "PageConfigs\\ShopConfig.xml";
            var configs = pageConfigManager.LoadConfig(path);
            foreach (var item in configs)
            {
                if(item.PageType==PageType.DetailPage)
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
