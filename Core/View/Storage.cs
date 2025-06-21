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

namespace DotAge.Core.View
{
    static class GameData
    {
        public static List<EntityControler> EntityControlers = new List<EntityControler>();
        public static EntityControler MainControler = new EntityControler();
        public static Dictionary<int, Group> GameGroups { get; } = new Dictionary<int, Group>();
        public static int DefaultGroupID = 0;
        public static Dictionary<int, Entity> GameEntities { get; } = new Dictionary<int, Entity>();
        public static int MaxGameCreature = 65536;
        public static int MaxGameGroup = 1024;
        public static List<int> EmptyCreatureID = Enumerable.Range(0, MaxGameCreature).ToList();
        public static List<int> EmptyGroupID = Enumerable.Range(0, MaxGameGroup).ToList();

        public static Dictionary<int , UIElement> UIElements = new Dictionary<int, UIElement>();
        public static Dictionary<Point , List<Terrain>> GameMaps = new Dictionary<Point, List<Terrain>>();

        public static List<ZoneEntity> ZoneEntities = new List<ZoneEntity>();

        static GameData()
        {
            Thread thread = new(() => { while (true) { Thread.Sleep(10000); }; }){Name = "test"};
            thread.Start();
        }

        public static bool AddControler(EntityControler _controler)
        {
            if (_controler == null)
            {
                return false;
            }
            else
            {
                if (EntityControlers.Count == 0)
                {
                    MainControler = _controler;
                }
                EntityControlers.Add(_controler);
                return true;
            }
        }

        public static bool AddGroup(Group group)
        {

            if (group == null)
            {
                return false;
            }
            else
            {
                if (GameGroups.ContainsKey(group.ID) || !EmptyGroupID.Contains(group.ID))
                {
                    return false;
                }
                else if (group.ID == -1 && EmptyGroupID.Count != 0)
                {
                    Random random = new Random();
                    int rn = random.Next(0, EmptyGroupID.Count - 1);
                    GameGroups.Add(EmptyGroupID[rn], group);
                    EmptyGroupID.Remove(rn);
                }
                else if (group.ID >= 0 && group.ID < MaxGameGroup && EmptyGroupID.Count != 0)
                {
                    GameGroups.Add(group.ID, group);
                    EmptyGroupID.Remove(group.ID);
                }
                else
                {
                    return false;
                }
                return true;
            }
        }

        public static bool JoinGroup(Creature creature, int GroupID)
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

        public static int AddEntity(Entity _entity)
        {
            if (_entity == null)
            {
                return -1;
            }
            else
            {
                if (_entity.ID >= -1 && _entity.ID < MaxGameCreature)
                {
                    if (EmptyCreatureID.Count == 0)
                    {
                        return -1;
                    }
                    if (_entity.ID == -1 || !EmptyCreatureID.Contains(_entity.ID))
                    {
                        int rn = (int)Random.Shared.NextInt64(0, EmptyCreatureID.Count - 1);
                        GameEntities.Add(EmptyCreatureID[rn], _entity);
                        _entity.ID = EmptyCreatureID[rn];
                        EmptyCreatureID.RemoveAt(rn);
                    }
                    else
                    {
                        GameEntities.Add(_entity.ID, _entity);
                        EmptyCreatureID.Remove(_entity.ID);
                    }
                    GameIndex.SetEntity(_entity);
                    return _entity.ID;
                }
                else
                {
                    return -1;
                }
            }
        }

        public static int AddUIElement(UIElement _ui)
        {
            int t = (int)Random.Shared.NextInt64(0, 1024);
            UIElements.Add(t, _ui);
            return t;
        }

        public static bool RemoveEntity(int _entityID)
        {
            if (GameEntities.Keys.Contains(_entityID))
            {
                GameEntities.Remove(_entityID);
                EmptyCreatureID.Add(_entityID);
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
            Thread thread = new Thread(() => { test(); });
            thread.Name = "test1";
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

        public static bool UpdateEntity(int _entityID)
        {
            foreach (var item in GameData.GameEntities[_entityID].MapIndexes)
            {
                GridIndex[item].EntityIndex.Remove(_entityID);
            }
            GameData.GameEntities[_entityID].MapIndexes = new List<Point>();
            SetEntity(GameData.GameEntities[_entityID]);
            return true;
        }

        public static bool BindIndexToEntity(int _entityID, Point _indexPoint)
        {
            if (GridIndex.ContainsKey(_indexPoint))
            {
                if (!GridIndex[_indexPoint].EntityIndex.Contains(_entityID))
                {
                    GridIndex[_indexPoint].EntityIndex.Add(_entityID);
                    
                }
                GameData.GameEntities[_entityID].AddMapIndex(_indexPoint);
                return true;
            }
            return false;
        }

        public static bool SetEntity(Entity _entity)
        {
            int _ID = _entity.ID;
            var _Size = _entity._physicEntity.Size;
            var _Position = _entity._physicEntity.Position;
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
                    BindIndexToEntity(_ID, tmpPoint);
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
        private static Dictionary<string, TextureProperty> LoadedTextures = new Dictionary<string, TextureProperty>();
        public static List<string> BufferTextureNames = new List<string>();
        public static Dictionary<string, int> TextureLoadPosition = new Dictionary<string, int>();

        static TextureManager()
        {
            LoadTexture("default_texture", 32, 32, 32, 32);
            LoadTexture("Character", 2048, 2048, 32, 32);
            LoadTexture("Status", 512, 512, 32, 32);

            string[] chanames = {
                "MissingTexture" , "Block_White" , "Empty" ,"Mine_Stone" ,"Mine_Gold" ,"Mine_Coal" ,"Tree" ,"Turret_Gun" ,
                "Human_Engineer" , "Bullet_Yellow" ,"Shadow_White" ,"White_EightSIde" ,"Crash_Frame" , "Wihte_Ball" , "Castle_Bright" ,
                "Castle_Dark" , "Boundary_Blue"
            };
            string[] stanames =
            {
                "health_bar_front" , "health_bar_mid" , "health_bar_end" , "health_bar_background" , "health_bar_per" , "shelf"
            };
            LoadedTextures["default_texture"].LoadNames(new string[] { "default" });
            LoadedTextures["Character"].LoadNames(chanames);
            LoadedTextures["Status"].LoadNames(stanames);
        }

        public static bool LoadTexture(string TextureName, int TextureWidth, int TextureHeight, int UnitWidth, int UnitHeight)
        {
            if (LoadedTextures.ContainsKey(TextureName))
            {
                return false;
            }
            else
            {
                if (TextureWidth % UnitWidth != 0 || TextureHeight % UnitHeight != 0)
                {
                    return false;
                }

                var _tmpTextureProperty = new TextureProperty(TextureWidth, TextureHeight, UnitWidth, UnitHeight);
                _tmpTextureProperty.Description = "Texture Name : " + TextureName;
                _tmpTextureProperty.InitTextures();
                LoadedTextures[TextureName] = _tmpTextureProperty;
                for (int i = 0; i < LoadedTextures[TextureName].HeightCount; i++)
                {
                    for (int j = 0; j < LoadedTextures[TextureName].WidthCount; j++)
                    {
                        LoadedTextures[TextureName].Textures[j, i] = new Rectangle(UnitWidth * j, UnitHeight * i, UnitWidth, UnitHeight);
                    }
                }
                BufferTextureNames.Add(TextureName);
                return true;
            }
        }

        public static (int, Rectangle?) GetTextureRegionByName(string TextureName, string TextureRegion)
        {
            if (TextureLoadPosition.ContainsKey(TextureName))
            {
                if (LoadedTextures[TextureName].TextureNames.ContainsKey(TextureRegion))
                {
                    return (TextureLoadPosition[TextureName], LoadedTextures[TextureName].GetTextureByName(TextureRegion));
                }
                else
                {
                    return (-1, null);
                }
            }
            else
            {
                return (-1, null);
            }
        }

        public static (int, Rectangle?) GetTextureRegionByIndex(string TextureName, int TextureIndex)
        {
            if (TextureLoadPosition.ContainsKey(TextureName))
            {
                if (LoadedTextures[TextureName].CheckIndexVaild(TextureIndex))
                {
                    return (TextureLoadPosition[TextureName], LoadedTextures[TextureName].GetTextureX(TextureIndex));
                }
                else
                {
                    return (-1, null);
                }
            }
            else
            {
                return (-1, null);
            }
        }

        public static bool AddAnimation()
        {
            return true;
        }
    }
}
