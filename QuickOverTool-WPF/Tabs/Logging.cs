using System.Windows;
using System.ComponentModel;
using System;
using System.Collections.Generic;

namespace QuickDataTool
{
    public partial class MainWindow : Window
    {
        #region Initialization
        public void InitializeLogging()
        {
            tabLogging.DataContext = Logging.GetInstance();
            Logging.GetInstance().Refresh();
        }
        #endregion
        public void SaveLogs(object sender, RoutedEventArgs e)
        {
            Logging.GetInstance().SaveLogs();
        }

        public void CopyLogs(object sender, RoutedEventArgs e)
        {
            if (logBox.SelectedItem == null) return;   // Prevent no-selection from crashing the app
            Clipboard.SetText(logBox.SelectedItem.ToString());
        }

        public void ClearLogs(object sender, RoutedEventArgs e)
        {
            Logging.GetInstance().ClearLogs(logBox);
        }
    }
}
