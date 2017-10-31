using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.Utility;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Main.FrameWork;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace _9M.Work.WPF_Main.Views.Photo
{
    /// <summary>
    /// Interaction logic for PhotoOut.xaml
    /// </summary>
    public partial class PhotoOut : UserControl
    {
        BaseDAL dal = new BaseDAL();

        public PhotoOut()
        {
            InitializeComponent();
            this.DataContext = this;
            btnSearch_Click(null, null);
        }

        #region 加载列表

        void bind(List<PhotographyModel> resultlist, ObservableCollection<PhotographyModel> targetCollection)
        {
            new Thread(() =>
            {
                foreach (PhotographyModel m in resultlist)
                {
                    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, new Action(delegate
                    {
                        targetCollection.Add(m);
                    }));

                }

            }).Start();
        }

        public void LoadData(int pageIndex, int pageSize)
        {
            try
            {


                #region 原始Sql

                ObservableCollection<PhotographyModel> ObserCollection = new ObservableCollection<PhotographyModel>();
                photolist.ItemsSource = ObserCollection;

                StringBuilder sbSql = new StringBuilder();
                StringBuilder sbSqlWhere = new StringBuilder();

                if (txtCondition.Text.Trim().Length > 0)
                {
                    sbSqlWhere.Append(string.Format(" and (photoid like '%{0}%' or photographer like '%{0}%')", txtCondition.Text.Trim()));
                }

                var dateStart = date_start.Text;
                var dateEnd = date_end.Text;

                if (dateStart.Trim().Length > 0 && dateEnd.Trim().Length > 0)
                {
                    sbSqlWhere.Append(string.Format(" and  createTime>='{0} 00:00:00' and createTime<='{1} 23:59:59", dateStart, dateEnd));
                }


                System.Data.DataTable Count = dal.QueryDataTable(string.Format("select count(1) as [count] from T_Photography where 1=1 {0}", sbSqlWhere.ToString()), new object[] { });

                if (Count.Rows.Count > 0)
                {

                    int totalCount = Int32.Parse(Count.Rows[0][0].ToString());
                    TotalPage = (totalCount % PageSize == 0) ? (totalCount / PageSize).ToString() : ((totalCount / PageSize) + 1).ToString();
                    if (totalCount > 0)
                    {
                        sbSql.Append(string.Format(@"                                           
                                            select * from (
                                                        
                                               select ROW_NUMBER() Over(order by id desc) as rownumber,* from T_Photography where 1=1 {0}
                                                        
                                                  ) a where rownumber between {1} and {2} order by rownumber asc", sbSqlWhere.ToString(), (pageIndex - 1) * pageSize + 1, pageIndex * pageSize));

                        var rmCollection = dal.QueryList<PhotographyModel>(sbSql.ToString());
                        bind(rmCollection, ObserCollection);

                    }
                }

                #endregion
            }
            catch (Exception err)
            {
                string msg = err.Message.ToString();
            }

        }

        #endregion

        #region 分页相关属性

        delegate ObservableCollection<PhotographyModel> NextPageHandle();

        void NextPage()
        {
            LoadData(Convert.ToInt32(CurrentPage), PageSize);
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
        private int _pageSize = 100;
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

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    LoadData(Int32.Parse(this.CurrentPage), this.PageSize);
                }));

            }).Start();
        }

        private void btnOperator_Click(object sender, RoutedEventArgs e)
        {
            //string oper = sender.GetType().Equals(typeof(Button)) ? ((Button)sender).Tag.ToString() : ((MenuItem)sender).Tag.ToString();
            _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new PhotoOutDialog(null, OperationStatus.ADD), false);
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            btnOperator_Click(this, null);
        }

        private void dg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PhotographyModel item = photolist.CurrentItem as PhotographyModel;
            if (item != null)
            {
                List<ExpressionModelField> whereField = new List<ExpressionModelField>();
                whereField.Add(new ExpressionModelField() { Name = "photoid", Value = item.photoid });
                List<PhotographyDetailModel> list = dal.GetList<PhotographyDetailModel>(whereField.ToArray(), new OrderModelField[] { new OrderModelField() { PropertyName = "id", IsDesc = true } });
                photodetaillist.ItemsSource = list;
                labInfo.Content = string.Format("合计: {0}件", list.Count.ToString());
            }


        }

        private void btn_PhotoNoPrint(object sender, RoutedEventArgs e)
        {

            PhotographyModel item = photolist.CurrentItem as PhotographyModel;
            if (item != null)
            {
                PrintHelper ph = new PrintHelper();
                DataTable sourcedt = new DataTable();
                sourcedt.Columns.Add("拍照单号");
                DataRow dr = sourcedt.NewRow();
                dr["拍照单号"] = item.photoid;
                sourcedt.Rows.Add(dr);
                ph.PrintPhotoNo(sourcedt);
            }
        }

        private void btnOperator_Edit(object sender, RoutedEventArgs e)
        {
            PhotographyModel item = photolist.CurrentItem as PhotographyModel;

            if (item != null)
            {
                _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new PhotoOutDialog(item, OperationStatus.Edit), false);
            }
        }

        private void btnImport(object sender, RoutedEventArgs e)
        {
            string sheelName = txtTableName.Text.Trim();
            if (sheelName == "")
            {
                MessageBox.Show("表格名称不能为空！", "提示");
                return;
            }
            OpenFileDialog ofp = new OpenFileDialog();
            //过滤器
            ofp.Filter = "XLS|*.xls|XLSX|*.xlsx|TXT|*.txt";
            if (ofp.ShowDialog() == true) //选择完成之后
            {
                DataTable table = ExcelNpoi.ExcelToDataTable(sheelName, true, ofp.FileName);

                //遍获每款的品牌名称
                List<BrandModel> brandlist = dal.GetAll<BrandModel>();

                //明细列表
                List<PhotographyDetailModel> list = new List<PhotographyDetailModel>();


                foreach (DataRow dr in table.Rows)
                {
                    string brandEN = GoodsHelper.BrandEn(dr["编号"].ToString());
                    BrandModel bModel = brandlist.Single(a => a.BrandEN.Equals(brandEN));
                    string brandCN = bModel != null ? bModel.BrandCN : "";
                    list.Add(new PhotographyDetailModel() { goodsno = dr["编号"].ToString(), num = 1, color = dr["规格"].ToString(), brandCN = brandCN, brandEN = brandEN });
                }

                //品牌所有集合
                String brandCnCollection = String.Join(",", list.Select(x => x.brandCN).Distinct().ToArray());
                //所有款数
                int kuanCount = list.Select(p => p.goodsno).ToList().Distinct().Count();


                if (MessageBox.Show("数据加载完毕,是否保存?", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    //获取拍照编号
                    string dateSN = DateTime.Now.ToString("yyMMdd");
                    string sn = dal.QueryDataTable(string.Format("select COUNT(*)+1 as 'no' from dbo.T_Photography where substring(photoid,1,6)='{0}'", dateSN)).Rows[0][0].ToString();
                    string photoNo = dateSN + StringHelper.FillZero(sn, 4);

                    PhotographyModel pm = new PhotographyModel()
                    {
                        photoid = photoNo,
                        tType = "模特拍摄",
                        createTime = DateTime.Now,
                        createEmp = CommonLogin.CommonUser.UserName,
                        catNum = 0,
                        ImageRepairNum = 0,
                        photoNumber = list.Count,
                        kuanNumber = kuanCount,
                        brandColl = brandCnCollection
                    };

                    List<string> sql = new List<string>();
                    sql.Add(string.Format("insert into T_Photography(photoid,tType,catNum,createTime,createEmp,ImageRepairNum,kuanNumber,brandColl,photoNumber) values('{0}','{1}',{2},'{3}','{4}',{5},{6},'{7}',{8})", pm.photoid, pm.tType, pm.catNum, pm.createTime, pm.createEmp, pm.ImageRepairNum, pm.kuanNumber, pm.brandColl, list.Count));

                    foreach (PhotographyDetailModel di in list)
                    {


                        sql.Add(string.Format("insert into T_PhotographyDetail(photoid,goodsno,color,num,brandEn,brandCN) values('{0}','{1}','{2}',1,'{3}','{4}')", photoNo, di.goodsno, di.color, di.brandEN, di.brandCN));
                    }

                    if (dal.ExecuteTransaction(sql, null))
                    {
                        LoadData(Convert.ToInt32(CurrentPage), PageSize);
                    }
                }



            }
        }

    }
}
