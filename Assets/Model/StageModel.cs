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
    public int Round { get; set; }
    public int MapId { get; set; }
    [JsonIgnore]
    public string MapName { get; set; }
    public int[] Dice { get; set; }
}