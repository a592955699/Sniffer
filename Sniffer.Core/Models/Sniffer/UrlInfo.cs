using System;
using System.Collections.Generic;
using System.Text;

namespace Sniffer.Core.Models.Sniffer
{
    /// <summary>
    /// Url/标题模型
    /// </summary>
    [Serializable]
    public class UrlInfo
    {
        public UrlInfo() { }
        public UrlInfo(string url, string title)
        {
            Title = title;
            Url = url;
        }
        public string Url { get; set; }
        public string Title { get; set; }
    }
}
