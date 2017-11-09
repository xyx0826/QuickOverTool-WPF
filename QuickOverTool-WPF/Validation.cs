using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace QuickOverTool_WPF
{
    class Validation
    {
        // 检查守望先锋
        public static bool Overwatch(string path)
        {
            MainWindow.SetLabel("test");
            string buildInfoPath = Path.Combine(path, ".build.info");
            if (File.Exists(buildInfoPath)) return true;
            else return false;
        }
        // 检查 DataTool
        public static bool DataTool()
        {
            string sharedPath = Path.GetDirectoryName
                (Assembly.GetEntryAssembly().CodeBase).Substring(6);
            string dataToolExecutable = Path.Combine
                (sharedPath, "DataTool.exe");
            if (File.Exists(dataToolExecutable))
            {
                string cascLibDll = Path.Combine(sharedPath, "CascLib.dll");
                return true;
            }
            else
            {
                return false;
            }
        }
        // .product.db 读取守望先锋信息
        public static void GetOverwatchInfo()
        {
            if (!File.Exists(Path.Combine(MainWindow.tbOWPath, ".product.db")))
            {
                labelOverwatchBranch.Content = "无法读取";
                return;
            }

            string pdbPath = Path.Combine(textBoxOverwatchPath.Text, ".product.db");
            string pdbRead;
            StreamReader pdbStream = new StreamReader(pdbPath);

            using (var reader = File.OpenText(pdbPath))
            {
                pdbRead = pdbStream.ReadLine();
                while (pdbStream.Peek() >= 0)
                {
                    pdbRead = pdbStream.ReadLine();
                    if (pdbRead.EndsWith("B&")) break;
                }
                pdbStream.BaseStream.Position = 0;
                pdbStream.DiscardBufferedData();
                while (pdbStream.Peek() >= 0)
                {
                    pdbRead = pdbStream.ReadLine();
                    if (pdbRead.Contains("prometheus"))
                    {
                        labelOverwatchBranch.Content = "正式服";
                        pdbStream.Close();
                        break;
                    }
                }
            }
        }
        // 检查单更新
        public static void Checklist()
        {
            // Validation.DataTool 核心程序
            if (DataTool())
            {
                labelOverToolExecutable.Foreground = new SolidColorBrush(Colors.Green);
                labelOverToolExecutable.Content = "有效";
            }
            else
            {
                labelOverToolExecutable.Foreground = new SolidColorBrush(Colors.Red);
                labelOverToolExecutable.Content = "无效";
            }
            // Validation.DataTool 完整性
            if (File.Exists(Path.Combine(sharedPath, "OWLib.dll")) &&
                File.Exists(Path.Combine(sharedPath, "CascLib.dll")) &&
                File.Exists(Path.Combine(sharedPath, "ow.keys")))
            {
                labelOverToolIntegrity.Foreground = new SolidColorBrush(Colors.Green);
                labelOverToolIntegrity.Content = "完整";
            }
            else
            {
                labelOverToolIntegrity.Foreground = new SolidColorBrush(Colors.Red);
                labelOverToolIntegrity.Content = "不完整";
            }
            // 守望先锋信息
            GetOverwatchInfo();
            // 守望先锋完整性
            if (File.Exists(Path.Combine(textBoxOverwatchPath.Text, "Overwatch Launcher.exe")))
            {
                labelOverwatchIntegrity.Foreground = new SolidColorBrush(Colors.Green);
                labelOverwatchIntegrity.Content = "有效";
            }
            else
            {
                labelOverwatchIntegrity.Foreground = new SolidColorBrush(Colors.Red);
                labelOverwatchIntegrity.Content = "不完整";
            }
            // CascLib 版本
            if (File.Exists(cascLibDll))
            {
                labelCascLibVersion.Content =
                FileVersionInfo.GetVersionInfo(Path.Combine(sharedPath, "OWLib.dll")).FileVersion;
            }
        }
    }
}
