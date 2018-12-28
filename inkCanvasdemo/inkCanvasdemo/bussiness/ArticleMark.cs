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
    /// 标记实例所需的参数
    /// </summary>
    public class ArticleMark
    {
        /// <summary>
        /// 标记起始位置
        /// </summary>
        public Point start_location;

        /// <summary>
        /// 标记结束位置
        /// </summary>
        public Point end_location;

        /// <summary>
        /// 标记编号
        /// </summary>
        public string mark_num; 

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="start_location">标记起始位置</param>
        /// <param name="end_location">标记结束位置</param>
        public ArticleMark(string mark_num, Point start_location, Point end_location)
        {
            this.mark_num = mark_num;
            this.start_location = start_location;
            this.end_location = end_location;
        }
    }
}
