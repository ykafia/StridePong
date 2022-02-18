using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.Engine.Events;
using Stride.UI;
using Stride.UI.Controls;
using Stateless;
using Stride.Core;

namespace StridePong
{
    [DataContract]
    public class WaitAnimation : AsyncScript
    {
        
        private string animationName;
        private GameActions action;
        private AnimationComponent Cmp;

        public WaitAnimation(string name, GameActions a, AnimationComponent cmp = null)
        {
            animationName = name;
            action = a;
        }

        public override async Task Execute()
        {

            while(Game.IsRunning)
            {
                await Cmp.Ended(Cmp.PlayingAnimations.First(x => x.Name == animationName));
                UIManagerScript.UIEvents.Broadcast(new EventData(action));
                Entity.Remove(this);
            }
        }        
    }
}