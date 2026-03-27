using DotAge.Core.Control;
using DotAge.Core.Model;
using DotAge.Core.Model.Delegates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;

namespace DotAge.Core.View
{
    public class GameData
    {
        public List<EntityControler> EntityControlers = new List<EntityControler>();
        public EntityControler MainControler = null;
        public List<Group> GameGroups { get; } = new List<Group>();

        public List<ZoneEntity> ZoneEntities = new List<ZoneEntity>();
        public EntityControler CurrentEntityControler = null;
        public List<TerrainChunk> MapChunks = new List<TerrainChunk>();

        public Player MainPlayer = new Player();
        private List<Entity> Entities = new List<Entity>();
        private List<IPhysicEntity> PhysicEntities = new List<IPhysicEntity>();
        private List<IRenderEntity> RenderEntities = new List<IRenderEntity>();
        private List<IGameEntity> GameEntities = new List<IGameEntity>();
        public List<ILocation> CrashBox = new List<ILocation>();

        public GameIndex GameIndex = new GameIndex();

        static GameData()
        {

        }
        
        public bool SetMainPlayer(Player player)
        {
            if (player == null)
            {
                return false;
            }
            else
            {
                MainPlayer = player;
                AddControler(player.Controler);
                return true;
            }
        }

        public bool AddControler(EntityControler _controler)
        {
            if (_controler == null)
            {
                return false;
            }
            else
            {
                if (EntityControlers.Count == 0)
                {
                    CurrentEntityControler = _controler;
                }
                EntityControlers.Add(_controler);
                return true;
            }
        }

        public bool JoinGroup(Creature creature, Group TargetGroup)
        {
            if (creature == null || TargetGroup == null)
            {
                return false;
            }
            else
            {
                TargetGroup.JoinCreature(ref creature);
                return true;
            }
        }

        public bool AddRenderEntity(IRenderEntity entity)
        {
            if (entity == null)
            {
                return false;
            }
            else
            {
                RenderEntities.Add(entity);
                return true;
            }
        }

        public bool RemoveRenderEntity(IRenderEntity entity)
        {
            if (RenderEntities.Contains(entity))
            {
                RenderEntities.Remove(entity);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AddEntity(Entity Entity)
        {
            if (Entity == null)
            {
                return false;
            }
            else
            {
                GameEntities.Add(Entity);
                AddPhysicEntity(Entity);
                RenderEntities.Add(Entity);
                Entities.Add(Entity);
                GameIndex.SetEntity(Entity);
                return true;
            }
        }

        public bool RemoveEntity(Entity _entity)
        {
            if (GameEntities.Contains(_entity))
            {
                GameEntities.Remove(_entity);
                RemovePhysicEntity(_entity);
                RenderEntities.Remove(_entity);
                Entities.Remove(_entity);
                _entity.UnbindAllIndex();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AddTerrainChunk(TerrainChunk chunk)
        {
            if (chunk == null)
            {
                return false;
            }
            else
            {
                MapChunks.Add(chunk);
                foreach (var terrain in chunk.Terrains)
                {
                    AddPhysicEntity(terrain);
                }
                return true;
            }
        }

        public bool RemoveTerrainChunk(TerrainChunk chunk)
        {
            if (MapChunks.Contains(chunk))
            {
                MapChunks.Remove(chunk);
                foreach (var terrain in chunk.Terrains)
                {
                    RemovePhysicEntity(terrain);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AddPhysicEntity(IPhysicEntity entity)
        {
            if (entity == null)
            {
                return false;
            }
            else
            {
                PhysicEntities.Add(entity);
                GameIndex.SetCrashBox(entity);
                return true;
            }
        }

        public bool RemovePhysicEntity(IPhysicEntity entity)
        {
            if (PhysicEntities.Contains(entity))
            {
                PhysicEntities.Remove(entity);
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<Entity> GetRangeEntity(Vector2 position, float range)
        {
            GameIndex.GetRangeIndex(position, range);
            return null;
        }
    }

    public class GameIndex
    {
        public List<BaseIndex> GridIndex = new List<BaseIndex>();
        private float GridBlockWidth = 32 * 16;//px
        private float GridBlockHeight = 32 * 16;//px

        public GameIndex()
        {
            BuildEmptyIndex(new Vector2(), 2);
        }

        public bool UpdateIndex(Entity entity)
        {
            if (entity.BelongIndex != null)
            {
                for (int i = 0;i < entity.BelongIndex.Count;i++)
                {
                    if (i >= entity.BelongIndex.Count)
                    {
                        break;
                    }
                    var index = entity.BelongIndex[i];
                    if (index.CheckEntity(entity) == false && entity.BelongIndex.Count > 1)
                    {
                        index.EntityIndex.Remove(entity);
                        entity.BelongIndex.Remove(index);
                    }
                }
                SetEntity(entity);
            }
            return true;
        }

        public List<Point> GetRangePoints(Vector2 Position,Vector2 Size)
        {
            List<Point> result = new List<Point>();

            int _xstart = (int)MathF.Floor(Position.X / GridBlockWidth);
            int _ystart = (int)MathF.Floor(Position.Y / GridBlockHeight);
            int _xend = (int)MathF.Floor((Position.X + Size.X) / GridBlockWidth);
            int _yend = (int)MathF.Floor((Position.Y + Size.Y) / GridBlockHeight);
            for (int _yi = _ystart; _yi <= _yend; _yi++)
            {
                for (int _xi = _xstart; _xi <= _xend; _xi++)
                {
                    result.Add(new Point(_xi, _yi));
                }
            }
            return result;
        }

        public bool BuildEmptyIndex(Vector2 Position, float Range)//Rect Index
        {
            float _xstart = Position.X - Range;
            float _ystart = Position.Y - Range;
            float _xend = Position.X + Range;
            float _yend = Position.Y + Range;
            Point[] SEpoints = GetStartEndPoint(new Vector2(_xstart, _ystart), new Vector2(_xend, _yend));
            for (int _y = SEpoints[0].Y; _y <= SEpoints[1].Y; _y++)
            {
                for (int _x = SEpoints[0].X; _x <= SEpoints[1].X; _x++)
                {
                    SetIndex(_x, _y , out BaseIndex _);
                }
            }
            return true;
        }

        public Point[] GetStartEndPoint(Vector2 Start, Vector2 End)
        {
            Point StartPoint = new Point((int)MathF.Floor(Start.X), (int)MathF.Floor(Start.Y));
            Point EndPoint = new Point((int)MathF.Floor(End.X), (int)MathF.Floor(End.Y));
            return new Point[] { StartPoint, EndPoint };
        }

        public BaseIndex CheckVaild(Point point)
        {
            BaseIndex Status = GridIndex.Find(x => x.IndexPosition == point);
            if (Status == null)
            {
                SetIndex(point.X, point.Y , out BaseIndex t);
                return t;
            }
            return Status;
        }

        public BaseIndex PositionGetIndex(Point Pos)
        {
            int _x = (int)MathF.Floor(Pos.X / GridBlockWidth);
            int _y = (int)MathF.Floor(Pos.Y / GridBlockHeight);
            var result = GridIndex.Find(x => x.IndexPosition == Pos);
            if (result != null)
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public List<BaseIndex> RectangleGetIndex(RectF Rect)
        {
            int _xstart = (int)MathF.Floor(Rect.Left / GridBlockWidth);
            int _ystart = (int)MathF.Floor(Rect.Top / GridBlockHeight);
            int _xend = (int)MathF.Floor(Rect.Right / GridBlockWidth);
            int _yend = (int)MathF.Floor(Rect.Bottom / GridBlockHeight);
            List<BaseIndex> result = new List<BaseIndex>();
            for (int _yi = _ystart; _yi <= _yend; _yi++)
            {
                for (int _xi = _xstart; _xi <= _xend; _xi++)
                {
                    result.Add(CheckVaild(new Point(_xi, _yi)));
                }
            }
            return result;
        }

        public List<BaseIndex> GetRangeIndex(Vector2 Position, float Range)
        {
            RectF rectF = new RectF(new Vector2(Position.X - Range, Position.Y - Range), new Vector2(Range * 2, Range * 2));
            return RectangleGetIndex(rectF);
        }

        public List<BaseIndex> GetRangeIndex(RectF Range)
        {
            return RectangleGetIndex(Range);
        }

        public bool SetCrashBox(IPhysicEntity Crashbox)
        {
            foreach (Point Position in GetRangePoints(Crashbox.PhysicProperty.Position , Crashbox.PhysicProperty.Size))
            { 
                BaseIndex index = CheckVaild(Position);
                index.CrashIndex.Add(Crashbox);
                
            }
            return true;
        }

        public bool SetEntity(Entity entity)
        {
            if (entity.BelongIndex == null)
            {
                entity.BelongIndex = new List<BaseIndex>();
            }
            foreach (Point Position in GetRangePoints(entity.PhysicProperty.Position, entity.PhysicProperty.Size))
            {
                BaseIndex index = CheckVaild(Position);
                if (index.CheckEntity(entity) == false)
                {
                    index.EntityIndex.Add(entity);
                }
                if (entity.BelongIndex.Contains(index) == false)
                {
                    entity.BelongIndex.Add(index);
                }
                if (index.CrashIndex.Contains(entity) == false)
                {
                    index.CrashIndex.Add(entity);
                }
            }
            return true;
        }

        public bool SetIndex(int X, int Y, out BaseIndex index)
        {
            index = null;
            if (GridIndex.Find(x => x.IndexPosition == new Point(X, Y)) == null)
            {
                index = new BaseIndex(new Point(X, Y));
                GridIndex.Add(index);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public static class TextureManager
    {
        public static Dictionary<string, TextureProperty> LoadedTextures = new Dictionary<string, TextureProperty>();

        static TextureManager()
        {

        }

        public static bool LoadTexture(Texture2D LoadTexture, int UnitWidth, int UnitHeight)
        {
            if (LoadedTextures.ContainsKey(LoadTexture.Name))
            {
                return false;
            }
            else
            {
                if ((((LoadTexture.Width - 1) % (UnitWidth + 1)) != 0) || ((LoadTexture.Height - 1) % (UnitHeight + 1) != 0))
                {
                    return false;
                }

                var _tmpTextureProperty = new TextureProperty(UnitWidth, UnitHeight , LoadTexture);
                _tmpTextureProperty.Description = "Texture Name : " + LoadTexture.Name;
                LoadedTextures[LoadTexture.Name] = _tmpTextureProperty;
                return true;
            }
        }

        public static TextureRegion GetTextureRegionByName(string TextureName)
        {
            foreach (var textureProperty in LoadedTextures.Values)
            {
                var tmpRegion = textureProperty.GetTextureRegion(TextureName);
                if (tmpRegion != null)
                {
                    return tmpRegion;
                }
            }
            return null;
        }
    }
}
