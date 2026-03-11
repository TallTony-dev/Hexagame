using FunGameWahoo;
using Hexagame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CardGamemeow
{
    internal class HexagonForce
    {
        public HexagonForce() { }

        float playerRot = 0;
        float playerRadius = 0.4f;
        float playerSize = 0.15f;

        List<HexBarrier> activeBarriers = new List<HexBarrier>();
        
        double timeCount = 3;
        public void UpdateGame(double deltaTime)
        {
            display.Update(deltaTime);
            timeCount += deltaTime;
            if (timeCount >= 3)
            {
                timeCount = 0;
                activeBarriers.Add(new HexBarrier());
            }

            bool isLost = false;
            foreach (var barrier in activeBarriers)
            {
                barrier.Update(deltaTime);
                if (barrier.CollidesWith(playerRot, playerRadius))
                {
                    isLost = true;
                }
            }

            if (InputManager.IsKeyDown(ConsoleKey.LeftArrow))
            {
                playerRot += 0.4f;
            }
            if (InputManager.IsKeyDown(ConsoleKey.RightArrow))
            {
                playerRot -= 0.4f;
            }

        }


        
        Display display = new Display(50,50);
        public void DrawGame()
        {
            

            for (int i = activeBarriers.Count - 1; i >= 0; i--)
            {
                var barrier = activeBarriers[i];
                barrier.Draw(display);
                if (barrier.radius <= 0)
                {
                    activeBarriers.Remove(barrier);
                }
            }
            //draw player here
            Matrix4x4 rot = Matrix4x4.CreateRotationZ(playerRot);

            Vector4 a = Vector4.Transform(new Vector4(-playerSize / 2, playerRadius, 0.9f, 1), rot); //bottom left
            Vector4 b = Vector4.Transform(new Vector4(playerSize / 2, playerRadius, 0.9f, 1), rot); // bottom right
            Vector4 c = Vector4.Transform(new Vector4(-playerSize / 2, playerRadius + playerSize, 0.9f, 1), rot); //top left
            Vector4 d = Vector4.Transform(new Vector4(playerSize / 2, playerRadius + playerSize, 0.9f, 1), rot); //top right

            display.DrawSquare(c, d, a, b, ConsoleColor.Blue);


            display.DrawToConsole();
            display.Clear();
        }

    }
}
