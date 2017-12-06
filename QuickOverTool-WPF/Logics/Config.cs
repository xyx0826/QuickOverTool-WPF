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
            if (Default.List_OWInsts == null)
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
            Default.List_OWInsts.Add(path);
            Default.Save();
        }
    }
}
