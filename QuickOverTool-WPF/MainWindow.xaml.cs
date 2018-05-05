using OWorkbench.Logics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows;
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

        public MainWindow()
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
            
            InitializeComponent();
            Logging.GetInstance().IncrementDebug("MainWindow: OWorkbench is initializing.");
            Config.GetInstance().InitConfig();
            InitializeDataToolHandling();
            InitializeLogging();

            BNetDetector.CheckForBattleNet();
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
            Application.Current.Shutdown();
        }

        protected Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("Notifications.Wpf"))
            {
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFile(new Uri(Networking.GetGUIDependency()), ".\\Notifications.Wpf.dll");
                }
                UIString.GetInstance().SetNotif("OWorkbench is downloading a required assembly file. Please restart OWorkbench.");
            }
            return null;
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
