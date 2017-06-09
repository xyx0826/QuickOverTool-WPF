using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

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
        // 主窗体初始化
        public MainWindow()
        {
            InitializeComponent();

            // 启动检查守望先锋 & OverTool 有效性
            CheckOverToolValidity();
            if (!String.IsNullOrEmpty(textBoxPath.Text))
            {
                CheckOverwatchValidity(textBoxPath.Text);
            }
            updateChecklist();
        }
        // 检查守望先锋
        public bool CheckOverwatchValidity(string path)
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
        public bool CheckOverToolValidity()
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
            // OverTool 核心程序
            if (CheckOverToolValidity())
            {
                labelOverToolExecutable.Foreground = new SolidColorBrush(Colors.Green);
                labelOverToolExecutable.Content = "有效";
            }
            else
            {
                labelOverToolExecutable.Foreground = new SolidColorBrush(Colors.Red);
                labelOverToolExecutable.Content = "无效";
            }
            // OverTool 完整性
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
            // 守望先锋信息
            GetOverwatchInfo();
            // 守望先锋完整性
            if (File.Exists(textBoxPath.Text + "\\data\\casc\\data\\shmem"))
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
        // .build.info 读取守望先锋信息
        public void GetOverwatchInfo()
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
                            buildInfoStream.Close();
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
        public delegate void AddLogRuntime(string content);

        public void AddLog(string content)
        {
            // TODO: 编码问题
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new AddLogRuntime(AddLog), content);
                return;
            }
            else
            {
                textBoxOutput.AppendText("\n" + content);
                textBoxOutput.ScrollToEnd();
            }
        }
        
        public void NoSpecify()
        {
            textBoxSpecify.Text = "";
            textBoxSpecify.IsEnabled = false;
        }

        public void NoOutput()
        {
            textBoxOutputPath.Text = "";
            textBoxOutputPath.IsEnabled = false;
        }

        // 检测模式选择
        private string whichRadioButton()
        {
            if (radioButtonListGeneralCosmetics.IsChecked == true)
            {
                NoOutput();
                NoSpecify();
                return "g";
            }
            else if (radioButtonListHeroCosmetics.IsChecked == true)
            {
                NoOutput();
                NoSpecify();
                return "t";
            }
            else if (radioButtonListMaps.IsChecked == true)
            {
                NoOutput();
                NoSpecify();
                return "m";
            }
            else if (radioButtonListTextures.IsChecked == true)
            {
                NoOutput();
                return "T";
            }
            else if (radioButtonListStrings.IsChecked == true)
            {
                NoOutput();
                NoSpecify();
                return "s";
            }
            else if (radioButtonListNPCs.IsChecked == true)
            {
                NoOutput();
                NoSpecify();
                return "n";
            }
            else if (radioButtonListLootbox.IsChecked == true)
            {
                NoOutput();
                NoSpecify();
                return "l";
            }
            else if (radioButtonListKeys.IsChecked == true)
            {
                NoOutput();
                NoSpecify();
                return "Z";
            }
            else if (radioButtonExtractGeneralCosmetics.IsChecked == true)
            {
                return "G";
            }
            else if (radioButtonExtractHeroCosmetics.IsChecked == true)
            {
                return "x";
            }
            else if (radioButtonExtractMaps.IsChecked == true)
            {
                return "M";
            }
            else if (radioButtonExtractVoice.IsChecked == true)
            {
                return "v";
            }
            else if (radioButtonExtractSoundAllSkins.IsChecked == true)
            {
                return "V";
            }
            else if (radioButtonExtractWeaponSkins.IsChecked == true)
            {
                return "w";
            }
            else if (radioButtonExtractNPCs.IsChecked == true)
            {
                NoSpecify();
                return "N";
            }
            else if (radioButtonExtractLootbox.IsChecked == true)
            {
                NoSpecify();
                return "L";
            }
            else return "";
        }
        // 选定守望先锋路径
        private void buttonPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            DialogResult folderBrowserResult = folderBrowser.ShowDialog();
            textBoxPath.Text = folderBrowser.SelectedPath;
            CheckOverwatchValidity(textBoxPath.Text);
            GetOverwatchInfo();
        }
        // 选定输出路径
        private void buttonOutputPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            DialogResult folderBrowserResult = folderBrowser.ShowDialog();
            textBoxOutputPath.Text = folderBrowser.SelectedPath;
            AddLog("选定了输出路径：" + textBoxOutputPath.Text);
            textBoxOutputPath.BorderBrush = new SolidColorBrush(Colors.Blue);
            return;
        }
        // 开始
        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            textBoxOutput.Text = "";
            whichRadioButton();
            // 判断：是否选择了守望先锋路径
            if (labelValidity.Content.ToString() == "守望先锋目录无效") return;
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
            if (textBoxSpecify.IsEnabled == true)
                cmdLine = cmdLine + " " + textBoxSpecify.Text;
            AddLog("命令行：OverTool.exe" + cmdLine);
            // 启动
            StartUp(cmdLine);

            textBoxOutputPath.IsEnabled = true;
            textBoxSpecify.IsEnabled = true;
        }
        // 启动进程
        private void StartUp(string command)
        {
            using (Process overTool = new Process())
            {
                { // OverTool 进程配置
                    overTool.StartInfo.FileName = "OverTool.exe";
                    overTool.StartInfo.Arguments = command;
                    overTool.StartInfo.UseShellExecute = false;
                    overTool.StartInfo.RedirectStandardOutput = true;
                    overTool.StartInfo.RedirectStandardError = true;
                    overTool.StartInfo.CreateNoWindow = true;
                }

                overTool.Start();
                overTool.BeginOutputReadLine();
                overTool.BeginErrorReadLine();
                overTool.OutputDataReceived += new DataReceivedEventHandler(OverTool_DataReceived);
                overTool.ErrorDataReceived += new DataReceivedEventHandler(OverTool_DataReceived);
                AddLog("运行结束。");
            }
        }
        // 获取输出
        private void OverTool_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                if (e.Data.Contains("Exception"))
                {
                    AddLog("发生错误。请确认 OverTool 版本与守望先锋版本匹配。");
                }
                AddLog(e.Data);
            }
        }

        private void buttonSaveOutput_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxOutputPath.Text))
            {
                try
                {
                    File.WriteAllText(Path.Combine(textBoxOutputPath.Text, "log.txt"), textBoxOutput.Text);
                    textBoxOutput.Text = "写入了日志到 " + Path.Combine(textBoxOutputPath.Text, "log.txt");
                }
                catch
                {
                    textBoxOutputPath.BorderBrush = new SolidColorBrush(Colors.Red);
                }
            }
            else textBoxOutputPath.BorderBrush = new SolidColorBrush(Colors.Red);
        }

        private void buttonTaskkill_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process[] proc = Process.GetProcessesByName("OverTool");
                proc[0].Kill();
            }
            catch
            {
                buttonTaskkill.Content = "进程不存在！";
            }
        }
    }
}
