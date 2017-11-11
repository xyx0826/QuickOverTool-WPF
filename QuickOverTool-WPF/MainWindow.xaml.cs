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
    /// MainWindow.xaml logics.
    /// Mostly button events.
    /// </summary>
    public partial class MainWindow : Window
    {
        // About window and query window
        AboutWindow about = new AboutWindow();
        QueryWindow query = new QueryWindow();
        // Color brushes...
        SolidColorBrush red = new SolidColorBrush(Colors.Red);
        SolidColorBrush green = new SolidColorBrush(Colors.Green);
        SolidColorBrush blue = new SolidColorBrush(Colors.Blue);
        SolidColorBrush gray = new SolidColorBrush(Colors.Gray);
        // DataTool path and PID
        int dataToolPID = -1;
        string sharedPath = Path.GetDirectoryName
            (Assembly.GetEntryAssembly().CodeBase).Substring(6);
        // Mode parameters dictionary
        Dictionary<string, string> modes = new Dictionary<string, string>();

        public MainWindow()
        {
            InitializeComponent();
            PopulateDict();
            FlushChecklist();
        }
        // Mode dictionary populator
        private void PopulateDict()
        {
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
        // Update checklist
        public void FlushChecklist()
        {
            // Reset colors of important controls
            textBoxOverwatchPath.BorderBrush = gray;
            textBoxOutputPath.BorderBrush = gray;
            groupBoxModes.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffd5dfe5"));
            buttonExtractQuery.BorderBrush = gray;
            // Retrieve results from validators
            string[] dataTool = Validation.DataTool(sharedPath);
            string[] overwatch = Validation.Overwatch(textBoxOverwatchPath.Text);
            // Datatool.exe
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
            // Ow.keys
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
            // Overwatch
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
        // Log increment
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
        // Get mode selection
        private string GetRadioButton()
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
            FlushChecklist();
            textBoxOutput.Text = "";    // Clear log output
            string command;
            try
            {
               command = FabricateCmdline();
            }
            catch (ArgumentException x)
            {
                AddLog(x.Message);
                return;
            }
            // Launch
            AddLog("Time now: " + DateTime.Now.ToString());
            AddLog("Cmdline: DataTool.exe" + command);
            AddLog("Output: " + textBoxOutputPath.Text);
            StartUp(command);
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
            about.Show();
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

        private void radioButtonExtractHeroCosmetics_Checked(object sender, RoutedEventArgs e)
        {
            buttonExtractQuery.Visibility = Visibility.Visible;
        }

        private void radioButtonExtractHeroCosmetics_Unchecked(object sender, RoutedEventArgs e)
        {
            buttonExtractQuery.Visibility = Visibility.Hidden;
        }

        private void buttonExtractQuery_Click(object sender, RoutedEventArgs e)
        {
            query.Show();
        }
    }
}
