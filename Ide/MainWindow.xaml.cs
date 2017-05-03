using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        ObservableCollection<Project> Projects = new ObservableCollection<Project>();
        ObservableCollection<Method> Methods = new ObservableCollection<Method>();

        public MainWindow()
        {
            InitializeComponent();

            // Create default projects directory if it doesn't exist yet
            string projectDir = Properties.Settings.Default.ProjectsDirectory;
            if (projectDir == "")
                Properties.Settings.Default.ProjectsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Ide Projects";

            if (!Directory.Exists(Properties.Settings.Default.ProjectsDirectory))
                Directory.CreateDirectory(Properties.Settings.Default.ProjectsDirectory);

            // Load settings
            TextEditor.TextWrapping = (TextWrapping)Properties.Settings.Default.TextWrap;

            // Test
            Projects.Add(new Project(Properties.Settings.Default.ProjectsDirectory, "C#", "Code", "WPF"));

            //TODO INotifyPropertyChanged (?)
            ProjectTree.ItemsSource = Projects;
            MethodList.ItemsSource = Methods;


            // Load cache
            //XmlSerializer serializer = new XmlSerializer(typeof(Cache));
            //if (File.Exists("Cache.xml"))
            //{
            //    using (TextReader reader = new StreamReader("Cache.xml"))
            //    {
            //        try
            //        {
            //            Cache cache = (Cache)serializer.Deserialize(reader);
            //            TextEditor.TextWrapping = cache.WordWrap;
            //        }
            //        catch
            //        {
            //            Console.WriteLine("Error! Cache failed deserializing!");
            //        }
            //    }
            //}
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private TreeViewItem CreateProjectItem(TreeViewItem parent, string path, bool folder)
        {
            return null;
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
            CreateProjectWindow newProject = new CreateProjectWindow();

            if (newProject.ShowDialog() == true)
            {
                StackPanel holder = new StackPanel();
                holder.Orientation = Orientation.Horizontal;

                Image img = new Image() { Source = new BitmapImage(new Uri("Resources/FileTypes/ProjectFolderOpen_16x.png", UriKind.Relative)) };
                holder.Children.Add(img);
                TextBlock fileName = new TextBlock() { Text = "Project '" + newProject.ProjectName.Text + "'", Margin = new Thickness(5, 0, 0, 0) };
                holder.Children.Add(fileName);

                TreeViewItem projectItem = new TreeViewItem();
                projectItem.Tag = "Project";
                projectItem.Header = holder;
                projectItem.FontWeight = FontWeights.Bold;
                ProjectTree.Items.Add(projectItem);
                projectItem.ExpandSubtree();

                CreateProjectTree(projectItem, newProject.LocationText.Text + "/..");

                //TODO: Save project path for later use
            }
        }

        private void RenameFileFolderProject(object sender, RoutedEventArgs e)
        {
            InputWindow input = new InputWindow();

            if (input.ShowDialog() == true)
            {
                string newName = input.Input.Text;

                //TODO: Check if folder of same name already exists
                if (newName != "")
                {
                    if (ProjectTree.SelectedItem.GetType() == typeof(FileItem))
                    {
                        FileItem selectedItem = (FileItem)ProjectTree.SelectedItem;
                        selectedItem.Name = newName;
                        
                    }
                    else if (ProjectTree.SelectedItem.GetType() == typeof(FolderItem))
                    {
                        FolderItem selectedItem = (FolderItem)ProjectTree.SelectedItem;
                        selectedItem.Name = newName;
                    }
                    else
                    {
                        Project selectedProject = (Project)ProjectTree.SelectedItem;
                        selectedProject.Name = newName;
                    }
                }
                else
                {
                    //TODO Show invalid name message
                }
            }
        }

        private void CloseProject(object sender, RoutedEventArgs e)
        {
            //TODO: check if anything is not saved
            MessageBoxResult saveResult = MessageBox.Show("Do you want to save your work before closing the project?", "Close Project Save", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            // Clean up tree view if Cancel was not selected
            if (saveResult != MessageBoxResult.Cancel)
            {
                Project selectedItem = (Project)ProjectTree.SelectedItem;
                Projects.Remove(selectedItem);
            }

            TabItem tab = (TabItem)TextEditor.Parent;
            tab.Content = "";
            tab.Header = "No File";
            tab.FontStyle = FontStyles.Italic;
        }

        private void CreateFileFolder(object sender, RoutedEventArgs e)
        {
            InputWindow input = new InputWindow();
            //TODO Add creating folders

            if (input.ShowDialog() == true)
            {
                string fileName = input.Input.Text;
                if (fileName == "")
                    fileName = "Untitled.txt"; // Default name
                else if (!fileName.Contains("."))
                    fileName += ".txt"; // Default extension

                //TODO: Add under (folder selected) or next (item selected) to currently selected item
                //TODO: Check if file already exists

                CreateProjectItem((TreeViewItem)ProjectTree.Items.GetItemAt(0), fileName, false);
                TextEditor.Text = "/* Default text */";
            }
        }

        private void DeleteFileFolder(object sender, RoutedEventArgs e)
        {
            if (ProjectTree.SelectedItem.GetType() == typeof(FileItem))
            {
                FileItem selectedItem = (FileItem)ProjectTree.SelectedItem;
                File.Delete(selectedItem.Info.FullName);
                selectedItem.ContainingCollection.Remove(selectedItem);
            }
            else
            {
                FolderItem selectedItem = (FolderItem)ProjectTree.SelectedItem;
                Directory.Delete(selectedItem.Info.FullName);
                selectedItem.ContainingCollection.Remove(selectedItem);
            }

            TabItem tab = (TabItem)TextEditor.Parent;
            tab.Header = "";
            tab.Header = "No File";
            tab.FontStyle = FontStyles.Italic;
        }

        private void ProjectItemSelected(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // Remove previous items
            Methods.Clear();

            if (e.NewValue != null && e.NewValue.GetType() == typeof(FileInfo))
            {
                FileInfo selectedItem = (FileInfo)e.NewValue;

                TabItem tab = (TabItem)TextEditor.Parent;
                Regex regex = new Regex(@"(private|protected|public) (.+?)\)");

                // Show file contents
                if (tab != null)
                {
                    using (var reader = new StreamReader(selectedItem.FullName, Encoding.Unicode))
                        TextEditor.Text = reader.ReadToEnd();
                    tab.Header = selectedItem.Name;
                    tab.FontStyle = FontStyles.Normal;
                }

                // Add selected class methods to method list
                if (selectedItem.Extension == ".cs")
                {
                    using (var reader = new StreamReader(selectedItem.FullName, Encoding.Unicode))
                    {
                        while (!reader.EndOfStream)
                        {
                            foreach (Match match in regex.Matches(reader.ReadLine()))
                            {
                                if (match.Groups[2] != null)
                                {
                                    Methods.Add(new Method(match.Groups[1].Value, match.Groups[2].Value, selectedItem)); // 1: type, 2: 
                                }
                            }
                        }
                    }
                }
            }
        }

        private void MethodSelected(object sender, SelectionChangedEventArgs e)
        {
            ListView methodList = (ListView)sender;
            Method selectedItem = (Method)methodList.SelectedItem;

            if (selectedItem != null)
            {
                string method = selectedItem.Signature;
                FileInfo filePath = selectedItem.ContainingFile;

                TabItem tab = (TabItem)TextEditor.Parent;

                Regex regex = new Regex(method.Replace("(", @"\(").Replace(")", @"\)") + @"[^{]*.\n([^}]*)}");

                MatchCollection matches;
                using (var reader = new StreamReader(selectedItem.ContainingFile.FullName, Encoding.Unicode))
                    matches = regex.Matches(reader.ReadToEnd());

                if (matches != null && matches.Count > 0)
                {
                    //TODO Navigate to whole file
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
            {
                //MessageBox.Show("Settings", "OK", MessageBoxButton.OK);
            }
        }

        private void Exit(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Save settings
            Properties.Settings.Default.TextWrap = (int)TextEditor.TextWrapping;
            Properties.Settings.Default.Save();

            // Save to cache
            //XmlSerializer serializer = new XmlSerializer(typeof(Cache));
            //using (TextWriter writer = new StreamWriter("Cache.xml"))
            //{
            //    Cache cache = new Cache(TextEditor.TextWrapping);
            //    serializer.Serialize(writer, cache);
            //}
        }
    }
}
