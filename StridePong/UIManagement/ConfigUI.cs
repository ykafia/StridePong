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
        GameType gameType = GameType.Game1P;
        private TextBlock PlayerNum => Page.RootElement.FindName("ConfigMenu").FindName("PlayerNumber") as TextBlock;
        bool isSoundOn = true;
        int scoreMax = 3;
        private TextBlock ScoreMaxConfig => Page.RootElement.FindName("ConfigMenu").FindName("ScoreMax") as TextBlock;
        private TextBlock StartButton => Page.RootElement.FindName("ConfigMenu").FindName("PressText2") as TextBlock;
        private TextBlock QuitButton => Page.RootElement.FindName("ConfigMenu").FindName("Quit") as TextBlock;


        private int selected = 0;

        private List<TextBlock> selection = new();

        public override void Start()
        {
            selection.Add(PlayerNum);
            selection.Add(ScoreMaxConfig);
            selection.Add(StartButton);
            selection.Add(QuitButton);
            PlayerNum.Text = gameType == GameType.Game2P ? "< 2 Players" : "  1 Player   >";
            // ScoreMaxConfig.Text = isSoundOn ? "  Sound : On  >" : "< Sound : Off  ";
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
                if(Input.IsKeyPressed(Keys.Right) && gameType == GameType.Game1P)
                {
                    gameType = GameType.Game2P;
                    PlayerNum.Text = "< 2 Players  ";
                }
                if(Input.IsKeyPressed(Keys.Left) && gameType == GameType.Game2P)
                {
                    gameType = GameType.Game1P;
                    PlayerNum.Text = "  1 Player   >";
                }
            }
            if(selected == 1)
            {
                if(Input.IsKeyPressed(Keys.Right) && scoreMax < 9)
                {
                    scoreMax += 3;
                }
                if(Input.IsKeyPressed(Keys.Left) && scoreMax > 3)
                {
                    scoreMax -= 3;
                }
                ScoreMaxConfig.Text = $"Score max : {(scoreMax > 3 ? '<' : ' ')} {scoreMax} {(scoreMax < 9 ? '>' : ' ')}";
            }
            if(selected == 2)
            {
                if(Input.IsKeyPressed(Keys.Enter))
                {
                    UIManagerScript.UIEvents.Broadcast(
                            new ConfigEventData(
                                new GameConfig{
                                    nbPlayer = gameType, 
                                    scoreMax = scoreMax
                                }
                            )
                        );
                }
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
