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

            var userName = "";
            if (value is string str)
                userName = str;
            else if (value is UserListElement ule)
                userName = ule.UserName;

            string name = OwnName;

            if (name != null && userName == name)
                return "you";

            return userName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
