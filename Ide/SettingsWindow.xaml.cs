using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Ide
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        Dictionary<string, List<string>> LanguageTypes;

        public SettingsWindow()
        {
            InitializeComponent();

            // Prepare language<->types map
            LanguageTypes = new Dictionary<string, List<string>>();

            // Load settings
            var languages = Properties.Settings.Default.Languages;
            if (languages != null)
            {
                foreach (var language in languages)
                    AddItem(LanguagesList, language);
            }
            else
            {
                // Add default languages
                if (AddItem(LanguagesList, "C++"))
                {
                    LanguageTypes["C++"].Add("Empty");
                    LanguageTypes["C++"].Add("Console");
                }
                if (AddItem(LanguagesList, "C#"))
                {
                    LanguageTypes["C#"].Add("Empty");
                    LanguageTypes["C#"].Add("WPF");
                }
            }

            var types = Properties.Settings.Default.Types;
            if (types != null)
            {
                for (var i = 0; i < types.Count(); i++)
                    LanguageTypes[languages[i]] = types[i];
            }

            var frameworks = Properties.Settings.Default.Frameworks;
            if (frameworks != null)
            {
                foreach (var item in frameworks)
                    AddItem(FrameworksList, item);
            }
            else
            {
                // Add default frameworks
                AddItem(FrameworksList, "QT");
                AddItem(FrameworksList, "SDL2");
                AddItem(FrameworksList, ".NET");
            }
        }

        private void Browse(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.InitialDirectory = Properties.Settings.Default.ProjectsDirectory;
            dlg.FileName = "Select Folder";

            if (dlg.ShowDialog() == true)
            {
                Properties.Settings.Default.ProjectsDirectory = System.IO.Path.GetDirectoryName(dlg.FileName);
            }
        }

        private bool AddItem(ListView list, string text)
        {
            if (!LanguageTypes.ContainsKey(text))
            {
                ListViewItem item = new ListViewItem();
                item.Content = text;
                list.Items.Add(item);
                LanguageTypes.Add(text, new List<string>());
                return true;
            }
            else
            {
                //TODO Notify type already exists
                return false;
            }
        }

        private void AddItem(object sender, RoutedEventArgs e)
        {
            string listStr = (string)((Button)sender).Tag;
            ListView list = (ListView)FindName(listStr);

            InputWindow language = new InputWindow();

            if (language.ShowDialog() == true)
            {
                string text = language.Input.Text;

                if (list == TypesList)
                {
                    // Add type
                    string selectedLangauge = (string)((ListViewItem)LanguagesList.SelectedItem).Content;

                    var types = new List<string>();
                    if (LanguageTypes.TryGetValue(selectedLangauge, out types))
                    {
                        if (!types.Contains(text))
                        {
                            //TODO Don't save types list on Cancel
                            types.Add(text);
                            LanguageTypes[selectedLangauge] = types;
                            ListTypes();
                        }
                        else
                        {
                            //TODO Notify type already exists
                        }
                    }
                }
                else
                {
                    // Add language
                    AddItem(list, text);
                }
            }
        }

        private void EditItem(object sender, RoutedEventArgs e)
        {
            string listStr = (string)((Button)sender).Tag;
            ListView list = (ListView)FindName(listStr);
            ListViewItem item = (ListViewItem)list.SelectedItem;

            InputWindow language = new InputWindow();
            language.Input.Text = (string)item.Content;

            if (language.ShowDialog() == true)
            {
                item.Content = language.Input.Text;
            }
        }

        private void RemoveItem(object sender, RoutedEventArgs e)
        {
            string listStr = (string)((Button)sender).Tag;
            ListView list = (ListView)FindName(listStr);
            ListViewItem selectedItem = (ListViewItem)list.SelectedItem;
            list.Items.Remove(selectedItem);
        }

        private void ListTypes()
        {
            TypesList.Items.Clear();

            ListViewItem selectedLanguageItem = (ListViewItem)LanguagesList.SelectedItem;
            if (selectedLanguageItem != null)
            {
                string selectedLangauge = (string)selectedLanguageItem.Content;

                List<string> types;
                if (LanguageTypes.TryGetValue(selectedLangauge, out types))
                {
                    foreach (var type in types)
                    {
                        TypesList.Items.Add(type);
                    }
                }
            }
        }

        private void ListTypes(object sender, SelectionChangedEventArgs e)
        {
            ListTypes();
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            // Save settings
            var languages = new List<string>();
            foreach (ListViewItem item in LanguagesList.Items)
                languages.Add(item.Content.ToString());
            Properties.Settings.Default.Languages = languages;

            var types = new List<List<string>>();
            foreach (var item in languages)
            {
                var itemTypes = new List<string>();
                LanguageTypes.TryGetValue(item, out itemTypes);
                types.Add(itemTypes);
            }
            Properties.Settings.Default.Types = types;

            var frameworks = new List<string>();
            foreach (ListViewItem item in FrameworksList.Items)
                frameworks.Add(item.Content.ToString());
            Properties.Settings.Default.Frameworks = frameworks;

            Properties.Settings.Default.Save();

            // Close
            DialogResult = true;
        }
    }
}
