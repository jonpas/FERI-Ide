using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace Ide
{
    /// <summary>
    /// Interaction logic for CreateProjectWindow.xaml
    /// </summary>
    public partial class CreateProjectWindow : Window
    {
        public CreateProjectWindow()
        {
            InitializeComponent();

            // Create default projects directory if it doesn't exist yet
            string projectDir = Properties.Settings.Default.ProjectsDirectory;
            if (projectDir == "")
                Properties.Settings.Default.ProjectsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Ide Projects";

            if (!Directory.Exists(Properties.Settings.Default.ProjectsDirectory))
                Directory.CreateDirectory(Properties.Settings.Default.ProjectsDirectory);

            LocationText.Text = Properties.Settings.Default.ProjectsDirectory + @"\NewProject.xml";
        }

        private void ListTypes(object sender, SelectionChangedEventArgs e)
        {
            TypesList.Items.Clear();

            ComboBox languagesList = (ComboBox)sender;
            string selectedLanguage = (string)languagesList.SelectedItem;
            int selectedLangaugeIndex = languagesList.SelectedIndex;

            foreach (var type in Properties.Settings.Default.Types[selectedLangaugeIndex])
            {
                TypesList.Items.Add(type);
            }
        }

        private void Browse(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.InitialDirectory = Properties.Settings.Default.ProjectsDirectory;
            dlg.Filter = "Text documents (.xml)|*.xml"; // Filter files by extension

            if (dlg.ShowDialog() == true)
            {
                LocationText.Text = dlg.FileName;
            }
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            if (ProjectName.Text == "")
                ProjectName.Text = "Untitled";

            DialogResult = true;
        }
    }
}
