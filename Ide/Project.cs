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
using System.Xml.Serialization;

namespace Ide
{
    public class Project : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        //TODO Add one more serializable property (task requirement)

        [XmlIgnore]
        public DirectoryInfo Info { get; set; }

        [XmlIgnore]
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

        [XmlIgnore]
        public string Location
        {
            get { return Info.FullName; }
        }
   
        public string ProjectFile { get; set; }

        [XmlIgnore]
        public IList FilesFolders
        {
            get
            {
                var children = new CompositeCollection();

                FoldersCollection = new ObservableCollection<FolderItem>();
                foreach (var dir in Info.GetDirectories())
                {
                    FoldersCollection.Add(new FolderItem(dir, FoldersCollection, this, null));
                }
                children.Add(new CollectionContainer { Collection = FoldersCollection });

                FilesCollection = new ObservableCollection<FileItem>();
                foreach (var file in Info.GetFiles().Where(name => !IgnoredItems.Contains(name.Name)))
                {
                    FilesCollection.Add(new FileItem(file, FilesCollection, this, null));
                }
                children.Add(new CollectionContainer { Collection = FilesCollection });

                return children;
            }
        }

        [XmlIgnore]
        public ObservableCollection<FolderItem> FoldersCollection { get; set; }

        [XmlIgnore]
        public ObservableCollection<FileItem> FilesCollection { get; set; }

        [XmlArrayItem("Item")]
        public List<string> IgnoredItems { get; set; }


        private Project() { }

        public Project(string path, string projectFile, string language, string type, string framework)
        {
            Info = new DirectoryInfo(path);
            ProjectFile = projectFile;
            Language = language;
            Type = type;
            Framework = framework;

            IgnoredItems = new List<string>();
            IgnoredItems.Add(projectFile);
        }

        public Project(string path, string projectFile, string language, string type, string framework, List<string> ignoredItems)
        {
            Info = new DirectoryInfo(path);
            ProjectFile = projectFile;
            Language = language;
            Type = type;
            Framework = framework;

            IgnoredItems = ignoredItems;
            IgnoredItems.Add(projectFile);
        }

        public void AddFile(string fileName)
        {
            string path = Location + "/" + fileName;
            File.WriteAllText(path, "/* Default text */");
            FilesCollection.Add(new FileItem(path, FilesCollection, this, null));
        }

        public void AddFolder(string folderName)
        {
            string path = Location + "/" + folderName;
            Directory.CreateDirectory(path);
            FoldersCollection.Add(new FolderItem(path, FoldersCollection, this, null));
        }
    }
}
