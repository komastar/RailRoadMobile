public enum JointType
{
    None = 0,
    Wall,
    Rail,
    Road,
    Block,
    Count
}

public enum GamePhaseState
{
    None = 0,
    Start,
    Ready,
    Draw,
    Build,
    End,
    Count
}

public enum TileState
{
    Empty,
    Ready,
    Build,
    Fix
}

public enum WayType
{
    Empty = 0,
    Rail_I,
    Rail_L,
    Rail_T,
    Rail_T_To_Road_I,
    Rail_X,
    RailToRoad_I,
    RailToRoad_L,
    Road_And_Rail_X,
    Road_I,
    Road_L,
    Road_L_Rail_L,
    Road_T,
    Road_T_To_Rail_I,
    Road_X,
    RoadToRail_X,
    Count,
    Road_Start,
    Rail_Start
}

public enum Direction
{
    Up = 0,
    Left,
    Down,
    Right
}
