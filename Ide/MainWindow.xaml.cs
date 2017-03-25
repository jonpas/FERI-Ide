using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Xml.Serialization;

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

            // Load cache
            XmlSerializer serializer = new XmlSerializer(typeof(Cache));
            if (File.Exists("Cache.xml"))
            {
                using (TextReader reader = new StreamReader(@"Cache.xml"))
                {
                    try
                    {
                        Cache cache = (Cache)serializer.Deserialize(reader);
                        TextEditor.TextWrapping = cache.WordWrap; //TODO: Change the box check
                    }
                    catch
                    {
                        Console.WriteLine("Error! Cache failed deserializing!");
                    }
                }
            }
        }

        private void ToggleButtonAvailability(Control sender)
        {
            sender.IsEnabled = !sender.IsEnabled;
            sender.Opacity = sender.IsEnabled ? 1 : 0.25;
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private TreeViewItem CreateProjectItem(TreeViewItem parent, string path, bool folder)
        {
            StackPanel holder = new StackPanel();
            holder.Orientation = Orientation.Horizontal;

            Image img = new Image();
            Uri imgUri;
            if (folder)
            {
                imgUri = new Uri(@"Resources\FileTypes\Folder_16x.png", UriKind.Relative);
            }
            else
            {
                switch (path)
                {
                    case string f when f.EndsWith(".cs"):
                        imgUri = new Uri(@"Resources\FileTypes\CS_16x.png", UriKind.Relative);
                        break;
                    case string f when f.EndsWith(".xaml"):
                        imgUri = new Uri(@"Resources\FileTypes\XMLFile_16x.png", UriKind.Relative);
                        break;
                    case string f when f.EndsWith(".config"):
                        imgUri = new Uri(@"Resources\FileTypes\ConfigurationFile_16x.png", UriKind.Relative);
                        break;
                    case string f when f.EndsWith(".png"):
                        imgUri = new Uri(@"Resources\FileTypes\Image_16x.png", UriKind.Relative);
                        break;
                    default:
                        imgUri = new Uri(@"Resources\FileTypes\Document_16x.png", UriKind.Relative);
                        break;
                }
            }

            img.Source = new BitmapImage(imgUri);
            holder.Children.Add(img);
            TextBlock fileName = new TextBlock() { Text = File.Exists(path) || Directory.Exists(path) ? System.IO.Path.GetFileName(path) : path, Margin = new Thickness(5, 0, 0, 0) };
            holder.Children.Add(fileName);

            TreeViewItem item = new TreeViewItem();
            item.Header = holder;
            item.FontWeight = FontWeights.Regular; // Inherits 'Bold' from parent
            parent.Items.Add(item);

            return item;
        }

        private void CreateProjectTree(TreeViewItem parent, string parentDir)
        {
            foreach (var dir in Directory.GetDirectories(parentDir))
            {
                TreeViewItem dirItem = CreateProjectItem(parent, dir, true);
                CreateProjectTree(dirItem, dir);
            }

            foreach (var file in Directory.GetFiles(parentDir))
            {
                CreateProjectItem(parent, file, false);
            }
        }

        private void CreateProject(object sender, RoutedEventArgs e)
        {
            StackPanel holder = new StackPanel();
            holder.Orientation = Orientation.Horizontal;

            Image img = new Image() { Source = new BitmapImage(new Uri(@"Resources\FileTypes\ProjectFolderOpen_16x.png", UriKind.Relative)) };
            holder.Children.Add(img);
            TextBlock fileName = new TextBlock() { Text = "Project '" + new DirectoryInfo(@"..\..").Name + "'", Margin = new Thickness(5, 0, 0, 0) };
            holder.Children.Add(fileName);

            TreeViewItem projectItem = new TreeViewItem();
            projectItem.Header = holder;
            projectItem.FontWeight = FontWeights.Bold;
            ProjectTree.Items.Add(projectItem);
            projectItem.ExpandSubtree();

            CreateProjectTree(projectItem, @"..\..\");

            ToggleButtonAvailability(CreateProjectMenuItem);
            ToggleButtonAvailability(CloseProjectMenuItem);
        }

        private void CloseProject(object sender, RoutedEventArgs e)
        {
            //TODO: check if anything is not saved
            MessageBoxResult saveResult = MessageBox.Show("Do you want to save your work before closing the project?", "Close Project Save", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            // Clean up tree view if Cancel was not selected
            if (saveResult != MessageBoxResult.Cancel)
            {
                ProjectTree.Items.Clear();
            }

            ToggleButtonAvailability(CreateProjectMenuItem);
            ToggleButtonAvailability(CloseProjectMenuItem);

            TabItem tab = (TabItem)TextEditor.Parent;
            tab.Content = "";
            tab.Header = "No File";
            tab.FontStyle = FontStyles.Italic;
        }

        private void CreateFile(object sender, RoutedEventArgs e)
        {
            if (ProjectTree.HasItems)
            {
                CreateProjectItem((TreeViewItem)ProjectTree.Items.GetItemAt(0), "Untitled.txt", false);
                TextEditor.Text = "/* Default text */";
            }
            else
            {
                MessageBox.Show("Create or open a project to add files.", "No project open!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteFile(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectedItem = (TreeViewItem)ProjectTree.SelectedItem;
            TreeViewItem parentItem = (TreeViewItem)selectedItem.Parent;
            parentItem.Items.Remove(selectedItem);

            TabItem tab = (TabItem)TextEditor.Parent;
            tab.Header = "";
            tab.Header = "No File";
            tab.FontStyle = FontStyles.Italic;
        }

        private void CreateMethodItem(string type, string content, string filePath)
        {
            StackPanel holder = new StackPanel();
            holder.Orientation = Orientation.Horizontal;

            Uri imgUri;
            switch (type)
            {
                case "private":
                    imgUri = new Uri(@"Resources\FileTypes\MethodPrivate_16x.png", UriKind.Relative);
                    break;
                case "protected":
                    imgUri = new Uri(@"Resources\FileTypes\MethodProtect_16x.png", UriKind.Relative);
                    break;
                case "public":
                    imgUri = new Uri(@"Resources\FileTypes\Method_purple_16x.png", UriKind.Relative);
                    break;
                default:
                    imgUri = new Uri(@"Resources\FileTypes\Document_16x.png", UriKind.Relative);
                    break;
            }

            Image img = new Image() { Source = new BitmapImage(imgUri) };
            holder.Children.Add(img);
            TextBlock text = new TextBlock() { Text = content, Margin = new Thickness(5, 0, 0, 0) };
            holder.Children.Add(text);

            ListViewItem item = new ListViewItem();
            item.Content = holder;
            item.Tag = filePath;
            MethodList.Items.Add(item);
        }

        private void ProjectItemSelected(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // Remove previous items
            MethodList.Items.Clear();

            TreeViewItem selectedItem = (TreeViewItem)e.NewValue;
            StackPanel selectedItemHolder = (StackPanel)selectedItem.Header;
            TextBlock selectedItemText = (TextBlock)selectedItemHolder.Children[1];

            // Only disable button if null or root item selected
            if (selectedItem == null || selectedItem == (TreeViewItem)ProjectTree.Items.GetItemAt(0))
            {
                if (DeleteFileButton.IsEnabled)
                    ToggleButtonAvailability(DeleteFileButton);
            }
            else
            {
                if (!DeleteFileButton.IsEnabled)
                    ToggleButtonAvailability(DeleteFileButton);

                TabItem tab = (TabItem)TextEditor.Parent;
                Regex regex = new Regex(@"(private|protected|public) (.+?)\)");

                foreach (var filePath in Directory.GetFiles(@"..\..\", selectedItemText.Text, SearchOption.AllDirectories))
                {
                    // Show file contents
                    TextEditor.Text = File.ReadAllText(filePath);
                    tab.Header = System.IO.Path.GetFileName(filePath);
                    tab.FontStyle = FontStyles.Normal;

                    // Add selected class methods to method list
                    if (selectedItemText.Text.Contains(".cs"))
                        foreach (var line in File.ReadAllLines(filePath))
                            foreach (Match match in regex.Matches(line))
                                if (match.Groups[2] != null)
                                    CreateMethodItem(match.Groups[1].Value, match.Groups[2].Value + ")", filePath);
                }
            }
        }
        private void ProjectTreeFolderExpanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectedItem = (TreeViewItem)e.OriginalSource;
            StackPanel selectedItemHolder = (StackPanel)selectedItem.Header;
            Image selectedItemImage = (Image)selectedItemHolder.Children[0];
            string selectedItemImageSource = selectedItemImage.Source.ToString();

            if (selectedItemImageSource.Contains("Folder") && selectedItem != (TreeViewItem)ProjectTree.Items.GetItemAt(0))
                selectedItemImage.Source = new BitmapImage(new Uri(@"Resources\FileTypes\FolderOpen_16x.png", UriKind.Relative));
        }

        private void ProjectTreeFolderCollapsed(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectedItem = (TreeViewItem)e.OriginalSource;
            StackPanel selectedItemHolder = (StackPanel)selectedItem.Header;
            Image selectedItemImage = (Image)selectedItemHolder.Children[0];
            string selectedItemImageSource = selectedItemImage.Source.ToString();

            if (selectedItemImageSource.Contains("Folder") && selectedItem != (TreeViewItem)ProjectTree.Items.GetItemAt(0))
                selectedItemImage.Source = new BitmapImage(new Uri(@"Resources\FileTypes\Folder_16x.png", UriKind.Relative));
        }

        private void MethodSelected(object sender, SelectionChangedEventArgs e)
        {
            ListView methodList = (ListView)sender;
            ListViewItem selectedItem = (ListViewItem)methodList.SelectedItem;

            if (selectedItem != null)
            {
                StackPanel selectedItemHolder = (StackPanel)selectedItem.Content;
                TextBlock selectedItemText = (TextBlock)selectedItemHolder.Children[1];
                string method = selectedItemText.Text;
                string filePath = (string)selectedItem.Tag;
                TabItem tab = (TabItem)TextEditor.Parent;

                Regex regex = new Regex(method.Replace("(", @"\(").Replace(")", @"\)") + @"[^{]*.\n([^}]*)}");
                MatchCollection matches = regex.Matches(File.ReadAllText(filePath));

                if (matches != null && matches.Count > 0)
                {
                    TextEditor.Text = matches[0].Groups[1].Value.Replace("            ", "");

                    Regex regexName = new Regex(@"(\S*)\(");
                    MatchCollection matchesName = regexName.Matches(method);
                    if (matchesName != null && matchesName.Count > 0)
                        tab.Header = matchesName[0].Groups[1] + "()";
                }
                else
                {
                    TextEditor.Text = "Nothing";
                    tab.Header = "No File";
                }
                tab.FontStyle = FontStyles.Italic;
            }
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            SettingsWindow settings = new SettingsWindow();
            if (settings.ShowDialog() == true)
                MessageBox.Show("Settings", "OK", MessageBoxButton.OK);
        }

        private void ToggleWordWrap(object sender, RoutedEventArgs e)
        {
            if (TextEditor.TextWrapping == TextWrapping.Wrap)
                TextEditor.TextWrapping = TextWrapping.NoWrap;
            else
                TextEditor.TextWrapping = TextWrapping.Wrap;
        }

        private void SaveCache(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Save to cache
            XmlSerializer serializer = new XmlSerializer(typeof(Cache));
            using (TextWriter writer = new StreamWriter(@"Cache.xml"))
            {
                Cache cache = new Cache(TextEditor.TextWrapping);
                serializer.Serialize(writer, cache);
            }
        }
    }
}
