using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static QuickDataTool.Properties.Settings;

namespace QuickDataTool.Logics
{
    public class Config
    {
        public void ConfigInit()
        {
            if (String.IsNullOrWhiteSpace(Default.Path_CurrentOW))
                Default.Path_CurrentOW = "C:\\Program Files (x86)\\Overwatch";
            if (String.IsNullOrWhiteSpace(Default.Path_Output))
                Default.Path_Output = ".\\";
            if (Default.List_OWInsts.Count == 0)
            {
                Default.List_OWInsts = new List<String>
                {
                    "C:\\Program Files (x86)\\Overwatch"
                };
            }
        }

        public Dictionary<string, string> ReadGenericConfig()
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            foreach (SettingsPropertyValue prop in Default.PropertyValues)
            {
                settings.Add(prop.Name, prop.PropertyValue.ToString());
            }
            return settings;
        }

        public List<String> ReadOWInstConfig()
        {
            return Default.List_OWInsts;
        }

        public void AddOWInst(string path)
        {
            if (Default.List_OWInsts.IndexOf(path) == -1)
            {
                Default.List_OWInsts.Add(path);
                Default.Save();
            }
            else
                throw new ArgumentException("Failed to add " + path + " to installation library." +
                    "Path already exists.");

        }

        public void DelOWInst(string path)
        {
            Default.List_OWInsts.Remove(path);
            Default.Save();
        }

        public void SetOWInst(int index)
        {
            Default.Path_CurrentOW = Default.List_OWInsts[index];
        }

        public int CountOWInst()
        {
            return Default.List_OWInsts.Count();
        }
    }
}
