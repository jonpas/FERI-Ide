using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ide
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void CreateProject(object sender, RoutedEventArgs e)
        {
            //TODO: not use dummy hardcoded content
            TreeViewItem itemProject = new TreeViewItem();
            itemProject.Header = "Project \"Ide\"";
            ProjectTree.Items.Add(itemProject);
            itemProject.ExpandSubtree();

            TreeViewItem item = new TreeViewItem();
            item.Header = "Resources";
            itemProject.Items.Add(item);
            TreeViewItem itemSub = new TreeViewItem();
            itemSub.Header = "picture_folder.png";
            item.Items.Add(itemSub);
            itemSub = new TreeViewItem();
            itemSub.Header = "picture_save.png";
            item.Items.Add(itemSub);

            item = new TreeViewItem();
            item.Header = "App.config";
            itemProject.Items.Add(item);
            item = new TreeViewItem();
            item.Header = "App.xaml";
            itemProject.Items.Add(item);

            item = new TreeViewItem();
            item.Header = "MainWindow.xaml";
            itemProject.Items.Add(item);
            itemSub = new TreeViewItem();
            itemSub.Header = "MainWindow.xaml.cs";
            item.Items.Add(itemSub);

        }

        private void CloseProject(object sender, RoutedEventArgs e)
        {
            //TODO: check if anything is not saved
            MessageBoxResult saveResult = MessageBox.Show("Do you want to save your work before closing the project?", "Close Project Save", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            // Clean up tree view if Cancel was not selected
            if (saveResult != MessageBoxResult.Cancel)
            {
                ProjectTree.Items.Remove(ProjectTree.SelectedItem);
                //TODO: ability to have sub-item selected
            }
        }

        private void ListClassMethods(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // Remove previous items
            MethodList.Items.Clear();

            TreeViewItem selectedItem = e.NewValue as TreeViewItem;
            if (selectedItem == null)
            {
                // Disable delete file button
                DeleteFileButton.IsEnabled = false;
            }
            else
            {
                // Enable delete file button
                DeleteFileButton.IsEnabled = true;

                // Add selected class methods to method list
                ListViewItem item;
                if (selectedItem.Header.ToString().Contains(".cs"))
                {
                    //TODO: not use dummy hardcoded content
                    item = new ListViewItem();
                    item.Content = "void CreateProject()";
                    MethodList.Items.Add(item);

                    item = new ListViewItem();
                    item.Content = "void CloseProject()";
                    MethodList.Items.Add(item);

                    item = new ListViewItem();
                    item.Content = "void ListClassMethods()";
                    MethodList.Items.Add(item);
                }
            }
        }

        private void CreateFile(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = "new_file.txt";
            ProjectTree.Items.Add(item);
        }

        private void DeleteFile(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)ProjectTree.SelectedItem;
            ProjectTree.Items.Remove(ProjectTree.SelectedItem);
            //TODO: ability to remove sub-items
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            if (settings.ShowDialog() == true)
            {
                MessageBox.Show("Settings", "OK", MessageBoxButton.OK);
            }
        }
    }
}
