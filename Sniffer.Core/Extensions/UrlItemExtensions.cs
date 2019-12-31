using Sniffer.Core.Models.Sniffer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Sniffer.Core.Extensions
{
    /// <summary>
    /// Url配置扩展静态类
    /// </summary>
    public static class UrlItemExtensions
    {
        /// <summary>
        /// 根据 UrlItem 正则匹配获取 UrlInfo
        /// </summary>
        /// <param name="snifferUrlItem"></param>
        /// <param name="inputText"></param>
        /// <returns></returns>
        public static List<UrlInfo> Regex(this UrlItem snifferUrlItem, string inputText)
        {
            var urlItems = new List<UrlInfo>();
            if (string.IsNullOrWhiteSpace(inputText) || snifferUrlItem == null)
            {
                return urlItems;
            }
            RegexOptions regexOptions = RegexOptions.IgnoreCase;
            Regex regex = new Regex(snifferUrlItem.RegexString, regexOptions);
            var matchs = regex.Matches(inputText);
            foreach (Match item in matchs)
            {
                String url = item.Groups[snifferUrlItem.UrlGroupIndex].Value;
                if(!String.IsNullOrWhiteSpace(snifferUrlItem.UrlFomart))
                {
                    url = String.Format(snifferUrlItem.UrlFomart, url);
                }
                String title = item.Groups[snifferUrlItem.TitleGroupIndex].Value;
                urlItems.Add(new UrlInfo(url,title));
            }
            return urlItems;
        }
    }
}
