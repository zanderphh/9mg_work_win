using _9M.Work.DbObject;
using _9M.Work.Utility;
using _9M.Work.WPF_Main.FrameWork;
using _9M.Work.WPF_Main.Infrastrcture;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;

namespace _9M.Work.WPF_Main.Views.DataCenter
{
    /// <summary>
    /// SetPrice.xaml 的交互逻辑
    /// </summary>
    public partial class SetPrice : UserControl
    {
        private BaseDAL dal = new BaseDAL();
        private ObservableCollection<WareBindModel> warelist;
        public SetPrice()
        {
            InitializeComponent();
            int SH = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            scrolls.Height = SH - 470;
            warelist = new ObservableCollection<WareBindModel>();
            UpdateGoodsGrid.ItemsSource = warelist;
            //让文本框得到焦点
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                 (Action)(() => { Keyboard.Focus(tb_Goodsno); }));
            Keyboard.Focus(tb_Goodsno);
        }

        public void GetBrandClassStock(string WareNo)
        {
            string sql = string.Format(@"select 0 as Id, a.WareNo,WareName,SUM(b.stock) as Stock,'0' as Price,'' as Remark from  dbo.T_WareList a 
            join dbo.T_WareSpecList b on a.WareNo = b.WareNo where a.WareNo = '{0}' group by a.WareNo,a.WareName", WareNo);
            WareBindModel model = dal.QuerySingle<WareBindModel>(sql, new object[] { });
            if (model != null)
            {
                WareBindModel mm = warelist.Where(x => x.WareNo.Equals(model.WareNo)).SingleOrDefault();
                if (mm == null)
                {
                    warelist.Add(model);
                    //往临时记录里面插入一条记录
                    string Insert = string.Format(@"if not exists (select wareno from T_SetPriceTemp where WareNo ='{5}') insert into T_SetPriceTemp values('{0}','{1}','{2}',{6},'{3}','{4}')", DateTime.Now, model.WareNo, model.WareName, "", "", model.WareNo, model.Stock);
                    dal.ExecuteSql(Insert, new object[] { });
                    //如果只剩下一条那么直接选中
                    if (warelist.Count == 1)
                    {
                        UpdateGoodsGrid.SelectedItem = model;
                    }
                }
                else
                {
                    CCMessageBox.Show("己存在的款");
                }
            }
            else
            {
                CCMessageBox.Show("不存在的款");
            }
            OnFocus();
        }

        private void tb_Goodsno_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string GoodsNo = tb_Goodsno.Text;
                if (!string.IsNullOrEmpty(GoodsNo))
                {
                    GetBrandClassStock(GoodsNo);
                }
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int Tag = Convert.ToInt32((sender as Button).Tag);
            switch (Tag)
            {
                case 0: //上一条
                    //得到当前的选中下标
                    MoveSelect(false);
                    break;
                case 1: //下一条
                    MoveSelect(true);
                    break;
                case 2: //确定

                    //得到价格
                    string Price = GetPrice();
                    if (string.IsNullOrEmpty(Price))
                    {
                        CCMessageBox.Show("请填写价格");
                        return;
                    }
                    List<string> updatesql = new List<string>();
                    int NextIndex = 0;
                    for (int i = 0; i < UpdateGoodsGrid.Items.Count; i++)
                    {
                        DataGridRow dr = ControlHelper.GetRow(UpdateGoodsGrid, i);
                        if (dr.IsSelected == true)
                        {
                            (ControlHelper.FindGridControl(UpdateGoodsGrid, 3, i) as TextBlock).Text = tb_remark.Text;
                            TextBlock tb = ControlHelper.FindGridControl(UpdateGoodsGrid, 4, i) as TextBlock;
                            tb.Foreground = new SolidColorBrush(Colors.Red);
                            tb.Text = Price;
                            //修改临时表里的记录
                            updatesql.Add(string.Format(@"UPDATE T_SetPriceTemp set Price ='{0}' ,Remark = '{1}' where WareNo = '{2}'", Price, tb_remark.Text, (ControlHelper.FindGridControl(UpdateGoodsGrid, 0, i) as TextBlock).Text));
                            NextIndex = i;
                        }
                    }
                    //选中接下来没有填价格的行
                    NextIndex = warelist.Count - 1 == NextIndex ? -1 : NextIndex + 1;
                    UpdateGoodsGrid.SelectedIndex = NextIndex;
                    dal.ExecuteTransaction(updatesql, null);
                    ClearAll();
                    tb_price1.Focus();
                    break;
            }
        }

        /// <summary>
        /// 得到价格
        /// </summary>
        /// <returns></returns>
        public string GetPrice()
        {
            string Price = string.Empty;
            string Price1 = tb_price1.Text;
            string Price2 = tb_price2.Text;
            if (string.IsNullOrEmpty(Price1) && string.IsNullOrEmpty(Price2))
            {
                Price = string.Empty;
            }
            else if (!string.IsNullOrEmpty(Price1) && string.IsNullOrEmpty(Price2))
            {
                Price = Price1;
            }
            else if (string.IsNullOrEmpty(Price1) && !string.IsNullOrEmpty(Price2))
            {
                Price = Price2;
            }
            else
            {
                Price = Price1 + "-" + Price2;
            }
            return Price;
        }
        /// <summary>
        /// 选中或取消行
        /// </summary>
        /// <param name="IsDown">TRUE为向下，FALSE向上</param>
        public void MoveSelect(bool IsDown)
        {
            bool AllSelect = false;
            //得到选中行的最后一行
            int LastSelectedIndex = 0;
            for (int i = 0; i < UpdateGoodsGrid.Items.Count; i++)
            {
                DataGridRow drs = ControlHelper.GetRow(UpdateGoodsGrid, i);
                if (drs.IsSelected == true)
                {
                    LastSelectedIndex = i;
                    AllSelect = true;
                }
            }
            int SetIndex = 0;

            if (IsDown)
            {
                SetIndex = LastSelectedIndex == warelist.Count - 1 ? LastSelectedIndex : LastSelectedIndex + 1;
            }
            else
            {
                SetIndex = LastSelectedIndex;
            }
            SetIndex = AllSelect == true ? SetIndex : 0;
            DataGridRow dr = ControlHelper.GetRow(UpdateGoodsGrid, SetIndex);
            dr.IsSelected = IsDown;

        }

        public void OnFocus()
        {
            tb_Goodsno.Clear();
            tb_Goodsno.Focus();
        }

        public void ClearAll()
        {
            tb_Goodsno.Clear();
            tb_price1.Clear();
            tb_price2.Clear();
            tb_remark.Clear();
        }

        //提交定价
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (warelist.Count > 0)
            {
                if (CCMessageBox.Show("您是否要提交修改", "提示", CCMessageBoxButton.YesNo) == CCMessageBoxResult.Yes)
                {
                    List<GoodsModel_Up> list = new List<GoodsModel_Up>();
                    List<string> sqllist = new List<string>();
                    //提交的时候清空临时记录
                    sqllist.Add("delete from T_SetPriceTemp");
                    DateTime dt = DateTime.Now;
                    foreach (var item in warelist)
                    {
                        GoodsModel_Up up = new GoodsModel_Up();
                        up.GoodsNo = item.WareNo;
                        string p = item.Price;
                        string[] arry = p.Split('-');
                        up.GoodsPrice = Convert.ToDecimal(arry[0]);
                        if (arry.Length > 1)
                        {
                            up.GoodsPrice2 = Convert.ToDecimal(arry[1]);
                        }
                        up.GoodsRemark = item.Remark;
                        list.Add(up);
                        string sql = string.Format("insert into T_SetPrice values('{0}','{1}','{2}','{3}','{4}')", dt, item.WareNo, item.WareName, item.Price, item.Remark);
                        sqllist.Add(sql);
                    }
                    //请求WebServices
                    UpdateGoodsServiceReference.GetGoodsServerSoapClient cliet = new UpdateGoodsServiceReference.GetGoodsServerSoapClient();
                    string Res = cliet.UpDatePrice("E045443B12BC", JsonConvert.SerializeObject(list));
                    Dictionary<string, string> dic = (Dictionary<string, string>)JsonConvert.DeserializeObject(Res, typeof(Dictionary<string, string>));
                    string code = dic["code"];
                    CCMessageBox.Show(dic["msg"]);
                    //写入提交记录
                    if (code.Equals("0"))
                    {
                        bool bs = dal.ExecuteTransaction(sqllist, null);
                        ClearData();
                        ClearAll();
                    }
                }
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            FormInit.OpenDialog(this, new SetPriceLog(), false);
        }

        private void Button_Click3(object sender, RoutedEventArgs e)
        {
            if (CCMessageBox.Show("您是否要清空记录", "提示", CCMessageBoxButton.YesNo) == CCMessageBoxResult.Yes)
            {
                dal.ExecuteSql("delete from T_SetPriceTemp", new object[] { });
                ClearData();
            }
        }

        public void ClearData()
        {
            warelist.Clear();
            UpdateGoodsGrid.ItemsSource = null;
            UpdateGoodsGrid.ItemsSource = warelist;
        }

        private void Button_Click4(object sender, RoutedEventArgs e)
        {
            if (CCMessageBox.Show("您是否要删除记录", "提示", CCMessageBoxButton.YesNo) == CCMessageBoxResult.Yes)
            {
                WareBindModel ms = (WareBindModel)UpdateGoodsGrid.SelectedItem;
                if (ms != null)
                {
                    warelist.Remove(ms);
                    //清空临时表里的记录
                    dal.ExecuteSql(string.Format(@"delete from T_SetPriceTemp where Wareno = '{0}'", ms.WareNo), new object[] { });
                    UpdateGoodsGrid.ItemsSource = null;
                    UpdateGoodsGrid.ItemsSource = warelist;
                }
                else
                {
                    CCMessageBox.Show("请选择要删除的行");
                }
            }
        }

        //加载临时记录
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            List<WareBindModel> mlist = dal.QueryList<WareBindModel>("select WareNo,WareName,stock, Price,Remark from T_SetPriceTemp", new object[] { });
            if (mlist.Count > 0)
            {
                if (CCMessageBox.Show("是否读取临时记录", "提示", CCMessageBoxButton.YesNo) == CCMessageBoxResult.Yes)
                {
                    ClearData();
                    mlist.ForEach(x =>
                    {
                        warelist.Add(x);
                    });
                }
            }
            else
            {
                CCMessageBox.Show("没有临时记录");
            }
        }
    }

    public class WareBindModel
    {
        public int Id { get; set; }
        public string WareNo { get; set; }
        public string WareName { get; set; }
        public int Stock { get; set; }
        public string Remark { get; set; }
        public string Price { get; set; }
    }

    public class GoodsModel_Up
    {
        public string GoodsNo { get; set; }
        public decimal GoodsPrice { get; set; }
        public decimal GoodsPrice2 { get; set; }
        public decimal GoodsPrice3 { get; set; }
        public string GoodsRemark { get; set; }
    }
}
