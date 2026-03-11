using System;
using System.Numerics;

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
            rotVel = (rand.NextSingle() - 0.5f) * 3f;
        }

        public float radius = 3f;
        float rotVel = 0;
        float rotation = 0;
        float width = 0.2f;
        bool[] SolidSides = new bool[6];

        public bool CollidesWith(float objRotation, float objRadius, float objWidth, float objLength)
        {
            //llm: check player collison stuff
            float outerRadius = radius + width;

            // Player radial span is [objRadius, objRadius + objLength] (matches draw code)
            float playerInner = objRadius;
            float playerOuter = objRadius + objLength;

            // No radial overlap means no collision
            if (playerOuter < radius || playerInner > outerRadius)
                return false;

            // The player is positioned along +Y before rotation, so its actual
            // world angle is objRotation + PI/2 (barrier angles are from +X axis)
            float playerAngle = objRotation + MathF.PI / 2f;

            // Half angular width of the player at its radial distance
            float halfAngularWidth = MathF.Atan2(objWidth / 2f, objRadius);

            for (int i = 0; i < SolidSides.Length; i++)
            {
                if (!SolidSides[i])
                    continue;

                float sideStart = rotation + i * (MathF.PI / 3);
                float sideMid = sideStart + MathF.PI / 6f;
                float sideHalf = MathF.PI / 6f;

                // Shortest angular distance between player center and side center
                float diff = AngleDiff(playerAngle, sideMid);

                // Overlap if the distance between centers is less than sum of half-widths
                if (diff < sideHalf + halfAngularWidth)
                    return true;
            }

            return false;
        }

        // Returns the shortest unsigned angular distance in [0, PI]
        private static float AngleDiff(float a, float b)
        {
            float diff = (a - b) % (MathF.PI * 2);
            if (diff < 0) diff += MathF.PI * 2;
            if (diff > MathF.PI) diff = MathF.PI * 2 - diff;
            return diff;
        }

        public void Update(double deltaTime)
        {
            radius -= 0.5f * (float)deltaTime;
            rotation += rotVel * (float)deltaTime;
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
