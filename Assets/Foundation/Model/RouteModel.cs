public class RouteModel : IActor
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsRotate { get; set; }
    public bool IsFlip { get; set; }
    public bool IsTransfer { get; set; }
    public EJointType[] Joints { get; set; }
}