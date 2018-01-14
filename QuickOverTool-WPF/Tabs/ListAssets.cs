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
        public class ListMode
        {
            public string Name { get; set; }
            public string Value { get; set; }

            public ListMode(string name, string value)
            {
                Name = name;
                Value = value;
            }
        }

        private void PopulateModesList()
        {
            List<ListMode> modes = new List<ListMode>();
            modes.Add(new ListMode("helli", "test"));

        }
    }
}
