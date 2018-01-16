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
        private KeyValuePair<string, string> CreatePair(string text, string value)
        {
            return new KeyValuePair<string, string>(text, value);
        }
        private void PopulateListAssets()
        {
            List<KeyValuePair<string, string>> modes = new List<KeyValuePair<string, string>>();
            modes.Add(CreatePair("List achievements", "list-achievements"));
            modes.Add(CreatePair("List chat replacements", "list-chat-replacements"));
            modes.Add(CreatePair("List general unlocks", "list-general-unlocks"));
            comboListAssets.ItemsSource = modes;
            comboListAssets.SelectedValuePath = "Value";
            comboListAssets.DisplayMemberPath = "Key";
        }
    }
}