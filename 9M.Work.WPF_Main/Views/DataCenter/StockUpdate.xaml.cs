using _9M.Work.DbObject;
using _9M.Work.ErpApi;
using _9M.Work.Model;
using _9M.Work.TopApi;
using _9M.Work.WPF_Main.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
using Top.Api.Domain;

namespace _9M.Work.WPF_Main.Views.DataCenter
{
    /// <summary>
    /// StockUpdate.xaml 的交互逻辑
    /// </summary>
    public partial class StockUpdate : UserControl, INotifyPropertyChanged
    {

        List<NoPayModel> nopaylist = new List<NoPayModel>();
        BaseDAL dal = new BaseDAL();
        WdgjSource wdgj = new WdgjSource();
        TopSource top = null;
        List<ShopModel> shopList = null;
        //订单
        public delegate Dictionary<string, object> Handler(object[] parm);
        //查询库存

        public StockUpdate()
        {
            InitializeComponent();
            this.DataContext = this;
            DataList = new ObservableCollection<UpdateStockModel>();
            shopList = dal.GetAll<ShopModel>();
        }

        #region Property
        ObservableCollection<UpdateStockModel> _dataList;
        public ObservableCollection<UpdateStockModel> DataList
        {
            get
            {
                return this._dataList;
            }
            set
            {
                if (this._dataList != value)
                {
                    this._dataList = value;
                    OnPropertyChanged("DataList");
                }
            }
        }
        #endregion


        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        //加载未付款的订单
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            Handler handler = new Handler(InitTrade);
            handler.BeginInvoke(new object[] { }, TradeCallBack, null);
        }

        private void TradeCallBack(IAsyncResult ar)
        {
            Handler handler = (Handler)((AsyncResult)ar).AsyncDelegate;
            var dic = handler.EndInvoke(ar);
            string Msg = dic.First().Key;
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (string.IsNullOrEmpty(Msg.ToString()))
                {
                    CCMessageBox.Show("成功");
                }
                else
                {
                    CCMessageBox.Show("失败: " + Msg);
                }
            }));
        }

        public Dictionary<string, object> InitTrade(object[] parm)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            string Msg = string.Empty;
            bar.LoadBar(true);
            bar.Loading(true);
            try
            {
                nopaylist = new CommonTrade().GetNoPayList().Where(x => !string.IsNullOrEmpty(x.OuterSkuId)).ToList();
                if (nopaylist.Count == 0)
                {
                    Msg = "没有可用数据";
                }
            }
            catch (Exception ex)
            {
                Msg = ex.Message;
            }
            finally
            {
                bar.LoadBar(false);
                bar.Loading(false);
            }
            dic.Add(Msg, null);
            return dic;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var it = new TopSource().GetItemListDetail(new List<string>() { "542813278059", "541168447178" },"desc");
                string GoodsNo = tb_GoodsNo.Text.Trim();
                if (!string.IsNullOrEmpty(GoodsNo))
                {
                    DataList = new ObservableCollection<UpdateStockModel>();
                    //查询管家库存
                    var list = wdgj.SpecListByGoodsNo(GoodsNo);
                    list.ForEach(x =>
                    {
                        DataList.Add(new UpdateStockModel() { GoodsNo = x.GoodsNo, SpecCode = x.SpecCode, SpecName = x.SpecName, GjStock = x.Stock });
                    });
                    //组装数据
                    if (list.Count > 0)
                    {
                        if (Chk_C.IsChecked == true)
                        {
                            DataList = InstallSkuByGoodsNo(1000,false,GoodsNo,DataList);
                        }
                        if (Chk_FX.IsChecked == true)
                        {
                            DataList = InstallSkuByGoodsNo(1003, true, GoodsNo, DataList);
                        }
                        if (Chk_FD.IsChecked == true)
                        {
                            DataList = InstallSkuByGoodsNo(1030, false, GoodsNo, DataList);
                        }
                        if (Chk_TMALL.IsChecked == true)
                        {
                            DataList = InstallSkuByGoodsNo(1022, false, GoodsNo, DataList);
                        }
                        if (Chk_AN.IsChecked == true)
                        {
                            DataList = InstallSkuByGoodsNo(1027, false, GoodsNo, DataList);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 组装数据 （将获取的数据绑定到对向）
        /// </summary>
        /// <param name="ShopId"></param>
        /// <param name="IsFx"></param>
        /// <param name="GoodsNo"></param>
        /// <param name="datalist"></param>
        /// <returns></returns>
        public ObservableCollection<UpdateStockModel> InstallSkuByGoodsNo(int ShopId, bool IsFx, string GoodsNo, ObservableCollection<UpdateStockModel> datalist)
        {
            top = new TopSource(shopList.Where(x => x.shopId == ShopId).First());
            if (IsFx)
            {
                FenxiaoProduct pro = top.GetScItemByCode(GoodsNo, "skus");
                if (pro != null)
                {
                    List<FenxiaoSku> fxlist = pro.Skus;
                    foreach (var item in datalist)
                    {
                        FenxiaoSku sku = fxlist.Find(x =>
                        {
                            return x.OuterId.Equals(item.GoodsNo + item.SpecCode, StringComparison.CurrentCultureIgnoreCase);
                        });
                        if (sku != null)
                        {
                            item.StockFX = sku.Quantity;
                        }
                    }
                }
            }
            else
            {
                List<Sku> skulist = null;
                var Item = top.GetItemList(new List<string>() { GoodsNo }, "sku");
                if (Item.Count > 0)
                {
                    skulist = Item[0].Skus;
                    foreach (var item in datalist)
                    {
                        Sku sku = skulist.Find(x =>
                        {
                            return x.OuterId.Equals(item.GoodsNo + item.SpecCode, StringComparison.CurrentCultureIgnoreCase);
                        });
                        if (sku != null)
                        {
                            switch (ShopId)
                            {
                                case 1000:   //C
                                    item.StockC = sku.Quantity;
                                    break;
                                case 1022:   //天猫
                                    item.StockTmall = sku.Quantity;
                                    break;
                                case 1027:   //AINO
                                    item.StockAiNo = sku.Quantity;
                                    break;
                                case 1030:   //副店
                                    item.StockFD = sku.Quantity;
                                    break;
                            }
                        }
                    }
                }
            }
            return datalist;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }


    public class UpdateStockModel
    {
        public string GoodsNo { get; set; }
        public string SpecCode { get; set; }
        public string SpecName { get; set; }
        public long GjStock { get; set; }
        public int NoPayCount { get; set; }
        public long StockC { get; set; }
        public long StockFX { get; set; }
        public long StockFD { get; set; }
        public long StockTmall { get; set; }
        public long StockAiNo { get; set; }
    }
}
