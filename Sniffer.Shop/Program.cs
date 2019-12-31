using Sniffer.Core;
using Sniffer.Core.Enums;
using Sniffer.Core.Models.Configs;
using Sniffer.Core.Models.Pages;
using Sniffer.Core.Models.Sniffer;
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
            Console.WriteLine("Hello World!");

            //#region 存储目录
            //ShopPlug shopPlug = new ShopPlug();
            //Console.WriteLine("请输入文件存储目录,如不输入则默认 F:\\files");
            //String readRootPath = Console.ReadLine();
            //if (!String.IsNullOrWhiteSpace(readRootPath))
            //{
            //    shopPlug.RootPath = readRootPath.Trim().Replace("/", "\\").TrimEnd('\\');
            //}
            //Console.WriteLine("文件存储目录: {0}", shopPlug.RootPath);
            //#endregion

            //#region 商店地址
            //Console.WriteLine("请输入待采集商店网站，不输入则默认 https://gz.17zwd.com/shop/21675.htm");
            //String readShopUrl = Console.ReadLine();
            //if (!String.IsNullOrWhiteSpace(readShopUrl))
            //{
            //    shopUrl = readShopUrl.Trim();
            //}
            //Console.WriteLine("待采集商店网站: {0}", shopUrl);
            //#endregion

            //#region 商店采集页数
            //while (true)
            //{
            //    Console.WriteLine("请输入待采集页数，不输入则默认 1");
            //    String readPageCountStr = Console.ReadLine();
            //    if (String.IsNullOrWhiteSpace(readPageCountStr))
            //    {
            //        pageCount = 1;
            //        break;
            //    }
            //    int readPageCount = 0;
            //    if (int.TryParse(readPageCountStr, out readPageCount) && readPageCount > 0)
            //    {
            //        pageCount = readPageCount;
            //        break;
            //    }
            //}
            //Console.WriteLine("待采集页数: {0}", pageCount);
            //#endregion

            //注册编码（放在将要指定编码，进行文件解析前）
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            SnifferContext context = new SnifferContext();

            //ListConfig rootConfig = new ListConfig()
            //{
            //    MethodType = MethodType.GET,
            //    MaxPage = pageCount,
            //    StartIndex = 1,
            //    Increment = 1,
            //    PageType = PageType.ListPage,
            //    Title = "shop 首页",
            //    Url = shopUrl,
            //    Encoding = Encoding.UTF8,
            //    Plug = shopPlug,
            //    PageCountSnifferItem = new SnifferItem(@"<div class=""all-goods-num"">共 <span>(\d+)</span> 件相关商品</div>", 1),
            //    UrlItem = new UrlItem(@"<a class=""promote-shop-image""[\s]*href=""([^""]*)""[\s]*title=""([^""]*)""", 2, 1, "https://gz.17zwd.com{0}"),
            //    SubConfig = new DetailConfig()
            //    {
            //        MethodType = MethodType.GET,
            //        PageType = PageType.DetailPage,
            //        IsList = false,
            //        Encoding = Encoding.UTF8,
            //        Plug = shopPlug,
            //        FieldItems = new List<FieldItem>() {
            //            new FieldItem()
            //            {
            //                Name="店铺Id",
            //                SnifferItem = new SnifferItem()
            //                {
            //                    RegexString=@"<meta property=""og:product:nick"" content=""name=.*; url=https://.*?/shop/(\d+).htm""/>",
            //                    ValueGroupIndex=1
            //                }
            //            },
            //            new FieldItem()
            //            {
            //                Name="产品Id",
            //                SnifferItem = new SnifferItem()
            //                {
            //                    RegexString=@"<link rel=""canonical"" href=""https://.*?/item\?GID=(\d+)"" />",
            //                    ValueGroupIndex=1
            //                }
            //            }
            //        }
            //    }
            //};

            //ListPage rootPage = new ListPage(null, rootConfig);

            //context.AddToWaitPages(rootPage);


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
