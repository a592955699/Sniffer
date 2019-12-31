using System;
using System.Collections.Generic;
using System.Text;

namespace Sniffer.Core.Models.Sniffer
{
    /// <summary>
    /// 采集字段的配置项
    /// </summary>
    [Serializable]
    public class FieldItem
    {
        public FieldItem() { }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; set; }
        /// <summary>
        /// 采集配置
        /// </summary>
        public SnifferItem SnifferItem { get; set; }
    }
}
