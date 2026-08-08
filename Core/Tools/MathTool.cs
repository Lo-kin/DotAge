using DotAge.Core.Model;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotAge.Core.Tools
{
    
    static class MathTool
    {
        public const float EPS = 1e-6f;
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
            float flag = 1;
            float ab = ProjectVec.X * BeingProjectedVec.X + ProjectVec.Y * BeingProjectedVec.Y;
            if (ab > 0)
            {
                flag = 1;
            }
            else if (ab == 0)
            {
                flag = 0;
            }
            else if(ab < 0)
            {
                flag = -1;
            }
            var value = flag * ((ProjectVec.X * BeingProjectedVec.X + ProjectVec.Y * BeingProjectedVec.Y) / (MathF.Pow(BeingProjectedVec.X, 2) + MathF.Pow(BeingProjectedVec.Y, 2))) * BeingProjectedVec;
            return value;
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

        public static bool LineCrossPoint(LineF line1 , LineF line2 , out Vector2 CrossPoint)
        {
            CrossPoint = Vector2.Zero;

            float det = Cross2D(line1.Line, line2.Line);

            if (MathF.Abs(det) <= 1e-14)
            {
                return false;
            }

            Vector2 LineGap = line1.Start - line2.Start;

            float t = Cross2D(line2.Line, LineGap) / det;
            float u = Cross2D(line1.Line, LineGap) / det;

            if (t > -EPS && t < 1.0f + EPS && u > -EPS && u < 1.0f + EPS)
            {
                CrossPoint = line1.Start + t * line1.Line;
                return true;
            }
            else
            {
                return false;
            }

        }

        public static Vector2 Ceiling(Vector2 vec)
        {
            return new Vector2(MathF.Ceiling(vec.X), MathF.Ceiling(vec.Y));
        }

        public static Vector2 Floor(Vector2 vec)
        {
            return new Vector2(MathF.Floor(vec.X), MathF.Floor(vec.Y));
        }

        public static Vector2 Vector3To2(Vector3 vec)
        {
            return new Vector2(vec.X, vec.Y);
        }

        public static float Cross2D(Vector2 vec1, Vector2 vec2)
        {
            return vec1.X * vec2.Y - vec1.Y * vec2.X;
        }

        public static float Dot2D(Vector2 vec1, Vector2 vec2)
        {
            return vec1.X * vec2.X + vec1.Y * vec2.Y;
        }

        public static List<Vector2> GetRayCrossRect(RayF ray , float length, RectF rect)
        {
            List<Vector2> crossPoints = new List<Vector2>();
            Vector2 rayStart = ray.Position;
            Vector2 rayDirection = ray.Direct;
            Vector2 rayEnd = ray.GetEnd(length);
            Vector2 crossPoint;
            if (LineCrossPoint(ray.GetLine(length), new RayF(new Vector2(rect.Left, rect.Top), Vector2.UnitX).GetLine(rect.Size.X), out crossPoint))
            {
                crossPoints.Add(crossPoint);
            }
            if (LineCrossPoint(ray.GetLine(length), new RayF(new Vector2(rect.Left, rect.Top), Vector2.UnitY).GetLine(rect.Size.Y), out crossPoint))
            {
                crossPoints.Add(crossPoint);
            }
            if (LineCrossPoint(ray.GetLine(length), new RayF(new Vector2(rect.Left, rect.Bottom), Vector2.UnitX).GetLine(rect.Size.X), out crossPoint))
            {
                crossPoints.Add(crossPoint);
            }
            if (LineCrossPoint(ray.GetLine(length), new RayF(new Vector2(rect.Right, rect.Top), -Vector2.UnitY).GetLine(rect.Size.Y), out crossPoint))
            {
                crossPoints.Add(crossPoint);
            }
            crossPoints.Sort((a, b) => Vector2.Distance(rayStart, a).CompareTo(Vector2.Distance(rayStart, b)));
            return crossPoints;
        }
    }
}
