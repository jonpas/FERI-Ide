using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Ide
{
    public class Project : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public DirectoryInfo Info { get; set; }

        public string Name
        {
            get { return Info.Name; }
            set
            {
                Info.MoveTo(Info.Parent.FullName + "/" + value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }

        public string Language { get; set; }

        public string Type { get; set; }

        public string Framework { get; set; }

        public string Location
        {
            get { return _location; }
            set
            {
                _location = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }
        private string _location;

        public IList FilesFolders
        {
            get
            {
                var children = new CompositeCollection();

                var subDirItems = new ObservableCollection<FolderItem>();
                foreach (var dir in Info.GetDirectories())
                {
                    subDirItems.Add(new FolderItem(dir, subDirItems));
                }

                children.Add(new CollectionContainer { Collection = subDirItems });

                var fileItems = new ObservableCollection<FileItem>();
                foreach (var file in Info.GetFiles())
                {
                    fileItems.Add(new FileItem(file, fileItems));
                }
                children.Add(new CollectionContainer { Collection = fileItems });

                return children;
            }
        }

        public Project(DirectoryInfo dir, string language, string type, string framework)
        {
            Info = dir;
            Language = language;
            Type = type;
            Framework = framework;
        }

        public Project(string path, string language, string type, string framework)
        {
            Info = new DirectoryInfo(path);
            Language = language;
            Type = type;
            Framework = framework;
        }
    }
}
