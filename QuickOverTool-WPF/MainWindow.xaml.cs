using OWorkbench.Logics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using static OWorkbench.Properties.Settings;

namespace OWorkbench
{
    public partial class MainWindow : Window
    {
        // DataTool path and PID
        int dataToolPID = -1;
        string sharedPath = Path.GetDirectoryName
            (Assembly.GetEntryAssembly().CodeBase).Substring(6);
        // Mode parameters dictionary
        Dictionary<string, string> modes = new Dictionary<string, string>();
        // Populate mode dictionary and load config upon application launch
        public MainWindow()
        {
            Logging.GetInstance().IncrementDebug("MainWindow: OWorkbench is initializing.");
            InitializeComponent();

            Config.GetInstance().InitConfig();
            InitializeDataToolHandling();
            InitializeLogging();
            FlushInst();

            DataContext = UIString.GetInstance();
            tabStart.DataContext = UIString.GetInstance();
            tabConfig.DataContext = DataToolConfig.GetInstance();
            
            CheckGUIUpdate();
            CheckDTUpdate();

            WindowMain.Title += " | " + UIString.GetInstance().BenchVersion;
        }
        // Save config and close application upon MainWindow closure
        protected override void OnClosed(EventArgs e)
        {
            Default.Upgrade();
            Default.Save();
            base.OnClosed(e);
            System.Windows.Application.Current.Shutdown();
        }

        /* TBD in OWB
        private void buttonSaveCmdline_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] batFile = {"echo off\n",
                ".\\DataTool.exe" + FabricateCmdline(),
                "\npause\n"};
                File.WriteAllLines(".\\_" + GetMode() + ".bat", batFile);
                Logging.GetInstance().Increment("Written batch file for mode " + GetMode() +
                    " to _" + GetMode() + ".bat.");
            }
            catch
            {
                Logging.GetInstance().Increment("Settings invalid. Please check your mode settings.");
            }
        }
        */
    }
}
