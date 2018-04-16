using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OWorkbench
{
    class Networking
    {
        /// <summary>
        /// Polls AppVeyor API for information regarding a project.
        /// </summary>
        /// <param name="project">Project name, in the format of "[authorname]/[repoName]"</param>
        /// <param name="branch">Branch name</param>
        /// <param name="innerText">The artifact file name to search for</param>
        /// <returns>An array of latest build version, message and artifact link.</returns>
        public static string[] PollAppveyorInfo(string project, string branch, string innerText)
        {
            // Request XML data from Appveyor API
            HttpWebRequest req = WebRequest.Create("https://ci.appveyor.com/api/projects/" + project + "/branch/" + branch) as HttpWebRequest;
            req.Accept = "application/xml";
            HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
            // Parse XML response
            XmlDocument appveyor = new XmlDocument();
            appveyor.Load(resp.GetResponseStream());
            string version = appveyor.GetElementsByTagName("Version")[0].InnerText;
            string message = appveyor.GetElementsByTagName("Message")[0].InnerText;
            string download = MakeDownloadURL(appveyor.GetElementsByTagName("JobId")[0].InnerText,
                                              innerText);
            return new string[] { version, message, download };
        }

        public static string[] GetDTInfo()
        {
            return PollAppveyorInfo("yukimono/owlib", "overwatch/1.14", "dist%2Ftoolchain-release.zip");
        }

        public static string[] GetGUIInfo()
        {
            return PollAppveyorInfo("xyx0826/quickovertool-wpf-5nemq", "experimental", "output%2FOWorkbench.exe");
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
