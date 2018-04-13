using System;
using System.Windows;
using System.Collections.Generic;
using static QuickDataTool.Properties.Settings;
using QuickDataTool.Logics;
using System.Windows.Controls;

namespace QuickDataTool
{
    public partial class MainWindow : Window
    {
        #region Initialization
        public void InitializeDataToolHandling()
        {
            tabListAssets.DataContext = ListAssetsHandler.GetInstance();
            tabExtrAssets.DataContext = ExtrAssetsHandler.GetInstance();
            PopulateModes();
        }
        #endregion
        #region Populate modes
        private void PopulateModes()
        {
            // Populate list modes
            List<KeyValuePair<string, string>> listModes = new List<KeyValuePair<string, string>>();
            listModes.Add(CreateMode("(Please select a mode...)", null));
            listModes.Add(CreateMode("List achievements", "list-achievements"));
            listModes.Add(CreateMode("List chat replacements", "list-chat-replacements"));
            listModes.Add(CreateMode("List general unlocks", "list-general-unlocks"));
            listModes.Add(CreateMode("List heroes", "list-heroes"));
            listModes.Add(CreateMode("List user highlights", "list-highlights"));
            listModes.Add(CreateMode("List encryption keys", "list-keys"));
            listModes.Add(CreateMode("List lootboxes", "list-lootbox"));
            listModes.Add(CreateMode("List maps", "list-maps"));
            listModes.Add(CreateMode("List subtitles", "list-subtitles"));
            listModes.Add(CreateMode("List subtitles (from audio data)", "list-subtitles-real"));
            listModes.Add(CreateMode("List unlocks", "list-hero-unlocks"));
            comboListAssets.ItemsSource = listModes;
            comboListAssets.SelectedValuePath = "Value";
            comboListAssets.DisplayMemberPath = "Key";
            // Populate extraction modes
            List<KeyValuePair<string, string>> extrModes = new List<KeyValuePair<string, string>>();
            extrModes.Add(CreateMode("(Please select a mode...)", null));
            extrModes.Add(CreateMode("Dump strings", "dump-strings"));
            extrModes.Add(CreateMode("Extract abilities", "extract-abilities"));
            extrModes.Add(CreateMode("Extract general", "extract-general"));
            extrModes.Add(CreateMode("Extract hero voice", "extract-hero-voice"));
            extrModes.Add(CreateMode("Extract lootbox", "extract-lootbox"));
            extrModes.Add(CreateMode("Extract maps", "extract-maps"));
            extrModes.Add(CreateMode("Extract map environment data", "extract-map-envs"));
            extrModes.Add(CreateMode("Extract NPCs", "extract-npcs"));
            extrModes.Add(CreateMode("Extract unlocks", "extract-unlocks"));
            comboExtrAssets.ItemsSource = extrModes;
            comboExtrAssets.SelectedValuePath = "Value";
            comboExtrAssets.DisplayMemberPath = "Key";
        }

        private static KeyValuePair<string, string> CreateMode(string displayName, string command)
        {
            return new KeyValuePair<string, string>(displayName, command);
        }
        #endregion
        #region Global interaction
        
        /// <summary>
        /// Hides and shows certain parts of the UI according to the extrmode combobox selection.
        /// </summary>
        private void comboExtrAssets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((KeyValuePair<string, string>)comboExtrAssets.SelectedItem).Value == "extract-map-envs")
                groupBoxMapOptions.Visibility = Visibility.Collapsed;
            else groupBoxMapOptions.Visibility = Visibility.Visible;
        }

        public void ResetOptions(object sender, RoutedEventArgs e)  // Fire corresponding ResetOptions()
        {
            if (((Button)sender).Name.Contains("List")) ListAssetsHandler.GetInstance().ResetOptions();
            else if (((Button)sender).Name.Contains("Extr")) ExtrAssetsHandler.GetInstance().ResetOptions();
        }

        /// <summary>
        /// Fire corresponding launch logics according to the pressed button.
        /// </summary>
        public void Launch(object sender, RoutedEventArgs e)
        {
            object handler;
            if ((((Button)sender).DataContext) == ListAssetsHandler.GetInstance()) // If the button belongs to list mode
            {
                if (ListAssetsHandler.GetInstance().ComboBoxMode.Value == null)
                {
                    UIString.GetInstance().SetNotif(lblNotif.Dispatcher, "ERROR: Please choose a list mode.");
                    return;
                }
                handler = ListAssetsHandler.GetInstance();
            }
            if ((((Button)sender).DataContext) == ExtrAssetsHandler.GetInstance()) // If the button belongs to extr mode
            {
                if (ExtrAssetsHandler.GetInstance().comboBoxMode.Value == null)
                {
                    UIString.GetInstance().SetNotif(lblNotif.Dispatcher, "ERROR: Please choose a list mode.");
                    return;
                }
                handler = ExtrAssetsHandler.GetInstance();
            }

            // Initial check passed
            tabControl.SelectedIndex = 4; // Jump to log tab
            string cmdLine = " \"" + UIString.GetInstance().CurrentOWPath + "\" " + ListAssetsHandler.GetInstance().ComboBoxMode.Value;
            if (Default.TAB2_OutputJSON) cmdLine += " --json";
            Logging.GetInstance().ClearLogs(logBox);
            Logging.GetInstance().Increment(logBox, "Starting DataTool now. Cmdline: DataTool.exe " + cmdLine);
            StartDataTool(PrepareDataTool(cmdLine));
        }
        #endregion
    }
}