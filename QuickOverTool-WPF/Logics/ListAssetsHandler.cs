using System.Collections.Generic;
using System.ComponentModel;
using static OWorkbench.Properties.Settings;

namespace OWorkbench.Logics
{
    class ListAssetsHandler : INotifyPropertyChanged
    {
        private static ListAssetsHandler _uniqueInstance;
        
        public static ListAssetsHandler GetInstance()
        {
            if (_uniqueInstance == null) _uniqueInstance = new ListAssetsHandler();
            return _uniqueInstance;
        }

        #region ListAssets controls handler
        public bool IsTabSelected { get; set; }

        public KeyValuePair<string, string> comboBoxMode;

        public int ComboBoxIndex    // Binded ComboBox index
        {
            get { return Default.TAB_LIST_ModeIndex; }
            set
            {
                Default.TAB_LIST_ModeIndex = value;
            }
        }

        public KeyValuePair<string, string> ComboBoxMode    // This KeyValuePair is temporary and is reset every app launch
        {
            get { return comboBoxMode; }
            set { comboBoxMode = value; }
        }
        
        public bool IsJson  // Binded JSON checkbox
        {
            get { return Default.TAB_LIST_OutputJSON; }
            set
            {
                Default.TAB_LIST_OutputJSON = value;
            }
        }

        public void ResetOptions()
        {
            Default.TAB_LIST_ModeIndex = 0;
            Default.TAB_LIST_OutputJSON = true;
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
