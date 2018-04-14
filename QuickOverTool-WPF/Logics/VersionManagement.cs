using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace QuickDataTool
{
    class VersionManagement
    {
        private bool _isOWValid;
        public string GetOWVersion(string owPath)
        {
            string[] buildInfo;
            _isOWValid = true;
            List<string> versions = new List<string>();
            string latestVersion = "0.00.0.0.00000";
            try
            {
                buildInfo = File.ReadAllLines(owPath + "\\.build.info");
            }
            catch
            {
                _isOWValid = false;
                return "(Version unknown)";
                // throw new FileNotFoundException("Failed to read version info for " + owPath + ": .build.info not found.");
            }
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
            if (latestVersion != "0.00.0.0.00000") return latestVersion;
            else
            {
                _isOWValid = false;
                throw new FileNotFoundException("Failed to read version info for " + owPath +
                    ": .build.info invalid.");
            }
        }

        public bool IsOWPtr(string owPath)
        {
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
                        if (pdbRead.Contains("prometheus_test")) return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public List<String> CheckDTIntegerity(string dtPath)
        {
            string[] files = {"\\CascLib.dll",
                              "\\DataTool.exe",
                              "\\DynamicExpresso.Core.dll",
                              "\\librevorb.dll",
                              "\\Newtonsoft.Json.dll",
                              "\\OpenTK.dll",
                              "\\ow.events",
                              "\\ow.keys",
                              "\\OWlib.dll",
                              "\\OWReplayLib.dll",
                              "\\RevorbStd.dll",
                              "\\STULib.dll",
                              "\\System.Data.HashFunction.Core.dll",
                              "\\System.Data.HashFunction.CRC.dll",
                              "\\System.Data.HashFunction.Interfaces.dll",
                              "\\ZstdNet.dll",
                              "\\Third Party\\packed_codebooks_aoTuV_603.bin",
                              "\\Third Party\\texconv.exe",
                              "\\x64\\libzstd.dll",
                              "\\x86\\libzstd.dll"};
            List<string> missingFiles = new List<string>();
            foreach (string file in files)
            {
                if (!File.Exists(dtPath + file)) missingFiles.Add(file);
            }
            return missingFiles;
        }

        public string GetDTVersion(string dtPath)
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
