using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotAge.Core.Control;
using DotAge.Core.Model.Dialogue;
using DotAge.Core.View;
using Microsoft.Xna.Framework;

namespace DotAge.Core.Model
{
    public interface IPhysicEntity
    {
        PhysicEntity PhysicProperty { get; set; }
    }

    public interface IRenderEntity
    {
        RenderEntity RenderProperty { get; set; }
    }
    public interface IGameEntity
    {
        GameEntity GameProperty { get; set; }
    }

    class AbstructPhysicEntity : IPhysicEntity
    {
        public PhysicEntity PhysicProperty { get; set; }
    }

    public class Entity : IPhysicEntity, IRenderEntity, IGameEntity
    {
        public int ID = -1;
        public EngineAccessor EngineAccess = null;
        public List<Entity> Children = new List<Entity>();
        public Entity Parent = null;
        public PhysicEntity PhysicProperty { get; set; } = new PhysicEntity();
        public RenderEntity RenderProperty { get; set; } = new RenderEntity();
        public GameEntity GameProperty { get; set; } = new GameEntity();
        public event Action OnUpdate;

        public Entity(EngineAccessor accessor)
        {
            PhysicProperty.SourceEntity = this;
            RenderProperty.SourceEntity = this;
            GameProperty.SourceEntity = this;
            this.EngineAccess = accessor;
            RenderProperty.LoadTexture("MissingTexture");
            OnUpdate += () => 
            {
                PhysicProperty.Update();
                RenderProperty.UpdatePosition(PhysicProperty.Position);
                RenderProperty.UpdateSize(PhysicProperty.Size);
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

        public virtual void Crash(Entity BeingCrashedEntity)
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
            var parent = Parent;
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
            parent.GameProperty.KilledEntity.Add(entity);
            entity.GameProperty.BeingKilledEntity = parent;
            return true;
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
        public MessageEntity me = new MessageEntity();
        public Soildre(EngineAccessor accessor) : base(accessor)
        {
            RenderProperty.LoadTexture("Human_Engineer");
            EngineAccess.AddMessageEntity(me);
        }

        public override bool Update()
        {
            me.Message = $"Health={GameProperty.Health}\nMoney={GameProperty.Money}";
            foreach (var item in GameProperty.KilledEntity)
            {
                me.Message += $"\n{item}";
            }
            me.Message += $"\nBeKilled={GameProperty.BeingKilledEntity}";

            return base.Update();
        }
    }

    public class Zombie : Creature
    {
        public Zombie(EngineAccessor accessor) : base(accessor)
        {
            RenderProperty.LoadTexture("Tree");
        }

        public override void Crash(Entity _entity)
        {
            if (_entity is Zombie == false)
            {
                _entity.GameProperty.ModifyHealth(-0.5f , this);
            }
            base.Crash(_entity);
        }

        public override bool Update()
        {
            if (EngineAccess == null)
            {
                return base.Update();
            }
            var target = EngineAccess.GetAllEntities().OfType<Soildre>();
            if (target.Count() != 0)
            {
                PhysicProperty.Pioneer.UpdateDirect(target.First().PhysicProperty.Position - PhysicProperty.Position);
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
                        PhysicProperty = new PhysicEntity()
                        {
                            Position = this.PhysicProperty.Position,
                            SpeedLength = 2f,
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
                foreach (var entity in EngineAccess.GetAllEntities())
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

        public override void Crash(Entity entity)
        {
            if (entity is Zombie)
            {
                entity.GameProperty.ModifyHealth(-Damage , this);
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
            base.Crash(entity); 
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
            RenderProperty.LoadTexture("Mine_Gold");
        }

        public override void Dig()
        {
            Storage--;
            base.Dig();
        }

        public override void Crash(Entity BeingCrashedEntity)
        {
            if (BeingCrashedEntity is GoldMine == false)
            {
                BeingCrashedEntity.GameProperty.ModifyMoney(1f);
                EngineAccess?.RemoveEntity(this);
            }

            base.Crash(BeingCrashedEntity);
        }
    }
}
