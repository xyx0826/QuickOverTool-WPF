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
        // Some useful variables
        int dataToolPID;
        string sharedPath = Path.GetDirectoryName
            (Assembly.GetEntryAssembly().CodeBase).Substring(6);
        // 主窗体初始化
        public MainWindow()
        {
            InitializeComponent();
            FlushChecklist();
        }

        // 检查单更新
        public void FlushChecklist()
        {
            // DataTool 核心程序
            string[] dataTool = Validation.DataTool(sharedPath);
            string[] overwatch = Validation.Overwatch(textBoxOverwatchPath.Text);
            if (dataTool[0] != null)
            {
                labelOverToolExecutable.Foreground = new SolidColorBrush(Colors.Green);
                labelOverToolExecutable.Content = dataTool[0];
            }
            else
            {
                labelOverToolExecutable.Foreground = new SolidColorBrush(Colors.Red);
                labelOverToolExecutable.Content = "无效";
            }
            // DataTool 完整性
            if (dataTool[1] != null)
            {
                labelOverToolIntegrity.Foreground = new SolidColorBrush(Colors.Green);
                labelOverToolIntegrity.Content = "完整";
            }
            else
            {
                labelOverToolIntegrity.Foreground = new SolidColorBrush(Colors.Red);
                labelOverToolIntegrity.Content = "不完整";
            }
            // 守望先锋版本
            if (overwatch != null)
            {
                labelValidity.Content = "守望先锋目录有效";
                labelValidity.Foreground = new SolidColorBrush(Colors.Green);
                textBoxOverwatchPath.BorderBrush = new SolidColorBrush(Colors.Blue);

                labelOverwatchVersion.Foreground = new SolidColorBrush(Colors.Green);
                labelOverwatchVersion.Content = overwatch[0];
                labelOverwatchBranch.Foreground = new SolidColorBrush(Colors.Green);
                labelOverwatchBranch.Content = overwatch[1];
            }
            else
            {
                labelValidity.Content = "守望先锋目录无效";
                textBoxOverwatchPath.BorderBrush = new SolidColorBrush(Colors.Red);
                labelValidity.Foreground = new SolidColorBrush(Colors.Red);

                labelOverwatchVersion.Foreground = new SolidColorBrush(Colors.Red);
                labelOverwatchVersion.Content = "未知";
                labelOverwatchBranch.Foreground = new SolidColorBrush(Colors.Red);
                labelOverwatchBranch.Content = "未知";
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
        
        /* public void NoSpecify()
        {
            textBoxSpecify.Text = "";
            textBoxSpecify.IsEnabled = false;
        }

        public void NoOutput()
        {
            textBoxOutputPath.Text = "";
            textBoxOutputPath.IsEnabled = false;
        } */

        // 检测模式选择
        // 为模式返回命令行参数，并决定输出路径与额外选项的可用性
        private string whichRadioButton()
        {
            if (radioButtonListGeneralCosmetics.IsChecked == true)
            {
                // NoOutput();
                // NoSpecify();
                return "g";
            }
            else if (radioButtonListHeroCosmetics.IsChecked == true)
            {
                // NoOutput();
                // NoSpecify();
                return "t";
            }
            else if (radioButtonListMaps.IsChecked == true)
            {
                // NoOutput();
                // NoSpecify();
                return "m";
            }
            else if (radioButtonListStrings.IsChecked == true)
            {
                // NoOutput();
                // NoSpecify();
                return "s";
            }
            else if (radioButtonListLootbox.IsChecked == true)
            {
                // NoOutput();
                // NoSpecify();
                return "l";
            }
            else if (radioButtonListKeys.IsChecked == true)
            {
                // NoOutput();
                // NoSpecify();
                return "Z";
            }
            else if (radioButtonExtractGeneralCosmetics.IsChecked == true)
            {
                // NoSpecify();
                return "G";
            }
            else if (radioButtonExtractHeroCosmetics.IsChecked == true)
            {
                // NoSpecify();
                return "x";
            }
            else if (radioButtonExtractMaps.IsChecked == true)
            {
                return "M";
            }
            else if (radioButtonExtractLootbox.IsChecked == true)
            {
                // NoSpecify();
                return "L";
            }
            else return null;
        }
        // 选定守望先锋路径
        private void buttonPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            DialogResult folderBrowserResult = folderBrowser.ShowDialog();
            textBoxOverwatchPath.Text = folderBrowser.SelectedPath;
            Validation.Overwatch(textBoxOverwatchPath.Text);
            FlushChecklist();
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
            // Sync the checklist once again
            FlushChecklist();
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
            if (textBoxOutputPath.IsEnabled && 
                String.IsNullOrEmpty(textBoxOverwatchPath.Text)) return;
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
            if (checkBoxSkipKeys.IsChecked == true) cmdLine = cmdLine + " -n";
            if (checkBoxExpert.IsChecked == true) cmdLine = cmdLine + " --ex";
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
            /* Deprecated - 命令行：附加参数
            if (textBoxSpecify.IsEnabled == true)
                cmdLine = cmdLine + " \"" + textBoxSpecify.Text + "\"";
            */
            /* Deprecated - 命令行：提取英雄内容
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
            */
            AddLog("命令行：DataTool.exe" + cmdLine);
            // 启动
            StartUp(cmdLine);

            textBoxOutputPath.IsEnabled = true;
            // textBoxSpecify.IsEnabled = true;
        }
        // 启动进程
        private void StartUp(string command)
        {
            using (Process dataTool = new Process())
            {
                { // Validation.DataTool 进程配置
                    dataTool.StartInfo.FileName = "DataTool.exe";
                    dataTool.StartInfo.Arguments = command;
                    dataTool.StartInfo.UseShellExecute = false;
                    dataTool.StartInfo.RedirectStandardOutput = true;
                    dataTool.StartInfo.StandardOutputEncoding = Encoding.Default;
                    dataTool.StartInfo.RedirectStandardError = true;
                    dataTool.StartInfo.StandardErrorEncoding = Encoding.Default;
                    dataTool.StartInfo.CreateNoWindow = true;
                }

                dataTool.Start();
                dataToolPID = dataTool.Id;
                dataTool.BeginOutputReadLine();
                dataTool.BeginErrorReadLine();
                dataTool.OutputDataReceived += new DataReceivedEventHandler(DataTool_DataReceived);
                dataTool.ErrorDataReceived += new DataReceivedEventHandler(DataTool_DataReceived);
            }
        }
        // 获取输出
        private void DataTool_DataReceived(object sender, DataReceivedEventArgs e)
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
                Process proc = Process.GetProcessById(dataToolPID);
                proc.Kill();
                AddLog("DataTool 进程被强行终止了。");
            }
            catch
            {
                AddLog("未能终止 DataTool 进程；或许其并未在运行。");
                return;
            }
        }

        private void checkBoxCopyright_Checked(object sender, RoutedEventArgs e)
        {
            checkBoxCopyright.BorderBrush = new SolidColorBrush(Colors.White);
        }
    }
}
