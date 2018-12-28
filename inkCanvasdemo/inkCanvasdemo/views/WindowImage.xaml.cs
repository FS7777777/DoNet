using inkCanvasdemo.bussiness;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace inkCanvasdemo.views
{
    /// <summary>
    /// WindowImage.xaml 的交互逻辑
    /// </summary>
    public partial class WindowImage : Window
    {
        public WindowImage()
        {
            InitializeComponent();
            pic_init();
        }

        // 本次移动开始时的坐标点位置-相对于父容器
        private Point StartPosition;
        //是否在移动
        private bool isMove = true;
        /// 原始图片
        private BitmapSource originalImage = null;
        /// 当前绘制图形
        RenderTargetBitmap bitmap = null;
        //鼠标点击图像位置
        Point image_position_start = new Point();
        //鼠标点击图像结束位置
        Point image_position_end = new Point();
        //当前图像标记数
        private int mark_num = 1;

        private List<ArticleMark> marks = new List<ArticleMark>();

        #region 图像缩放平移
        /// <summary>
        /// 图片缩放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void img_ScaleTransform(object sender, MouseWheelEventArgs e)
        {
            if (!isMove)
            {
                return;
            }
            Point centerPoint = e.GetPosition(this.root);
            this.sfr.CenterX = centerPoint.X;
            this.sfr.CenterY = centerPoint.Y;
            //防止鼠标滑动幅度太大导致图片缩小到看不见
            if ((sfr.ScaleX < 0.1 || sfr.ScaleY < 0.1) && e.Delta < 0)
            {
                return;
            }
            sfr.ScaleX += (double)e.Delta / 3500;
            sfr.ScaleY += (double)e.Delta / 3500;
        }

        /// <summary>
        /// 图片鼠标移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void img_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isMove)
            {
                return;
            }
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }
            var current = e.GetPosition(root);
            //将坐标都转换成image坐标系下的坐标
            Point point0 = root.TranslatePoint(current, (UIElement)img);
            Point point1 = root.TranslatePoint(StartPosition, (UIElement)img);
            translate.X += (point0.X - point1.X);
            translate.Y += (point0.Y - point1.Y);


            StartPosition = current;
        }
        /// <summary>
        /// 保存最原始图像信息
        /// </summary>
        private void pic_init()
        {
            //originalImage = new BitmapImage(new Uri(@"resource\images\big.jpg", UriKind.Relative));
            originalImage = this.img.Source as BitmapSource;
        }

        private void img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StartPosition = e.GetPosition(root);
            if (isMove)
            {
                return;
            }
            image_position_start = e.GetPosition((IInputElement)sender);
        }

        private void img_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isMove)
            {
                return;
            }
            image_position_end = e.GetPosition((IInputElement)sender);
            draw_article_marks();
        }
        #endregion


        #region 多点触控操作
        private void image_ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            if (!isMove)
            {
                return;
            }
            e.ManipulationContainer = root;
            e.Mode = ManipulationModes.All;
        }

        private void image_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (!isMove)
            {
                return;
            }
            FrameworkElement element = (FrameworkElement)e.Source;
            element.Opacity = 0.5;
            //Matrix matrix = ((TransformGroup)element.RenderTransform).get .Matrix;
            Matrix matrix = this.matr.Matrix;

            var deltaManipulation = e.DeltaManipulation;

            Point center = new Point(element.ActualWidth / 2, element.ActualHeight / 2);
            center = matrix.Transform(center);

            matrix.ScaleAt(deltaManipulation.Scale.X, deltaManipulation.Scale.Y, center.X, center.Y);

            matrix.Translate(e.DeltaManipulation.Translation.X, e.DeltaManipulation.Translation.Y);

            //((MatrixTransform)element.RenderTransform).Matrix = matrix;
            this.matr.Matrix = matrix;
        }

        private void image_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (!isMove)
            {
                return;
            }
            FrameworkElement element = (FrameworkElement)e.Source;
            element.Opacity = 1;
        }

        private void img_TouchDown(object sender, TouchEventArgs e)
        {
            if (isMove)
            {
                return;
            }
            TouchPoint tp = e.GetTouchPoint((IInputElement)sender);
            image_position_start = tp.Position;
        }

        private void img_TouchUp(object sender, TouchEventArgs e)
        {
            if (isMove)
            {
                return;
            }
            image_position_end = e.GetTouchPoint((IInputElement)sender).Position;
            //绘制标记
            draw_article_marks();
        }
        #endregion

        /// <summary>
        /// 开始执行标记操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void start_mark(object sender, RoutedEventArgs e)
        {
            isMove = false;
        }
        /// <summary>
        /// 取消标记开始进行图像缩放平移操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancel_mark(object sender, RoutedEventArgs e)
        {
            isMove = true;
        }
        /// <summary>
        /// 放大--基于当前画板中心点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void img_big(object sender, RoutedEventArgs e)
        {
            if (!isMove)
            {
                return;
            }
            Point centerPoint = new Point(this.root.Width / 2,this.root.Height/2);
            this.sfr.CenterX = centerPoint.X;
            this.sfr.CenterY = centerPoint.Y;
            //每次点击图片放大0.1
            sfr.ScaleX += 0.1;
            sfr.ScaleY += 0.1;
        }
        /// <summary>
        /// 缩小--基于当前画板中心点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void img_small(object sender, RoutedEventArgs e)
        {
            if (!isMove)
            {
                return;
            }
            Point centerPoint = new Point(this.root.Width / 2, this.root.Height / 2);
            this.sfr.CenterX = centerPoint.X;
            this.sfr.CenterY = centerPoint.Y;
            //防止缩小到一定比例图片会消失
            if (sfr.ScaleX <= 0.2 || sfr.ScaleY <= 0.2)
            {
                return;
            }
            sfr.ScaleX -= 0.1;
            sfr.ScaleY -= 0.1;
        }
        /// <summary>
        /// 图片复位操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reset_image(object sender, RoutedEventArgs e)
        {
            e.Handled = false;
            //鼠标操作产生的偏移
            //复原缩放比例
            this.sfr.ScaleX = 1;
            this.sfr.ScaleY = 1;
            //复原位移
            this.translate.X = 0;
            this.translate.Y = 0;
            //多点触控产生的偏移
            this.matr.Matrix = new Matrix(1,0,0,1,0,0);
        }
        /// <summary>
        /// 绘制图形
        /// </summary>
        private void draw_article_marks()
        {
            //标记数不足10前面补0
            string str_mark_num = mark_num.ToString();
            if (mark_num < 10)
            {
                str_mark_num = str_mark_num.PadLeft(2,'0');
            }
            //绘制标记
            ArticleMark articleMark = new ArticleMark(str_mark_num, image_position_start, image_position_end);
            marks.Add(articleMark);
            mark_num++;
            bitmap = DrawMark.drawMarks(this.img.Source as BitmapSource, articleMark);
            this.img.Source = bitmap;
        }
        /// <summary>
        /// 图像重绘。因为要满足删除已有的某条标记
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void re_draw_image(object sender, RoutedEventArgs e)
        {
            if (marks.Count == 0)
            {
                return;
            }
            marks.RemoveAt(new Random().Next(marks.Count));
            bitmap = DrawMark.reDrawMarks(originalImage as BitmapSource, marks);
            this.img.Source = bitmap;
        }
        /// <summary>
        /// 保存标记完成的图像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void save_image(object sender, RoutedEventArgs e)
        {
            if (bitmap == null)
            {
                return;
            }
            System.IO.FileStream fs = null;
            try
            {
                string path = "F://hello.png";
                PngBitmapEncoder encode = new PngBitmapEncoder();
                encode.Frames.Add(BitmapFrame.Create(bitmap));
                fs = System.IO.File.Open(path, System.IO.FileMode.OpenOrCreate);
                encode.Save(fs);
                fs.Close();
                //重置标记数
                mark_num = 1;
                MessageBox.Show("保存成功，图片路径："+ path);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }

        }

    }
}
