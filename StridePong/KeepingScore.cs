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

namespace StridePong
{
    public enum Paddle 
    {
        Left,
        Right
    }
    public enum PlayerScore
    {
        OneToLeft,
        OneToRight,
        Reset
    }
    public class KeepingScore : AsyncScript
    {
        // Declared public member fields and properties will show in the game studio
        
        public static EventKey<PlayerScore> PointTo = new ("General");
        private EventReceiver<PlayerScore> NewPoint = new(PointTo);
        public Entity PaddleRight;
        public Entity PaddleLeft;


        private TextBlock TRight => Entity.Get<UIComponent>().Page.RootElement.FindName("ScoreRight") as TextBlock;
        private TextBlock TLeft => Entity.Get<UIComponent>().Page.RootElement.FindName("ScoreLeft") as TextBlock; 
        private uint scoreLeft = 0;
        private uint scoreRight = 0;

        public override async Task Execute()
        {
            
            TRight.Text = "0";
            TLeft.Text = "0";
            while(Game.IsRunning)
            {
                
                // Do stuff every new frame
                var paddle = await NewPoint.ReceiveAsync();
                if(paddle == PlayerScore.OneToLeft)
                {
                    scoreLeft+=1;
                    TLeft.Text = scoreLeft.ToString();
                }
                else if(paddle == PlayerScore.OneToRight)
                {
                    scoreRight+=1;
                    TRight.Text = scoreRight.ToString();
                }
                else if(paddle == PlayerScore.Reset)
                {
                    scoreRight = 0;
                    scoreLeft = 0;
                    TRight.Text = scoreRight.ToString();
                    TLeft.Text = scoreLeft.ToString();
                }

                if(scoreLeft >= 3 || scoreRight >= 3 )
                {
                    UIManagerScript.ChangeUI.Broadcast(scoreLeft>=3?GameActions.EndGameLeft:GameActions.EndGameRight);
                    scoreRight = 0;
                    scoreLeft = 0;
                    TRight.Text = scoreRight.ToString();
                    TLeft.Text = scoreLeft.ToString();
                }
            }
        }   
    }
}
