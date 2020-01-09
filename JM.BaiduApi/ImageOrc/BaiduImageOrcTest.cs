using JM.BaiduApi.Config;
using JM.BaiduApi.ImageOrc.Model;
using Newtonsoft.Json;

namespace JM.BaiduApi.ImageOrc
{
    public class BaiduImageOrcTest
    {
        public void Test()
        {
            //去配置文档读取配置
            BaiduConfig baiduConfig = new BaiduConfig() {
                ApiKey= "GFGZBfK9NzXe7DxZKl9A2Mb3",
                SecretKey= "qmsRizCCGVr6iiaBqITZuMrjLF3tYhMk"
            };

            var token = AccessToken.getAccessToken(baiduConfig);
            var advancedGeneralResult = AdvancedGeneral.advancedGeneral(token.access_token, "f:\\files\\hcp.png");
        }
    }
}
