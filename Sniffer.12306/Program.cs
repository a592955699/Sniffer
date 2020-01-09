using JM.BaiduApi.ImageOrc;
using JM.BaiduApi.TextOrc;
using System;

namespace Sniffer._12306
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            VerificationCodeTest verificationCodeTest = new VerificationCodeTest();
            //下载验证码
            verificationCodeTest.DownloadVerificationCodeImage();

            verificationCodeTest.VerificationCodeKeyword();
        }
    }
}
