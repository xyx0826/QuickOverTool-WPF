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
            // LogTester();
        }
        #endregion
        // Log testing
        BackgroundWorker logPopulator = new BackgroundWorker();
        BackgroundWorker logGenerator = new BackgroundWorker();
        List<string> mainList = new List<string>();

        public void LogTester()
        {
            logGenerator.DoWork += Spam;
            logGenerator.RunWorkerAsync();
            logPopulator.DoWork += Pile;
            logPopulator.RunWorkerAsync();
        }

        public void Spam(object sender, DoWorkEventArgs e)
        {
            
            Random rnd = new Random();
            for(int i = 0; i < 10000; i ++)
            {
                mainList.Add(i + ". " + rnd.Next(10000000, 90000000).ToString() + rnd.Next(10000000, 90000000).ToString());
            }
        }

        public void Pile(object sender, DoWorkEventArgs e)
        {
            while(true)
            {
                try
                {
                    logger.Increment(logBox.Dispatcher, mainList[0]);
                    mainList.RemoveAt(0);
                    System.Threading.Thread.Sleep(5);
                }
                catch { }
            }
        }
    }
}
