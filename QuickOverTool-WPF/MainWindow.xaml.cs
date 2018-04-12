using QuickDataTool.Logics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using static QuickDataTool.Properties.Settings;

namespace QuickDataTool
{
    public partial class MainWindow : Window
    {
        // DataTool path and PID
        int dataToolPID = -1;
        string sharedPath = Path.GetDirectoryName
            (Assembly.GetEntryAssembly().CodeBase).Substring(6);
        UIString uiStringProvider = new UIString();
        // Mode parameters dictionary
        Dictionary<string, string> modes = new Dictionary<string, string>();
        // Populate mode dictionary and load config upon application launch
        public MainWindow()
        {
            InitializeComponent();
            
            FlushInst();
            DataContext = uiStringProvider;

            configProvider.InitConfig();
            InitializeDataToolHandling();
            InitializeLogging();

            CheckGUIUpdate();
            CheckDTUpdate();
        }
        // Save config and close application upon MainWindow closure
        protected override void OnClosed(EventArgs e)
        {
            Default.Upgrade();
            Default.Save();
            base.OnClosed(e);
            System.Windows.Application.Current.Shutdown();
        }
        // Read paths from config file
        private void ReadConfig()
        {
            textBoxOverwatchPath.Text = Default.Path_CurrentOW;
            textBoxOutputPath.Text = Default.Path_Output;
        }

        // Get mode selection
        private string GetMode()
        {
            ComboBoxItem selection = new ComboBoxItem();
            if (radioButtonListMode.IsChecked == true)
            {
                selection = (ComboBoxItem)comboBoxList.SelectedItem;
                return modes[selection.Name];
            }
            else if (radioButtonExtractMode.IsChecked == true)
            {
                selection = (ComboBoxItem)comboBoxExtract.SelectedItem;
                return modes[selection.Name];
            }
            else
            {
                groupBoxModesNew.BorderBrush = new SolidColorBrush(Colors.Red);
                throw new ArgumentException("Mode is not selected; please select a mode.");
            }
        }
        // 选定输出路径
        private void buttonOutputPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            DialogResult folderBrowserResult = folderBrowser.ShowDialog();
            textBoxOutputPath.Text = folderBrowser.SelectedPath;
            Logging.GetInstance().Increment("Output path: " + textBoxOutputPath.Text);
            textBoxOutputPath.BorderBrush = new SolidColorBrush(Colors.Blue);
            return;
        }
        // 开始
        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            textBoxOutput.Text = "";    // Clear log output
            string command;
            try
            {
               command = FabricateCmdline();
            }
            catch (ArgumentException x)
            {
                Logging.GetInstance().Increment(x.Message);
                return;
            }
            // Launch
            Logging.GetInstance().Increment("Time now: " + DateTime.Now.ToString());
            Logging.GetInstance().Increment("Cmdline: DataTool.exe" + command);
            Logging.GetInstance().Increment("Output: " + textBoxOutputPath.Text);
            StartUp(command);
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
                Logging.GetInstance().Increment("Output path invalid or permission denied.");
            }
        }

        private void buttonSaveCmdline_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] batFile = {"echo off\n",
                ".\\DataTool.exe" + FabricateCmdline(),
                "\npause\n"};
                File.WriteAllLines(".\\_" + GetMode() + ".bat", batFile);
                Logging.GetInstance().Increment("Written batch file for mode " + GetMode() +
                    " to _" + GetMode() + ".bat.");
            }
            catch
            {
                Logging.GetInstance().Increment("Settings invalid. Please check your mode settings.");
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
                    Logging.GetInstance().Increment("DataTool is terminated.");
                }
                else if (prompt == System.Windows.Forms.DialogResult.No) return;
            }
            catch
            {
                Logging.GetInstance().Increment("Termination failed; DataTool might not be running.");
                return;
            }
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

        private void UpdateQueryEditor(object sender, RoutedEventArgs e)
        {
            if (radioButtonExtractMode.IsChecked == true && 
                (e_heroUnlocks.IsSelected == true ||
                e_npcs.IsSelected == true ||
                e_heroVoice.IsSelected == true))
                buttonExtractQuery.Visibility = Visibility.Visible;
            else buttonExtractQuery.Visibility = Visibility.Hidden;
        }
    }
}
