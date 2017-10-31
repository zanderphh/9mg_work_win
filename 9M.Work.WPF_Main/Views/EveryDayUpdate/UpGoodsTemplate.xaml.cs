using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.WPF_Common;
using _9M.Work.WPF_Common.WpfBind;
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

namespace _9M.Work.WPF_Main.Views.EveryDayUpdate
{
    /// <summary>
    /// UpGoodsTemplate.xaml 的交互逻辑
    /// </summary>
    public partial class UpGoodsTemplate : UserControl
    {
        private List<Label> labellist;
        private BaseDAL dal = new BaseDAL(BaseDAL.TemplateConnectionString);
        public UpGoodsTemplate()
        {
            InitializeComponent();
            //得到所有属性值
            labellist = WPFControlsSearchHelper.GetChildObjects<Label>(Panel_Attribute, "").Where(x => x.FontWeight != FontWeights.Bold).ToList();
            labellist.ForEach(x =>
            {
                x.MouseLeftButtonDown += x_MouseLeftButtonDown;
            });
            Init();
        }

        public void Init()
        {
            //类目下拉眶
            var list = dal.QueryList<TemplateCategoryModel>("select * from dbo.tSizeCategory", new object[] { });
            list.Insert(0, new TemplateCategoryModel() { Id = 0, Name = "请选择" });
            ComboBoxBind.BindComboBox(com_category, list, "Name", "Id");
            if (list.Count > 0)
            {
                com_category.SelectedIndex = 0;
            }
        }

        public void BindProperty(int CategoryId)
        {

        }

        //选择属性
        private void x_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Label label = sender as Label;
            string LabelText = label.Content.ToString();
            int index = labellist.FindIndex(x => x.Content.Equals(LabelText));
            int Air = Convert.ToInt32(Math.Ceiling(Convert.ToDouble((index + 1)) / 3));
            for (int i = (Air - 1) * 3; i < Air * 3; i++)
            {
                SolidColorBrush backbrush = i == index ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333")) : new SolidColorBrush(Colors.White);
                SolidColorBrush fontbrush = i == index ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Black); ;
                labellist[i].Background = backbrush;
                labellist[i].Foreground = fontbrush;
            }
        }

        //下拉类目
        private void com_category_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TemplateCategoryModel categorymodel = (sender as ComboBox).SelectedItem as TemplateCategoryModel;
            if (categorymodel.Id != 0)
            {
                SizeGrid.Children.Clear();
                SizeGrid.RowDefinitions.Clear();
                SizeGrid.ColumnDefinitions.Clear();
                string sizesql = string.Format(@"select Id,Size,Label,ClassId from dbo.tSizeCategoryMapping where classid = {0} and ishide=0 order by sort", categorymodel.Id);
                string sizetitlelistsql = string.Format(@"select b.* from  dbo.tSizePreportyMapping a join dbo.tSizePreporty b on a.PreportyId = b.id
                where a.CategoryId={0}", categorymodel.Id);
                List<TemplateSizeModel> sizelist = dal.QueryList<TemplateSizeModel>(sizesql, new object[] { });
                List<TemplateSizeTitleModel> sizetitlelist = dal.QueryList<TemplateSizeTitleModel>(sizetitlelistsql, new object[] { });

                int col = sizetitlelist.Count + 2 > 8 ? sizetitlelist.Count + 2 : 8;
                //开始布局
                for (int i = 0; i < sizelist.Count + 1; i++)
                {
                    RowDefinition row = new RowDefinition();
                    SizeGrid.RowDefinitions.Add(row);
                }
                for (int j = 0; j < col; j++)
                {
                    ColumnDefinition column = new ColumnDefinition();
                    SizeGrid.ColumnDefinitions.Add(column);
                }

                for (int i = 0; i < sizelist.Count + 1; i++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        Border bs = new Border() { BorderBrush = new SolidColorBrush(Colors.Gray), BorderThickness = new Thickness(0.1),Height = 30 };
                        bs.Padding = new Thickness(0.5, 0.5, 0.5, 0.5);
                        //列头值
                        string Content = string.Empty;
                        Label la = new Label() { VerticalAlignment = System.Windows.VerticalAlignment.Center,FontSize = 12};
                        //行值
                        TextBox tb = new TextBox() { FontSize = 12, Height = 22,Foreground = new SolidColorBrush(Colors.Gray)};
                        //如果是第一行那么就是标头
                        if (i == 0)
                        {
                            bs.Height = 40;
                            if (j == 0)
                            {
                                Content = "尺码";
                            }
                            else if (j == 1)
                            {
                                Content = "标码";
                            }
                            else
                            {
                                int index = j - 2;
                                if (index < sizetitlelist.Count)
                                {
                                    Content = sizetitlelist[index].preportyName;
                                }
                            }
                            la.Content = Content;
                            la.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333"));
                            la.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                            la.Foreground = new SolidColorBrush(Colors.White);
                            la.FontSize = 15;
                            la.Height = 32;
                            bs.Child = la;
                        }
                        else
                        {
                            if (j < sizetitlelist.Count + 2)
                            {
                                if (j == 0)//如果是第一列
                                {
                                    tb.Text = sizelist[i - 1].Size;

                                }
                                else if (j == 1)//如果是第二列
                                {
                                    tb.Text = sizelist[i - 1].Label;

                                }
                                else//那么就是值
                                {
                                 
                                }

                                bs.Child = tb;
                            }
                        }
                        Grid.SetRow(bs, i);
                        Grid.SetColumn(bs, j);
                        SizeGrid.Children.Add(bs);
                    }
                }
            }
        }
    }

}
