using System.Windows;
using System.ComponentModel;
using System;
using System.Collections.Generic;

namespace QuickDataTool
{
    public partial class MainWindow : Window
    {
        #region Initialization
        Logging logger = new Logging();
        public void InitializeLogging()
        {
            tabLogging.DataContext = logger;
            logger.Refresh();
        }
        #endregion
        public void SaveLogs(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void CopyLogs(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void ClearLogs(object sender, RoutedEventArgs e)
        {
            logger.ClearLogs(logBox);
        }
    }
}
