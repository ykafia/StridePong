using Stride.Engine;

namespace StridePong
{
    class StridePongApp
    {
        static void Main(string[] args)
        {
            using (var game = new Game())
            {
                game.WindowCreated += (s,e) => {game.Window.IsMouseVisible = false; game.Window.IsFullscreen = true;}; 
                game.Run();
            }
        }
    }
}
