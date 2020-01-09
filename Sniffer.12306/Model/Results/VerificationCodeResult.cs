using System;
using System.Collections.Generic;
using System.Text;

namespace Sniffer._12306.Model.Results
{
    /// <summary>
    /// 获取验证码的结果model
    /// </summary>
    [Serializable]
    public class VerificationCodeResult
    {
        /// <summary>
        /// base64 验证码图片内容
        /// </summary>
        public string image { get; set; }
        /// <summary>
        /// 状态描述
        /// </summary>
        public string result_message { get; set; }
        /// <summary>
        /// 状态码
        /// </summary>
        public int result_code { get; set; }
    }
}
