using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Visifire.Charts;

namespace _9M.Work.WPF_Main.FrameWork
{
    public class ChartHelper
    {
        /// <summary>
        /// 将一个图表渲染在BORDER容器中
        /// </summary>
        /// <param name="TuBiao">图表容器</param>
        /// <param name="name">图表标题</param>
        /// <param name="width">图表的宽度</param>
        /// <param name="height">图表的高度</param>
        /// <param name="Suffix">Y轴的单位</param>
        /// <param name="valuex">X轴的值</param>
        /// <param name="valuey">Y轴的值</param>
        public static void CreateChartColumn(Border TuBiao, string name, double width,double height, string Suffix, List<string> valuex, List<string> valuey)
        {
            //创建一个图标
            Chart chart = new Chart();
            //设置图标的宽度和高度
            chart.Width = width;
            chart.Height = height;
            //  chart.Margin = new Thickness(100, 5, 10, 5);
            //是否启用打印和保持图片
            chart.ToolBarEnabled = false;

            //设置图标的属性
            chart.ScrollingEnabled = false;//是否启用或禁用滚动
            chart.View3D = true;//3D效果显示

            //创建一个标题的对象
            Title title = new Title();
            title.FontWeight = FontWeights.Bold;
            title.FontSize = 40;
            //设置标题的名称
            title.Text = name;
            title.Padding = new Thickness(0, 10, 5, 0);

            //向图标添加标题
            chart.Titles.Add(title);

            Axis yAxis = new Axis();
            //设置图标中Y轴的最小值永远为0           
            yAxis.AxisMinimum = 0;
            //设置图表中Y轴的后缀          
            yAxis.Suffix = Suffix;
            chart.AxesY.Add(yAxis);

            // 创建一个新的数据线。               
            DataSeries dataSeries = new DataSeries();
            dataSeries.LabelEnabled = true;
            //dataSeries.LabelText = "#AxisXLabel, #YValue";
            dataSeries.LabelText = "#YValue";
            dataSeries.LabelFontWeight = FontWeight.FromOpenTypeWeight(20);
            dataSeries.LabelFontSize = 12;

            // 设置数据线的格式
            dataSeries.RenderAs = RenderAs.StackedColumn;//柱状Stacked


            // 设置数据点              
            DataPoint dataPoint;
            for (int i = 0; i < valuex.Count; i++)
            {
                // 创建一个数据点的实例。                   
                dataPoint = new DataPoint();
                // 设置X轴点                    
                dataPoint.AxisXLabel = valuex[i];
                //设置Y轴点                   
                dataPoint.YValue = double.Parse(valuey[i]);
                //添加一个点击事件        
                //  dataPoint.MouseLeftButtonDown += new MouseButtonEventHandler(dataPoint_MouseLeftButtonDown);
                //添加数据点                   
                dataSeries.DataPoints.Add(dataPoint);
            }

            // 添加数据线到数据序列。                
            chart.Series.Add(dataSeries);

            //将生产的图表增加到Grid，然后通过Grid添加到上层Grid.           
            Grid gr = new Grid();
            gr.Children.Add(chart);
            TuBiao.Child = gr;
        }
    }
}
