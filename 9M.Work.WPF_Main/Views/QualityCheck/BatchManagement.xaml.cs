using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.Utility;
using _9M.Work.WPF_Main.Infrastrcture;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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

namespace _9M.Work.WPF_Main.Views.QualityCheck
{
    /// <summary>
    /// BatchManagement.xaml 的交互逻辑
    /// </summary>
    public partial class BatchManagement : UserControl
    {
        BaseDAL dal = new BaseDAL();
        public BatchManagement()
        {
            InitializeComponent();
            BindGrid();
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            int Tag = Convert.ToInt32((sender as Button).Tag);
            QualityBatchModel model = (QualityBatchModel)BatchGrid.SelectedItem;

            switch (Tag)
            {
                case 0:  //新建批次
                    FormInit.OpenDialog(this, new BatchOperation(OperationStatus.ADD, model), false);
                    break;
                case 1: //修改批次
                    if (model == null)
                    {
                        CCMessageBox.Show("请选中要修改的行");
                        return;
                    }
                    FormInit.OpenDialog(this, new BatchOperation(OperationStatus.Edit, model), false);
                    break;
                case 2:
                    break;
                case 3:  //生成导入管家的EXCEL
                    if (model == null)
                    {
                        CCMessageBox.Show("请选择需要生成的批次");
                        return;
                    }
                    //款号与规格
                    List<WareModel> warelist = dal.QueryList<WareModel>(string.Format("select * from  dbo.T_WareList where Batchid = {0}", model.Id), new object[] { });
                    List<WareSpecModel> speclist = dal.QueryList<WareSpecModel>(string.Format("select * from dbo.T_WareSpecList where WareNo in (select WareNo from  dbo.T_WareList where Batchid = {0}) order by WareNo", model.Id), new object[] { });
                    List<CategoryModel> catelist = dal.GetAll<CategoryModel>();
                    DataTable waredt = new DataTable();
                    waredt.Columns.Add("编号", typeof(string));
                    waredt.Columns.Add("品名", typeof(string));
                    waredt.Columns.Add("类别", typeof(string));
                    waredt.Columns.Add("别名", typeof(string));
                    waredt.Columns.Add("规格", typeof(string));
                    waredt.Columns.Add("条码", typeof(string));
                    waredt.Columns.Add("固定成本价", typeof(decimal));
                    waredt.Columns.Add("多规格标记", typeof(int));
                    waredt.Columns.Add("单位", typeof(string));
                    waredt.Columns.Add("零售价", typeof(decimal));
                    waredt.Columns.Add("批发价", typeof(string));
                    waredt.Columns.Add("会员价", typeof(string));
                    waredt.Columns.Add("自定价1", typeof(string));
                    waredt.Columns.Add("自定价2", typeof(string));
                    waredt.Columns.Add("自定价3", typeof(string));
                    waredt.Columns.Add("品牌", typeof(string));
                    waredt.Columns.Add("产地", typeof(string));
                    waredt.Columns.Add("重量", typeof(string));
                    waredt.Columns.Add("积分", typeof(string));
                    waredt.Columns.Add("标记", typeof(string));
                    waredt.Columns.Add("长", typeof(string));
                    waredt.Columns.Add("宽", typeof(string));
                    waredt.Columns.Add("高", typeof(string));
                    waredt.Columns.Add("耗材消耗天", typeof(string));
                    waredt.Columns.Add("采购员", typeof(string));
                    waredt.Columns.Add("自定义1", typeof(string));
                    waredt.Columns.Add("自定义2", typeof(string));
                    waredt.Columns.Add("自定义3", typeof(string));
                    waredt.Columns.Add("自定义4", typeof(string));
                    waredt.Columns.Add("备注", typeof(string));
                    waredt.Columns.Add("上架日期", typeof(string));
                    waredt.Columns.Add("最低零售", typeof(decimal));
                    warelist.ForEach(x =>
                    {
                        DataRow dr = waredt.NewRow();
                        dr["编号"] = x.WareNo;
                        dr["品名"] = x.WareName;
                        dr["类别"] = catelist.Where(y => y.Id == x.CategoryId).FirstOrDefault().CategoryName;
                        dr["条码"] = x.WareNo;
                        dr["自定价1"] = x.Price;
                        dr["单位"] = "件";
                        dr["多规格标记"] = 1;
                        dr["零售价"] = 0;
                        dr["采购员"] = "质检部";
                        dr["最低零售"] = 0;
                        dr["备注"] = x.HouDu + " " + x.Remark;
                        dr["重量"] = x.Weight;
                        waredt.Rows.Add(dr);
                    });

                    DataTable specdt = new DataTable();
                    specdt.Columns.Add("编号", typeof(string));
                    specdt.Columns.Add("规格", typeof(string));
                    specdt.Columns.Add("附加码", typeof(string));
                    specdt.Columns.Add("颜色", typeof(string));
                    specdt.Columns.Add("尺码", typeof(string));
                    specdt.Columns.Add("过期日期", typeof(string));
                    //处理list
                    List<string> WareNoList = warelist.Select(x => x.WareNo).ToList();
                    WareNoList.ForEach(x =>
                    {
                        //得到有配饰的集合
                        List<WareSpecModel> list = speclist.Where(y => y.WareNo == x && y.Color.Contains("有")).ToList();

                        if (list.Count > 0)
                        {
                            //得到配饰的名字
                            string Color = list.FirstOrDefault().Color;
                            string PeiShi = Color.Substring(Color.IndexOf('有'), Color.Length - Color.IndexOf('有'));
                            List<WareSpecModel> nolist = speclist.Where(y => y.WareNo == x && !y.Color.Contains("有")).ToList();
                            //把没有配饰的加上无配饰
                            //string NoPeishi = PeiShi.Replace("有", "无");
                            //nolist.ForEach(zs =>
                            //{
                            //    zs.Color = zs.Color + NoPeishi;
                            //});
                        }
                    });
                    speclist.ForEach(x =>
                    {
                        DataRow dr = specdt.NewRow();
                        dr["编号"] = x.WareNo;
                        dr["规格"] = x.Color + x.Size;
                        dr["附加码"] = x.SpecCode;
                        dr["颜色"] = x.Color;
                        dr["尺码"] = x.Size;
                        specdt.Rows.Add(dr);
                    });
                    string Msg = "导入完成";
                    try
                    {
                        string Dir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\管家导入" + DateTime.Now.ToFileTime().ToString();
                        System.IO.Directory.CreateDirectory(Dir);
                        ExcelNpoi.TableToExcel(waredt, Dir + @"\" + "商品.xls");
                        ExcelNpoi.TableToExcel(specdt, Dir + @"\" + "规格.xls");
                    }
                    catch (Exception ex)
                    {
                        Msg = ex.Message;
                    }
                    CCMessageBox.Show(Msg);
                    break;
                case 4: //查询
                    string Text = tb_QueryText.Text;
                    if (string.IsNullOrEmpty(tb_QueryText.Text))
                    {
                        BindGrid();
                    }
                    else
                    {
                        string sql = string.Format(@"select * from T_QualityCheck_Batch
                        where BatchName like '%{0}%' or branden like '%{1}%' ", Text, Text);
                        List<QualityBatchModel> list = dal.QueryList<QualityBatchModel>(sql, new object[] { });
                        BatchGrid.ItemsSource = list;
                    }
                    break;
                case 5: //包装设置
                    FormInit.OpenDialog(this, new LockingSet(), false);
                    break;
                case 6: //批次锁定
                    if (model == null)
                    {
                        CCMessageBox.Show("请选择需要生成的批次");
                        return;
                    }
                    CCMessageBoxResult res = CCMessageBox.Show("锁定批次按【是】锁定,【否】取消锁定", "提示", CCMessageBoxButton.YesNoCancel);
                    if (res == CCMessageBoxResult.Yes) //锁定
                    {
                        model.IsLock = true;
                        dal.Update<QualityBatchModel>(model);
                    }
                    else if (res == CCMessageBoxResult.No)//取消锁定
                    {
                        model.IsLock = false;
                        dal.Update<QualityBatchModel>(model);
                    }
                    break;
                case 7:
                    if (model == null)
                    {
                        CCMessageBox.Show("请选择需要生成的批次");
                        return;
                    }
                    FormInit.OpenDialog(this, new BunchWare(model), false);
                    break;
                case 8:

                    List<int> idCollection = new List<int>();
                    IEnumerator ie = BatchGrid.SelectedItems.GetEnumerator();
                    while (ie.MoveNext())
                    {
                        QualityBatchModel m = ie.Current as QualityBatchModel;
                        idCollection.Add(m.Id);
                    }
                    if (idCollection.Count.Equals(0))
                    {
                        CCMessageBox.Show("请在列表中选择要操作的行");
                        return;
                    }
                    CCMessageBoxResult cbr = CCMessageBox.Show("报表显示请按【是】,取消报表显示请按【否】", "提示", CCMessageBoxButton.YesNoCancel);

                    bool isDisplay = (cbr == CCMessageBoxResult.No) ? false : true;

                    string s = string.Format("update InSideWorkServer.dbo.T_QualityCheck_Batch set IsDisplay={0} where id in({1})", Convert.ToInt32(isDisplay), string.Join(",", idCollection.ToArray()));

                    if (dal.ExecuteSql(s) > 0)
                    {
                        CCMessageBox.Show("设置成功！");
                        BindGrid();
                    }

                    break;
            }
        }

        public void BindGrid()
        {
            List<OrderModelField> orderField = new List<OrderModelField>();
            orderField.Add(new OrderModelField() { PropertyName = "Id", IsDesc = true });
            List<QualityBatchModel> list = dal.GetAll<QualityBatchModel>(orderField.ToArray());
            BatchGrid.ItemsSource = list;
        }

        private void dataPager_PageChanged(object sender, WPF_Common.Controls.Pager.PageChangedEventArgs args)
        {
            int k = 1;
        }
    }
}
