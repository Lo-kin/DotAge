using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotAge.Core.Control;
using DotAge.Core.Model.Dialogue;
using DotAge.Core.View;
using Microsoft.Xna.Framework;

namespace DotAge.Core.Model
{
    public interface IIndexable
    {
        public List<BaseIndex> BelongIndex { get; set; }

        public bool UpdateBelongIndex(RectF CrashBox)
        {
            foreach (var index in BelongIndex.ToArray())
            {
                if (index.IndexRange.Contains(CrashBox) == false)
                {
                    BelongIndex.Remove(index);
                }
            }
            return true;
        }

        public bool RegisterIndex(BaseIndex index)
        {
            if (index != null)
            {
                BelongIndex.Add(index);
                return true;
            }
            return false;
        }

        public bool UnbindAllIndex()
        {
            foreach (var index in BelongIndex.ToArray())
            {
                index.RemoveEntity(this as Entity);
                index.RemoveCrashBox(this as IPhysicEntity);
            }
            BelongIndex.Clear();
            return true;
        }

        public bool UnregisterIndex(BaseIndex index)
        {
            if (index != null)
            {
                return BelongIndex.Remove(index);
            }
            return false;
        }

        public List<Entity> GetRangeEntity()
        {
            return [.. BelongIndex.SelectMany(index => index.EntityIndex).Distinct()];
        }

        public List<IPhysicEntity> GetRangeCrashBox()
        {
            return [.. BelongIndex.SelectMany(index => index.CrashIndex).Distinct()];
        }
    }

    public interface IPhysicEntity
    {
        PhysicBase PhysicProperty { get; set; }

        public bool OnCrash(IPhysicEntity physicEntity)
        {
            return false;
        }

    }

    public interface IRenderEntity
    {
        RenderEntity RenderProperty { get; set; }
        public virtual bool UpdateRender() => true;
    }
    public interface IGameEntity
    {
        GameEntity GameProperty { get; set; }
        public bool IsAlive() => GameProperty.IsAlive;
    }

    class AbstructPhysicEntity : IPhysicEntity
    {
        public PhysicBase PhysicProperty { get; set; }
    }

    public class Entity : IPhysicEntity, IRenderEntity, IGameEntity , IIndexable
    {
        public int ID = -1;
        public EngineAccessor EngineAccess = null;
        public List<Entity> Children = new List<Entity>();
        public Entity Parent = null;
        public PhysicBase PhysicProperty { get; set; } = new PhysicEntity();
        public RenderEntity RenderProperty { get; set; } = new RenderEntity();
        public GameEntity GameProperty { get; set; } = new GameEntity();
        public List<BaseIndex> BelongIndex { get; set; } = new List<BaseIndex>();

        public event Action OnUpdate;

        public bool IsRenderFollowPhysic = true;
        public Vector2 FaceTo { get { return (PhysicProperty as PhysicEntity).Pioneer.Direct; } }

        public Entity(EngineAccessor accessor)
        {
            this.EngineAccess = accessor;

            RenderProperty.LoadTexture("MissingTexture");
            OnUpdate += () => 
            {
                if (IsRenderFollowPhysic == true)
                {
                    RenderProperty.UpdatePosition(PhysicProperty.Position);
                    RenderProperty.UpdateSize(PhysicProperty.Size);
                }
                UpdateBelongIndex();
            };
            PhysicProperty.OnCrashEvent += (pb) => 
            {
                OnCrash(pb);
            };
        }

        public Type GetEntityType(EngineAccessor accessor)
        {
            return GetType();
        }

        public virtual bool Update()//整合更新安排的事件
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

        public virtual void OnCrash(IPhysicEntity BeingCrashedEntity)
        {

        }

        public virtual ItemInformation? OnClick()
        {
            return null;
        }

        public virtual bool Kill(Entity entity)
        {
            if (entity == null)
            {  return false; }
            var parent = this;
            if ( parent != null)
            {
                while (true)
                {
                    if (parent.Parent == null)
                    {
                        break;
                    }
                    else
                    {
                        parent = parent.Parent;
                    }
                }
            }
            parent.GameProperty.KilledEntity.Add(entity);
            entity.GameProperty.BeingKilledEntity = parent;
            if (entity.GameProperty.KilledEntity.Count % 2 == 0)
            {
                parent.GameProperty.SkillPoint++;
            }
            return true;
        }

        public bool UpdateBelongIndex()
        {
            BelongIndex = EngineAccess?.GetRangeIndex(new RectF(PhysicProperty.Position , PhysicProperty.Size)) ?? [];
            return true;
        }

        public bool UnbindAllIndex()
        {
            foreach (var index in BelongIndex.ToArray())
            {
                index.RemoveEntity(this as Entity);
                index.RemoveCrashBox(this as IPhysicEntity);
            }
            BelongIndex.Clear();
            return true;
        }

        public bool RegisterIndex(BaseIndex index)
        {
            if (index != null)
            {
                BelongIndex.Add(index);
                return true;
            }
            return false;
        }

        public bool UnregisterIndex(BaseIndex index)
        {
            if (index != null)
            {
                return BelongIndex.Remove(index);
            }
            return false;
        }

        public List<Entity> GetRangeEntity()
        {
            return [.. BelongIndex.SelectMany(index => index.EntityIndex).Distinct()];
        }

        public List<IPhysicEntity> GetRangeCrashBox()
        {
            return [.. BelongIndex.SelectMany(index => index.CrashIndex).Distinct()];
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
        public Soildre(EngineAccessor accessor) : base(accessor)
        {
            RenderProperty.LoadTexture("TV_Happy");
            
        }

        public override bool Update()
        {
            return base.Update();
        }
    }

    public class Zombie : Creature
    {

        public Zombie(EngineAccessor accessor) : base(accessor)
        {
            RenderProperty.LoadTexture("TV_Normal");

        }

        public override void OnCrash(IPhysicEntity _entity)
        {
            if (_entity is Zombie == false)
            {
                (_entity as Zombie).GameProperty.ModifyHealth(-0f , this);
            }
            base.OnCrash(_entity);
        }

        public override bool Update()
        {
            if (EngineAccess == null)
            {
                return base.Update();
            }
            var target = GetRangeEntity().OfType<Soildre>();
            if (target.Count() != 0)
            {
                (PhysicProperty as PhysicEntity).Pioneer.UpdateDirect(target.First().PhysicProperty.Position - PhysicProperty.Position);
            }
            return base.Update();
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
                            SpeedLength = 5f,
                            Size = new Vector2(8, 8),
                            Pioneer = new Pioneer()
                            {
                                Direct = Vector2.Normalize(AttackTarget.PhysicProperty.Position - this.PhysicProperty.Position)
                            }
                        },
                    };

                    CreateChildEntity(bullet);
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

        public override void OnCrash(IPhysicEntity entity)
        {
            if (entity is Zombie)
            {
                (entity as Zombie).GameProperty.ModifyHealth(-Damage , this);
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
            base.OnCrash(entity); 
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

        public override void OnCrash(IPhysicEntity BeingCrashedEntity)
        {
            if (BeingCrashedEntity is Soildre)
            {
                (BeingCrashedEntity as Soildre).GameProperty.ModifyMoney(1f);
                EngineAccess?.RemoveEntity(this);
            }

            base.OnCrash(BeingCrashedEntity);
        }
    }

    class Door : Entity
    {
        public bool IsOpen = false;
        public bool IsProcessing = false;
        public float process = 0f;

        public Door(EngineAccessor accessor) : base(accessor)
        {
            IsRenderFollowPhysic = false;
            RenderProperty.LoadTexture("MissingTexture");
            RenderProperty.Sprite.Size = new Vector2(32, 64);
            PhysicProperty.Size = new Vector2(32, 64);
        }

        public bool Use()
        {
            if (IsProcessing == true)
            {
                return false;
            }
            IsOpen = !IsOpen;
            if (IsOpen == true)
            {
                PhysicProperty.Size = new Vector2(0 , 0);
                RenderProperty.Sprite.Size = new Vector2(0, 0);
                Thread thread = new Thread(() =>
                {
                    IsProcessing = true;
                    process = 0f;
                    while (process < 1f)
                    {
                        Animating();
                        Thread.Sleep(10);
                    }
                    IsProcessing = false;
                });
                thread.Start();
            }
            else
            {
                RenderProperty.Sprite.Size = new Vector2(32, 64);
                PhysicProperty.Size = new Vector2(32, 64);
                Thread thread = new Thread(() =>
                {
                    IsProcessing = true;
                    process = 0f;
                    while (process < 1f)
                    {
                        Animating();
                        Thread.Sleep(10);
                    }
                    IsProcessing = false;
                });
                thread.Start();
            }
            return true;
        }

        public bool Animating()
        {
            process += 0.01f;
            if (IsOpen == true)
            {
                RenderProperty.Sprite.Size = new Vector2(32 * (1 - process), 64);
                PhysicProperty.Size = new Vector2(32 * (1 - process), 64 * (1 - process));
            }
            else
            {
                RenderProperty.Sprite.Size = new Vector2(32 * process, 64);
                PhysicProperty.Size = new Vector2(32 * process, 64 * process);
            }
            return true;
        }
    }
}
