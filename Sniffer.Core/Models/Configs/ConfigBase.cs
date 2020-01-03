using Sniffer.Core.Enums;
using Sniffer.Core.Plugs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sniffer.Core.Models.Configs
{
    /// <summary>
    /// 配置项
    /// </summary>
    public class ConfigBase
    {
        public String Name { get; set; }
        /// <summary>
        /// 页面Url
        /// </summary>
        public String Url { get; set; }
        ///// <summary>
        ///// 页面标题
        ///// </summary>
        //public String Title { get; set; }
        /// <summary>
        /// 编码格式
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        /// <summary>
        /// url请求方式
        /// </summary>
        public MethodType MethodType { get; set; } = MethodType.GET;
        /// <summary>
        /// 页面类型
        /// </summary>
        public PageType PageType { get; set; }
        /// <summary>
        /// 子页配置
        /// </summary>
        public ConfigBase SubConfig { get; set; }
        public IPlug Plug { get; set; }
    }
}
