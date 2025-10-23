using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DotAge.Core.View;
using Microsoft.Xna.Framework;

namespace DotAge.Core.Model
{
    class Terrain
    {
        public RenderEntity RenderEntity { get; set; } = new RenderEntity();
        public LocationPhysicEntity LocalPhysicEntity { get; set; } = new LocationPhysicEntity();
        //public ZoneEntity ZoneEntity { get; set; } = new ZoneEntity();
        public MessageEntity MessageFrame { get; set; } = new MessageEntity();
        public Terrain()
        {

        }

        public void UpdatePosition()
        {
            RenderEntity.UpdatePosition(LocalPhysicEntity.Position);
            RenderEntity.UpdateSize(LocalPhysicEntity.Size);
        }

        public void Crash(int _EntityID)
        {
            // This method can be overridden by derived classes to handle crash events.
            // Currently, it does nothing.
        }

        
    }

    class VoidBlock : Terrain
    {

    }

    class Grass : Terrain
    {
        public Grass()
        {

        }
    }

    class Boundary : Terrain
    {
        public Boundary()
        {

        }
    }
}
