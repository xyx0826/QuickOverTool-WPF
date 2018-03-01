using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using static QuickDataTool.Properties.Settings;

namespace QuickDataTool.Logics
{
    public class Config
    {
        /// <summary>
        /// Initialize the configurations on first run.
        /// </summary>
        public void InitConfig()
        {
            if (String.IsNullOrWhiteSpace(Default.Path_CurrentOW))  // Default Overwatch path
                Default.Path_CurrentOW = "C:\\Program Files (x86)\\Overwatch";
            if (String.IsNullOrWhiteSpace(Default.Path_Output)) // Default output path
                Default.Path_Output = ".\\";
            if (Default.List_OWInsts.Count == 0)    // Default Overwatch path in selector
            {
                Default.List_OWInsts = new List<String>
                {
                    "C:\\Program Files (x86)\\Overwatch"
                };
            }
            if (Default.TAB2_Array == null) // Default ListAssets parameters
                Default.TAB2_Array = new object[] { 0, null, false, true };
            if (Default.TAB3_Array == null) // Default ExtrAssets parameters
                Default.TAB3_Array = new object[] { 0, null, false, true };
        }
        #region Overwatch installation management
        public void AddOWInst(string path)
        {
            if (Default.List_OWInsts.IndexOf(path) == -1)
            {
                Default.List_OWInsts.Add(path);
                Default.Save();
            }
            else
                throw new ArgumentException("Failed to add " + path + " to installation library. " +
                    "Path already exists.");
        }
        
        public void DelOWInst(string path)
        {
            Default.List_OWInsts.Remove(path);
        }

        public void UseOWInst(int index)
        {
            Default.Path_CurrentOW = Default.List_OWInsts[index];
        }

        public int CountOWInst()
        {
            return Default.List_OWInsts.Count();
        }
        #endregion
    }
}
