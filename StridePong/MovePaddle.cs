using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.Physics;

namespace StridePong
{
    public class MovePaddle : SyncScript
    {
        // Declared public member fields and properties will show in the game studio
        
        public Keys Up = Keys.Up;
        public Keys Down = Keys.Down;

        public float Speed = 5;
        

        public override void Update()
        {
            if(Input.IsKeyDown(Up) && Entity.Transform.Position.Y < 4)
            {
                Entity.Transform.Position.Y += Speed * (float)Game.UpdateTime.Elapsed.TotalSeconds;
            }
            if(Input.IsKeyDown(Down) && Entity.Transform.Position.Y > -4)
            {
                Entity.Transform.Position.Y -= Speed * (float)Game.UpdateTime.Elapsed.TotalSeconds;
            }
        }
    }
}
