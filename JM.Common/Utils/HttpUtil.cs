using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace Sniffer.Core.Helpers
{
    public class HttpUtil
    {
        public static String DoGet(String url)
        {
            return DoGet(url, Encoding.UTF8);
        }
        public static String DoGet(String url, Encoding encoding)
        {
            return DoGet(url,encoding,null);
        }
        public static String DoGet(string url, CookieContainer cookieContainer)
        {
            return DoGet(url, Encoding.UTF8, cookieContainer);
        }
        public static String DoGet(string url, Encoding encoding, CookieContainer cookieContainer)
        {
            var httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            httpWebRequest.Proxy = null;
            if (cookieContainer != null)
                httpWebRequest.CookieContainer = cookieContainer;
            httpWebRequest.ContentType = "text/html";
            httpWebRequest.Timeout = 1000 * 30;
            httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            //当证书出错时，可以跳过证书验证
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            httpWebRequest.ServicePoint.ConnectionLimit = int.MaxValue;
            httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.142 Safari/537.36";
            //httpWebRequest.AllowAutoRedirect = true;
            httpWebRequest.KeepAlive = false;
            return DoGet(httpWebRequest, encoding);
        }
        public static String DoGet(HttpWebRequest httpWebRequest, Encoding encoding)
        {
            httpWebRequest.Method = "GET";
            string result = string.Empty;
            
            using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var responseStream = httpWebResponse.GetResponseStream())
                {
                    //处理gzip格式的流
                    if (httpWebResponse.ContentEncoding != null && httpWebResponse.ContentEncoding.ToLower().Contains("gzip"))
                    {
                        var gzipStream = new GZipStream(responseStream, CompressionMode.Decompress);
                        using (StreamReader streamReader = new StreamReader(gzipStream, encoding))
                        {
                            result = streamReader.ReadToEnd();
                            streamReader.Close();
                        }
                    }
                    else
                    {
                        using (StreamReader streamReader = new StreamReader(responseStream, encoding))
                        {
                            result = streamReader.ReadToEnd();
                            streamReader.Close();
                        }
                    }
                    responseStream.Close();
                    return result;
                }
            }
           
        }

        public static String DoPost(String url, CookieContainer cookieContainer)
        {
            return DoPost(url, null, Encoding.UTF8, cookieContainer);
        }
        public static String DoPost(String url, String postData, CookieContainer cookieContainer)
        {
            return DoPost(url, postData, Encoding.UTF8, cookieContainer);
        }
        public static String DoPost(String url, String postData)
        {
            return DoPost(url, postData, Encoding.UTF8,null);
        }
        public static String DoPost(String url, String postData, Encoding encoding)
        {
            return DoPost(url, postData, encoding, null);
        }
        public static String DoPost(String url, String postData, Encoding encoding, CookieContainer cookieContainer)
        {
            var httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            httpWebRequest.Proxy = null;
            if (cookieContainer != null)
                httpWebRequest.CookieContainer = cookieContainer;
            httpWebRequest.ContentType = "text/html";
            httpWebRequest.Timeout = 1000 * 30;
            httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            //当证书出错时，可以跳过证书验证
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            httpWebRequest.ServicePoint.ConnectionLimit = int.MaxValue;
            httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.142 Safari/537.36";
            httpWebRequest.AllowAutoRedirect = true;
            httpWebRequest.KeepAlive = false;
            return DoPost(httpWebRequest, postData, encoding);
        }
        public static String DoPost(HttpWebRequest httpWebRequest, String postData, Encoding encoding)
        {
            httpWebRequest.Method = "POST";

            if(!String.IsNullOrWhiteSpace(postData))
            {
                byte[] byteArray = encoding.GetBytes(postData);
                httpWebRequest.ContentLength = byteArray.Length;
                using (Stream reqStream = httpWebRequest.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);
                }
            }
            string result = string.Empty;
            using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var responseStream = httpWebResponse.GetResponseStream())
                {
                    //处理gzip格式的流
                    if (httpWebResponse.ContentEncoding != null && httpWebResponse.ContentEncoding.ToLower().Contains("gzip"))
                    {
                        var gzipStream = new GZipStream(responseStream, CompressionMode.Decompress);
                        using (StreamReader streamReader = new StreamReader(gzipStream, encoding))
                        {
                            result = streamReader.ReadToEnd();
                            streamReader.Close();
                        }
                    }
                    else
                    {
                        using (StreamReader streamReader = new StreamReader(responseStream, encoding))
                        {
                            result = streamReader.ReadToEnd();
                            streamReader.Close();
                        }
                    }
                    responseStream.Close();
                    return result;
                }
            }
        }

        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="url">所下载的路径</param>
        /// <param name="path">本地保存的路径</param>
        /// <param name="overwrite">当本地路径存在同名文件时是否覆盖</param>
        /// <param name="callback">实时状态回掉函数</param>
        /// Action<文件名,文件的二进制, 文件大小, 当前已上传大小>
        public static void DownloadFile(string url, string path, bool overwrite, Action<string, string, byte[], long, long> callback = null)
        {
            // 设置参数
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //获取文件名
            string fileName = response.Headers["Content-Disposition"];//attachment;filename=FileName.txt
            string contentType = response.Headers["Content-Type"];//attachment;

            if (string.IsNullOrEmpty(fileName))
                fileName = response.ResponseUri.Segments[response.ResponseUri.Segments.Length - 1];
            else
                fileName = fileName.Remove(0, fileName.IndexOf("filename=") + 9);
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            using (Stream responseStream = response.GetResponseStream())
            {
                long totalLength = response.ContentLength;
                ///文件byte形式
                byte[] b = Encoding.Default.GetBytes(url);
                //创建本地文件写入流
                if (System.IO.File.Exists(Path.Combine(path, fileName)))
                {
                    fileName = DateTime.Now.Ticks + fileName;
                }
                using (Stream stream = new FileStream(Path.Combine(path, fileName), overwrite ? FileMode.Create : FileMode.CreateNew))
                {
                    byte[] bArr = new byte[1024];
                    int size;
                    while ((size = responseStream.Read(bArr, 0, bArr.Length)) > 0)
                    {
                        stream.Write(bArr, 0, size);
                        callback?.Invoke(fileName, contentType, b, totalLength, stream.Length);
                    }
                }
            }
        }

        /// <summary>
        /// 将cookie字符串添加到 CookieContainer
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="cc"></param>
        /// <param name="domian"></param>
        /// <returns></returns>
        public static CookieContainer AddCookieToContainer(string cookie, CookieContainer cc, string domian)
        {
            string[] tempCookies = cookie.Split(';');
            string tempCookie = null;
            int Equallength = 0;//  =的位置
            string cookieKey = null;
            string cookieValue = null;
            //qg.gome.com.cn  cookie
            for (int i = 0; i < tempCookies.Length; i++)
            {
                if (!string.IsNullOrEmpty(tempCookies[i]))
                {
                    tempCookie = tempCookies[i];
                    Equallength = tempCookie.IndexOf("=");

                    if (Equallength != -1)       //有可能cookie 无=，就直接一个cookiename；比如:a=3;ck;abc=;
                    {
                        cookieKey = tempCookie.Substring(0, Equallength).Trim();
                        //cookie=

                        if (Equallength == tempCookie.Length - 1)    //这种是等号后面无值，如：abc=;
                        {
                            cookieValue = "";
                        }
                        else
                        {
                            cookieValue = tempCookie.Substring(Equallength + 1, tempCookie.Length - Equallength - 1).Trim();
                        }
                    }
                    else
                    {
                        cookieKey = tempCookie.Trim();
                        cookieValue = "";
                    }
                    if (cookieValue.IndexOf(",") != -1)
                    {
                        cookieValue = cookieValue.Replace(",", "%2c");
                    }
                    cc.Add(new Cookie(cookieKey, cookieValue, "", domian));
                }
            }
            return cc;
        }
    }
}
