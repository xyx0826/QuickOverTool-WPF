using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuickDataTool.Tabs
{
    public partial class MainWindow : Window
    {
        public void StartDataTool(Process dataTool)
        {
            dataTool.Start();
            dataTool.BeginOutputReadLine();
            dataTool.BeginErrorReadLine();
            dataTool.OutputDataReceived += new DataReceivedEventHandler(DataTool_DataReceived);
            dataTool.ErrorDataReceived += new DataReceivedEventHandler(DataTool_DataReceived);
        }

        public void DataTool_DataReceived(object sender, DataReceivedEventArgs e)
        {
            Logging logger = new Logging();
            logger.Increment(Encoding.UTF8.GetString
                    (Encoding.Default.GetBytes(e.Data)));
        }
    }
}
