using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotAge.Core.Control;
using DotAge.Core.View;
using Microsoft.Xna.Framework;

namespace DotAge.Core.Model
{
    class BaseStruct
    {
    }

    public struct ItemInformation
    {
        public static ItemInformation NullItem => new("Null", "This is a null item information.", "This is a null item content.");

        public string Title = "Item Information";
        public string Description = "This is a item information.";
        public string Content  = "This is a item content."; 
        public (int , Rectangle?) Icon = (-1, null);
        public ItemInformation(string title, string description, string content)
        {
            Title = title;
            Description = description;
            Content = content;
        }
    }

    class Group
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public Color TintColor { get; set; } = Color.White;
        public int FlagTexutre;
        public List<int> IncludeCreatures { get; set; } = new List<int>();

        public bool JoinCreature(ref Creature creature)
        {
            if (creature == null)
            {
                return false;
            }
            else
            {
                IncludeCreatures.Add(IncludeCreatures.Count);
                return true;
            }

        }
    }

    struct Margin
    {
        public RectF Container
        {
            get
            {
                return new RectF(new Vector2(0, 0), new Vector2(GameSetting.ScreenWidth, GameSetting.ScreenHeight));
                //用匿名函数来实现类似于矩形的数据绑定如何呢

            }
            set
            {

            }
        }
        public float Left { get; set; } = 0;
        public float Right { get; set; } = 0;
        public float Top { get; set; } = 0;
        public float Bottom { get; set; } = 0;

        public Margin(float _left, float _top, float _right, float _bottom)
        {
            Left = _left;
            Top = _top;
            Right = _right;
            Bottom = _bottom;
        }

        public RectF ToRectF()
        {
            RectF retRectF = new RectF();
            var newLeft = Container.Left + Left;
            var newRight = Container.Right - Right;
            var newTop = Container.Top + Top;
            var newBottom = Container.Bottom - Bottom;
            retRectF = new RectF(newLeft, newTop, newRight, newBottom);
            return retRectF;
        }

        public static RectF ToRectF(Margin margin)
        {
            RectF retRectF = new RectF();
            var newLeft = margin.Container.Left + margin.Left;
            var newRight = margin.Container.Right - margin.Right;
            var newTop = margin.Container.Top + margin.Top;
            var newBottom = margin.Container.Bottom - margin.Bottom;
            retRectF = new RectF(newLeft, newTop, newRight, newBottom);
            return retRectF;
        }

    }

    struct Ray
    {
        public Vector2 Position;
        public Vector2 Direct;
    }

    struct RectF
    {
        public float Right { get { return Position.X + Size.X; } }
        public float Left { get { return Position.X; } }
        public float Top { get { return Position.Y; } }
        public float Bottom { get { return Position.Y + Size.Y; } }
        public Vector2 Center { get { return (Position + Size) / 2; } }
        public Vector2 Position { get; set; } = new Vector2();
        public Vector2 Size { get; set; } = new Vector2();

        public static RectF CombineRectangel(RectF rect1, RectF rect2)
        {
            float _xL = MathF.Min(rect1.Left, rect2.Left);
            float _xR = MathF.Max(rect1.Right, rect2.Right);
            float _yT = MathF.Min(rect1.Top, rect2.Top);
            float _yB = MathF.Max(rect1.Bottom, rect2.Bottom);
            return new RectF(_xL, _yT, _xR, _yB);
        }

        public static RectF ExpandRectangel(RectF rect, Vector2 expandVec)
        {
            float _xL = Math.Min(rect.Position.X, rect.Position.X + expandVec.X);
            float _xR = Math.Max(rect.Position.X + rect.Size.X, rect.Position.X + rect.Size.X + expandVec.X);
            float _yT = Math.Min(rect.Position.Y, rect.Position.Y + expandVec.Y);
            float _yB = Math.Max(rect.Position.Y + rect.Size.Y, rect.Position.Y + rect.Size.Y + expandVec.Y);
            return new RectF(_xL, _yT, _xR, _yB);
        }

        public static (CrashInfo, CrashInfo) Direction(RectF rect1, RectF rect2)
        {
            (CrashInfo, CrashInfo) result = (new CrashInfo(), new CrashInfo());
            if (rect1.Center.X > rect2.Center.X)
            {
                result.Item1.XCrashDirecton = -1;
            }
            else if (rect1.Center.X < rect2.Center.X)
            {
                result.Item1.XCrashDirecton = 1;
            }
            else
            {
                result.Item1.XCrashDirecton = 0;
            }
            if (rect1.Center.Y > rect2.Center.Y)
            {
                result.Item1.YCrashDirecton = -1;
            }
            else if (rect1.Center.Y < rect2.Center.Y)
            {
                result.Item1.YCrashDirecton = 1;
            }
            else
            {
                result.Item1.YCrashDirecton = 0;
            }
            result.Item2 = result.Item1.GetNegative();
            return result;

        }

        public RectF(Vector2 _position, Vector2 _size)
        {
            Position = _position;
            Size = _size;
        }

        public RectF(float _left, float _top, float _right, float _bottom)
        {
            Position = new Vector2(_left, _top);
            Size = new Vector2(_right - _left, _bottom - _top);
        }

        public static Vector2 ContainCount(RectF rect1, RectF rect2)
        {
            Vector2 xyContain = new Vector2();
            if (IsContain(rect1, rect2) == true)
            {
                RectF TwoRect = CombineRectangel(rect1, rect2);
                xyContain.X = -TwoRect.Size.X + rect1.Size.X + rect2.Size.X;
                xyContain.Y = -TwoRect.Size.Y + rect1.Size.Y + rect2.Size.Y;
            }
            return xyContain;
        }

        public static bool IsContain(RectF rect1, Vector2 point)
        {
            bool stat = false;
            if (point.X > rect1.Left && point.X < rect1.Right && point.Y > rect1.Top && point.Y < rect1.Bottom)
            {
                stat = true;
            }
            return stat;
        }

        public static bool IsContain(RectF rect1, RectF rect2)
        {
            bool stat = false;
            if (rect1.Right > rect2.Left && rect1.Left < rect2.Right && rect1.Bottom > rect2.Top && rect1.Top < rect2.Bottom)
            {
                stat = true;
            }
            return stat;
        }


        public RectF TestMove(Vector2 Vec)
        {
            RectF tmp = this;
            tmp.Move(Vec);
            return tmp;
        }

        public RectF TestMove(Vector2 Speed, int Time)
        {
            RectF tmp = this;
            tmp.Move(Speed * Time);
            return tmp;
        }

        public bool Move(Vector2 Vec)
        {
            Position += Vec;
            return true;
        }

        public bool ChangeSize(Vector2 Vec)
        {
            Size += Vec;
            return true;
        }
    }

    struct CrashInfo
    {
        public int XCrashDirecton { get; set; }
        public int YCrashDirecton { get; set; }

        public (Direction, Direction) Direct
        {
            get
            {
                Direction directionx;
                Direction directiony;
                if (XCrashDirecton == -1)
                {
                    directionx = Direction.Left;
                }
                else if (XCrashDirecton == 1)
                {
                    directionx = Direction.Right;
                }
                else
                {
                    directionx = Direction.Middle;
                }
                if (YCrashDirecton == -1)
                {
                    directiony = Direction.Top;
                }
                else if (YCrashDirecton == 1)
                {
                    directiony = Direction.Bottom;
                }
                else
                {
                    directiony = Direction.Middle;
                }
                return (directionx, directiony);
            }
            set
            {
                if (value.Item1 == Direction.Left)
                {
                    XCrashDirecton = -1;
                }
                else if (value.Item1 == Direction.Right)
                {
                    XCrashDirecton = 1;
                }
                else
                {
                    XCrashDirecton = 0;
                }
                if (value.Item2 == Direction.Top)
                {
                    YCrashDirecton = -1;
                }
                else if (value.Item2 == Direction.Bottom)
                {
                    YCrashDirecton = 1;
                }
                else
                {
                    YCrashDirecton = 0;
                }
            }
        }

        public void Negative()
        {
            XCrashDirecton *= -1;
            YCrashDirecton *= -1;
        }

        public CrashInfo GetNegative()
        {
            CrashInfo tmp = this;
            tmp.XCrashDirecton *= -1;
            tmp.YCrashDirecton *= -1;
            return tmp;
        }
    }


    static class VectorHelper
    {
        public static Vector2 Direct(Vector2 vector)
        {
            Vector2 result = new Vector2();

            if (vector.X > 0)
            {
                result.X = 1;
            }
            else if (vector.X < 0)
            {
                result.X = -1;
            }
            if (vector.Y > 0)
            {
                result.Y = 1;
            }
            else if (vector.Y < 0)
            {
                result.Y = -1;
            }
            return result;
        }
    }

    enum Direction
    {
        Middle = 0,
        Top = 1,
        Right = 2,
        Bottom = 3,
        Left = 4

    }
}
