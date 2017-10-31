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
using _9M.Work.Model;
using _9M.Work.DbObject;
using System.IO;
using _9M.Work.WPF_Common;

namespace _9M.Work.WPF_Main.Views.EveryDayUpdate
{
    /// <summary>
    /// FuDaiGoodsEdit.xaml 的交互逻辑
    /// </summary>
    public partial class FuDaiGoodsEdit : UserControl,BaseDialog
    {
        private BaseDAL dal = new BaseDAL();
        private FuDaiGoodsModel model;
        public FuDaiGoodsEdit(FuDaiGoodsModel model)
        {
            InitializeComponent();
            this.DataContext = this;
            this.model = model;
            Init(model);
        }

        public void Init(FuDaiGoodsModel model)
        {
            tb_BatchName.Text = dal.GetSingle<FuDaiBatchModel>(x=>x.ID==model.BatchID).BatchName;
            tb_Brand.Text = model.Brand;
            tb_Category.Text = model.CategoryName;
            tb_SellCount.Text = model.SellMore.ToString();
            tb_GoodsNoALL.Text = model.GoodsALL;
            tb_IsSell.Text = model.IsSell?"是":"否";
            tb_Price.Text = model.Price.ToString();
            tb_Size.Text = model.Size.ToString();
            com_Class.Text = model.Class;
            lab_Time.Content = model.CreateTime.ToString();

            //字节绑定不会占用图片。可以删除
            string Url = System.IO.Path.Combine(CommonLogin.RemoteDir, model.GoodsNo + @"\\" + model.ImageUrl + ".jpg");
            if (File.Exists(Url))
            {
                img_panel.Source = new BitmapImage(new Uri(Url, UriKind.RelativeOrAbsolute));
            }
            
        }

        public DelegateCommand CancelCommand
        {
            get
            {
                return new DelegateCommand(CloseDialog);
            }
        }

        public string Title
        {
            get
            {
                return "修改福袋";
            }
        }

        public void CloseDialog()
        {
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           int Tag =   Convert.ToInt32((sender as Button).Tag);
            switch (Tag)
            {
                case 0:
                    FuDaiGoodsModel fu = model;
                    fu.Class = com_Class.Text;
                    fu.Price = Convert.ToDecimal(tb_Price.Text);
                    fu.Size = tb_Size.Text;
                    dal.Update<FuDaiGoodsModel>(fu);
                    var b =  (FormInit.FindFather(this) as FuDaiGoodsUp).FudaiGoodsTemp;
                    b.LoadGoodsData(1,30);
                    CloseDialog();
                    break;
                case 1:
                    CloseDialog();
                    break;
            }
        }
    }
}
