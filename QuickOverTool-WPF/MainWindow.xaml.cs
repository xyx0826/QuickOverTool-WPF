using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace QuickOverTool_WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        // 全局变量：二进制文件路径
        private string sharedPath;
        private string buildInfoPath;
        private string overToolExecutable;
        private string cascLibDll;
        // 全局变量：.build.info 读取
        private StreamReader buildInfoStream;
        // 主窗体
        public MainWindow()
        {
            InitializeComponent();

            // 启动检查守望先锋 & OverTool 有效性
            checkOverToolValidity();
            if (!String.IsNullOrEmpty(textBoxPath.Text))
            {
                checkOverwatchValidity(textBoxPath.Text);
            }

            updateChecklist();
        }
        // 检查守望先锋
        public bool checkOverwatchValidity(string path)
        {
            buildInfoPath = Path.Combine(path, ".build.info");
            if (File.Exists(buildInfoPath))
            {
                buildInfoPath = Path.Combine(path, ".build.info");
                labelValidity.Content = "守望先锋目录有效";
                textBoxPath.BorderBrush = new SolidColorBrush(Colors.Blue);
                labelValidity.Foreground = new SolidColorBrush(Colors.Green);
                return true;
            }
            else
            {
                buildInfoPath = null;
                labelValidity.Content = "守望先锋目录无效";
                textBoxPath.BorderBrush = new SolidColorBrush(Colors.Red);
                labelValidity.Foreground = new SolidColorBrush(Colors.Red);
                return false;
            }
        }
        // 检查 OverTool
        public bool checkOverToolValidity()
        {
            sharedPath = Path.GetDirectoryName
                (Assembly.GetEntryAssembly().CodeBase).Substring(6);
            overToolExecutable = Path.Combine
                (sharedPath, "OverTool.exe");
            if (File.Exists(overToolExecutable))
            {
                cascLibDll = Path.Combine(sharedPath, "CascLib.dll");
                labelValidity.Content = "检测到了 OverTool";
                labelValidity.Foreground = new SolidColorBrush(Colors.Green);
                return true;
            }
            else
            {
                labelValidity.Content = "未检测到 OverTool";
                labelValidity.Foreground = new SolidColorBrush(Colors.Red);
                return false;
            }
        }
        // 检查单更新
        public void updateChecklist()
        {
            // Overwatch Version

            if (checkOverToolValidity())
            {
                labelOverToolExecutable.Foreground = new SolidColorBrush(Colors.Green);
                labelOverToolExecutable.Content = "有效";
            }
            else
            {
                labelOverToolExecutable.Foreground = new SolidColorBrush(Colors.Red);
                labelOverToolExecutable.Content = "无效";
            }

            if (File.Exists(Path.Combine(sharedPath, "OWLib.dll")) &&
                File.Exists(Path.Combine(sharedPath, "CascLib.dll")) &&
                File.Exists(Path.Combine(sharedPath, "ow.keys")) &&
                File.Exists(Path.Combine(sharedPath, "ow.events")))
            {
                labelOverToolIntegrity.Foreground = new SolidColorBrush(Colors.Green);
                labelOverToolIntegrity.Content = "完整";
            }
            else
            {
                labelOverToolIntegrity.Foreground = new SolidColorBrush(Colors.Red);
                labelOverToolIntegrity.Content = "不完整";
            }

            labelCascLibVersion.Content =
                FileVersionInfo.GetVersionInfo(Path.Combine(sharedPath, "CascLib.dll")).FileVersion;

            buildInfoReader();
        }

        public void buildInfoReader()
        {
            string buildInfoRead;
            if (string.IsNullOrEmpty(buildInfoPath)) return;
            else
            {
                buildInfoStream = new StreamReader(buildInfoPath);
                {
                    try
                    {
                        while (buildInfoStream.Peek() >= 0)
                        {
                            buildInfoRead = buildInfoStream.ReadLine();

                            while (!buildInfoRead.StartsWith("us"))
                            {
                                buildInfoRead = buildInfoStream.ReadLine();
                                if (buildInfoRead.StartsWith("us")) break;
                            }
                            
                            var buildInfoParsed = buildInfoRead.Split('|');
                            labelOverwatchVersion.Content = buildInfoParsed[11];
                            labelOverwatchBranch.Content = buildInfoParsed[6];
                            break;
                        }
                    }
                    catch
                    {
                        labelOverwatchVersion.Content = "无法读取";
                        labelOverwatchBranch.Content = "无法读取";
                    }
                    
                }
            }
        }
        // 日志输出增行
        public void addLog(string content)
        {
            textBoxOutput.AppendText("\n" + content);
        }
        // 检测模式选择
        private string whichRadioButton()
        {
            if (radioButtonListGeneralCosmetics.IsChecked == true) return "g";
            else if (radioButtonListHeroCosmetics.IsChecked == true) return "t";
            else if (radioButtonListMaps.IsChecked == true) return "m";
            else if (radioButtonListTextures.IsChecked == true) return "T";
            else if (radioButtonListStrings.IsChecked == true) return "s";
            else if (radioButtonListNPCs.IsChecked == true) return "n";
            else if (radioButtonListLootbox.IsChecked == true) return "I";
            else if (radioButtonListKeys.IsChecked == true) return "Z";
            else if (radioButtonExtractGeneralCosmetics.IsChecked == true) return "G";
            else if (radioButtonExtractHeroCosmetics.IsChecked == true) return "x";
            else if (radioButtonExtractMaps.IsChecked == true) return "M";
            else if (radioButtonExtractVoice.IsChecked == true) return "v";
            else if (radioButtonExtractSoundAllSkins.IsChecked == true) return "V";
            else if (radioButtonExtractWeaponSkins.IsChecked == true) return "w";
            else if (radioButtonExtractNPCs.IsChecked == true) return "N";
            else if (radioButtonExtractLootbox.IsChecked == true) return "L";
            else return "";
        }
        // 走你
        private void buttonPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            DialogResult folderBrowserResult = folderBrowser.ShowDialog();
            textBoxPath.Text = folderBrowser.SelectedPath;
            checkOverwatchValidity(textBoxPath.Text);
            buildInfoReader();
        }

        private void buttonOutputPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            DialogResult folderBrowserResult = folderBrowser.ShowDialog();
            textBoxOutputPath.Text = folderBrowser.SelectedPath;
            addLog("选定了输出路径：" + textBoxOutputPath.Text);
            return;
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            // 判断：是否选择了守望先锋路径
            if (labelValidity.Content.ToString() == "守望先锋目录无效") return;
            else if (checkOverwatchValidity(textBoxPath.Text) == true)
                textBoxPath.IsEnabled = false;
            // 构建命令行
            string cmdLine;
            // 命令行：选定语言
            cmdLine = " --language=" + comboBoxLanguage.SelectedItem.
                ToString().Substring(38, 4);
            // 命令行：复选框
            if (checkBoxSkipAnimation.IsChecked == true) cmdLine = cmdLine + " -A";
            if (checkBoxSkipGUI.IsChecked == true) cmdLine = cmdLine + "-I";
            if (checkBoxSkipKeys.IsChecked == true) cmdLine = cmdLine + "-n";
            if (checkBoxSkipModel.IsChecked == true) cmdLine = cmdLine + "-M";
            if (checkBoxSkipRef.IsChecked == true) cmdLine = cmdLine + "-R";
            if (checkBoxSkipSound.IsChecked == true) cmdLine = cmdLine + "-S";
            if (checkBoxSkipTexture.IsChecked == true) cmdLine = cmdLine + "-T";
            if (checkBoxExpert.IsChecked == true) cmdLine = cmdLine + "--ex";
            if (checkBoxCollision.IsChecked == true) cmdLine = cmdLine + "-C";
            // 命令行：守望先锋路径
            cmdLine = cmdLine + " \"" + textBoxPath.Text + "\"";
            // 命令行：模式判断 + 选定模式
            if (whichRadioButton() == "")
            {
                groupBoxModes.BorderBrush = new SolidColorBrush(Colors.Red);
                return;
            }
            else cmdLine = cmdLine + " " + whichRadioButton();
            // 命令行：导出路径
            if (textBoxOutputPath.IsEnabled == true)
            {
                cmdLine = cmdLine + " \"" + textBoxOutputPath.Text + "\"";
            }
            // 命令行：附加参数
            if (!String.IsNullOrEmpty(textBoxSpecify.Text))
                cmdLine = cmdLine + " " + textBoxSpecify.Text;
            addLog("命令行：OverTool.exe" + cmdLine);
            addLog("OverTool 现在启动...");
            // 启动
            using (Process overTool = new Process())
            {
                { // OverTool 进程配置
                    overTool.StartInfo.FileName = "OverTool.exe";
                    overTool.StartInfo.Arguments = cmdLine;
                    overTool.StartInfo.UseShellExecute = false;
                    overTool.StartInfo.RedirectStandardOutput = true;
                    overTool.StartInfo.RedirectStandardError = true;
                    overTool.StartInfo.CreateNoWindow = true;
                }

                overTool.Start();
                overTool.BeginOutputReadLine();
                overTool.BeginErrorReadLine();
            }
        }
    }
}
