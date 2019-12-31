using Sniffer.Core.Enums;
using Sniffer.Core.Helpers;
using Sniffer.Core.Models.Configs;
using Sniffer.Core.Models.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sniffer.Core.Models.Sniffer.Pages
{
    /// <summary>
    /// 采集业基类
    /// </summary>
    [Serializable]
    public class PageBase
    {
        public PageBase() {
            this.Id = Guid.NewGuid().ToString();
        }
        public PageBase(PageBase parent,ConfigBase config):this()
        {
            this.ParentPage = parent;
            this.Config = config;
            this.Url = config.Url;
            this.Title = config.Name;
        }
        public String Id { get; set; }
        /// <summary>
        /// 页面Url
        /// </summary>
        public String Url { get; set; }
        /// <summary>
        /// 页面标题
        /// </summary>
        public String Title { get; set; }
        /// <summary>
        /// 父页
        /// </summary>
        public PageBase ParentPage { get; set; }
        public bool IsListPage {
            get {
                return Config.PageType == PageType.ListPage;
           }
        }
        /// <summary>
        /// 配置
        /// </summary>
        public ConfigBase Config { get; set; }
        /// <summary>
        /// 子页集合
        /// 如果是列表页，这就是子页集合。
        /// 如果是详细页，这里就是详细页内容的分页集合
        /// </summary>
        public List<PageBase> SubPages { get; set; } = new List<PageBase>();
        /// <summary>
        /// 采集的源 html
        /// </summary>
        protected String _text;
        /// <summary>
        /// 获取html源文件
        /// </summary>
        public String Body {
            get {
                if(_text==null)
                {                    
                    if(string.IsNullOrWhiteSpace(Url))
                    {
                        throw new Exception("该采集页没有采集配置");
                    }
                    try
                    {
                        _text = HttpHelper.DoGet(Url, Config.Encoding);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        _text = "";
                    }
                }
                return _text;
            }
            set {
                _text = value;
            }
        }
        /// <summary>
        /// 本页是否处理完毕
        /// </summary>
        public bool Done { get; set; } = false;
        /// <summary>
        /// 
        /// </summary>
        public SnifferState SnifferState { get; set; } = SnifferState.None;
        /// <summary>
        /// 判断本任务以及所有子任务是否完毕
        /// 如果完毕则通知父页
        /// </summary>
        /// <param name="snifferManager"></param>
        public void PageDone(SnifferManager snifferManager)
        {
            //snifferManager?.OnPageDoneEventHandler(this);
            //if (this.Config.Plug != null)
            //    this.Config.Plug.OnPageDoneEventHandler(this);

            //所有子页已完成，则标注已完成状态
            if (SubPages.All(x => x.Done && x.SnifferState == SnifferState.Complate))
            {
                SnifferState = SnifferState.Complate;
            }

            if (ParentPage == null)
            {
                if (SubPages.All(x => x.Done && x.SnifferState == SnifferState.Complate))
                {
                    //触发顶级页采集完毕事件
                    snifferManager?.OnRootPageDoneEventHandler(this);
                    if (this.Config.Plug != null)
                        this.Config.Plug.OnRootPageDoneEventHandler(this);
                }
            }
            else
            {
                //当前页以及当前页的所有子页完毕
                if (SnifferState == SnifferState.Complate)
                {
                    if (Config.PageType == PageType.DetailPage)
                    {
                        //当前详细页以及详细页内容采集完毕
                        snifferManager?.OnDetailPageDoneEventHandler(this as DetailPage);
                        if (this.Config.Plug != null)
                            this.Config.Plug.OnDetailPageDoneEventHandler(this as DetailPage);
                    }
                    else
                    {
                        //列表页以及所有子页采集完毕
                        snifferManager?.OnListPageDoneEventHandler(this as ListPage);
                        if (this.Config.Plug != null)
                            this.Config.Plug.OnListPageDoneEventHandler(this as ListPage);
                    }

                    //触发父页 PageDone
                    ParentPage.PageDone(snifferManager);
                }
            }
        }
    }
}
