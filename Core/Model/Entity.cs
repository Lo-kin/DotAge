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
    interface IPhysicEntity
    {
        PhysicEntity PhysicProperty { get; set; }
    }

    interface IRenderEntity
    {
        RenderEntity RenderProperty { get; set; }
    }
    interface IGameEntity
    {
        GameEntity GameProperty { get; set; }
    }

    class AbstructPhysicEntity : IPhysicEntity
    {
        public PhysicEntity PhysicProperty { get; set; }
    }

    class Entity : IPhysicEntity, IRenderEntity, IGameEntity
    {
        public int ID = -1;
        public List<Entity> Children = new List<Entity>();
        public Entity Parent = null;
        public PhysicEntity PhysicProperty { get; set; } = new PhysicEntity();
        public RenderEntity RenderProperty { get; set; } = new RenderEntity();
        public GameEntity GameProperty { get; set; } = new GameEntity();
        public event Action OnUpdate;

        public Entity()
        {
            RenderProperty.LoadTexture("MissingTexture");
            OnUpdate += () => 
            {
                PhysicProperty.Update();
                RenderProperty.UpdatePosition(PhysicProperty.Position);
                RenderProperty.UpdateSize(PhysicProperty.Size);
            };
        }

        public Type GetEntityType()
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

        public virtual void Crash(Entity BeingCrashedEntity)
        {

        }

        public virtual ItemInformation? OnClick()
        {
            return null;
        }
    }

    class Creature : Entity
    {
        public Creature()
        {
            PhysicProperty.Position = new Vector2(0, 0);
            PhysicProperty.Size = new Vector2(32, 32);
        }

    }

    class CityEntity : Entity
    {
        public CityEntity()
        {

            
        }
    }

    class Soildre : Creature
    {
        public Soildre()
        {

        }

        public override void Crash(Entity _entity)
        {
            GameProperty.ModifyHealth(-0.7f);
            base.Crash(_entity);
        }
    }

    class Turret : Creature
    {
        public Turret()
        {
        }
    }

    class Mine : Creature
    {
        public bool Minable = true;
        public float Storage = 100;

        public Mine()
        {

        }

        public virtual void Dig()
        {

        }

    }

    class Bullet : Entity
    {
        public int PierceCount = 1;
        public float Damage = 0.9f;

        public Bullet()
        {
        }

        public override void Crash(Entity entity)
        {
            entity.GameProperty.ModifyHealth(-Damage);
            PierceCount--;
            if (PierceCount <= 0)
            {
                Engine.RemoveEntityList.Contains(entity);
            }
            base.Crash(entity); 
        }
    }

    class GoldMine : Mine
    {
        public GoldMine()
        {
            GameProperty.Name = "Gold_Mine";
        }

        public override void Dig()
        {
            Storage--;
            base.Dig();
        }
    }
}
