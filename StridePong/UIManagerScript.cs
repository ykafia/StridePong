using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.Engine.Events;
using Stride.Core;
using Stateless;
using Stride.UI.Controls;

namespace StridePong
{
    public class UIManagerScript : AsyncScript
    {
        
        #region static UI events
        public static EventKey<GameActions> ChangeUI = new("Global");
        public static EventKey<GameConfig> ConfigGameEvent = new("Global");
        public static EventKey<Paddle> WinGameEvent = new("Global");

        #endregion

        #region Data Members

        [DataMember(10)]
        public UIPage MenuPage;
        [DataMember(20)]
        public UIPage StartPage;
        [DataMember(30)]
        public UIPage InGamePage;
        [DataMember(40)]
        public UIPage WinPage;

        #endregion
        
        #region EventReceivers
        private readonly EventReceiver<GameActions> uiReceiver = new(ChangeUI);
        private readonly EventReceiver<Paddle> winReceiver = new(WinGameEvent);
        private readonly EventReceiver<GameConfig> configReceiver = new(ConfigGameEvent);
        #endregion

        #region StateMachine attributes
        [DataMemberIgnore]
        public StateMachine<GameStates,GameActions> UIStates;

        private StateMachine<GameStates,GameActions>.TriggerWithParameters<Paddle> winParams;
        private StateMachine<GameStates,GameActions>.TriggerWithParameters<GameConfig> configParams;
        #endregion

        #region Utility fields
        
        [DataMemberIgnore]
        private UIComponent UI => Entity.Get<UIComponent>();

        private TextBlock WinMessage => WinPage.RootElement.FindName("WinMessage") as TextBlock;

        private Entity paddleLeft;
        private Entity GameEntity;
        private Entity BallEntity;

        private PressButtonEvent PressStart = new PressButtonEvent{Key = Keys.Space, Action = GameActions.ShowConfig};
        private PressButtonEvent PressReturn = new PressButtonEvent{Key = Keys.Space, Action = GameActions.BackToMenu};
        private PressButtonEvent PressAnyReturn = new PressButtonEvent{Key = Keys.None, Action = GameActions.BackToMenu};
        #endregion


        public override async Task Execute()
        {
            await Script.NextFrame();
            Configure();
            while(Game.IsRunning)
            {
                var (win,config,ui) = 
                    (winReceiver.ReceiveAsync(),
                    configReceiver.ReceiveAsync(),
                    uiReceiver.ReceiveAsync());
                
                Task completed = await Task.WhenAny(
                    win,config,ui
                );
                if(completed == win)
                {
                    var res = win.Result;
                    UIStates.Fire(winParams,res);
                }
                else if(completed == config)
                {
                    var res = config.Result;
                    UIStates.Fire(configParams,res);
                }
                else if(completed == ui)
                {
                    var res = ui.Result;
                    UIStates.Fire(res);
                }
                
                
            }
        }

        private void Configure()
        {
            UI.Page = MenuPage;
            GameEntity = new Entity(Vector3.Zero,"GameEntity");
            var prefab = Content.Load<Prefab>("PongPrefab");
            prefab.Instantiate().ForEach(GameEntity.AddChild);
            paddleLeft = GameEntity.GetChildren().First(x => x.Name.Contains("Left"));
            Entity.Get<KeepingScore>().PaddleLeft = paddleLeft;
            Entity.Get<KeepingScore>().PaddleRight = GameEntity.GetChildren().First(x => x.Name.Contains("Right"));
            BallEntity = GameEntity.GetChildren().First(x => x.Get<BallCollisionCheck>() != null);
            UIStates = new StateMachine<GameStates, GameActions>(GameStates.Menu);
            Entity.Add(PressStart);


            configParams = UIStates.SetTriggerParameters<GameConfig>(GameActions.StartGame);
            winParams = UIStates.SetTriggerParameters<Paddle>(GameActions.EndGame);


            UIStates
                .Configure(GameStates.Menu)
                .OnEntry(t => MenuUICode())
                .Permit(GameActions.ShowConfig,GameStates.Config);
            UIStates
                .Configure(GameStates.Config)
                .OnEntry(t => StartUICode())
                .Permit(GameActions.StartGame,GameStates.InGame);
            UIStates
                .Configure(GameStates.InGame)
                .OnEntryFrom(configParams, config => BeginGame(config))
                .Permit(GameActions.EndGame,GameStates.Win);
            UIStates
                .Configure(GameStates.Win)
                .OnEntryFrom(winParams, winner => EndGame(winner))
                .Permit(GameActions.BackToMenu,GameStates.Menu);
        }

        private void StartUICode()
        {
            Entity.Remove(PressStart);
            MenuPage.RootElement.FindName("Config").Visibility = Stride.UI.Visibility.Visible;
            MenuPage.RootElement.FindName("PressText").Visibility = Stride.UI.Visibility.Hidden;
            Entity.Add(new ConfigUI());
        }

        private void MenuUICode()
        {
            UI.Page = MenuPage;
            Entity.Remove(PressAnyReturn);
            Entity.Add(PressStart);
        }
        private void BeginGame(GameConfig config)
        {
            if(config.nbPlayer == GameType.Game1P)
            {
                paddleLeft.Remove<MovePaddle>();
                if(paddleLeft.Get<LeftPaddleAI>() == null)
                    paddleLeft.Add(new LeftPaddleAI{Ball = BallEntity});
            }
            else if(config.nbPlayer == GameType.Game1P)
            {
                paddleLeft.Remove<LeftPaddleAI>();
                if(paddleLeft.Get<MovePaddle>() == null)
                    paddleLeft.Add(new MovePaddle{Down = Keys.S, Up = Keys.Z, Speed = 5});
            }
            Entity.Get<KeepingScore>().ScoreMax = config.scoreMax;
            MenuPage.RootElement.FindName("Config").Visibility = Stride.UI.Visibility.Hidden;
            MenuPage.RootElement.FindName("PressText").Visibility = Stride.UI.Visibility.Visible;
            Entity.Remove<ConfigUI>();
            UI.Page = InGamePage;
            SceneSystem.SceneInstance.RootScene.Entities.Add(GameEntity);
        }
        private void EndGame(Paddle player)
        {
            Entity.Add(PressAnyReturn);
            UI.Page = WinPage;
            WinMessage.Text = $"Player {player} wins";
            foreach(var e in GameEntity.GetChildren().Where(x => x.Get<MovePaddle>() != null))
                e.Transform.Position.Y = 0;
            SceneSystem.SceneInstance.RootScene.Entities.Remove(GameEntity);
        }

    }
}
