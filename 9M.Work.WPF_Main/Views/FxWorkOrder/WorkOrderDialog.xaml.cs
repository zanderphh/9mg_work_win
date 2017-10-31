using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.TopApi;
using _9M.Work.Utility;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Common.ValueObjects;
using _9M.Work.WPF_Main.Infrastrcture;
using Microsoft.Practices.Prism.Commands;
using System;
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
using Top.Api.Domain;

namespace _9M.Work.WPF_Main.Views.FxWorkOrder
{

    /// <summary>
    /// Interaction logic for WorkOrderDialog.xaml
    /// </summary>
    public partial class WorkOrderDialog : UserControl, BaseDialog
    {
        BaseDAL dal = new BaseDAL();

        #region 属性

        private FxWorkOrderModel _paramFxWorkOrderModel;
        public FxWorkOrderModel ParamFxWorkOrderModel
        {
            get { return _paramFxWorkOrderModel; }
            set
            {
                if (_paramFxWorkOrderModel != value)
                {
                    _paramFxWorkOrderModel = value;
                    this.OnPropertyChanged("ParamFxWorkOrderModel");
                }
            }
        }

        private ObservableCollection<SubPurchaseOrder> _gridDataSource;
        public ObservableCollection<SubPurchaseOrder> GridDataSource
        {
            get { return _gridDataSource; }
            set
            {
                if (_gridDataSource != value)
                {
                    _gridDataSource = value;
                    this.OnPropertyChanged("GridDataSource");
                }
            }
        }

        private Visibility _wattingVisibility = Visibility.Visible;
        public Visibility WattingVisibility
        {
            get { return _wattingVisibility; }
            set
            {
                if (_wattingVisibility != value)
                {
                    _wattingVisibility = value;
                    this.OnPropertyChanged("WattingVisibility");
                }
            }
        }

        #endregion

        #region Dialog
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
            get { return "工单处理"; }
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

        #endregion

        delegate void CallTopFenxiaoPurchaseHandle(string fenXiaoId);
        delegate void LoadingImageHandle(string questionId);

        public WorkOrderDialog(FxWorkOrderModel model)
        {
            InitializeComponent();
            this.DataContext = this;


            string sql = string.Format("select * from DB_9MFenXiao.dbo.WorkOrder where id={0}", model.id);

            DataTable dt = dal.QueryDataTable(sql);
            List<FxWorkOrderModel> convertModels = ConvertType.ConvertToModel<FxWorkOrderModel>(dt);
            ParamFxWorkOrderModel = convertModels.First();


            if (ParamFxWorkOrderModel.status == "已完成")
            {
                btnTracking.Visibility = System.Windows.Visibility.Collapsed;
                btnEnd.Visibility = System.Windows.Visibility.Collapsed;
                btnCopy.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                btnTracking.Visibility = System.Windows.Visibility.Visible;
                btnEnd.Visibility = System.Windows.Visibility.Visible;
                btnCopy.Visibility = System.Windows.Visibility.Collapsed;
            }

            GridDataSource = new ObservableCollection<SubPurchaseOrder>();
            tradelist.ItemsSource = GridDataSource;
            CallTopFenxiaoPurchaseHandle handle = new CallTopFenxiaoPurchaseHandle(CallTopFenXiaoPurchase);
            handle.BeginInvoke(model.tradeId, null, null);
        }

        private void CallTopFenXiaoPurchase(string fenXiaoTradeId)
        {
            #region 暂屏蔽(必须用分销流水号获取订单，当前改为订单编号)

            //List<Erp_Model_GJRefund> tradeIdCollection = new List<Erp_Model_GJRefund>() { };

            //string[] fxId = fenXiaoTradeId.Split(' ');

            //foreach (string f in fxId)
            //{
            //    if (f != "")
            //    {
            //        if (tradeIdCollection.Where(a => a.tbTradeNo == f).Count() == 0)
            //        {
            //            tradeIdCollection.Add(new Erp_Model_GJRefund() { tbTradeNo = f });
            //        }
            //    }
            //}

            //TopSource com = new TopSource();
            ////分销采购单
            //List<PurchaseOrder> tradeCollectionByFxApi = com.GetTradeInfoByFX(tradeIdCollection);

            ////集合采购单所有的子单
            //List<SubPurchaseOrder> allSubPurchaseOrder = new List<SubPurchaseOrder>();

            //if (allSubPurchaseOrder.Count > 0)
            //{
            //    foreach (PurchaseOrder p in tradeCollectionByFxApi)
            //    {
            //        if (p.SubPurchaseOrders != null)
            //        {
            //            foreach (SubPurchaseOrder s in p.SubPurchaseOrders)
            //            {
            //                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, new Action(delegate
            //                {
            //                    GridDataSource.Add(s);
            //                }));
            //            }
            //        }

            //    }
            //}

            #endregion

            WattingVisibility = System.Windows.Visibility.Collapsed;

            LoadingImageHandle loading = new LoadingImageHandle(LoadingImage);
            loading.BeginInvoke(ParamFxWorkOrderModel.questionId, null, null);

            if (ParamFxWorkOrderModel.status == EnumHelper.GetEnumTextVal((int)FxWorkOrderEnum.Untreated, typeof(FxWorkOrderEnum)))
            {
                if (SetStatus(FxWorkOrderEnum.BeingProcessed, ParamFxWorkOrderModel.id))
                {
                    ParamFxWorkOrderModel.status = EnumHelper.GetEnumTextVal((int)FxWorkOrderEnum.BeingProcessed, typeof(FxWorkOrderEnum));
                }
            }

        }

        private void LoadingImage(string questionId)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, new Action(delegate
            {
                string sql = string.Format("select * from DB_9MFenXiao.dbo.WorkOrderImage where questionid='{0}'", questionId);
                DataTable dt = dal.QueryDataTable(sql);
                int RowCount = dt.Rows.Count;

                ImageBox.Children.Clear();
                StackPanel stack = new StackPanel() { Orientation = Orientation.Vertical };
                stack.Margin = new Thickness(0, 0, 0, 0);
                //绑定控件
                for (int i = 0; i < RowCount; i++)
                {
                    Image image = new Image() { Width = 380, Height = 400 };
                    image.Margin = new Thickness(0, 10, 0, 0);

                    string httpURL = "http://fx.9mg.cn/Services/" + dt.Rows[i]["img"].ToString();
                    image.Tag = httpURL;
                    image.Source = new BitmapImage(new Uri(httpURL, UriKind.Absolute));
                    image.MouseLeftButtonDown += image_MouseLeftButtonDown;
                    stack.Children.Add(image);
                }

                ImageBox.Children.Add(stack);

            }));


        }

        private void image_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            Image img = (Image)sender;
            System.Diagnostics.Process.Start(img.Tag.ToString());
        }

        private bool SetStatus(FxWorkOrderEnum fxWorkOrderStatus, int id)
        {

            string status = EnumHelper.GetEnumTextVal((int)fxWorkOrderStatus, typeof(FxWorkOrderEnum));
            string sql = "";

            if (fxWorkOrderStatus == FxWorkOrderEnum.BeingProcessed)
            {
                sql = string.Format("update DB_9MFenXiao.dbo.WorkOrder set [status]='{0}',operatorTime=getdate(),operatorEmp='{1}' where id={2}", status, CommonLogin.CommonUser.UserName, id);
            }
            else if (fxWorkOrderStatus == FxWorkOrderEnum.WaitingTracking)
            {
                sql = string.Format("update DB_9MFenXiao.dbo.WorkOrder set [status]='{0}' where id={1}", status, id);
            }
            else if (fxWorkOrderStatus == FxWorkOrderEnum.end)
            {
                sql = string.Format("update DB_9MFenXiao.dbo.WorkOrder set [status]='{0}',endTime=getdate(),operatorEmp='{1}' where id={2}", status, CommonLogin.CommonUser.UserName, id);
            }

            try
            {

                if (dal.ExecuteSql(sql) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        private void btn_setTracking(object sender, RoutedEventArgs e)
        {
            if (ParamFxWorkOrderModel.status == EnumHelper.GetEnumTextVal((int)FxWorkOrderEnum.BeingProcessed, typeof(FxWorkOrderEnum)))
            {
                if (SetStatus(FxWorkOrderEnum.WaitingTracking, ParamFxWorkOrderModel.id))
                {
                    ParamFxWorkOrderModel.status = EnumHelper.GetEnumTextVal((int)FxWorkOrderEnum.WaitingTracking, typeof(FxWorkOrderEnum));
                    MessageBox.Show("已设置!", "提示");
                }
                else
                {
                    MessageBox.Show("设置失败！", "警告");
                }
            }
            else
            {
                MessageBox.Show("只有在处理中的状态才能进行设置!", "提示");
            }
        }

        private void btn_setEnd(object sender, RoutedEventArgs e)
        {

            string tag = ((Button)sender).Tag.ToString();

            if (ParamFxWorkOrderModel.status == EnumHelper.GetEnumTextVal((int)FxWorkOrderEnum.BeingProcessed, typeof(FxWorkOrderEnum)) || ParamFxWorkOrderModel.status == EnumHelper.GetEnumTextVal((int)FxWorkOrderEnum.WaitingTracking, typeof(FxWorkOrderEnum)))
            {
                if (SetStatus(FxWorkOrderEnum.end, ParamFxWorkOrderModel.id))
                {
                    btn_copy(sender, e);
                    ParamFxWorkOrderModel.status = EnumHelper.GetEnumTextVal((int)FxWorkOrderEnum.end, typeof(FxWorkOrderEnum));

                    if (tag == "endReply")
                    {
                        btn_OpenAliWang(sender, e);
                    }

                    WorkOrderList rl = ((WorkOrderList)FormInit.FindFather(this));
                    rl.LoadRefundData(Convert.ToInt32(rl.CurrentPage), rl.PageSize);
                    CloseDialog();
                }
                else
                {
                    MessageBox.Show("设置失败！", "警告");
                }
            }
            else
            {
                MessageBox.Show("只有在'处理中'或者'等待踪踪'的状态才能进行设置!", "提示");
            }

        }

        private void btn_UpdatePrice(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("开发中。。。!", "提示");
        }

        private void btn_Reset(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("开发中。。。!", "提示");
        }

        private void btn_OpenAliWang(object sender, RoutedEventArgs e)
        {

            string url = " https://awp.taobao.com/bs/wwlight.html?ver=3&touid=" + ParamFxWorkOrderModel.aliWang.Replace("：", ":") + "&siteid=cntaobao&status=1&charset=utf-8";
            System.Diagnostics.Process.Start(url);
        }

        private void btn_copy(object sender, RoutedEventArgs e)
        {

            string copyText = string.Format("工单编号：{0}\r\n问题分类：{1}\r\n订单号：{2}\r\n问题描述：{3}\r\n已经处理完成，请查看\r\n",
                ParamFxWorkOrderModel.questionId,
                ParamFxWorkOrderModel.questionType,
                ParamFxWorkOrderModel.tradeId,
                ParamFxWorkOrderModel.questionDesc);
            Clipboard.SetText(copyText);

        }
    }
}
