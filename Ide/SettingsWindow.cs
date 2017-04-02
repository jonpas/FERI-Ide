using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        Dictionary<string, List<string>> languageTypes;

        public SettingsWindow()
        {
            InitializeComponent();

            // Prepare language<->types map
            languageTypes = new Dictionary<string, List<string>>();

            // Load settings
            var languages = Properties.Settings.Default.Languages;
            if (languages != null)
            {
                foreach (var language in languages)
                {
                    AddItem(LanguagesList, language);
                }
            }

            var types = Properties.Settings.Default.Types;
            if (types != null)
            {
                for (var i = 0; i < types.Count(); i++)
                {
                    languageTypes[languages[i]] = types[i];
                }
            }

            var frameworks = Properties.Settings.Default.Frameworks;
            if (frameworks != null)
            {
                foreach (var item in frameworks)
                {
                    AddItem(FrameworksList, item);
                }
            }
        }

        private void AddItem(ListView list, string text)
        {
            ListViewItem item = new ListViewItem();
            item.Content = text;
            list.Items.Add(item);
            languageTypes.Add(text, new List<string>());
        }

        private void AddItem(object sender, RoutedEventArgs e)
        {
            string listStr = (string)((Button)sender).Tag;
            ListView list = (ListView)FindName(listStr);

            InputWindow language = new InputWindow();

            if (language.ShowDialog() == true)
            {
                string text = language.Input.Text;
                var types = new List<string>() { "test" };

                if (list == TypesList)
                {
                    // Add type
                    string selectedLangauge = (string)((ListViewItem)LanguagesList.SelectedItem).Content;

                    if (languageTypes.TryGetValue(selectedLangauge, out types))
                    {
                        types.Add(text);
                        languageTypes[selectedLangauge] = types;
                        ListTypes();
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

            string selectedLangauge = (string)((ListViewItem)LanguagesList.SelectedItem).Content;

            List<string> types;
            if (languageTypes.TryGetValue(selectedLangauge, out types))
            {
                foreach (var type in types)
                {
                    TypesList.Items.Add(type);
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
                languageTypes.TryGetValue(item, out itemTypes);
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
