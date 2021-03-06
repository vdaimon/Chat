using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WPFClient
{
    public class INPC : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected void Set<T>(ref T property, T value, Action callback = null, [CallerMemberName] string propName = "")
        {
            if ((property == null && value != null) || !property.Equals(value))
            {
                property = value;

                if (callback != null)
                    callback();

                RaisePropertyChanged(propName);
            }
        }

        protected void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
