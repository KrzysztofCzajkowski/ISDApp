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
