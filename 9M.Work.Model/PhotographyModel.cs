using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class PhotographyModel
    {
        /// <summary>
        /// 自增编号
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 单号
        /// </summary>
        public string photoid { get; set; }
        /// <summary>
        /// 平铺照&模特照
        /// </summary>
        public string tType { get; set; }
        /// <summary>
        /// 拍摄时间
        /// </summary>
        public DateTime? photoTime { get; set; }
        /// <summary>
        /// 拍摄人
        /// </summary>
        public string photographer { get; set; }
        /// <summary>
        /// 拍摄数量
        /// </summary>
        public int photoNumber { get; set; }
        /// <summary>
        /// 拍摄完成时间
        /// </summary>
        public DateTime? photoEndTime { get; set; }
        /// <summary>
        /// 切图员工
        /// </summary>
        public string catImage { get; set; }
        /// <summary>
        /// 切图时间
        /// </summary>
        public DateTime? catImageTime { get; set; }
        /// <summary>
        /// 切图数量
        /// </summary>
        public int catNum { get; set; }

        /// <summary>
        /// 修图员工
        /// </summary>
        public string ImageRepair { get; set; }
        /// <summary>
        /// 修图时间
        /// </summary>
        public DateTime? ImageRepairTime { get; set; }
        /// <summary>
        /// 修图数量
        /// </summary>
        public int ImageRepairNum { get; set; }
        /// <summary>
        /// 建单时间
        /// </summary>
        public DateTime? createTime { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string createEmp { get; set; }
        /// <summary>
        /// 总款数
        /// </summary>
        public int kuanNumber { get; set; }
        /// <summary>
        /// 品集集合名称
        /// </summary>
        public string brandColl { get; set; }
    }
}
