using Sniffer.Core.Models.Sniffer.Pages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Sniffer.Core
{
    /// <summary>
    /// 采集上下文
    /// </summary>
    public class SnifferContext
    {
        public SnifferContext() {
            Id = Guid.NewGuid().ToString();
        }
        public String Id { get; set; }
        /// <summary>
        /// 待采集的页
        /// </summary>
        public ConcurrentQueue<PageBase> WaitPages { get; private set; } = new ConcurrentQueue<PageBase>();
        /// <summary>
        /// 正则采集的页
        /// </summary>
        public ConcurrentDictionary<String, PageBase> WorkPages { get; private set; } = new ConcurrentDictionary<String, PageBase>();
        /// <summary>
        /// 采集结束的页
        /// </summary>
        public ConcurrentDictionary<String, PageBase> ComplatePages { get; private set; } = new ConcurrentDictionary<String, PageBase>();
        /// <summary>
        /// 将页面加入待采集集合中
        /// </summary>
        /// <param name="page"></param>
        public void AddToWaitPages(PageBase page)
        {
            WaitPages.Enqueue(page);
        }
        /// <summary>
        /// 将页面加入待采集集合中
        /// </summary>
        /// <param name="page"></param>
        public void AddToWaitPages(List<PageBase> pages)
        {
            foreach (var page in pages)
            {
                WaitPages.Enqueue(page);
            }
        }        
        /// <summary>
        /// 将处理完毕的采集页（包括成功以及报错的页），放入到处理完毕的集合中
        /// </summary>
        /// <param name="page"></param>
        public void AddToComplatePages(PageBase page)
        {
            page.Done = true;
            PageBase o;
            WorkPages.TryRemove(page.Id, out o);
            ComplatePages.TryAdd(page.Id, page);
        }
        /// <summary>
        /// 获取待采集的页面
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public bool GetWaitPage(out PageBase page)
        {
            bool flag = WaitPages.TryDequeue(out page);
            if(flag)
            {
                WorkPages.TryAdd(page.Id, page);
            }
            return flag;
        }
    }
}
