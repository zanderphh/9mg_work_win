using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.Utility;
using _9M.Work.WPF_Common;
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

namespace _9M.Work.WPF_Main.Views.Photo
{
    /// <summary>
    /// Interaction logic for PhotoOutDialog.xaml
    /// </summary>
    public partial class PhotoOutDialog : UserControl, BaseDialog
    {
        BaseDAL dal = new BaseDAL();
        OperationStatus currentOperation = OperationStatus.ADD;
        List<BrandModel> brandlist = null;


        #region 属性

        private ObservableCollection<PhotographyDetailModel> _rdCollection = new ObservableCollection<PhotographyDetailModel>();

        public ObservableCollection<PhotographyDetailModel> rdCollection
        {
            get { return _rdCollection; }
            set
            {
                if (_rdCollection != value)
                {
                    _rdCollection = value;
                    this.OnPropertyChanged("rdCollection");
                }
            }
        }

        private PhotographyModel _photography = new PhotographyModel();
        public PhotographyModel photography
        {
            get { return _photography; }
            set { _photography = value; }
        }


        #endregion
        public PhotoOutDialog(PhotographyModel item, OperationStatus status)
        {
            InitializeComponent();
            this.DataContext = this;
            currentOperation = status;
            if (currentOperation == OperationStatus.Edit)
            {
                List<ExpressionModelField> field = new List<ExpressionModelField>();
                field.Add(new ExpressionModelField() { Name = "photoid", Value = item.photoid });
                List<PhotographyDetailModel> list = dal.GetList<PhotographyDetailModel>(field.ToArray(), new OrderModelField[] { });

                foreach (PhotographyDetailModel m in list)
                {
                    rdCollection.Add(m);
                }

                txtModel.Text = item.tType;

                photography = item;
            }

            brandlist = dal.GetAll<BrandModel>();
        }


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
            get { return "拍照建单"; }
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

        private void txtGoodsno_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtGoodsno.Text.Trim() == "")
                {
                    MessageBox.Show("商品编号不能为空！", "警告");
                    return;
                }

                listBind(txtGoodsno.Text.Trim());
            }
        }

        private void listBind(string goodsno)
        {
            System.Data.DataTable goodsnoTable = dal.QueryDataTable(string.Format("SELECT distinct * FROM dbo.T_PhotographyDetail WHERE goodsno='{0}'", goodsno), new object[] { });

            if (ckIsRepair.IsChecked == false)
            {
                if (goodsnoTable.Rows.Count > 0)
                {
                    MessageBox.Show(string.Format("该款号已拍,请√选补拍", "提示"));
                    return;
                }
            }

            //获取该款号的颜色
            string sql = string.Format("select distinct Color,'{0}' as goodsno from dbo.T_WareSpecList where WareNo='{0}'", goodsno);
            System.Data.DataTable source = dal.QueryDataTable(sql, new object[] { });

            if (ckIsRepair.IsChecked == false)
            {
                if (source.Rows.Count > 0)
                {
                    if (source.Rows.Count > 1)
                    {
                        List<string> color = new List<string>();
                        foreach (DataRow dr in source.Rows)
                        {
                            color.Add(dr["Color"].ToString());
                        }

                        if (MessageBox.Show(string.Format("该款有{0}种颜色:{1},是否添加?", source.Rows.Count, String.Join(",", color.ToArray())), "警告", MessageBoxButton.YesNo) == MessageBoxResult.No)
                        {
                            txtGoodsno.Text = "";
                            txtGoodsno.Focus();
                            return;
                        }
                    }

                    //列表中是否存在此款号
                    if (rdCollection.Where(a => a.goodsno.Equals(goodsno)).Count() > 0)
                    {
                        MessageBox.Show("此商品已存在列表中！", "警告");
                        txtGoodsno.Text = "";
                        txtGoodsno.Focus();
                        return;
                    }

                    DataRow[] drs = source.Select("Color like '%有%'");

                    //先添加有配饰，有腰带，有毛领的商品
                    foreach (DataRow dr in drs)
                    {
                        string brandEN = GoodsHelper.BrandEn(goodsno);
                        BrandModel bModel = brandlist.Single(a => a.BrandEN.Equals(brandEN));
                        string brandCN = bModel != null ? bModel.BrandCN : "";
                        rdCollection.Add(new PhotographyDetailModel() { goodsno = goodsno, num = 1, color = dr["Color"].ToString(), brandEN = brandEN, brandCN = brandCN });

                    }

                    foreach (DataRow dr in source.Rows)
                    {

                        if (rdCollection.Where(a => a.color.Contains(dr["Color"].ToString()) && a.goodsno.Equals(dr["Goodsno"].ToString())).Count().Equals(0))
                        {
                            string brandEN = GoodsHelper.BrandEn(goodsno);
                            BrandModel bModel = brandlist.Single(a => a.BrandEN.Equals(brandEN));
                            string brandCN = bModel != null ? bModel.BrandCN : "";
                            rdCollection.Add(new PhotographyDetailModel() { goodsno = goodsno, num = 1, color = dr["Color"].ToString(), brandEN = brandEN, brandCN = brandCN });
                        }
                    }

                    txtGoodsno.Text = "";
                    txtGoodsno.Focus();

                }
            }
            else
            {
                if (source.Rows.Count > 1)
                {
                    //DataTable dtNew = source.Clone();
                    //foreach (DataRow dr in source.Rows)
                    //{
                    //   DataRow [] drs=  goodsnoTable.Select(string.Format("color='{0}'",dr["Color"]));
                    //   if (drs.Count() == 0)
                    //   {
                    //       dtNew.Rows.Add(dr.ItemArray);
                    //   }
                    //}
                    //if (dtNew.Rows.Count == 0)
                    //{
                    //    MessageBox.Show("当前没有可补拍的颜色");
                    //}

                    _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new PhotoOutColorDialog(source), false, 2);
                }
            }
        }


        private void btn_RemoveItem(object sender, RoutedEventArgs e)
        {
            PhotographyDetailModel photoItem = photoGoodslist.SelectedItem as PhotographyDetailModel;
            rdCollection.Remove(photoItem);
            txtGoodsno.Focus();
        }

        private void btn_submit(object sender, RoutedEventArgs e)
        {
            //获取拍摄方式
            string type = txtModel.Text.Trim();
            if (type.Equals("拍摄方式"))
            {
                MessageBox.Show("请选择拍摄方式", "提示");
                return;
            }

            List<string> sql = new List<string>();

            string photoNo = string.Empty;
            PhotographyModel pm = null;

            if (currentOperation == OperationStatus.ADD)
            {
                //获取拍照编号
                string dateSN = DateTime.Now.ToString("yyMMdd");
                string sn = dal.QueryDataTable(string.Format("select COUNT(*)+1 as 'no' from dbo.T_Photography where substring(photoid,1,6)='{0}'", dateSN)).Rows[0][0].ToString();
                photoNo = dateSN + StringHelper.FillZero(sn, 4);
                pm = new PhotographyModel()
                {
                    photoid = photoNo,
                    tType = type,
                    createTime = DateTime.Now,
                    createEmp = CommonLogin.CommonUser.UserName,
                    catNum = 0,
                    ImageRepairNum = 0
                };
            }
            else if (currentOperation == OperationStatus.Edit)
            {
                photoNo = photography.photoid;

                pm = new PhotographyModel()
                {
                    photoid = photography.photoid,
                    tType = type,
                    createTime = DateTime.Now,
                    createEmp = CommonLogin.CommonUser.UserName,
                    catNum = 0,
                    ImageRepairNum = 0
                };

                sql.Add(string.Format("delete T_Photography where photoid='{0}'", photoNo));
                sql.Add(string.Format("delete T_PhotographyDetail where photoid='{0}'", photoNo));
            }

            ObservableCollection<PhotographyDetailModel> detailItems = photoGoodslist.ItemsSource as ObservableCollection<PhotographyDetailModel>;


            //品牌所有集合
            String brandCnCollection = String.Join(",", detailItems.Select(x => x.brandCN).Distinct().ToArray());
            //所有款数
            int kuanCount = detailItems.Select(p => p.goodsno).ToList().Distinct().Count();


            sql.Add(string.Format("insert into T_Photography(photoid,tType,catNum,createTime,createEmp,ImageRepairNum,photoNumber,kuanNumber,brandColl) values('{0}','{1}',{2},'{3}','{4}',{5},{6},{7},'{8}')", pm.photoid, pm.tType, pm.catNum, pm.createTime, pm.createEmp, pm.ImageRepairNum, detailItems.Count, kuanCount, brandCnCollection));



            foreach (PhotographyDetailModel di in detailItems)
            {
                sql.Add(string.Format("insert into T_PhotographyDetail(photoid,goodsno,color,num,brandEn,brandCN) values('{0}','{1}','{2}',1,'{3}','{4}')", photoNo, di.goodsno, di.color, di.brandEN, di.brandCN));
            }

            if (dal.ExecuteTransaction(sql, null))
            {
                rdCollection.Clear();
                PhotoOut fatherDialog = (PhotoOut)FormInit.FindFather(this);
                fatherDialog.LoadData(Convert.ToInt32(fatherDialog.CurrentPage), fatherDialog.PageSize);
                this.CloseDialog();
            }


        }

        private void btn_addGoods(object sender, RoutedEventArgs e)
        {
            if (txtGoodsno.Text.Trim() == "")
            {
                MessageBox.Show("商品编号不能为空！", "警告");
                return;
            }

            listBind(txtGoodsno.Text.Trim());
        }
    }
}
