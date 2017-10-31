using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.Utility;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Main.Infrastrcture;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace _9M.Work.WPF_Main.Views.QualityCheck
{
    /// <summary>
    /// BunchWare.xaml 的交互逻辑
    /// </summary>
    public partial class BunchWare : UserControl, BaseDialog
    {
        private BaseDAL dal = new BaseDAL();
        private QualityBatchModel model;
        public BunchWare(QualityBatchModel model)
        {
            InitializeComponent();
            this.model = model;
            this.DataContext = this;
            BindCheckListBox(8);
        }

        public void BindCheckListBox(int RowCount)
        {
            check_panel.Children.Clear();
            List<UserInfoModel> userlist = dal.QueryList<UserInfoModel>(string.Format(@"select * from dbo.T_userinfo where DeptId = (select id from dbo.T_department where deptname = '质检部')"), new object[] { });
            int Count = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(userlist.Count) / RowCount));
            for (int i = 0; i < Count; i++)
            {
                StackPanel stack = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 10, 0, 0) };
                for (int j = 0; j < RowCount; j++)
                {
                    int Index = i * RowCount + j;
                    if (Index == userlist.Count)
                    {
                        break;
                    }
                    CheckBox check = new CheckBox() { Content = userlist[Index].UserName, Margin = new Thickness(15, 0, 0, 0), FontWeight = FontWeights.Bold };
                    stack.Children.Add(check);
                }
                check_panel.Children.Add(stack);
            }
        }
        private void Btn_CommandClick(object sender, RoutedEventArgs e)
        {
            //得到所有人数
            List<CheckBox> list = WPFControlsSearchHelper.GetChildObjects<CheckBox>(check_panel, "");
            List<string> Namelist = list.Where(x => x.IsChecked == true).Select(x => x.Content.ToString()).ToList();
            if (Namelist.Count == 0)
            {
                CCMessageBox.Show("请选中名字");
                return;
            }

            //得到所有件数
            string Sql = string.Format(@"select a.*,b.InSideGroupId from  (SELECT  wareno, SUM(stock) as stock from dbo.T_WareSpecList where WareNo in (select WareNo from T_WareList where Batchid = {0})
            group by WareNo) a join   dbo.T_WareList b on a.WareNo = b.WareNo
            order by b.InSideGroupId", model.Id);
            List<BunchWareModel> warelist = dal.QueryList<BunchWareModel>(Sql, new object[] { });
            int Sum = warelist.Sum(x => x.Stock);
            //得到平均数
            int Avg = Sum / Namelist.Count;
            int startindex = 0;
            DataTable dt = new DataTable();
            dt.Columns.Add("员工", typeof(string));
            dt.Columns.Add("货位", typeof(string));
            dt.Columns.Add("总数", typeof(int));
            //开始分配
            for (int j = 0; j < Namelist.Count; j++)
            {
                bool b = false;
                int Flag = 0;
                string Pos = string.Empty;
                for (int i = startindex; i < warelist.Count; i++)
                {
                    if (warelist[i].Stock <= 0 && i != warelist.Count-1)
                    {
                        continue;
                    }
                    Flag += warelist[i].Stock;
                    Pos += warelist[i].InSideGroupId.ToString() + ",";
                    if (Flag >= Avg || i == warelist.Count - 1)
                    {
                        b = true;
                        startindex = i+1;
                        break;
                    }
                }
                if(b==true)
                {
                    DataRow dr = dt.NewRow();
                    dr["员工"] = Namelist[j];
                    dr["货位"] = Pos.TrimEnd(',');
                    dr["总数"] = Flag;
                    dt.Rows.Add(dr);
                    Flag = 0;
                    Pos = string.Empty;
                    
                }
            }
            string ExcelPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\分配"+System.Guid.NewGuid()+".xls";
            ExcelNpoi.TableToExcelForXLS(dt,ExcelPath);
            CCMessageBox.Show("己成功生成在桌面");
        }

        public Microsoft.Practices.Prism.Commands.DelegateCommand CancelCommand
        {
            get { return new DelegateCommand(CloseDialog); }
        }

        public void CloseDialog()
        {
            FormInit.CloseDialog(this);
        }

        public string Title
        {
            get { return "打捆分配"; }
        }

        public bool IsNavigationTarget(Microsoft.Practices.Prism.Regions.NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedFrom(Microsoft.Practices.Prism.Regions.NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedTo(Microsoft.Practices.Prism.Regions.NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }
    }
}
