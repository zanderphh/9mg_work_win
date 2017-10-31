using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.Utility;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Common.WpfBind;
using Microsoft.Win32;
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
    /// WareMeasure.xaml 的交互逻辑
    /// </summary>
    public partial class WareMeasure : UserControl
    {
        private BaseDAL dal = new BaseDAL();
        //尺码偏移（逻辑选规格）
        private int Deviation = 2;
        private int UserId = CommonLogin.CommonUser.Id;
        //绩效的模式（测量）
        private int Performanceid = Convert.ToInt32(Performance.Measure);
        public WareMeasure()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            List<QualityBatchModel> qualist = dal.GetAll<QualityBatchModel>(new OrderModelField[] { new OrderModelField() { IsDesc = true, PropertyName = "Id" } });
            ComboBoxBind.BindComboBox(Com_Batch, qualist, "BatchName", "Id");
            if (qualist.Count > 0)
            {
                Com_Batch.SelectedIndex = 0;
            }

            //如果没有绩效信息。那就加入一条
            string checkSql = string.Format(@"  if not exists
  (select * from T_Performance
  where  CONVERT(varchar(100), dates, 111) = CONVERT(varchar(100), GETDATE(), 111)
  and userId = {0} and Performance = {1})
  insert into T_Performance values ({2},CONVERT(varchar(100), GETDATE(), 111)  ,0,{3})", UserId, Performanceid, UserId, Performanceid);
            List<string> sqllist = new List<string>();
            sqllist.Add(checkSql);
            bool bs = dal.ExecuteTransaction(sqllist, null);
            if (!bs)
            {
                CCMessageBox.Show("绩效初始化失败,请及时与管理员联系");
                return;
            }
        }

        public void BindList()
        {
            QualityBatchModel model = Com_Batch.SelectedItem as QualityBatchModel;
            //排序选款
            List<SizeGroup> groupList = new List<SizeGroup>();
            groupList.Add(new SizeGroup() { Size = "XS", Group = 1 });
            groupList.Add(new SizeGroup() { Size = "S", Group = 2 });
            groupList.Add(new SizeGroup() { Size = "M", Group = 3 });
            groupList.Add(new SizeGroup() { Size = "L", Group = 4 });
            groupList.Add(new SizeGroup() { Size = "XL", Group = 5 });
            groupList.Add(new SizeGroup() { Size = "XXL", Group = 6 });
            groupList.Add(new SizeGroup() { Size = "XXXL", Group = 7 });
            List<WareMeasureModel> MeasureList = new List<WareMeasureModel>();
            //得到所有的有尺码的集合
            string Sql = string.Format(@"select c.BatchName,b.WareNo,b.WareName,b.InSideGroupId,a.Size from T_WareSpecList a 
                        join T_WareList b on a.WareNo = b.WareNo join T_QualityCheck_Batch c on b.Batchid = c.id
                        where a.Activity=1 and Batchid = {0} and b.Ismeasure =0", model.Id);
            List<WareMeasureModel> list = dal.QueryList<WareMeasureModel>(Sql, new object[] { });
            //得到款号
            List<string> WareList = list.GroupBy(x => x.WareNo).Select(x => x.Key).ToList();
            WareList.ForEach(x =>
            {
                //得到款号的所有规格
                List<WareMeasureModel> filerlist = list.FindAll(y =>
                {
                    return x.Equals(y.WareNo);
                }).GroupBy(s => s.Size).Select(g => g.First()).ToList();

                //得到所有的规格排序
                var query = from p in filerlist
                            from pp in groupList
                            where p.Size == pp.Size
                            orderby pp.Group
                            select pp.Group;


                List<int> sizelist = query.ToList();
                //获得逻辑间隔得到正确测量次数
                int count = 1;  //默认次数
                int Min = sizelist[0];
                int firstDev = Min + Deviation;
                for (int i = 0; i < sizelist.Count; i++)
                {
                    if (i != 0)
                    {
                        if (sizelist[i] > (firstDev))
                        {
                            count++;
                            //计算第二次的偏移比较
                            firstDev = count * (Deviation + 1);
                        }
                    }
                }
                string AllSize = string.Empty;

                filerlist.ForEach(z =>
                {
                    AllSize += z.Size + "    ";
                });

                WareMeasureModel wmm = new WareMeasureModel();
                var temp = filerlist[0];
                wmm.IsChecked = false;
                wmm.BatchName = temp.BatchName;
                wmm.InSideGroupId = temp.InSideGroupId;
                wmm.WareName = temp.WareName;
                wmm.WareNo = temp.WareNo;
                wmm.Weight = count;
                wmm.Size = AllSize.Trim();
                MeasureList.Add(wmm);
            });
            MeasureList = MeasureList.OrderByDescending(x => x.Weight).ToList();
            WareGrid.ItemsSource = MeasureList;
        }


        private void Btn_CommandClick(object sender, RoutedEventArgs e)
        {
            int Tag = Convert.ToInt32((sender as Button).Tag);
            switch (Tag)
            {
                case 1: // 查询
                    QualityBatchModel model = Com_Batch.SelectedItem as QualityBatchModel;
                    if (model != null)
                    {
                        BindList();
                    }
                    else
                    {
                        CCMessageBox.Show("请选择批次");
                        return;
                    }
                    break;
                case 2:   //打单
                    //得到选中的集合
                    List<WareMeasureModel> Itemlist = (WareGrid.ItemsSource as List<WareMeasureModel>).Where(x => x.IsChecked == true).ToList();
                    if (Itemlist.Count == 0)
                    {
                        CCMessageBox.Show("请选中行");
                        return;
                    }
                    DataTable dt = ListToDataTable.ConvertToDataTable<WareMeasureModel>(Itemlist);
                    SaveFileDialog sf = new SaveFileDialog();
                    sf.Filter = "XLS|*.xls|XLSX|*.xlsx|TXT|*.txt";
                    if (sf.ShowDialog() == true)
                    {
                        ExcelNpoi.TableToExcelForXLS(dt, sf.FileName);
                        List<string> sqllist = new List<string>();
                        //插入绩效
                        sqllist.Add(string.Format(@"update T_Performance set finishcount = finishcount+{0} where CONVERT(varchar(100), dates, 111) = CONVERT(varchar(100), GETDATE(), 111)
                        and userId = {1} and Performance = {2}", Itemlist.Count, UserId, Performanceid));
                        //修改商品的测量状态
                        int Bathid = (Com_Batch.SelectedItem as QualityBatchModel).Id;
                        string WareNos = string.Empty;
                        Itemlist.ForEach(x => {
                            WareNos += x.WareNo + ",";
                        });
                        sqllist.Add(string.Format(@"update T_WareList set IsMeasure = 1 where batchid = {0} and WareNo in ('{1}')", Bathid, WareNos.TrimEnd(',').Replace(",","','")));
                        bool bs = dal.ExecuteTransaction(sqllist,null);
                        if (!bs)
                        {
                            CCMessageBox.Show("绩效失败,请与管理员联系");
                        }
                    }
                    //刷新
                    BindList();
                    break;
            }
        }


    }

    public class SizeGroup
    {
        public string Size { get; set; }
        public int Group { get; set; }
    }
}
