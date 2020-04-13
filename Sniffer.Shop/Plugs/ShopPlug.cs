using OpenQA.Selenium.Chrome;
using Sniffer.Core;
using Sniffer.Core.Models.Pages;
using Sniffer.Core.Models.Sniffer.Pages;
using Sniffer.Core.Plugs;
using Sniffer.Shop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Sniffer.Shop.Plugs
{
    public class ShopPlug : IPlug
    {
        ChromeDriver driver = null;
        ShopConfig ShopConfig { get; set; }
        string fileName = "Product.txt";
        public ShopPlug()
        {
            ShopConfig = ShopConfig.LoadConfig();
        }
        public void OnDetailPageDoneEventHandler(DetailPage page, SnifferContext snifferContext)
        {
            String filePath = Path.Combine(ShopConfig.SavePath, fileName);
            String key, value;
            page.Data.TryGetValue("店铺Id", out key);
            page.Data.TryGetValue("产品Id", out value);
            String url = String.Format("https://seller.17zwd.com/site/goods/img/download?goods_id={0}&shop_id={1}\r\n", value, key);
            AppendFile(filePath, url);

            //保存后，清除数据，减少内存开销
            page.Data.Clear();
            page.Body = "";
        }

        public void OnListPageDoneEventHandler(ListPage page, SnifferContext snifferContext)
        {
        }

        public void OnListUrlPageDoneEventHandler(ListPage page, SnifferContext snifferContext)
        {
        }

        public void OnPageDoneEventHandler(PageBase page, SnifferContext snifferContext)
        {
        }

        public void OnRootPageDoneEventHandler(PageBase page, SnifferContext snifferContext)
        {
            String filePath = Path.Combine(ShopConfig.SavePath, fileName);
            List<string> urls = ReadFile(filePath);

            if (driver == null)
            {
                string driverDiectory = Path.Combine(AppContext.BaseDirectory, "ref");
                driver = new ChromeDriver(driverDiectory);
            }

            DownLoadZip downLoadZip = new DownLoadZip(driver);
            downLoadZip.Execute(urls);
            driver.Close();
            driver.Dispose();
        }

        private List<String> ReadFile(String filePath)
        {
            List<String> result = new List<string>();
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    string line = sr.ReadLine();
                    while (line != null)
                    {
                        result.Add(line);
                        line = sr.ReadLine();
                    }
                }
            }
            return result;
        }


        private void AppendFile(String filePath, String text)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            if (!Directory.Exists(fileInfo.DirectoryName))
            {
                Directory.CreateDirectory(fileInfo.DirectoryName);
            }

            using (FileStream fs = new FileStream(filePath, FileMode.Append))
            {
                using (StreamWriter streamWriter = new StreamWriter(fs))
                {
                    Byte[] bytes = Encoding.UTF8.GetBytes(text);
                    fs.Write(bytes);
                }
            }
        }

        public CookieContainer GetCookieContainer()
        {
            return null;
        }
    }
}
