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
        public RenderEntity _renderEntity { get; set; } = new RenderEntity();
        public LocationPhysicEntity _localPhysicEntity { get; set; } = new LocationPhysicEntity();
        public Terrain()
        {
            _renderEntity = new RenderEntity(1, new int[] {0 });
            _renderEntity.Canvas.RenderProperties[0].Visibility = true;
        }

        public void UpdatePosition()
        {
            
            _renderEntity.UpdatePosition(_localPhysicEntity.Position);
            _renderEntity.UpdateSize(_localPhysicEntity.Size);
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
            _renderEntity.ChangeFront(TextureManager.GetTextureRegionByName("Character", "Block_White"));
            
            _renderEntity.Canvas.SetAllTint(Color.Green);
        }


    }

    class Boundary : Terrain
    {
        public Boundary()
        {
            _renderEntity.ChangeFront(TextureManager.GetTextureRegionByName("Character", "Boundary_Blue"));
        }
    }
}
