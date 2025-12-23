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
        public static Vector2 VectorDirect(Vector2 vector2)
        {
            Vector2 direct = new Vector2(0, 0);
            if (vector2.X > 0)
            {
                direct.X = 1;
            }
            else if (vector2.X < 0)
            {
                direct.X = -1;
            }
            if (vector2.Y > 0)
            {
                direct.Y = 1;
            }
            else if (vector2.Y < 0)
            {
                direct.Y = -1;
            }
            return direct;
        }

        public static Vector2 DemicalVector(Vector2 input)
        {
            return new Vector2(MathF.Floor(input.X), MathF.Floor(input.Y));
        }
        public static Vector2 AbsVector(Vector2 v)
        {
            return new Vector2(MathF.Abs(v.X), MathF.Abs(v.Y));
        }
        public static Vector2 VectorSign(Vector2 v)
        {
            return new Vector2(MathF.Sign(v.X), MathF.Sign(v.Y));
        }
        public static Vector2 Project(Vector2 ProjectVec , Vector2 BeingProjectedVec)
        {
            if ((BeingProjectedVec.X == 0 && BeingProjectedVec.Y == 0) || (ProjectVec.X == 0 && ProjectVec.Y == 0))
            {
                return Vector2.Zero;
            }
            Vector2 bn = new Vector2 (BeingProjectedVec.X, BeingProjectedVec.Y);
            bn.Normalize();
            Vector2 pn = new Vector2 (ProjectVec.X, ProjectVec.Y);
            pn.Normalize();
            return System.Numerics.Vector2.Dot(pn.ToNumerics(), bn.ToNumerics()) * pn * BeingProjectedVec.Length();
        }
        public static Point HorizonLayout(int Position , int Limit)
        {
            return new Point(Position % Limit, Position / Limit);
        }

        public static Point VerticalLayout(int Position, int Limit)
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

        public static Vector2 MaxVector2(Vector2 vec1, Vector2 vec2)
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
