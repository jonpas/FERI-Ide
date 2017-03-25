using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace Ide
{
    public class Cache
    {
        public Cache()
        {
            WordWrap = TextWrapping.NoWrap;
        }

        public Cache(TextWrapping wordWrap)
        {
            WordWrap = wordWrap;
        }

        //public TreeView ProjectTree { get; set; }
        //public ListView MethodList { get; set; }

        public TextWrapping WordWrap { get; set; }
    }
}
