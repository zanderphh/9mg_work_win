using _9M.Work.JosApi;
using _9M.Work.TopApi;
using _9M.Work.Utility;
using _9Mg.Work.TopApi;
using JdSdk.Domain.Ware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Top.Api.Domain;

namespace _9M.Work.WPF_Common
{
    public class TemplateUtil
    {

        //数据AdContent作为出售链接 ,JDpirce作为价格
        public string NewTemplate(string FileName, List<Ware> itemlist, int RowCount, int width, int twidth)
        {
            StringBuilder sb = new StringBuilder();
            string s = FileHelper.ReadFile(FileName);
            int b = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(Convert.ToDouble(Convert.ToDouble(itemlist.Count) / RowCount))));
            string newHTML = string.Empty;
            string newHTMLForPrice = string.Empty;
            int index = 0; //第一个TR的索引
            int index_t = 0; //第二个TR的索引
            for (int i = 0; i < b; i++)
            {
                string sflag = s;
                Regex regTD = new Regex(@"<td.+?>[\s\S]*?</td>");
                // Regex regTD = new Regex(@"<td[^>]*?>(.*?)</td>");
                Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);
                Regex rSrc = new Regex(@"([a-zA-Z0-9_]+)(.gif|.jpg|.jpeg|.GIF|.JPG|.JPEG|.png)");

                Regex regA = new Regex(@"<\s*a\shref=*[^>]*>([^<]|<(?!/a))*<\s*/a\s*>");
                Regex regSpan = new Regex("<span.*>(.*)</span>");
                foreach (Match m in regTD.Matches(sflag))
                {
                    MatchCollection matches = regImg.Matches(m.Groups.SyncRoot.ToString());
                    MatchCollection matchespan = regSpan.Matches(m.Groups.SyncRoot.ToString());
                    //生成第一个TR
                    for (int j = 0; j < matches.Count; j++)
                    {
                        if ((i * RowCount + index) >= itemlist.Count)
                        {
                            newHTML += string.Format("<td width=\"{0}\" height=\"{1}\"></td>", twidth, twidth);
                            continue;
                        }
                        string newImgHTML = rSrc.Replace(matches[j].Groups.SyncRoot.ToString(), itemlist[i * RowCount + index].Logo);

                        newHTML += regImg.Replace(m.Groups.SyncRoot.ToString(), newImgHTML);
                        newHTML = newHTML.Replace("#Link", itemlist[i * RowCount + index].AdContent);
                        if (index <= RowCount - 2)
                        {
                            newHTML += string.Format("<td width=\"{0}\">&nbsp;</td>", width);
                        }
                        index++;
                    }

                    //生成第二个TR
                    for (int k = 0; k < matchespan.Count; k++)
                    {
                        if ((i * RowCount + index_t) >= itemlist.Count)
                        {
                            newHTMLForPrice += string.Format("<td width=\"240\" height=\"36\"></td>");
                            continue;
                        }
                        newHTMLForPrice += m.Value.Replace("#Link", itemlist[i * RowCount + index_t].AdContent).Replace("88.00", itemlist[i * RowCount + index_t].JdPrice);
                        if (index_t <= RowCount - 2)
                        {
                            newHTMLForPrice += string.Format("<td width=\"{0}\">&nbsp;</td>", width);
                        }
                        index_t++;
                    }
                }
                newHTML = "<tr>" + newHTML + "</tr>";
                newHTMLForPrice = "<tr>" + newHTMLForPrice + "</tr>";

                //替换
                Regex regTR = new Regex(@"<tr>[\s\S]*?</tr>");
                MatchCollection ms = regTR.Matches(sflag);
                for (int j = 0; j < ms.Count; j++)
                {
                    if (j == 0)
                    {
                        sflag = sflag.Replace(ms[0].Value, newHTML);
                    }
                    else if (j == 1)
                    {
                        sflag = sflag.Replace(ms[1].Value, newHTMLForPrice);
                    }
                }
                sb.Append(sflag);
                index = 0;
                index_t = 0;
                newHTML = string.Empty;
                newHTMLForPrice = string.Empty;
            }
            //上新日期与头部
            string DateString = DateTime.Now.ToString("MM/dd");
            string Head = string.Format(@"<div style='
    background: url(http://img10.360buyimg.com/imgzone/jfs/t2128/92/1480936516/9882/c45fd73e/566127cfN513ad293.jpg);
    height: 134px;
    width: 990px;
    overflow: hidden;'>
    <h1 style='margin: 72px 0 0 419px;'>{0}</h1>
    </div><br/>", DateString);
            return Head + sb.ToString();
        }

        //DETAIL为出售链接,PostFee为价格,PicUrl为图片
        public string NewTemplate(string FileName, List<Item> itemlist, int RowCount, int width, int twidth)
        {
            StringBuilder sb = new StringBuilder();
            string s = FileHelper.ReadFile(FileName);
            int b = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(Convert.ToDouble(Convert.ToDouble(itemlist.Count) / RowCount))));
            string newHTML = string.Empty;
            string newHTMLForPrice = string.Empty;
            int index = 0; //第一个TR的索引
            int index_t = 0; //第二个TR的索引
            for (int i = 0; i < b; i++)
            {
                string sflag = s;
                Regex regTD = new Regex(@"<td.+?>[\s\S]*?</td>");
                // Regex regTD = new Regex(@"<td[^>]*?>(.*?)</td>");
                Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);
                Regex rSrc = new Regex(@"([a-zA-Z0-9_]+)(.gif|.jpg|.jpeg|.GIF|.JPG|.JPEG|.png)");

                Regex regA = new Regex(@"<\s*a\shref=*[^>]*>([^<]|<(?!/a))*<\s*/a\s*>");
                Regex regSpan = new Regex("<span.*>(.*)</span>");
                foreach (Match m in regTD.Matches(sflag))
                {
                    MatchCollection matches = regImg.Matches(m.Groups.SyncRoot.ToString());
                    MatchCollection matchespan = regSpan.Matches(m.Groups.SyncRoot.ToString());
                    //生成第一个TR
                    for (int j = 0; j < matches.Count; j++)
                    {
                        if ((i * RowCount + index) >= itemlist.Count)
                        {
                            newHTML += string.Format("<td width=\"{0}\" height=\"{1}\"></td>", twidth, twidth);
                            continue;
                        }
                        string newImgHTML = rSrc.Replace(matches[j].Groups.SyncRoot.ToString(), itemlist[i * RowCount + index].PicUrl);

                        newHTML += regImg.Replace(m.Groups.SyncRoot.ToString(), newImgHTML);
                        newHTML = newHTML.Replace("#Link", itemlist[i * RowCount + index].DetailUrl);
                        if (index <= RowCount - 2)
                        {
                            newHTML += string.Format("<td width=\"{0}\">&nbsp;</td>", width);
                        }
                        index++;
                    }

                    //生成第二个TR
                    for (int k = 0; k < matchespan.Count; k++)
                    {
                        if ((i * RowCount + index_t) >= itemlist.Count)
                        {
                            newHTMLForPrice += string.Format("<td width=\"240\" height=\"36\"></td>");
                            continue;
                        }
                        newHTMLForPrice += m.Value.Replace("#Link", itemlist[i * RowCount + index_t].DetailUrl).Replace("88.00", itemlist[i * RowCount + index_t].PostFee);
                        if (index_t <= RowCount - 2)
                        {
                            newHTMLForPrice += string.Format("<td width=\"{0}\">&nbsp;</td>", width);
                        }
                        index_t++;
                    }
                }
                newHTML = "<tr>" + newHTML + "</tr>";
                newHTMLForPrice = "<tr>" + newHTMLForPrice + "</tr>";

                //替换
                Regex regTR = new Regex(@"<tr>[\s\S]*?</tr>");
                MatchCollection ms = regTR.Matches(sflag);
                for (int j = 0; j < ms.Count; j++)
                {
                    if (j == 0)
                    {
                        sflag = sflag.Replace(ms[0].Value, newHTML);
                    }
                    else if (j == 1)
                    {
                        sflag = sflag.Replace(ms[1].Value, newHTMLForPrice);
                    }
                }
                sb.Append(sflag);
                index = 0;
                index_t = 0;
                newHTML = string.Empty;
                newHTMLForPrice = string.Empty;
            }
            //上新日期与头部
            string DateString = DateTime.Now.ToString("MM/dd");
            string Head = string.Format(@"<div style='
    background: url(https://img.alicdn.com/imgextra/i3/25412303/TB2gv2ilFXXXXXwXpXXXXXXXXXX_!!25412303.jpg);
    height: 134px;
    width: 950px;
    overflow: hidden;'>
    <h1 style='margin: 72px 0 0 419px;'>{0}</h1>
    </div><br/>", DateString);
            return Head + sb.ToString();
        }
        /// <summary>
        /// 组装对象
        /// </summary>
        /// <param name="wareutil"></param>
        /// <param name="dic"></param>
        /// <param name="CustomPrice">是否有自定价</param>
        /// <returns></returns>
        public List<Ware> GetList(WareManager wareutil, Dictionary<string, string> dic, bool CustomPrice)
        {
            List<Ware> warelist = new List<Ware>();
            //查询所有商品
           // List<Ware> idlist = wareutil.SearchWares(null, null, null, null, null, null, null);
            List<Ware> idlist = wareutil.GetWares(Model.GoodsStatus.All,null,null,null);
            //得到所有款号查询SKU
            List<string> itemlist = dic.Select(x => x.Key).ToList();
            List<string> wareidlist = idlist.Where(x => itemlist.Contains(x.ItemNum)).Select(y => y.WareId.ToString()).ToList();
            List<JdSdk.Domain.Ware.Sku> skulist = wareutil.GetSkuListByID(wareidlist, null, false);
            //组装对象
            foreach (var item in dic)
            {
                //寻找款
                Ware ware = idlist.Find(x =>
                {
                    return x.ItemNum.Equals(item.Key, StringComparison.CurrentCultureIgnoreCase);
                });
                if (ware != null)
                {
                    //寻找SKU组成出售链接
                    JdSdk.Domain.Ware.Sku sk = skulist.Find(y =>
                    {
                        return y.WareId == ware.WareId;
                    });
                    if (sk != null)
                    {
                        Ware t = new Ware();
                        t.AdContent = string.Format("http://item.jd.com/{0}.html", sk.SkuId); //出售链接
                        t.JdPrice = CustomPrice ? item.Value : ware.JdPrice; //加入价格
                        t.Logo = ware.Logo;
                        warelist.Add(t);
                    }
                }
            }
            return warelist;
        }

        /// <summary>
        /// 组装对象
        /// </summary>
        /// <param name="wareutil"></param>
        /// <param name="dic"></param>
        /// <param name="CustomPrice">是否有自定价</param>
        /// <returns></returns>
        public List<Item> GetList(TopSource wareutil, Dictionary<string, string> dic, bool CustomPrice)
        {
            List<Item> warelist = new List<Item>();

            //得到所有款号查询SKU
            List<string> itemlist = dic.Select(x => x.Key).ToList();
            List<Item> list = wareutil.GetItemList(itemlist, "detail_url,pic_url");
            //组装对象
            foreach (var item in dic)
            {
                //寻找款
                Item ware = list.Find(x =>
                {
                    return x.OuterId.Equals(item.Key, StringComparison.CurrentCultureIgnoreCase);
                });
                if (ware != null)
                {
                    //寻找SKU组成出售链接

                    //Item t = new Item();
                    //  t.AdContent = string.Format("http://item.jd.com/{0}.html", sk.SkuId); //出售链接
                    ware.PostFee = CustomPrice ? item.Value : ware.Price; //加入价格
                    warelist.Add(ware);

                }
            }
            return warelist;
        }


        public string NewCode(string FileName, WareManager wareutil, Dictionary<string, string> dic, bool CustomPrice)
        {
            List<Ware> list = GetList(wareutil, dic, CustomPrice);
            return NewTemplate(FileName, list, 4, 10, 238);
        }

        public string NewCode(string FileName, TopSource wareutil, Dictionary<string, string> dic, bool CustomPrice)
        {
            List<Item> list = GetList(wareutil, dic, CustomPrice);
            return NewTemplate(FileName, list, 4, 10, 238);
        }
    }
}
