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

        public bool LoadEntity(Entity entity)
        {
            if (entity == null)
            {
                return false;
            }
            ControlEntity = entity;
            Controler = new(entity);
            return true;
        }
    }
}
