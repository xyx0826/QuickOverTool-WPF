using System;
using System.Windows;
using System.Collections.Generic;
using static OWorkbench.Properties.Settings;
using OWorkbench.Logics;
using System.Windows.Controls;

namespace OWorkbench
{
    public partial class MainWindow : Window
    {
        #region Initialization
        QueryWindow _queryWindow;
        public void InitializeDataToolHandling()
        {
            tabListAssets.DataContext = ListAssetsHandler.GetInstance();
            tabExtrAssets.DataContext = ExtrAssetsHandler.GetInstance();
            _queryWindow = new QueryWindow();
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
                groupBoxMapOptions.Visibility = Visibility.Visible;
            else groupBoxMapOptions.Visibility = Visibility.Collapsed;
            if (ExtrAssetsHandler.GetInstance().ComboBoxMode.Value == "extract-hero-voice" ||
                ExtrAssetsHandler.GetInstance().ComboBoxMode.Value == "extract-maps" ||
                ExtrAssetsHandler.GetInstance().ComboBoxMode.Value == "extract-map-envs" ||
                ExtrAssetsHandler.GetInstance().ComboBoxMode.Value == "extract-npcs" ||
                ExtrAssetsHandler.GetInstance().ComboBoxMode.Value == "extract-unlocks")
                buttonExtractQuery.Visibility = Visibility.Visible;
            else buttonExtractQuery.Visibility = Visibility.Collapsed;
        }

        private void buttonExtractQuery_Click(object sender, RoutedEventArgs e)
        {
            _queryWindow.Show();
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
            string cmdLine = "";

            // List assets
            if ((((Button)sender).DataContext) == ListAssetsHandler.GetInstance()) // If the button belongs to list mode
            {
                if (ListAssetsHandler.GetInstance().ComboBoxMode.Value == null)
                {
                    UIString.GetInstance().SetNotif(lblNotif.Dispatcher, "ERROR: Please choose a list mode.");
                    return;
                }
                // Cmdline fabrication
                cmdLine += "--language=" + DataToolConfig.GetInstance().ComboBoxLanguage.Content + " ";   // Language
                cmdLine += " \"" + UIString.GetInstance().CurrentOWPath + "\" "
                    + ListAssetsHandler.GetInstance().ComboBoxMode.Value;
            }
            // Extract Assets
            if ((((Button)sender).DataContext) == ExtrAssetsHandler.GetInstance()) // If the button belongs to extr mode
            {
                if (ExtrAssetsHandler.GetInstance().ComboBoxMode.Value == null)
                {
                    UIString.GetInstance().SetNotif(lblNotif.Dispatcher, "ERROR: Please choose a list mode.");
                    return;
                }
                ExtrAssetsHandler handler = ExtrAssetsHandler.GetInstance();
                // Cmdline fabrication
                cmdLine += "--language=" + DataToolConfig.GetInstance().ComboBoxLanguage.Content + " ";   // Language
                
                if (!handler.noExtTextures) // if texture will be extracted
                {
                    if (!handler.noConTextures) // if texture will be converted
                    {
                        if (!handler.isLosslessTexture) cmdLine += "--convert-lossless-textures=false ";    // texture lossless
                        cmdLine += "--convert-textures-type=" + handler.comboBoxFormat.Content + " ";   // texture format
                    }
                    else cmdLine += "--convert-textures=false ";
                }
                else cmdLine += "--skip-textures=true ";

                if (!handler.noExtSound)    // if sound will be extracted
                {
                    if (handler.noConSound) cmdLine += "--convert-sound=false ";
                }
                else cmdLine += "--skip-sound=true ";

                if (!handler.noExtModels)   // if models will be extracted
                {
                    cmdLine += "--lod=" + handler.modelLOD + " ";   // model LOD
                    if (handler.noConModels) cmdLine += "--convert-models=false ";
                }
                else cmdLine += "--skip-models=true ";

                if (!handler.noExtAnimation)   // if models will be extracted
                {
                    if (handler.noConAnimation) cmdLine += "--convert-animations=false ";
                }
                else cmdLine += "--skip-animations=true ";

                if (!handler.noExtRefpose)  // if refposes will be extracted
                    cmdLine += "--extract-refpose=true ";

                if (handler.noConAnything) cmdLine += "--raw "; // convert nothing

                if (handler.ComboBoxMode.Value == "extract-map-envs")   // if map env extraction is active
                {
                    if (handler.noEnvSound) cmdLine += "--skip-map-env-sound=true ";
                    if (handler.noEnvLUT) cmdLine += "--skip-map-env-lut=true ";
                    if (handler.noEnvBlend) cmdLine += "--skip-map-env-blend=true ";
                    if (handler.noEnvGround) cmdLine += "--skip-map-env-ground=true ";
                    if (handler.noEnvSky) cmdLine += "--skip-map-env-sky=true ";
                    if (handler.noEnvSkybox) cmdLine += "--skip-map-env-skybox=true ";
                    if (handler.noEnvEntity) cmdLine += "--skip-map-env-entity=true ";
                }
                
                cmdLine += " \"" + UIString.GetInstance().CurrentOWPath + "\" "  // Game path and mode
                    + handler.ComboBoxMode.Value + " ";
                cmdLine += "\"" + Default.TAB_SETTINGS_OutputPath + "\" ";
            }
            
            if (Default.TAB_LIST_OutputJSON) cmdLine += " --json";
            if (buttonExtractQuery.IsVisible) cmdLine += _queryWindow.GetQueries();

            tabControl.SelectedIndex = 5;
            if (!Default.DebugMode) Logging.GetInstance().ClearLogs(logBox); // Debug mode will not clear logs
            Logging.GetInstance().Increment(logBox, DateTime.Now.ToString() + " - Starting DataTool now.\nCmdline: DataTool.exe " + cmdLine);
            StartDataTool(PrepareDataTool(cmdLine));
        }
        #endregion
    }
}