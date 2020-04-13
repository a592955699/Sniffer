using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Sniffer.Shop.Models
{
    [Serializable]
    public class ShopConfig
    {
        public string Account { get; set; }
        public string PassWord { get; set; }
        public string SavePath { get; set; }

        public static ShopConfig LoadConfig()
        {
            ShopConfig shopConfig = new ShopConfig();
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            configuration.GetSection("ShopConfig").Bind(shopConfig);

            shopConfig.SavePath = Path.Combine(shopConfig.SavePath, DateTime.Now.ToString("yyyyMMdd_HH_mm_ss"));

            return shopConfig;
        }
    }
}
