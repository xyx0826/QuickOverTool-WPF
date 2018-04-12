﻿using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace QuickDataTool
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
            Logging.GetInstance().Increment("You are using QuickDataTool " + Assembly.GetExecutingAssembly().GetName().Version
                + "; latest version is " + result[0] + ".0");
            Logging.GetInstance().Increment("Download latest QuickDataTool here:");
            Logging.GetInstance().Increment(result[2] + "\n");
        }

        private void worker_DTCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string[] result = (string[])e.Result;
            Logging.GetInstance().Increment("You are using DataTool " + Validation.DataTool(".\\")[0]
                + "; latest version is " + result[0] + ".0");
            Logging.GetInstance().Increment("Download the latest DataTool in \"Tool Version\" tab.\n");
        }
    }
}
