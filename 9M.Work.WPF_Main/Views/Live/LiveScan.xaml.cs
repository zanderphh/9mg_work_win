using _9M.Work.DbObject;
using _9M.Work.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace _9M.Work.WPF_Main.Views.Live
{
    /// <summary>
    /// Interaction logic for LiveScan.xaml
    /// </summary>
    public partial class LiveScan : UserControl
    {
        BaseDAL dal = new BaseDAL();

        #region 属性

        private ObservableCollection<LiveCheckModel> _liveCheckCollection = new ObservableCollection<LiveCheckModel>();

        public ObservableCollection<LiveCheckModel> liveCheckCollection
        {
            get { return _liveCheckCollection; }
            set
            {
                if (_liveCheckCollection != value)
                {
                    _liveCheckCollection = value;
                    this.OnPropertyChanged("liveCheckCollection");
                }
            }
        }




        #endregion

        public LiveScan()
        {
            InitializeComponent();
            this.DataContext = this;
            Bind();
            txtScanGoodsNo.Focus();
        }

        public void Bind()
        {
            ScanDG.ItemsSource = liveCheckCollection;

            List<ExpressionModelField> listWhere = new List<ExpressionModelField>();
            listWhere.Add(new ExpressionModelField() { Name = "batchNo", Value = 1 });

            var datasource = dal.GetList<LiveCheckModel>(listWhere.ToArray());

            foreach (LiveCheckModel m in datasource)
            {
                liveCheckCollection.Add(m);
            }
        }

        private void txtScanGoodsNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtScanGoodsNo.Text.Trim() == "")
                {
                    MessageBox.Show("商品编号不能为空！", "警告");
                    return;
                }


                bool bFind = liveCheckCollection.Any<LiveCheckModel>(p => p.goodsno.ToUpper()==txtScanGoodsNo.Text.Trim().ToUpper());
                if (bFind)
                {
                    var removeModel = liveCheckCollection.Single(p => p.goodsno.ToUpper() == txtScanGoodsNo.Text.Trim().ToUpper());
                    var addModel = removeModel;
                    addModel.num++;
                    liveCheckCollection.Remove(removeModel);
                    liveCheckCollection.Insert(0, addModel);
                    txtScanGoodsNo.Text= "";
                    txtScanGoodsNo.Focus();
                }
                else
                {
                    MessageBox.Show("列表中无此商品","提示");
                }
            }
        }
    }
}
