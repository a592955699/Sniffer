using JM.BaiduApi.ImageOrc;
using JM.BaiduApi.TextOrc;
using Sniffer.Core;
using Sniffer.Core.Enums;
using Sniffer.Core.Models.Configs;
using Sniffer.Core.Models.Pages;
using Sniffer.Core.Models.Sniffer;
using Sniffer.Shop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Sniffer.Shop
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("商铺采集小程序，管理员QQ 592955699!");
            var shopConfig = ShopConfig.LoadConfig();
            Console.WriteLine($"商铺账号：{shopConfig.Account}");
            Console.WriteLine($"文件地址：{shopConfig.SavePath}");

            //注册编码（放在将要指定编码，进行文件解析前）
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            SnifferContext snifferContext = new SnifferContext();

            PageConfigManager pageConfigManager = new PageConfigManager();
            String path = AppContext.BaseDirectory + "PageConfigs\\ShopConfig.xml";
            var configs = pageConfigManager.LoadConfig(path);
            foreach (var item in configs)
            {
                Console.WriteLine($"商铺地址：{item.Url}");                
                if (item.PageType==PageType.DetailPage)
                {
                    DetailPage rootDetailPage = new DetailPage(null, item);
                    snifferContext.AddToWaitPages(rootDetailPage);
                }
                else
                {
                    ListPage rootListPage = new ListPage(null, item);
                    snifferContext.AddToWaitPages(rootListPage);
                    Console.WriteLine($"采集页数：{rootListPage.ListPageConfig.MaxPage}");
                }
            }

            SnifferManager sniffer = new SnifferManager(snifferContext);

            sniffer.OnPageDoneEventHandler = (page,context) =>
            {
                Console.WriteLine($"OnPageDoneEventHandler\t\t{page.Url}");
                Console.WriteLine($"待处理页面 {context.GetWaitPageCount()} 个");
            };
            sniffer.OnRootPageDoneEventHandler = (page, context) =>
            {
                Console.WriteLine($"OnRootPageDoneEventHandler\t{page.Url}");
            };
            sniffer.OnListUrlPageDoneEventHandler = (page, context) =>
            {
                Console.WriteLine($"OnListUrlPageDoneEventHandler\t{page.Url}");
            };
            sniffer.OnListPageDoneEventHandler = (page, context) =>
            {
                Console.WriteLine($"OnListPageDoneEventHandler\t{page.Url}");
            };
            //详细页采集完毕，组合生成url待下载
            sniffer.OnDetailPageDoneEventHandler = (page, context) =>
            {
                //Console.WriteLine($"OnDetailPageDoneEventHandler\t{page.Url}");
            };
            sniffer.Execute();
        }
    }
}
