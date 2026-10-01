using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotAge.Core.Control;
using DotAge.Core.View;
using Microsoft.Xna.Framework;

namespace DotAge.Core.Model
{
    public class Entity : IPhysic, IRender, IGame , ITrigger
    {
        public int ID = -1;
        public EngineAccessor EngineAccess = null;
        public List<Entity> Children = new List<Entity>();
        public Entity Parent = null;
        public Action OnUpdate;
        public Func<Vector2, bool> OnUpdateForce { get ; set ; }
        public Func<IPhysic, bool> OnCrash { get ; set ; }
        public Func<Vector2, bool> OnUpdatePosition { get; set; }
        public Func<Vector2, bool> OnUpdateSize { get; set; }
        public Func<Vector2, bool> OnTrigger { get; set; }
        public Vector2 LastPosition { get ; set ; }
        public Vector2 Speed { get ; set ; }
        public Vector2 Accelerate { get ; set ; }
        public float Mass { get; set; } = 10f;
        public Vector2 FaceForward { get ; set ; }
        public Vector2 WishForward { get ; set ; }
        public Vector2 Position { get ; set ; }
        public Vector2 Size { get ; set ; } = new Vector2(32 , 32);
        public Vector2 Scale { get; set; } = Vector2.One;
        public float Rotation { get; set; } = 0f;
        public ValueClamp<float> Health { get; set; } = new ValueClamp<float>(100, 0, 100);
        public ValueClamp<float> Sight { get ; set ; } = new ValueClamp<float>(float.PositiveInfinity , 20f, 100f);
        public ValueClamp<float> Money { get ; set ; } = new ValueClamp<float>(float.PositiveInfinity , 0, 0);
        public string Name { get ; set ; }
        public bool IsFixedToWindow { get ; set ; }
        public string Description { get ; set ; }
        public Stator TriggerStator { get ; set ; }
        public MessageEntity InformationSource { get ; set ; }
        public RenderProperty RenderProp { get ; set ; }

        public Entity(EngineAccessor accessor)
        {
            this.EngineAccess = accessor;
            var trp = new TextureRenderProperty();
            trp.Frames.InsertFrame(TextureManager.GetTextureRegionByName("MissingTexture"), 1);
            trp.Location.InsertFrame(this, 1);
            RenderProp = trp;
        }

        public virtual bool Update()
        {
            if (OnUpdate != null)
            {
                OnUpdate();
            }
            return true;
        }

        public bool BindChild(Entity entity)
        {
            if (entity != null)
            {
                Children.Add(entity);
                return true;
            }
            return false;
        }

        public bool UnBindChild(Entity entity)
        {
            if (entity != null)
            {
                Children.Remove(entity);
                return true;
            }
            return false;
        }

        public bool BindParent(Entity entity)
        {
            if (entity != null)
            {
                entity.BindChild(this);
                Parent = entity;
                return true;
            }
            return false;
        }

        public bool UnBindParent()
        {
            Parent = null;
            return true;
        }

        public bool CreateChildEntity(Entity entity)
        {
            if (EngineAccess != null)
            {
                if (entity != null)
                {
                    entity.BindParent(this);
                    EngineAccess.AddEntity(entity);
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            return $"Entity ID: {ID}, Type: {GetType().Name}, Position: {Position}, Size: {Size}, IsAlive: {Health.CurrentValue < Health.MinValue}";
        }
    }

    public class Creature : Entity
    {
        public Creature(EngineAccessor accessor) : base(accessor)
        {
            PhysicProperty.Position = new Vector2(0, 0);
            PhysicProperty.Size = new Vector2(32, 32);
            
        }
    }

    class CityEntity : Entity
    {
        public CityEntity(EngineAccessor accessor) : base(accessor)
        {

            
        }
    }

    class Soildre : Creature
    {
        public int BeAttackTime = 0;

        public Soildre(EngineAccessor accessor) : base(accessor)
        {
            PhysicProperty.IsSoild = true;
            RenderProperty = new LerpRenderEntity();
            RenderProperty.LoadTexture("Human_Engineer");
        }

        public override bool Update()
        {
            base.Update();
            if (EngineAccess != null)
            {
                if (EngineAccess.GetGameTime() - BeAttackTime > 10)
                {
                    RenderProperty.Sprite.BaseProperty.TintColor = Color.White;
                }
            }
            if (PhysicProperty.IsMoving == true)
            {
                (RenderProperty as LerpRenderEntity).LoadAsLerp.PushSize(new Vector2(36, 32));
                (RenderProperty as LerpRenderEntity).LoadAsLerp.PushSize(new Vector2(28, 32));
            }
            else
            {
                (RenderProperty as LerpRenderEntity).LoadAsLerp.PushSize(new Vector2(32, 32));
                (RenderProperty as LerpRenderEntity).LoadAsLerp.PushSize(new Vector2(32, 32));
            }
            return true;
        }

        public override void OnHealthChange(float delta , Entity source)
        {
            RenderProperty.Sprite.BaseProperty.TintColor = Color.Red;
            BeAttackTime = EngineAccess.GetGameTime();
            base.OnHealthChange(delta , source);
        }
    }

    public class TargetCore : Creature
    {         
        public TargetCore(EngineAccessor accessor) : base(accessor)
        {
            RenderProperty.LoadTexture("TV_Happy");
            GameProperty.Health = 1000;
        }
    }

    public class Zombie : Creature
    {
        public Entity Target = null;
        public int AttackInterval = 75;
        public int LastAttackTime = 0;
        public int AttackTime = 25;

        public Zombie(EngineAccessor accessor) : base(accessor)
        {
            RenderProperty.LoadTexture("TV_Normal");

        }

        public override bool OnCrash(IPhysicEntity _entity)
        {
            if (_entity is Soildre == true)
            {
                (_entity as Soildre).GameProperty.ModifyHealth(-0f , this);
            }
            return base.OnCrash(_entity);
        }
        
        public override bool Update()
        {
            if (EngineAccess == null)
            {
                return base.Update();
            }
            if (Target == null)
            {
                Target = GetRangeEntity().Find(match => match.GetType() == typeof(Soildre));
            }
            else
            {
                (PhysicProperty as PhysicEntity).Pioneer.UpdateDirect(Target.PhysicProperty.Position - PhysicProperty.Position);
                if( EngineAccess.GetGameTime() - LastAttackTime >= AttackTime)
                {
                    RenderProperty.LoadTexture("TV_Normal");
                }
                if (EngineAccess.GetGameTime() - LastAttackTime >= AttackInterval)
                {
                    int t = 0;
                    GetRangeEntity().Where(entity => entity.GetType() == typeof(Soildre) || entity.GetType() == typeof(Wall)).ToList().ForEach(entity =>
                    {
                        if ((entity.PhysicProperty.Position - PhysicProperty.Position).Length() <= 40)
                        {
                            entity.OnHealthChange(-10f, this);
                            t++;
                        }
                    });
                    if (t > 0)
                    {
                        LastAttackTime = EngineAccess.GetGameTime();
                        RenderProperty.LoadTexture("TV_Bad");
                        EngineAccess.PlaySoundEffect("hit");
                    }

                }
            }
            return base.Update();
        }

        public override bool BeUse(IGameEntity gameEntity)
        {
            RenderProperty.Sprite.BaseProperty.TintColor = Color.Red;
            Target = this;
            return base.BeUse(gameEntity);
        }
    }

    class Turret : Creature
    {
        public Entity AttackTarget = null;
        public float AttackRange = 500;
        public int ShootInterval = 20;
        public int During = 0;
        public int BulletPCount = 1;
        public float BullectDamage = 9;
        public Turret(EngineAccessor accessor) : base(accessor)
        {
            RenderProperty.LoadTexture("Turret_Gun");
        }

        public override bool Update()
        {
            if (AttackTarget == null)
            {
                LoacateTarget();
            }
            else
            {
                if (AttackTarget.GameProperty.IsAlive == false)
                {
                    AttackTarget = null;
                    return base.Update();
                }
                if ((AttackTarget.PhysicProperty.Position - PhysicProperty.Position).Length() > AttackRange)
                {
                    LoacateTarget();
                }
                else
                {
                    During++;
                    if (During < ShootInterval)
                    {
                        return base.Update();
                    }
                    else
                    {
                        During = 0;
                    }
                    var bullet = new Bullet(EngineAccess) 
                    {
                        Parent = this,
                        PierceCount = BulletPCount,
                        Damage = BullectDamage,
                        PhysicProperty = new PhysicEntity()
                        {
                            Position = this.PhysicProperty.Position,
                            SpeedLength = 0.05f,
                            Size = new Vector2(8, 8),
                            Pioneer = new Pioneer()
                            {
                                Direct = Vector2.Normalize(AttackTarget.PhysicProperty.CrashBox.Center - this.PhysicProperty.Position)
                            }
                        },
                    };

                    CreateChildEntity(bullet);
                    EngineAccess.PlaySoundEffect("shoot");
                }
            }
            return base.Update();
        }

        public bool LoacateTarget()
        {
            Entity? ScanEntity = null;
            if (EngineAccess != null)
            {
                foreach (var entity in GetRangeEntity())
                {
                    if (entity is Zombie && entity != this && entity.Children.Contains(this) == false)
                    {
                        if (ScanEntity == null)
                        {
                            if ((entity.PhysicProperty.Position - PhysicProperty.Position).Length() <= AttackRange)
                            {
                                ScanEntity = entity;
                            }
                            continue;
                        }
                        if ((entity.PhysicProperty.Position - ScanEntity.PhysicProperty.Position).Length() <= AttackRange)
                        {
                            ScanEntity = entity;
                        }
                    }
                }
            }
            

            AttackTarget = ScanEntity;
            if (ScanEntity == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    class Mine : Creature
    {
        public bool Minable = true;
        public float Storage = 100;

        public Mine(EngineAccessor accessor) : base(accessor)
        {

        }

        public virtual void Dig()
        {

        }

    }

    class Bullet : Entity
    {
        public int PierceCount = 1;
        public float Damage = 9f;
        public int AliveTime = 300;//frame

        public Bullet(EngineAccessor accessor) : base(accessor)
        {
            RenderProperty.LoadTexture("Bullet_Yellow");
            PhysicProperty.SpeedLength = 0.5f;
        }

        public override bool OnCrash(IPhysicEntity entity)
        {
            if (entity is Zombie)
            {
                (entity as Zombie).OnHealthChange(-Damage , this);
                PierceCount--;
                if (PierceCount <= 0)
                {
                    if (EngineAccess != null)
                    {
                        EngineAccess.RemoveEntity(this);
                    }

                }
            }
            else
            {

            }
            return base.OnCrash(entity); 
        }

        public override bool Update()
        {
            AliveTime--;
            if (AliveTime <= 0)
            {
                if (EngineAccess != null)
                {
                    EngineAccess.RemoveEntity(this);
                }
            }
            return base.Update();
        }
    }

    class GoldMine : Mine
    {
        public GoldMine(EngineAccessor accessor) : base(accessor)
        {
            GameProperty.Name = "Gold_Mine";
            RenderProperty.LoadTexture("MissingTexture");
        }

        public override void Dig()
        {
            Storage--;
            base.Dig();
        }

        public override bool OnCrash(IPhysicEntity BeingCrashedEntity)
        {
            if (BeingCrashedEntity is Soildre)
            {
                (BeingCrashedEntity as Soildre).GameProperty.ModifyMoney(1f);
                EngineAccess?.RemoveEntity(this);
            }

            return base.OnCrash(BeingCrashedEntity);
        }
    }


    class Door : Entity
    {
        public bool IsOpen = false;
        public bool IsProcessing = false;
        public float process = 0f;

        public Door(EngineAccessor accessor) : base(accessor)
        {
            RenderProperty = new LerpRenderEntity();
            (RenderProperty as LerpRenderEntity).LoadAsLerp.AutoLoop = false;
            IsRenderFollowPhysic = true;
            RenderProperty.LoadTexture("MissingTexture");
            (RenderProperty as LerpRenderEntity).LoadAsLerp.InitLerp(new TextureRenderProperty() , new TextureRenderProperty(), 1);
            RenderProperty.UpdateSize(new Vector2(0, 64));
            RenderProperty.UpdateSize(new Vector2(32, 64));
            PhysicProperty.Size = new Vector2(32, 64);
            PhysicProperty.IsSoild = true;
            GameProperty.Trigger.TriggerLoacation = new Location(new Vector2(32 * 3, 32 * 4), PhysicProperty.Size);
        }

        public override bool BeUse(IGameEntity gameEntity)
        {
            if (IsProcessing == true)
            {
                return false;
            }
            IsOpen = !IsOpen;
            if (IsOpen == true)
            {
                PhysicProperty.Size = new Vector2(0 , 0);
                (RenderProperty as LerpRenderEntity).LoadAsLerp.ReverseLerp();
            }
            else
            {
                PhysicProperty.Size = new Vector2(32, 64);
                (RenderProperty as LerpRenderEntity).LoadAsLerp.ReverseLerp(); 
            }
            return true;
        }
    }
}
