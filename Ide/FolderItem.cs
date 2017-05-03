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

        public IList Children
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


        public FolderItem(DirectoryInfo dir, ObservableCollection<FolderItem> containingCollection)
        {
            Info = dir;
            ContainingCollection = containingCollection;
        }

        public FolderItem(string path, ObservableCollection<FolderItem> containingCollection)
        {
            Info = new DirectoryInfo(path);
            ContainingCollection = containingCollection;
        }
    }
}
