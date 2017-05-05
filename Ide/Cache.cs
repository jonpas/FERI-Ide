using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public List<OpenProject> OpenProjects { get; set; }


        private Cache() { }

        public Cache(ObservableCollection<Project> projects)
        {
            OpenProjects = new List<OpenProject>();
            foreach (var proj in projects)
            {
                OpenProjects.Add(new OpenProject(proj.Location, proj.ProjectFile));
            }
        }
    }

    public class OpenProject
    {
        public string Location { get; set; }

        public string ProjectFile { get; set; }

        [XmlIgnore]
        public string ProjectFileLocation
        {
            get { return Location + "/" + ProjectFile; }
        }


        public OpenProject() { }

        public OpenProject(string location, string projFile)
        {
            Location = location;
            ProjectFile = projFile;
        }
    }
}
