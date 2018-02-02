using System.Windows;
using System.ComponentModel;
using System;

namespace QuickDataTool
{
    public partial class MainWindow : Window
    {
        #region Iniialization
        Logging logger = new Logging();
        public void InitializeLogging()
        {
            tabLogging.DataContext = logger;
            logger.Refresh();
            // LogTester();
        }
        #endregion
        // Log testing
        BackgroundWorker logPopulator = new BackgroundWorker();

        public void LogTester()
        {
            logPopulator.DoWork += Spam;
            logPopulator.RunWorkerAsync();
        }

        public void Spam(object sender, DoWorkEventArgs e)
        {
            Random rnd = new Random();
            while (true)
            {
                logger.Increment(logBox.Dispatcher, rnd.Next(10000000, 90000000).ToString() + rnd.Next(10000000, 90000000).ToString());
                System.Threading.Thread.Sleep(20);
            }
        }
    }
}
