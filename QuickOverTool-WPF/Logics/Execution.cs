using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickDataTool.Logics
{
    class Execution
    {
        public Process PrepareDataTool(string cmdline)
        {
            using (Process dataTool = new Process())
            {
                dataTool.StartInfo.FileName = "DataTool.exe";
                dataTool.StartInfo.Arguments = cmdline;
                // dataTool.StartInfo.UseShellExecute = false;
                // dataTool.StartInfo.RedirectStandardOutput = true;
                dataTool.StartInfo.StandardOutputEncoding = Encoding.Default;
                // dataTool.StartInfo.RedirectStandardError = true;
                dataTool.StartInfo.StandardErrorEncoding = Encoding.Default;
                dataTool.StartInfo.CreateNoWindow = true;
                return dataTool;
            }
        }
    }
}
