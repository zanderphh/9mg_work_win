using _9M.Work.DbObject;
using _9M.Work.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
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

namespace _9M.Work.WPF_Main.Views.Activity
{
    /// <summary>
    /// Interaction logic for DmMarketingSale.xaml
    /// </summary>
    public partial class DmMarketingSale : UserControl
    {

        BaseDAL dal = new BaseDAL();

        public DmMarketingSale()
        {
            InitializeComponent();
            this.DataContext = this;
            DataSourceBind();
            MsgInfoBind();
        }

        public void MsgInfoBind()
        {
            string sql = @" select COUNT(*) as total from T_FxDM where isGrant=1
                            union all
                            select COUNT(*) as total from T_FxDM where isUse=1";

            DataTable dt = dal.QueryDataTable(sql, new object[] { });
            if (dt.Rows.Count > 0)
            {
                string txt = string.Format("当前投入暗号{0}个,使用{1}个,使用率{2}%", dt.Rows[0][0].ToString(), dt.Rows[1][0].ToString(), Convert.ToDouble(dt.Rows[1][0]) / Convert.ToDouble(dt.Rows[0][0]) * 100);
                txtMsgInfo.Content = txt;
            }
        }

        public void DataSourceBind()
        {

            ObservableCollection<DataSourceModel> ObserCollection = new ObservableCollection<DataSourceModel>();
            DMGridlist.ItemsSource = ObserCollection;

            string sql = string.Format(@"
                                        with tab as
                                        (
                                        select distributor,COUNT(*) useCount from dbo.T_FxDM where isuse=1 group by distributor
                                        ),
                                        tab1 as 
                                        (
                                        select distributor,COUNT(*) endCount,SUM(money) endTotal  from dbo.T_FxDM where isEnd=1 group by distributor 
                                        ),
                                        tab2 as
                                        (
                                        select distributor,sum(money) wattingEndTotal from dbo.T_FxDM where isuse=1 and isend=0 group by distributor 
                                        )

                                        select tab.*,ISNULL(tab1.endCount,0) endCount,ISNULL(tab1.endTotal,0) endTotal,ISNULL(tab2.wattingEndTotal,0) wattingEndTotal  from tab 
                                        left join tab1 on tab.distributor=tab1.distributor
                                        left join tab2 on tab.distributor=tab2.distributor");

            List<DataSourceModel> models = dal.QueryList<DataSourceModel>(sql, new object[] { });

            foreach (DataSourceModel m in models)
            {
                ObserCollection.Add(m);
            }


        }

        private void btn_end(object sender, RoutedEventArgs e)
        {
            DataSourceModel selectedrow = DMGridlist.SelectedItem as DataSourceModel;
            if (selectedrow != null)
            {
                if (MessageBox.Show("确认返款", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    string sql = string.Format("update InSideWorkServer.dbo.T_FxDM set isEnd=1,endTime=GETDATE() where distributor='{0}' and isUse=1 and isGrant=1", selectedrow.distributor);
                    int row = dal.ExecuteSql(sql);
                    if (row > 0)
                    {
                        MessageBox.Show(string.Format("返款数量为{0}", row));
                        DataSourceBind();
                    }
                    else
                    {
                        MessageBox.Show("返款数量为0");
                    }
                }

            }
        }

        private void btn_OpenWindow(object sender, RoutedEventArgs e)
        {
            _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new DMCreate(), false);
        }
    }



    public class DataSourceModel
    {
        public string distributor { get; set; }
        public int useCount { get; set; }
        public int endCount { get; set; }
        public decimal endTotal { get; set; }
        public decimal wattingEndTotal { get; set; }

    }
}
