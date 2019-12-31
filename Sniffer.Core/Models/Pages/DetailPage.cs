using Sniffer.Core.Models.Configs;
using Sniffer.Core.Models.Sniffer.Pages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sniffer.Core.Models.Pages
{
    /// <summary>
    /// 详细页
    /// 详细页，也有可能内容分页
    /// 所以详细页是一个特殊的列表页
    /// </summary>
    [Serializable]
    public class DetailPage : ListPage
    {
        public DetailPage() : base() { }
        public DetailPage(PageBase parent, ConfigBase config)
            :base(parent,config)
        { }

        /// <summary>
        /// 详细页配置项
        /// </summary>
        public DetailConfig DetailPageConfig
        {
            get
            {
                if (Config == null)
                    return null;
                else
                    return base.Config as DetailConfig;

            }
        }
        public Dictionary<String, String> Data { get; set; } = new Dictionary<string, String>();
    }
}
