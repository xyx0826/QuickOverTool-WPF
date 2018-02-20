using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Forms;

namespace QuickOverTool_WPF
{
    public partial class AboutWindow 
    {
        private void buttonPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.ShowDialog();
            utilTextBox.Text = folderBrowser.SelectedPath;
        }

        public void OnUtilRun(object sender, RoutedEventArgs e)
        {
            utilLabelStatus.Visibility = Visibility.Visible;
            utilLabelStatus.Content = "Working...";
            utilButtonGo.IsEnabled = false;
            
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += ProcessDirectory;
            worker.RunWorkerCompleted += (o, args) =>
            {
                utilLabelStatus.Content = "Done";
                utilButtonGo.IsEnabled = true;
            };
            worker.RunWorkerAsync(utilTextBox.Text);
        }

        public void ProcessDirectory(object sender, DoWorkEventArgs e)
        {
            string sharedPath = Path.GetDirectoryName
            (Assembly.GetEntryAssembly().CodeBase).Substring(6);
            string[] files = GetFileList((string)e.Argument);
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
