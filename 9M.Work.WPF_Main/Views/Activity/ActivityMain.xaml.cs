using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.TopApi;
using _9M.Work.Utility;
using _9M.Work.WPF_Common.ValueObjects;
using _9M.Work.WPF_Common.WpfBind;
using _9M.Work.WPF_Main.Infrastrcture;
using _9Mg.Work.TopApi;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
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

namespace _9M.Work.WPF_Main.Views.Activity
{
    /// <summary>
    /// ActivityMain.xaml 的交互逻辑
    /// </summary>
    public partial class ActivityMain : UserControl
    {
        private ObservableCollection<ActivityModel> dataSource;
        private BaseDAL dal = new BaseDAL();
        private SessionUserModel User = WPF_Common.CommonLogin.CommonUser;
        //异步委托
        public delegate List<ActivityLogModel> AsyncEventHandler(bool IsBegin, ActivityModel acm, List<ActivityGoodsModel> goodslist);
        public ActivityMain()
        {
            InitializeComponent();
            this.DataContext = this;

            BindShop();
            BindActitvyStatus();

            BindActivityName();

            dataSource = new ObservableCollection<ActivityModel>();


            NextPageSearchCommand = new DelegateCommand(NextPageSearchCommandFunc);
            LoadActivityData(PageIndex, PageSize);

        }

        public void BindShop()
        {
            List<ShopModel> shop = dal.GetList<ShopModel>(x => x.id == 1000 || x.id == 1003 || x.id == 1027);
            shop.Insert(0, new ShopModel() { shopId = 0, shopName = "请选择" });
            ComboBoxBind.BindComboBox(shopCombox, shop, "shopName", "shopId");
            shopCombox.SelectedIndex = 0;
        }

        public void BindActivityName()
        {
            ComboBoxBind.BindEnum(com_actitype, typeof(ActivityType), true);
        }

        public void BindActitvyStatus()
        {
            ComboBoxBind.BindEnum(com_status, typeof(ActivityStatus), true);
        }

        public void LoadActivityData(int PageIndex, int PageSize)
        {
            dataSource.Clear();
            ActivityGridlist.ItemsSource = null;
            ActivityGridlist.ItemsSource = dataSource;
            LeftModelField include = new LeftModelField() { PropertyName = "ShopModel" };
            //排序表达式
            OrderModelField[] order = new OrderModelField[1] {
                new OrderModelField() { IsDesc=true,PropertyName="Createdate"}
            };
            //条件表达式
            List<ExpressionModelField> explist = new List<ExpressionModelField>();
            //店铺
            int ShopId = Convert.ToInt32(shopCombox.SelectedValue);
            if (ShopId > 0)
            {
                explist.Add(new ExpressionModelField() { Name = "Shopid", Relation = EnumRelation.Equal, Value = ShopId });
            }
            //活动开始时间
            DateTime? date = date_start.SelectedDate;
            if (date != null)
            {
                explist.Add(new ExpressionModelField() { Name = "Startdate", Relation = EnumRelation.GreaterThanOrEqual, Value = date });
            }
            //活动类型
            int activityvalue = Convert.ToInt32(com_actitype.SelectedValue);
            if (activityvalue > 0)
            {
                explist.Add(new ExpressionModelField() { Name = "ActivityName", Relation = EnumRelation.GreaterThanOrEqual, Value = com_actitype.Text });
            }
            //活动名称
            string activityname = tb_actiname.Text;
            if (!string.IsNullOrEmpty(activityname))
            {
                explist.Add(new ExpressionModelField() { Name = "ActivityRealName", Relation = EnumRelation.Equal, Value = activityname });
            }
            //活动状态
            int activitystatus = Convert.ToInt32(com_status.SelectedValue);
            if (activitystatus > 0)
            {
                explist.Add(new ExpressionModelField() { Name = "ActivityStatus", Relation = EnumRelation.GreaterThanOrEqual, Value = activitystatus });
            }

            Dictionary<string, object> dic = dal.GetListPaged_Include<ActivityModel>(PageIndex, PageSize, include, explist.ToArray(), order);
            int totalCount = Convert.ToInt32(dic["total"]);
            TotalCount = totalCount;

            TotalPage = (totalCount % PageSize == 0) ? (totalCount / PageSize).ToString() : ((totalCount / PageSize) + 1).ToString();
            List<ActivityModel> list = dic["rows"] as List<ActivityModel>;
            list.ForEach(x =>
            {
                dataSource.Add(x);
            });
        }

        #region 分页相关属性

        void NextPage()
        {
            LoadActivityData(Convert.ToInt32(CurrentPage), PageSize);
        }

        /// <summary>
        /// 分页查询命令
        /// </summary>
        private void NextPageSearchCommandFunc()
        {
            NextPage();
        }



        /// <summary>
        /// 总页数
        /// </summary>
        private string _totalPage = string.Empty;
        public string TotalPage
        {
            get { return _totalPage; }
            set
            {
                _totalPage = value;
                this.OnPropertyChanged("TotalPage");

            }
        }


        /// <summary>
        /// 当前页
        /// </summary>
        private string _currentPage = "1";
        public string CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                this.OnPropertyChanged("CurrentPage");
            }
        }


        /// <summary>
        /// 每页显示的记录数
        /// </summary>
        private int _pageSize = 30;
        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = value;
                this.OnPropertyChanged("PageSize");
            }
        }

        /// <summary>
        /// 当前页索引
        /// </summary>
        private int _pageIndex;
        public int PageIndex
        {
            get { return _pageIndex; }
            set
            {
                _pageIndex = value;
                this.OnPropertyChanged("PageIndex");
            }
        }

        /// <summary>
        /// 记录总数
        /// </summary>
        private int _totalCount;
        public int TotalCount
        {
            get { return _totalCount; }
            set
            {
                _totalCount = value;

                this.OnPropertyChanged("_totalCount");
            }
        }
        /// <summary>
        /// 分页管理
        /// </summary>
        public ICommand NextPageSearchCommand { get; set; }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int Tag = Convert.ToInt32((sender as Button).Tag);
            switch (Tag)
            {
                case 1: //添加
                    FormInit.OpenDialog(this, new ActivitySet(OperationStatus.ADD, new ActivityModel()), false);
                    break;
                case 2: //编辑
                    ActivityModel acm = (ActivityModel)ActivityGridlist.SelectedItem;
                    if (acm != null)
                    {
                        FormInit.OpenDialog(this, new ActivitySet(OperationStatus.Edit, acm), false);
                    }
                    break;
                case 3: //删除
                    if (CCMessageBox.Show("是否删除活动与商品信息", "提示", CCMessageBoxButton.YesNo) == CCMessageBoxResult.No)
                    {
                        return;
                    }
                    bool b = true;
                    ActivityModel delmodel = (ActivityModel)ActivityGridlist.SelectedItem;
                    if (delmodel != null)
                    {
                        using (_9MWorkDataContext context = new _9MWorkDataContext(BaseDAL.DBConnectionString))
                        {
                            using (TransactionScope transaction = new TransactionScope())
                            {
                                try
                                {
                                    context.Set<ActivityModel>().Attach(delmodel);
                                    List<ActivityGoodsModel> datalist = dal.GetList<ActivityGoodsModel>(x => x.ActivityNo == delmodel.ActivityNo);
                                    foreach (ActivityGoodsModel item in datalist)
                                    {
                                        context.Set<ActivityGoodsModel>().Attach(item);
                                    }
                                    context.Set<ActivityGoodsModel>().RemoveRange(datalist);
                                    context.Set<ActivityModel>().Remove(delmodel);
                                    context.SaveChanges();
                                    transaction.Complete();
                                }
                                catch (Exception ex)
                                {
                                    b = false;
                                }
                            }
                        }
                    }
                    string Msg = b ? "成功" : "失败";
                    CCMessageBox.Show(Msg);
                    if (b)
                    {
                        LoadActivityData(PageIndex, PageSize);
                    }

                    break;
                case 4: //查询
                    LoadActivityData(PageIndex, PageSize);
                    break;
            }
        }

     
        private void Button_StatusClick(object sender, RoutedEventArgs e)
        {
            int Tag = Convert.ToInt32((sender as Button).Tag);
            ActivityModel acm = (ActivityModel)ActivityGridlist.SelectedItem;
            switch (Tag)
            {
                case 1: //开始活动
                    if (CCMessageBox.Show("开始活动将改变商品的属性,是否开始", "提示", CCMessageBoxButton.YesNo) == CCMessageBoxResult.Yes)
                    {
                        List<ActivityGoodsModel> goodslist = dal.GetList<ActivityGoodsModel>(x => x.ActivityNo == acm.ActivityNo);
                        if (goodslist.Count == 0)
                        {
                            CCMessageBox.Show("没有设置活动的商品,活动不能开始");
                            return;
                        }
                        AsyncEventHandler asy = new AsyncEventHandler(this.EditActivity);
                        //异步调用开始，没有回调函数和AsyncState,都为null
                        IAsyncResult ir = asy.BeginInvoke(true,acm,goodslist, CallBackEditActivity, null);
                    }
                    break;
                case 2: //处理活动
                    if (acm.ActivityStatus == Convert.ToInt32(ActivityStatus.WaitOperation))
                    {

                    }
                    break;
                case 3:  //完成活动
                    if (CCMessageBox.Show("完成活动将改变商品的属性,是否开始", "提示", CCMessageBoxButton.YesNo) == CCMessageBoxResult.Yes)
                    {

                    }
                    break;
            }
        }




        /// <summary>
        /// 修改活动状态
        /// </summary>
        public List<ActivityLogModel> EditActivity(bool IsBegin, ActivityModel acm, List<ActivityGoodsModel> goodslist)
        {
            //打开加载
            bar.LoadBar(true);
            //打开加载中的字符
            bar.Loading(true);
            List<ActivityLogModel> LogList = new List<ActivityLogModel>();
            List<string> goodsnolist = goodslist.Select(x => x.GoodsNo).ToList();
            ShopModel shop = acm.ShopModel;
            TopSource com = new TopSource(shop); 
           // DistributorRecord dr = new DistributorRecord(shop.appKey, shop.appSecret, shop.sessionKey); //分销
            List<Item> itemlist = null;
            int ShopId = shop.shopId;
            if (ShopId != 1003)  //活动店铺判断
            {
                itemlist = com.GetItemList(goodsnolist, string.Empty);
            }
            //修改方式
            UpdateGoodsSub sub = new UpdateGoodsSub();
            if (IsBegin)
            {
                sub.PostStatus = acm.Supdatepost ? 2 : 1;
                sub.Dis = acm.Supdatedis ? 2 : 1;
                sub.SyncPrice = acm.Supdateprice;
            }
            else
            {
                sub.PostStatus = acm.Eupdatepost ? 2 : 1;
                sub.Dis = acm.Eupdatedis ? 2 : 1;
                sub.SyncPrice = acm.Eupdateprice;
            }

            List<ProductCat> catlist = com.getProductLine();
            List<FenXiaoPropModel> porplist = dal.QueryList<FenXiaoPropModel>(@"select * from T_FenXiaoProp", new object[] { });
            for (int i = 0; i < goodslist.Count; i++)
            {
                ActivityLogModel alm = new ActivityLogModel();
                string Respones = string.Empty;
                try
                {
                    if (IsBegin)
                    {
                        //价格
                        if (acm.Supdateprice)
                        {
                            sub.Price = goodslist[i].Activityprice.ToString();
                        }
                        //标题
                        sub.RealTitle = acm.Supdatetitle ? goodslist[i].Activittitle : string.Empty;
                    }
                    else
                    {
                        //价格
                        if (acm.Eupdateprice)
                        {
                            sub.Price = goodslist[i].Defaultprice.ToString();
                        }
                        //标题
                        sub.RealTitle = acm.Eupdatetitle ? goodslist[i].Defaulttitle : string.Empty;
                    }

                    if (ShopId == 1003) //活动店铺判断
                    {
                        //分销修改
                        FenxiaoProduct item = com.GetScItemByCode(goodslist[i].GoodsNo,string.Empty);
                        if (item != null)
                        {
                            Respones = com.UpdateProductByModel(item, sub, new List<NoPayModel>(), catlist, porplist);
                        }
                        else
                        {
                            Respones = "没有查询到款";
                        }
                    }
                    else
                    {
                        //C店修改
                        Item It = itemlist.Where(x => x.OuterId.Equals(goodslist[i].GoodsNo)).FirstOrDefault();
                        if (It != null)
                        {
                            Respones = com.UpdateItemByModel(It, sub, new List<NoPayModel>());
                        }
                        else
                        {
                            Respones = "没有查询到款";
                        }
                    }
                }
                catch (Exception ex)
                {
                    Respones = ex.Message;
                }
                alm.ActivityNo = acm.ActivityNo;
                alm.GoodsNo = goodslist[i].GoodsNo;
                alm.LogTime = DateTime.Now;
                alm.UserName = User.UserName;
                alm.LogType = IsBegin ? Convert.ToInt32(ActivityStatus.Doing) : Convert.ToInt32(ActivityStatus.Finish);
                //如果修改不成功
                if (!Respones.Equals("success"))
                {
                    alm.ErrorMsg = Respones;
                    alm.Finish = false;
                }
                else
                {
                    alm.Finish = true;
                }
                LogList.Add(alm);
                //更新进度条
                int current = i == goodslist.Count - 1 ? goodslist.Count : i + 1;
                bar.UpdateBarValue(goodslist.Count, current);
            }
            bool logbool = dal.AddList<ActivityLogModel>(LogList);
            bar.Loading(false);
            bar.LoadBar(false);
            return LogList.Where(x => x.Finish == false).ToList();
        }

        /// <summary>
        /// 修改活动状态的回调
        /// </summary>
        /// <param name="ar"></param>
        private void CallBackEditActivity(IAsyncResult ar)
        {
            AsyncEventHandler handler = (AsyncEventHandler)((AsyncResult)ar).AsyncDelegate;
            List<ActivityLogModel> loglist = handler.EndInvoke(ar);
        }



        /// <summary>
        /// 处理活动
        /// </summary>
        /// <param name="acm"></param>
        public void OperationActivity(ActivityModel acm)
        {

        }

        /// <summary>
        /// 完成活动
        /// </summary>
        /// <param name="acm"></param>
        public void FinishActivity(ActivityModel acm)
        {

        }
    }
}
