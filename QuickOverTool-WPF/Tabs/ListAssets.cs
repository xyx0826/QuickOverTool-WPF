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
            modes.Add(CreatePair("List heroes", "list-heroes"));
            modes.Add(CreatePair("List user highlights", "list-highlights"));
            modes.Add(CreatePair("List encryption keys", "list-keys"));
            modes.Add(CreatePair("List lootboxes", "list-lootbox"));
            modes.Add(CreatePair("List maps", "list-maps"));
            modes.Add(CreatePair("List subtitles", "list-subtitles"));
            modes.Add(CreatePair("List subtitles (from audio data)", "list-subtitles-real"));
            modes.Add(CreatePair("List unlocks", "list-hero-unlocks"));
            comboListAssets.ItemsSource = modes;
            comboListAssets.SelectedValuePath = "Value";
            comboListAssets.DisplayMemberPath = "Key";
        }
    }
}