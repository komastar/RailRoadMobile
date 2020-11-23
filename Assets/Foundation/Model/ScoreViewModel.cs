using System;

public class ScoreViewModel
{
    public int TotalScore { get; set; }
    public int NetworkScore { get; set; }
    public int RailScore { get; set; }
    public int RoadScore { get; set; }
    public int PenaltyScore { get; set; }

    public void Calculate()
    {
        TotalScore = NetworkScore + RailScore + RoadScore - PenaltyScore;
    }
}