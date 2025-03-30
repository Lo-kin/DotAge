using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotAge.Render;
using Microsoft.Xna.Framework;

namespace DotAge.Core
{
    interface IEntity
    {
        Ray ModifyShootingPosition(Ray RalativeRay);
    }


    class Entity
    {
        public int ID = -1;
        public PhysicEntity PhysicEntity { get; set; } = new PhysicEntity();//M
        public RenderEntity RenderEntity { get; set; } = new RenderEntity();//V
        public GameEntity GameEntity { get; set; } = new GameEntity();//C
        public Path PathNode = new Path();
        public Type GetEntityType()
        {
            return this.GetType();
        }
        public virtual bool UpdatePosition()
        {
            RenderEntity.UpdatePosition(PhysicEntity.Position, PhysicEntity.Size);
            return false;
        }
        public virtual void Trigger()
        {

        }
    }

    class Creature : Entity
    {
        public Creature()
        {
            PhysicEntity.Position = new Vector2(0,0);
            PhysicEntity.Size = new Vector2(16, 16);
        }

        public override bool UpdatePosition()
        {
            return base.UpdatePosition();
        }

    }



    class Soildre : Creature
    {
        public Soildre()
        {
            RenderEntity.ChangeFront(TextureName.White_Ball);
            RenderEntity.ChangeBackground(TextureName.Empty);
            RenderEntity.ChangeRalatePosition(new Vector2(0, 5) , 0);
        }

        public override bool UpdatePosition()
        {
                
            return base.UpdatePosition();
        }
    }

    class Turret : Creature
    {
        public Turret()
        {
            RenderEntity.ChangeFront(TextureName.Turret_Gun);
            RenderEntity.ChangeBackground(TextureName.Shadow_White);
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
        public float Damage = 10;

        public Bullet()
        {
            PhysicEntity.Size = new Vector2(4,4);
            RenderEntity.RenderSize = new Vector2(4,4);
            RenderEntity.ChangeFront(TextureName.Bullet_Yellow);
            
        }

        public bool Trigger(int CreatureID)
        {
            ((Creature)GameData.GameEntities[CreatureID]).GameEntity.ModifyHealth(-Damage);
            PierceCount--;
            if (PierceCount <= 0)
            {
                GameData.GameEntities.Remove(this.ID);
            }
            return true;
        }
    }

    class GoldMine : Mine
    {
        public GoldMine()
        {
            RenderEntity.ChangeFront(TextureName.Mine_Gold);
            RenderEntity.ChangeBackground(TextureName.Shadow_White);
            GameEntity.Name = "Gold_Mine";
        }

        public override void Dig()
        {
            Storage--;
            base.Dig();
        }
    }
}
