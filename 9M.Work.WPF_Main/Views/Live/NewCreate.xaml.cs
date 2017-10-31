using _9M.Work.DbObject;
using _9M.Work.ErpApi;
using _9M.Work.Model;
using _9M.Work.TopApi;
using _9M.Work.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace _9M.Work.WPF_Main.Views.Live
{
    /// <summary>
    /// Interaction logic for NewCreate.xaml
    /// </summary>
    public partial class NewCreate : UserControl
    {
        BaseDAL dal = new BaseDAL();
        GoodsManager manager = new GoodsManager();
        public string LiveNo { get; set; }
        public NewCreate()
        {
            InitializeComponent();
            this.DataContext = this;
            txtScanGoodsNo.Focus();


            //获取直播编号
            string dateSN = DateTime.Now.ToString("yyMMdd");
            string sn = dal.QueryDataTable(string.Format("select COUNT(*)+1 as 'no' from dbo.T_Live where substring(liveId,1,6)='{0}'", dateSN)).Rows[0][0].ToString();
            LiveNo = dateSN + StringHelper.FillZero(sn, 4);

        }

        public NewCreate(LiveModel model)
        {

            InitializeComponent();
            this.DataContext = this;
            txtScanGoodsNo.Focus();


            LiveNo = model.liveId;
            date_start.Text = model.liveDate.ToString("yyyy-MM-dd");

            List<ExpressionModelField> whereField = new List<ExpressionModelField>();
            whereField.Add(new ExpressionModelField() { Name = "no", Value = model.liveId });
            List<LiveGoodsModel> list = dal.GetList<LiveGoodsModel>(whereField.ToArray(), new OrderModelField[] { new OrderModelField() { PropertyName = "serialNum", IsDesc = false } });
            foreach (LiveGoodsModel m in list)
            {
                liveGoodsCollection.Add(m);
            }
            DetailListDG.ItemsSource = liveGoodsCollection;


        }

        #region 属性

        private ObservableCollection<LiveGoodsModel> _liveGoodsCollection = new ObservableCollection<LiveGoodsModel>();

        public ObservableCollection<LiveGoodsModel> liveGoodsCollection
        {
            get { return _liveGoodsCollection; }
            set
            {
                if (_liveGoodsCollection != value)
                {
                    _liveGoodsCollection = value;
                    this.OnPropertyChanged("liveGoodsCollection");
                }
            }
        }

        #endregion

        private void txtScanGoodsNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtScanGoodsNo.Text.Trim() == "")
                {
                    MessageBox.Show("商品编号不能为空！", "警告");
                    return;
                }

                if (date_start.Text.Trim() == "")
                {
                    MessageBox.Show("直播日期不能为空！", "警告");
                    return;
                }

                LiveGoodsModel m = new LiveGoodsModel();
                string SKU = txtScanGoodsNo.Text.Trim();
                string G, S;
                GoodsHelper.SplitGoodsDetail(SKU, out G, out S);

                if (G.Length > 0)
                {

                    m.no = LiveNo;
                    m.goodsno = G;
                    m.sku = SKU;
                    int kuan = 0;

                    //获取规格颜色
                    List<SpecModel> speclist = manager.SpecList(G);

                    SpecModel sm = speclist.Find(a => a.GoodsNo.Equals(G) && a.SpecCode.Equals(S));
                    if (sm.SpecName != null)
                    {
                        m.specName = sm.SpecName;
                    }

                    //是否已存在相同的款
                    var coll = liveGoodsCollection.Where(a => a.goodsno.Equals(m.goodsno));

                    if (coll.Count() > 0)
                    {
                        m.serialNum = coll.First<LiveGoodsModel>().serialNum;
                        m.tbLink = coll.First<LiveGoodsModel>().tbLink;

                        MessageBox.Show("此款号商品已存在,序号为" + m.serialNum.ToString());
                    }
                    else
                    {
                        kuan = 1;

                        if (liveGoodsCollection.Count > 0)
                        {
                            m.serialNum = liveGoodsCollection.Max(a => a.serialNum) + 1;
                        }
                        else
                        {
                            m.serialNum = 1;
                        }

                        TopSource com = new TopSource();
                        List<string> outeridlist = new List<string>();
                        outeridlist.Add(G);
                        var info = com.GetItemList(outeridlist, null);

                        if (info.Count > 0)
                        {
                            if (info[0].NumIid > 0)
                            {
                                m.tbLink = "https://item.taobao.com/item.htm?id=" + info[0].NumIid;
                                //System.Diagnostics.Process.Start("https://item.taobao.com/item.htm?id=" + info[0].NumIid);
                            }
                        }
                    }



                    List<string> sql = new List<string>();
                    sql.Add(string.Format(@"if not exists(select id from dbo.T_Live where liveid='{0}')
                                        begin
                                         insert into dbo.T_Live(liveId,liveDate,kuanNumber,number) values('{0}','{1}',1,1)
                                        end
                                        else
                                        begin
                                          update dbo.T_Live set kuanNumber=kuanNumber+{2},number=number+1 where liveid='{0}'
                                        end", LiveNo, date_start.Text.Trim(), kuan));


                    sql.Add(string.Format("insert into T_LiveGoods(serialNum,no,goodsno,sku,specName,tbLink,remark,isStop) values({0},'{1}','{2}','{3}','{4}','{5}','{6}','7')",
                        m.serialNum, m.no, m.goodsno, m.sku, m.specName, m.tbLink, m.remark, m.isStop));


                    if (dal.ExecuteTransaction(sql, null))
                    {
                        liveGoodsCollection.Add(m);
                        var result = liveGoodsCollection.OrderBy(a => a.serialNum);
                        DetailListDG.ItemsSource = result;
                        txtScanGoodsNo.Text = "";
                        txtScanGoodsNo.Focus();
                    }
                    else
                    {
                        MessageBox.Show("本条数保存异常,数据已回滚,请重新扫描", "提示");
                    }
                }
                else
                {

                }
            }
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(((TextBlock)sender).Text);
        }

        private void btn_delClick(object sender, RoutedEventArgs e)
        {
            LiveGoodsModel item = DetailListDG.CurrentItem as LiveGoodsModel;

            if (item != null)
            {
                if (MessageBox.Show("是否继续删除?", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    liveGoodsCollection.Remove(item);


                    List<string> sql = new List<string>();
                    sql.Add(string.Format(@"update dbo.T_Live set kuanNumber=kuanNumber-1,number=number-1 where liveid='{0}'", item.no));


                    sql.Add(string.Format("delete T_LiveGoods where sku='{0}' and no='{1}'", item.sku, item.no));


                    if (dal.ExecuteTransaction(sql, null))
                    {
                        var result = liveGoodsCollection.OrderBy(a => a.serialNum);
                        DetailListDG.ItemsSource = result;
                        txtScanGoodsNo.Text = "";
                        txtScanGoodsNo.Focus();
                    }
                    else
                    {
                        MessageBox.Show("本条数保存异常,数据已回滚", "提示");
                    }


                }
            }
        }
    }
}
