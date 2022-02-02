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

        private Entity GameEntity;
        private Entity BallEntity;

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
            Entity.Get<KeepingScore>().PaddleLeft = prefab.Entities.First(x => x.Name.Contains("Left"));
            Entity.Get<KeepingScore>().PaddleRight = prefab.Entities.First(x => x.Name.Contains("Right"));
            BallEntity = GameEntity.GetChildren().First(x => x.Get<BallCollisionCheck>() != null);
            UIStates = new StateMachine<GameStates, GameActions>(GameStates.Menu);
            UIStates
                .Configure(GameStates.Menu)
                .OnEntry(t => MenuUICode())
                .Permit(GameActions.ShowStart,GameStates.Start)
                .Permit(GameActions.StartGame, GameStates.InGame);
            UIStates
                .Configure(GameStates.Start)
                .OnEntry(t => StartUICode())
                .Permit(GameActions.StartGame,GameStates.InGame);
            UIStates
                .Configure(GameStates.InGame)
                .OnEntry(t => BeginGame())
                .Permit(GameActions.EndGame,GameStates.Win);
            UIStates
                .Configure(GameStates.Win)
                .OnEntry(t => EndGame())
                .Permit(GameActions.BackToMenu,GameStates.Menu);
        }

        private void StartUICode()
        {
            UI.Page = StartPage;
        }

        private void MenuUICode()
        {
            UI.Page = MenuPage;
        }
        private void BeginGame()
        {
            UI.Page = InGamePage;
            SceneSystem.SceneInstance.RootScene.Entities.Add(GameEntity);
        }
        private void EndGame()
        {
            UI.Page = WinPage;
            //var scene = SceneSystem.SceneInstance.RootScene;
            SceneSystem.SceneInstance.RootScene.Entities.Remove(GameEntity);

        }

    }
}
