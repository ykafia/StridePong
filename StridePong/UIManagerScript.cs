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
        // Declared public member fields and properties will show in the game studio
        public static EventKey<GameActions> ChangeUI = new("Global");

        [DataMember(10)]
        public UIPage MenuPage;
        [DataMember(20)]
        public UIPage StartPage;
        [DataMember(30)]
        public UIPage InGamePage;
        [DataMember(40)]
        public UIPage WinPage;
        

        [DataMemberIgnore]
        private EventReceiver<GameActions> receiver = new(ChangeUI); 

        [DataMemberIgnore]
        public StateMachine<GameStates,GameActions> UIStates;

        [DataMemberIgnore]
        private UIComponent UI => Entity.Get<UIComponent>();

        private TextBlock WinMessage => WinPage.RootElement.FindName("WinMessage") as TextBlock;

        private Entity paddleLeft;
        private Entity GameEntity;
        private Entity BallEntity;

        PressButtonEvent PressStart = new PressButtonEvent{Key = Keys.Space, Action = GameActions.ShowConfig};
        PressButtonEvent PressReturn = new PressButtonEvent{Key = Keys.Space, Action = GameActions.BackToMenu};
        PressButtonEvent PressAnyReturn = new PressButtonEvent{Key = Keys.None, Action = GameActions.BackToMenu};

        public override async Task Execute()
        {
            await Script.NextFrame();
            Configure();
            while(Game.IsRunning)
            {
                var action = await receiver.ReceiveAsync();
                UIStates.Fire(action);
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
            UIStates
                .Configure(GameStates.Menu)
                .OnEntry(t => MenuUICode())
                .Permit(GameActions.ShowConfig,GameStates.Config);
                // .Permit(GameActions.StartGame, GameStates.InGame);
            UIStates
                .Configure(GameStates.Config)
                .OnEntry(t => StartUICode())
                .Permit(GameActions.StartGame1P,GameStates.InGame)
                .Permit(GameActions.StartGame2P,GameStates.InGame);
                
            UIStates
                .Configure(GameStates.InGame)
                .OnEntry(t => BeginGame(t))
                .Permit(GameActions.EndGameLeft,GameStates.WinLeft)
                .Permit(GameActions.EndGameRight,GameStates.WinRight);
            UIStates
                .Configure(GameStates.WinLeft)
                .OnEntry(t => EndGame(1))
                .Permit(GameActions.BackToMenu,GameStates.Menu);
            UIStates
                .Configure(GameStates.WinRight)
                .OnEntry(t => EndGame(2))
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
        private void BeginGame()
        {
            MenuPage.RootElement.FindName("Config").Visibility = Stride.UI.Visibility.Hidden;
            MenuPage.RootElement.FindName("PressText").Visibility = Stride.UI.Visibility.Visible;
            Entity.Remove<ConfigUI>();
            UI.Page = InGamePage;
            SceneSystem.SceneInstance.RootScene.Entities.Add(GameEntity);
        }
        private void BeginGame(StateMachine<GameStates,GameActions>.Transition t)
        {
            switch(t.Trigger)
            {
                case GameActions.StartGame1P :
                    paddleLeft.Remove<MovePaddle>();
                    if(paddleLeft.Get<LeftPaddleAI>() == null)
                        paddleLeft.Add(new LeftPaddleAI{Ball = BallEntity});
                    break;
                case GameActions.StartGame2P :
                    paddleLeft.Remove<LeftPaddleAI>();
                    if(paddleLeft.Get<MovePaddle>() == null)
                        paddleLeft.Add(new MovePaddle{Down = Keys.S, Up = Keys.Z, Speed = 5});
                    break;
            }
            MenuPage.RootElement.FindName("Config").Visibility = Stride.UI.Visibility.Hidden;
            MenuPage.RootElement.FindName("PressText").Visibility = Stride.UI.Visibility.Visible;
            Entity.Remove<ConfigUI>();
            UI.Page = InGamePage;
            SceneSystem.SceneInstance.RootScene.Entities.Add(GameEntity);
        }
        private void EndGame(int player)
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
