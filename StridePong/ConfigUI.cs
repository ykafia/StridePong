using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.UI.Controls;

namespace StridePong
{
    public class ConfigUI : SyncScript
    {
        // Declared public member fields and properties will show in the game studio

        private UIPage Page => Entity.Get<UIComponent>().Page;
        bool is2Player = false;
        private TextBlock PlayerNum => Page.RootElement.FindName("ConfigMenu").FindName("PlayerNumber") as TextBlock;
        bool isSoundOn = true;
        private TextBlock SoundConfig => Page.RootElement.FindName("ConfigMenu").FindName("Sound") as TextBlock;
        private TextBlock StartButton => Page.RootElement.FindName("ConfigMenu").FindName("PressText2") as TextBlock;
        private TextBlock QuitButton => Page.RootElement.FindName("ConfigMenu").FindName("Quit") as TextBlock;


        private int selected = 0;

        private List<TextBlock> selection = new();

        public override void Start()
        {
            selection.Add(PlayerNum);
            selection.Add(SoundConfig);
            selection.Add(StartButton);
            selection.Add(QuitButton);
            PlayerNum.Text = is2Player ? "< 2 Players" : "  1 Player   >";
            SoundConfig.Text = isSoundOn ? "  Sound : On  >" : "< Sound : Off  ";
        }

        public override void Update()
        {
            if(Input.IsKeyPressed(Keys.Down))
                selected = Mod(selected + 1, 4);
            else if(Input.IsKeyPressed(Keys.Up))
                selected = Mod(selected - 1, 4);
            
            selection.ForEach(x => {x.BackgroundColor = Color.Transparent; x.TextColor = Color.White;});
            selection[selected].BackgroundColor = Color.White;
            selection[selected].TextColor = Color.Black;

            if(selected == 0)
            {
                if(Input.IsKeyPressed(Keys.Right) && !is2Player)
                {
                    is2Player = true;
                    PlayerNum.Text = "< 2 Players  ";
                }
                if(Input.IsKeyPressed(Keys.Left) && is2Player)
                {
                    is2Player = false;
                    PlayerNum.Text = "  1 Player   >";
                }
            }
            if(selected == 1)
            {
                if(Input.IsKeyPressed(Keys.Right) && isSoundOn)
                {
                    isSoundOn = false;
                    SoundConfig.Text = "< Sound : Off  ";
                }
                if(Input.IsKeyPressed(Keys.Left) && !isSoundOn)
                {
                    isSoundOn = true;
                    SoundConfig.Text = "  Sound : On  >";
                }
            }
            if(selected == 2)
            {
                if(Input.IsKeyPressed(Keys.Enter))
                    UIManagerScript.ChangeUI.Broadcast(is2Player ? GameActions.StartGame2P : GameActions.StartGame1P);
            }
            if(selected == 3)
            {
                if(Input.IsKeyPressed(Keys.Enter))
                    (Game as Game).Exit();
            }
        }

        int Mod(int x, int m) {
            return (x%m + m)%m;
        }
    }
}
