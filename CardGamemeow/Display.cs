using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace Hexagame
{
    internal class Display
    {
        const string ESC = "\e";
        const string SUFFIX = "m";
        const string SEPARATOR = ";";
        const string RESET = ESC + "[0" + SUFFIX;
        private static readonly Dictionary<ConsoleColor, string> ForegroundMap = new Dictionary<ConsoleColor, string>
        {
            { ConsoleColor.Black, "30" },
            { ConsoleColor.DarkRed, "31" },
            { ConsoleColor.DarkGreen, "32" },
            { ConsoleColor.DarkYellow, "33" },
            { ConsoleColor.DarkBlue, "34" },
            { ConsoleColor.DarkMagenta, "35" },
            { ConsoleColor.DarkCyan, "36" },
            { ConsoleColor.Gray, "37" },
            { ConsoleColor.DarkGray, "90" },
            { ConsoleColor.Red, "91" },
            { ConsoleColor.Green, "92" },
            { ConsoleColor.Yellow, "93" },
            { ConsoleColor.Blue, "94" },
            { ConsoleColor.Magenta, "95" },
            { ConsoleColor.Cyan, "96" },
            { ConsoleColor.White, "97" }
        };
        public static string GetAnsiForegroundColor(ConsoleColor color)
        {
            if (ForegroundMap.ContainsKey(color))
            {
                return $"{ESC}[{ForegroundMap[color]}{SUFFIX}";
            }
            return "";
        }



        (float strength, ConsoleColor color, float distFromCam)[,] values;

        Matrix4x4 viewMatrix = Matrix4x4.Identity;

        public Display(int width, int height)
        {
            values = new (float strength, ConsoleColor color, float distFromCam)[width, height];
            InitializeDepthBuffer();
        }

        //returns double signed area of triangle
        private float EdgeFunction(Vector4 a, Vector4 b, Vector4 c)
        {
            return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
        }

        public void DrawTriangle((Vector4 vec, ConsoleColor col)[] vertices)
        {
            int maxXLen = values.GetLength(0);
            int maxYLen = values.GetLength(1);

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].vec = Vector4.Transform(vertices[i].vec, viewMatrix);
                // LLM: Convert from world space to pixel space
                vertices[i].vec = new Vector4(
                    (vertices[i].vec.X + 1f) * 0.5f * maxXLen,
                    (vertices[i].vec.Y + 1f) * 0.5f * maxYLen,
                    vertices[i].vec.Z,
                    vertices[i].vec.W);
            }
            Vector4 a = vertices[0].vec;
            Vector4 b = vertices[1].vec;
            Vector4 c = vertices[2].vec;

            for (int x = (int)vertices.Min(x => x.vec.X); x < vertices.Max(x => x.vec.X); x++)
            {
                if (x >= 0 && x < maxXLen) {
                    for (int y = (int)vertices.Min(x => x.vec.Y); y < vertices.Max(x => x.vec.Y); y++)
                    {
                        if (y >= 0 && y < maxYLen) {
                            //go to all the values in between and set stuff

                            Vector4 p = new(x, y, 0, 0);
                            float ABP = EdgeFunction(a, b, p);
                            float BCP = EdgeFunction(b, c, p);
                            float CAP = EdgeFunction(c, a, p);
                            //if (ABP >= 0 && BCP >= 0 && CAP >= 0)
                            //{
                                float dist = a.Z * a.W + b.Z * b.W + c.Z * c.W;
                                if (dist < values[x, y].distFromCam)
                                {
                                    values[x, y].distFromCam = dist;
                                    values[x, y].color = vertices[0].col;
                                }
                            //}
                        }
                    }
                }
            }
        }

        public void Clear()
        {
            values = new (float strength, ConsoleColor color, float distFromCam)[values.GetLength(0), values.GetLength(1)];
            InitializeDepthBuffer();
        }

        private void InitializeDepthBuffer()
        {
            for (int x = 0; x < values.GetLength(0); x++)
                for (int y = 0; y < values.GetLength(1); y++)
                    values[x, y].distFromCam = float.MaxValue;
        }

        private char MapToChar(float fullness)
        {
            //takes in normalized 0-1 and maps to a char
            if (fullness == 0) return 'z';
            if (fullness < 0.2) return '░';
            if (fullness < 0.4) return '▒';
            if (fullness < 0.7) return '▓';
            return '█';
        }

        public void DrawToConsole()
        {
            StringBuilder buf = new StringBuilder();
            (char c, ConsoleColor col)[,] display = new (char, ConsoleColor)[values.GetLength(0), values.GetLength(1)];

            float maxDist = 0;
            for (int x = 0; x < values.GetLength(0); x++)
            {
                for (int y = 0; y < values.GetLength(1); y++)
                {
                    if (values[x, y].distFromCam != float.MaxValue && values[x, y].distFromCam > maxDist)
                        maxDist = values[x, y].distFromCam;
                }
            }

            //convert values to chars here
            for (int x = 0; x < values.GetLength(0); x++)
            {
                for (int y = 0; y < values.GetLength(1); y++)
                {
                    (float strength, ConsoleColor color, float distFromCam) value = values[x, y];
                    if (maxDist == 0 || value.distFromCam == float.MaxValue)
                        display[x, y].c = MapToChar(0);
                    else
                        display[x, y].c = MapToChar(value.distFromCam / maxDist);
                    display[x, y].col = value.color;

                }
            }
            


            //draw header
            for (int x = 0; x < display.GetLength(0) * 2 + 2; x++)
            {
                if (x == 0) { buf.Append('┌'); continue; }
                if (x == display.GetLength(0) * 2 + 1) { buf.Append('┐'); continue; }
                buf.Append('─');
            }
            buf.AppendLine();

            buf.AppendLine("│ is that the one hexagon game I dont remmeber the │");

            for (int x = 0; x < display.GetLength(0) * 2 + 2; x++)
            {
                if (x == 0) { buf.Append('└'); continue; }
                if (x == display.GetLength(0) * 2 + 1) { buf.Append('┘'); continue; }
                buf.Append('─');
            } buf.AppendLine();




            for (int x = 0; x < display.GetLength(0) * 2 + 2; x++)
            {
                if (x == 0) { buf.Append('┌'); continue; }
                if (x == display.GetLength(0) * 2 + 1) { buf.Append('┐'); continue; }
                buf.Append('─');
            }
            buf.AppendLine();

            for (int y = 0; y < display.GetLength(1); y++)
            {
                for (int x = -1; x < display.GetLength(0); x++)
                {
                    if (x == -1)
                    {
                        buf.Append('│');
                        continue;
                    }

                    //buf.Append(GetAnsiForegroundColor(display[x, y].col));
                    buf.Append(display[x, y].c);
                    //buf.Append(RESET);

                    buf.Append(" ");
                }
                buf.Append('│');
                buf.AppendLine();
            }
            for (int x = 0; x < display.GetLength(0) * 2 + 2; x++)
            {
                if (x == 0) { buf.Append('└'); continue; }
                if (x == display.GetLength(0) * 2 + 1) { buf.Append('┘'); continue; }
                buf.Append('─');
            }
            buf.AppendLine();

            Console.Clear();
            Console.Write(buf.ToString());
        }
    }
}
