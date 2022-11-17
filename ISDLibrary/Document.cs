using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISDLibrary;
public class Document
{
    public string Name { get; }
    public int Cluster { get; set; }

    public Document(string name, int cluster = 0)
    {
        Name = name;
        Cluster = cluster;
    }
}
