using DotAge.Core.Control;
using DotAge.Core.Model.Economy;
using DotAge.Core.Tools;
using DotAge.Core.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotAge.Core.Model
{

    class GameEntity
    {
        public float LastHealth = 100f;
        public float Health = 100f;
        public float MaxHealth = 100f;
        public Ray ShootingPostion { get; set; }
        public string Name { get; set; }
        public bool Visibility { get; set; } = true;
        public bool IsRenderFollowCrashbox { get; set; } = false;
        public float Money = 100f;

        public Dictionary<Type, int> ProductCount = new Dictionary<Type, int>();

        public string GetProductCount
        {
            get
            {
                return string.Join("\n", ProductCount.Select(x => x.Key.Name + " : " + x.Value.ToString()));
            }
        }

        public float SetHealth(float _health)
        {
            LastHealth = Health;
            Health = _health;
            //HealthSetEvent?.Invoke(Health , MaxHealth);
            return Health;
        }

        public float ModifyHealth(float _health)
        {
            if (Health + _health > MaxHealth)
            {
                _health = MaxHealth - Health;
            }
            SetHealth(Health + _health);
            return Health;
        }

        public Ray ModifyShootingPosition(Ray _ray)
        {
            ShootingPostion = _ray;
            return _ray;
        }

        public float ModifyMoney(float _money)
        {
            Money += _money;
            return Money;
        }

        public bool Buy(Product _product, ProductValue _price)
        {
            if (Money < _price.Price)
            {
                return false;
            }
            else
            {
                Money -= _price.Price;
                if (ProductCount.ContainsKey(_product.GetType) == false)
                {
                    ProductCount.Add(_product.GetType, 1);
                }
                else
                {
                    ProductCount[_product.GetType]++;
                }
                return true;
            }
        }

        public bool Sale(Product _product, ProductValue _price)
        {
            if (ProductCount.ContainsKey(_product.GetType) == false)
            {
                return false;
            }
            else
            {

                if (ProductCount[_product.GetType] <= 0)
                {
                    return false;
                }
                Money += _price.Price;
                ProductCount[_product.GetType]--;
                return true;
            }
        }
    }

    class RenderEntity
    {
        public Vector2 RenderPosition = new Vector2();
        public Vector2 RenderSize = new Vector2(32, 32);
        public RenderPropertyGroup Canvas = new RenderPropertyGroup(16);

        public RenderEntity(int CanvasCount = 16, int[] InitCanvas = null)
        {
            Canvas = new RenderPropertyGroup(CanvasCount, RenderSize, RenderPosition, InitCanvas);
            Canvas.SetAllTextureState(true);
            Canvas.SetAllTextState(true);
        }

        public bool UpdatePosition(Vector2 Position)
        {
            RenderPosition = Position;
            Canvas.SetAllPosition(RenderPosition);
            return true;
        }

        public bool UpdateSize(Vector2 Size)
        {
            RenderSize = Size;
            Canvas.SetAllSize(RenderSize);
            return true;
        }

        public bool RemoveTexture(int Index)
        {

            return true;
        }

        public bool ChangeRalatePosition(Vector2 Position, int Index)
        {
            return Canvas.SetOffset(Index, Position);
        }

        public bool ChangeFront((int, Rectangle?) _textureInfo)
        {
            return ChangeCanvas(Canvas.RenderProperties.Length - 1, _textureInfo);
        }

        public bool ChangeBackground((int, Rectangle?) _textureInfo)
        {
            return ChangeCanvas(0, _textureInfo);
        }

        public bool ChangeCanvas(int ChangePos, (int, Rectangle?) _textureInfo)
        {
            if (Canvas.CheckVaild(ChangePos) == false)
            {
                return false;
            }
            else
            {
                Canvas.RenderProperties[ChangePos].RenderTexture = _textureInfo;
            }
            return true;
        }
    }

    class PhysicEntity
    {
        public Path PathNodes = new Path();
        public Vector2 Force = new Vector2();//m^2/ms
        public float SpeedLength = 100f;
        public Vector2 SpeedForward = new Vector2(0, 0); //m/ms
        public Vector2 SpeedVec { get { return SpeedLength * SpeedForward; } }

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

        }

        public bool MoveForward(Vector2 _vec)
        {
            Position += _vec;
            return true;
        }

        public bool UpdateWish(Vector2 Fiction = new Vector2(), float Tick = 10)
        {
            if (PathNodes.GetNodes.Count == 1)
            {
                Console.WriteLine("PathNodes is Empty , Please Add Nodes First");
            }
            SpeedForward = PathNodes.Update(Position);
            WishForward += MathTool.MinVector2(PathNodes.RemainTarget, Tick * (SpeedVec + (Tick / 1000 * (Force - Fiction) / 2)) / 1000);
            return true;
        }

        public bool Update(Vector2 Fiction = new Vector2(), float Tick = 10)
        {
            Position += WishForward;
            WishForward = new Vector2(0, 0);

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

                    var SpeedX = MathF.Abs(physicEntity1.SpeedVec.X - physicEntity2.SpeedVec.X);
                    var TimeX = MinX / SpeedX;

                    var MinY = MathF.Min(MathF.Abs(BeforeMove1.Top - BeforeMove2.Bottom), MathF.Abs(BeforeMove1.Bottom - BeforeMove2.Top));

                    var SpeedY = MathF.Abs(physicEntity1.SpeedVec.Y - physicEntity2.SpeedVec.Y);
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

    class LocationPhysicEntity
    {
        public Vector2 Position = new Vector2(0, 0);
        public Vector2 Size = new Vector2(32, 32);
        public RectF CrashBox
        {
            get
            {
                return new RectF(Position, Size);
            }
        }
        public LocationPhysicEntity()
        {

        }

        public LocationPhysicEntity(Vector2 _position, Vector2 _size)
        {
            Position = _position;
            Size = _size;
        }
        public bool UpdatePosition(Vector2 _position)
        {
            Position = _position;
            return true;
        }
    }

    class CombineRenderEntity
    {

    }

    class ChainRenderEntity
    {//想象一条链子，由链接点与链接物体构成，这里指的是单个链接点
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

        public ChainRenderEntity(int CanvasCount = 3)
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
                for (int i = 0; i < RenderCanvas.Length; i++)
                {
                    RenderCanvas[i] = new RenderProperty() { Init = true, Visibility = true, TintColor = Color.White };
                }
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
                    RenderCanvas[i].ActualPosition = UniformPosition;
                    RenderCanvas[i].Size = UniformSize;
                }
            }
            return true;
        }

        public bool SetChainPoint(Vector2 Position)
        {
            ChainPoint = Position;
            for (int i = 0; i < RenderCanvas.Length; i++)
            {
                if (RenderCanvas[i].Init == true)
                {
                    RenderCanvas[i].ActualPosition = ChainPoint;
                }
            }
            return true;
        }
    }

    class BaseStorage
    {

    }

    class ZoneEntity
    {
        public bool IsFixedToMap = false;
        public RectF TriggerZone = new RectF(0, 0, 0, 0);
        public event Func<Vector2, bool> TriggerDelegate = null;
        public string Description = "Default Zone Entity";
        public int invokeCount = 0;
        public TwoStat Condition = TwoStat.None;
        public Point t = new Point(0, 0);//临时的点参数

        public bool BindDelegate(Func<Vector2, bool> _delegate)
        {
            if (_delegate == null)
            {
                return false;
            }
            else
            {
                TriggerDelegate += _delegate;
                return true;
            }
        }

        public bool CheckTrigger(Vector2 _position , TwoStat _condition)
        {
            if (RectF.IsContain(TriggerZone  , _position) && (_condition == Condition || Condition == TwoStat.Any))
            {
                return true;
            }
            return false;
        }

        public bool Trigger(Vector2 TPosition , TwoStat _condition)
        {
            if (CheckTrigger(TPosition , _condition) == false)
            {
                return false;
            }
            if (TriggerDelegate == null)
            {
                return false;
            }
            TriggerDelegate.Invoke(TPosition);
            invokeCount++;
            return true;
        }
    }
}
