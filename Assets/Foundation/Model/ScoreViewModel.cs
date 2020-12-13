using System;

public class ScoreViewModel
{
    public int TotalScore { get; set; }
    public int NetworkScore { get; set; }
    public int RailScore { get; set; }
    public int RoadScore { get; set; }
    public int PenaltyScore { get; set; }
    public int ConstructFailScore { get; set; }

    public void Init()
    {
        TotalScore = 0;
        NetworkScore = 0;
        RailScore = 0;
        RoadScore = 0;
        PenaltyScore = 0;
        ConstructFailScore = 0;
    }

    public void Calculate()
    {
        TotalScore = NetworkScore + RailScore + RoadScore - PenaltyScore - ConstructFailScore;
        TotalScore = TotalScore < 0 ? 0 : TotalScore;
    }
}