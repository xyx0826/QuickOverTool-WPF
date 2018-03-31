using System;
using System.ComponentModel;
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
            // Output path fabrication
            string outputPath;
            if (!String.IsNullOrEmpty(textBoxOutputPath.Text))
            {
                outputPath = " \"" + textBoxOutputPath.Text.Replace("\\", "\\\\") + "\"";
            }
            else
            {
                textBoxOutputPath.BorderBrush = new SolidColorBrush(Colors.Red);
                outputPath = " .\\";
                AddLog("Output path not found; setting it to DataTool directory.");
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
            if (checkBoxNoMdl.IsChecked == true) cmdLine += " --convert-models=false";
            if (checkBoxNoAni.IsChecked == true) cmdLine += " --convert-animations=false";
            if (checkBoxRefpose.IsChecked == true) cmdLine += " --extract-refpose=true";
            if (textBoxLOD.Text != "0") cmdLine += " --lod=" + textBoxLOD.Text;
            if (checkBoxJSONOut.IsChecked == true) cmdLine += " --json";
            if (checkBoxNoTex.IsChecked == false)
                cmdLine += (" --convert-textures-type=" + comboBoxTextureFmt.SelectedItem. // Texture conversion type
                    ToString().Substring(38, 3));

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
                cmdLine += GetRadioButton();
            }
            catch
            {
                groupBoxModesNew.BorderBrush = new SolidColorBrush(Colors.Red);
                throw new ArgumentException("Mode is not selected; please select a mode.");
            }
            // Output path addition
            cmdLine += outputPath;
            // Extract queries
            if (radioButtonExtractMode.IsChecked == true && 
                (e_heroUnlocks.IsSelected == true ||
                e_npcs.IsSelected == true ||
                e_heroVoice.IsSelected == true ||
                e_maps.IsSelected == true))
            {
                if (String.IsNullOrWhiteSpace(query.GetQueries()) &&
                    (e_heroUnlocks.IsSelected == true ||
                    e_npcs.IsSelected == true))
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
                { // DataTool process configuration
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
                    BackgroundWorker aliveChecker = new BackgroundWorker();
                    aliveChecker.DoWork += CheckAlive;
                    aliveChecker.RunWorkerCompleted += OnProcessDead;

                    dataTool.Start();
                    dataToolPID = dataTool.Id;
                    aliveChecker.RunWorkerAsync(dataTool);

                    buttonStart.IsEnabled = false;
                    buttonStart.Content = "DataTool is running...";
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

        private void CheckAlive(object sender, DoWorkEventArgs e)
        {
            ((Process)e.Argument).WaitForExit();
        }

        private void OnProcessDead(object sender, RunWorkerCompletedEventArgs e)
        {
            buttonStart.IsEnabled = true;
            buttonStart.Content = "DataTool has exited.";
            buttonStart.Foreground = Brushes.Green;
        }
    }
}
