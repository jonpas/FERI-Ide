using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Ide
{
    /// <summary>
    /// Interaction logic for CreateProjectWindow.xaml
    /// </summary>
    public partial class CreateProjectWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string SelectedLocation
        {
            get { return _selectedLocation; }
            set
            {
                _selectedLocation = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }
        private string _selectedLocation;

        public string SelectedLanguage = "";
        public string SelectedType = "";
        public string SelectedFramework = "";

        public string Author
        {
            get { return _author; }
            set
            {
                _author = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }
        private string _author;

        public string Version
        {
            get { return _version; }
            set
            {
                _version = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }
        private string _version;

        private string NewProjectDirectory = Properties.Settings.Default.ProjectsDirectory + @"\" + Constants.DefaultProjectDirectory;

        public CreateProjectWindow()
        {
            InitializeComponent();

            // Defaults
            SelectedLocation = NewProjectDirectory + @"\" + Constants.DefaultProjectFile;
            Author = Environment.UserName;
            Version = "1.0.0";

            Directory.CreateDirectory(NewProjectDirectory); // (Possibly) Temporary folder for SaveFileDialog
        }

        private void SetLanguage(object sender, SelectionChangedEventArgs e)
        {
            TypesList.Items.Clear();

            ComboBox languagesList = (ComboBox)sender;
            SelectedLanguage = (string)languagesList.SelectedItem;
            int selectedLangaugeIndex = languagesList.SelectedIndex;

            foreach (var type in Properties.Settings.Default.Types[selectedLangaugeIndex])
                TypesList.Items.Add(type);
        }

        private void SetType(object sender, SelectionChangedEventArgs e)
        {
            ComboBox typesList = (ComboBox)sender;
            SelectedType = (string)typesList.SelectedItem;
        }

        private void SetFramework(object sender, SelectionChangedEventArgs e)
        {
            ComboBox frameworksList = (ComboBox)sender;
            SelectedFramework = (string)frameworksList.SelectedItem;
        }

        private void Browse(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.InitialDirectory = NewProjectDirectory;
            dlg.FileName = Constants.DefaultProjectFile;
            dlg.Filter = "Text documents (.xml)|*.xml"; // Filter files by extension

            if (dlg.ShowDialog() == true)
            {
                SelectedLocation = dlg.FileName;
            }

            // Delete temporary folder if not left selected and is not used as a project (contains files/folders)
            if (Path.GetDirectoryName(dlg.FileName) != NewProjectDirectory && !Directory.EnumerateFileSystemEntries(NewProjectDirectory).Any())
                Directory.Delete(NewProjectDirectory);
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
