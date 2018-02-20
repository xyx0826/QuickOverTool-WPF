using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;

namespace QuickOverTool_WPF
{
    public partial class AboutWindow : Window
    {
        private void buttonPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            DialogResult folderBrowserResult = folderBrowser.ShowDialog();
            textBoxUtility.Text = folderBrowser.SelectedPath;
        }

        public void OnUtilRun(object sender, RoutedEventArgs e)
        {
            lblUtilStatus.Visibility = Visibility.Hidden;
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += ProcessDirectory;
            worker.RunWorkerAsync(textBoxUtility.Text);
            lblUtilStatus.Visibility = Visibility.Visible;
        }

        public void ProcessDirectory(object sender, DoWorkEventArgs e)
        {
            string sharedPath = Path.GetDirectoryName
            (Assembly.GetEntryAssembly().CodeBase).Substring(6);
            string[] files = GetFileList((string)(e.Argument));
            List<string> uniqueFiles = new List<string>();
            List<string> hashes = new List<string>();
            Directory.CreateDirectory(sharedPath + "\\Sorted Files");
            foreach (string file in files)
            {
                string hash = HashFile(file);
                if (!hashes.Contains(hash)) uniqueFiles.Add(file);
                hashes.Add(hash);
            }
            foreach (string file in uniqueFiles)
            {
                File.Move(file, sharedPath + "\\Sorted Files\\" + Path.GetFileName(file));
            }
        }

        public string[] GetFileList(string directory)
        {
            return Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
        }

        public string HashFile(string file)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(file))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty);
                }
            }
        }
    }
}
