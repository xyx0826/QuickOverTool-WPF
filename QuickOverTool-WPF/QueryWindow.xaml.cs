using System.ComponentModel;
using System.Windows;

namespace QuickOverTool_WPF
{
    /// <summary>
    /// QueryWindow.xaml 的交互逻辑
    /// </summary>
    public partial class QueryWindow : Window
    {

        public QueryWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            windowQuery.Hide();
        }
        // Method for passing the query back to MainWindow
        public string GetQueries()
        {
            return " " + textBoxQuery.Text;
        }
        // Event selector
        private void ComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            string selection = "(event=";
            if (checkBoxExcept.IsChecked == true) selection += "!";
            selection += comboBoxEvent.SelectedItem
                .ToString().Substring(37);
            selection += ")";
            textBoxQuery.Text += selection;
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            windowQuery.Hide();
        }
        // Will replace with more beautiful code later
        private void buttonSkin_Click(object sender, RoutedEventArgs e)
        {
            textBoxQuery.Text += "|skin=";
        }

        private void buttonIcon_Click(object sender, RoutedEventArgs e)
        {
            textBoxQuery.Text += "|icon=";
        }

        private void buttonSpray_Click(object sender, RoutedEventArgs e)
        {
            textBoxQuery.Text += "|spray=";
        }

        private void buttonEmote_Click(object sender, RoutedEventArgs e)
        {
            textBoxQuery.Text += "|emote=";
        }

        private void buttonPose_Click(object sender, RoutedEventArgs e)
        {
            textBoxQuery.Text += "|victorypose=";
        }

        private void buttonIntro_Click(object sender, RoutedEventArgs e)
        {
            textBoxQuery.Text += "|highlightintro=";
        }

        private void butonCommon_Click(object sender, RoutedEventArgs e)
        {
            textBoxQuery.Text += "(rarity=common)";
        }

        private void buttonRare_Click(object sender, RoutedEventArgs e)
        {
            textBoxQuery.Text += "(rarity=rare)";
        }

        private void buttonLegendary_Click(object sender, RoutedEventArgs e)
        {
            textBoxQuery.Text += "(rarity=legedary)";
        }

        private void buttonHelp_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
