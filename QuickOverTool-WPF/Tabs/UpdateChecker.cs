using System;
using System.ComponentModel;
using System.Net;
using System.Reflection;
using System.Windows;

namespace OWorkbench
{
    public partial class MainWindow : Window
    {
        BackgroundWorker worker1 = new BackgroundWorker();
        BackgroundWorker worker2 = new BackgroundWorker();

        private void CheckGUIUpdate()
        {
            worker1.DoWork += worker_GUI;
            worker1.RunWorkerCompleted += worker_GUICompleted;
            worker1.WorkerReportsProgress = true;
            worker1.RunWorkerAsync();
        }

        private void CheckDTUpdate()
        {
            worker2.DoWork += worker_DT;
            worker2.RunWorkerCompleted += worker_DTCompleted;
            worker2.WorkerReportsProgress = true;
            worker2.RunWorkerAsync();
        }

        private void worker_GUI(object sender, DoWorkEventArgs e)
        {
            e.Result = Networking.GetGUIInfo();
        }

        private void worker_DT(object sender, DoWorkEventArgs e)
        {
            e.Result = Networking.GetDTInfo();
        }

        private void worker_GUICompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string[] result = (string[])e.Result;
            if (Assembly.GetExecutingAssembly().GetName().Version.CompareTo(new Version(result[0])) < 0)    // Remote has a new version
            {
                Logging.GetInstance().Increment("OWorkbench has an update! Latest version is " + result[0] + ".");
                Logging.GetInstance().Increment("Automatic downloading initiated.");
                Logging.GetInstance().IncrementDebug("UpdateChecker: found new OWorkbench version at " + result[2]);
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFileAsync(new Uri(result[2]), ".\\OWorkbench_" + result[0] + ".exe");
                }
                UIString.GetInstance().SetNotif("New OWorkbench version will be downloaded to .\\OWorkbench_" + result[0] + ".exe.");
            }
        }

        private void worker_DTCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string[] result = (string[])e.Result;
            if (Validation.DataTool(".\\") == null || 
                new Version(Validation.DataTool(".\\")).CompareTo(new Version(result[0])) < 0)    // Remote has a new version
            {
                Logging.GetInstance().Increment("DataTool has an update! Latest version is " + result[0] + ".");
                Logging.GetInstance().Increment("Download the latest DataTool in \"Tool Version\" tab.\n");
            }
        }
    }
}
