using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotAge.Core.Tools
{
    static class MathTool
    {
        public static Point LengthTransToPointX(int Position , int Limit)
        {
            return new Point(Position % Limit, Position / Limit);
        }

        public static Point LengthTransToPointY(int Position, int Limit)
        {
            return new Point(Position / Limit, Position % Limit);
        }

        public static Vector2 GetVector2Mod(Vector2 MainVec2 , Vector2 SubVec2)
        {
            return new Vector2( MainVec2.X % SubVec2.X , MainVec2.Y % SubVec2.Y);
        }

        public static Vector2 MinVector2(Vector2 vec1 , Vector2 vec2)
        {
            if (vec1.Length() > vec2.Length())
            {
                return vec2;
            }
            else
            {
                return vec1;
            }
        }

        public static Vector2 ManVector2(Vector2 vec1, Vector2 vec2)
        {
            if (vec1.Length() < vec2.Length())
            {
                return vec2;
            }
            else
            {
                return vec1;
            }
        }
    }
}
