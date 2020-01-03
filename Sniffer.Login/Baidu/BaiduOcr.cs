using Baidu.Aip.Ocr;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sniffer.Login.Baidu
{
    public class BaiduOcr
    {
        Ocr client = null;

        public string BaiduApiKey { get; set; }
        public string BaiduSecretKey { get; set; }

        public BaiduOcr()
        {
            client = new Ocr(BaiduApiKey, BaiduSecretKey);
            client.Timeout = 60000;
        }
        public BaiduOcr(string baiduApiKey, string baiduSecretKey) : base()
        {
            BaiduApiKey = baiduApiKey;
            BaiduSecretKey = baiduSecretKey;
        }

        public BaiduResult OcrResult(byte[] image, Dictionary<string, object> options = null)
        {
            try
            {
                // 调用通用文字识别, 图片参数为本地图片，可能会抛出网络等异常，请使用try/catch捕获
                var result = client.GeneralBasic(image);

                Console.WriteLine(result);
                return result.ToObject<BaiduResult>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public string OcrSingleResult(byte[] image, Dictionary<string, object> options = null)
        {
            var result = OcrResult(image, options);
            if (result == null || result.words_result_num <= 0)
            {
                return string.Empty;
            }
            else
            {
                return result.words_result.FirstOrDefault().words;
            }
        }

        public List<string> OcrMultipleResult(byte[] image, Dictionary<string, object> options = null)
        {
            var result = OcrResult(image, options);
            if (result == null)
            {
                return new List<string>();
            }
            else
            {
                return result.words_result.Select(x => x.words).ToList();
            }
        }
    }
}
