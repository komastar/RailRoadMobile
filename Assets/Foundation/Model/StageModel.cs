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
    [JsonIgnore]
    public int Id { get; set; }
    public string Name { get; set; }
    public int Round { get; set; }
    public int MapId { get; set; }
    public string MapName { get; set; }
    public int[] Dice { get; set; }
}