using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;

namespace QuickOverTool_WPF
{
    public partial class MainWindow : Window
    {
        BackgroundWorker worker = new BackgroundWorker();
        private void CheckGUIUpdate()
        {
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.WorkerReportsProgress = true;
            worker.RunWorkerAsync();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = Networking.GetGUIInfo();
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string[] result = (string[])e.Result;
            AddLog("You are using QuickDataTool " + Assembly.GetExecutingAssembly().GetName().Version
                + "; latest version is " + result[0] + ".0");
            AddLog("Download latest QuickDataTool here: " + result[1]);
        }
    }
}
