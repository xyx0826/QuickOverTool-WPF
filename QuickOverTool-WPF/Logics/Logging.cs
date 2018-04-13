﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using static QuickDataTool.Properties.Settings;

namespace QuickDataTool
{
    class Logging : INotifyPropertyChanged
    {
        private static Logging _uniqueInstance;
        private static readonly object _threadLock = new object();

        private Logging()
        {

        }

        public static Logging GetInstance() // Ensure singleton model
        {
            if (_uniqueInstance == null)
                lock (_threadLock)
                    _uniqueInstance = new Logging();
            return _uniqueInstance;
        }

        public void ClearLogs(System.Windows.Controls.ListBox listBox)
        {
            logCollection = new ObservableCollection<string>();
            Increment("Logs have been cleared.");
            Refresh();
        }

        public void SaveLogs()
        {
            DateTime timeNow = DateTime.Now;
            string logPath = Path.GetFullPath(Default.Path_Output)
                + "QDT-Log-" + timeNow.Hour + timeNow.Minute + timeNow.Second + ".log";
            using (StreamWriter logFile = new StreamWriter(@logPath))
            {
                foreach (String log in logCollection) logFile.WriteLine(log);
            }
            Logging.GetInstance().Increment("Log written to " + @logPath + ".");
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

        public delegate void IncrementDelegate(string log); // Delegated increment

        public void Increment(string log) // Standard increment
        {
            logCollection.Add(log);
            Refresh();
        }

        public void Increment(ListBox box, string log) // Dispatched increment
        {
            box.Dispatcher.Invoke(new IncrementDelegate(Increment), log);
        }
    }
}
