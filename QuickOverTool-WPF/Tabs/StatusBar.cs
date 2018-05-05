using System.ComponentModel;

namespace OWorkbench
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
