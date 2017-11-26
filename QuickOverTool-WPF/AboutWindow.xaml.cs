using System.Windows;
using System.Windows.Data;
using System.Diagnostics;
using System.Windows.Navigation;
using System.Net;
using System.Xml;
using System.IO.Compression;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace QuickOverTool_WPF
{
    /// <summary>
    /// AboutWindow.xaml logics
    /// </summary>
    public partial class AboutWindow : Window
    {
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            windowAbout.Hide();
        }

        public string Text
        {
            get
            {
                return "QuickDataTool is made by xyx0826.\n" +
                    "This project can be found at:\n" +
                    "https://github.com/xyx0826/QuickOverTool-WPF/tree/datatool. \n\n" +
                    "Thanks dynaomi, zingballyhoo, SombraOW and Js41637 for making OverTool toolchain, the ultimate toolset for extracting Overwatch game assets.\n\n" +
                    "If you need a GUI for OverTool-ing pre-1.14 game, please consider checking out Yernemm's OverTool GUI.\n" +
                    "You can find Yernemm's GUI at:\n" +
                    "https://yernemm.xyz/projects/OverToolGUI.";

            }
            set { }
        }

        public string Warning
        {
            get
            {
                return "Warning:\n" +
                    "New builds may be UNSTABLE.\n" +
                    "They may contain bugs, or be totally broken.\n" +
                    "Please only update when you know what will happen.\n\n";
            }
            set { }
        }

        public string[] DTInfo
        {
            get
            {
                return GetDTInfo();
            }
            set { }
        }

        public AboutWindow()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private string[] GetDTInfo()
        {
            // Request XML data from Appveyor API
            HttpWebRequest req = WebRequest.Create("https://ci.appveyor.com/api/projects/yukimono/owlib/branch/overwatch/1.14") as HttpWebRequest;
            req.Accept = "application/xml";
            HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
            // Parse XML response
            XmlDocument appveyor = new XmlDocument();
            appveyor.Load(resp.GetResponseStream());
            string dtVersion = appveyor.GetElementsByTagName("Version")[0].InnerText;
            string dtMessage = appveyor.GetElementsByTagName("Message")[0].InnerText;
            string dtDownload = MakeDownloadURL(appveyor.GetElementsByTagName("JobId")[0].InnerText,
                                              "dist%2Ftoolchain-release.zip");

            return new string[] { dtVersion, dtMessage, dtDownload};
        }

        private string MakeDownloadURL(string jobID, string filename)
        {
            return "https://ci.appveyor.com/api/buildjobs/" +
                    jobID +
                    "/artifacts/" +
                    filename;
        }

        private void DownloadNewDataTool(object sender, RequestNavigateEventArgs e)
        {
            string zipPath = ".\\datatool_" + GetDTInfo()[0] + ".zip";
            // Download new build from appveyor
            textBlockDownloader.Text = "Downloading...";
            using (WebClient wc = new WebClient())
            {
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileAsync(new System.Uri(e.Uri.AbsoluteUri), zipPath);
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(Unzip);
            }
        }

        void Unzip(object sender, AsyncCompletedEventArgs e)
        {
            string zipPath = ".\\datatool_" + GetDTInfo()[0] + ".zip";
            // Read zip content and remove old build
            List<string> files = new List<string>();
            ZipArchive zip = ZipFile.OpenRead(zipPath);
            foreach (ZipArchiveEntry file in zip.Entries)
            {
                File.Delete(".\\" + file.Name);
            }
            try
            {
                ZipFile.ExtractToDirectory(zipPath, ".\\");
            }
            catch { }
            textBlockDownloader.Text = "Update successful.";
        }

        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            downloadBar.Value = e.ProgressPercentage;
        }

        /*
        private void Unzip(string zipFile)
        {
            
        }
        */
    }
}
