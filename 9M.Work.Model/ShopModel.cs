using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Model
{
    public class ShopModel
    {
        public ShopModel()
        {
            ActivityList = new List<ActivityModel>();
        }
        public int id { get; set; }
        public int shopId { get; set; }
        public string shopName { get; set; }
        public string appKey { get; set; }
        public string appSecret { get; set; }
        public string sessionKey { get; set; }
        public string invokeUrl { get; set; }
        public bool? isHaveApi { get; set; }

        public List<ActivityModel> ActivityList { get; set; }
    }
}
