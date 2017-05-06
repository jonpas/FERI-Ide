using ICSharpCode.AvalonEdit;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
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
            //TODO Encode files in UTF-8 without BOM (writing and reading)
            InitializeComponent();

            // Create default projects directory if it doesn't exist yet
            string projectDir = Properties.Settings.Default.ProjectsDirectory;
            if (projectDir == "")
                Properties.Settings.Default.ProjectsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/" + Constants.ProjectsFolder;

            if (!Directory.Exists(Properties.Settings.Default.ProjectsDirectory))
                Directory.CreateDirectory(Properties.Settings.Default.ProjectsDirectory);

            // Load settings
            TextEditor.WordWrap = Properties.Settings.Default.TextWrap;

            // Load cache
            if (File.Exists(Constants.CacheFile))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Cache));
                using (TextReader reader = new StreamReader(Constants.CacheFile))
                {
                    try
                    {
                        Cache cache = (Cache)serializer.Deserialize(reader);
                        List<OpenProject> openProjects = cache.OpenProjects;
                        foreach (var openProj in openProjects)
                        {
                            ReadProject(openProj.ProjectFileLocation, openProj.Location);
                        }
                    }
                    catch
                    {
                        //TODO Write to pop-up
                        Console.WriteLine("Error! Cache failed deserializing!");
                    }
                }
            }

            // Set binding sources
            ProjStruct.ProjectTree.ItemsSource = Projects;
            ProjStruct.MethodList.ItemsSource = Methods;

            //TEST
            //Projects.Add(new Project(Properties.Settings.Default.ProjectsDirectory, "Ide.proj.xml", "C#", "Code", "WPF"));
            //Projects.Add(new Project(Properties.Settings.Default.ProjectsDirectory, "Project.xml", "C++", "Code", "WPF"));
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private bool IsProjectOpen(string path)
        {
            foreach (Project proj in Projects)
            {
                if (proj.Location == path)
                {
                    return true;
                }
            }
            return false;
        }

        private void ReadProject(string projFilePath, string dir)
        {
            if (File.Exists(projFilePath) && !IsProjectOpen(dir)) //TODO Check if project already open
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Project));
                using (TextReader reader = new StreamReader(projFilePath))
                {
                    try
                    {
                        Project proj = (Project)serializer.Deserialize(reader);
                        // Create new project with defined path (project file does not contain location)
                        Projects.Add(new Project(dir, proj.ProjectFile, proj.Language, proj.Type, proj.Framework, proj.IgnoredItems));
                    }
                    catch
                    {
                        //TODO Write to pop-up
                        Console.WriteLine("Error! Project failed deserializing!");
                    }
                }
            }
            else
            {
                //TODO Error pop-up
                Console.WriteLine("Error! No project file or project already open!");
            }
        }

        private void CreateProject(object sender, RoutedEventArgs e)
        {
            CreateProjectWindow newProjWin = new CreateProjectWindow();

            if (newProjWin.ShowDialog() == true)
            {
                string projectFolder = Path.GetDirectoryName(newProjWin.SelectedLocation);
                string projectFile = Path.GetFileName(newProjWin.SelectedLocation);
                Project newProj = new Project(projectFolder, projectFile, newProjWin.SelectedLanguage, newProjWin.SelectedType, newProjWin.SelectedFramework);

                XmlSerializer serializer = new XmlSerializer(typeof(Project));
                using (FileStream newProjFile = File.Create(newProjWin.SelectedLocation))
                {
                    serializer.Serialize(newProjFile, newProj);
                }

                Projects.Add(newProj);
            }
        }

        private void OpenProject(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = Properties.Settings.Default.ProjectsDirectory;
            dlg.Filter = "Project Files (.xml)|*.xml"; // Filter files by extension

            if (dlg.ShowDialog() == true)
            {
                ReadProject(dlg.FileName, Path.GetDirectoryName(dlg.FileName));
            }
        }

        private void CloseProject(object sender, RoutedEventArgs e)
        {
            //TODO: check if anything is not saved
            MessageBoxResult saveResult = MessageBox.Show("Do you want to save your work before closing the project?", "Close Project Save", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            // Clean up tree view if Cancel was not selected
            if (saveResult != MessageBoxResult.Cancel)
            {
                Project selectedItem = (Project)ProjStruct.ProjectTree.SelectedItem;
                Projects.Remove(selectedItem);
            }

            TabItem tab = (TabItem)TextEditor.Parent;
            if (tab != null)
            {
                tab.Content = "";
                tab.Header = "No File";
                tab.FontStyle = FontStyles.Italic;
            }
        }

        private void CreateFileFolder(object sender, RoutedEventArgs e)
        {
            InputWindow input = new InputWindow();
            if (input.ShowDialog() == true)
            {
                string fileName = input.Input.Text;
                if (fileName == "")
                    fileName = "Untitled.txt"; // Default name

                Button button = (Button)sender;
                bool wantedFolder = (string)button.Tag == "Folder";

                //TODO Cleanup
                //TODO Handle cannot create exceptions (in Project and FolderItem classes)
                if (ProjStruct.ProjectTree.SelectedItem.GetType() == typeof(FileItem))
                {
                    FileItem selectedItem = (FileItem)ProjStruct.ProjectTree.SelectedItem;
                    if (selectedItem.ContainingFolder != null)
                    {
                        if (wantedFolder)
                            selectedItem.ContainingFolder.AddFolder(fileName);
                        else
                            selectedItem.ContainingFolder.AddFile(fileName);
                    }
                    else
                    {
                        if (wantedFolder)
                            selectedItem.ContainingProject.AddFolder(fileName);
                        else
                            selectedItem.ContainingProject.AddFile(fileName);
                    }
                }
                else if (ProjStruct.ProjectTree.SelectedItem.GetType() == typeof(FolderItem))
                {
                    FolderItem selectedItem = (FolderItem)ProjStruct.ProjectTree.SelectedItem;
                    if (wantedFolder)
                        selectedItem.AddFolder(fileName);
                    else
                        selectedItem.AddFile(fileName);
                }
                else
                {
                    Project selectedItem = (Project)ProjStruct.ProjectTree.SelectedItem;
                    if (wantedFolder)
                        selectedItem.AddFolder(fileName);
                    else
                        selectedItem.AddFile(fileName);
                }
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
                    //TODO Cleanup
                    if (ProjStruct.ProjectTree.SelectedItem.GetType() == typeof(FileItem))
                    {
                        FileItem selectedItem = (FileItem)ProjStruct.ProjectTree.SelectedItem;
                        selectedItem.Name = newName;

                    }
                    else if (ProjStruct.ProjectTree.SelectedItem.GetType() == typeof(FolderItem))
                    {
                        FolderItem selectedItem = (FolderItem)ProjStruct.ProjectTree.SelectedItem;
                        selectedItem.Name = newName;
                    }
                    else
                    {
                        Project selectedProject = (Project)ProjStruct.ProjectTree.SelectedItem;
                        selectedProject.Name = newName;
                    }
                }
                else
                {
                    //TODO Show invalid name message
                }
            }
        }

        private void DeleteFileFolder(object sender, RoutedEventArgs e)
        {
            //TODO Add confirmation dialog
            if (ProjStruct.ProjectTree.SelectedItem.GetType() == typeof(FileItem))
            {
                FileItem selectedItem = (FileItem)ProjStruct.ProjectTree.SelectedItem;
                File.Delete(selectedItem.Info.FullName);
                selectedItem.ContainingCollection.Remove(selectedItem);
            }
            else
            {
                FolderItem selectedItem = (FolderItem)ProjStruct.ProjectTree.SelectedItem;
                Directory.Delete(selectedItem.Info.FullName);
                selectedItem.ContainingCollection.Remove(selectedItem);
            }

            TabItem tab = (TabItem)TextEditor.Parent;
            tab.Header = "";
            tab.Header = "No File";
            tab.FontStyle = FontStyles.Italic;
        }

        //TODO Move to ProjectStructure and make private
        /*private*/public void ProjectItemSelected(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // Remove previous items
            Methods.Clear();

            if (e.NewValue != null && e.NewValue.GetType() == typeof(FileItem))
            {
                FileItem selectedItemFile = (FileItem)e.NewValue;
                FileInfo selectedItem = selectedItemFile.Info;

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
                                    Methods.Add(new Method(match.Groups[1].Value, match.Groups[2].Value + ")", selectedItem)); // 1: type, 2: signature
                                }
                            }
                        }
                    }
                }
            }
        }

        //TODO Move to ProjectStructure and make private
        /*private*/public void MethodSelected(object sender, SelectionChangedEventArgs e)
        {
            ListView methodList = (ListView)sender;
            Method selectedItem = (Method)methodList.SelectedItem;

            if (selectedItem != null)
            {
                string method = selectedItem.Signature;
                FileInfo filePath = selectedItem.ContainingFile;

                TabItem tab = (TabItem)TextEditor.Parent;

                //TODO Fix showing end of the method
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

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = Properties.Settings.Default.ProjectsDirectory;
            dlg.Filter = Project.AllowedFileTypes; // Filter files by extension

            if (dlg.ShowDialog() == true)
            {
                TabItem tab = (TabItem)TextEditor.Parent;
                TextEditor.Text = File.ReadAllText(dlg.FileName);
                tab.Header = Path.GetFileName(dlg.FileName);
                tab.FontStyle = FontStyles.Italic;
            }
        }

        private void SaveFile(object sender, RoutedEventArgs e)
        {
            FileItem selectedItem = (FileItem)ProjStruct.ProjectTree.SelectedItem;
            File.WriteAllText(selectedItem.Location, TextEditor.Text);
        }

        private void SaveFileAs(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.InitialDirectory = Properties.Settings.Default.ProjectsDirectory;
            dlg.Filter = Project.AllowedFileTypes; // Filter files by extension

            if (dlg.ShowDialog() == true)
            {
                File.WriteAllText(dlg.FileName, TextEditor.Text);
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
            Properties.Settings.Default.TextWrap = TextEditor.WordWrap;
            Properties.Settings.Default.Save();

            // Save cache
            XmlSerializer serializer = new XmlSerializer(typeof(Cache));
            using (TextWriter writer = new StreamWriter(Constants.CacheFile))
            {
                Cache cache = new Cache(Projects);
                serializer.Serialize(writer, cache);
            }
        }
    }
}
