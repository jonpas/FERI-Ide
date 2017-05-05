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
        //TODO Add 2 additional settings (task requirement)
        public string SelectedLocation = "";
        public string SelectedLanguage = "";
        public string SelectedType = "";
        public string SelectedFramework = "";

        public CreateProjectWindow()
        {
            InitializeComponent();

            LocationBox.Text = Properties.Settings.Default.ProjectsDirectory + @"\" + Constants.DefaultProjectFile;
        }

        private void SetLanguage(object sender, SelectionChangedEventArgs e)
        {
            TypesList.Items.Clear();

            ComboBox languagesList = (ComboBox)sender;
            SelectedLanguage = (string)languagesList.SelectedItem;
            int selectedLangaugeIndex = languagesList.SelectedIndex;

            foreach (var type in Properties.Settings.Default.Types[selectedLangaugeIndex])
            {
                TypesList.Items.Add(type);
            }
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
            dlg.InitialDirectory = Properties.Settings.Default.ProjectsDirectory;
            dlg.Filter = "Text documents (.xml)|*.xml"; // Filter files by extension

            if (dlg.ShowDialog() == true)
            {
                LocationBox.Text = dlg.FileName;
                SelectedLocation = LocationBox.Text;
            }
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
