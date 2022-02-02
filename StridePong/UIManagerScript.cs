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

        public override async Task Execute()
        {
            Configure();
            while(Game.IsRunning)
            {
                var action = await receiver.ReceiveAsync();
                UIStates.Fire(action);
            }
        }

        private void Configure()
        {
            UIStates = new StateMachine<GameStates, GameActions>(GameStates.Menu);
            UIStates
                .Configure(GameStates.Menu)
                .OnEntry(t => MenuUICode())
                .Permit(GameActions.ShowStart,GameStates.Start);
            UIStates
                .Configure(GameStates.Start)
                .OnEntry(t => StartUICode())
                .Permit(GameActions.StartGame,GameStates.InGame);
            UIStates
                .Configure(GameStates.InGame)
                .OnEntry(t => UI.Page = InGamePage)
                .Permit(GameActions.EndGame,GameStates.Win);
            UIStates
                .Configure(GameStates.Win)
                .OnEntry(t => UI.Page = WinPage)
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

    }
}
