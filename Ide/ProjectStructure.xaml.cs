using System.Windows;
using System.Windows.Controls;

namespace Ide
{
    /// <summary>
    /// Interaction logic for ProjectStructure.xaml
    /// </summary>
    public partial class ProjectStructure : UserControl
    {
        public delegate void GetProjectItemSelected(object sender, object projectItem);
        public event GetProjectItemSelected OnProjectItemSelected;

        public delegate void GetMethodSelected(object sender, ListView methodListItem);
        public event GetMethodSelected OnMethodSelected;

        public ProjectStructure()
        {
            InitializeComponent();
        }

        private void ProjectItemSelected(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            OnProjectItemSelected?.Invoke(this, e.NewValue);
        }

        public void MethodSelected(object sender, SelectionChangedEventArgs e)
        {
            OnMethodSelected?.Invoke(this, (ListView)sender);
        }
    }
}
