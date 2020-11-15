using Newtonsoft.Json;

public class StageListModel : IActor
{
    public int Id { get; set; }
    public string Name { get; set; }
    [JsonIgnore]
    public StageModel Stage { get; set; }
}

public class StageModel
{
    public string Name { get; set; }
    public string MapName { get; set; }
}