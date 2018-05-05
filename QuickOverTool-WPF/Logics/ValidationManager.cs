using System;
using System.Diagnostics;
using System.IO;

namespace OWorkbench
{
    /// <summary>
    /// Methods for getting the info on
    /// DataTool and Overwatch.
    /// </summary>
    class Validation
    {
        // Validate Overwatch and return its version and branch
        public static string[] Overwatch(string path)
        {
            string version;
            string branch = "Live";
            try
            {
                string pdbPath = Path.Combine(path, ".product.db");
                StreamReader pdbStream = new StreamReader(pdbPath);
                using (var reader = File.OpenText(pdbPath))
                {
                    Logging.GetInstance().IncrementDebug("Validation: now reading .product.db at installation " + path);
                    string pdbRead = pdbStream.ReadLine();
                    while (pdbStream.Peek() >= 0)
                    {
                        pdbRead = pdbStream.ReadLine();
                        if (pdbRead.Contains("prometheus_test"))
                        {
                            branch = "PTR";
                        }
                        if (pdbRead.EndsWith("B&")) break;
                    }
                    version = pdbRead.Substring(pdbRead.IndexOf("1."), 14);
                }
                return new string[] { version, branch };
            }
            catch (Exception e)
            {
                // If .product.db does not even exist
                Logging.GetInstance().IncrementDebug("Validation: error reading .product.db. " + e.Message);
                return null;
            }
        }
        // Validate DataTool and ow.keys
        public static string DataTool(string path)
        {
            string version;
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
            return version;
        }
    }
}
