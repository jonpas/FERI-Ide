using System.IO;

namespace Ide
{
    public class Method
    {
        public string Type { get; set; }

        public string Signature { get; set; }

        public FileInfo ContainingFile { get; set; }


        public Method(string type, string signature, FileInfo containingFile)
        {
            Type = type;
            Signature = signature;
            ContainingFile = containingFile;
        }
    }
}
