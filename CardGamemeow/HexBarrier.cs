using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Hexagame
{
    internal class HexBarrier
    {
        public HexBarrier() { }

        float radius = 0.2f;
        float rotation = 0;
        float width = 0.5f;
        bool[] SolidSides = new bool[6];

        public bool CollidesWith(float rotation, float radius)
        {
            return true;
        }

        public void Update(double deltaTime)
        {
            radius += 0.03f;
            rotation += 0.2f;
        }


        private (Vector4, ConsoleColor)[] GetVertices()
        {
            (Vector4, ConsoleColor)[] vertices = new (Vector4, ConsoleColor)[36];
           
            float currentRot = 0;
            for (int i = 0; i < SolidSides.Length; i++)
            {
                int vi = i * 6;
                Matrix4x4 rot = Matrix4x4.CreateRotationZ(rotation + currentRot);
                //should be clockwise i think
                Vector4 a = Vector4.Transform(new Vector4(-radius / 2, radius, 0, 1), rot); //bottom left
                Vector4 b = Vector4.Transform(new Vector4(radius / 2, radius, 1, 1), rot); // bottom right
                Vector4 c = Vector4.Transform(new Vector4((-radius - width) / 2, radius + width, 1, 1), rot); //top left
                Vector4 d = Vector4.Transform(new Vector4((radius + width) / 2, radius + width, 1, 1), rot); //top right


                vertices[vi] = (a, ConsoleColor.Blue);
                vertices[vi + 1] = (c, ConsoleColor.Blue);
                vertices[vi + 2] = (d, ConsoleColor.Blue);
                vertices[vi + 3] = (b, ConsoleColor.Magenta);
                vertices[vi + 4] = (c, ConsoleColor.Magenta);
                vertices[vi + 5] = (d, ConsoleColor.Magenta);
                currentRot += 3.14159263f / 3;
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
