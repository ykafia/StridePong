// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using Stride.Core.Mathematics;
// using Stride.Input;
// using Stride.Engine;
// using Stride.Engine.Events;
// using Stride.UI;
// using Stride.UI.Controls;
// using Stateless;
// using Stride.Core;

// namespace StridePong
// {
//     [DataContract]
//     public class AnimationEvent : AsyncScript
//     {

//         private AnimationComponent Cmp => Entity.Get<AnimationComponent>();
//         public override async Task Execute()
//         {

//             while(Game.IsRunning)
//             {
//                 await Cmp.Ended(Cmp.PlayingAnimations[0]);
//                 UIManagerScript.UIEvents.Broadcast(GameActions.StartGame);
//             }
//         }        
//     }
// }