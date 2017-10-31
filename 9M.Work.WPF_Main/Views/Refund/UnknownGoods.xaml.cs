using _9M.Work.DbObject;
using _9M.Work.Model;
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

namespace _9M.Work.WPF_Main.Views.Refund
{
    /// <summary>
    /// Interaction logic for unknownGoods.xaml
    /// </summary>
    public partial class unknownGoods : UserControl
    {

        BaseDAL dal = new BaseDAL();

        public unknownGoods()
        {
            InitializeComponent();
            this.DataContext = this;
            ItemSourceBind();
        }


        #region 数据绑定

        public void ItemSourceBind()
        {
            List<ExpressionModelField> fields_or = new List<ExpressionModelField>();
            OrderModelField orderField = new OrderModelField() { PropertyName = "id", IsDesc = true };

            if (txtInput.Text.Trim() != "")
            {
                fields_or.Add(new ExpressionModelField() { Name = "ExpressCode", Value = txtInput.Text.Trim(), Relation = EnumRelation.Contains });
                fields_or.Add(new ExpressionModelField() { Name = "UName", Value = txtInput.Text.Trim(), Relation = EnumRelation.Contains });
                fields_or.Add(new ExpressionModelField() { Name = "Mobile", Value = txtInput.Text.Trim(), Relation = EnumRelation.Contains });
            }

            Dictionary<string, object> dic = new Dictionary<string, object>();

            if (fields_or.Count > 0)
            {
                dic = dal.GetListPaged<UnknownGoodsModel>(1, 20, new ExpressionModelField[] { }, fields_or.ToArray(), new[] { orderField });
            }
            else
            {
                dic = dal.GetListPaged<UnknownGoodsModel>(1, 20, new[] { orderField });
            }

            List<UnknownGoodsModel> list = dic["rows"] as List<UnknownGoodsModel>;
            dg_unknowngoodslist.ItemsSource = list;


        }


        #endregion

        private void btn_Operator(object sender, RoutedEventArgs e)
        {
            object Tag = new object();

            if (sender.GetType() == typeof(Button))
            {
                Tag = ((Button)sender).Tag;
            }
            else if (sender.GetType() == typeof(MenuItem))
            {
                Tag = ((MenuItem)sender).Tag;
            }

            UnknownGoodsModel selectedrow = dg_unknowngoodslist.SelectedItem as UnknownGoodsModel;

            if (Tag.Equals("add"))
            {
                _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new AddUnknownGoods(new UnknownGoodsModel()), true);
            }
            else if (Tag.Equals("edit"))
            {

                if (selectedrow == null)
                {
                    MessageBox.Show("请选择要编辑的行", "提示");
                    return;
                }
                else
                {
                    _9M.Work.WPF_Main.Infrastrcture.FormInit.OpenDialog(this, new AddUnknownGoods(selectedrow), true);
                }
            }
            else if (Tag.Equals("del"))
            {
                if (selectedrow == null)
                {
                    MessageBox.Show("请选择要删除的行", "提示");
                    return;
                }
                else
                {
                    if (MessageBox.Show("删除后数据将无法恢复,是否继续要删除", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        try
                        {
                            if (dal.Delete(selectedrow))
                            {
                                ItemSourceBind();
                            }
                            else
                            {
                                MessageBox.Show("删除数据失败", "提示");
                            }
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show("错误提示:" + err.Message.ToString(), "提示");
                        }

                    }
                }
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ItemSourceBind();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(((TextBlock)sender).Text);
        }
    }

}
