﻿using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using static OWorkbench.Properties.Settings;

namespace OWorkbench
{
    class UIString : INotifyPropertyChanged
    {
        #region Singleton implementation
        private static UIString _uniqueInstance;
        private static readonly object _threadLock = new object();

        private UIString()
        {
            currentOWSize = "?";
        }

        public static UIString GetInstance() // Ensure singleton model
        {
            if (_uniqueInstance == null)
                lock (_threadLock)
                    _uniqueInstance = new UIString();
            return _uniqueInstance;
        }
        #endregion

        #region Binding implementation
        public event PropertyChangedEventHandler PropertyChanged;

        public void Rebind(string name)
        {
            OnPropertyChanged(name);
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
        #region Header strings
        public string BenchDir
        {
            get
            {
                return Path.GetDirectoryName(
                       Assembly.GetEntryAssembly().
                       CodeBase).Substring(6);
            }
        } // OWorkbench working directory
        public string BenchVersion
        {
            get
            {
                return "v. " +
                  Assembly.GetExecutingAssembly()
                  .GetName().Version.ToString();
            }
        } // OWorkbench version
        public string CurrentOWVersion
        {
            get
            {
                VersionManagement vm = new VersionManagement();
                string s = vm.GetOWVersion(Default.Path_CurrentOW);
                if (s != null) return s;
                else return "Unknown";
            }
        } // Current Overwatch version
        public string CurrentOWServer
        {
            get
            {
                VersionManagement vm = new VersionManagement();
                if (vm.IsOWPtr(Default.Path_CurrentOW))
                {
                    ToastManager.GetInstance().CreateToast("PTR Incompatibility",
                        "Overwatch PTR build detected. While you can still run DataTool with it, note that PTR builds are incompatible with DataTool.", 2);
                    return "PTR";
                }

                else return "Live";
            }
        } // Current Overwatch server
        public string CurrentOWPath
        {
            get { return Path.GetFullPath(Default.Path_CurrentOW); }
        } // Current Overwatch path

        private string currentOWSize;

        public string CurrentOWSize
        {
            get { return "(" + currentOWSize + " GB)"; }
            set
            {
                currentOWSize = value;
                OnPropertyChanged(null);
            }
        }
        public string DTVersion
        {
            get
            {
                VersionManagement vm = new VersionManagement();
                string s = vm.GetDTVersion(BenchDir);
                if (s != null) return s;
                else return "Unknown";
            }
        } // DataTool version
        public string DTIntegrity
        {
            get
            {
                VersionManagement vm = new VersionManagement();
                List<string> list = vm.CheckDTIntegerity(BenchDir);
                if (list.Count == 0) return "Complete";
                else
                {
                    // output missing files to log
                    ToastManager.GetInstance().CreateToast("DataTool File Incomplete",
                        "Some files required by DataTool could not be found. See the log for details. Try updating DataTool to resolve this issue.", 3);
                    Logging.GetInstance().Increment("At least " + list.Count + " DataTool dependency files are missing.");
                    foreach (string file in list)
                    {
                        Logging.GetInstance().Increment("    " + file);
                    }
                    return "Incomplete";
                }
            }
        } // Datatool file integrity
        #endregion
        #region Notif banner string and highlight
        // Notif banner itself
        private string notif;

        public string Notif
        {
            get { return notif; }
            set
            {
                notif = value;
                OnPropertyChanged(null);
            }
        }
        // Notif banner color
        private Brush notifBrush = Brushes.Black;

        public Brush NotifBrush
        {
            get { return notifBrush; }
            set
            {
                notifBrush = value;
                OnPropertyChanged(null);
            }
        }
        // Only set notif message, or also apply a highlight effect
        public void SetNotif(string s)
        {
            Notif = s;
        }

        public void SetNotif(Dispatcher dispatcher, string s)
        {
            Notif = s;
            BackgroundWorker colorWorker = new BackgroundWorker();
            colorWorker.DoWork += ColorWorker;
            colorWorker.RunWorkerAsync(dispatcher);
        }
        // Highlight the banner for 2 seconds in red
        public void ColorWorker(object sender, DoWorkEventArgs e)
        {
            ((Dispatcher)e.Argument).Invoke(new System.Action(() => { NotifBrush = Brushes.Red; }));
            System.Threading.Thread.Sleep(2000);
            ((Dispatcher)e.Argument).Invoke(new System.Action(() => { NotifBrush = Brushes.Black; }));
        }
        #endregion

        public int DownloadProgress { get; set; }

        public bool LoggingOnLaunch
        {
            get { return Default.TAB_QUICKSTART_LoggingOnLaunch; }
            set
            {
                Default.TAB_QUICKSTART_LoggingOnLaunch = value;
                Default.Save();
            }
        }

        public bool DebugMode
        {
            get { return Default.DebugMode; }
            set
            {
                Default.DebugMode = value;
                Default.Save();
            }
        }
    }
}
