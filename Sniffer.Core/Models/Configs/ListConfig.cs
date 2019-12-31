using Sniffer.Core.Models.Sniffer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sniffer.Core.Models.Configs
{
    [Serializable]
    public class ListConfig : ConfigBase
    {
        /// <summary>
        /// 子页Url采集配置
        /// </summary>
        public UrlItem UrlItem { get; set; }
        /// <summary>
        /// 起始页数
        /// </summary>
        public int StartIndex { get; set; } = 1;
        /// <summary>
        /// 页码递增量
        /// </summary>
        public int Increment { get; set; } = 1;
        /// <summary>
        /// 最大采集页数
        /// </summary>
        public int MaxPage { get; set; } = int.MaxValue;
        /// <summary>
        /// 采集总页数配置项
        /// </summary>
        public SnifferItem PageCountSnifferItem;
    }
}
