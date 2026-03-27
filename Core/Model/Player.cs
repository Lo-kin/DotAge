using DotAge.Core.Control;
using DotAge.Core.View;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotAge.Core.Model
{
    public class Player
    {
        public EngineAccessor Accessor { get; set; }
        public Entity ControlEntity { get; set; } 
        public Camera2D Camera { get; set; }
        public EntityControler Controler { get; set; }
        public float ViewRange { get; set; } = 100f;
        public Player()
        {

        }

        public Player(Entity entity , EngineAccessor accessor) 
        {
            SetEntity(entity);
            SetAccessor(accessor);
        }

        public bool SetEntity(Entity entity)
        {
            if (ControlEntity != null)
                return false;
            Controler = new EntityControler(entity);
            ControlEntity = entity;
            return true;
        }

        public bool SetAccessor(EngineAccessor accessor)
        {
            if (Accessor != null)
                return false;
            Accessor = accessor;
            return true;
        }
    }
}
