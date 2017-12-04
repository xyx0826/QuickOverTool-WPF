using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace QuickOverTool_WPF
{
    public class VersionManagement
    {
        public string GetOverwatchVersion(string owPath)
        {
            List<string> versions = new List<string>();
            string latestVersion = "0.00.0.0.00000";
            string[] buildInfo = File.ReadAllLines(owPath + ".build.info");
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
                if (Int32.Parse(version.Substring(version.Length - 4)) >
                    Int32.Parse(latestVersion.Substring(latestVersion.Length - 4)))
                {
                    latestVersion = version;
                }
            }
            return latestVersion;
        }

        public List<String> CheckDataTool(string dtPath)
        {
            string[] files = {"CascLib.dll",
                              "DataTool.exe",
                              "ow.events",
                              "ow.keys",
                              "OWlib.dll",
                              "OWReplayLib.dll",
                              "STULib.dll",
                              "Third Party\\packed_codebooks_aoTuV_603.bin",
                              "Third Party\\revorb.exe",
                              "Third Party\\texconv.exe",
                              "Third Party\\ww2ogg.exe" };
            List<string> missingFiles = new List<string>();
            foreach (string file in files)
            {
                if (!File.Exists(dtPath + file)) missingFiles.Add(file);
            }
            return missingFiles;
        }

        public string GetDataToolVersion(string dtPath)
        {
            string version;
            try
            {
                version = FileVersionInfo.GetVersionInfo(
                    Path.Combine(dtPath, "DataTool.exe"))
                    .FileVersion;
            }
            catch
            {
                return null;
            }
            return version;
        }
    }
}
