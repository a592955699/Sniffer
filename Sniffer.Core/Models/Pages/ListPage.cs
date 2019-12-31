using Sniffer.Core.Models.Configs;
using Sniffer.Core.Models.Sniffer;
using Sniffer.Core.Models.Sniffer.Pages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sniffer.Core.Models.Pages
{
    /// <summary>
    /// 列表页
    /// </summary>
    [Serializable]
    public class ListPage : PageBase
    {
        public ListPage() : base() { }
        public ListPage(PageBase parent, ConfigBase config)
            : base(parent, config)
        { }
        /// <summary>
        /// 列表页配置项
        /// </summary>
        public ListConfig ListPageConfig
        {
            get
            {
                if (Config == null)
                    return null;
                else
                    return base.Config as ListConfig;

            }
        }
    }
}
