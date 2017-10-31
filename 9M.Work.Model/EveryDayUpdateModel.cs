using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class CustomItem
    {
        //标题
        public string Title { get; set; }


        //折扣价
        public string NewPrice { get; set; }


        //原价
        public string OldPrice { get; set; }


        //唯一标识
        public long NumIid { get; set; }


        //图片URL地址
        public string PicUrl { get; set; }


        //款号
        public string OuterId { get; set; }


        //详情地址
        public string DetailUrl { get; set; }

        //详情页第五张图片
        public string FivePicUrl { get; set; }

        //索引序号
        public string SerialNo { get; set; }

        //差价
        public string DiffPrice { get; set; } 
    }

    public class UploadExcelModel
    {

        /// <summary>
        /// 款号
        /// </summary>
        public string Goodsno { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public string Price { get; set; }

    }

    public class EverydayUpdateModel
    {

        public string DateString { get; set; }

        public List<CustomItem> ItemList { get; set; }
    }


    public class EverydayUpdateModelByFXtable
    {
        public string DateString { get; set; }

        public List<List<CustomItem>> ItemList { get; set; }
    }
}
