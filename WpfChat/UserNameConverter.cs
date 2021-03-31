using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WPFClient
{
    public class UserNameConverter : IValueConverter
    {
        public string OwnName { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "all";

            string name = OwnName;

            if (name != null && value.ToString() == name)
                return "you";

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString() == "all")
                return null;
            if (value.ToString() == "you")
                return parameter;
            return DependencyProperty.UnsetValue;
        }
    }
}
