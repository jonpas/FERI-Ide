using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Data;

namespace Ide
{
    // Adapted and expanded from: https://xinyustudio.wordpress.com/?s=WPF%20tree
    public class FolderItem : INotifyPropertyChanged
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

        public string Location
        {
            get { return Info.FullName; }
        }

        public IList Children
        {
            get
            {
                var children = new CompositeCollection();

                FoldersCollection = new ObservableCollection<FolderItem>();
                foreach (var dir in Info.GetDirectories())
                {
                    FoldersCollection.Add(new FolderItem(dir, FoldersCollection, ContainingProject, this));
                }
                children.Add(new CollectionContainer { Collection = FoldersCollection });

                FilesCollection = new ObservableCollection<FileItem>();
                foreach (var file in Info.GetFiles())
                {
                    FilesCollection.Add(new FileItem(file, FilesCollection, ContainingProject, this));
                }
                children.Add(new CollectionContainer { Collection = FilesCollection });

                return children;
            }
        }

        public ObservableCollection<FolderItem> ContainingCollection
        {
            get { return _containingCollection; }
            set
            {
                _containingCollection = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }
        private ObservableCollection<FolderItem> _containingCollection;

        public Project ContainingProject { get; set; }

        public FolderItem ContainingFolder { get; set; }

        public ObservableCollection<FolderItem> FoldersCollection { get; set; }

        public ObservableCollection<FileItem> FilesCollection { get; set; }


        public FolderItem(DirectoryInfo dir, ObservableCollection<FolderItem> containingCollection, Project containingProject, FolderItem containingFolder)
        {
            Info = dir;
            ContainingCollection = containingCollection;
            ContainingProject = containingProject;
            ContainingFolder = containingFolder;
        }

        public FolderItem(string path, ObservableCollection<FolderItem> containingCollection, Project containingProject, FolderItem containingFolder)
        {
            Info = new DirectoryInfo(path);
            ContainingCollection = containingCollection;
            ContainingProject = containingProject;
            ContainingFolder = containingFolder;
        }

        public void AddFile(string fileName)
        {
            string path = Location + "/" + fileName;
            File.WriteAllText(path, "/* Default text */", Encoding.UTF8);
            FilesCollection.Add(new FileItem(path, FilesCollection, ContainingProject, null));
        }

        public void AddFolder(string folderName)
        {
            string path = Location + "/" + folderName;
            Directory.CreateDirectory(path);
            FoldersCollection.Add(new FolderItem(path, FoldersCollection, ContainingProject, null));
        }
    }
}
