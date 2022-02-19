using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.Physics;
using Stride.Core;

namespace StridePong
{
    [DataContract]
    public class BallVelocity : SyncScript
    {
        // Declared public member fields and properties will show in the game studio
        public float Speed {get;set;} = 10;
        
        private RigidbodyComponent Ball => Entity.Get<RigidbodyComponent>();
        [DataMemberIgnore]
        public Vector3 Velocity {get;set;} = new(0);
        private readonly Random rand = new();

        private Border[] walls = new Border[]{Border.Top,Border.Down};
        private Border[] paddles = new Border[]{Border.Right,Border.Left};
        public override void Start()
        {
            // Initialization of the script.
            Entity.Transform.Position = new(0);
            Velocity = Vector3.UnitX * Speed * (rand.Next(0,2) * 2 -1);
        }

        public void Reflect(Border border, float deltaPos)
        {
            var normal = border switch 
            {
                Border.Top => -Vector2.UnitY,
                Border.Down => Vector2.UnitY,
                Border.Left => -Vector2.UnitX,
                Border.Right => Vector2.UnitX,
                _ => throw new Exception()
            };
            Velocity = 
                Vector3.Normalize(new(Vector2.Reflect(Ball.LinearVelocity.XY(),normal),0))* 
                Speed + Vector3.UnitY * -deltaPos * 6;
        }

        public override void Update()
        {
            Entity.Transform.Position += Velocity * (float)Game.UpdateTime.Elapsed.TotalSeconds;
            var pos = Entity.Transform.Position;
            if(pos.X > 14 || pos.X < -14 || pos.Y > 6 || pos.Y < -6)
            {
                Entity.Transform.Position = new(0); 
                Start();
            }

        }
    }
}
