using System;
using System.Windows;
using System.Collections.Generic;
using static OWorkbench.Properties.Settings;
using OWorkbench.Logics;
using System.Windows.Controls;
using System.Text;

namespace OWorkbench
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
            List<KeyValuePair<string, string>> listModes = new List<KeyValuePair<string, string>>
            {
                CreateMode("(Please select a mode...)", null),
                CreateMode("List achievements", "list-achievements"),
                CreateMode("List chat replacements", "list-chat-replacements"),
                CreateMode("List general unlocks", "list-general-unlocks"),
                CreateMode("List heroes", "list-heroes"),
                CreateMode("List user highlights", "list-highlights"),
                CreateMode("List encryption keys", "list-keys"),
                CreateMode("List lootboxes", "list-lootbox"),
                CreateMode("List maps", "list-maps"),
                CreateMode("List subtitles", "list-subtitles"),
                CreateMode("List subtitles (from audio data)", "list-subtitles-real"),
                CreateMode("List unlocks", "list-hero-unlocks")
            };
            comboListAssets.ItemsSource = listModes;
            comboListAssets.SelectedValuePath = "Value";
            comboListAssets.DisplayMemberPath = "Key";
            // Populate extraction modes
            List<KeyValuePair<string, string>> extrModes = new List<KeyValuePair<string, string>>
            {
                CreateMode("(Please select a mode...)", null),
                CreateMode("Dump strings", "dump-strings"),
                CreateMode("Extract abilities", "extract-abilities"),
                CreateMode("Extract general", "extract-general"),
                CreateMode("Extract unlocks", "extract-unlocks"),
                CreateMode("Extract hero voice", "extract-hero-voice"),
                CreateMode("Extract lootbox", "extract-lootbox"),
                CreateMode("Extract maps", "extract-maps"),
                CreateMode("Extract map environment data", "extract-map-envs"),
                CreateMode("Extract NPCs", "extract-npcs")
            };
            comboExtrAssets.ItemsSource = extrModes;
            comboExtrAssets.SelectedValuePath = "Value";
            comboExtrAssets.DisplayMemberPath = "Key";
            // Populate query editor types
            List<KeyValuePair<string, string>> queryTypes = new List<KeyValuePair<string, string>>
            {
                CreateMode("Skin", "skin"),
                CreateMode("Icon", "icon"),
                CreateMode("Spray", "spray"),
                CreateMode("Emote", "emote"),
                CreateMode("Voice line", "voiceline"),
                CreateMode("Victory pose", "victory pose"),
                CreateMode("Highlight intro", "highlightintro")
            };
            comboBoxQueryType.ItemsSource = queryTypes;
            comboBoxQueryType.SelectedValuePath = "Value";
            comboBoxQueryType.DisplayMemberPath = "Key";
            // Populate query editor tags
            List<KeyValuePair<string, string>> queryTags = new List<KeyValuePair<string, string>>
            {
                CreateMode("Rarity", "rarity"),
                CreateMode("Event", "event"),
                CreateMode("OWL Team", "leagueTeam")
            };
            comboBoxQueryTag.ItemsSource = queryTags;
            comboBoxQueryTag.SelectedValuePath = "Value";
            comboBoxQueryTag.DisplayMemberPath = "Key";
            // Populate query editor sound modes
            List<KeyValuePair<string, string>> querySoundModes = new List<KeyValuePair<string, string>>
            {
                CreateMode("Sound Restriction", "soundRestriction"),
                CreateMode("Group Restriction", "groupRestriction")
            };
            comboBoxQuerySound.ItemsSource = querySoundModes;
            comboBoxQuerySound.SelectedValuePath = "Value";
            comboBoxQuerySound.DisplayMemberPath = "Key";
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
            ExtrAssetsHandler.GetInstance().UpdateVisibility();
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
            // Once again, refresh OW path to prevent weird config glitches
            Config.GetInstance().UseOWInst(comboOWInsts.SelectedIndex);

            // Start fabricating the command line
            StringBuilder cmdLine = CommandLineFabricator.CreateBaseCommand();
            if (ExtrAssetsHandler.GetInstance().IsTabSelected)
                cmdLine = CommandLineFabricator.AppendExtractFlags(cmdLine);

            cmdLine = CommandLineFabricator.AppendPathAndMode(cmdLine);
            if (ExtrAssetsHandler.GetInstance().IsTabSelected)
                try { cmdLine = CommandLineFabricator.AppendExtractQueries(cmdLine); }
                catch (Exception ex)
                {
                    Logging.GetInstance().Increment(ex.Message);
                    ToastManager.GetInstance().CreateToast("Query Error", "The query supplied for this extraction mode is invalid. See log for details.", 3);
                    return;
                }

            // Launch
            tabControl.SelectedIndex = 5;   // Go to logging tab
            if (!Default.DebugMode)
                Logging.GetInstance().ClearLogs(logBox); // Debug mode will not clear logs

            Logging.GetInstance().Increment(logBox, DateTime.Now.ToString() + " - Starting DataTool now.\nCmdline: DataTool.exe " + cmdLine.ToString());
            StartDataTool(PrepareDataTool(cmdLine.ToString()));
        }
        #endregion
    }
}
