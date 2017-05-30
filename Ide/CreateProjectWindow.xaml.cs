using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Ide
{
    /// <summary>
    /// Interaction logic for CreateProjectWindow.xaml
    /// </summary>
    public partial class CreateProjectWindow : Window
    {
        //TODO Add 2 additional settings (task requirement)
        public string SelectedLocation = "";
        public string SelectedLanguage = "";
        public string SelectedType = "";
        public string SelectedFramework = "";

        private string NewProjectDirectory = Properties.Settings.Default.ProjectsDirectory + @"\" + Constants.DefaultProjectDirectory;

        public CreateProjectWindow()
        {
            InitializeComponent();

            Directory.CreateDirectory(NewProjectDirectory); // Temporary folder for select
            LocationBox.Text = NewProjectDirectory;
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
                LocationBox.Text = dlg.FileName;
                SelectedLocation = LocationBox.Text;
            }

            // Delete temporary folder if not left selected
            if (Path.GetDirectoryName(dlg.FileName) != NewProjectDirectory)
                Directory.Delete(NewProjectDirectory);
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
