using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.Core;

namespace StridePong
{
    public class LeftPaddleAI : SyncScript
    {
        // Declared public member fields and properties will show in the game studio
        [DataMember(10)]
        public Entity Ball;
        
        [DataMember(20)]
        public float Speed = 4f;
 
        public override void Update()
        {
            var pos = Entity.Transform.Position;
            var ballPos = Ball.Transform.Position;

            var delta = ballPos - pos;

            if(delta.Y > 0.2 && Entity.Transform.Position.Y < 4)
                Entity.Transform.Position.Y += Speed * (float)Game.UpdateTime.Elapsed.TotalSeconds;
            else if(delta.Y < -0.2 && Entity.Transform.Position.Y > -4)
                Entity.Transform.Position.Y -= Speed * (float)Game.UpdateTime.Elapsed.TotalSeconds;
        }
    }
}
