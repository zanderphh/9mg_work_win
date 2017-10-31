using _9M.Work.WPF_Common.ValueObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace _9M.Work.WPF_Main.Infrastrcture
{
    public class GridBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                 if (((int)value).Equals((int)FlagColorEnume.AColor))
                {
                    value =  new SolidColorBrush((Color)ColorConverter.ConvertFromString("#99ff00")); 
                }
                else if (((int)value).Equals((int)FlagColorEnume.BColor))
                {
                    value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff99cc")); 
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
