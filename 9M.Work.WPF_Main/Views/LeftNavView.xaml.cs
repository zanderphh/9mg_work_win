using _9M.Work.WPF_Main.Infrastrcture;
using _9M.Work.WPF_Main.Views.DataCenter;
using _9M.Work.WPF_Main.Views.EveryDayUpdate;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _9M.Work.WPF_Main.Views
{
    /// <summary>
    /// LeftNavView.xaml 的交互逻辑
    /// </summary>
    public partial class LeftNavView : UserControl
    {
        public LeftNavView()
        {
            InitializeComponent();
        }

        //打开窗体
        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock tb  =  sender as TextBlock;
            object obj =   FindForm(tb.Text);
          //  FormInit.OpenView(tb.Text, obj);
            
        }


        /// <summary>
        /// 导航
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> TreeDic()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("图片搜索",new ImageFind());
            dic.Add("上新代码", new EveryDayUpdateCode());
            dic.Add("数据导出", new DataExport());
            return dic;
        }

        public object FindForm(string Header)
        {
            Dictionary<string, object> dic = TreeDic();
            return dic.First(x => x.Key.Equals(Header)).Value;
        }
    }
}
