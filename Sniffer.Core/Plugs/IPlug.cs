﻿using Sniffer.Core.Models.Pages;
using Sniffer.Core.Models.Sniffer.Pages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sniffer.Core.Plugs
{
    public interface IPlug
    {
        /// <summary>
        /// 根页面采集完毕
        /// </summary>
        /// <param name="page"></param>
        void OnRootPageDoneEventHandler(PageBase page);
        /// <summary>
        /// 列表的所有分页的url采集完毕
        /// </summary>
        /// <param name="page"></param>
        void OnListUrlPageDoneEventHandler(ListPage page);
        /// <summary>
        /// 详细页采集完毕
        /// 详细页内容有分页，需要所有内容都采集完毕，才执行
        /// <param name="page"></param>
        /// </summary>
        void OnDetailPageDoneEventHandler(DetailPage page);
        /// <summary>
        /// 表页采以及下面对应的详细页集完毕
        /// </summary>
        /// <param name="page"></param>
        void OnListPageDoneEventHandler(ListPage page);
        /// <summary>
        /// 页面采集完毕
        /// </summary>
        /// <param name="page"></param>
        void OnPageDoneEventHandler(PageBase page);
    }
}
