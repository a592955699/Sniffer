using BaiduApi.Core.Config;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace BaiduApi.Core.ImageOrc
{
    public static class AccessToken
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config">百度云中开通对应服务应用的配置</param>
        /// <returns></returns>
        public static String getAccessToken(BaiduConfig config)
        {
            String authHost = "https://aip.baidubce.com/oauth/2.0/token";
            HttpClient client = new HttpClient();
            List<KeyValuePair<String, String>> paraList = new List<KeyValuePair<string, string>>();
            paraList.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            paraList.Add(new KeyValuePair<string, string>("client_id", config.ApiKey));
            paraList.Add(new KeyValuePair<string, string>("client_secret", config.SecretKey));

            HttpResponseMessage response = client.PostAsync(authHost, new FormUrlEncodedContent(paraList)).Result;
            String result = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(result);
            return result;
        }
    }
}
