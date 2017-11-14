using System.Windows;
using System.Windows.Data;

namespace QuickOverTool_WPF
{
    /// <summary>
    /// AboutWindow.xaml logics
    /// </summary>
    public partial class AboutWindow : Window
    {
        public string Text
        {
            get
            {
                return "QuickDataTool is made by xyx0826.\n" +
                    "This project can be found at:\n\n" +
                    "https://github.com/xyx0826/QuickOverTool-WPF/tree/datatool. \n\n" +
                    "Thanks dynaomi, zingballyhoo, SombraOW and Js41637 for making OverTool toolchain, the ultimate toolset for extracting Overwatch game assets.\n\n" +
                    "QuickDataTool is my experimental project. It is my first WPF program.\n\n" +
                    "The GUI for OverTool (Overwatch pre-1.14) is called QuickOverTool, it can be found under the master branch in the GitHub repo.\n\n" +
                    "If you need a GUI for OverTool-ing pre-1.14 game, please consider checking out Yernemm's OverTool GUI. They did an awesome job at writing a handy interface that even comes with file conversion.\n\n" +
                    "You can find Yernemm's GUI at:\n\n" +
                    "https://yernemm.xyz/projects/OverToolGUI.";

            }
            set { }
        }

        public AboutWindow()
        {
            InitializeComponent();
        }
        
        // Todo: add data through binding, instead of XAML
    }
}
