using System.Windows;
using System.ComponentModel;

namespace QuickDataTool
{
    public partial class MainWindow : Window
    {
        partial void Initialize()
        {
            logBox.DataContext = logger;
            logger.Refresh();
            LogTester();
        }
        // Log testing
        BackgroundWorker logPopulator = new BackgroundWorker();

        public void LogTester()
        {
            logPopulator.DoWork += Spam;
            logPopulator.RunWorkerAsync();
        }

        public void Spam(object sender, DoWorkEventArgs e)
        {
            int i = 0;
            while (true)
            {
                logger.Increment(logBox.Dispatcher, i.ToString());
                i++;
            }
        }
    }
}
