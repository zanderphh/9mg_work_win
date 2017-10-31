using _9M.Work.DbObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
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

namespace _9M.Work.WPF_Main.Views.QualityCheck
{
    /// <summary>
    /// Interaction logic for ShoesScan.xaml
    /// </summary>
    public partial class ShoesScan : UserControl
    {

        BaseDAL dal = new BaseDAL();

        public ShoesScan()
        {
            InitializeComponent();
            txtGoodsno.Focus();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtGoodsno.Text.Trim() != null)
                {
                    string goodsno = txtGoodsno.Text.Trim().Substring(0, 8);

                    int SN = Convert.ToInt32(dal.QueryDataTable(string.Format(@"if exists (select sn from dbo.T_ShoesScan where goodsno='{0}')
begin
    select sn from dbo.T_ShoesScan where goodsno='{0}'
end
else
begin

    insert into dbo.T_ShoesScan(goodsno,sn) values('{0}',(select ISNULL(max(sn),0)+1 from dbo.T_ShoesScan))

    select ISNULL(max(sn),0) as sn from dbo.T_ShoesScan
    
  end", goodsno)).Rows[0][0]);

                    labSN.Content = SN.ToString();

                    SpeechSynthesizer synth = new SpeechSynthesizer();
                    synth.Volume = 100;
                
                    synth.SpeakAsync(SN.ToString());
                 

                    txtGoodsno.Text = "";
                    txtGoodsno.Focus();
                }


            }
        }
    }
}
