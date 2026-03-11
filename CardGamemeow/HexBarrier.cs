using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace Hexagame
{
    internal class HexBarrier
    {
        public HexBarrier()
        {
            Random rand = new Random();
            for (int i = 0; i < SolidSides.Length; i++)
            {
                SolidSides[i] = rand.Next(0, 2) == 0 ? false : true;
            }
        }

        public float radius = 1.5f;
        float rotation = 0;
        float width = 0.2f;
        bool[] SolidSides = new bool[6];

        public bool CollidesWith(float rotation, float radius)
        {
            return true;
        }

        public void Update(double deltaTime)
        {
            radius -= 0.5f * (float)deltaTime;
            rotation += 2f * (float)deltaTime;
        }


        private (Vector4, ConsoleColor)[] GetVertices()
        {
            (Vector4, ConsoleColor)[] vertices = new (Vector4, ConsoleColor)[36];
           
            float currentRot = 0;
            for (int i = 0; i < SolidSides.Length; i++)
            {
                if (SolidSides[i])
                {
                    int vi = i * 6;
                    //Matrix4x4 rot = Matrix4x4.CreateRotationZ(rotation + currentRot);

                    float angle1 = rotation + i * (MathF.PI / 3);
                    float angle2 = rotation + (i + 1) * (MathF.PI / 3);

                    Vector4 a = new Vector4(MathF.Cos(angle1) * radius, MathF.Sin(angle1) * radius, 1, 1);
                    Vector4 b = new Vector4(MathF.Cos(angle2) * radius, MathF.Sin(angle2) * radius, 1, 1);

                    float outerR = radius + width;
                    Vector4 c = new Vector4(MathF.Cos(angle1) * outerR, MathF.Sin(angle1) * outerR, 1, 1);
                    Vector4 d = new Vector4(MathF.Cos(angle2) * outerR, MathF.Sin(angle2) * outerR, 1, 1);


                    vertices[vi] = (a, ConsoleColor.Magenta);
                    vertices[vi + 1] = (d, ConsoleColor.Magenta);
                    vertices[vi + 2] = (c, ConsoleColor.Magenta);
                    vertices[vi + 3] = (a, ConsoleColor.Magenta);
                    vertices[vi + 4] = (b, ConsoleColor.Magenta);
                    vertices[vi + 5] = (d, ConsoleColor.Magenta);
                    currentRot += 3.14159263f / 3;
                }
            }
            return vertices;
        }

        public void Draw(Display display)
        {
            (Vector4 vec, ConsoleColor col)[] vertices = GetVertices();

            for (int i = 0; i < vertices.Length; i += 3)
            {
                display.DrawTriangle(vertices[i..(i + 3)]);
            }


        }
    }
}
