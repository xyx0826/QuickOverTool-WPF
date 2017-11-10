using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Linq;

namespace QuickOverTool_WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        // Some useful variables
        int dataToolPID = -1;
        string sharedPath = Path.GetDirectoryName
            (Assembly.GetEntryAssembly().CodeBase).Substring(6);
        Dictionary<string, string> modes = new Dictionary<string, string>();
        // 主窗体初始化
        public MainWindow()
        {
            InitializeComponent();
            FlushChecklist();
            // Populate the mode dictionary
            modes.Add("radioButtonListHeroes", "list-heroes");
            modes.Add("radioButtonListGeneralCosmetics", "list-general-unlocks");
            modes.Add("radioButtonListHeroCosmetics", "list-unlocks");
            modes.Add("radioButtonListMaps", "list-maps");
            modes.Add("radioButtonListStrings", "dump-strings");
            modes.Add("radioButtonListLootbox", "extract-lootbox");
            modes.Add("radioButtonListKeys", "list-keys");
            modes.Add("radioButtonExtractGeneralCosmetics", "extract-general");
            modes.Add("radioButtonExtractHeroCosmetics", "extract-unlocks");
            modes.Add("radioButtonExtractMaps", "extract-maps");
            modes.Add("radioButtonExtractLootbox", "extract-lootbox");
            modes.Add("radioButtonListSubtitles", "list-subtitles");
            modes.Add("radioButtonListSubtitlesReal", "list-subtitles-real");
            modes.Add("radioButtonListHighlights", "list-highlights");
            modes.Add("radioButtonListChat", "list-chat-replacements");
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
                labelOverToolExecutable.Content = "Not Found";
            }
            // DataTool 完整性
            if (dataTool[1] != null)
            {
                labelOverToolIntegrity.Foreground = new SolidColorBrush(Colors.Green);
                labelOverToolIntegrity.Content = "Found";
            }
            else
            {
                labelOverToolIntegrity.Foreground = new SolidColorBrush(Colors.Red);
                labelOverToolIntegrity.Content = "Not Found";
            }
            // 守望先锋版本
            if (overwatch != null)
            {
                labelValidity.Content = "Overwatch is Valid";
                labelValidity.Foreground = new SolidColorBrush(Colors.Green);
                textBoxOverwatchPath.BorderBrush = new SolidColorBrush(Colors.Blue);

                labelOverwatchVersion.Foreground = new SolidColorBrush(Colors.Green);
                labelOverwatchVersion.Content = overwatch[0];
                labelOverwatchBranch.Foreground = new SolidColorBrush(Colors.Green);
                labelOverwatchBranch.Content = overwatch[1];
                // DataTool does not support PTR builds
                if (overwatch[1] == "PTR")
                {
                    AddLog("(Potential) PTR build detected. DataTool is incompatible with PTR.");
                    labelOverwatchBranch.Foreground = new SolidColorBrush(Colors.Red);
                } 
            }
            else
            {
                labelValidity.Content = "Overwatch is Invalid";
                textBoxOverwatchPath.BorderBrush = new SolidColorBrush(Colors.Red);
                labelValidity.Foreground = new SolidColorBrush(Colors.Red);

                labelOverwatchVersion.Foreground = new SolidColorBrush(Colors.Red);
                labelOverwatchVersion.Content = "N/A";
                labelOverwatchBranch.Foreground = new SolidColorBrush(Colors.Red);
                labelOverwatchBranch.Content = "N/A";
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

        // 检测模式选择
        // 为模式返回命令行参数，并决定输出路径与额外选项的可用性
        private string whichRadioButton()
        {
            foreach (System.Windows.Controls.RadioButton selection 
                in gridGroupBoxModes.Children)
            {
                if (selection.IsChecked == true) return modes[selection.Name];
            }
            return null;
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
            AddLog("Output path: " + textBoxOutputPath.Text);
            textBoxOutputPath.BorderBrush = new SolidColorBrush(Colors.Blue);
            return;
        }
        // 开始
        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            // Refresh the checklist once again
            FlushChecklist();
            textBoxOutput.Text = "";
            // 判断：是否选择了守望先锋路径
            if (textBoxOutputPath.IsEnabled && 
                String.IsNullOrEmpty(textBoxOverwatchPath.Text)) return;
            // 命令行：选定语言
            string cmdLine = " --language=" + comboBoxLanguage.SelectedItem.
                ToString().Substring(38, 4);
            // 命令行：复选框
            if (checkBoxQuiet.IsChecked == true) cmdLine += " --quiet";
            if (checkBoxSkipKeys.IsChecked == true) cmdLine += " --skip-keys";
            if (checkBoxGraceful.IsChecked == true) cmdLine += " --graceful-exit";
            if (checkBoxExpert.IsChecked == true) cmdLine += " --expert";

            if (checkBoxRCN.IsChecked == true) cmdLine += " --rcn";
            if (checkBoxCDNValidate.IsChecked == true) cmdLine += " --validate-cache";
            if (checkBoxCDNIndex.IsChecked == true) cmdLine += " --cache";
            if (checkBoxCDNData.IsChecked == true) cmdLine += " --cache-data";
            // 命令行：守望先锋路径
            cmdLine = cmdLine + " \"" + textBoxOverwatchPath.Text + "\" ";
            // 命令行：模式判断 + 选定模式
            if (whichRadioButton() != null) cmdLine += whichRadioButton();
            else
            {
                groupBoxModes.BorderBrush = new SolidColorBrush(Colors.Red);
                AddLog("Please select a mode.");
                return;
            }
            // 命令行：导出路径
            if (textBoxOutputPath.IsEnabled == true && 
                !String.IsNullOrEmpty(textBoxOutputPath.Text))
            {
                cmdLine = cmdLine + " \"" + textBoxOutputPath.Text + "\"";
            }
            // If custom cmdline is specified
            if (checkBoxCommand.IsChecked == true)
            {
                cmdLine = " " + textBoxCommand.Text;
                AddLog("Custom cmdline is checked.");
            }
            else textBoxCommand.Text = cmdLine;
            // Launch
            AddLog("Time now: " + DateTime.Now.ToString());
            AddLog("Cmdline: DataTool.exe" + cmdLine);
            AddLog("Output: " + textBoxOutputPath.Text);
            StartUp(cmdLine);
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
                try
                {
                    dataTool.Start();
                    dataToolPID = dataTool.Id;
                }
                catch
                {
                    AddLog("Launch unsuccessful. Check DataTool validity.");
                    return;
                }
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
            try
            {
                File.WriteAllText(Path.Combine(textBoxOutputPath.Text, "log.txt"), textBoxOutput.Text);
                textBoxOutput.Text = "Log written to " + Path.Combine(textBoxOutputPath.Text, "log.txt");
            }
            catch
            {
                textBoxOutputPath.BorderBrush = new SolidColorBrush(Colors.Red);
                AddLog("Output path invalid or permission denied.");
            }
        }

        private void buttonTaskkill_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process proc = Process.GetProcessById(dataToolPID);
                DialogResult prompt = System.Windows.Forms.MessageBox.
                    Show("DataTool is still running. Terminate?", "Confirm", MessageBoxButtons.YesNo);
                if (prompt == System.Windows.Forms.DialogResult.Yes)
                {
                    proc.Kill();
                    AddLog("DataTool is terminated.");
                }
                else if (prompt == System.Windows.Forms.DialogResult.No) return;
            }
            catch
            {
                AddLog("Termination failed; DataTool might not be running.");
                return;
            }
        }

        private void buttonAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow window = new AboutWindow();
            window.Show();
        }

        private void checkBoxCommand_Checked(object sender, RoutedEventArgs e)
        {
            textBoxCommand.IsEnabled = true;
            textBoxCommand.BorderBrush = new SolidColorBrush(Colors.Green);
        }

        private void checkBoxCommand_UnChecked(object sender, RoutedEventArgs e)
        {
            textBoxCommand.IsEnabled = false;
            textBoxCommand.BorderBrush = new SolidColorBrush(Colors.Gray);
        }

        private void buttonClearCommand_Click(object sender, RoutedEventArgs e)
        {
            textBoxCommand.Text = "";
        }
    }
}
