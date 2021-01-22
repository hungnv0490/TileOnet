
public enum GameState
{
    None,
    Play,
    Lose,
    Win
}

public enum MoveTypeAtStart
{
    None,
    Bottom,
    Top,
    Right,
    Left,
    BottomLeft,
    BottomRight,
    TopLeft,
    TopRight
}


public enum MoveTypeInGame
{
    None,
    BottomTop,
    TopBottom,
    RightLeft,
    LeftRight,
    ColCenter,
    RowCenter
}