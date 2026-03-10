using FunGameWahoo;
using System.Text;
using System.Xml.Linq;

namespace CardGamemeow
{
    internal class Program
    {
        static void Main(string[] args)
        {
            double prevTime = (double)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000;

            double count = 0;

            HexagonForce hexagonForce = new HexagonForce();

            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.Green;

            while (true)
            {
                double deltaTime = ((double)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000) - prevTime;
                prevTime += deltaTime;

                count += deltaTime;
                if (count >= 0.15)
                {
                    count = 0;

                    InputManager.UpdateKey();
                    hexagonForce.UpdateGame(deltaTime);

                    hexagonForce.DrawGame();


                    

                }
            }
        }
    }
}
