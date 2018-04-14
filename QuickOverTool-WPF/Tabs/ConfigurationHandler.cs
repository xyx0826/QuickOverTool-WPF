using System;
using System.Windows;
using System.Collections.Generic;
using static QuickDataTool.Properties.Settings;
using QuickDataTool.Logics;
using System.Windows.Controls;
using System.Windows.Forms;

namespace QuickDataTool
{
    public partial class MainWindow : Window
    {
        // 选定输出路径
        private void buttonOutputPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            DialogResult folderBrowserResult = folderBrowser.ShowDialog();
            textBoxOutputPath.Text = (String.IsNullOrEmpty(folderBrowser.SelectedPath) ? textBoxOutputPath.Text : folderBrowser.SelectedPath);
            Logging.GetInstance().Increment("Output path: " + textBoxOutputPath.Text);
            return;
        }
    }
}
