using _9M.Work.AOP.Goods;
using _9M.Work.Model;
using _9M.Work.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Printing;
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
using _9M.Work.Model.Log;
using Microsoft.Practices.Unity;
using _9M.Work.WPF_Main.FrameWork;
using _9M.Work.Model.Report;
using _9M.Work.WPF_Common;

namespace _9M.Work.WPF_Main.ControlTemplate.PrintTemplate
{
    /// <summary>
    /// LabelTemplate.xaml 的交互逻辑
    /// </summary>
    public partial class LabelTemplate : UserControl, PrintInterface
    {
        PrintHelper ph = new PrintHelper();

        public PrintType PrintT
        {
            get;set;
        }
        #region AOP
        public void PrintLabe(PrintLabeModel x, int count, bool DoublePrint, IntercepGoodsLogModel LogModel)
        {
            List<PrintLabeModel> list = new List<PrintLabeModel>();
            for (int i = 0; i < count; i++)
            {
                list.Add(x);
            }
            ph.PrintWareLabel(list);
        }

        public void PrintLabe(List<PrintLabeModel> modellist, bool DoublePrint, IntercepGoodsLogModel LogModel)
        {
            ph.PrintWareLabel(modellist);
        }

        #endregion

        [InjectionConstructor]
        public LabelTemplate()
        {
            InitializeComponent();
            //初始化标签值
        }
    }
}
