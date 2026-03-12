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
        float playerRadius = 0.5f;
        float playerSize = 0.1f;

        List<HexBarrier> activeBarriers = new List<HexBarrier>();
        
        double timeCount = 3;
        int score = 0;

        GameState state = GameState.Menu;
        public void UpdateGame(double deltaTime)
        {
            if (state == GameState.inGame)
            {
                display.Update(deltaTime);
                timeCount += deltaTime;
                if (timeCount >= 2)
                {
                    timeCount = 0;
                    activeBarriers.Add(new HexBarrier());
                }


                foreach (var barrier in activeBarriers)
                {
                    barrier.Update(deltaTime);
                    if (barrier.CollidesWith(playerRot, playerRadius, playerSize, playerSize))
                    {
                        state = GameState.Loss;
                        activeBarriers.Clear();
                        break;
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
            else if (state == GameState.Menu)
            {
                display.DrawMenu();
                if (InputManager.IsKeyPressed(ConsoleKey.Enter))
                {
                    state = GameState.inGame;
                }
            }
            else if (state == GameState.Loss)
            {
                display.DrawLossMenu(score);
                if (InputManager.IsKeyPressed(ConsoleKey.Enter))
                {
                    state = GameState.Menu;
                    score = 0;
                }
            }
            else if (state == GameState.Victory)
            {
                display.DrawWinMenu();
                if (InputManager.IsKeyPressed(ConsoleKey.Enter))
                {
                    state = GameState.Menu;
                }
            }
        }


        
        Display display = new Display(50,50);
        public void DrawGame()
        {

            if (state == GameState.inGame)
            {
                for (int i = activeBarriers.Count - 1; i >= 0; i--)
                {
                    var barrier = activeBarriers[i];
                    barrier.Draw(display);
                    if (barrier.radius <= 0)
                    {
                        activeBarriers.Remove(barrier);
                        score++;
                    }
                }
                //draw player here
                Matrix4x4 rot = Matrix4x4.CreateRotationZ(playerRot);

                Vector4 a = Vector4.Transform(new Vector4(-playerSize / 2, playerRadius, 0.9f, 1), rot); //bottom left
                Vector4 b = Vector4.Transform(new Vector4(playerSize / 2, playerRadius, 0.9f, 1), rot); // bottom right
                Vector4 c = Vector4.Transform(new Vector4(-playerSize / 2, playerRadius + playerSize, 0.9f, 1), rot); //top left
                Vector4 d = Vector4.Transform(new Vector4(playerSize / 2, playerRadius + playerSize, 0.9f, 1), rot); //top right

                display.DrawSquare(c, d, a, b, ConsoleColor.Blue);


                display.DrawGameToConsole(score);
                display.Clear();
            }
        }


        enum GameState { Menu, inGame, Loss, Victory }
    }
}
