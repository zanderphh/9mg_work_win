using _9M.Work.DbObject;
using _9M.Work.ErpApi;
using _9M.Work.Model;
using _9M.Work.Model.WdgjWebService;
using _9M.Work.WPF_Common;
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


namespace _9M.Work.WPF_Main.Views.EveryDayUpdate.FuDaiTemplate
{
    /// <summary>
    /// UpGoods.xaml 的交互逻辑
    /// </summary>
    public partial class UpGoods : UserControl
    {
        private BaseDAL dal = new BaseDAL();
        private WdgjSource wdgj = new WdgjSource();
        public UpGoods()
        {
            InitializeComponent();
            BindCheckListBox(5);
        }


        public void BindCheckListBox(int RowCount)
        {
            Panel_Brand.Children.Clear();
            List<string> brandlist = dal.GetAll<FuDaiBatchModel>().GroupBy(x => x.Brand).Select(x => x.Key).ToList();
            int Count = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(brandlist.Count) / RowCount));
            for (int i = 0; i < Count; i++)
            {
                StackPanel stack = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 10, 0, 0) };
                for (int j = 0; j < RowCount; j++)
                {
                    int Index = i * RowCount + j;
                    if (Index == brandlist.Count)
                    {
                        break;
                    }
                    CheckBox check = new CheckBox() { Content = brandlist[Index], Margin = new Thickness(15, 0, 0, 0), FontWeight = FontWeights.Bold };
                    stack.Children.Add(check);
                }
                Panel_Brand.Children.Add(stack);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<string> brandlist = WPFControlsSearchHelper.GetChildObjects<CheckBox>(Panel_Brand, "").Where(x => x.IsChecked == true).Select(x => x.Content.ToString()).ToList();
            RadioButton classradio = WPFControlsSearchHelper.GetChildObjects<RadioButton>(Radio_Class, "").Where(x => x.IsChecked == true).FirstOrDefault(); ;
            List<WrapPanel> wraplist = WPFControlsSearchHelper.GetChildObjects<WrapPanel>(Radio_Size, "");
            Dictionary<string, int> dic = new Dictionary<string, int>();
            string Class = string.Empty;

            if (classradio != null)
            {
                Class = classradio.Content.ToString().Contains("冬") ? "冬" : "春夏秋";
            }

            wraplist.ForEach(x =>
            {
                string Size = (x.Children[0] as RadioButton).Content.ToString();
                string flag = (x.Children[1] as TextBox).Text;
                int Count = string.IsNullOrEmpty(flag) ? 0 : Convert.ToInt32(flag);
                dic.Add(Size, Count);
            });
            int Tag = Convert.ToInt32((sender as Button).Tag);

            switch (Tag)
            {
                case 0: //福袋预览

                    //点上架的时候先同步管家库存(因为每个福袋的库存都是1.如果管家库存为0那么就是己销售)
                    List<FuDaiGoodsModel> fudailist = dal.GetList<FuDaiGoodsModel>(x => x.IsSell == false).ToList();
                    List<string> GoodsList = fudailist.GroupBy(x => x.GoodsNo).Select(x => x.Key).ToList();
                    List<V_GoodsSpecModel> wdgjlist = wdgj.SpecList(new SpecListRequest() { GoodsNoList = GoodsList });
                    var NeedUpdateList = (from pp in fudailist
                                where wdgjlist.Where(x => x.Stock == 0).Select(x => x.GoodsNoAll).Contains(pp.GoodsALL)
                                select pp.GoodsALL).ToList();
                  
                    if (NeedUpdateList.Count>0)
                    {
                        string updatesql = string.Format(@"update T_FuDaiGoods set Issell=1 where GoodsALL in ('{0}')",string.Join(",",NeedUpdateList.ToArray()).Replace(",","','"));
                        dal.ExecuteSql(updatesql);
                    }

                    List < ExpressionModelField > Fieldlist = new List<ExpressionModelField>();
                    if (!string.IsNullOrEmpty(Class))
                    {
                        Fieldlist.Add(new ExpressionModelField() { Name = "Class", Relation = EnumRelation.Equal, Value = Class });
                    }
                    if (brandlist.Count > 0)
                    {
                        brandlist.ForEach(x=> {
                            Fieldlist.Add(new ExpressionModelField() { Name = "Brand", Relation = EnumRelation.Equal, Value = x });
                        });
                    }
                    Fieldlist.Add(new ExpressionModelField() { Name = "IsSell", Relation = EnumRelation.Equal, Value = false });
                    List<FuDaiGoodsModel> goodslist = dal.GetList<FuDaiGoodsModel>(Fieldlist.ToArray(), new OrderModelField[] { new OrderModelField() { IsDesc = false, PropertyName = "CreateTime" } });
                    List<FuDaiGoodsModel> list = new List<FuDaiGoodsModel>();
                    //选款
                    foreach (var Item in dic)
                    {
                        string Size = Item.Key;
                        int Count = Item.Value;
                        
                        list.AddRange(goodslist.Where(x => x.Size.Equals(Size)).Skip(0).Take(Count).ToList());
                    }
                    if (list.Count > 0)
                    {
                        FormInit.OpenDialog(this, new FuDaiUpGoodsPreview(list), false);
                    }
                    else
                    {
                        CCMessageBox.Show("没有符合条件的预览");
                    }
                    break;
                case 1:
                    break;
            }
        }
    }
}
