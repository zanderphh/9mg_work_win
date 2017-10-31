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
using _9M.Work.Utility;
using _9Mg.Work.TopApi;
using Top.Api.Domain;
using _9M.Work.Model;
using _9M.Work.TopApi;

namespace _9M.Work.WPF_Main.Views.Activity
{
    /// <summary>
    /// TaoBaoGoodsNoSet.xaml 的交互逻辑
    /// </summary>
    public partial class TaoBaoGoodsNoSet : UserControl, BaseDialog
    {
        string activityNo;
        ShopModel shop;
        public TaoBaoGoodsNoSet(ShopModel shop, string activityNo)
        {
            InitializeComponent();
            this.DataContext = this;
            this.shop = shop;
            this.activityNo = activityNo;
        }

        public DelegateCommand CancelCommand
        {
            get { return new DelegateCommand(CloseDialog); }
        }

        public string Title
        {
            get
            {
                return "同步淘宝款号";
            }
        }

        public void CloseDialog()
        {
            FormInit.CloseDialog(this, 2);
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
            int Tag = Convert.ToInt32((sender as Button).Tag);
            switch (Tag)
            {
                case 0:
                    string Text = rich_GoodsNo.Text;
                    if (string.IsNullOrEmpty(Text))
                    {
                        CCMessageBox.Show("请填写款号");
                        return;
                    }
                    List<string> GoodsList = GoodsHelper.GoodsListByLine(rich_GoodsNo.Text);
                    ActivitySet ac = (ActivitySet)FormInit.FindFather(this, typeof(ActivitySet));
                    TopSource com = new TopSource(shop);
                    List<Item> itemlist = com.GetItemList(GoodsList, string.Empty);
                    if (itemlist == null)
                    {
                        CCMessageBox.Show("查询遇到错误");
                        return;
                    }
                    List<ComboBox> comlist = WPF_Common.WPFControlsSearchHelper.GetChildObjects<ComboBox>(ac.Sta_Start, "");
                    List<ActivityGoodsModel> glist = new List<ActivityGoodsModel>();
                    itemlist.ForEach(x =>
                    {
                        ActivityGoodsModel model = new ActivityGoodsModel()
                        {
                            GoodsNo = x.OuterId,
                            Defaultprice = Convert.ToDecimal(x.Price),
                            Defaulttitle = x.Title,
                            ActivityNo = activityNo,
                            Activittitle = !string.IsNullOrEmpty(ac.tb_appendtitle.Text) && comlist[3].SelectedIndex ==1 ? GoodsHelper.RetrunActityTitle(x.Title, ac.tb_appendtitle.Text) : x.Title
                        };
                        glist.Add(model);
                    });
                    ac.GoodsList = glist;
                    ac.BindGrid(OperationStatus.ADD);
                    CloseDialog();
                    break;
                case 1:
                    CloseDialog();
                    break;
            }
        }
    }
}
