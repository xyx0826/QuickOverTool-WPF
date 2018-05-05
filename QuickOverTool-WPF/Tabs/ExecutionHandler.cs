using OWorkbench.Logics;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;

namespace OWorkbench
{
    public partial class MainWindow : Window
    {
        public Process PrepareDataTool(string cmdline)
        {
            Process dataTool = new Process();
            dataTool.StartInfo.FileName = "DataTool.exe";
            dataTool.StartInfo.Arguments = cmdline;
            dataTool.StartInfo.UseShellExecute = false;
            dataTool.StartInfo.RedirectStandardOutput = true;
            dataTool.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            dataTool.StartInfo.RedirectStandardError = true;
            dataTool.StartInfo.StandardErrorEncoding = Encoding.UTF8;
            dataTool.StartInfo.CreateNoWindow = true;
            return dataTool;
        }

        public void StartDataTool(Process dataTool)
        {
            using (dataTool)
            {
                BackgroundWorker aliveChecker = new BackgroundWorker();
                aliveChecker.DoWork += CheckAlive;
                aliveChecker.RunWorkerCompleted += OnProcessDead;
                
                Logging.GetInstance().Increment("List and extract tabs are now temporarily disabled until DataTool terminates or exits.");
                
                try
                {
                    BNetDetector.CheckForBattleNet();   // Do a final check on Battle.net instances
                    dataTool.Start();
                    dataToolPID = dataTool.Id;
                }
                catch (Exception e)
                {
                    Logging.GetInstance().IncrementDebug("ExecutionHandler: DataTool launch exception /// " + e.Message);
                    Logging.GetInstance().Increment("DataTool launch failure. Update your DataTool from \"Tool Version\" tab.");
                    ToggleRunningState(false);
                    return;
                }
                Logging.GetInstance().Increment("Starting DataTool now. PID: " + dataTool.Id);
                ToggleRunningState(true);
                aliveChecker.RunWorkerAsync(dataTool);

                dataTool.BeginOutputReadLine();
                dataTool.BeginErrorReadLine();
                dataTool.OutputDataReceived += new DataReceivedEventHandler(DataTool_DataReceived);
                dataTool.ErrorDataReceived += new DataReceivedEventHandler(DataTool_DataReceived);
            }
        }

        /// <summary>
        /// Toggle the display of lower right corner icons.
        /// </summary>
        /// <param name="state">Whether DataTool is running or not.</param>
        public void ToggleRunningState(bool state)
        {
            if (state)
            {
                // Certain tabs have to be disabled to prevent multi-launch
                tabListAssets.IsEnabled = false;
                tabExtrAssets.IsEnabled = false;
                lblInactive.Visibility = Visibility.Collapsed;
                lblRunning.Visibility = Visibility.Visible;
            }
            else
            {
                tabListAssets.IsEnabled = true;
                tabExtrAssets.IsEnabled = true;
                lblInactive.Visibility = Visibility.Visible;
                lblRunning.Visibility = Visibility.Collapsed;
            }
        }

        public void DataTool_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null) Logging.GetInstance().Increment(logBox, e.Data);
        }

        private void CheckAlive(object sender, DoWorkEventArgs e)
        {
            ((Process)e.Argument).WaitForExit();
        }

        private void OnProcessDead(object sender, RunWorkerCompletedEventArgs e)
        {
            ToastManager.GetInstance().CreateToast("DataTool Termination", "DataTool has finished working.", 1);
            Logging.GetInstance().Increment("DataTool has exited.");
            ToggleRunningState(false);
        }
    }
}
