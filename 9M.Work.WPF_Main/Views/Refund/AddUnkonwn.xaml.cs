using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.WPF_Common.ValueObjects;
using _9M.Work.WPF_Main.Infrastrcture;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
    /// Interaction logic for AddUnkonwn.xaml
    /// </summary>
    public partial class AddUnkonwn : UserControl, BaseDialog
    {

        BaseDAL dal = new BaseDAL();

        public string Golbal_RefundNo = string.Empty;

        public AddUnkonwn(string RefundNo)
        {
            InitializeComponent();
            this.DataContext = this;
            Golbal_RefundNo = RefundNo;
            //Bind();
        }

        #region Dialog
        public Microsoft.Practices.Prism.Commands.DelegateCommand CancelCommand
        {
            get { return new DelegateCommand(CloseDialog); }
        }

        public void CloseDialog()
        {
            FormInit.CloseDialog(this, 2);
        }

        public string Title
        {
            get { return "未知款添加"; }
        }

        public bool IsNavigationTarget(Microsoft.Practices.Prism.Regions.NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedFrom(Microsoft.Practices.Prism.Regions.NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedTo(Microsoft.Practices.Prism.Regions.NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 选择图片
        public string OpenFileDialog(string _filetype)
        {
            Microsoft.Win32.OpenFileDialog op = new Microsoft.Win32.OpenFileDialog();
            op.RestoreDirectory = true;
            op.Filter = _filetype;
            op.ShowDialog();
            return op.FileName;
        }

        private void btn_selected(object sender, RoutedEventArgs e)
        {
            string filetype = "图片文件(*.jpg,*.png)|*.jpg;*.png";
            string imgpath = OpenFileDialog(filetype);
            if (!string.IsNullOrEmpty(imgpath))
            {
                System.Windows.Controls.Image img = (System.Windows.Controls.Image)this.FindName(((Button)sender).Tag.ToString());
                img.Source = new BitmapImage(new Uri(imgpath, UriKind.RelativeOrAbsolute));
            }
        }
        #endregion

        #region 保存

        private void btn_Save(object sender, RoutedEventArgs e)
        {

            List<string> sql = new List<string>();

            List<string> imgPath = new List<string>();
            IEnumerator ie = imageGrid.Children.GetEnumerator();

            while (ie.MoveNext())
            {
                if (ie.Current.GetType() == typeof(System.Windows.Controls.Image))
                {
                    ImageSource iSource = (ie.Current as System.Windows.Controls.Image).Source;
                    if (iSource.GetType() != typeof(System.Windows.Interop.InteropBitmap))
                    {
                        if (!iSource.ToString().Contains("selected.png"))
                        {
                            imgPath.Add(iSource.ToString().Replace(@"file:///", ""));
                        }
                    }
                }
            }

            if (imgPath.Count > 0)
            {

                int t = 0;

                imgPath.ForEach(delegate(string s)
                {
                    FileStream fs = new FileStream(s, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    Byte[] buffer = br.ReadBytes((int)fs.Length);

                    using (SqlConnection conn = new SqlConnection(BaseDAL.DBConnectionString))
                    {
                        try
                        {
                            conn.Open();
                            SqlCommand cmd = new SqlCommand();
                            cmd.Connection = conn;
                            cmd.CommandText = string.Format("insert into dbo.T_Unknownlist(refundNo,img,title)values(@RefundNo,@buffer,@title)", Golbal_RefundNo, buffer, txtDESC.Text);

                            SqlParameter par = new SqlParameter("@buffer", SqlDbType.Image);
                            par.Value = buffer;
                            cmd.Parameters.Add(par);

                            SqlParameter par1 = new SqlParameter("@RefundNo", SqlDbType.VarChar);
                            par1.Value = Golbal_RefundNo;
                            cmd.Parameters.Add(par1);

                            SqlParameter par2 = new SqlParameter("@title", SqlDbType.NVarChar);
                            par2.Value = txtDESC.Text;
                            cmd.Parameters.Add(par2);

                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                t++;
                            }
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show("错误提示" + err.Message.ToString(), "提示");
                        }

                        conn.Close();
                    }
                });

                if (t > 0)
                {
                    try
                    {

                        if (dal.ExecuteSql(string.Format("update T_Refund set refundStatus={1},confirmAmount=isnull((select count(1) from T_RefundDetail where refundNo='{0}' and confirmReceipt={2}),0)+1 WHERE refundNo='{0}'",
                                                                Golbal_RefundNo, (int)RefundSatausEnum.refundException,
                                                                (int)ReceiptStatus.yes
                                                                )) > 0)
                        {
                            MessageBox.Show(string.Format("保存成功！成功上传{0}张图片", t), "提示");
                            UnpackingCheck rl = ((UnpackingCheck)FormInit.FindFather(this, typeof(UnpackingCheck)));
                            rl.dataBind(Golbal_RefundNo);
                            CloseDialog();

                        }
                        else
                        {
                            MessageBox.Show(string.Format("标记异常单失败，可手动将此订单标记异常！图片已成功上传{0}张图片", t), "提示");
                        }
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(string.Format("标记异常单失败，原因可能为{1}，可手动将此订单标记异常！图片已成功上传{0}张图片", t, err.Message.ToString()), "提示");
                    }
                }

            }

        }

        #endregion

        #region 绑定

        public void Bind()
        {
            List<UnknownlistModel> list = dal.GetList<UnknownlistModel>(new ExpressionModelField[] { new ExpressionModelField() { Name = "refundNo", Value = Golbal_RefundNo } });

            if (list.Count > 0)
            {

                txtDESC.Text = list[0].title;
                byte[] image_bytes = list[0].img;
                MemoryStream ms = new MemoryStream(image_bytes);
                System.Drawing.Bitmap bmap = new System.Drawing.Bitmap(ms);
                img1.Source = _9M.Work.Utility.ImageConvert.BitmapToBitmapSource(bmap);
            }
        }

        #endregion

        #region 删除图片
        private void btn_ClearImg(object sender, RoutedEventArgs e)
        {
            ImageSource iSource = img1.Source;
            if (iSource.GetType() == typeof(System.Windows.Interop.InteropBitmap))
            {
                try
                {
                    if (dal.ExecuteSql(string.Format("delete T_Unknownlist where id={0}", img1.Tag)) < 1)
                    {
                        MessageBox.Show("删除失败!", "提示");
                    }
                    else
                    {
                        img1.Source = new BitmapImage(new Uri("/Images/selected.png", UriKind.RelativeOrAbsolute));
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(string.Format("失败原因:{0}", err.Message.ToString()), "提示");
                }
            }
            else
            {
                if (!iSource.ToString().Contains("selected.png"))
                {
                    img1.Source = new BitmapImage(new Uri("/Images/selected.png", UriKind.RelativeOrAbsolute));
                }
            }
        }

        #endregion

    }
}
