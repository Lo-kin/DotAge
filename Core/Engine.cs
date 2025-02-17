using System;
using System.Collections.Generic;
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

                    
                    if (GameTime == 1)
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
                    
                    if (GameTime % 20 == 0 && GameTime <= 1000)
                    {
                        
                        Random _r = new Random();
                        int flag = (int)_r.NextInt64(1, GameData.GameGroups.Count);
                        
                        Soildre soildre = new Soildre();
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
                    
                    foreach (var item in GameData.GameCreatures.Values)
                    {
                        item.UpdatePosition();
                        //BaseIndex[,] _index = GameIndex.RectangleGetIndex(item.WishRange);
                        
                        
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
