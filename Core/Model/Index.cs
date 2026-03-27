using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotAge.Core.Model
{
    public class BaseIndex
    {
        public Point IndexPosition { get; set; } = new Point(0, 0);
        public Vector2 IndexSize { get; set; } = new Vector2(1024, 1024);
        public RectF IndexRange { get {return new RectF(IndexPosition.ToVector2() * IndexSize, IndexSize); } }
        public List<TerrainChunk> ChunkIndex { get; set; } = new List<TerrainChunk>();
        public List<Entity> EntityIndex { get; set; } = new List<Entity>();
        public List<IPhysicEntity> CrashIndex { get; set; } = new List<IPhysicEntity>();

        public BaseIndex(Point indexPosition)
        {
            IndexPosition = indexPosition;
        }

        public bool CheckEntity(Entity _entity)
        {
            if (EntityIndex.Contains(_entity) && _entity.PhysicProperty.CrashBox.Contains(IndexRange))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateIndex(out List<Entity> OutIndexEntity , out List<IPhysicEntity> OutIndexCrashBox)
        {
            OutIndexEntity = EntityIndex.Where(e => !IndexRange.Contains(e.PhysicProperty.CrashBox)).ToList();
            EntityIndex = [.. EntityIndex.Except(OutIndexEntity)];
            OutIndexCrashBox = CrashIndex.Where(c => !IndexRange.Contains(c.PhysicProperty.CrashBox)).ToList();
            CrashIndex = [.. CrashIndex.Except(OutIndexCrashBox)];
            return true;
        }

        public bool AddEntity(Entity Entity)
        {
            ArgumentNullException.ThrowIfNull(Entity);
            if (!EntityIndex.Contains(Entity) && IndexRange.Contains(Entity.PhysicProperty.CrashBox))
            {
                EntityIndex.Add(Entity);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveEntity(Entity Entity) 
        {
            if (EntityIndex.Contains(Entity))
            {
                EntityIndex.Remove(Entity);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AddCrashBox(IPhysicEntity CrashBox)
        {
            ArgumentNullException.ThrowIfNull(CrashBox);
            if (!CrashIndex.Contains(CrashBox) && IndexRange.Contains(CrashBox.PhysicProperty.CrashBox))
            {
                CrashIndex.Add(CrashBox);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveCrashBox(IPhysicEntity _crashBox)
        {
            if (CrashIndex.Contains(_crashBox))
            {
                CrashIndex.Remove(_crashBox);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
