using RamPressureAnalyzer.ViewModels;
using RamPressureAnalyzer;
using System.Windows;
using System.Windows.Controls;

namespace RamPressureAnalyzer.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}