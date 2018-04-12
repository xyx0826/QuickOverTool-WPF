using System;
using System.Windows;
using System.Collections.Generic;
using static QuickDataTool.Properties.Settings;
using QuickDataTool.Logics;

namespace QuickDataTool
{
    public partial class MainWindow : Window
    {
        #region Initialization
        ListAssetsHandler listAssetsHandler = new ListAssetsHandler();
        ExtrAssetsHandler extrAssetsHandler = new ExtrAssetsHandler();
        public void InitializeDataToolHandling()
        {
            tabListAssets.DataContext = listAssetsHandler;
            tabExtrAssets.DataContext = extrAssetsHandler;
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
        public void ResetOptions(object sender, RoutedEventArgs e)
        {
            listAssetsHandler.ResetOptions();
            extrAssetsHandler.ResetOptions();
        }

        public Object[] GetSummary()
        {
            Object[] values = new Object[] {listAssetsHandler.ComboBoxIndex,
                listAssetsHandler.ComboBoxMode.Value,
                listAssetsHandler.IsJson,
                listAssetsHandler.ComboBoxMode};
            return values;
        }

        public void Launch(object sender, RoutedEventArgs e)
        {
            if (listAssetsHandler.ComboBoxMode.Value == null) // Check mode selection
            {
                uiStringProvider.SetNotif(lblNotif.Dispatcher, "ERROR: Please choose a list mode.");
                return;
            }
            tabControl.SelectedIndex = 4; // Jump to log tab
            string cmdLine = " \"" + uiStringProvider.CurrentOWPath + "\" " + ((KeyValuePair<string, string>)Default.TAB2_Array[1]).Value;
            if ((bool)Default.TAB2_Array[2]) cmdLine += " --json";
            Logging.GetInstance().Increment(logBox.Dispatcher, "Starting DataTool now. Cmdline: DataTool.exe " + cmdLine);
            StartDataTool(PrepareDataTool(cmdLine));
        }
        #endregion
    }
}