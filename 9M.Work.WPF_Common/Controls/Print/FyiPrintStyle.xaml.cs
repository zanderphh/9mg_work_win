using _9M.Work.ErpApi;
using _9M.Work.Model;
using _9M.Work.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Printing;
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
using System.Windows.Threading;

namespace _9M.Work.WPF_Common.Controls.Print
{
    /// <summary>
    /// FyiPrintStyle.xaml 的交互逻辑
    /// </summary>
    public partial class FyiPrintStyle : UserControl
    {
        private GoodsManager manager = new GoodsManager();
        private BackgroundWorker _backTask = new BackgroundWorker();
        private BarCode.Code128 code128 = new BarCode.Code128();
        private GoodsModel data = null;
        private PrintDialog dialog = new PrintDialog();
        public double TemplateWidth { get; set; }
        public double TemplateHeight { get; set; }
        public FyiPrintStyle()
        {
            InitializeComponent();
            _backTask.DoWork += _backTask_DoWork;
            _backTask.RunWorkerCompleted += _backTask_RunWorkerCompleted;
        }

        private void _backTask_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Send, new Action(
            delegate
            {
                lab_brand.Text = "KS";
                lab_goodsno.Text = "33SC0001";
                Img_Code.Source = ImageConvert.BitmapToBitmapImage(code128.GetCodeImage("33SC0001124", BarCode.Code128.Encode.Code128B));
            }));
            Thread.Sleep(50);
        }
        private void _backTask_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            PrintTicket pt = dialog.PrintTicket;
            pt.PageMediaSize = new PageMediaSize(TemplateWidth, TemplateHeight);//A4纸
            dialog.PrintVisual(PrintStack, "Print Test1");
        }

        public void Print(string WareNoAll)
        {
            GoodsModel model = manager.GetGoodsAll(WareNoAll);
            if (model == null)
            {
                return;
            }
            this.data = model;
            _backTask.RunWorkerAsync();

        }
    }
}
