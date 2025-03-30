using System;
using System.Collections;
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
        private Version Engine_Version = new Version(0 , 1 , 0);

        public delegate int AddRP(ref RenderProperty rp);
        public event AddRP AddRenderProperty;
        public delegate bool RemoveRP(int Pos);
        public event RemoveRP RemoveRenderProperty;
        public delegate bool ModifyRP(int Pos, RenderProperty rp);
        public event ModifyRP ModifyRenderProperty;

        public (Vector2, float) CrashLoadRange = (new Vector2() , 2);

        public bool GraphicEventAble = false;

        public const int GameTick = 20;
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
            float length = 200;
            while (true)
            {
                DateTime StartExcute = DateTime.Now;
                
                if (GraphicEventAble)
                {
                    if (GameTime % 1 == 0 && GameTime <= 1000)
                    {
                        Random random = new Random();  /*
                        for (int i = 0; i<= 50; i++)
                        {*/
                        Soildre soildre1 = new Soildre();
                        //soildre1.RenderPosition = new Vector2(100, 100);
                        //var x = MathF.Cos(MathF.Acos(GameTime));
                        //var y = MathF.Sqrt(1 - (x * x));
                        soildre1.PhysicEntity.Position = new Vector2(320,240);
                        var x = MathF.Cos(GameTime / (1 * MathF.PI));
                        var y = MathF.Sqrt(1 - (x * x));
                        var sign = MathF.Pow(-1, GameTime);
                        var excu = GameTime % 960;
                        if (GameTime >= 480 )
                        {
                            excu -= (GameTime % 480)*2;
                        }
                        
                        soildre1.PhysicEntity.PathNodes.AddNode(new Vector2(320 + x * length * sign, 240 + y * length * sign));
                        soildre1.PhysicEntity.PathNodes.AddNode(new Vector2(320 - x * length * sign, 240 - y * length * sign));
                        GameData.AddCreature(soildre1);
                        AddRenderGroups(soildre1);

                        /*Soildre soildre2 = new Soildre();
                        //soildre1.RenderPosition = new Vector2(100, 100);
                        soildre2.CreaturePhysicEntity.Position = new Vector2(100, 100);
                        soildre2.CreaturePhysicEntity.PathNodes.AddNode(new Vector2(300, 300));
                        soildre2.CreaturePhysicEntity.PathNodes.AddNode(new Vector2(100, 100));
                        GameData.AddCreature(soildre2);
                        AddRenderGroups(soildre2);*/
                    }
                        /*}
                    }*/
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
                    /*
                    GameIndex.BuildEmptyIndex(CrashLoadRange.Item1, CrashLoadRange.Item2);
                    BaseIndex[,] _index = GameIndex.GetRangeIndex(CrashLoadRange.Item1, CrashLoadRange.Item2);//??????没有加入index吗
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

                            }
                        }
                    }
                    */
                    foreach (var item in GameData.GameEntities.Values)
                    {

                        item.PhysicEntity.UpdateWish();
                        object Info = null;
                        //(Vector2, (CrashInfo, CrashInfo))? Info = PhysicEntity.Crash(GameData.GameCreatures.Values.ToArray()[0].CreaturePhysicEntity , GameData.GameCreatures.Values.ToArray()[1].CreaturePhysicEntity);
                        if (Info != null)
                        {/*
                            tessta = Info.Value.Item1.ToString() + "\n" + Info.Value.Item2.Item1.Direct + "\n" + Info.Value.Item2.Item2.Direct;
                            item.CreaturePhysicEntity.WishForward += VectorHelper.Direct(item.CreaturePhysicEntity.WishForward) * Info.Value.Item1 / 2;
                        */}
                        
                        
                        item.PhysicEntity.Update();
                        item.UpdatePosition();

                        ModifyRenderGroups(item);
                    }
                    GameTime++;
                }
                DateTime EndExcute = DateTime.Now;
                TimeSpan ts = EndExcute - StartExcute;
                float SleepTime = 10f;
                if (ts.TotalMilliseconds <= GameTick)
                {
                    Thread.Sleep(GameTick - (int)ts.TotalMilliseconds);
                }
                else
                {
                    SleepTime = (float)ts.TotalMilliseconds;
                }
                tessta = (1000 / SleepTime).ToString();
            }
            
            return true;
        }

        public static string tessta = "";

        private bool AddRenderGroups(Entity _entity)
        {
            for (int j = 0; j < _entity.RenderEntity.RenderCanvas.Length; j++)
            {
                AddRenderProperty(ref _entity.RenderEntity.RenderCanvas[j]);
            }
            return true;
        }

        private bool ModifyRenderGroups(Entity _entity)
        {
            for (int i = 0; i < _entity.RenderEntity.RenderCanvas.Length; i++)
            {
                ModifyRenderProperty(_entity.RenderEntity.RenderCanvas[i].RenderOrder, _entity.RenderEntity.RenderCanvas[i]);
            }
            return true;
        }

        public bool CheckCrash(RectF rectangle1 , RectF rectangle2)
        {
            if (RectF.IsContain(rectangle1,rectangle2) == true)
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
