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
        public PhysicEntity _physicEntity { get; set; } = new PhysicEntity();//M
        public RenderEntity _renderEntity { get; set; } = new RenderEntity();//V
        public GameEntity _gameEntity { get; set; } = new GameEntity();//C
        public List<Point> MapIndexes = new List<Point>();

        public Entity()
        {
            _renderEntity = new RenderEntity(16, new int[] { 15});
            _renderEntity.ChangeCanvas(1 , TextureManager.GetTextureRegionByName("Character", "Empty"));
            //RenderEntity.ChangeBackground(TextureManager.GetTextureRegionByName("Character", "Crash_Frame"));
        }

        public Type GetEntityType()
        {
            return GetType();
        }

        public bool AddMapIndex(Point _index)
        {
            if (!MapIndexes.Contains(_index))
            {
                MapIndexes.Add(_index);
                return true;
            }
            return false;
        }

        public virtual bool UpdatePosition()
        {
            _renderEntity.UpdatePosition(_physicEntity.Position);
            
            return false;
        }

        public virtual bool UpdateSize()
        {
            if (_gameEntity.IsRenderFollowCrashbox == true)
            {
                _renderEntity.UpdateSize(_physicEntity.Size);
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
            _rayForce.Position = Controlers.CurrentMapMousePosition.ToVector2();
            _rayForce.Direct = Controlers.CurrentMapMousePosition.ToVector2() - _physicEntity.Position;
            _child._physicEntity.PathNodes.ForceRay = _rayForce.Direct;
            _child._physicEntity.Position = Controlers.CurrentMapMousePosition.ToVector2();
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
            _physicEntity.Position = new Vector2(0, 0);
            _physicEntity.Size = new Vector2(32, 32);
        }

        public override bool UpdatePosition()
        {
            return base.UpdatePosition();
        }

    }

    class CityEntity : Entity
    {
        public CityEntity()
        {
            _renderEntity.ChangeFront(TextureManager.GetTextureRegionByName("Character", "Castle_Bright"));
            
        }
    }

    class Soildre : Creature
    {
        public Soildre()
        {
            _renderEntity.ChangeFront(TextureManager.GetTextureRegionByName("Character" , "Human_Engineer"));
            _physicEntity.Position = new Vector2(0, 0);
            _renderEntity.ChangeRalatePosition(new Vector2(0, -16), 1);
        }

        Paragraph paragraph = new Paragraph()
        {

        };

        public void AddSpeaker()
        {

            Sentence sentence = new Sentence()
            {
                Content = "Hello World , this is a complex content for display our new technologi way.",
                Title = "Test",
                LoopShow = false,
                TimerDueTime = 0,
                TimerPeriod = 75
            };
            Sentence sentence1 = new Sentence()
            {
                Content = "Idk what can i do more.",
                Title = "Test",
                LoopShow = false,
                TimerDueTime = 0,
                TimerPeriod = 75
            };
            Sentence sentence2 = new Sentence()
            {
                Content = "Otherwise , i want to play dota2 more than cs2",
                Title = "Test",
                LoopShow = false,
                TimerDueTime = 0,
                TimerPeriod = 75
            };
            paragraph.Sentences.Add(sentence);
            paragraph.Sentences.Add(sentence1);
            paragraph.Sentences.Add(sentence2);

            paragraph.Start();

        }

        public override bool UpdatePosition()
        {
            paragraph.Update();
            _renderEntity.Canvas.RenderProperties[15].Text = paragraph.GetCurrentContent;
            return base.UpdatePosition();
        }

        public override void Crash(int _EntityID)
        {
            _gameEntity.ModifyHealth(-0.7f);
            base.Crash(_EntityID);
        }

        public override void Trigger()
        {
            
            _gameEntity.ModifyMoney(0);
            base.Trigger();
        }
    }

    class Turret : Creature
    {
        public Turret()
        {
            _renderEntity.ChangeFront(TextureManager.GetTextureRegionByName("Character", "Turret_Gun"));
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
            _gameEntity.IsRenderFollowCrashbox = true;
            _physicEntity.Size = new Vector2(32, 32);

            //RenderEntity.RenderSize = new Vector2(16, 16);
            _renderEntity.ChangeFront(TextureManager.GetTextureRegionByName("Character", "Wihte_Ball"));
            _renderEntity.Canvas.RenderProperties[^1].TintColor = Color.Gray;
            //RenderEntity.ChangeBackground(TextureName.Crash_Frame);

            _physicEntity.PathNodes.IsFollowForceRay = true;
            _physicEntity.PathNodes.Cycle = false;
        }

        public override void Crash(int _EntityID)
        {
            GameData.GameEntities[_EntityID]._gameEntity.ModifyHealth(-Damage);
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
            _renderEntity.ChangeFront(TextureManager.GetTextureRegionByName("Character" , "Mine_Gold"));
            _renderEntity.ChangeBackground(TextureManager.GetTextureRegionByName("Character", "Shadow_White"));
            _gameEntity.Name = "Gold_Mine";
        }

        public override void Dig()
        {
            Storage--;
            base.Dig();
        }
    }
}
