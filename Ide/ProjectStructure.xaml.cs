using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Ide
{
    /// <summary>
    /// Interaction logic for ProjectStructure.xaml
    /// </summary>
    public partial class ProjectStructure : UserControl
    {
        public static readonly DependencyProperty SelectedProjectItemProperty =
            DependencyProperty.Register("SelectedProjectItem", typeof(object), typeof(ProjectStructure));

        public static readonly DependencyProperty SelectedProjectValueProperty =
            DependencyProperty.Register("SelectedProjectValue", typeof(object), typeof(ProjectStructure));

        public object SelectedProjectItem
        {
            get { return GetValue(SelectedProjectItemProperty); }
            set { SetValue(SelectedProjectItemProperty, value); }
        }

        public object SelectedProjectValue
        {
            get { return GetValue(SelectedProjectValueProperty); }
            set { SetValue(SelectedProjectValueProperty, value); }
        }


        public ProjectStructure()
        {
            InitializeComponent();
        }


        public delegate void GetSelectedProjectItemChanged(object sender, object projectItem);
        public event GetSelectedProjectItemChanged SelectedProjectItemChanged;

        public delegate void GetSelectedMethodChanged(object sender, ListView methodListItem);
        public event GetSelectedMethodChanged SelectedMethodChanged;

        private void ProjectItemSelected(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedProjectItem = ((TreeView)sender).SelectedItem;
            SelectedProjectValue = ((TreeView)sender).SelectedValue;
            SelectedProjectItemChanged?.Invoke(this, e.NewValue);
        }

        public void MethodSelected(object sender, SelectionChangedEventArgs e)
        {
            SelectedMethodChanged?.Invoke(this, (ListView)sender);
        }
    }
}
