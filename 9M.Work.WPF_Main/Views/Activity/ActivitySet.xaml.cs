using _9M.Work.Model;
using _9M.Work.WPF_Common.ValueObjects;
using _9M.Work.WPF_Common.WpfBind;
using _9M.Work.WPF_Main.Infrastrcture;
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
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Win32;
using _9M.Work.WPF_Main.FrameWork;
using System.Data;
using _9M.Work.DbObject;
using _9M.Work.WPF_Common;
using System.Transactions;
using System.Data.Entity;
using _9M.Work.Utility;
using System.Collections.ObjectModel;

namespace _9M.Work.WPF_Main.Views.Activity
{
    /// <summary>
    /// ActivitySet.xaml 的交互逻辑
    /// </summary>
    public partial class ActivitySet : UserControl, BaseDialog
    {
        private BaseDAL dal = new BaseDAL();
        private ObservableCollection<ActivityGoodsModel> dataSource = new ObservableCollection<ActivityGoodsModel>();
        public List<ActivityGoodsModel> GoodsList { get; set; }
        private OperationStatus status;
        private ActivityModel acm;
        private string activityNo;
        //活动设置的控件组
        List<ComboBox> slist;
        List<ComboBox> elist;
        public ActivitySet(OperationStatus status, ActivityModel acm)
        {
            this.acm = acm;
            this.status = status;
            InitializeComponent();
            this.DataContext = this;
            slist = WPFControlsSearchHelper.GetChildObjects<ComboBox>(Sta_Start, string.Empty);
            elist = WPFControlsSearchHelper.GetChildObjects<ComboBox>(Sta_End, string.Empty);
            BindShop(acm);
            ComboBoxBind.BindEnum(com_type, typeof(ActivityType), true);
            if (status == OperationStatus.Edit) //如果是修改。如果加载值与界面
            {
                activityNo = acm.ActivityNo;
                EditInit(acm);
                BindGrid(status);
            }
            if (status == OperationStatus.ADD) //如果是添加生成编号
            {
                activityNo = System.Guid.NewGuid().ToString().Replace("-", "");
            }
        }

        public void EditInit(ActivityModel acm)
        {
            //活动名称
            com_type.Text = acm.ActivityName;
            //动态活动
            radio_activity.IsChecked = acm.Isactivity;
            //活动时间
            date_start.SelectedDate = acm.Startdate;
            date_end.SelectedDate = acm.Enddate;
            //活动名称
            tb_typename.Text = acm.ActivityRealName;
            //活动前后设置
            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0:
                        slist[i].SelectedIndex = acm.Supdatepost ? 1 : 0;
                        elist[i].SelectedIndex = acm.Eupdatepost ? 1 : 0;
                        break;
                    case 1:
                        slist[i].SelectedIndex = acm.Supdatedis ? 1 : 0;
                        elist[i].SelectedIndex = acm.Eupdatedis ? 1 : 0;
                        break;
                    case 2:
                        slist[i].SelectedIndex = acm.Supdateprice ? 1 : 0;
                        elist[i].SelectedIndex = acm.Eupdateprice ? 1 : 0;
                        break;
                    case 3:
                        slist[i].SelectedIndex = acm.Supdatetitle ? 1 : 0;
                        elist[i].SelectedIndex = acm.Eupdatetitle ? 1 : 0;
                        break;
                }
            }

        }

        public void BindShop(ActivityModel acm)
        {
            List<ShopModel> shop = dal.GetList<ShopModel>(x => x.id == 1000 || x.id == 1003 ||  x.id == 1027);
            ComboBoxBind.BindComboBox(com_shop, shop, "shopName", "shopId");
            if (status == OperationStatus.ADD)
            {
                com_shop.SelectedIndex = 0;
            }
            else
            {
                com_shop.SelectedIndex = shop.FindIndex(x => x.shopName == acm.ShopModel.shopName);
            }
        }


        public void BindGrid(OperationStatus status)
        {
            ActivityGoodsGridlist.ItemsSource = null;
            ActivityGoodsGridlist.ItemsSource = dataSource;

            List<ActivityGoodsModel> goodslist = null;
            if (status == OperationStatus.Edit)
            {
                goodslist = dal.GetList<ActivityGoodsModel>(x => x.ActivityNo == acm.ActivityNo);
            }
            else
            {
                goodslist = GoodsList;
            }

            goodslist.ForEach(x =>
            {
                dataSource.Add(x);
            });
            lab_count.Content = dataSource.Count.ToString();
        }

        public DelegateCommand CancelCommand
        {
            get { return new DelegateCommand(CloseDialog); }
        }

        public string Title
        {
            get
            {
                return status == OperationStatus.Edit ? "修改活动" : "添加活动";
            }
        }

        public void CloseDialog()
        {
            //刷新父窗体
            ActivityMain am = (ActivityMain)FormInit.FindFather(this);
            am.LoadActivityData(am.PageIndex, am.PageSize);
            FormInit.CloseDialog(this);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        private void Btn_CommandClick(object sender, RoutedEventArgs e)
        {
            string Append = tb_appendtitle.Text;
            ShopModel shop = (ShopModel)com_shop.SelectedItem;
            int Tag = Convert.ToInt32((sender as Button).Tag);
            switch (Tag)
            {
                case 1: //Excel导入
                    DataTable dt;
                    try
                    {
                        dt = GlobalUtil.ReadExcel(new OpenFileDialog());
                    }
                    catch (Exception ex)
                    {
                        CCMessageBox.Show("导入错误,请检查表格是否被打开");
                        return;
                    }
                    if (dt != null)
                    {

                        GoodsList = new List<ActivityGoodsModel>();
                        foreach (DataRow dr in dt.Rows)
                        {
                            ActivityGoodsModel agm = new ActivityGoodsModel();
                            agm.GoodsNo = dr["款号"].ToString();
                            agm.Defaultprice = Convert.ToDecimal(dr["原价"]);
                            agm.Defaulttitle = dr["原标题"].ToString();
                            agm.Activittitle = !string.IsNullOrEmpty(Append) ? GoodsHelper.RetrunActityTitle(agm.Defaulttitle, Append) : agm.Defaulttitle;
                            agm.Activityprice = Convert.ToDecimal(dr["活动价"]);
                            GoodsList.Add(agm);
                        }
                        BindGrid(OperationStatus.ADD);
                    }
                    break;
                case 2: //同步淘宝
                    if (slist[2].SelectedIndex==1)
                    {
                        CCMessageBox.Show("需要改价的商品不能同步淘宝");
                        return;
                    }
                    FormInit.OpenDialog(this, new TaoBaoGoodsNoSet(shop, activityNo), true, 2);
                    break;
                case 3: //删除所有款号
                    Delete(true);
                    break;
                case 4: //编辑活动
                    if (date_start.SelectedDate == null || date_end.SelectedDate == null || com_type.SelectedIndex == 0 || string.IsNullOrEmpty(tb_typename.Text))
                    {
                        CCMessageBox.Show("请完整的编辑内容");
                        return;
                    }

                    //活动列表
                    ActivityModel model = new ActivityModel();
                    model.Shopid = Convert.ToInt32(com_shop.SelectedValue);
                    model.ActivityNo = activityNo;
                    model.ActivityName = com_type.Text;
                    model.Isactivity = Convert.ToBoolean(radio_activity.IsChecked);
                    model.Startdate = Convert.ToDateTime(date_start.SelectedDate);
                    model.Enddate = Convert.ToDateTime(date_end.SelectedDate);
                    model.ActivityRealName = tb_typename.Text;
                    model.Supdatepost = slist[0].SelectedIndex == 1;
                    model.Supdatedis = slist[1].SelectedIndex == 1;
                    model.Supdateprice = slist[2].SelectedIndex == 1;
                    model.Supdatetitle = slist[3].SelectedIndex == 1;
                    model.Eupdatepost = elist[0].SelectedIndex == 1;
                    model.Eupdatedis = elist[1].SelectedIndex == 1;
                    model.Eupdateprice = elist[2].SelectedIndex == 1;
                    model.Eupdatetitle = elist[3].SelectedIndex == 1;
                    model.ActivityStatus = Convert.ToInt32(ActivityStatus.Wait);
                    ////活动商品
                    //if (GoodsList == null)
                    //{
                    //    GoodsList = new List<ActivityGoodsModel>();
                    //}
                    List<ActivityGoodsModel> goodslist = dataSource.ToList();
                    //自动匹配标题加入活动编号

                    goodslist.ForEach(x =>
                    {
                        x.ActivityNo = activityNo;
                        x.Activittitle = !string.IsNullOrEmpty(Append) && model.Supdatetitle == true ? GoodsHelper.RetrunActityTitle(x.Defaulttitle, Append) : x.Defaulttitle;
                    });
                    //判断是否添加活动
                    if (goodslist.Count == 0)
                    {
                        if (CCMessageBox.Show("您还没有添加活动商品,是否新建活动", "提示", CCMessageBoxButton.YesNo) == CCMessageBoxResult.No)
                        {
                            return;
                        }
                    }

                    bool Comit = true;
                    //事务提交
                    using (_9MWorkDataContext context = new _9MWorkDataContext(BaseDAL.DBConnectionString))
                    {
                        using (TransactionScope transaction = new TransactionScope())
                        {
                            try
                            {
                                if (status == OperationStatus.ADD)
                                {
                                    model.Createdate = DateTime.Now;
                                    context.Set<ActivityModel>().Add(model);
                                    context.Set<ActivityGoodsModel>().AddRange(goodslist);
                                    //context.Entry<ActivityModel>(model).State = EntityState.Modified;
                                }
                                else
                                {
                                    model.Id = acm.Id;
                                    model.Createdate = acm.Createdate;
                                    model.ActivityStatus = acm.ActivityStatus;
                                    //context.Set<ActivityModel>().Attach(model);
                                    context.Entry<ActivityModel>(model).State = EntityState.Modified;
                                    //foreach (ActivityGoodsModel item in goodslist)
                                    //{
                                    //    context.Set<ActivityGoodsModel>().Attach(item);
                                    //}
                                    List<ActivityGoodsModel> datalist = dal.GetList<ActivityGoodsModel>(x => x.ActivityNo == activityNo);
                                    foreach (ActivityGoodsModel item in datalist)
                                    {
                                        context.Set<ActivityGoodsModel>().Attach(item);
                                    }
                                    context.Set<ActivityGoodsModel>().RemoveRange(datalist);
                                    context.Set<ActivityGoodsModel>().AddRange(goodslist);
                                }
                                context.SaveChanges();
                                transaction.Complete();
                                string Msg = goodslist.Count == 0 ? "成功 还没有编辑活动商品" : "成功";
                                CCMessageBox.Show(Msg);
                            }
                            catch (Exception ex)
                            {
                                Comit = false;
                                CCMessageBox.Show("失败：" + ex.Message);
                            }
                        }
                    }
                    if (Comit)
                    {
                        CloseDialog();
                    }
                    break;
                case 5:
                    CloseDialog();
                    break;
                case 6: //删除单款
                    Delete(false);
                    break;
            }
        }

        public void Delete(bool ALL)
        {
            if (CCMessageBox.Show("是否删除商品", "提示", CCMessageBoxButton.YesNo) == CCMessageBoxResult.No)
            {
                return;
            }
            GoodsList = new List<ActivityGoodsModel>();
            if (ALL)
            {
                dataSource.Clear();
            }
            else
            {
                if (ActivityGoodsGridlist.SelectedItem != null)
                {
                    dataSource.Remove(ActivityGoodsGridlist.SelectedItem as ActivityGoodsModel);
                }
            }
            BindGrid(OperationStatus.ADD);
        }
    }
}
