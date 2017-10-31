using _9M.Work.ErpApi;
using _9M.Work.Model;
using _9M.Work.WPF_Main.FrameWork;
using _9M.Work.WPF_Main.Views.Print;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Media;
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
using System.Windows.Threading;

namespace _9M.Work.WPF_Main.Views.WareHouse
{
    /// <summary>
    /// TradeCheck.xaml 的交互逻辑
    /// </summary>
    public partial class TradeCheck : UserControl, INotifyPropertyChanged
    {
        private SoundPlayer sp = new SoundPlayer();
        GoodsManager manager = new GoodsManager();
        List<GoodsModel> list = null;
        PrintLabel lab = new PrintLabel();
        PrintHelper ph = new PrintHelper();
        public TradeCheck()
        {
            InitializeComponent();
            this.DataContext = this;
            //让文本框得到焦点
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                 (Action)(() => { Keyboard.Focus(tb_Billd); }));
            Keyboard.Focus(tb_Billd);
        }

        public ICommand QueryCommand
        {
            get { return new DelegateCommand(QueryStockInBiLL); }
        }

        public ICommand QueryCommandCode
        {
            get { return new DelegateCommand(QueryStockInGoodsNo); }
        }

        private void QueryStockInGoodsNo()
        {
            if (list != null)
            {
                string Loation = string.Empty;
                string GoodsDetail = tb_GoodsNoAll.Text.Trim();
                
                int Index = list.FindIndex(x => (x.GoodsNo + x.SpecCode).Equals(GoodsDetail, StringComparison.CurrentCultureIgnoreCase));
                if (Index > -1)
                {
                   DataGridRow dr =   ControlHelper.GetRow(BatchGrid,Index);
                    BatchGrid.SelectedItem = list[Index];
                    //校验数量
                    TextBox tb = ControlHelper.FindGridTemplateControl(BatchGrid, 6, Index, "tb_xiaoyan") as TextBox;
                    string Count = string.IsNullOrEmpty(tb.Text)?"1":(Convert.ToInt32(tb.Text)+1).ToString();
                    tb.Text = Count.ToString();
                    //实际数量
                    int Rcount = Convert.ToInt32((ControlHelper.FindGridControl(BatchGrid, 5, Index) as TextBlock).Text);
                    if (Convert.ToInt32( tb.Text) <= Rcount)
                    {
                        Loation = AppDomain.CurrentDomain.BaseDirectory + @"Sounds\Success.WAV";
                        dr.Background = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        Loation = AppDomain.CurrentDomain.BaseDirectory + @"Sounds\Error.WAV";
                        dr.Background = new SolidColorBrush(Colors.Red);
                    }
                    //打印标签
                    ph.PrintLabel(new List<string>() { GoodsDetail });
                    tb_GoodsNoAll.Clear();
                    tb_GoodsNoAll.Focus();
                }
                else //没有获取到货品
                {
                    Loation = AppDomain.CurrentDomain.BaseDirectory + @"Sounds\Error.WAV";
                    tb_GoodsNoAll.Clear();
                    tb_GoodsNoAll.Focus();
                }
                if (!System.IO.File.Exists(Loation))
                {
                    CCMessageBox.Show("没有音频文件");
                    return;
                }
                sp.SoundLocation = Loation;
                sp.Play();
            }
        }

        private void QueryStockInBiLL()
        {
            string tradeid = tb_Billd.Text;
            if (!string.IsNullOrEmpty(tradeid))
            {
                list = manager.GoodsByTrade(tradeid).OrderByDescending(x => x.Sellcount).ToList();
                if (list.Count > 0)
                {
                    BatchGrid.ItemsSource = list;
                    tb_GoodsNoAll.Focus();
                }
            }
        }

        private void tb_Billd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                QueryStockInBiLL();
            }
        }

        private void tb_GoodsNoAll_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                QueryStockInGoodsNo();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Button_KeepTrade_Cache(object sender, RoutedEventArgs e)
        {
            int tradecount = 0;
            int realcount = 0;
            for (int i = 0; i < BatchGrid.Items.Count; i++)
            {
                string t = (ControlHelper.FindGridControl(BatchGrid,5,i) as TextBlock).Text;
                string r = (ControlHelper.FindGridTemplateControl(BatchGrid, 6, i, "tb_xiaoyan") as TextBox).Text;
                int tcount = 0;
                int rcount = 0;
                int.TryParse(t, out tcount);
                int.TryParse(r, out rcount);
                tradecount += tcount;
                realcount += rcount;
            }
            string res = realcount != tradecount ? "校验失败" : "校验成功";
            MessageBox.Show(res);
        }
    }
}
