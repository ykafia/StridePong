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

// namespace StridePong
// {
//     public class PressStartEvent : SyncScript
//     {
//         public override void Update()
//         {
//             if(Input.DownKeys.Contains(Keys.Space))
//                 UIManagerScript.UIEvents.Broadcast(GameActions.StartGame);
//         }
//     }
// }