public class MapModel
{
    public int Id;
    public string Name;
    public float NodeSize;
    public GridInt MapSize;
    public NodeModel[] Nodes;

    public MapModel()
    {
        Name = "NewMap";
        NodeSize = 1f;
    }
}
