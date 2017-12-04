using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace QuickDataTool
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

        public static string MakeDownloadURL(string jobID, string filename)
        {
            return "https://ci.appveyor.com/api/buildjobs/" +
                    jobID +
                    "/artifacts/" +
                    filename;
        }
    }
}
