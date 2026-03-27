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
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotAge.Core.Model
{
    public interface ILocation
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public RectF CrashBox { get { return new RectF(Position, Size); } }

        public bool UpdatePosition(Vector2 NewPosition)
        {
            if (NewPosition == Position || NewPosition.X == float.NaN || NewPosition.Y == float.NaN)
            {
                return false;
            }
            else
            {
                Position = NewPosition;
                return true;
            }
        }

        public bool UpdateSize(Vector2 NewSize)
        {
            if (NewSize == Size || NewSize.X == float.NaN || NewSize.Y == float.NaN)
            {
                return false;
            }
            else
            {
                Size = NewSize;
                return true;
            }
        }
    }

    interface IClick
    {
        ZoneEntity ClickZone { get; set; }
        ItemInformation? OnClick();
    }

    public class Location : ILocation
    {
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Vector2 Size { get; set; } = new Vector2(32, 32);
        public RectF CrashBox
        {
            get
            {
                return new RectF(Position, Size);
            }
        }
        public Location()
        {
        }
    }

    public class GameEntity
    {
        public float LastHealth = 100f;
        public float Health = 100f;
        public float MaxHealth = 500f;
        public Ray ShootingPostion { get; set; }
        public string Name { get; set; }
        public bool Visibility { get; set; } = true;
        public List<Entity> KilledEntity = new List<Entity>();
        public Entity BeingKilledEntity = null;
        public bool IsRenderFollowCrashbox { get; set; } = false;
        public float Money = 0f;
        public bool IsAlive { get { return Health > 0; } }

        public Dictionary<Type, int> ProductCount = new Dictionary<Type, int>();
        public int SkillPoint = 0;
        public int PierceCount = 1;
        public int Damage = 9;
        public int SightRange = 10;//unit block
        public List<IRenderEntity> SeekedEntitites = new List<IRenderEntity>();
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

        public float SetHealth(float _health, Entity Source)
        {
            LastHealth = Health;
            Health = _health;
            if (Health < 0)
            {
                BeingKilledEntity = Source;
            }
            //HealthSetEvent?.Invoke(Health , MaxHealth);
            return Health;
        }

        public float ModifyHealth(float _health, Entity Source)
        {
            if (Health + _health > MaxHealth)
            {
                _health = MaxHealth - Health;
            }
            SetHealth(Health + _health, Source);
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

    public class RenderEntity
    {
        public Sprite Sprite = new TextureSprite();
        public bool BlockVision = false;

        public RenderEntity()
        {
            var sprite = new TextureSprite();//per 1 tick do lerp
            sprite.UpdatePosition (Vector2.Zero);
            sprite.UpdateSize(new Vector2(32, 32));
            Sprite = sprite;
        }

        public bool SetToLerp()
        {
            var newsprite = new LerpSprite();
            newsprite.RenderProperty = Sprite.RenderProperty;
            return true;
        }

        public bool SetToTexture()
        {
            var newsprite = new TextureSprite();
            newsprite.RenderProperty = Sprite.RenderProperty;
            return true;
        }

        public bool SetToText()
        {
            var newsprite = new TextSprite();
            newsprite.RenderProperty = Sprite.RenderProperty;
            return true;
        }

        public virtual bool UpdatePosition(Vector2 Position)
        {
            Sprite.UpdatePosition(Position);
            return true;
        }

        public virtual bool UpdateSize(Vector2 Size)
        {
            Sprite.UpdateSize(Size);
            return true;
        }


        public virtual bool LoadTexture(string asssteName)
        {
            (Sprite.RenderProperty as TextureRenderProperty).Region = TextureManager.GetTextureRegionByName(asssteName);
            return true;
        }
        public bool RemoveTexture(int Index)
        {
            return true;
        }
    }


    public class AnimationProperty : RenderEntity
    {
        public Animation Animation = new Animation();

        public bool Update(int Tick)
        {
            return Animation.Update(Tick);
        }

        public AnimationProperty() 
        {

        }
    }

    public class PhysicEntity : PhysicBase
    {
        public Path PathNodes { get; set; } = new Path();
        public Pioneer Pioneer { get; set; } = new Pioneer();
        public Vector2 Force = new Vector2();//m^2/ms

        public PhysicEntity()
        {

        }

        public override bool UpdateWish(Vector2 Fiction = new Vector2(), int Tick = 10)
        {
            WishForward += Pioneer.Update(Tick , SpeedLength);
            return base.UpdateWish();
        }

        public override bool UpdatePosition()
        {
            return base.UpdatePosition();
        }

        public static Vector2 Crash()
        {
            return new Vector2();
        }
    }

    public abstract class PhysicBase : ILocation
    {
        public Vector2 Position { get ; set; } = Vector2.Zero;
        public Vector2 Size { get ; set; } = new Vector2(32,32);
        public RectF CrashBox
        {
            get
            {
                return new RectF(Position, Size);
            }
        }
        public Vector2 WishForward { get ; set; } = Vector2.Zero;
        public float SpeedLength { get; set ; } = 1f;
        public Vector2 SpeedForward { get; set; } = Vector2.Zero;
        public RectF WishRange
        {
            get
            {
                return RectF.ExpandRectangel(CrashBox, WishForward);
            }
        }
        public Action<IPhysicEntity> OnCrashEvent;
        public bool IsMoving { get; set; } = false;
        public bool IsSizeChanging { get; set; } = false;

        public virtual bool MoveForward(Vector2 _vec)
        {
            WishForward += _vec;
            return true;
        }

        public virtual bool UpdatePosition()
        {
            Position += WishForward;
            if (WishForward != Vector2.Zero)
            {
                IsMoving = true;
            }
            else
            {
                IsMoving = false;
            }
            WishForward = Vector2.Zero;
            return true;
        }

        public virtual bool SetPosition(Vector2 _pos)
        {
            Position = _pos;
            return true;
        }

        public virtual bool UpdateWish(Vector2 Fiction = default, int Tick = 10)
        {
            WishForward += SpeedForward * SpeedLength * Tick;
            return true;
        }

        public virtual bool Crash(IPhysicEntity physicBase)
        {
            if (RectF.IsContain(this.CrashBox, physicBase.PhysicProperty.CrashBox))
            {
                OnCrashEvent(physicBase);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class ZoneEntity
    {
        public ILocation TrigLoacation;
        public bool IsFixedToMap = false;
        public RectF TriggerZone = new RectF(0, 0, 0, 0);
        public event Func<RectF, bool> TriggerDelegate = null;
        public string Description = "Default Zone Entity";
        public int InvokeCount = 0;
        public Stator ZoneStator =new Stator();
        public MessageEntity InformationSource = new();  

        public ZoneEntity(ILocation Loacation , TwoStatus TriggerState)
        {
            TrigLoacation = Loacation;
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
            if (RectF.IsContain(TriggerZone , TrigLoacation.CrashBox))
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
                TriggerDelegate.Invoke(RectF.CrossZone(TriggerZone, TrigLoacation.CrashBox));
                InvokeCount++;
                return true;
            }

        }
    }

    public class MessageEntity
    {

        public ItemInformation MessageItem = ItemInformation.NullItem;
    }
}
