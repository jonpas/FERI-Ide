using System.Windows;
using System.Windows.Controls;

namespace Ide
{
    /// <summary>
    /// Interaction logic for ProjectStructure.xaml
    /// </summary>
    public partial class ProjectStructure : UserControl
    {
        public ProjectStructure()
        {
            InitializeComponent();
        }

        private void ProjectItemSelected(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
        }

        public void MethodSelected(object sender, SelectionChangedEventArgs e)
        {
         
        }
    }
}
