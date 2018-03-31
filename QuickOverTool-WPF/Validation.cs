using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace QuickOverTool_WPF
{
    /// <summary>
    /// Methods for getting the info on
    /// DataTool and Overwatch.
    /// </summary>
    class Validation
    {
        /* Validate Overwatch and return its version and branch
        public static string[] Overwatch(string path)
        {
            string version;
            
        } */
        public static string[] Overwatch(string owPath)
        {
            string[] buildInfo;
            List<string> versions = new List<string>();
            string latestVersion = "0.00.0.0.00000";
            try
            {
                buildInfo = File.ReadAllLines(owPath + "\\.build.info");
                Regex pattern = new Regex(@"^\d.\d{2}.\d.\d.\d{5}");
                foreach (string buildEntry in buildInfo)
                {
                    string[] keys = buildEntry.Split('|');
                    foreach (string key in keys)
                    {
                        if (pattern.IsMatch(key)) versions.Add(key);
                    }
                }
                foreach (string version in versions)
                {
                    if (Int32.Parse(version.Substring(version.Length - 5)) >
                        Int32.Parse(latestVersion.Substring(latestVersion.Length - 5)))
                    {
                        latestVersion = version;
                    }
                }
            }
            catch { }
            if (latestVersion == "0.00.0.0.00000")
            {
                // throw new FileNotFoundException("Failed to read version info for " + owPath +
                //     ": .build.info invalid.");
                latestVersion = "Unknown";
            }
            // Attempt to get branch info
            string branch = "Live/Unknown";
            try
            {
                string pdbPath = Path.Combine(owPath, ".product.db");
                StreamReader pdbStream = new StreamReader(pdbPath);
                using (var reader = File.OpenText(pdbPath))
                {
                    string pdbRead = pdbStream.ReadLine();
                    while (pdbStream.Peek() >= 0)
                    {
                        pdbRead = pdbStream.ReadLine();
                        if (pdbRead.Contains("prometheus_test")) branch = "PTR";
                        if (pdbRead.EndsWith("B&")) break;
                    }
                }
                return new string[] { latestVersion, branch };
            }
            catch
            {
                // If .product.db does not even exist
                return null;
            }
        }
        // Validate DataTool and ow.keys
        public static string[] DataTool(string path)
        {
            string version;
            string keys = null;
            try
            {
                version = FileVersionInfo.GetVersionInfo(
                    Path.Combine(path, "DataTool.exe"))
                    .FileVersion;
            }
            catch
            {
                version = null;
            }
            if (File.Exists(
                Path.Combine(path, "ow.keys")))
                keys = "ok";
            return new string[] { version, keys };
        }

        public static string QuickDataTool
        {
            get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }
    }
}
