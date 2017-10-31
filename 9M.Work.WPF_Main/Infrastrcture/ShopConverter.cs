using _9M.Work.DbObject;
using _9M.Work.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace _9M.Work.WPF_Main.Infrastrcture
{
    public class ShopConverter : IValueConverter
    {
        private static BaseDAL dal = new BaseDAL();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string res = string.Empty;
            if (value != null)
            {
                int shopId = System.Convert.ToInt32(value);
                List<ShopModel> list = dal.GetAll<ShopModel>();
                if (list != null && list.Count > 0)
                {
                    ShopModel model = list.SingleOrDefault(a => a.id.Equals(shopId));
                    if (model != null)
                    {
                        res = model.shopName;
                    }
                }
            }
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
