using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using static OWorkbench.Properties.Settings;

namespace OWorkbench.Logics
{
    class ExtrAssetsHandler : INotifyPropertyChanged
    {
        #region Singleton implementation
        private static ExtrAssetsHandler _uniqueInstance;

        private ExtrAssetsHandler()
        {
            noExtRefpose = true;
            isLosslessTexture = true;
        }

        public static ExtrAssetsHandler GetInstance()
        {
            if (_uniqueInstance == null) _uniqueInstance = new ExtrAssetsHandler();
            return _uniqueInstance;
        }
        #endregion

        #region ExtractAssets controls handler: global options
        // Variables binding to UI controls
        public bool IsTabSelected { get; set; }

        public KeyValuePair<string, string> ComboBoxMode { get; set; }
        public ComboBoxItem comboBoxFormat { get; set; }
        public int modelLOD { get; set; }
        public bool isLosslessTexture { get; set; }
        public bool noExtTextures { get; set; }
        public bool noExtSound { get; set; }
        public bool noExtModels { get; set; }
        public bool noExtAnimation { get; set; }
        public bool noExtRefpose { get; set; }
        public bool noConAnything { get; set; }
        public bool noConTextures { get; set; }
        public bool noConSound { get; set; }
        public bool noConModels { get; set; }
        public bool noConAnimation { get; set; }
        public bool noEnvSound { get; set; }
        public bool noEnvEntity { get; set; }
        public bool noEnvLUT { get; set; }
        public bool noEnvSkybox { get; set; }
        public bool noEnvBlend { get; set; }
        public bool noEnvCubeMap { get; set; }
        public bool noEnvGround { get; set; }
        public bool noEnvSky { get; set; }

        /*
        public int ComboBoxModeIndex    // Adapter: Mode ComboBox index <-> AppSettings
        {
            get { return Default.TAB3_ModeIndex; }
            set { Default.TAB3_ModeIndex = value; }
        }
        */

        public void ResetOptions()  // Resets options - NYI
        {
            OnPropertyChanged(null);
        }
        #endregion
        #region ExtractAssets controls handler: query parameters
        // Visibility binding for 4 query editors
        public Visibility queryGeneralVisb { get; set; } = Visibility.Collapsed;
        public Visibility queryVoiceVisb { get; set; } = Visibility.Collapsed;
        public Visibility queryNpcVisb { get; set; } = Visibility.Collapsed;
        public Visibility queryMapVisb { get; set; } = Visibility.Collapsed;

        public void UpdateVisibility()
        {
            queryGeneralVisb = queryVoiceVisb = queryNpcVisb = queryMapVisb = Visibility.Collapsed;

            switch (ComboBoxMode.Value)
            {
                case "extract-unlocks":
                    queryGeneralVisb = Visibility.Visible;
                    break;
                case "extract-hero-voice":
                    queryVoiceVisb = Visibility.Visible;
                    break;
                case "extract-npcs":
                    queryNpcVisb = Visibility.Visible;
                    break;
                case "extract-maps":
                    queryMapVisb = Visibility.Visible;
                    break;
            }

            OnPropertyChanged(null);
        }

        // Unlockable queries
        public string queryUnlockable_Name { get; set; }
        public string queryUnlockable_Param { get; set; }
        public KeyValuePair<string,string> queryUnlockable_Type { get; set; }
        public KeyValuePair<string,string> queryUnlockable_Tag { get; set; }
        // Voice queries
        public string queryVoice_Name { get; set; }
        public KeyValuePair<string, string> queryVoice_Type { get; set; }
        public string queryVoice_Param { get; set; }
        // NPC queries
        public string queryNpc_Name { get; set; }
        // Map queries
        public string queryMap_Name { get; set; }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
