using System.ComponentModel;

namespace QuickDataTool
{
    public class Logging : INotifyPropertyChanged
    {
        private string log;
        public string Log
        {
            get { return log; }
            set { log += "\n" + value; OnPropertyChanged("Text"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void Increment(string s)
        {
            log += "\n" + s;
            OnPropertyChanged("Text");
        }
    }
}
