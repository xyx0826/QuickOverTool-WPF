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
        public void InitializeListAssets()
        {
            tabListAssets.DataContext = listAssetsHandler;
            PopulateListAssets();
        }
        #endregion
        #region Populate modes
        private void PopulateListAssets()
        {
            List<KeyValuePair<string, string>> modes = new List<KeyValuePair<string, string>>();
            modes.Add(CreateListMode("(Please select a mode...)", null));
            modes.Add(CreateListMode("List achievements", "list-achievements"));
            modes.Add(CreateListMode("List chat replacements", "list-chat-replacements"));
            modes.Add(CreateListMode("List general unlocks", "list-general-unlocks"));
            modes.Add(CreateListMode("List heroes", "list-heroes"));
            modes.Add(CreateListMode("List user highlights", "list-highlights"));
            modes.Add(CreateListMode("List encryption keys", "list-keys"));
            modes.Add(CreateListMode("List lootboxes", "list-lootbox"));
            modes.Add(CreateListMode("List maps", "list-maps"));
            modes.Add(CreateListMode("List subtitles", "list-subtitles"));
            modes.Add(CreateListMode("List subtitles (from audio data)", "list-subtitles-real"));
            modes.Add(CreateListMode("List unlocks", "list-hero-unlocks"));
            comboListAssets.ItemsSource = modes;
            comboListAssets.SelectedValuePath = "Value";
            comboListAssets.DisplayMemberPath = "Key";
        }

        private static KeyValuePair<string, string> CreateListMode(string displayName, string command)
        {
            return new KeyValuePair<string, string>(displayName, command);
        }
        #endregion
        #region Global interaction
        public void ResetOptions(object sender, RoutedEventArgs e)
        {
            listAssetsHandler.ResetOptions();
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
            if (listAssetsHandler.GoToLogging) tabControl.SelectedIndex = 4; // Jump to log tab
            string cmdLine = " \"" + uiStringProvider.CurrentOWPath + "\" " + ((KeyValuePair<string, string>)Default.TAB2_Array[1]).Value;
            if ((bool)Default.TAB2_Array[2]) cmdLine += " --json";
            logger.Increment(logBox.Dispatcher, "Starting DataTool now. Cmdline: DataTool.exe " + cmdLine);
            StartDataTool(PrepareDataTool(cmdLine));
        }
        #endregion
    }
}