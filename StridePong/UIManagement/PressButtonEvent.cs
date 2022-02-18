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
using Stride.Graphics;
using System.IO;

namespace StridePong
{
    public class PressButtonEvent : SyncScript
    {
        public Keys Key {get;set;} = Keys.None;
        public GameActions Action {get;set;}
        public override void Update()
        {
            if(Key == Keys.None){
                if(Input.HasPressedKeys)
                {
                    UIManagerScript.UIEvents.Broadcast(new EventData(Action));
                    Entity.Remove(this);
                }
            }
            else if(Input.IsKeyPressed(Key))
            {
                UIManagerScript.UIEvents.Broadcast(new EventData(Action));
                Entity.Remove(this);
            }
        }
    }
}