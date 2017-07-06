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
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        // 全局变量：二进制文件路径
        private string sharedPath;
        private string buildInfoPath;
        private string pdbPath;
        private string overToolExecutable;
        private string cascLibDll;
        // 全局变量：.build.info 读取
        private StreamReader pdbStream;
        // 全局变量：OverTool 进程 ID
        private int overToolPID;
        // 主窗体初始化
        public MainWindow()
        {
            InitializeComponent();

            // 启动检查守望先锋 & OverTool 有效性
            if (!String.IsNullOrEmpty(textBoxOverwatchPath.Text))
            {
                CheckOverwatchValidity(textBoxOverwatchPath.Text);
            }
            UpdateChecklist();
        }
        // 检查守望先锋
        public bool CheckOverwatchValidity(string path)
        {
            buildInfoPath = Path.Combine(path, ".build.info");
            if (File.Exists(buildInfoPath))
            {
                labelValidity.Content = "守望先锋目录有效";
                textBoxOverwatchPath.BorderBrush = new SolidColorBrush(Colors.Blue);
                labelValidity.Foreground = new SolidColorBrush(Colors.Green);
                return true;
            }
            else
            {
                buildInfoPath = null;
                labelValidity.Content = "守望先锋目录无效";
                textBoxOverwatchPath.BorderBrush = new SolidColorBrush(Colors.Red);
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
                return true;
            }
            else
            {
                return false;
            }
        }
        // 检查单更新
        public void UpdateChecklist()
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
        // .product.db 读取守望先锋信息
        public void GetOverwatchInfo()
        {
            if (!File.Exists(Path.Combine(textBoxOverwatchPath.Text, ".product.db")))
            {
                labelOverwatchBranch.Content = "无法读取";
                return;
            }

            pdbPath = Path.Combine(textBoxOverwatchPath.Text, ".product.db");
            string pdbRead;
            pdbStream = new StreamReader(pdbPath);

            using (var reader = File.OpenText(pdbPath))
            {
                pdbRead = pdbStream.ReadLine();
                while (pdbStream.Peek() >= 0)
                {
                    pdbRead = pdbStream.ReadLine();
                    if (pdbRead.EndsWith("B&"))
                    {
                        break;
                    }
                }
                pdbStream.BaseStream.Position = 0;
                pdbStream.DiscardBufferedData();
                while (pdbStream.Peek() >= 0)
                {
                    pdbRead = pdbStream.ReadLine();
                    if (pdbRead.Contains("prometheus_test"))
                    {
                        labelOverwatchBranch.Content = "PTR (测试服)";
                        pdbStream.Close();
                        break;
                    }
                    else if (pdbRead.Contains("prometheus"))
                    {
                        labelOverwatchBranch.Content = "正式服";
                        pdbStream.Close();
                        break;
                    }
                }
            }
        }
        // 日志输出增行
        public delegate void AddLogRuntime(string content);

        public void AddLog(string content)
        {
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
        // 为模式返回命令行参数，并决定输出路径与额外选项的可用性
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
                NoSpecify();
                return "G";
            }
            else if (radioButtonExtractHeroCosmetics.IsChecked == true)
            {
                NoSpecify();
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
            else if (radioButtonExtractAnnouncer.IsChecked == true)
            {
                NoSpecify();
                return "c";
            }
            else if (radioButtonExtractMapAudio.IsChecked == true)
            {
                return "a";
            }
            else if (radioButtonExtractNPCVoice.IsChecked == true)
            {
                return "v";
            }
            else return "";
        }
        // 选定守望先锋路径
        private void buttonPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            DialogResult folderBrowserResult = folderBrowser.ShowDialog();
            textBoxOverwatchPath.Text = folderBrowser.SelectedPath;
            CheckOverwatchValidity(textBoxOverwatchPath.Text);
            GetOverwatchInfo();
            UpdateChecklist(); // Thanks NGA ID: 平衡先生 (38935700)
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
            // 版权告知
            if (checkBoxCopyright.IsChecked != true)
            {
                labelCopyright.Foreground = new SolidColorBrush(Colors.Yellow);
                AddLog("守望先锋的所有资源版权均归暴雪娱乐所有，请勿将其用于商业用途。");
                return;
            }
            textBoxOutput.Text = "";
            whichRadioButton();
            // 判断：是否选择了守望先锋路径
            if (!CheckOverwatchValidity(textBoxOverwatchPath.Text)) return;
            // 判断：是否按需选择了输出路径
            if (textBoxOutputPath.IsEnabled &&
                string.IsNullOrEmpty(textBoxOutputPath.Text))
            {
                textBoxOutputPath.BorderBrush = new SolidColorBrush(Colors.Red);
                AddLog("需要选择输出路径。");
                return;
            }
            // 构建命令行
            string cmdLine;
            // 命令行：选定语言
            cmdLine = " --language=" + comboBoxLanguage.SelectedItem.
                ToString().Substring(38, 4);
            // 命令行：复选框
            if (checkBoxSkipAnimation.IsChecked == true) cmdLine = cmdLine + " -A";
            if (checkBoxSkipGUI.IsChecked == true) cmdLine = cmdLine + " -I";
            if (checkBoxSkipKeys.IsChecked == true) cmdLine = cmdLine + " -n";
            if (checkBoxSkipModel.IsChecked == true) cmdLine = cmdLine + " -M";
            if (checkBoxSkipRef.IsChecked == true) cmdLine = cmdLine + " -R";
            if (checkBoxSkipSound.IsChecked == true) cmdLine = cmdLine + " -S";
            if (checkBoxSkipTexture.IsChecked == true) cmdLine = cmdLine + " -T";
            if (checkBoxExpert.IsChecked == true) cmdLine = cmdLine + " --ex";
            if (checkBoxCollision.IsChecked == true) cmdLine = cmdLine + " -C";
            // 命令行：守望先锋路径
            cmdLine = cmdLine + " \"" + textBoxOverwatchPath.Text + "\"";
            // 命令行：模式判断 + 选定模式
            if (whichRadioButton() == "")
            {
                groupBoxModes.BorderBrush = new SolidColorBrush(Colors.Red);
                AddLog("请选择工作模式。");
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
                cmdLine = cmdLine + " \"" + textBoxSpecify.Text + "\"";
            // 命令行：提取英雄内容
            if (radioButtonExtractHeroCosmetics.IsChecked == true)
            {
                if (string.IsNullOrEmpty(textBoxExtractionHero.Text))
                {
                    AddLog("没有指定提取英雄。请使用指定的游戏语言中的名称。");
                    return;
                }
                string[] selectedItem = comboBoxExtractionType.SelectedItem.
                    ToString().Split('（');
                string cosmeticsType = selectedItem[0].Substring(38, selectedItem[0].Length - 38);
                cmdLine = cmdLine + " \"" + cosmeticsType + "\"" 
                    + " \"" + textBoxExtractionHero.Text + "\"";
            }
            // 命令行：跳过提取 - 已弃用
            /*
            if (radioButtonExtractHeroCosmetics.IsChecked == true)
            {
                cmdLine = cmdLine + " +";
                if (checkBoxSkipTexture.IsChecked == false) cmdLine = cmdLine + 't';
                else cmdLine = cmdLine + 'T';
                if (checkBoxSkipAnimation.IsChecked == false) cmdLine = cmdLine + 'a';
                else cmdLine = cmdLine + 'A';
                if (checkBoxSkipModel.IsChecked == false) cmdLine = cmdLine + 'm';
                else cmdLine = cmdLine + 'M';
                if (checkBoxSkipSound.IsChecked == false) cmdLine = cmdLine + 's';
                else cmdLine = cmdLine + 'S';
                // Default, not all models have collision models
                cmdLine = cmdLine + 'c';
                if (checkBoxSkipRef.IsChecked == false) cmdLine = cmdLine + 'r';
                else cmdLine = cmdLine + 'R';
                if (checkBoxSkipGUI.IsChecked == false) cmdLine = cmdLine + 'i';
                else cmdLine = cmdLine + 'I';
            }
            */
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
                    overTool.StartInfo.StandardOutputEncoding = Encoding.Default;
                    overTool.StartInfo.RedirectStandardError = true;
                    overTool.StartInfo.StandardErrorEncoding = Encoding.Default;
                    overTool.StartInfo.CreateNoWindow = true;
                }

                overTool.Start();
                overToolPID = overTool.Id;
                overTool.BeginOutputReadLine();
                overTool.BeginErrorReadLine();
                overTool.OutputDataReceived += new DataReceivedEventHandler(OverTool_DataReceived);
                overTool.ErrorDataReceived += new DataReceivedEventHandler(OverTool_DataReceived);
            }
        }
        // 获取输出
        private void OverTool_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                AddLog(Encoding.UTF8.GetString
                    (Encoding.Default.GetBytes(e.Data)));
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
                Process proc = Process.GetProcessById(overToolPID);
                proc.Kill();
                AddLog("OverTool 进程被强行终止了。");
            }
            catch
            {
                AddLog("未能终止 OverTool 进程；或许其并未在运行。");
                return;
            }
        }

        private void checkBoxCopyright_Checked(object sender, RoutedEventArgs e)
        {
            checkBoxCopyright.BorderBrush = new SolidColorBrush(Colors.White);
        }
    }
}
