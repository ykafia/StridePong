using Stride.Engine;

namespace StridePong
{
    class StridePongApp
    {
        static void Main(string[] args)
        {
            using (var game = new Game())
            {
                game.Run();
            }
        }
    }
}
