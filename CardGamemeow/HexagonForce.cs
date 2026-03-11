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
        float playerRadius = 2;

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

        }


        
        Display display = new Display(40,40);
        public void DrawGame()
        {
            

            foreach (var barrier in activeBarriers)
            {
                barrier.Draw(display);
            }
            //draw player here
            Matrix4x4 rot = Matrix4x4.CreateRotationZ(playerRot);

            Vector4 a = Vector4.Transform(new Vector4(-playerRadius / 2, playerRadius, 0, 1), rot); //bottom left
            Vector4 b = Vector4.Transform(new Vector4(playerRadius / 2, playerRadius, 1, 1), rot); // bottom right
            Vector4 c = Vector4.Transform(new Vector4((-playerRadius) / 2, playerRadius, 1, 1), rot); //top left
            Vector4 d = Vector4.Transform(new Vector4((playerRadius) / 2, playerRadius, 1, 1), rot); //top right

            display.DrawSquare(c, d, a, b);


            display.DrawToConsole();
            display.Clear();
        }

    }
}
