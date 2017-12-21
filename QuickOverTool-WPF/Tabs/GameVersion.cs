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
        Config cfg = new Config();
        VersionManagement vm = new VersionManagement();

        public void AddInst(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            DialogResult folderBrowserResult = folderBrowser.ShowDialog();
            try
            {
                cfg.AddOWInst(folderBrowser.SelectedPath);
            }
            catch (ArgumentException f)
            {
                AddLog(f.Message);
            }
            FlushInst();
        }

        public void DelInst(object sender, RoutedEventArgs e)
        {
            if (cfg.CountOWInst() == 1)
            {
                AddLog("Failed to delete installation record. This is the last one.");
                return;
            }
            try
            {
                string path = comboOWInsts.SelectedItem.ToString();
                cfg.DelOWInst(path);
                FlushInst();
            }
            catch (NullReferenceException)
            {
                AddLog("Failed to remove installation from settings. " +
                    "Please select an installation first.");
            }
        }

        public void SetInst(object sender, RoutedEventArgs e)
        {
            if (comboOWInsts.SelectedIndex != -1)
            {
                cfg.SetOWInst(comboOWInsts.SelectedIndex);
                uistring.Rebind(null);
            }
            else AddLog("Failed to set Overwatch installation. " +
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
                    AddLog(e.Message);
                    key = "(Unknown version)";
                }
                try { versionDict.Add(path, key); }
                catch (ArgumentException)
                {
                    AddLog("Failed to add this installation. Path duplicate.");
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
