using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

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


        public ObservableCollection<Project> Projects { get; }
        public ObservableCollection<Method> Methods { get; }


        public ProjectStructure()
        {
            InitializeComponent();

            // Create collections
            Projects = new ObservableCollection<Project>();
            Methods = new ObservableCollection<Method>();

            // Set binding sources
            ProjectTree.ItemsSource = Projects;
            MethodList.ItemsSource = Methods;

            //TEST
            //Projects.Add(new Project(Properties.Settings.Default.ProjectsDirectory, "Ide.proj.xml", "C#", "Code", "WPF"));
            //Projects.Add(new Project(Properties.Settings.Default.ProjectsDirectory, "Project.xml", "C++", "Code", "WPF"));
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

        public void ReadProject(string projFilePath, string dir)
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

        public void CreateProject(string location, string language, string type, string framework)
        {
            string projectFolder = Path.GetDirectoryName(location);
            string projectFile = Path.GetFileName(location);
            Project newProj = new Project(projectFolder, projectFile, language, type, framework);

            XmlSerializer serializer = new XmlSerializer(typeof(Project));
            using (FileStream newProjFile = File.Create(location))
            {
                serializer.Serialize(newProjFile, newProj);
            }

            Projects.Add(newProj);
        }

        public void CloseProject(Project selectedProject)
        {
            Projects.Remove(selectedProject);
        }

        public void AddMethod(string type, string signature, FileInfo containingFile)
        {
            Methods.Add(new Method(type, signature, containingFile));
        }

        public void ClearMethods()
        {
            Methods.Clear();
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

        private void CreateMethod(object sender, RoutedEventArgs e)
        {
            //TODO Method creation window instead of string splitting (with disabled buttons if empty fields)
            InputWindow input = new InputWindow();
            if (input.ShowDialog() == true)
            {
                string[] methodInfo = input.Input.Text.Split(null); // Space separation
                if (methodInfo.Length > 1)
                {
                    FileItem selectedFile = (FileItem)SelectedProjectItem;
                    AddMethod(methodInfo[0], methodInfo[0] + " " + methodInfo[1], selectedFile.Info);
                }
            }
        }

        private void EditMethod(object sender, RoutedEventArgs e)
        {
            Method selectedMethod = (Method)MethodList.SelectedItem;

            //TODO Method creation window instead of string splitting (with disabled buttons if empty fields)
            InputWindow input = new InputWindow();
            input.Input.Text = selectedMethod.Type + " " + selectedMethod.Signature;

            if (input.ShowDialog() == true)
            {
                string[] methodInfo = input.Input.Text.Split(null); // Space separation
                if (methodInfo.Length > 1)
                {
                    selectedMethod.Type = methodInfo[0];
                    selectedMethod.Signature = methodInfo[0] + " " + methodInfo[1];
                }
            }
        }

        private void RemoveMethod(object sender, RoutedEventArgs e)
        {
            Method selectedMethod = (Method)MethodList.SelectedItem;
            Methods.Remove(selectedMethod);
        }
    }
}
