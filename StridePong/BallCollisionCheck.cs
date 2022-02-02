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
    public class BallCollisionCheck : AsyncScript
    {
        // Declared public member fields and properties will show in the game studio

        public StaticColliderComponent TriggerRight;
        public StaticColliderComponent TriggerLeft;

        public Entity PaddleLeft;
        public Entity PaddleRight;
        public Entity Top;
        public Entity Bottom;
        
        private RigidbodyComponent Left => PaddleLeft.GetChild(0).Get<RigidbodyComponent>();
        private RigidbodyComponent Right => PaddleRight.GetChild(0).Get<RigidbodyComponent>();
        private StaticColliderComponent TopB => Top.Get<StaticColliderComponent>();
        private StaticColliderComponent BottomB => Bottom.Get<StaticColliderComponent>();
        

        private RigidbodyComponent Ball => Entity.Get<RigidbodyComponent>();

        public override async Task Execute()
        {
            Entity.Add(new InitialBallVelocity());
            var body = Entity.Get<RigidbodyComponent>();
            while(Game.IsRunning)
            {
                // Do stuff every new frame
                var collision = await body.NewCollision();
                if(collision.ColliderA == TriggerRight || collision.ColliderB == TriggerRight)
                {
                    Entity.Transform.Position = new(0);
                    Entity.Get<InitialBallVelocity>().Start();
                    KeepingScore.PointTo.Broadcast(PlayerScore.OneToLeft);
                } 
                else if(collision.ColliderA == TriggerLeft || collision.ColliderB == TriggerLeft)
                {
                    Entity.Transform.Position = new(0);
                    Entity.Get<InitialBallVelocity>().Start();
                    KeepingScore.PointTo.Broadcast(PlayerScore.OneToRight);
                }
                else if(collision.ColliderA == Left || collision.ColliderB == Left)
                {
                    var deltaPos = PaddleLeft.Transform.Position.Y - Entity.Transform.Position.Y;
                    Entity.Get<InitialBallVelocity>().Reflect(Border.Left,deltaPos);
                }
                else if(collision.ColliderA == Right || collision.ColliderB == Right)
                {
                    var deltaPos = PaddleRight.Transform.Position.Y - Entity.Transform.Position.Y;
                    Entity.Get<InitialBallVelocity>().Reflect(Border.Right,deltaPos);
                }
                else if(collision.ColliderA == TopB || collision.ColliderB == TopB)
                {
                    // var deltaPos = Entity.Transform.Position.Y - Top.Transform.WorldMatrix.TranslationVector.Y;
                    Entity.Get<InitialBallVelocity>().Reflect(Border.Top,0);
                }
                else if(collision.ColliderA == BottomB || collision.ColliderB == BottomB)
                {
                    // var deltaPos = Entity.Transform.Position.Y - Bottom.Transform.WorldMatrix.TranslationVector.Y;
                    Entity.Get<InitialBallVelocity>().Reflect(Border.Down,0);
                }
                
            }
        }
    }
}
