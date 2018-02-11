using System.Collections.Generic;
using System.ComponentModel;
using static QuickDataTool.Properties.Settings;

namespace QuickDataTool.Logics
{
    class ControlHandler : INotifyPropertyChanged
    {
        #region ListAssets controls handler
        public int ComboBoxIndex
        {
            get { return (int)Default.TAB2_Array[0]; }
            set
            {
                Default.TAB2_Array[0] = value;
            }
        }

        public KeyValuePair<string, string> ComboBoxMode
        {
            get { return (KeyValuePair<string, string>)Default.TAB2_Array[1]; }
            set { Default.TAB2_Array[1] = value; }
        }
        
        public bool IsJson
        {
            get { return (bool)Default.TAB2_Array[2]; }
            set
            {
                Default.TAB2_Array[2] = value;
                Default.TAB2_Array[2] = value;
            }
        }

        public bool GoToLogging
        {
            get { return (bool)Default.TAB2_Array[3]; }
            set
            {
                Default.TAB2_Array[3] = value;
                Default.TAB2_Array[3] = value;
            }
        }

        public void ResetOptions()
        {
            object[] template = { 0, null, false, false };
            Default.TAB2_Array = template;
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
