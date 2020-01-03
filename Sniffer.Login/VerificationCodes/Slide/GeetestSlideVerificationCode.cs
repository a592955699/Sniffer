using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using Sniffer.Login.Extensions;

namespace Sniffer.Login.VerificationCodes.Slide
{
    public class GeetestSlideVerificationCode : ISlideVerificationCode
    {
        #region 属性
        /// <summary>
        /// 拖动按钮
        /// </summary>
        private string _slidButton = "gt_slider_knob";
        /// <summary>
        /// 原始图层
        /// </summary>
        private string _originalMap = "gt_fullbg";
        /// <summary>
        /// 原始图加缺口背景图
        /// </summary>
        private string _newMap = "gt_bg";
        /// <summary>
        /// 缺口图层
        /// </summary>
        private string _sliceMap = "gt_slice";
        /// <summary>
        /// 重试次数
        /// </summary>
        private int _tryTimes = 6;
        /// <summary>
        /// 缺口图默认偏移像素
        /// </summary>
        private int _leftOffset = 4;

        private string _fullScreenPath = AppDomain.CurrentDomain.BaseDirectory + "全屏.png";
        private string _originalMapPath = AppDomain.CurrentDomain.BaseDirectory + "原图.png";
        private string _newMapPath = AppDomain.CurrentDomain.BaseDirectory + "新图.png";
        #endregion

        public bool Pass(RemoteWebDriver remoteWebDriver)
        {
            int failTimes = 0;
            bool flag = false;
            do
            {
                //#TODO 检查图层是否正常弹出
                //截图
                Console.WriteLine("开始截图...");
                ScreenMap(remoteWebDriver);

                Console.WriteLine("开始计算距离...");
                //获取缺口图层位移距离
                var distance = GetDistance();

                //获取移动轨迹
                Console.WriteLine("开始获取移动轨迹...");
                var moveEntitys = GetMoveEntities(distance);

                //移动
                Console.WriteLine("开始移动...");
                Move(remoteWebDriver, moveEntitys);

                Console.WriteLine("休眠3秒,显示等待提交验证码...");
                Thread.Sleep(3000);

                Console.WriteLine("开始检查认证是否通过...");
                //检查移动是否成功
                flag = CheckSuccess(remoteWebDriver);
                if (flag)
                    break;
            } while (++failTimes < _tryTimes);
            return flag;
        }
        #region 内部方法
        protected  virtual bool CheckSuccess(RemoteWebDriver remoteWebDriver)
        {
            //WebDriverWait wait = new WebDriverWait(remoteWebDriver, TimeSpan.FromSeconds(5));
            //IWebElement gt_ajax_tip = null;
            //gt_ajax_tip = wait.Until<IWebElement>((d) =>
            //{
            //    try
            //    {
            //        return d.FindElement(By.CssSelector(".gt_holder .gt_ajax_tip.gt_success"));
            //    }
            //    catch (Exception ex)
            //    {
            //        return null;
            //    }
            //});
            //if (gt_ajax_tip == null)
            //{
            //    Console.WriteLine("验证失败,显示等待6秒刷新验证码...");
            //    Thread.Sleep(6000);
            //    return false;
            //}
            //else
            //{
            //    return true;
            //}

            var gt_slider_knob = remoteWebDriver.FindElementExt(By.ClassName(_slidButton), 10);
            if (gt_slider_knob == null)
            {
                return true;
            }
            else
            {
                Console.WriteLine("验证失败,显示等待6秒刷新验证码...");
                Thread.Sleep(6000);
                return false;
            }
        }
        private void Move(RemoteWebDriver remoteWebDriver,List<MoveEntity> moveEntities)
        {
            var slidButton = GetSlidButtonElement(remoteWebDriver);
            Actions builder = new Actions(remoteWebDriver);
            builder.ClickAndHold(slidButton).Perform();
            int offset = 0;
            int index = 0;
            foreach (var item in moveEntities)
            {
                index++;
                builder = new Actions(remoteWebDriver);
                builder.MoveByOffset(item.X, item.Y).Perform();
                //Console.WriteLine("向右总共移动了:" + (offset = offset + item.X));
                //if (offset != 0 && index != moveEntities.Count)
                //    Thread.Sleep(item.MillisecondsTimeout / offset);
            }
            builder.Release().Perform();
        }
        private List<MoveEntity> GetMoveEntities(int distance)
        {
            List<MoveEntity> moveEntities = new List<MoveEntity>();
            int allOffset = 0;
            do
            {
                int offset = 0;
                double offsetPercentage = allOffset / (double)distance;

                if (offsetPercentage > 0.5)
                {
                    if (offsetPercentage < 0.85)
                    {
                        offset = new Random().Next(10, 20);
                    }
                    else
                    {
                        offset = new Random().Next(2, 5);
                    }
                }
                else
                {
                    offset = new Random().Next(20, 30);
                }
                allOffset += offset;
                int y = (new Random().Next(0, 1) == 1 ? new Random().Next(0, 2) : 0 - new Random().Next(0, 2));
                moveEntities.Add(new MoveEntity(offset,y , offset));
            } while (allOffset <= distance + 5);

            //最后一部分移动
            var moveOver = allOffset > distance;
            for (int j = 0; j < Math.Abs(distance - allOffset);)
            {
                int step = 3;

                int offset = moveOver ? -step : step;
                int sleep = new Random().Next(100, 200);
                moveEntities.Add(new MoveEntity(offset,0, sleep)); ;
               
                j = j + step;
            }
            return moveEntities;
        }
        /// <summary>
        /// 比较两张图片的像素，确定阴影图片位置
        /// </summary>
        /// <param name="oldBmp"></param>
        /// <param name="newBmp"></param>
        /// <returns></returns>
        private int GetArgb(Bitmap oldBmp, Bitmap newBmp)
        {
            //由于阴影图片四个角存在黑点(矩形1*1) 
            for (int i = 0; i < newBmp.Width; i++)
            {

                for (int j = 0; j < newBmp.Height; j++)
                {
                    if ((i >= 0 && i <= 1) && ((j >= 0 && j <= 1) || (j >= (newBmp.Height - 2) && j <= (newBmp.Height - 1))))
                    {
                        continue;
                    }
                    if ((i >= (newBmp.Width - 2) && i <= (newBmp.Width - 1)) && ((j >= 0 && j <= 1) || (j >= (newBmp.Height - 2) && j <= (newBmp.Height - 1))))
                    {
                        continue;
                    }

                    //获取该点的像素的RGB的颜色
                    Color oldColor = oldBmp.GetPixel(i, j);
                    Color newColor = newBmp.GetPixel(i, j);
                    if (Math.Abs(oldColor.R - newColor.R) > 60 || Math.Abs(oldColor.G - newColor.G) > 60 || Math.Abs(oldColor.B - newColor.B) > 60)
                    {
                        return i;
                    }
                }
            }
            return 0;
        }
        /// <summary>
        /// 获取实际图层缺口实际距离
        /// </summary>
        /// <returns></returns>
        private int GetDistance()
        {
            using (Bitmap oldBitmap = (Bitmap)Image.FromFile(_originalMapPath))
            {
                using (Bitmap newBitmap = (Bitmap)Image.FromFile(_newMapPath))
                {
                    var distance = GetArgb(oldBitmap, newBitmap);
                    distance = distance - _leftOffset;
                    return distance;
                }
            }
        }
        /// <summary>
        /// 截图
        /// </summary>
        /// <param name="remoteWebDriver"></param>
        private void ScreenMap(RemoteWebDriver remoteWebDriver)
        {
            //显示原始图
            ShowOriginalMap(remoteWebDriver);
            //全屏截图
            FullScreen(remoteWebDriver);
            //获取原始图层
            var originalElement = GetOriginalElement(remoteWebDriver);
            //保存原始图
            CutBitmap(_fullScreenPath, _originalMapPath, originalElement);

            //显示新图层
            ShowNewMap(remoteWebDriver);
            //全屏截图
            FullScreen(remoteWebDriver);
            //获取新图层
            var newElement = GetNewMapElement(remoteWebDriver);
            //保存新图
            CutBitmap(_fullScreenPath, _newMapPath, newElement);
            //显示缺口图
            ShowSliceMap(remoteWebDriver);
        }
        /// <summary>
        /// 截图
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="targetPath"></param>
        /// <param name="webElement"></param>
        private void CutBitmap(string sourcePath, string targetPath, IWebElement webElement)
        {
            //获取原始图
            using (var bitmap = (Bitmap)Image.FromFile(sourcePath))
            {
                var newBitmap = bitmap.Clone(new Rectangle(webElement.Location, webElement.Size), System.Drawing.Imaging.PixelFormat.DontCare);
                newBitmap.Save(targetPath);
                newBitmap.Dispose();
                bitmap.Dispose();
            }
        }
        /// <summary>
        /// 全屏截图
        /// </summary>
        /// <param name="remoteWebDriver"></param>
        private void FullScreen(RemoteWebDriver remoteWebDriver)
        {
            remoteWebDriver.GetScreenshot().SaveAsFile(_fullScreenPath);
        }
        /// <summary>
        /// 获取原始图层元素
        /// </summary>
        /// <param name="remoteWebDriver"></param>
        /// <returns></returns>
        protected virtual IWebElement GetOriginalElement(RemoteWebDriver remoteWebDriver)
        {
            return remoteWebDriver.FindElementExt(By.ClassName(_originalMap), 10);
        }
        /// <summary>
        /// 获取原始图加缺口背景图元素
        /// </summary>
        /// <param name="remoteWebDriver"></param>
        /// <returns></returns>
        protected virtual IWebElement GetNewMapElement(RemoteWebDriver remoteWebDriver)
        {
            return remoteWebDriver.FindElementExt(By.ClassName(_newMap), 10);
        }
        /// <summary>
        /// 获取缺口图层元素
        /// </summary>
        /// <param name="remoteWebDriver"></param>
        /// <returns></returns>
        protected virtual IWebElement GetSliceMapElement(RemoteWebDriver remoteWebDriver)
        {
            return remoteWebDriver.FindElementExt(By.ClassName(_sliceMap), 10);
        }
        /// <summary>
        /// 获取拖动按钮元素
        /// </summary>
        /// <param name="remoteWebDriver"></param>
        /// <returns></returns>
        protected virtual IWebElement GetSlidButtonElement(RemoteWebDriver remoteWebDriver)
        {
            return remoteWebDriver.FindElementExt(By.ClassName(_slidButton), 10);
        }
        /// <summary>
        /// 显示原始图层
        /// </summary>
        /// <param name="remoteWebDriver"></param>
        protected virtual bool ShowOriginalMap(RemoteWebDriver remoteWebDriver)
        {
            remoteWebDriver.ExecuteScript("$('." + _newMap + "').hide();$('." + _originalMap + "').show();$('." + _sliceMap + "').hide();");
            Console.WriteLine("显示原始图");
            Thread.Sleep(100);

            //#TODO 判断JS执行后是否正确
            return true;
        }
        /// <summary>
        /// 显示原始图加缺口背景之后的图层
        /// </summary>
        /// <param name="remoteWebDriver"></param>
        /// <returns></returns>
        protected virtual bool ShowNewMap(RemoteWebDriver remoteWebDriver)
        {
            remoteWebDriver.ExecuteScript("$('." + _newMap + "').show();$('." + _originalMap + "').hide();$('." + _sliceMap + "').hide();");
            Console.WriteLine("显示原始图加缺口背景之后的图层");
            Thread.Sleep(100);

            //#TODO 判断JS执行后是否正确
            return true;
        }
        /// <summary>
        /// 显示缺口图
        /// </summary>
        /// <param name="remoteWebDriver"></param>
        /// <returns></returns>
        protected virtual bool ShowSliceMap(RemoteWebDriver remoteWebDriver)
        {
            remoteWebDriver.ExecuteScript("$('." + _sliceMap + "').show();");
            Console.WriteLine("显示原始图加缺口背景之后的图层");
            Thread.Sleep(100);

            //#TODO 判断JS执行后是否正确
            return true;
        }
        #endregion
    }
}
