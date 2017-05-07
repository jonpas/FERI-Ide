using System.ComponentModel;
using System.IO;

namespace Ide
{
    public class Method : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Type { get; set; }

        public string Signature
        {
            get { return _signature; }
            set
            {
                _signature = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }
        private string _signature;

        public FileInfo ContainingFile { get; set; }


        public Method(string type, string signature, FileInfo containingFile)
        {
            Type = type;
            Signature = signature;
            ContainingFile = containingFile;
        }
    }
}
