using Microsoft.Extensions.Configuration;

namespace JM.Common.Utils
{
    public class ConfigUtil
    {
        /// <summary>
        /// 从 json 中获取配置文档
        /// </summary>
        /// <typeparam name="T">配置的类型</typeparam>
        /// <param name="jsonPath">json配置文档相对路径</param>
        /// <param name="section">The key of the configuration section</param>
        /// <returns></returns>
        public static T LoadConfig<T>(string jsonPath,string section)
            where T : new()
        {
            T config = new T();
            var builder = new ConfigurationBuilder().AddJsonFile(jsonPath);
            var configuration = builder.Build();
            configuration.GetSection(section).Bind(config);
            return config;
        }
    }
}
