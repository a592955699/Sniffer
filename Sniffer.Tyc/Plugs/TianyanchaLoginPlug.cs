using Newtonsoft.Json;
using Sniffer.Core.Models.Pages;
using Sniffer.Core.Models.Sniffer.Pages;
using Sniffer.Core.Plugs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Sniffer.Tyc.Plugs
{
    public class TianyanchaLoginPlug : IPlug
    {
        private string rootPath = AppContext.BaseDirectory + "tianyangcha\\";
        public CookieContainer GetCookieContainer()
        {
            return Program.CookieContainer;
        }

        public void OnDetailPageDoneEventHandler(DetailPage page)
        {
            Task.Run(()=> {
                string jsonString = JsonConvert.SerializeObject(page.Data);
                if (!Directory.Exists(rootPath))
                {
                    Directory.CreateDirectory(rootPath);
                }
                //string fileName = $"{rootPath}{HttpUtility.UrlEncode(page.Title.Trim())}.json";
                string fileName = $"{rootPath}{page.Id}.json";
                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.Write(jsonString);
                    }
                }
            });
        }

        public void OnListPageDoneEventHandler(ListPage page)
        {
        }

        public void OnListUrlPageDoneEventHandler(ListPage page)
        {
        }

        public void OnPageDoneEventHandler(PageBase page)
        {
        }

        public void OnRootPageDoneEventHandler(PageBase page)
        {
        }
    }
}
