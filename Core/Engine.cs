using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotAge.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DotAge.Core
{
    internal class Engine
    {
        private Version Engine_Version = new Version(1 , 0);

        public delegate int AddRP(ref RenderProperty rp);
        public event AddRP AddRenderProperty;
        public delegate bool RemoveRP(int Pos);
        public event RemoveRP RemoveRenderProperty;
        public delegate bool ModifyRP(int Pos, RenderProperty rp);
        public event ModifyRP ModifyRenderProperty;

        public (Vector2, float) CrashLoadRange = (new Vector2() , 2);

        public bool GraphicEventAble = false;

        public const int GameTick = 10;
        public int GameTime = 0;

        public Engine()
        {
            if (GameData.GameGroups.Count == 0)
            {
                Group DefaultGroup = new Group();
                DefaultGroup.Name = "DefaultGroup";
                DefaultGroup.TintColor = Color.White;
                DefaultGroup.ID = 0;
                Group RedGroup = new Group();
                RedGroup.Name = "RedGroup";
                RedGroup.TintColor = Color.Red;
                RedGroup.ID = 1;
                Group BlueGroup = new Group();
                BlueGroup.Name = "BlueGroup";
                BlueGroup.TintColor = Color.Blue;
                BlueGroup.ID = 2;
                GameData.AddGroup(DefaultGroup);
                GameData.AddGroup(RedGroup);
                GameData.AddGroup(BlueGroup);
            }
        }

        public bool MainLoop(string[] args)
        {
            while (true)
            {
                
                if (GraphicEventAble)
                {
                    if (GameTime == 0)
                    {
                        Soildre soildre1 = new Soildre();
                        soildre1.Position = new Vector2(100, 100);
                        soildre1.PathNode.AddNode(new Vector2(300, 300));
                        Soildre soildre2 = new Soildre();
                        soildre2.Position = new Vector2(300, 300);
                        soildre2.PathNode.AddNode(new Vector2(100, 100));
                        GameData.AddCreature(soildre1);
                        GameData.AddCreature(soildre2);
                        Soildre soildre3 = new Soildre();
                        soildre3.Position = new Vector2(150, 150);
                        soildre3.PathNode.AddNode(new Vector2(0, 0));
                        GameData.AddCreature(soildre3);
                        for (int i = 0; i < soildre1.RenderCanvas.Length; i++)
                        {
                            AddRenderProperty(ref soildre1.RenderCanvas[i]);
                        }
                        
                        for (int i = 0; i < soildre2.RenderCanvas.Length; i++)
                        {
                            AddRenderProperty(ref soildre2.RenderCanvas[i]);
                        }
                        for (int i = 0; i < soildre3.RenderCanvas.Length; i++)
                        {
                            AddRenderProperty(ref soildre3.RenderCanvas[i]);
                        }
                    }
                    /*
                    if (GameTime == 0)
                    {
                        Turret turret_Red = new Turret();
                        Turret turret_Blue = new Turret();
                        
                        turret_Red.Position = new Vector2(10, 232);
                        turret_Red.ID = 0;
                        turret_Blue.ID = 1;
                        turret_Blue.Position = new Vector2(614, 232);
                        GameData.AddCreature(turret_Red);
                        GameData.AddCreature(turret_Blue);
                        for (int i = 0; i < 8; i++)
                        {
                            AddRenderProperty(ref turret_Blue.RenderCanvas[i]);
                        }
                        for (int i = 0; i < 8; i++)
                        {
                            AddRenderProperty(ref turret_Red.RenderCanvas[i]);
                        }
                        for (int i = 0; i <= 10; i++)
                        {
                            GoldMine goldMine = new GoldMine();
                            Random _r = new Random();
                            var _x = _r.NextSingle() * 1000 % 640;
                            var _y = _r.NextSingle() * 1000 % 480;
                            goldMine.Position = new Vector2(_x, _y);
                            GameData.AddCreature(goldMine);
                            for (int j = 0; j < 8; j++)
                            {
                                AddRenderProperty(ref goldMine.RenderCanvas[j]);
                            }
                            
                        }
                    }
                    
                    if (GameTime % 20 == 1 && GameTime <= 1000)
                    {
                        
                        Random _r = new Random();
                        int flag = (int)_r.NextInt64(1, GameData.GameGroups.Count);
                        
                        Soildre soildre = new Soildre();
                        soildre.Health = GameTime;
                        soildre.SetGroup(flag);
                        if (flag == 1)
                        {
                            soildre.Position = new Vector2(10, 232);

                        }
                        else
                        {
                            soildre.Position = new Vector2(614, 232);

                        }


                        GameData.AddCreature(soildre);
                        
                        for (int i = 0; i < soildre.RenderCanvas.Length; i++)
                        {
                            AddRenderProperty(ref soildre.RenderCanvas[i]);
                        }
                        
                    }
                    */

                    GameIndex.BuildEmptyIndex(CrashLoadRange.Item1, CrashLoadRange.Item2);
                    BaseIndex[,] _index = GameIndex.GetRangeIndex(CrashLoadRange.Item1, CrashLoadRange.Item2);
                    foreach (var item in _index)
                    {
                        if (item.CreatureIndex.Count == 0)
                        {
                            continue;
                        }
                        for (int index1 = 0;index1 <= item.CreatureIndex.Count - 1;index1 ++)
                        {
                            
                            for (int index2 = index1 + 1; index2 <= item.CreatureIndex.Count - 1; index2++)
                            {
                                GameData.GameCreatures[item.CreatureIndex[index1]].UpdatePosition();
                                GameData.GameCreatures[item.CreatureIndex[index2]].UpdatePosition();
                                var IsCrash = GameData.GameCreatures[item.CreatureIndex[index1]].WishRange.IsContain(GameData.GameCreatures[item.CreatureIndex[index2]].WishRange);//先查看预期运动范围是否相交
                                if (IsCrash == true)
                                {
                                    var CrashCount = GameData.GameCreatures[item.CreatureIndex[index1]].CrashBox.ContainCount(GameData.GameCreatures[item.CreatureIndex[index2]].CrashBox);//二者的crashbox属性比较
                                    GameData.GameCreatures[item.CreatureIndex[index1]].WishForward += CrashCount / 2;
                                    GameData.GameCreatures[item.CreatureIndex[index2]].WishForward -= CrashCount / 2;
                                    //IsCrash = CrashCount;
                                }
                            }
                        }
                    }

                    foreach (var item in GameData.GameCreatures.Values)
                    {
                        item.CommitWishForward();
                        for (int i = 0; i < item.RenderCanvas.Length; i++)
                        {
                            ModifyRenderProperty(item.RenderCanvas[i].RenderOrder, item.RenderCanvas[i]);
                        }
                    }
                    GameTime++;
                }
                Thread.Sleep(GameTick);
            }
            
            return true;
        }
        public static Vector2 IsCrash = new Vector2(-1,-1);
        public bool CheckCrash(RectF rectangle1 , RectF rectangle2)
        {
            if (rectangle1.IsContain(rectangle2) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
