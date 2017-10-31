using _9M.Work.DbObject;
using _9M.Work.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.WPF_Main.FrameWork
{
    public class SqlDalHelp
    {
        public static List<PrintLabeModel> getWareFlagCode(string UserAlias, string goodsno, string color, string size)
        {
            BaseDAL dal = new BaseDAL();

            string printsql = string.Format(@"select   a.WareNo,a.WareNo+CONVERT(varchar, b.SpecCode) as SpecCode ,(b.Color+b.Size) as specname 
                        ,('{0}'+ CONVERT(varchar, a.InSideGroupId)) as FlagCode,b.Color,b.Size  from T_WareList a
                        left join T_WareSpecList b on a.WareNo = b.WareNo
                        where a.WareNo = '{1}' and b.Color = '{2}' and b.Size = '{3}'", UserAlias, goodsno, color, size);


            return dal.QueryList<PrintLabeModel>(printsql, new object[] { });
        }


        public static DataTable getWaterTagInfo(List<string> list)
        {
            BaseDAL dal = new BaseDAL();
            string sql = String.Format("select GoodsNo,Material from WorkPlatform.dbo.tGoodsBaseInfo where goodsno in('{0}')", String.Join("','", list.ToArray()));
            DataTable dt = dal.QueryDataTable(sql);

            DataTable sourcedt = new DataTable();
            sourcedt.Columns.Add("款号");
            sourcedt.Columns.Add("面料");
            sourcedt.Columns.Add("图1");
            sourcedt.Columns.Add("图2");
            sourcedt.Columns.Add("图3");
            sourcedt.Columns.Add("图4");

            foreach (DataRow dr in dt.Rows)
            {
                DataRow d = sourcedt.NewRow();
                d["款号"] = dr["GoodsNo"].ToString();
                d["面料"] = dr["Material"].ToString().Replace(" ", Environment.NewLine).Replace("面料：", "").Replace("面料:", "");
                d["图1"] = "http://work.9mg.cn/d/wt/w_02.png";
                d["图2"] = "http://work.9mg.cn/d/wt/w_03.png";
                d["图3"] = "http://work.9mg.cn/d/wt/w_04.png";
                d["图4"] = "http://work.9mg.cn/d/wt/w_06.png";
                sourcedt.Rows.Add(d);
            }
            return sourcedt;
        }
    }
}
