using DotAge.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotAge.Core
{
    static class GameData
    {
        public static Dictionary<int, Group> GameGroups { get; } = new Dictionary<int, Group>();
        public static int DefaultGroupID = 0;
        public static Dictionary<int, Entity> GameEntities { get; } = new Dictionary<int, Entity>();
        public static int MaxGameCreature = 65536;
        public static int MaxGameGroup = 1024;
        public static List<int> EmptyCreatureID = Enumerable.Range(0, MaxGameCreature).ToList();
        public static List<int> EmptyGroupID = Enumerable.Range(0, MaxGameGroup).ToList();

        static GameData()
        {
            Thread thread = new(() => { test(); })
            {
                Name = "test"
            };
            //thread.Start();
        }

        public static void test()
        {
            while (true) {
                Thread.Sleep(10000);
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

        public static bool AddCreature(Creature creature)
        {
            if (creature == null)
            {
                return false;
            }
            else
            {
                if (creature.ID == -1 && EmptyCreatureID.Count != 0)
                {
                    Random r = new Random();
                    int rn = (int)r.NextInt64(0, EmptyCreatureID.Count - 1);
                    GameEntities.Add(EmptyCreatureID[rn], creature);
                    creature.ID = EmptyCreatureID[rn];
                    EmptyCreatureID.RemoveAt(rn);
                    AddToIndex(creature);
                    return true;
                }
                else if (creature.ID >= 0 && EmptyCreatureID.Count != 0)
                {
                    if (EmptyCreatureID.Contains(creature.ID))
                    {
                        GameEntities.Add(creature.ID, creature);
                        EmptyCreatureID.Remove(creature.ID);
                    }
                    else
                    {
                        Random r = new Random();
                        int rn = (int)r.NextInt64(0, EmptyCreatureID.Count - 1);
                        GameEntities.Add(EmptyCreatureID[rn], creature);
                        creature.ID = EmptyCreatureID[rn];
                        EmptyCreatureID.RemoveAt(rn);
                    }
                    AddToIndex(creature);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public static bool RemoveCreature(int creatureID)
        {
            if (GameEntities.Keys.Contains(creatureID))
            {
                GameEntities.Remove(creatureID);
                EmptyCreatureID.Add(creatureID);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool AddToIndex(Creature _creature)
        {
            bool stat = GameIndex.SetCreature(_creature);
            return true;
        }
    }

    static class GameIndex
    {
        public static Dictionary<Point, BaseIndex> GridIndex = new Dictionary<Point, BaseIndex>();
        private static float GridBlockWidth = 32 * 16;//px
        private static float GridBlockHeight = 32 * 16;//px

        static GameIndex()
        {
            BuildEmptyIndex(new Vector2(), 2);
            Thread thread = new Thread(() => { test(); });
            thread.Name = "test1";
            //thread.Start();
        }

        public static void test()
        {
            while (true)
            {
                Thread.Sleep(10000);
            }
        }

        public static bool BuildEmptyIndex(Vector2 _position , float _range)//Rect Index
        {
            float _xstart = _position.X - _range;
            float _ystart = _position.Y - _range;
            float _xend = _position.X + _range;
            float _yend = _position.Y + _range;
            Point[] SEpoints = GetStartEndPoint(new Vector2(_xstart, _ystart) , new Vector2(_xend, _yend));
            for (int _y = SEpoints[0].Y; _y <= SEpoints[1].Y  ; _y++)
            {
                for (int  _x = SEpoints[0].X; _x <= SEpoints[1].X ; _x++)
                {
                    SetIndex(_x, _y , false);
                }
            }
            return true;
        }

        public static Point[] GetStartEndPoint(Vector2 Start , Vector2 End)
        {
            Point StartPoint = new Point((int)MathF.Floor(Start.X) , (int)MathF.Floor(Start.Y));
            Point EndPoint = new Point((int)MathF.Floor(End.X) , (int)MathF.Floor((End.Y)));
            return new Point[] {StartPoint , EndPoint };
        }

        public static bool CheckVaild(Point point)
        {
            return GridIndex.ContainsKey(point);
        }

        public static BaseIndex PositionGetIndex(Vector2 _pos)
        {
            int _x = (int)MathF.Floor(_pos.X / GridBlockWidth);
            int _y = (int)MathF.Floor(_pos.Y / GridBlockHeight);
            if (GridIndex.ContainsKey(new Point(_x,_y)))
            {
                return GridIndex[new Point(_x,_y)];
            }
            else
            {
                return new BaseIndex();
            }
        }

        public static BaseIndex[,] RectangleGetIndex(RectF _rect)
        {
            int _xstart = (int)MathF.Floor(_rect.Left / GridBlockWidth);
            int _ystart = (int)MathF.Floor(_rect.Top / GridBlockHeight);
            int _xend = (int)MathF.Floor(_rect.Right/ GridBlockWidth);
            int _yend = (int)MathF.Floor(_rect.Bottom / GridBlockHeight);
            BaseIndex[,] result = new BaseIndex[_xend - _xstart + 1 , _yend - _ystart + 1];
            //if (result)
            for (int _yi = _ystart; _yi <= _yend; _yi++)
            {
                for (int _xi = _xstart; _xi <= _xend; _xi++)
                {
                    if (CheckVaild(new Point( _xi, _yi)) == true)
                    {
                        result[_xi - _xstart, _yi - _ystart] = GridIndex[new Point(_xi, _yi)];
                    }
                    else
                    {
                        GridIndex[new Point(_xi, _yi)] = new BaseIndex();
                        result[_xi - _xstart, _yi - _ystart] = GridIndex[new Point(_xi, _yi)];
                    }
                }
            }
            return result;
        }

        public static BaseIndex[,] GetRangeIndex(Vector2 _position , float Range)
        {
            RectF rectF = new RectF(new Vector2(_position.X - Range , _position.Y - Range) , new Vector2(Range * 2 , Range * 2));
            return RectangleGetIndex(rectF);
        }

        public static bool SetCreature(Creature _creature)
        {
            int _ID = _creature.ID;
            var _Size = _creature.PhysicEntity.Size;
            var _Position = _creature.PhysicEntity.Position;
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
                        SetIndex(_xi, _yi , false);
                    }
                    GridIndex[tmpPoint].CreatureIndex.Add(_ID);
                }
            }
            return true;
        }

        public static int[] SetIndex(int _x , int _y , bool ForceOverwrite)
        {
            if (GridIndex.ContainsKey(new Point(_x , _y)) && ForceOverwrite == true)
            {
                GridIndex[new Point(_x, _y)] = new BaseIndex();
            }
            else if(!GridIndex.ContainsKey(new Point(_x, _y)))
            {
                GridIndex.Add(new Point(_x, _y), new BaseIndex());
            }
            return new int[0];
        }
    }
}
