using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ide
{
    public class FileItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public FileInfo Info { get; set; }

        public string Name
        {
            get { return Info.Name; }
            set
            {
                Info.MoveTo(Info.DirectoryName + "/" + value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }

        public string Extension
        {
            get { return Info.Extension; }
        }

        public ObservableCollection<FileItem> ContainingCollection
        {
            get { return _containingCollection; }
            set
            {
                _containingCollection = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }
        private ObservableCollection<FileItem> _containingCollection;

        public Project ContainingProject { get; set; }

        public FolderItem ContainingFolder { get; set; }


        public FileItem(FileInfo file, ObservableCollection<FileItem> containingCollection, Project containingProject, FolderItem containingFolder)
        {
            Info = file;
            ContainingCollection = containingCollection;
            ContainingProject = containingProject;
            ContainingFolder = containingFolder;
        }
    }
}
