using Baidu.Aip.Ocr;
using JM.BaiduApi.Config;
using JM.BaiduApi.ImageOrc;
using JM.BaiduApi.ImageOrc.Model;
using JM.BaiduApi.TextOrc.Model;
using JM.Common.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sniffer._12306.Model.Configs;
using Sniffer._12306.Model.Results;
using Sniffer.Core.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Sniffer._12306
{
    public class VerificationCodeTest
    {
        CookieContainer cookieContainer = new CookieContainer();
        string verificationCodePng = "f:\\files\\verificationCode.png";
        string verificationCodeGrayScalePng = "f:\\files\\verificationCodeGrayScale.png";
        string verificationCodeKeywordPng = "f:\\files\\verificationCode_keyword.png";
        string verificationCodePng1 = "f:\\files\\verificationCode_pic1.png";
        string verificationCodePng2 = "f:\\files\\verificationCode_pic2.png";
        string verificationCodePng3 = "f:\\files\\verificationCode_pic3.png";
        string verificationCodePng4 = "f:\\files\\verificationCode_pic4.png";
        string verificationCodePng5 = "f:\\files\\verificationCode_pic5.png";
        string verificationCodePng6 = "f:\\files\\verificationCode_pic6.png";
        string verificationCodePng7 = "f:\\files\\verificationCode_pic7.png";
        string verificationCodePng8 = "f:\\files\\verificationCode_pic8.png";
        /// <summary>
        /// 下载验证码图片
        /// </summary>
        public void DownloadVerificationCodeImage()
        {
            string verificationCodeUrl = "https://kyfw.12306.cn/passport/captcha/captcha-image64?login_site=E&module=login";
            string verificationCodeJson = HttpUtil.DoGet(verificationCodeUrl, cookieContainer);
            var verificationCodeResult = JsonConvert.DeserializeObject<VerificationCodeResult>(verificationCodeJson);

            FileUtil.Base64ToImageFile(verificationCodeResult.image, verificationCodePng);
            ImageUtil.RgbToGrayScale(verificationCodePng, verificationCodeGrayScalePng);
        }

        public string VerificationCodeKeyword()
        {
            BaiduConfig baiduTextConfig = ConfigUtil.LoadConfig<BaiduConfig>("appsettings.json", "BaiduTextConfig");
            BaiduConfig baiduImageConfig = ConfigUtil.LoadConfig<BaiduConfig>("appsettings.json", "BaiduImageConfig");
            _12306Config _12306Config = ConfigUtil.LoadConfig<_12306Config>("appsettings.json", "12306Config");

            ImageUtil.CaptureImage(verificationCodeGrayScalePng, 120, 0, verificationCodeKeywordPng, 170, 28);

            #region 获取验证码关键字
            
            var client = new Ocr(baiduTextConfig.ApiKey, baiduTextConfig.SecretKey);
            client.Timeout = 60000;
            var image = File.ReadAllBytes(verificationCodeGrayScalePng);
            var options = new Dictionary<string, object>{
                {"language_type", "CHN_ENG"},
                {"detect_direction", "true"},
                {"detect_language", "true"},
                {"probability", "true"}
            };
            // 调用通用文字识别, 图片参数为本地图片，可能会抛出网络等异常，请使用try/catch捕获
            var keywordJObj = client.GeneralBasic(image, options);
            BaiduResult baiduResult = keywordJObj.ToObject<BaiduResult>();
            Console.WriteLine(keywordJObj);
            string keyword = baiduResult.words_result.FirstOrDefault().words.TrimStart("请点击下图中所有的".ToArray());

            #endregion

            #region 将验证码切成八个图片
            ImageUtil.CaptureImage(verificationCodePng, 5, 40, verificationCodePng1, 67, 67);
            ImageUtil.CaptureImage(verificationCodePng, 76, 40, verificationCodePng2, 67, 67);
            ImageUtil.CaptureImage(verificationCodePng, 148, 40, verificationCodePng3, 67, 67);
            ImageUtil.CaptureImage(verificationCodePng, 224, 40, verificationCodePng4, 67, 67);

            ImageUtil.CaptureImage(verificationCodePng, 5, 115, verificationCodePng5, 67, 67);
            ImageUtil.CaptureImage(verificationCodePng, 76, 115, verificationCodePng6, 67, 67);
            ImageUtil.CaptureImage(verificationCodePng, 148, 115, verificationCodePng7, 67, 67);
            ImageUtil.CaptureImage(verificationCodePng, 224, 115, verificationCodePng8, 67, 67);
            #endregion

            var access_token = AccessToken.getAccessToken(baiduImageConfig);
            
            if(AdvancedGeneral.advancedGeneral(access_token.access_token, verificationCodePng1, keyword))
            {
                Console.WriteLine($"图片1命中 {keyword}");
            }
            if (AdvancedGeneral.advancedGeneral(access_token.access_token, verificationCodePng2, keyword))
            {
                Console.WriteLine($"图片2命中 {keyword}");
            }
            if (AdvancedGeneral.advancedGeneral(access_token.access_token, verificationCodePng3, keyword))
            {
                Console.WriteLine($"图片3命中 {keyword}");
            }
            if (AdvancedGeneral.advancedGeneral(access_token.access_token, verificationCodePng4, keyword))
            {
                Console.WriteLine($"图片4命中 {keyword}");
            }
            if (AdvancedGeneral.advancedGeneral(access_token.access_token, verificationCodePng5, keyword))
            {
                Console.WriteLine($"图片5命中 {keyword}");
            }
            if (AdvancedGeneral.advancedGeneral(access_token.access_token, verificationCodePng6, keyword))
            {
                Console.WriteLine($"图片6命中 {keyword}");
            }
            if (AdvancedGeneral.advancedGeneral(access_token.access_token, verificationCodePng6, keyword))
            {
                Console.WriteLine($"图片7命中 {keyword}");
            }
            if (AdvancedGeneral.advancedGeneral(access_token.access_token, verificationCodePng7, keyword))
            {
                Console.WriteLine($"图片8命中 {keyword}");
            }
            return null;
        }

        //public bool Hit(string access_token,string filePath,string keyword)
        //{
        //    var advancedGeneralResult = AdvancedGeneral.advancedGeneral(access_token, filePath);
        //    return advancedGeneralResult.result.Any(x => { return x.root.Contains(keyword) || x.keyword.Contains(keyword); });
        //}
    }
}
