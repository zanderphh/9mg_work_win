using _9M.Work.Model;
using _9M.Work.ErpApi;
using System.Windows;
using System.Windows.Controls;
using System.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Data;
using _9M.Work.Utility;

namespace _9M.Work.WPF_Main.Views.WareHouse
{
    /// <summary>
    /// StockIn.xaml 的交互逻辑
    /// </summary>
    public partial class StockIn : UserControl, INotifyPropertyChanged
    {
        GoodsManager ware = new GoodsManager();
        private SoundPlayer sp = new SoundPlayer();


        public event PropertyChangedEventHandler PropertyChanged;

        public StockIn()
        {
            InitializeComponent();
            this.DataContext = this;
            GoodsList = new ObservableCollection<GridSourceModel>();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        ObservableCollection<GridSourceModel> _goodslist;
        public ObservableCollection<GridSourceModel> GoodsList
        {
            get
            {
                return this._goodslist;
            }
            set
            {
                if (this._goodslist != value)
                {
                    this._goodslist = value;
                    OnPropertyChanged("GoodsList");
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string Sku = tb_GoodsNo.Text.Trim();
            GoodsModel model = ware.GetGoodsAll(Sku);
            string DeBugUrl = AppDomain.CurrentDomain.BaseDirectory;
            sp.SoundLocation = model == null ? DeBugUrl + @"\Sounds\Error.WAV" : DeBugUrl + @"\Sounds\Success.WAV";
            sp.Play();


            if (model != null)
            {

                GridSourceModel selectedItem = null;
                bool b = true;
                int Index = GoodsList.FindIndex(x => x.Sku.Equals(model.GoodsNo + model.SpecCode, StringComparison.CurrentCultureIgnoreCase));
                if (Index > -1)
                {
                    b = false;
                    GoodsList[Index].GoodsCount = GoodsList[Index].GoodsCount + 1;
                    selectedItem = GoodsList[Index];
                }

                if (b)
                {
                    selectedItem = new GridSourceModel() { GoodsNo = model.GoodsNo, Sku = model.GoodsNo + model.SpecCode, GoodsName = model.GoodsName, SpecName = model.SpecName, GoodsCount = 1, Positions = "", Remark = "", Unit = "件" };
                    GoodsList.Add(selectedItem);
                }

                //首先滚动为末行  
                //BatchGrid.SelectedItem = BatchGrid.Items[BatchGrid.Items.Count - 1];
                //BatchGrid.CurrentColumn = BatchGrid.Columns[0];
                //BatchGrid.ScrollIntoView(BatchGrid.SelectedItem, BatchGrid.CurrentColumn);

                //获取焦点，滚动为目标行  
                BatchGrid.Focus();
                BatchGrid.SelectedItem = selectedItem;
                BatchGrid.CurrentColumn = BatchGrid.Columns[0];
                BatchGrid.ScrollIntoView(BatchGrid.SelectedItem, BatchGrid.CurrentColumn);  

            }
            else
            {
                CCMessageBox.Show("不存在的商品");
            }
            tb_GoodsNo.Clear();
            tb_GoodsNo.Focus();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("编号", typeof(string));
            dt.Columns.Add("品名", typeof(string));
            dt.Columns.Add("规格", typeof(string));
            dt.Columns.Add("单位", typeof(string));
            dt.Columns.Add("数量", typeof(int));
            dt.Columns.Add("货位", typeof(string));
            dt.Columns.Add("备注", typeof(string));
            if (GoodsList.Count > 0)
            {
                foreach (var item in GoodsList)
                {
                    DataRow dr = dt.NewRow();
                    dr["编号"] = item.GoodsNo;
                    dr["品名"] = item.GoodsName;
                    dr["规格"] = item.SpecName;
                    dr["单位"] = item.Unit;
                    dr["数量"] = item.GoodsCount;
                    dr["货位"] = item.Positions;
                    dr["备注"] = item.Remark;
                    dt.Rows.Add(dr);
                }
            }
            string res = string.Empty;
            try
            {
                ExcelNpoi.TableToExcelForXLS(dt, Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"/入库单" + System.Guid.NewGuid() + ".xls");
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            CCMessageBox.Show(string.IsNullOrEmpty(res) ? "表格生成在桌面" : "生成错误\r\n" + res);
        }

        private void tb_GoodsNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click(sender, new RoutedEventArgs());
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("是否要清空表格","提示",MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                GoodsList.Clear();
            }
         
        }

        private void MenuClick(object sender, RoutedEventArgs e)
        {
            string oper = ((MenuItem)sender).Tag.ToString();

            GridSourceModel selectedrow = BatchGrid.SelectedItem as GridSourceModel;

            if (selectedrow != null)
            {
                if (oper.Equals("0"))
                {
                    if (MessageBox.Show("确认要继续？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        GoodsList.Remove(selectedrow);
                    }
                }
                else if (oper.Equals("1"))
                {
                    if (selectedrow.GoodsCount.Equals(1))
                    {
                        GoodsList.Remove(selectedrow);
                    }
                    else
                    {
                        selectedrow.GoodsCount = selectedrow.GoodsCount - 1;
                    }
                }
            }
            else
            {
                MessageBox.Show("未选中行，请先选择行数据后再操作", "提示");
            }
            
        }
    }

    public class GridSourceModel : INotifyPropertyChanged
    {

        public string Sku { get; set; }
        public string GoodsNo { get; set; }
        public string GoodsName { get; set; }
        public string SpecName { get; set; }
        public string Unit { get; set; }
        private int _goodsCount;
        public string Positions { get; set; }
        public string Remark { get; set; }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public int GoodsCount
        {
            get { return _goodsCount; }
            set
            {
                if (_goodsCount != value)
                {
                    _goodsCount = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("GoodsCount"));
                }
            }
        }
    }
}
