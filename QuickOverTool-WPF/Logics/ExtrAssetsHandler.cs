using System.Collections.Generic;
using System.ComponentModel;
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

        #region ExtractAssets controls handler
        // Variables binding to UI controls
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
        public bool noEnvGround { get; set; }
        public bool noEnvSky { get; set; }

        public int ComboBoxModeIndex    // Adapter: Mode ComboBox index <-> AppSettings
        {
            get { return Default.TAB3_ModeIndex; }
            set
            {
                Default.TAB3_ModeIndex = value;
            }
        }


        public void ResetOptions()  // Resets options
        {
            // Default.TAB3_Array = new object[] { 0, null, false, true };
            OnPropertyChanged(null);
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
