using System;
using System.Collections.Generic;
using System.Text;

namespace Sniffer.Core.Models.Sniffer
{
    /// <summary>
    /// 采集的配置项
    /// </summary>
    [Serializable]
    public class SnifferItem
    {
        public SnifferItem() { }
        public SnifferItem(string regexString, int valueGroupIndex)
        {
            RegexString = regexString;
            ValueGroupIndex = valueGroupIndex;
        }
        /// <summary>
        /// 正则字符串
        /// </summary>
        public string RegexString { get; set; }
        /// <summary>
        /// 值所在组
        /// </summary>
        public int ValueGroupIndex { get; set; }
    }
}
