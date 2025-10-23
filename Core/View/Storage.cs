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
    class GameData
    {
        public List<EntityControler> EntityControlers = new List<EntityControler>();
        public EntityControler MainControler = null;
        public Dictionary<int, Group> GameGroups { get; } = new Dictionary<int, Group>();
        public List<Entity> GameEntities { get; } = new List<Entity>();

        public List<ZoneEntity> ZoneEntities = new List<ZoneEntity>();
        public EntityControler CurrentEntityControler = null;

        static GameData()
        {
            Thread thread = new(() => { while (true) { Thread.Sleep(10000); }; }){Name = "test"};
            //thread.Start();
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

        public bool JoinGroup(Creature creature, int GroupID)
        {
            if (creature == null)
            {
                return false;
            }
            else
            {
                if (GameGroups.ContainsKey(GroupID))
                {
                    GameGroups[GroupID].JoinCreature(ref creature);
                }
                return true;
            }
        }

        public int AddEntity(Entity _entity)
        {
            if (_entity == null)
            {
                return -1;
            }
            else
            {
                GameEntities.Add(_entity);
                GameIndex.SetEntity(_entity);
                return _entity.ID;
            }
        }

        public bool RemoveEntity(Entity _entity)
        {
            if (GameEntities.Contains(_entity))
            {
                GameEntities.Remove(_entity);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    static class GameIndex
    {
        public static Dictionary<Point, BaseIndex> GridIndex = new Dictionary<Point, BaseIndex>();
        private static float GridBlockWidth = 32 * 32;//px
        private static float GridBlockHeight = 32 * 32;//px

        static GameIndex()
        {
            BuildEmptyIndex(new Vector2(), 2);
            Thread thread = new Thread(() => { test(); })
            {
                Name = "test1"
            };
            thread.Start();
        }

        public static void test()
        {
            while (true)
            {
                Thread.Sleep(10000);
            }
        }

        public static bool BuildEmptyIndex(Vector2 _position, float _range)//Rect Index
        {
            float _xstart = _position.X - _range;
            float _ystart = _position.Y - _range;
            float _xend = _position.X + _range;
            float _yend = _position.Y + _range;
            Point[] SEpoints = GetStartEndPoint(new Vector2(_xstart, _ystart), new Vector2(_xend, _yend));
            for (int _y = SEpoints[0].Y; _y <= SEpoints[1].Y; _y++)
            {
                for (int _x = SEpoints[0].X; _x <= SEpoints[1].X; _x++)
                {
                    SetIndex(_x, _y, false);
                }
            }
            return true;
        }

        public static Point[] GetStartEndPoint(Vector2 Start, Vector2 End)
        {
            Point StartPoint = new Point((int)MathF.Floor(Start.X), (int)MathF.Floor(Start.Y));
            Point EndPoint = new Point((int)MathF.Floor(End.X), (int)MathF.Floor(End.Y));
            return new Point[] { StartPoint, EndPoint };
        }

        public static bool CheckVaild(Point point)
        {
            return GridIndex.ContainsKey(point);
        }

        public static BaseIndex PositionGetIndex(Vector2 _pos)
        {
            int _x = (int)MathF.Floor(_pos.X / GridBlockWidth);
            int _y = (int)MathF.Floor(_pos.Y / GridBlockHeight);
            if (GridIndex.ContainsKey(new Point(_x, _y)))
            {
                return GridIndex[new Point(_x, _y)];
            }
            else
            {
                return new BaseIndex();
            }
        }

        public static Dictionary<Point , BaseIndex> RectangleGetIndex(RectF _rect)
        {
            int _xstart = (int)MathF.Floor(_rect.Left / GridBlockWidth);
            int _ystart = (int)MathF.Floor(_rect.Top / GridBlockHeight);
            int _xend = (int)MathF.Floor(_rect.Right / GridBlockWidth);
            int _yend = (int)MathF.Floor(_rect.Bottom / GridBlockHeight);
            Dictionary<Point, BaseIndex> result = new Dictionary<Point, BaseIndex>();
            //if (result)
            for (int _yi = _ystart; _yi <= _yend; _yi++)
            {
                for (int _xi = _xstart; _xi <= _xend; _xi++)
                {
                    if (CheckVaild(new Point(_xi, _yi)) == true)
                    {
                        result.Add(new Point(_xi, _yi) , GridIndex[new Point(_xi, _yi)]);
                    }
                    else
                    {
                        GridIndex[new Point(_xi, _yi)] = new BaseIndex();
                        result.Add(new Point(_xi, _yi), GridIndex[new Point(_xi, _yi)]);
                    }
                }
            }
            return result;
        }

        public static Dictionary<Point, BaseIndex> GetRangeIndex(Vector2 _position, float Range)
        {
            RectF rectF = new RectF(new Vector2(_position.X - Range, _position.Y - Range), new Vector2(Range * 2, Range * 2));
            return RectangleGetIndex(rectF);
        }

        public static bool SetEntity(Entity _entity)
        {
            int _ID = _entity.ID;
            var _Size = _entity.PhysicProperty.Size;
            var _Position = _entity.PhysicProperty.Position;
            int _xstart = (int)MathF.Floor(_Position.X / GridBlockWidth);
            int _ystart = (int)MathF.Floor(_Position.Y / GridBlockHeight);
            int _xend = (int)MathF.Floor((_Position.X + _Size.X) / GridBlockWidth);
            int _yend = (int)MathF.Floor((_Position.Y + _Size.Y) / GridBlockHeight);
            for (int _yi = _ystart; _yi <= _yend; _yi++)
            {
                for (int _xi = _xstart; _xi <= _xend; _xi++)
                {
                    var tmpPoint = new Point(_xi, _yi);
                    if (!GridIndex.ContainsKey(tmpPoint))
                    {
                        SetIndex(_xi, _yi, false);
                    }
                }
            }
            return true;
        }

        public static int[] SetIndex(int _x, int _y, bool ForceOverwrite)
        {
            if (GridIndex.ContainsKey(new Point(_x, _y)) && ForceOverwrite == true)
            {
                GridIndex[new Point(_x, _y)] = new BaseIndex();
            }
            else if (!GridIndex.ContainsKey(new Point(_x, _y)))
            {
                GridIndex.Add(new Point(_x, _y), new BaseIndex());
            }
            return new int[0];
        }
    }

    public static class TextureManager
    {
        public static Dictionary<string, TextureProperty> LoadedTextures = new Dictionary<string, TextureProperty>();
        public static List<TextureRegion> TextureRegions = new List<TextureRegion>();

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
                if (LoadTexture.Width % UnitWidth != 0 || LoadTexture.Height % UnitHeight != 0)
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
            if (TextureRegions.Find(x => x.Name == TextureName) != null)
            {
                return TextureRegions.Find(x => x.Name == TextureName);
            }
            else
            {
                return null;
            }
        }

    }
}
