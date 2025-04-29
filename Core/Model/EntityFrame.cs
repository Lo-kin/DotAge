using DotAge.Core.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotAge.Core.Model
{

    class GameEntity
    {
        public float Health = 100f;
        public Ray ShootingPostion { get; set; }
        public string Name { get; set; }
        public bool Visibility { get; set; } = true;
        public bool IsRenderFollowCrashbox { get; set; } = false;
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
            InitialCanvas(16);
        }

        public RenderEntity(int CanvasCount)
        {
            InitialCanvas(CanvasCount);
        }

        public bool InitialCanvas(int CanvasCount)
        {
            if (CanvasCount <= 0)
            {
                return false;
            }
            else
            {
                RenderCanvas = new RenderProperty[CanvasCount];
                EmptyCanvas = Enumerable.Range(0, CanvasCount - 1).ToArray();
                return true;
            }
        }

        public bool UpdatePosition(Vector2 Position)
        {
            RenderPosition = Position;
            for (int i = 0; i < RenderCanvas.Length; i++)
            {
                if (RenderCanvas[i].Init == true)
                {
                    RenderCanvas[i].ActualPosition = RenderPosition;
                }

            }
            return true;
        }

        public bool UpdateSize(Vector2 Size)
        {
            RenderSize = Size;
            for (int i = 0; i < RenderCanvas.Length; i++)
            {
                if (RenderCanvas[i].Init == true)
                {
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
                RenderCanvas[Index].Offset = Position;
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
                    Offset = new Vector2(0, 0),
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
                    Offset = new Vector2(0, 0),
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

    class ChainRenderEntity
    {//想象一条链子，由链接点与链接物体构成，
        public Vector2 ChainPoint = new Vector2();
        public Vector2 EndChainpoint = new Vector2();
        public Vector2 UniformPosition = new Vector2();
        public Vector2 UniformSize = new Vector2(32, 32);
        public RectF ChainBox
        {
            get
            {
                return new RectF(UniformPosition, UniformSize);
            }
        }
        public RenderProperty[] RenderCanvas = new RenderProperty[16];

        public ChainRenderEntity(int CanvasCount = 16)
        {
            InitialCanvas(CanvasCount);
        }

        public bool InitialCanvas(int CanvasCount)
        {
            if (CanvasCount <= 0)
            {
                RenderCanvas = new RenderProperty[16];
                return false;
            }
            else
            {
                RenderCanvas = new RenderProperty[CanvasCount];
                return true;
            }
        }

        public bool SetUniform(Vector2 Position, Vector2 Size)
        {
            UniformPosition = Position;
            UniformSize = Size;
            for (int i = 0; i < RenderCanvas.Length; i++)
            {
                if (RenderCanvas[i].Init == true)
                {
                    RenderCanvas[i].Offset = UniformPosition;
                    RenderCanvas[i].Size = UniformSize;
                }
            }
            return true;
        }
    }
}
