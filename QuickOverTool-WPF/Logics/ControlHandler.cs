using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QuickDataTool.Properties.Settings;

namespace QuickDataTool.Logics
{
    class ControlHandler : INotifyPropertyChanged
    {
        #region ListAssets controls handler
        private object[] listAssetsOptions = { 0, null, false, false };

        public int ComboBoxIndex
        {
            get { return (int)listAssetsOptions[0]; }
            set { listAssetsOptions[0] = value; }
        }

        public KeyValuePair<string, string> ComboBoxMode
        {
            get { return (KeyValuePair<string, string>)listAssetsOptions[1]; }
            set { listAssetsOptions[1] = value; }
        }
        
        public bool IsJson
        {
            get { return (bool)listAssetsOptions[2]; }
            set { listAssetsOptions[2] = value; }
        }

        public bool GoToLogging
        {
            get { return (bool)listAssetsOptions[3]; }
            set { listAssetsOptions[3] = value; }
        }

        public void ResetOptions()
        {
            object[] template = { 0, null, false, false };
            listAssetsOptions = template;
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
