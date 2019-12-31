using Sniffer.Core.Enums;
using Sniffer.Core.Extensions;
using Sniffer.Core.Models.Configs;
using Sniffer.Core.Models.Pages;
using Sniffer.Core.Models.Sniffer;
using Sniffer.Core.Models.Sniffer.Pages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sniffer.Core
{
    public class SnifferManager
    {
        #region 
        /// <summary>
        /// 顶级页面采集完毕
        /// </summary>
        public Action<PageBase> OnRootPageDoneEventHandler { get; set; }
        /// <summary>
        /// 列表的所有分页的url采集完毕
        /// </summary>
        public Action<ListPage> OnListUrlPageDoneEventHandler { get; set; }
        /// <summary>
        /// 详细页采集完毕
        /// 详细页内容有分页，需要所有内容都采集完毕，才执行
        /// </summary>
        public Action<DetailPage> OnDetailPageDoneEventHandler { get; set; }
        /// <summary>
        /// 表页采以及下面对应的详细页集完毕
        /// </summary>
        public Action<ListPage> OnListPageDoneEventHandler { get; set; }
        /// <summary>
        /// 本页页面采集完毕
        /// 此功能不管子页面以及子页面状态
        /// </summary>
        public Action<PageBase> OnPageDoneEventHandler { get; set; } 
        #endregion

        public SnifferManager(SnifferContext context)
        {
            Context = context;
        }
        public SnifferContext Context;
        public void Execute()
        {
            while (Context.WaitPages.Count > 0)
            {
                PageBase page;
                Boolean flag = Context.GetWaitPage(out page);
                if(flag)
                {
                    Execute(page);
                }               
            }
            if(Context.WorkPages.Count>0)
            {
                Console.WriteLine($"还有{Context.WorkPages.Count}个正在采集中");
            }
            else
            {
                Console.WriteLine("所有页面已经采集完毕");
            }
        }
        private void Execute(PageBase page)
        {
            //标记为采集中
            page.SnifferState = SnifferState.Working;

            //Console.WriteLine("开始处理\t" + page.Config.Url);
            if (page.IsListPage)
            {
                ListPage listPage = page as ListPage;
                ExecuteListPage(listPage);
            }
            else
            {
                #region 详细页处理逻辑
                DetailPage detailPage = page as DetailPage;
                //详细页处理
                ExcuteDetailPage(detailPage);
                #endregion
            }
            OnPageDoneEventHandler?.Invoke(page);
            if (page.Config.Plug != null)
                page.Config.Plug.OnPageDoneEventHandler(page);
        }
        /// <summary>
        /// 列表页处理逻辑
        /// </summary>
        /// <param name="listPage"></param>
        private void ExecuteListPage(ListPage listPage)
        {
            try
            {
                List<UrlInfo> urlInfos = listPage.ListPageConfig.UrlItem.Regex(listPage.Body);
                listPage.SubPages.AddRange(CreatePages(urlInfos, listPage));

                //设置最大采集页数
                int count = listPage.ListPageConfig.PageCountSnifferItem.Regex<int>(listPage.Body, 1);
                if (count > listPage.ListPageConfig.MaxPage)
                {
                    count = listPage.ListPageConfig.MaxPage;
                }

                int startIndex = listPage.ListPageConfig.StartIndex + listPage.ListPageConfig.Increment;
                for (int i = startIndex; i <= count; i = i + listPage.ListPageConfig.Increment)
                {
                    List<UrlInfo> subUlrInfos = listPage.ListPageConfig.UrlItem.Regex(listPage.Body);
                    listPage.SubPages.AddRange(CreatePages(subUlrInfos, listPage));
                }
                Context.AddToWaitPages(listPage.SubPages);

                //触发列表页的所有Url采集完毕事件
                if (OnListUrlPageDoneEventHandler != null)
                    OnListUrlPageDoneEventHandler(listPage);
                if (listPage.Config.Plug != null)
                    listPage.Config.Plug.OnListUrlPageDoneEventHandler(listPage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                //标注为已完成
                Context.AddToComplatePages(listPage);

                //触发页面采集事件
                listPage.PageDone(this);
            }
        }

        private List<PageBase> CreatePages(List<UrlInfo> urlInfos, PageBase parentPage)
        {
            List<PageBase> pages = new List<PageBase>();
            foreach (var item in urlInfos)
            {
                pages.Add(CreatePage(item, parentPage));
            }
            return pages;
        }

        private PageBase CreatePage(UrlInfo urlInfo, PageBase parentPage)
        {
            //必须复制属性，否则由于引用问题，导致变量会被修改
            var config = parentPage.Config.SubConfig;
            if (config is DetailConfig)
            {
                var page = new DetailPage(parentPage, config);
                page.Url = urlInfo.Url;
                page.Title = urlInfo.Title;
                return page;
            }
            else
            {
                var page = new ListPage(parentPage, config);
                page.Url = urlInfo.Url;
                page.Title = urlInfo.Title;
                return page;
            }
        }

        /// <summary>
        /// 详细页采集
        /// </summary>
        /// <param name="detailPage"></param>
        private void ExcuteDetailPage(DetailPage detailPage)
        {
            try
            {
                foreach (FieldItem fieldItem in detailPage.DetailPageConfig.FieldItems)
                {
                    String value = fieldItem.SnifferItem.Regex<String>(detailPage.Body, fieldItem.DefaultValue);

                    //是否是父详细页的子页
                    if (detailPage.ParentPage is DetailPage && ((DetailPage)detailPage.ParentPage).DetailPageConfig.IsList)
                    {
                        #region 当前页是父详细页的子页,则将此内容加入到详细页上去
                        var parentPsage = ((DetailPage)detailPage.ParentPage);
                        String parentValue = "";
                        if (parentPsage.Data.TryGetValue(fieldItem.Name, out parentValue))
                        {
                            parentValue += value;
                            parentPsage.Data.Add(fieldItem.Name, parentValue);
                        }
                        else
                        {
                            parentPsage.Data.Add(fieldItem.Name, value);
                        }
                        #endregion
                    }
                    else
                    {
                        detailPage.Data.Add(fieldItem.Name, value);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                //标注为已完成
                Context.AddToComplatePages(detailPage);

                //触发页面采集事件
                detailPage.PageDone(this);
            }
        }
    }
}
