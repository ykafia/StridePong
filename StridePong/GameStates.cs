namespace StridePong
{
    public struct GameConfig
    {
        public GameType nbPlayer;
        public int scoreMax;
    }

    public enum GameType
    {
        Game1P,
        Game2P
    }

    public enum GameStates 
    {
        Menu,
        Config,
        InGame,
        Win
    }
    public enum GameActions 
    {
        ShowConfig,
        StartGame,
        EndGame,
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