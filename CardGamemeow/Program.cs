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

            HexagonForce hexagonForce = new HexagonForce();

            Console.CursorVisible = false;
            //Console.ForegroundColor = ConsoleColor.Green;

            while (true)
            {
                double deltaTime = ((double)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000) - prevTime;

                if (deltaTime >= 0.09)
                {
                    prevTime += deltaTime;

                    InputManager.UpdateKey();
                    hexagonForce.UpdateGame(deltaTime);

                    hexagonForce.DrawGame();


                    

                }
            }
        }
    }
}
