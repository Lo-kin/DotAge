using DotAge.Core.Control;
using DotAge.Core.View;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace DotAge.Core.Model
{
    public interface ILocation
    {
        public Func<Vector2, bool> OnUpdatePosition { get; set; }
        public Func<Vector2, bool> OnUpdateSize { get; set; }
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
                OnUpdatePosition?.Invoke(Position);
                return true;
            }
        }

        public bool DeltaPosition(Vector2 delta)
        {
            if (delta == Vector2.Zero || delta.X == float.NaN || delta.Y == float.NaN)
            {
                return false;
            }
            else
            {
                Position += delta;
                OnUpdatePosition?.Invoke(Position);
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
                OnUpdateSize?.Invoke(Size);
                return true;
            }
        }
    }


    public interface IPhysic : ILocation
    {
        public Func<Vector2 , bool> OnUpdateForce { get; set; }
        public Func<IPhysic , bool> OnCrash { get; set; }
        public Vector2 LastPosition { get; set; }
        public Vector2 Speed { get; set; }
        public Vector2 Accelerate { get; set; }
        public float Mass { get; set; }
        public Vector2 FaceForward { get; set; }

        public Vector2 WishForward { get; set; }
        public RectF WishRange
        {
            get
            {
                return RectF.ExpandRectangel(CrashBox, WishForward);
            }
        }
        public RectF WishDestination
        {
            get
            {
                return new RectF(Position + WishForward, Size);
            }
        }
        public Vector2 UpdateWish()
        {
            Speed += Accelerate * Engine.GameTickSecend;
            if (Speed.X != float.NaN && Speed.Y != float.NaN && Speed != Vector2.Zero && Speed != Vector2.Zero)
            {
                Vector2 TickMove = Speed * Engine.GameTickSecend;
                OnUpdatePosition?.Invoke(Position + TickMove);
                return TickMove;   
            }
            return Vector2.Zero;
        }



        public bool AddForce(Vector2 Force)
        {
            if (Force == Vector2.Zero || float.IsNaN(Force.X) || float.IsNaN(Force.Y))
            {
                return false;
            }
            else
            {
                Accelerate += Force / Mass;
                OnUpdateForce?.Invoke(Force);
                return true;
            }
        }

        public bool Crash(IPhysic physic)
        {
            if (RectF.IsContain(this.CrashBox, physic.CrashBox))
            {
                OnCrash?.Invoke(physic);
                return true;
            }
            else
            {
                return false;
            }
        }
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

        public Func<Vector2, bool> OnUpdatePosition { get ; set; }
        public Func<Vector2, bool> OnUpdateSize { get; set; }

        public Location(Vector2 position , Vector2 size)
        {
            Position = position;
            Size = size;
        }

        public Location(RectF rect)
        {
            Position = rect.Position;
            Size = rect.Size;
        }
    }

    interface IClick
    {
        ZoneEntity ClickZone { get; set; }
        ItemInformation? OnClick();
    }

    public class GameEntity
    {
        public Func<float , bool> OnHealthSet { get; set; }
        public Func<float , bool> OnDie { get; set; }
        public Func<float, bool> OnMoneyChange { get; set; }
        public bool IsAlive { get { return Health > 0; } }
        public float LastHealth = 100f;
        public float Health = 100f;
        public float MaxHealth = 500f;

        public string Name { get; set; }

        public float Money = 100f;

        public int Sight = 300;

        public float UseRange = 200f;
        public List<IRenderEntity> SeekedEntitites = new List<IRenderEntity>();

        public ZoneEntity Trigger = new(new Location(Vector2.Zero , Vector2.Zero) , TwoStatus.Active);

        public GameEntity()
        {

        }

        public float SetHealth(float health)
        {
            LastHealth = Health;
            Health = health;
            OnHealthSet?.Invoke(Health);
            if (Health <= 0)
            {
                OnDie?.Invoke(Health);
            }
            return Health;
        }

        public float ModifyHealth(float health)
        {
            if (Health + health > MaxHealth)
            {
                health = MaxHealth - Health;
            }
            SetHealth(Health + health);
            return Health;
        }

        public float ModifyMoney(float _money)
        {
            Money += _money;
            OnMoneyChange?.Invoke(Money);
            return Money;
        }

    }

    public class LerpRenderEntity : IRender
    {
        public Sprite Sprite { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public LerpRenderEntity()
        {
            Sprite = new LerpSprite(new TextureRenderProperty(), new TextureRenderProperty(), 1);//per 1 tick do lerp
            Sprite.BaseProperty.Position = Vector2.Zero;
            (Sprite.BaseProperty as TextureRenderProperty).Size = new Vector2(32, 32);
        }

        public override bool UpdatePosition(Vector2 Position)
        {
            LoadAsLerp.PushPostion(Position);
            return true;
        }

        public override bool UpdateSize(Vector2 Size)
        {
            LoadAsLerp.PushSize(Size);
            return true;
        }

        public bool ReverseLerp()
        {
            LoadAsLerp.ReverseLerp();
            return true;
        }

        public override bool LoadTexture(string asssteName)
        {
            (LoadAsLerp.BaseProperty as TextureRenderProperty).Region = TextureManager.GetTextureRegionByName(asssteName);
            return true;
        }
    }

    interface IRender
    {
        public Sprite Sprite { get; set; }

        /*
        public RenderEntity()
        {
            var sprite = new TextureSprite();//per 1 tick do lerp
            sprite.BaseProperty.Position = Vector2.Zero;
            (sprite.BaseProperty as TextureRenderProperty).Size = new Vector2(32, 32);
            Sprite = sprite;
        }*/

        public virtual bool UpdatePosition(Vector2 Position)
        {
            Sprite.BaseProperty.Position = Position;
            return true;
        }

        public virtual bool UpdateSize(Vector2 Size)
        {
            (Sprite.BaseProperty as TextureRenderProperty).Size = Size;
            return true;
        }


        public virtual bool LoadTexture(string asssteName)
        {
            (Sprite.BaseProperty as TextureRenderProperty).Region = TextureManager.GetTextureRegionByName(asssteName);
            return true;
        }
        public bool RemoveTexture(int Index)
        {

            return true;
        }
    }


    public class AnimationProperty : IRender
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

    public abstract class PhysicEntity : IPhysic
    {
        public Vector2 LastPosition { get; set; } = Vector2.Zero;
        public Vector2 _position = Vector2.Zero;
        public Vector2 Position { get { return _position; } set {LastPosition = _position; _position = value; } }
        public Vector2 Size { get ; set; } = new Vector2(32,32);
        public RectF CrashBox
        {
            get
            {
                return new RectF(Position, Size);
            }
        }
        public Vector2 WishForward = Vector2.Zero;
        public RectF WishRange
        {
            get
            {  
                return RectF.ExpandRectangel(CrashBox, WishForward);
            }
        }
        public RectF WishDestination
        {
            get
            {
                return new RectF(Position + WishForward, Size);
            }
        }
        
        public bool IsMoving { get; set; } = false;
        public bool IsSizeChanging { get; set; } = false;
        public bool IsSoild { get; set; } = false;

        public Vector2 _faceForward = Vector2.One;
        public Vector2 FaceForward { 
            get { return _faceForward; }
            set 
            { 
                if (value != Vector2.Zero) 
                {
                    _faceForward = Vector2.Normalize(value); 
                }
            } 
        }
        public Vector2 Speed { get; set; }
        public Vector2 Accelerate { get; set; }
        public float Mass { get; set; }
        public Func<IPhysicEntity, bool> OnCrashEvent { get; set; }
        public Func<Vector2, bool> OnUpdatePosition { get; set ; }
        public Func<Vector2, bool> OnUpdateSize { get ; set ; }
        public Func<Vector2, bool> OnDeltaPosition { get ; set ; }
        public Func<Vector2, bool> OnUpdateForce { get ; set ; }
        public Func<IPhysic, bool> OnCrash { get; set; }

        public PhysicEntity()
        {
            OnDeltaPosition += (vec) => {WishForward += vec; return true; };

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
    }

    public class ZoneEntity
    {
        public ILocation TriggerLoacation = new Location(Vector2.Zero , Vector2.Zero);
        public bool IsFixedToWindow = false;
        public event Func<Vector2, bool> TriggerDelegate = null;
        public string Description = "Default Zone Entity";
        public int InvokeCount = 0;
        public Stator TriggerStator = new Stator();
        public MessageEntity InformationSource = new();

        public ZoneEntity(ILocation Loacation, TwoStatus TriggerState)
        {
            TriggerLoacation = Loacation;
            TriggerStator = new Stator(TriggerState);
        }

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

        public bool CheckTrigger(Vector2 CheckPosition)
        {
            if (TriggerDelegate == null)
            {
                return false;
            }
            TriggerStator.Update(RectF.IsContain(TriggerLoacation.CrashBox, CheckPosition));
            if (TriggerStator.IsTriggered() == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Invoke(Vector2 InvokePosition)
        {
            if (CheckTrigger(InvokePosition) == false)
            {
                return false;
            }
            else
            {
                TriggerDelegate.Invoke(InvokePosition);
                InvokeCount++;
                return true;
            }
        }

        public bool CheckTrigger(RectF CheckRect)
        {
            if (TriggerDelegate == null)
            {
                return false;
            }
            TriggerStator.Update(RectF.IsContain(TriggerLoacation.CrashBox, CheckRect));
            if (TriggerStator.IsTriggered() == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Invoke(RectF InvokeRect)
        {
            if (CheckTrigger(InvokeRect) == false)
            {
                return false;
            }
            else
            {
                TriggerDelegate.Invoke(TriggerLoacation.CrashBox.Center);
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
