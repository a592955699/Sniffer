using Sniffer.Core.Models.Sniffer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sniffer.Core.Models.Configs
{
    /// <summary>
    /// 详细页配置
    /// </summary>
    [Serializable]
    public class DetailConfig : ListConfig
    {
        /// <summary>
        /// 是否有分页
        /// </summary>
        public bool IsList { get; set; }
        /// <summary>
        /// 采集字段配置集合
        /// </summary>
        public List<FieldItem> FieldItems { get; set; }
    }
}
