using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;

namespace QuickDataTool
{
    public class Logging : INotifyPropertyChanged
    {
        public void ClearLogs(System.Windows.Controls.ListBox listBox)
        {
            listBox.ItemsSource = null;
            listBox.Items.Refresh();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void Refresh()
        {
            OnPropertyChanged(null);
        }

        private ObservableCollection<string> logCollection = new ObservableCollection<string>();

        public ObservableCollection<string> LogCollection
        {
            get { return logCollection; }
        }

        public delegate void IncrementDelegate(string log);

        public void Increment(string log)
        {
            try
            {
                logCollection.Add(log);
                Refresh();
            }
            catch { throw new System.Exception(); }
        }

        public void Increment(Dispatcher dispatcher, string log)
        {
            dispatcher.Invoke(new IncrementDelegate(Increment), log);
        }
    }
}
