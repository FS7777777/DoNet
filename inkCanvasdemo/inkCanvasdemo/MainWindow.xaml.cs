using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace inkCanvasdemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //更改 InkCanvas 对象的 EditingMode 属性，可以改变要在 InkCanvas 上执行的操作
        //该属性的值是一个 InkCanvasEditingMode 枚举，常见功能如下
        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as RadioButton).Content.ToString() == "绘制墨迹")
            {
                //绘制墨迹
                inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
            }

            else if ((sender as RadioButton).Content.ToString() == "按点擦除")
            {
                //使用橡皮擦按点擦除墨迹
                inkCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
            }

            else if ((sender as RadioButton).Content.ToString() == "按线擦除")
            {
                //使用橡皮擦按绘制的墨迹将某一笔墨迹擦除掉，注意是某一笔，如果多次绘制但是有交叉也是不可以的
                inkCanvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
            }

            else if ((sender as RadioButton).Content.ToString() == "选中墨迹")
            {
                //选中某一笔墨迹，进行拖动和缩放以及按 Delete 键删除，注意是某一笔
                inkCanvas.EditingMode = InkCanvasEditingMode.Select;
            }

            else if ((sender as RadioButton).Content.ToString() == "停止操作")
            {
                //不做任何
                inkCanvas.EditingMode = InkCanvasEditingMode.None;
            }
        }
    }
}
