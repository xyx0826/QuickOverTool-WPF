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
        public static bool IsNewBuildAvail()
        {
            // AppVeyor gives a json response
            XmlDocument appveyor = new XmlDocument();
            //appveyor.Load("https://gist.githubusercontent.com/xyx0826/dd6945a1d288a9b5a2427224814d87ff/raw/7c22afeafd90b2c08f1d5df953cb1f1552692918/test.xml");
            appveyor.Load("https://gist.githubusercontent.com/xyx0826/dd6945a1d288a9b5a2427224814d87ff/raw/26535820c0c0ea0ae26054f6114cc7c23db89094/test.xml");
            XmlElement root = appveyor.DocumentElement;
            string remoteVer = root.SelectSingleNode("/ProjectBuildResults/Build/BuildNumber").InnerText;
            return true;
        }
    }
}
