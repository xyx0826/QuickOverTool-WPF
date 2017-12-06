using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using static QuickDataTool.Properties.Settings;

namespace QuickDataTool
{
    public partial class MainWindow : Window
    {
        public string BenchDir
        {
            get
            {
                return Path.GetDirectoryName(
                       Assembly.GetEntryAssembly().
                       CodeBase).Substring(6);
            }
        } // OWorkbench working directory
        public string BenchVersion
        {
            get
            {
                return "v. " +
                  Assembly.GetExecutingAssembly()
                  .GetName().Version.ToString();
            }
        } // OWorkbench version
        public string CurrentOWVersion
        {
            get
            {
                VersionManagement vm = new VersionManagement();
                string s = vm.GetOWVersion(Default.Path_CurrentOW);
                if (s != null) return s;
                else return "Unknown";
            }
        } // Current Overwatch version
        public string CurrentOWServer
        {
            get
            {
                VersionManagement vm = new VersionManagement();
                if (vm.IsOWPtr(Default.Path_CurrentOW)) return "PTR";
                else return "Live";
            }
        } // Current Overwatch server
        public string CurrentOWPath
        {
            get { return Default.Path_CurrentOW; }
        } // Current Overwatch path
        public string DTVersion
        {
            get
            {
                VersionManagement vm = new VersionManagement();
                string s = vm.GetDTVersion(BenchDir);
                if (s != null) return s;
                else return "Unknown";
            }
        } // DataTool version
        public string DTIntegrity
        {
            get
            {
                VersionManagement vm = new VersionManagement();
                List<string> list = vm.CheckDTIntegerity(BenchDir);
                if (list == null) return "Complete";
                else return "Incomplete";
            }
        } // Datatool file integrity
    }
}
