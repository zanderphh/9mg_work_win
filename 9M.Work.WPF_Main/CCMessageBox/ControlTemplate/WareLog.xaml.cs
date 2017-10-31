using _9M.Work.DbObject;
using _9M.Work.Model;
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

namespace _9M.Work.WPF_Main.ControlTemplate
{
    /// <summary>
    /// WareLog.xaml 的交互逻辑
    /// </summary>
    public partial class WareLog : UserControl
    {
        private BaseDAL dal = new BaseDAL();
        public WareLog()
        {
            InitializeComponent();
        }

        public void BindLog(string WareNo)
        {
            List<WareLogModel> list = dal.GetList<WareLogModel>(x => x.WareNo.Equals(WareNo, StringComparison.CurrentCultureIgnoreCase));
            LogGrid.ItemsSource = list;
        }
    }
}
