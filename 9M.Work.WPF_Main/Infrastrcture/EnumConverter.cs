using _9M.Work.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace _9M.Work.WPF_Main.Infrastrcture
{
    [ValueConversion(typeof(int), typeof(string))]
    public class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string statusName = string.Empty;
            Type type = parameter.GetType();
            if (type.IsEnum)
            {
                List<EnumEntity> list = EnumHelper.GetEnumList(type);
                foreach (var item in list)
                {
                    if (System.Convert.ToInt32(value) == item.Value)
                    {
                        statusName = item.Text;
                        break;
                    }
                }
            }
            return statusName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
