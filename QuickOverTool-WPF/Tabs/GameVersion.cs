using System.Windows;
using System.Collections.Generic;
using System.IO;
using static QuickDataTool.Properties.Settings;
using System;
using QuickDataTool.Logics;
using System.Windows.Forms;

namespace QuickDataTool
{
    public partial class MainWindow : Window
    {
        Config configProvider = new Config();
        VersionManagement vm = new VersionManagement();

        public void AddInst(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            DialogResult folderBrowserResult = folderBrowser.ShowDialog();
            try
            {
                configProvider.AddOWInst(folderBrowser.SelectedPath);
            }
            catch (ArgumentException f)
            {
                uiStringProvider.SetNotif(lblNotif.Dispatcher, f.Message);
            }
            FlushInst();
        }

        public void DelInst(object sender, RoutedEventArgs e)
        {
            if (configProvider.CountOWInst() == 1)
            {
                uiStringProvider.SetNotif(lblNotif.Dispatcher, "Failed to delete installation record. This is the last one.");
                return;
            }
            try
            {
                string path = comboOWInsts.SelectedItem.ToString();
                configProvider.DelOWInst(path);
                FlushInst();
            }
            catch (NullReferenceException)
            {
                uiStringProvider.SetNotif(lblNotif.Dispatcher, "Failed to remove installation from settings. " +
                    "Please select an installation first.");
            }
        }

        public void SetInst(object sender, RoutedEventArgs e)
        {
            if (comboOWInsts.SelectedIndex != -1)
            {
                configProvider.UseOWInst(comboOWInsts.SelectedIndex);
                uiStringProvider.Rebind(null);
            }
            else uiStringProvider.SetNotif(lblNotif.Dispatcher, "Failed to set Overwatch installation. " +
                    "Please select an installation first.");
        }

        public void FlushInst()
        {
            Dictionary<String, String> versionDict = new Dictionary<string, string>();
            foreach (string path in Default.List_OWInsts) // Reads insts from user config
            {
                string key;
                try { key = vm.GetOWVersion(path); }
                catch (FileNotFoundException e)
                {
                    uiStringProvider.SetNotif(lblNotif.Dispatcher, e.Message);
                    key = "(Unknown version)";
                }
                try { versionDict.Add(path, key); }
                catch (ArgumentException)
                {
                    uiStringProvider.SetNotif(lblNotif.Dispatcher, "Failed to add this installation. Path duplicate.");
                    continue;
                }
            }
            // Repopulate ComboBox
            comboOWInsts.Items.Clear();
            foreach (KeyValuePair<string, string> inst in versionDict)
            {
                comboOWInsts.Items.Add(inst.Key);
                comboOWInsts.SelectedIndex = 0;
            }
        }
    }
}
