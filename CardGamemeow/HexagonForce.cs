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
        
        double timeCount = 0.1;
        public void UpdateGame(double deltaTime)
        {
            timeCount += deltaTime;
            if (timeCount > 0.1)
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


        
        Display display = new Display(20,20);
        public void DrawGame()
        {
            

            foreach (var barrier in activeBarriers)
            {
                barrier.Draw(display);
            }
            //draw player here

            display.DrawToConsole();
            display.Clear();
        }

    }
}
