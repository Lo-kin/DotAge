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
    interface IClick
    {
        ZoneEntity ClickZone { get; set; }
        ItemInformation? OnClick();
    }

    interface IEntityProperty
    {
        public Entity SourceEntity { get; set; }

    }

    public class EntityProperty : IEntityProperty
    {
        public Entity SourceEntity { get; set; }

        public EntityProperty()
        {

        }
    }

    public class GameEntity : EntityProperty
    {
        public float LastHealth = 100f;
        public float Health = 100f;
        public float MaxHealth = 100f;
        public Ray ShootingPostion { get; set; }
        public string Name { get; set; }
        public bool Visibility { get; set; } = true;
        public List<Entity> KilledEntity = new List<Entity>();
        public Entity BeingKilledEntity = null;
        public bool IsRenderFollowCrashbox { get; set; } = false;
        public float Money = 0f;
        public bool IsAlive { get { return Health > 0; } }

        public Dictionary<Type, int> ProductCount = new Dictionary<Type, int>();

        public string GetProductCount
        {
            get
            {
                return string.Join("\n", ProductCount.Select(x => x.Key.Name + " : " + x.Value.ToString()));
            }
        }

        public GameEntity()
        {

        }

        public float SetHealth(float _health , Entity Source)
        {
            LastHealth = Health;
            Health = _health;
            if (Health < 0)
            {
                BeingKilledEntity = Source;
                Source.Kill(SourceEntity);
            }
            //HealthSetEvent?.Invoke(Health , MaxHealth);
            return Health;
        }

        public float ModifyHealth(float _health , Entity Source)
        {
            if (Health + _health > MaxHealth)
            {
                _health = MaxHealth - Health;
            }
            SetHealth(Health + _health , Source);
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

    public class RenderEntity : EntityProperty
    {
        public TextureSprite Sprite = new TextureSprite();
        public RenderEntity()
        {

        }

        public bool UpdatePosition(Vector2 Position)
        {
            Sprite.Position = Position;
            return true;
        }

        public bool UpdateSize(Vector2 Size)
        {
            Sprite.Size = Size;
            return true;
        }

        public bool RemoveTexture(int Index)
        {

            return true;
        }

        public bool LoadTexture(string asssteName)
        {
            Sprite.Region = TextureManager.GetTextureRegionByName(asssteName);
            return true;
        }
    }

    public class PhysicEntity : EntityProperty
    {
        public Path PathNodes = new Path();
        public Pioneer Pioneer = new Pioneer();
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

        public bool UpdateWish(Vector2 Fiction = new Vector2(), int Tick = 10)
        {
            /*
            if (PathNodes.GetNodes.Count == 1)
            {
                Console.WriteLine("PathNodes is Empty , Please Add Nodes First");
            }
            SpeedForward = PathNodes.Update(Position);
            */
            WishForward += Pioneer.Update(Tick , SpeedLength);
            return true;
        }

        public bool Update(Vector2 Fiction = new Vector2(), float Tick = 10)
        {
            UpdateWish();
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

    class BaseStorage
    {

    }

    public class ZoneEntity
    {
        public IPhysicEntity CurrentPhysicEntity;
        public bool IsFixedToMap = false;
        public RectF TriggerZone = new RectF(0, 0, 0, 0);
        public event Func<RectF, bool> TriggerDelegate = null;
        public string Description = "Default Zone Entity";
        public int InvokeCount = 0;
        public Stator ZoneStator =new Stator();
        public MessageEntity InformationSource = new();  

        public ZoneEntity(IPhysicEntity physicEntity , TwoStatus TriggerState)
        {
            CurrentPhysicEntity = physicEntity;
            ZoneStator = new Stator(TriggerState);
        }

        public bool BindDelegate(Func<RectF, bool> _delegate)
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

        public bool CheckTrigger()
        {
            if (TriggerDelegate == null)
            {
                return false;
            }
            if (RectF.IsContain(TriggerZone , CurrentPhysicEntity.PhysicProperty.CrashBox))
            {
                ZoneStator.Update(true);
                if (ZoneStator.IsTriggered() == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else 
            {
                ZoneStator.Update(false);
                return false;
            }
        }

        public bool Trigger()
        {   
            if (CheckTrigger() == false)
            {
                return false;
            }
            else
            {
                TriggerDelegate.Invoke(RectF.CrossZone(TriggerZone, CurrentPhysicEntity.PhysicProperty.CrashBox));
                InvokeCount++;
                return true;
            }

        }
    }

    public class MessageEntity
    {
        public ItemInformation MessageItem = ItemInformation.NullItem;
        public string Message { get; set; } = "default";
    }
}
