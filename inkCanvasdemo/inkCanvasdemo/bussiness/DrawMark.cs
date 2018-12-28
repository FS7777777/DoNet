using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace inkCanvasdemo.bussiness
{
    /// <summary>
    /// 图像法标记 基于使用RenderTargetBitmap和DrawingVisual 方式
    /// </summary>
    public class DrawMark
    {
        /// <summary>
        /// 重新绘制全部标记绘制标记
        /// </summary>
        /// <param name="originalImage">原始图像信息</param>
        /// <param name="articleMarks">历史标记集合</param>
        /// <returns></returns>
        public static RenderTargetBitmap reDrawMarks(BitmapSource originalImage,List<ArticleMark> articleMarks)
        {
            //创建DrawingVisual类，生成可绘制图像实例DrawingContext
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            //将原始图片绘制到visual对象中
            Rect rc = new Rect(0, 0, originalImage.Width, originalImage.Height);
            drawingContext.DrawImage(originalImage, rc);
            //重汇标记
            foreach (ArticleMark mark in articleMarks)
            {
                //定义画笔
                Pen pen = new Pen(Brushes.Red, 2);
                drawingContext.DrawLine(pen, mark.start_location, mark.end_location);

                //画矩形
                Rect rt = new Rect(mark.end_location, new Size(40, 40));
                drawingContext.DrawRectangle(null, pen, rt);

                //画标记编号
                FormattedText formattedText = new FormattedText(mark.mark_num, new CultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(new FontFamily(), FontStyles.Normal, FontWeights.Normal, new FontStretch()), 40, Brushes.Red);
                drawingContext.DrawText(formattedText, mark.end_location);
            }
            drawingContext.Close();
            RenderTargetBitmap bmp = new RenderTargetBitmap((int)originalImage.PixelWidth, (int)originalImage.PixelHeight, originalImage.DpiX, originalImage.DpiY, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);
            return bmp;
        }
        /// <summary>
        /// 绘制标记,在已有标记图片的基础上新增一条标记
        /// </summary>
        /// <param name="imageSource">当前标记图像</param>
        /// <param name="articleMark">标记信息</param>
        /// <returns></returns>
        public static RenderTargetBitmap drawMarks(BitmapSource imageSource, ArticleMark articleMark)
        {
            //创建DrawingVisual类，生成可绘制图像实例DrawingContext
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            //将原始图片绘制到visual对象中
            Rect rc = new Rect(0, 0, imageSource.Width, imageSource.Height);
            drawingContext.DrawImage(imageSource, rc);

            //定义画笔
            Pen pen = new Pen(Brushes.Red, 2);
            drawingContext.DrawLine(pen, articleMark.start_location, articleMark.end_location);

            //画矩形
            Rect rt = new Rect(articleMark.end_location, new Size(40, 40));
            drawingContext.DrawRectangle(null, pen, rt);

            //画标记编号
            FormattedText formattedText = new FormattedText(articleMark.mark_num, new CultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(new FontFamily(), FontStyles.Normal, FontWeights.Normal, new FontStretch()), 40, Brushes.Red);
            drawingContext.DrawText(formattedText, articleMark.end_location);
            
            drawingContext.Close();
            RenderTargetBitmap bmp = new RenderTargetBitmap((int)imageSource.PixelWidth, (int)imageSource.PixelHeight, imageSource.DpiX, imageSource.DpiY, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);
            return bmp;
        }
    }
}
