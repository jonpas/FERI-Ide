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

                var subDirItems = new ObservableCollection<FolderItem>();
                foreach (var dir in Info.GetDirectories())
                {
                    subDirItems.Add(new FolderItem(dir, subDirItems, ContainingProject, this));
                }

                children.Add(new CollectionContainer { Collection = subDirItems });

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

        public ObservableCollection<FileItem> FilesCollection { get; set; }


        public FolderItem(DirectoryInfo dir, ObservableCollection<FolderItem> containingCollection, Project containingProject, FolderItem containingFolder)
        {
            Info = dir;
            ContainingCollection = containingCollection;
            ContainingProject = containingProject;
            ContainingFolder = containingFolder;
        }

        public void AddFile(string fileName)
        {
            FilesCollection.Add(new FileItem(new FileInfo(Location + "/" + fileName), FilesCollection, ContainingProject, null));
        }
    }
}
