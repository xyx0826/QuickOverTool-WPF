using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace QuickOverTool_WPF
{
    /// <summary>
    /// MainWindow.xaml logics.
    /// Process-related methods, e.g. cmdline, startup.
    /// </summary>
    public partial class MainWindow : Window
    {
        private string FabricateCmdline()
        {
            // If custom cmdline is specified
            if (checkBoxCommand.IsChecked == true)
            {
                AddLog("Custom cmdline is checked; using specified cmdline instead.");
                return " " + textBoxCommand.Text;
            }

            // Language
            string cmdLine = " --language=" + comboBoxLanguage.SelectedItem.
                ToString().Substring(38, 4);

            // Flags
            if (checkBoxQuiet.IsChecked == true) cmdLine += " --quiet";
            if (checkBoxSkipKeys.IsChecked == true) cmdLine += " --skip-keys";
            if (checkBoxGraceful.IsChecked == true) cmdLine += " --graceful-exit";
            if (checkBoxExpert.IsChecked == true) cmdLine += " --expert";
            if (checkBoxRCN.IsChecked == true) cmdLine += " --rcn";
            if (checkBoxCDNValidate.IsChecked == true) cmdLine += " --validate-cache";
            if (checkBoxCDNIndex.IsChecked == true) cmdLine += " --cache";
            if (checkBoxCDNData.IsChecked == true) cmdLine += " --cache-data";
            if (checkBoxNoTex.IsChecked == true) cmdLine += " --convert-textures=false";
            if (checkBoxNoSnd.IsChecked == true) cmdLine += " --convert-sound=false";

            // Overwatch path
            if (textBoxOutputPath.IsEnabled &&
                String.IsNullOrEmpty(textBoxOverwatchPath.Text))
            {
                textBoxOverwatchPath.BorderBrush = new SolidColorBrush(Colors.Red);
                throw new ArgumentException("Overwatch path not found; please specify a valid Overwatch path.");
            }

            cmdLine = cmdLine + " \"" + textBoxOverwatchPath.Text + "\" ";
            // Mode
            try
            {
                cmdLine += GetMode();
            }
            catch
            {
                groupBoxModesNew.BorderBrush = new SolidColorBrush(Colors.Red);
                throw new ArgumentException("Mode is not selected; please select a mode.");
            }
            // Export path
            if (!String.IsNullOrEmpty(textBoxOutputPath.Text))
                cmdLine = cmdLine + " \"" + textBoxOutputPath.Text + "\"";
            else
            {
                textBoxOutputPath.BorderBrush = new SolidColorBrush(Colors.Red);
                AddLog("Output path not found; setting it to DataTool directory.");
                cmdLine += " .\\";
            }

            // Extract queries
            if (radioButtonExtractMode.IsChecked == true && 
                (e_heroUnlocks.IsSelected == true ||
                e_npcs.IsSelected == true ||
                e_heroVoice.IsSelected == true))
            {
                if (String.IsNullOrWhiteSpace(query.GetQueries()) &&
                    e_heroUnlocks.IsSelected == true ||
                    e_npcs.IsSelected == true)
                {
                    buttonExtractQuery.BorderBrush = new SolidColorBrush(Colors.Red);
                    throw new ArgumentException("Queries not found; please enter a query in the editor.");
                }
                cmdLine += query.GetQueries();
            }

            textBoxCommand.Text = cmdLine;
            return cmdLine;
        }

        private void StartUp(string command)
        {
            using (Process dataTool = new Process())
            {
                { // Validation.DataTool Process Config
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
    }
}
