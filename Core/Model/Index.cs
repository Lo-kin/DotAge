using DotAge.Core.View;
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
        public Vector2 IndexSize { get; set; } = GameSetting.ChunkSize;
        public RectF IndexRange { get { return new RectF(IndexPosition.ToVector2() * IndexSize, IndexSize); } }
        public List<Entity> EntityIndex { get; set; } = [];
        public List<IPhysicEntity> CrashBoxIndex { get; set; } = [];
        public List<IRenderEntity> RenderIndex { get; set; } = [];
        public List<IGameEntity> GameEntityIndex { get; set; } = [];
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

        public bool CheckPoint(Point point)
        {
            if (IndexRange.Contains(point))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateIndex(out List<Entity> OutIndexEntity, out List<IPhysicEntity> OutIndexCrashBox, out List<IRenderEntity> OutIndexRender, out List<IGameEntity> OutIndexTrigger)
        {
            OutIndexEntity = [.. EntityIndex.Where(e => !IndexRange.Contains(e.PhysicProperty.CrashBox))];
            EntityIndex = [.. EntityIndex.Except(OutIndexEntity)];
            OutIndexCrashBox = [.. CrashBoxIndex.Where(c => !IndexRange.Contains(c.PhysicProperty.CrashBox))];
            CrashBoxIndex = [.. CrashBoxIndex.Except(OutIndexCrashBox)];
            OutIndexRender = [.. RenderIndex.Where(c => !IndexRange.Contains(c.RenderProperty.Sprite.BaseProperty.RenderBox))];
            RenderIndex = [.. RenderIndex.Except(OutIndexRender)];
            OutIndexTrigger = [.. GameEntityIndex.Where(c => !IndexRange.Contains(c.GameProperty.Trigger.TriggerLoacation.CrashBox))];
            GameEntityIndex = [.. GameEntityIndex.Except(OutIndexTrigger)];
            return true;
        }

        public bool UpdateEntityIndex(List<Entity> InEntity, out List<Entity> OutEntity)
        {
            List<Entity> InRangeEntity = InEntity.Count > 0 ? [.. InEntity.Where(e => IndexRange.Contains(e.PhysicProperty.CrashBox))] : [];
            EntityIndex.AddRange(InRangeEntity);
            OutEntity = [.. InEntity.Except(InRangeEntity)];
            return true;
        }

        public bool UpdateCrashBoxIndex(List<IPhysicEntity> InCrashBox, out List<IPhysicEntity> OutCrashBox)
        {
            List<IPhysicEntity> InRangeCrashBox = InCrashBox.Count > 0 ? [.. InCrashBox.Where(e => IndexRange.Contains(e.PhysicProperty.CrashBox))] : [];
            CrashBoxIndex.AddRange(InRangeCrashBox);
            OutCrashBox = [.. InCrashBox.Except(InRangeCrashBox)];
            return true;
        }

        public bool UpdateRenderIndex(List<IRenderEntity> InRender, out List<IRenderEntity> OutRender)
        {
            List<IRenderEntity> InRangeRender = InRender.Count > 0 ? [.. InRender.Where(e => IndexRange.Contains(e.RenderProperty.Sprite.BaseProperty.RenderBox))] : [];
            RenderIndex.AddRange(InRangeRender);
            OutRender = [.. InRender.Except(InRangeRender)];
            return true;
        }

        public bool UpdateGameEntityIndex(List<IGameEntity> InGameEntity, out List<IGameEntity> OutGameEntity)
        {
            List<IGameEntity> InRangeGameEntity = InGameEntity.Count > 0 ? [.. InGameEntity.Where(e => IndexRange.Contains(e.GameProperty.Trigger.TriggerLoacation.CrashBox))] : [];
            GameEntityIndex.AddRange(InRangeGameEntity);
            OutGameEntity = [.. InGameEntity.Except(InRangeGameEntity)];
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
            if (!CrashBoxIndex.Contains(CrashBox) && IndexRange.Contains(CrashBox.PhysicProperty.CrashBox))
            {
                CrashBoxIndex.Add(CrashBox);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveCrashBox(IPhysicEntity _crashBox)
        {
            if (CrashBoxIndex.Contains(_crashBox))
            {
                CrashBoxIndex.Remove(_crashBox);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AddRenderEntity(IRenderEntity RenderEntity)
        {
            ArgumentNullException.ThrowIfNull(RenderEntity);
            if (RenderEntity.RenderProperty.Sprite.BaseProperty is TextRenderProperty)
            {
                if (!RenderIndex.Contains(RenderEntity) && IndexRange.Contains(new RectF(RenderEntity.RenderProperty.Sprite.BaseProperty.Position, Vector2.One)))
                {
                    RenderIndex.Add(RenderEntity);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (!RenderIndex.Contains(RenderEntity) && IndexRange.Contains(new RectF(RenderEntity.RenderProperty.Sprite.BaseProperty.Position, (RenderEntity.RenderProperty.Sprite.BaseProperty as TextureRenderProperty).Size)))
                {
                    RenderIndex.Add(RenderEntity);
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        public bool RemoveRenderEntity(IRenderEntity RenderEntity)
        {
            if (RenderIndex.Contains(RenderEntity))
            {
                RenderIndex.Remove(RenderEntity);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AddGameEntity(IGameEntity GameEntity)
        {
            ArgumentNullException.ThrowIfNull(GameEntity);
            if (!GameEntityIndex.Contains(GameEntity) && IndexRange.Contains(GameEntity.GameProperty.Trigger.TriggerLoacation.CrashBox))
            {
                GameEntityIndex.Add(GameEntity);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveGameEntity(IGameEntity GameEntity)
        {
            if (GameEntityIndex.Contains(GameEntity))
            {
                GameEntityIndex.Remove(GameEntity);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
