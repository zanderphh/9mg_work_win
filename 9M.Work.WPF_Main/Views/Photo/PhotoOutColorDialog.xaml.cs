using _9M.Work.Model;
using _9M.Work.Utility;
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
    /// Interaction logic for PhotoOutColorDialog.xaml
    /// </summary>
    public partial class PhotoOutColorDialog : UserControl, BaseDialog
    {

        #region 属性

        private ObservableCollection<colorItem> _items = new ObservableCollection<colorItem>();

        public ObservableCollection<colorItem> Items
        {
            get { return _items; }
            set
            {
                if (_items != value)
                {
                    _items = value;
                    this.OnPropertyChanged("Items");
                }
            }
        }
        #endregion

        public PhotoOutColorDialog(DataTable dt)
        {
            InitializeComponent();
            this.DataContext = this;
            foreach (DataRow dr in dt.Rows)
            {
                Items.Add(new colorItem() { color = dr["Color"].ToString(), isSelected = false, goodsno = dr["goodsno"].ToString() });
            }

        }

        #region Dialog
        public Microsoft.Practices.Prism.Commands.DelegateCommand CancelCommand
        {
            get { return new DelegateCommand(CloseDialog); }
        }

        public void CloseDialog()
        {
            FormInit.CloseDialog(this, 2);
        }

        public string Title
        {
            get { return "颜色选择"; }
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

        private void btn_ok(object sender, RoutedEventArgs e)
        {
            PhotoOutDialog fatherDialog = (PhotoOutDialog)FormInit.FindFather(this, typeof(PhotoOutDialog));
            System.Collections.IEnumerator ie = photoGoodslist.ItemsSource.GetEnumerator();
            while (ie.MoveNext())
            {
                colorItem item = ie.Current as colorItem;

                if (item.isSelected.Equals(true))
                {
                    //列表中是否存在此款号
                    if (fatherDialog.rdCollection.Where(a => a.goodsno.Equals(item.goodsno) && a.color.Equals(item.color)).Count() > 0)
                    {
                        MessageBox.Show("此商品已存在列表中！", "警告");
                        fatherDialog.txtGoodsno.Text = "";
                        fatherDialog.txtGoodsno.Focus();
                        return;
                    }

                    string brandEN = GoodsHelper.BrandEn(item.goodsno);


                    BrandModel bModel = new _9M.Work.DbObject.BaseDAL().GetSingle<BrandModel>(a => a.BrandEN.Equals(brandEN));
                    string brandCN = bModel != null ? bModel.BrandCN : "";
                    fatherDialog.rdCollection.Add(new PhotographyDetailModel() { color = item.color, num = 1, goodsno = item.goodsno, brandEN = brandEN, brandCN = brandCN });
                    
                    fatherDialog.txtGoodsno.Text = "";
                    fatherDialog.txtGoodsno.Focus();
                }
            }
            CloseDialog();
        }

        private void ck_click(object sender, RoutedEventArgs e)
        {
            var item = photoGoodslist.SelectedItem as colorItem;
            if (item != null)
            {
                item.isSelected = !item.isSelected;
            }
        }
    }


    public class colorItem
    {
        public string goodsno { get; set; }
        public string color { get; set; }
        public bool isSelected { get; set; }
    }
}
