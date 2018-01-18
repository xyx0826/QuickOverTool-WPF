using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickDataTool.Tabs
{
    class StatusBar : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void Rebind(string name)
        {
            OnPropertyChanged(name);
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
