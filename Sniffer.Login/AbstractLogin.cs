using System;
using Sniffer.Login.Extensions;
using System.IO;
using System.Net;
using System.Text;
using System.IO.Compression;
using OpenQA.Selenium.Remote;
using Common.Logging;

namespace Sniffer.Login.SiteLogins
{
    public abstract class AbstractLogin
    {
        #region 属性定义
        public static ILog log = LogManager.GetLogger(typeof(AbstractLogin));
        public virtual CookieContainer CookieContainer { get; set; } = new CookieContainer();
        public virtual Encoding Encoding { get; set; } = Encoding.UTF8;
        public virtual string CookieFilePath { get; set; }
        public virtual string Accept { get; set; } = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
        public virtual string UserAgent { get; set; } = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.132 Safari/537.36";
        public virtual string DomainUrl { get; set; }
        public virtual string UserName { get; set; }
        public virtual string PassWord { get; set; }

        public RemoteWebDriver RemoteWebDriver { get; set; }
        #endregion

        #region 构造函数
        public AbstractLogin(RemoteWebDriver remoteWebDriver)
        {
            RemoteWebDriver = remoteWebDriver;
        }
        #endregion

        #region 主入口函数
        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public virtual bool Login()
        {
            try
            {
                if(!string.IsNullOrWhiteSpace(CookieFilePath) && File.Exists(CookieFilePath))
                {
                    HomePage();
                    ReadCookie();
                }
                

                //判断是否已经登录
                if (IsLogin())
                {
                    Console.WriteLine("已经登录");
                    CookieContainer = RemoteWebDriver.GetCookieContainer();
                    return true;
                }
                HomePage();
                BeforeInput();
                Input();
                AfterInput();

                //判断是否需要验证码
                var checkCodeType = GetCheckCodeType();

                if (checkCodeType == CheckCodeType.Image)
                {
                    if (!PassImageCheckCode())
                    {
                        return false;
                    }
                }
                else if (checkCodeType == CheckCodeType.Slide)
                {
                    if (!PassDragCheckCode())
                    {
                        return false;
                    }
                }
                bool postResult = SubmitInput();
                if (postResult)
                {
                    CookieContainer = RemoteWebDriver.GetCookieContainer();
                    WriteCookie();
                }
                Console.WriteLine("登录：" + postResult);
                return postResult;
            }
            catch (Exception ex)
            {
                log.Error("登录失败", ex);
                return false;
            }
        }
        #endregion

        #region 私有方法
        protected string GetRequestString(HttpWebRequest httpWebRequest)
        {
            string html = string.Empty;
            var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (Stream responseStream = httpWebResponse.GetResponseStream())
            {
                //处理gzip格式的流
                if (httpWebResponse.ContentEncoding != null && httpWebResponse.ContentEncoding.ToLower().Contains("gzip"))
                {
                    var gzipStream = new GZipStream(responseStream, CompressionMode.Decompress);
                    using (StreamReader streamReader = new StreamReader(gzipStream, Encoding))
                    {
                        html = streamReader.ReadToEnd();
                    }
                }
                else
                {
                    using (StreamReader streamReader = new StreamReader(responseStream, Encoding))
                    {
                        html = streamReader.ReadToEnd();
                    }
                }
            }
            return html;
        }
        #endregion

        #region abstract Method       
        
        /// <summary>
        /// 请求首页
        /// </summary>
        /// <returns></returns>
        protected abstract void HomePage();
        /// <summary>
        /// 是否登录
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        protected abstract bool IsLogin();
        ///// <summary>
        ///// 是否登录
        ///// </summary>
        ///// <param name="html"></param>
        ///// <returns></returns>
        //protected abstract bool IsLogin(string html);
        /// <summary>
        /// 验证码类型
        /// </summary>
        /// <returns></returns>
        protected abstract CheckCodeType GetCheckCodeType();
        /// <summary>
        /// 过验数字图片证码
        /// </summary>
        /// <returns></returns>
        protected abstract bool PassImageCheckCode();
        /// <summary>
        /// 过滑动阴影验证码
        /// </summary>
        /// <returns></returns>
        protected abstract bool PassDragCheckCode();
        /// <summary>
        /// 提交登录数据
        /// </summary>
        /// <returns></returns>
        protected abstract bool SubmitInput();

        protected void WriteCookie()
        {
            if (!string.IsNullOrWhiteSpace(CookieFilePath))
            {
                RemoteWebDriver.SaveCookieToFile(CookieFilePath);
            }
        }

        /// <summary>
        /// 读取文件反序列化 Cookie
        /// </summary>
        protected void ReadCookie()
        {
            if(!string.IsNullOrWhiteSpace(CookieFilePath))
            {
                RemoteWebDriver.LoadCookieByFile(CookieFilePath);
            }
        }
        /// <summary>
        /// 输入验证码之前执行
        /// </summary>
        protected abstract void BeforeInput();
        /// <summary>
        /// 输入账号密码
        /// </summary>
        protected abstract void Input();
        /// <summary>
        /// 输入验证码之后执行
        /// </summary>
        protected abstract void AfterInput();
        #endregion
    }
}
