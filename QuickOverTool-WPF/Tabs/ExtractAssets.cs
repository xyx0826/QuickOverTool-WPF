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
        ExtrAssetsHandler ExtrAssetsHandler = new ExtrAssetsHandler();
        public void InitializeExtrAssets()
        {
            tabExtrAssets.DataContext = ExtrAssetsHandler;
            PopulateExtrAssets();
        }
        #endregion
        #region Populate modes
        private void PopulateExtrAssets()
        {
            List<KeyValuePair<string, string>> modes = new List<KeyValuePair<string, string>>();
            modes.Add(CreateExtrMode("(Please select a mode...)", null));
            modes.Add(CreateExtrMode("List achievements", "list-achievements"));
            modes.Add(CreateExtrMode("List chat replacements", "list-chat-replacements"));
            modes.Add(CreateExtrMode("List general unlocks", "list-general-unlocks"));
            modes.Add(CreateExtrMode("List heroes", "list-heroes"));
            modes.Add(CreateExtrMode("List user highlights", "list-highlights"));
            modes.Add(CreateExtrMode("List encryption keys", "list-keys"));
            modes.Add(CreateExtrMode("List lootboxes", "list-lootbox"));
            modes.Add(CreateExtrMode("List maps", "list-maps"));
            modes.Add(CreateExtrMode("List subtitles", "list-subtitles"));
            modes.Add(CreateExtrMode("List subtitles (from audio data)", "list-subtitles-real"));
            modes.Add(CreateExtrMode("List unlocks", "list-hero-unlocks"));
            comboExtrAssets.ItemsSource = modes;
            comboExtrAssets.SelectedValuePath = "Value";
            comboExtrAssets.DisplayMemberPath = "Key";
        }

        private static KeyValuePair<string, string> CreateExtrMode(string displayName, string command)
        {
            return new KeyValuePair<string, string>(displayName, command);
        }
        #endregion
    }
}
