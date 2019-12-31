using System;
using System.Collections.Generic;
using System.Text;

namespace Sniffer.Shop.Models
{
    [Serializable]
    public class UrlModel
    {
        public int code { get; set; }
        public String msg { get; set; }
        public UrlItem data { get; set; }
        public class UrlItem
        {
            public String url { get; set; }
            public int download_num { get; set; }
        }
    }
}
