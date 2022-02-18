namespace StridePong
{
    public abstract class EventDataBase{}
    public class EventData : EventDataBase
    {
        public GameActions Action;

        public EventData(GameActions action)
        {
            Action = action;
        }
    }
    public class ConfigEventData : EventDataBase
    {
        public GameConfig Config;

        public ConfigEventData(GameConfig gameConfig)
        {
            Config = gameConfig;
        }
    }
    public class WinEventData : EventDataBase
    {
        public Paddle Winner;

        public WinEventData(Paddle left)
        {
            Winner = left;
        }
    }
}