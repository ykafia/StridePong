namespace StridePong
{
    public enum GameStates 
    {
        Menu,
        Config,
        InGame,
        WinLeft,
        WinRight
    }
    public enum GameActions 
    {
        ShowConfig,
        StartGame2P,
        StartGame1P,
        EndGameLeft,
        EndGameRight,
        BackToMenu
    }
    public enum Border
    {
        Top,
        Down,
        Left,
        Right
    }
}