using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

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
        // Trace to the button that fired the event
        private void buttonTypes_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = 
                (System.Windows.Controls.Button)sender;
            textBoxQuery.Text += ("|" + button.Content.ToString().ToLower() + "=");
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
    }
}
