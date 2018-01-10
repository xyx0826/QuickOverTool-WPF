using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace QuickOverTool_WPF
{
    class Networking
    {
        public static string[] GetDTInfo()
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
            return new string[] { dtVersion, dtMessage, dtDownload };
        }

        public static string[] GetGUIInfo()
        {
            // Request XML data from Appveyor API
            HttpWebRequest req = WebRequest.Create("https://ci.appveyor.com/api/projects/xyx0826/quickovertool-wpf/branch/datatool") as HttpWebRequest;
            req.Accept = "application/xml";
            HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
            // Parse XML response
            XmlDocument appveyor = new XmlDocument();
            appveyor.Load(resp.GetResponseStream());
            string dtVersion = appveyor.GetElementsByTagName("Version")[0].InnerText;
            string dtDownload = MakeDownloadURL(appveyor.GetElementsByTagName("JobId")[0].InnerText,
                                              "output%2FQuickDataTool.exe");
            return new string[] { dtVersion, dtDownload };
        }

        public static string MakeDownloadURL(string jobID, string filename)
        {
            return "https://ci.appveyor.com/api/buildjobs/" +
                    jobID +
                    "/artifacts/" +
                    filename;
        }
    }
}
