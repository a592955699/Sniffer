using BaiduApi.Core.Config;
using BaiduApi.Core.ImageOrc.Model;
using Newtonsoft.Json;

namespace BaiduApi.Core.ImageOrc
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

            var tokenJson = AccessToken.getAccessToken(baiduConfig);
            var tokenResult = JsonConvert.DeserializeObject<AccessTokenResult>(tokenJson);
            string resultJson = AdvancedGeneral.advancedGeneral(tokenResult.access_token, "f:\\files\\hcp.png");
            var advancedGeneralResult = JsonConvert.DeserializeObject<AdvancedGeneralResult>(resultJson);
        }
    }
}
