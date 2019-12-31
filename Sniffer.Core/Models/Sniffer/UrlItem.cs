using System;
using System.Collections.Generic;
using System.Text;

namespace Sniffer.Core.Models.Sniffer
{
    /// <summary>
    /// Url 的配置项
    /// </summary>
    [Serializable]
    public class UrlItem
    {
        public UrlItem() { }
        public UrlItem(string regexString, int titleGroupIndex, int urlGroupIndex,String urlFomart)
        {
            RegexString = regexString;
            TitleGroupIndex = titleGroupIndex;
            UrlGroupIndex = urlGroupIndex;
            UrlFomart = urlFomart;
        }
        /// <summary>
        /// 正则字符串
        /// </summary>
        public string RegexString { get; set; }
        /// <summary>
        /// 标题所在组
        /// </summary>
        public int TitleGroupIndex { get; set; }
        /// <summary>
        /// Url所在组
        /// </summary>
        public int UrlGroupIndex { get; set; }
        /// <summary>
        /// Url格式化
        /// 处理正则只匹配到了动态参数，url域名或者其他固定参数值没匹配的时候使用
        /// </summary>
        public String UrlFomart { get; set; }
    }
}
