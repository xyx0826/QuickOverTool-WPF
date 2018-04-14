using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using static OWorkbench.Properties.Settings;

namespace OWorkbench.Logics
{
    class DataToolConfig : INotifyPropertyChanged
    {
        #region Singleton implementation
        private static DataToolConfig _uniqueInstance;

        private DataToolConfig()
        {
            OutputPath = ".\\\\";
        }

        public static DataToolConfig GetInstance()
        {
            if (_uniqueInstance == null) _uniqueInstance = new DataToolConfig();
            return _uniqueInstance;
        }
        #endregion

        public int ComboBoxLangIndex
        {
            get { return Default.TAB_SETTINGS_LangIndex; }
            set
            {
                Default.TAB_SETTINGS_LangIndex = value;
                Default.Save();
            }
        }

        public ComboBoxItem ComboBoxLanguage { get; set; }

        public string OutputPath
        {
            get { return Default.TAB_SETTINGS_OutputPath; }
            set
            {
                Default.TAB_SETTINGS_OutputPath = value;
                Default.Save();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
