using Sniffer.Core.Enums;
using Sniffer.Core.Models.Configs;
using Sniffer.Core.Models.Sniffer;
using Sniffer.Core.Plugs;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Sniffer.Core
{
    public class PageConfigManager
    {
        XmlDocument xmlDoc = new XmlDocument();
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public List<ConfigBase> LoadConfig(String filePath)
        {
            List<ConfigBase> configs = new List<ConfigBase>();
            xmlDoc.Load(filePath);

            //根配置
            XmlNodeList rootXmlNodeList = xmlDoc.SelectNodes("//PageConfig[@Root=\"true\"]");

            for (int i = 0; i < rootXmlNodeList.Count; i++)
            {
                var config = LoadPageConfig(rootXmlNodeList[i].Attributes["Name"].Value);
                if (config != null)
                    configs.Add(config);
            }

            return configs;
        }
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private ConfigBase LoadPageConfig(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                return null;
            }
            ConfigBase configBase = null;
            XmlNode xmlNode = xmlDoc.SelectSingleNode($"//PageConfig[@Name=\"{name}\"]");
            string pageType = xmlNode.SelectSingleNode("PageType").InnerText;
           
            switch (pageType)
            {
                case "ListPage":
                    configBase = LoadListConfig(xmlNode);
                    configBase.Name = name;
                    break;
                case "DetailPage":
                    configBase = LoadDetailConfig(xmlNode);
                    configBase.Name = name;
                    break;
            }
            return configBase;
        }
        /// <summary>
        /// 获取详细页配置
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <returns></returns>
        private DetailConfig LoadDetailConfig(XmlNode xmlNode)
        {
            if (xmlNode == null) return null;
            DetailConfig config = new DetailConfig();

            #region ConfigBase 开始
            config.Encoding = Encoding.GetEncoding(xmlNode.SelectSingleNode("Encoding").InnerText);
            config.MethodType = xmlNode.SelectSingleNode("MethodType").InnerText == "POST" ? MethodType.POST : MethodType.GET;
            config.PageType = xmlNode.SelectSingleNode("PageType").InnerText == "ListPage" ? PageType.ListPage : PageType.DetailPage;
            string subPageConfig = xmlNode.SelectSingleNode("SubPageConfig")?.InnerText;
            string plugNameSpace = xmlNode.SelectSingleNode("Plug")?.InnerText;
            if (!string.IsNullOrWhiteSpace(plugNameSpace))
            {
                config.Plug = CreatePlug(plugNameSpace);
            }
            if (!string.IsNullOrWhiteSpace(subPageConfig))
            {
                config.SubConfig = LoadPageConfig(subPageConfig);
            }
            #endregion

            #region ListConfig 开始
            config.IsList = bool.Parse(xmlNode.SelectSingleNode("IsList").InnerText);
            if(config.IsList)
            {
                config.UrlItem = CreateUrlItem(xmlNode.SelectSingleNode("UrlItem"));
                config.StartIndex = int.Parse(xmlNode.SelectSingleNode("StartIndex").InnerText);
                config.Increment = int.Parse(xmlNode.SelectSingleNode("Increment").InnerText);
                config.MaxPage = int.Parse(xmlNode.SelectSingleNode("MaxPage").InnerText);
                config.PageCountSnifferItem = CreateSnifferItem(xmlNode.SelectSingleNode("PageCountSnifferItem"));
            }
            #endregion

            #region DetailPage 开始
            config.FieldItems = CreateFieldItems(xmlNode.SelectSingleNode("FieldItems"));
            #endregion
            return config;
        }
        /// <summary>
        /// 获取列表页配置
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <returns></returns>
        private ListConfig LoadListConfig(XmlNode xmlNode)
        {
            if (xmlNode == null) return null;
            ListConfig config = new ListConfig();

            #region ConfigBase 开始
            config.Url = xmlNode.SelectSingleNode("Url").InnerText;
            config.Encoding = Encoding.GetEncoding(xmlNode.SelectSingleNode("Encoding").InnerText);
            config.MethodType = xmlNode.SelectSingleNode("Encoding").InnerText == "POST" ? MethodType.POST : MethodType.GET;
            string subPageConfig = xmlNode.SelectSingleNode("SubPageConfig").InnerText;
            string plugNameSpace = xmlNode.SelectSingleNode("Plug").InnerText;
            if (!string.IsNullOrWhiteSpace(plugNameSpace))
            {
                config.Plug = CreatePlug(plugNameSpace);
            }
            if(!string.IsNullOrWhiteSpace(subPageConfig))
            {
                config.SubConfig = LoadPageConfig(subPageConfig);
            }
            #endregion

            #region ListConfig 开始
            config.UrlItem = CreateUrlItem(xmlNode.SelectSingleNode("UrlItem"));
            config.StartIndex = int.Parse(xmlNode.SelectSingleNode("StartIndex").InnerText);
            config.Increment = int.Parse(xmlNode.SelectSingleNode("Increment").InnerText);
            config.MaxPage = int.Parse(xmlNode.SelectSingleNode("MaxPage").InnerText);
            config.PageCountSnifferItem = CreateSnifferItem(xmlNode.SelectSingleNode("PageCountSnifferItem")); 
            #endregion
            return config;
        }
        /// <summary>
        /// 获取详细页数据采集规则集合
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <returns></returns>
        private List<FieldItem> CreateFieldItems(XmlNode xmlNode)
        {
            if (xmlNode == null) return null;
            List<FieldItem> fieldItems = new List<FieldItem>();
            foreach (XmlNode item in xmlNode.SelectNodes("FieldItem"))
            {
                FieldItem fieldItem = new FieldItem();
                fieldItem.DefaultValue = item.SelectSingleNode("DefaultValue").InnerText;
                fieldItem.Name = item.SelectSingleNode("Name").InnerText;
                fieldItem.SnifferItem = CreateSnifferItem(item.SelectSingleNode("SnifferItem"));
                fieldItems.Add(fieldItem);
            }
            return fieldItems;
        }
        /// <summary>
        /// 获取正则匹配想
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <returns></returns>
        private SnifferItem CreateSnifferItem(XmlNode xmlNode)
        {
            if (xmlNode == null) return null;
            SnifferItem snifferItem = new SnifferItem();
            snifferItem.RegexString = xmlNode.SelectSingleNode("RegexString").InnerText;
            snifferItem.ValueGroupIndex = int.Parse(xmlNode.SelectSingleNode("ValueGroupIndex").InnerText);
            return snifferItem;
        }
        /// <summary>
        /// 获取 UrlItem
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <returns></returns>
        private UrlItem CreateUrlItem(XmlNode xmlNode)
        {
            if (xmlNode == null) return null;
            UrlItem urlItem = new UrlItem();
            urlItem.RegexString = xmlNode.SelectSingleNode("RegexString").InnerText;            
            urlItem.UrlFomart = xmlNode.SelectSingleNode("UrlFomart").InnerText;
            urlItem.TitleGroupIndex = int.Parse(xmlNode.SelectSingleNode("TitleGroupIndex").InnerText);
            urlItem.UrlGroupIndex = int.Parse(xmlNode.SelectSingleNode("UrlGroupIndex").InnerText);
            return urlItem;
        }
        /// <summary>
        /// 根据权限定名，反射创建插件
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <returns></returns>
        private IPlug CreatePlug(string nameSpace)
        {
            var arr = nameSpace.Split(",");
            return (IPlug)System.Reflection.Assembly.Load(arr[1]).CreateInstance(arr[0], false);
        }
    }
}

