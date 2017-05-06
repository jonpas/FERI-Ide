using System.Windows;

namespace Ide
{
    /// <summary>
    /// Interaction logic for InputWindow.xaml
    /// </summary>
    public partial class InputWindow : Window
    {
        public InputWindow()
        {
            InitializeComponent();
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
