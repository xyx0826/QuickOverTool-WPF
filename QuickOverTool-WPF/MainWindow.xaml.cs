﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace QuickOverTool_WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly BackgroundWorker worker = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();
            checkOverToolValidity();
        }

        public bool checkOverwatchValidity(string path)
        {
            if (File.Exists(Path.Combine(path, ".build.info")))
            {
                string overwatchExecutablePath = Path.Combine(path, "Overwatch.exe");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool checkOverToolValidity()
        {
            string overToolLocation = Path.GetDirectoryName
                (Assembly.GetEntryAssembly().CodeBase);
            overToolLocation = Path.Combine(overToolLocation.Substring(6),
                "OverTool.exe");
            if (File.Exists(overToolLocation))
            {
                labelValidity.Content = "检测到了 OverTool";
                labelValidity.Foreground = new SolidColorBrush(Colors.Green);
                return true;
            }
            else
            {
                labelValidity.Content = "未检测到 OverTool";
                labelValidity.Foreground = new SolidColorBrush(Colors.Red);
                return false;
            }
        }

        public void addLog(string content)
        {
            textBoxOutput.AppendText("\n" + content);
        }

        private string whichRadioButton()
        {
            if (radioButtonListGeneralCosmetics.IsChecked == true) return "g";
            else if (radioButtonListHeroCosmetics.IsChecked == true) return "t";
            else if (radioButtonListMaps.IsChecked == true) return "m";
            else if (radioButtonListTextures.IsChecked == true) return "T";
            else if (radioButtonListStrings.IsChecked == true) return "s";
            else if (radioButtonListNPCs.IsChecked == true) return "n";
            else if (radioButtonListLootbox.IsChecked == true) return "I";
            else if (radioButtonListKeys.IsChecked == true) return "Z";
            else if (radioButtonExtractGeneralCosmetics.IsChecked == true) return "G";
            else if (radioButtonExtractHeroCosmetics.IsChecked == true) return "x";
            else if (radioButtonExtractMaps.IsChecked == true) return "M";
            else if (radioButtonExtractVoice.IsChecked == true) return "v";
            else if (radioButtonExtractSoundAllSkins.IsChecked == true) return "V";
            else if (radioButtonExtractWeaponSkins.IsChecked == true) return "w";
            else if (radioButtonExtractNPCs.IsChecked == true) return "N";
            else if (radioButtonExtractLootbox.IsChecked == true) return "L";
            else return "";
        }

        private void buttonPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            DialogResult folderBrowserResult = folderBrowser.ShowDialog();
            if (checkOverwatchValidity(textBoxPath.Text) == true)
            {
                labelValidity.Content = "守望先锋目录有效";
                labelValidity.Foreground = new SolidColorBrush(Colors.Green);
            }
            else
            {
                labelValidity.Content = "守望先锋目录无效";
                labelValidity.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        private void buttonOutputPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            DialogResult folderBrowserResult = folderBrowser.ShowDialog();
            textBoxOutputPath.Text = folderBrowser.SelectedPath;
            addLog("选定了输出路径：" + textBoxOutputPath.Text);
            return;
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            // 判断：是否选择了守望先锋路径
            if (String.IsNullOrEmpty(textBoxPath.Text) ||
                labelValidity.Content.ToString() == "守望先锋目录无效")
            {
                textBoxPath.BorderBrush = new SolidColorBrush(Colors.Red);
                return;
            }
            else if (checkOverwatchValidity(textBoxPath.Text) == true)
            {
                textBoxPath.BorderBrush = new SolidColorBrush(Colors.Blue);
                textBoxPath.IsEnabled = false;
            }
            else return;

            // 构建命令行
            string cmdLine;
            // 命令行：选定语言
            cmdLine = " --language=" + comboBoxLanguage.SelectedItem.
                ToString().Substring(38, 4);
            // 命令行：禁用项
            if (checkBoxSkipAnimation.IsChecked == true) cmdLine = cmdLine + " -A";
            if (checkBoxSkipGUI.IsChecked == true) cmdLine = cmdLine + "-I";
            if (checkBoxSkipKeys.IsChecked == true) cmdLine = cmdLine + "-n";
            if (checkBoxSkipModel.IsChecked == true) cmdLine = cmdLine + "-M";
            if (checkBoxSkipRef.IsChecked == true) cmdLine = cmdLine + "-R";
            if (checkBoxSkipSound.IsChecked == true) cmdLine = cmdLine + "-S";
            if (checkBoxSkipTexture.IsChecked == true) cmdLine = cmdLine + "-T";
            // 命令行：杂项
            if (checkBoxExpert.IsChecked == true) cmdLine = cmdLine + "--ex";
            if (checkBoxCollision.IsChecked == true) cmdLine = cmdLine + "-C";
            // 命令行：守望先锋路径
            cmdLine = cmdLine + " \"" + textBoxPath.Text + "\"";
            // 命令行：模式判断 + 选定模式
            if (whichRadioButton() == "")
            {
                groupBoxModes.BorderBrush = new SolidColorBrush(Colors.Red);
                return;
            }
            else cmdLine = cmdLine + " " + whichRadioButton();
            // 命令行：导出路径
            if (textBoxOutputPath.IsEnabled == true)
            {
                cmdLine = cmdLine + " \"" + textBoxOutputPath.Text + "\"";
            }
            // 命令行：附加参数
            if (!String.IsNullOrEmpty(textBoxSpecify.Text))
                cmdLine = cmdLine + " " + textBoxSpecify.Text;
            addLog("命令行：OverTool.exe" + cmdLine);
            addLog("OverTool 现在启动...");
            // 启动
            using (Process overTool = new Process())
            {
                { // OverTool 进程配置
                    overTool.StartInfo.FileName = "OverTool.exe";
                    overTool.StartInfo.Arguments = cmdLine;
                    overTool.StartInfo.UseShellExecute = false;
                    overTool.StartInfo.RedirectStandardOutput = true;
                    overTool.StartInfo.RedirectStandardError = true;
                    overTool.StartInfo.CreateNoWindow = true;
                }

                overTool.Start();
                overTool.BeginOutputReadLine();
                overTool.BeginErrorReadLine();
            }
        }
    }
}