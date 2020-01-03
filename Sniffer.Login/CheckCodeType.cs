using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sniffer.Login
{
    public enum CheckCodeType
    {
        /// <summary>
        /// 无验证码
        /// </summary>
        None,
        /// <summary>
        /// 随机图片验证
        /// </summary>
        Image,
        /// <summary>
        /// 滑动验证
        /// </summary>
        Slide
    }
}
