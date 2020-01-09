using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Text;

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
            //var builder = new ConfigurationBuilder()
            //   .AddJsonFile(cfg =>
            //   {
            //       cfg.Path = "appsettings.json";
            //       cfg.ReloadOnChange = true;
            //       cfg.Optional = true;
            //   });
            ShopConfig shopConfig = new ShopConfig();
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            configuration.GetSection("ShopConfig").Bind(shopConfig);
            return shopConfig;
        }
    }
}
