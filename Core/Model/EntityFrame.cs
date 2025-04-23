using DotAge.Core.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotAge.Core.Model
{
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

    class GameEntity
    {
        public float Health = 100f;
        public Ray ShootingPostion { get; set; }
        public string Name { get; set; }
        public bool Visibility { get; set; } = true;
        public int Money = 0;

        public float SetHealth(float _health)
        {
            Health = _health;
            return Health;
        }

        public float ModifyHealth(float _health)
        {
            Health += _health;
            return Health;
        }

        public Ray ModifyShootingPosition(Ray _ray)
        {
            ShootingPostion = _ray;
            return _ray;
        }

        public int ModifyMoney(int _money)
        {
            Money += _money;
            return Money;
        }
    }

    class RenderEntity
    {
        public Vector2 RenderPosition = new Vector2();
        public Vector2 RenderSize = new Vector2(32, 32);
        public RenderProperty[] RenderCanvas = new RenderProperty[16];
        public int[] EmptyCanvas = Enumerable.Range(0, 15).ToArray();

        public RenderEntity()
        {

        }

        public bool UpdatePosition(Vector2 Position, Vector2 Size)
        {
            RenderPosition = Position;
            RenderSize = Size;
            for (int i = 0; i < RenderCanvas.Length; i++)
            {
                if (RenderCanvas[i].Init == true)
                {
                    RenderCanvas[i].RenderPosition = RenderCanvas[i].RalativePosition + RenderPosition;
                    RenderCanvas[i].Size = RenderSize;
                }

            }
            return true;
        }

        public bool InsertTexture(TextureName name, int Index)
        {
            if (EmptyCanvas.Length != 0)
            {
                int MinIndex = Index;
                if (EmptyCanvas.Contains(Index))
                {
                    RenderCanvas[Index].RenderTexture = name;
                    return true;
                }
                foreach (int _index in EmptyCanvas)
                {
                    MinIndex = Math.Min(MinIndex, _index);
                }
                RenderCanvas[MinIndex].RenderTexture = name;
                return true;
            }
            return false;
        }

        public bool RemoveTexture(int Index)
        {

            return true;
        }

        public bool ChangeRalatePosition(Vector2 Position, int Index)
        {
            if (EmptyCanvas.Contains(Index) == true || Index >= 0 && Index < RenderCanvas.Length)
            {
                RenderCanvas[Index].RalativePosition = Position;
                return true;
            }
            return true;
        }

        public bool ChangeBackground(TextureName name)
        {
            if (RenderCanvas[0].Init == false)
            {
                RenderCanvas[0] = new RenderProperty()
                {
                    Init = true,
                    RenderTexture = name,
                    Visibility = true,
                    TintColor = Color.White,
                    RenderPosition = new Vector2(0, 0),
                    Size = RenderSize,
                };
            }
            RenderCanvas[0].RenderTexture = name;
            return true;
        }

        public bool ChangeFront(TextureName name)
        {
            if (RenderCanvas[^1].Init == false)
            {
                RenderCanvas[^1] = new RenderProperty()
                {
                    Init = true,
                    RenderTexture = name,
                    Visibility = true,
                    TintColor = Color.White,
                    RenderPosition = new Vector2(0, 0),
                    Size = RenderSize,
                };
            }
            RenderCanvas[^1].RenderTexture = name;
            return true;
        }

    }

    class PhysicEntity
    {
        public Path PathNodes = new Path();
        public Vector2 Force = new Vector2();//m^2/ms
        public Vector2 Speed = new Vector2(50, 50); //m/ms
        public Vector2 Position = new Vector2(0, 0);
        public Vector2 Size = new Vector2(32, 32);
        public RectF CrashBox
        {
            get
            {
                return new RectF(Position, Size);
            }
        }
        public Vector2 WishForward = new Vector2();
        public RectF WishRange
        {
            get
            {
                return RectF.ExpandRectangel(CrashBox, WishForward);
            }
        }

        public PhysicEntity()
        {
            //PathNodes.AddNode(Position);
        }

        public bool MoveForward()
        {
            return true;
        }

        public bool UpdateWish(Vector2 Fiction = new Vector2(), float Tick = 10)
        {
            PathNodes.Update(Position);
            if (PathNodes.RemainLength < Tick * Speed.Length() / 1000)
            {
                //Thread.Sleep(1000);
                WishForward = PathNodes.RemainTarget;
            }
            else
            {
                Vector2 TmpSpeed = Tick / 1000 * (Force - Fiction);
                Vector2 SpeedAvg = Speed + TmpSpeed / 2;
                WishForward = PathNodes.CurrentDirect * SpeedAvg * (Tick / 1000);
            }
            return true;
        }

        public bool Update(Vector2 Fiction = new Vector2(), float Tick = 10)
        {
            Vector2 SpeedAvg = Speed;
            Speed += Tick / 1000 * (Force - Fiction);
            SpeedAvg = (SpeedAvg + Speed) / 2;
            Position += WishForward;

            return true;
        }

        public static Vector2 Crash()
        {
            return new Vector2();
        }

        public static (Vector2, (CrashInfo, CrashInfo))? Crash(PhysicEntity physicEntity1, PhysicEntity physicEntity2)
        {
            if (RectF.IsContain(physicEntity1.WishRange, physicEntity2.WishRange))
            {
                /*
                if (RectF.IsContain(physicEntity1.CrashBox , physicEntity2.CrashBox) == true)
                {
                    return (RectF.ContainCount(physicEntity1.CrashBox, physicEntity2.CrashBox) , (new CrashInfo() , new CrashInfo()));
                }*/
                RectF BeforeMove1 = physicEntity1.CrashBox;
                RectF BeforeMove2 = physicEntity2.CrashBox;
                RectF AfterMove1 = physicEntity1.CrashBox.TestMove(physicEntity1.WishForward);
                RectF AfterMove2 = physicEntity2.CrashBox.TestMove(physicEntity2.WishForward);

                if (RectF.IsContain(AfterMove1, AfterMove2) == false)
                {
                    return null;
                }
                else
                {
                    CrashInfo crashInfo1 = new CrashInfo();
                    CrashInfo crashInfo2 = new CrashInfo();
                    Vector2 CrossCount = RectF.ContainCount(AfterMove1, AfterMove2);
                    (CrashInfo, CrashInfo) twodirect = RectF.Direction(AfterMove1, AfterMove2);

                    var MinX = MathF.Min(MathF.Abs(BeforeMove1.Right - BeforeMove2.Left), MathF.Abs(BeforeMove1.Right - BeforeMove2.Left));

                    var SpeedX = MathF.Abs(physicEntity1.Speed.X - physicEntity2.Speed.X);
                    var TimeX = MinX / SpeedX;

                    var MinY = MathF.Min(MathF.Abs(BeforeMove1.Top - BeforeMove2.Bottom), MathF.Abs(BeforeMove1.Bottom - BeforeMove2.Top));

                    var SpeedY = MathF.Abs(physicEntity1.Speed.Y - physicEntity2.Speed.Y);
                    var TimeY = MinY / SpeedY;

                    if (TimeY > TimeX)
                    {
                        crashInfo1.YCrashDirecton = 0;
                        crashInfo2.YCrashDirecton = 0;
                    }
                    else if (TimeY < TimeX)
                    {
                        crashInfo1.XCrashDirecton = 0;
                        crashInfo2.XCrashDirecton = 0;
                    }
                    return (CrossCount, (crashInfo1, crashInfo2));
                }
            }
            return null;
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
