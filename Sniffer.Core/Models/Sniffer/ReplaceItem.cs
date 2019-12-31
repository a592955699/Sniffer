using System;
using System.Collections.Generic;
using System.Text;

namespace Sniffer.Core.Models.Sniffer
{
    /// <summary>
    /// 正则替换的配置项
    /// </summary>
    [Serializable]
    public class ReplaceItem
    {
        /// <summary>
        /// 正则字符串
        /// </summary>
        public string RegexString { get; set; }
        /// <summary>
        /// 替换字符串
        /// </summary>
        public string Replacement { get; set; }
    }
}
