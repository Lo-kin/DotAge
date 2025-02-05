using DotAge.Core;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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
        public static Dictionary<int, Creature> GameCreatures { get; } = new Dictionary<int, Creature>();

        public static Dictionary<int, Mine> GameMines { get; } = new Dictionary<int, Mine>();
        public static int MaxGameCreature = 65536;
        public static int MaxGameGroup = 1024;
        public static List<int> EmptyCreatureID = Enumerable.Range(0, MaxGameCreature).ToList();
        public static List<int> EmptyGroupID = Enumerable.Range(0, MaxGameGroup).ToList();

        static GameData()
        {
            Thread thread = new Thread(() => { test(); });
            thread.Name = "test";
            //thread.Start();
        }

        public static void test()
        {
            while (true) {
                Thread.Sleep(1000);
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
                    GameCreatures.Add(EmptyCreatureID[rn], creature);
                    creature.ID = EmptyCreatureID[rn];
                    EmptyCreatureID.RemoveAt(rn);
                    return true;
                }
                else if (creature.ID >= 0 && EmptyCreatureID.Count != 0)
                {
                    if (EmptyCreatureID.Contains(creature.ID))
                    {
                        GameCreatures.Add(creature.ID, creature);
                        EmptyCreatureID.Remove(creature.ID);
                    }
                    else
                    {
                        Random r = new Random();
                        int rn = (int)r.NextInt64(0, EmptyCreatureID.Count - 1);
                        GameCreatures.Add(EmptyCreatureID[rn], creature);
                        creature.ID = EmptyCreatureID[rn];
                        EmptyCreatureID.RemoveAt(rn);
                    }
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
            if (GameCreatures.Keys.Contains(creatureID))
            {
                GameCreatures.Remove(creatureID);
                EmptyCreatureID.Add(creatureID);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
