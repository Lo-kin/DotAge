using DotAge.Core.Control;
using DotAge.Core.View;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotAge.Core.Model
{
    public class TerrainChunk
    {
        public Point ChunkSize = new Point(32, 32);
        public List<Terrain> Terrains { get; set; } = new List<Terrain>(); 
        public Terrain[,] TerrainMap = new Terrain[0,0];
        public TerrainChunk()
        {
            TerrainMap = new Terrain[ChunkSize.X, ChunkSize.Y];
        }

        public bool AddTerrain(Terrain terrain, Point position)
        {
            if (position.X < 0 || position.X >= ChunkSize.X || position.Y < 0 || position.Y >= ChunkSize.Y)
            {
                return false; // Out of bounds
            }
            if (TerrainMap[position.X, position.Y] != null)
            {
                return false; // Position already occupied
            }
            TerrainMap[position.X, position.Y] = terrain;
            Terrains.Add(terrain);
            return true;
        }

        public bool RemoveTerrain(Terrain terrain)
        {
            for (int x = 0; x < ChunkSize.X; x++)
            {
                for (int y = 0; y < ChunkSize.Y; y++)
                {
                    if (TerrainMap[x, y] == terrain)
                    {
                        TerrainMap[x, y] = null;
                        Terrains.Remove(terrain);
                        return true; // Successfully removed
                    }
                }
            }
            return false; // Terrain not found
        }

        public bool RemoveTerrainAt(Point position)
        {
            if (position.X < 0 || position.X >= ChunkSize.X || position.Y < 0 || position.Y >= ChunkSize.Y)
            {
                return false; // Out of bounds
            }
            Terrain terrain = TerrainMap[position.X, position.Y];
            if (terrain == null)
            {
                return false; // No terrain at this position
            }
            TerrainMap[position.X, position.Y] = null;
            Terrains.Remove(terrain);
            return true; // Successfully removed
        }

    }

    public class Terrain : IRenderEntity , IPhysicEntity
    {
        public RenderEntity RenderProperty { get; set; } = new RenderEntity();
        public PhysicBase PhysicProperty { get; set; } = new LocationEntity();
        public ZoneEntity ZoneEntity { get; set; } = new ZoneEntity(null , Control.TwoStatus.Active);
        public bool IsCrossable { get; set; } = false;
        public Terrain()
        {
            ZoneEntity = new ZoneEntity(PhysicProperty, Control.TwoStatus.Active);
            PhysicProperty.IsSoild = true;
        }

        public void UpdatePosition(Vector2 _position)
        {
            PhysicProperty.Position = _position;
            UpdateRender();
        }

        public void UpdateSize(Vector2 _size)
        {
            PhysicProperty.Size = _size;
        }

        public void UpdateRender()
        {
            RenderProperty.UpdatePosition(PhysicProperty.Position);
        }

        public virtual bool OnCrash(IPhysicEntity physicEntity)
        {
            // This method can be overridden by derived classes to handle crash events.
            // Currently, it does nothing.
            return true;
        }

        public void Update()
        {
            
        }

        public bool SetCrossable(bool status)
        {
            IsCrossable = status;
            if (IsCrossable == false)
            {
                PhysicProperty.Size = new Vector2(32, 32);
            }
            else
            {
                PhysicProperty.Size = Vector2.Zero;
            }
            return true;
        }
    }

    public class VoidBlock : Terrain
    {

    }

    public class Grass : Terrain
    {
        public Grass()
        {

        }
    }

    public class Boundary : Terrain
    {
        public Boundary()
        {

        }
    }

    class Wall : Terrain
    {
        public int crashCount = 100;
        public int LastSoundTime = 0;
        public int SoundCooldown = 50; // Cooldown time in milliseconds
        public Wall()
        {
            PhysicProperty.IsSoild = true;
            RenderProperty.LoadTexture("Blocker");
        }

        public override bool OnCrash(IPhysicEntity BeingCrashedEntity)
        {
            if (BeingCrashedEntity is Zombie)
            { 
                if (crashCount <= 0)
                {
                    RenderProperty.LoadTexture("Gate");
                    PhysicProperty.IsSoild = false;
                }
                else
                {
                    crashCount--;
                }
                if ((BeingCrashedEntity as Entity).EngineAccess.GetGameTime() - LastSoundTime >= SoundCooldown)
                {
                    (BeingCrashedEntity as Entity).EngineAccess.PlaySoundEffect("wood");
                    LastSoundTime = (BeingCrashedEntity as Entity).EngineAccess.GetGameTime();
                }
                
            }
            if (BeingCrashedEntity is Soildre)
            {
                if (crashCount <= 0)
                {
                    RenderProperty.LoadTexture("Blocker");
                    PhysicProperty.IsSoild = true;
                }
                if (crashCount < 100)
                {
                    crashCount++;
                }
            }
            return base.OnCrash(BeingCrashedEntity);
        }
    }


    public class Ground : Terrain
    {
        public Ground()
        {
            RenderProperty.LoadTexture("Steel_Ground");
            SetCrossable(true);
            UpdateRender();
        }
    }

    public class Water : Terrain
    {
        public Water()
        {
            PhysicProperty.IsSoild = true;
            RenderProperty.LoadTexture("Block_White");
            SetCrossable(false);
            UpdateRender();
        }
    }
}
