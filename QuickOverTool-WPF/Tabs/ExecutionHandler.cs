using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;

namespace QuickDataTool
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
                
                // Certain tabs have to be disabled to prevent multi-launch
                tabListAssets.IsEnabled = false;
                tabExtrAssets.IsEnabled = false;
                
                dataTool.Start();
                aliveChecker.RunWorkerAsync(dataTool);

                dataTool.BeginOutputReadLine();
                dataTool.BeginErrorReadLine();
                dataTool.OutputDataReceived += new DataReceivedEventHandler(DataTool_DataReceived);
                dataTool.ErrorDataReceived += new DataReceivedEventHandler(DataTool_DataReceived);
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
            tabListAssets.IsEnabled = true;
            tabExtrAssets.IsEnabled = true;
        }
    }
}
