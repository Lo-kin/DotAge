using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotAge.Core.Control;
using DotAge.Core.View;
using Microsoft.Xna.Framework;

namespace DotAge.Core.Model
{
    interface IEntity
    {
        Ray ModifyShootingPosition(Ray RalativeRay);
    }


    class Entity
    {
        public int ID = -1;
        public List<int> ChildID = new List<int>();
        public int ParentID = -1;
        public delegate bool ExcuteDelegates();
        public PhysicEntity PhysicEntity { get; set; } = new PhysicEntity();//M
        public RenderEntity RenderEntity { get; set; } = new RenderEntity();//V
        public GameEntity GameEntity { get; set; } = new GameEntity();//C

        public Entity()
        {

        }

        public Type GetEntityType()
        {
            return GetType();
        }

        public virtual bool UpdatePosition()
        {
            RenderEntity.UpdatePosition(PhysicEntity.Position);
            return false;
        }

        public virtual bool UpdateSize()
        {
            if (GameEntity.IsRenderFollowCrashbox == true)
            {
                RenderEntity.UpdateSize(PhysicEntity.Size);
            }
            return false;
        }

        public virtual bool Update()//整合更新安排的事件
        {

            return true;
        }

        public bool BindChild(int _childID)
        {
            if (GameData.GameEntities.ContainsKey(_childID))
            {
                GameData.GameEntities[_childID].ParentID = ID;
                ChildID.Add(_childID);
                return true;
            }
            return false;
        }

        public bool UnBindChild(int _childID)
        {
            if (GameData.GameEntities.ContainsKey(_childID))
            {
                GameData.GameEntities[_childID].ParentID = -1;
                ChildID.Remove(_childID);
                return true;
            }
            return false;
        }

        public bool BindParent(int _parentID)
        {
            if (GameData.GameEntities.ContainsKey(_parentID))
            {
                GameData.GameEntities[_parentID].ChildID.Add(ID);
                ParentID = _parentID;
                return true;
            }
            return false;
        }

        public bool UnBindParent(int _parentID)
        {
            if (GameData.GameEntities.ContainsKey(_parentID))
            {
                GameData.GameEntities[_parentID].ChildID.Remove(ID);
                ParentID = -1;
                return true;
            }
            return false;
        }

        public bool CreateChild(Entity _child)
        {
            if (_child == null)
            {
                return false;
            }
            Ray _rayForce = new Ray();
            _rayForce.Position = PhysicEntity.Position;
            _rayForce.Direct = Controlers.CurrentMapMousePosition.ToVector2() - PhysicEntity.Position;
            _child.PhysicEntity.PathNodes.ForceRay = _rayForce.Direct;
            _child.PhysicEntity.Position = PhysicEntity.Position;
            _child.ParentID = ID;
            Engine.CreateEntityList.Add(_child);
            return true;
        }

        public bool InvokeDelegates()
        {
            return true;
        }


        public virtual void Trigger()
        {

        }

        public virtual void Crash(int _EntityID)
        {

        }
    }

    class Creature : Entity
    {
        public Creature()
        {
            PhysicEntity.Position = new Vector2(0, 0);
            PhysicEntity.Size = new Vector2(32, 32);
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
            RenderEntity.ChangeFront(TextureName.Human_Engineer);
            RenderEntity.ChangeBackground(TextureName.Crash_Frame);
            //RenderEntity.ChangeRalatePosition(new Vector2(0, 5), 0);
        }

        public override bool UpdatePosition()
        {

            return base.UpdatePosition();
        }

        public override void Crash(int _EntityID)
        {
            GameEntity.ModifyHealth(0);
            base.Crash(_EntityID);
        }

        public override void Trigger()
        {
            
            GameEntity.ModifyMoney(1);
            base.Trigger();
        }
    }

    class Turret : Creature
    {
        public Turret()
        {
            RenderEntity.ChangeFront(TextureName.Turret_Gun);
            RenderEntity.ChangeBackground(TextureName.Crash_Frame);
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
        public int PierceCount = 1000;
        public float Damage = 10;

        public Bullet()
        {
            GameEntity.IsRenderFollowCrashbox = true;
            PhysicEntity.Size = new Vector2(4, 4);

            //RenderEntity.RenderSize = new Vector2(16, 16);
            RenderEntity.ChangeFront(TextureName.Wihte_Ball);
            RenderEntity.RenderCanvas[^1].TintColor = Color.Gray;
            //RenderEntity.ChangeBackground(TextureName.Crash_Frame);

            PhysicEntity.PathNodes.IsFollowForceRay = true;
            PhysicEntity.PathNodes.Cycle = false;
        }

        public override void Crash(int _EntityID)
        {
            GameData.GameEntities[_EntityID].GameEntity.ModifyHealth(-Damage);
            PierceCount--;
            if (PierceCount <= 0)
            {
                Engine.RemoveEntityList.Add(ID);
            }
            base.Crash(_EntityID); 
        }

        public override void Trigger()
        {

            
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
