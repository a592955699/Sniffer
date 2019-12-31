using Sniffer.Core.Models.Sniffer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Sniffer.Core.Extensions
{
    /// <summary>
    /// Item 配置扩展静态类
    /// </summary>
    public static class SnifferItemExtensions
    {
        /// <summary>
        /// 根据 Item 正则匹配获取对应的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="snifferItem"></param>
        /// <param name="inputText"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T Regex<T>(this SnifferItem snifferItem, string inputText, T defaultValue)
        {
            if (string.IsNullOrWhiteSpace(inputText) || snifferItem == null)
            {
                return defaultValue;
            }
            RegexOptions regexOptions = RegexOptions.IgnoreCase;
            regexOptions |= RegexOptions.Singleline;
            Regex regex = new Regex(snifferItem.RegexString, regexOptions);
            var match = regex.Match(inputText);
            if (match.Success)
            {
                var result = match.Groups[snifferItem.ValueGroupIndex].Value;
                return (T)Convert.ChangeType(result, typeof(T));
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
