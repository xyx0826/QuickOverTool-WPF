using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;

using static OWorkbench.Properties.Settings;

namespace OWorkbench.Logics
{
    class Config
    {
        private static Config _uniqueInstance;

        private Config()
        {

        }

        public static Config GetInstance()
        {
            if (_uniqueInstance == null) _uniqueInstance = new Config();
            return _uniqueInstance;
        }

        /// <summary>
        /// Initialize the configurations on first run.
        /// </summary>
        public void InitConfig()
        {
            if (String.IsNullOrWhiteSpace(Default.Path_CurrentOW))  // Default Overwatch path
            {
                Logging.GetInstance().IncrementDebug("Config: resetting Path_CurrentOW");
                Default.Path_CurrentOW = "C:\\Program Files (x86)\\Overwatch";
            }
            if (String.IsNullOrWhiteSpace(Default.Path_Output)) // Default output path
            {
                Logging.GetInstance().IncrementDebug("Config: resetting Path_Output");
                Default.Path_Output = ".\\";
            }
            if (Default.List_OWInsts == null || Default.List_OWInsts.Count == 0)    // Default Overwatch path in selector
            {
                Logging.GetInstance().IncrementDebug("Config: resetting List_OWInsts");
                Default.List_OWInsts = new List<String>
                {
                    "C:\\Program Files (x86)\\Overwatch"
                };
            }
        }
        #region Overwatch installation management
        public void AddOWInst(string path)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentException("Please select a path to add to installation library.");
            if (Default.List_OWInsts.IndexOf(path) == -1)
            {
                Default.List_OWInsts.Add(path);
                Default.Save();
                Logging.GetInstance().IncrementDebug("Config: added Overwatch installation " + path);
            }
            else
                throw new ArgumentException("Failed to add " + path + " to installation library. " +
                    "Path already exists.");
        }
        
        public void DelOWInst(string path)
        {
            Default.List_OWInsts.Remove(path);
            Logging.GetInstance().IncrementDebug("Config: removed Overwatch installation " + path);
        }

        public void UseOWInst(int index)
        {
            Default.Path_CurrentOW = Default.List_OWInsts[index];
            Default.Save();
            Logging.GetInstance().IncrementDebug("Config: using Overwatch installation #" + index);

            BackgroundWorker sizeWorker = new BackgroundWorker();
            sizeWorker.DoWork += GetOWInstSize;
            sizeWorker.RunWorkerCompleted += GetOWInstSizeCompleted;
            sizeWorker.RunWorkerAsync(Default.Path_CurrentOW);

        }

        public int CountOWInst()
        {
            int count = Default.List_OWInsts.Count();
            Logging.GetInstance().Increment("Config: returned Overwatch installations is " + count);
            return count;
        }

        public void GetOWInstSize(object sender, DoWorkEventArgs e)
        {
            long size = 0;
            double sizeInGb;
            string[] files;
            try
            {
                files = Directory.GetFiles((string)e.Argument, "*", SearchOption.AllDirectories);
            }
            catch (Exception ex)
            {
                return;
            }
            foreach (string file in files) size += new FileInfo(file).Length;
            sizeInGb = size / 1073741824.0;   // 1024 ^ 3
            e.Result = sizeInGb;
        }

        public void GetOWInstSizeCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
                UIString.GetInstance().CurrentOWSize = ((double)e.Result).ToString("0.000");
        }
        #endregion
    }
}
