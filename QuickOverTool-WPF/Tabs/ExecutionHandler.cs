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
            dataTool.StartInfo.StandardOutputEncoding = Encoding.Default;
            dataTool.StartInfo.RedirectStandardError = true;
            dataTool.StartInfo.StandardErrorEncoding = Encoding.Default;
            dataTool.StartInfo.CreateNoWindow = true;
            return dataTool;
        }

        public void StartDataTool(Process dataTool)
        {
            using (dataTool)
            {
                dataTool.Start();
                dataTool.BeginOutputReadLine();
                dataTool.BeginErrorReadLine();
                dataTool.OutputDataReceived += new DataReceivedEventHandler(DataTool_DataReceived);
                dataTool.ErrorDataReceived += new DataReceivedEventHandler(DataTool_DataReceived);
            }
        }

        public void DataTool_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null) Logging.GetInstance().Increment(logBox.Dispatcher, Encoding.UTF8.GetString
                (Encoding.Default.GetBytes(e.Data)));
        }
    }
}
