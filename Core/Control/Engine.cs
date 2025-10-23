using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotAge.Core.Model;
using DotAge.Core.Model.Dialogue;
using DotAge.Core.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DotAge.Core.Control
{
    internal class Engine
    {
        private readonly Version Engine_Version = new (0, 2, 0);

        public Graphic CurrentGrapic = null;
        public GameData GameData = new GameData();

        public bool GraphicEventAble = false;
        public bool OverModifyRenderProperty = false;

        public const int GameTick = 20;
        public int GameTime = 0;

        public Engine()
        {
            CurrentGrapic = new Graphic();
            
            Thread GrapicThread = new Thread(() => CurrentGrapic.Run())
            {
                IsBackground = true,
                Name = "GrapicThread"
            };
            GrapicThread.Start();
            
            while (!CurrentGrapic.IsLoaded)
            {
                Thread.Sleep(100);
            }
            GraphicEventAble = true;

            Texture2D defaultTexture = CurrentGrapic.Content.Load<Texture2D>("default_texture");
            Texture2D MainTexture = CurrentGrapic.Content.Load<Texture2D>("Character");
            Texture2D SubTexture = CurrentGrapic.Content.Load<Texture2D>("Status");
            TextureManager.LoadTexture(defaultTexture, 32, 32);
            TextureManager.LoadTexture(MainTexture, 32, 32);
            TextureManager.LoadTexture(SubTexture, 32, 32);
            string[] chanames = {
                "MissingTexture" , "Block_White" , "Empty" ,"Mine_Stone" ,"Mine_Gold" ,"Mine_Coal" ,"Tree" ,"Turret_Gun" ,
                "Human_Engineer" , "Bullet_Yellow" ,"Shadow_White" ,"White_EightSIde" ,"Crash_Frame" , "Wihte_Ball" , "Castle_Bright" ,
                "Castle_Dark" , "Boundary_Blue"
            };
            string[] stanames =
            {
                "bar_lt" , "bar_t" , "bar_rt" ,"bar_lb" , "bar_l" , "bar_rb" ,"bar_l" , "bar_r" , "bar_background" , "bar_per" , "shelf" , "Coin"
            };
            TextureManager.LoadedTextures["default_texture"].LoadNames(new string[] { "default" });
            TextureManager.LoadedTextures["Character"].LoadNames(chanames);
            TextureManager.LoadedTextures["Status"].LoadNames(stanames);
            foreach (var item in TextureManager.LoadedTextures)
            {
                TextureManager.TextureRegions.AddRange(item.Value.TextureRegions.Values);
            }

            List<Terrain> terrainList = new();
            for (int i = -1; i <= 40; i++)
            {
                for (int j = -1; j <= 40; j++)
                {
                    Terrain tmp;
                    if (i == -1 || j == -1 || i == 40 || j == 40)
                    {
                        Boundary boundary = new();
                        boundary.LocalPhysicEntity.Position = new Vector2(i * 32, j * 32);
                        tmp = boundary;
                    }
                    else
                    {
                        Grass grass = new();
                        grass.LocalPhysicEntity.Position = new Vector2(i * 32, j * 32);
                        tmp = grass;
                    }
                    tmp.UpdatePosition();
                    terrainList.Add(tmp);
                }
            }

            Soildre item1 = (Soildre)CreateEntity(new Soildre()
            {
                PhysicProperty = new PhysicEntity()
                {
                    Position = new Vector2(640, 320),
                    Size = new Vector2(32, 32),
                    PathNodes = new Path()
                    {
                        Cycle = false,
                        ForceRay = new Vector2(0, 0)
                    }
                },
            });
            EntityControler entityControler = new(item1);
            GameData.AddControler(entityControler);

            ZoneEntity _moveLeft = new ZoneEntity(Controlers.MouseEntity , TwoStatus.Any)
            {
                Description = "Screen Move To Left",
                TriggerZone = new Margin(0, 0, GameSetting.ScreenWidth - 100, 0).ToRectF(),
            };
            _moveLeft.BindDelegate((p) => { Graphic.ViewCamera.Move(new Vector2(1, 0)); return true; });
            ZoneEntity _moveRight = new ZoneEntity(Controlers.MouseEntity, TwoStatus.Any)
            {
                Description = "Screen Move To Right",

                TriggerZone = new Margin(GameSetting.ScreenWidth - 100, 0, 0, 0).ToRectF(),
            };
            _moveRight.BindDelegate((p) => { Graphic.ViewCamera.Move(new Vector2(-1, 0)); return true; });
            ZoneEntity _moveTop = new ZoneEntity(Controlers.MouseEntity, TwoStatus.Any)
            {
                Description = "Screen Move To Top",

                TriggerZone = new Margin(0, 0, 0, GameSetting.ScreenHeight - 100).ToRectF(),
            };
            _moveTop.BindDelegate((p) => {
                Graphic.ViewCamera.Move(new Vector2(0, 1)); return true;
            });
            ZoneEntity _moveBottom = new ZoneEntity(Controlers.MouseEntity, TwoStatus.Any)
            {
                Description = "Screen Move To Bottom",

                TriggerZone = new Margin(0, GameSetting.ScreenHeight - 100, 0, 0).ToRectF(),
            };
            _moveBottom.BindDelegate((p) => { Graphic.ViewCamera.Move(new Vector2(0, -1)); return true; });
            GameData.ZoneEntities.Add(_moveLeft);
            GameData.ZoneEntities.Add(_moveRight);
            GameData.ZoneEntities.Add(_moveTop);
            GameData.ZoneEntities.Add(_moveBottom);
        }

        public bool MainLoop(string[] args)
        {
            while (true)
            {
                DateTime StartExcute = DateTime.Now;
                //START EXCUTE
                if (GraphicEventAble)
                {
                    foreach (var item in RemoveEntityList)
                    {
                        RemoveEntity(item);
                    }
                    RemoveEntityList.Clear();
                    foreach (var item in CreateEntityList)
                    {
                        CreateEntity(item);
                    }
                    CreateEntityList.Clear();
                    Controlers.Update();
                    foreach (var item in GameData.EntityControlers)
                    {
                        foreach (var key in item.KeyDelegate.Keys)
                        {
                            if (Controlers.ChangeKeyStat[key] == item.KeyDelegate[key].TriggerStat || item.KeyDelegate[key].TriggerStat == TwoStatus.Any)
                            {
                                item.FrameKeys.Add((key , Controlers.ChangeKeyStat[key]));
                            }
                        }
                        foreach (var mouse in item.MouseDelegate.Keys)
                        {
                            if (Controlers.ChangeMouseStat[mouse] == item.MouseDelegate[mouse].TriggerStat || item.MouseDelegate[mouse].TriggerStat == TwoStatus.Any)
                            {
                                item.FrameMouse.Add((mouse , Controlers.ChangeMouseStat[mouse]));
                            }
                        }
                        item.EndFrame();
                    }
                    foreach (var item in GameData.ZoneEntities)
                    {
                        item.Trigger();
                    }
                    foreach (var item in GameData.GameEntities)
                    {
                        item.Update();
                    }
                    GameTime++;
                }
                
                //END EXCUTE
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
                
            }

            return true;
        }

        public static List<Entity> CreateEntityList = new();
        public static List<Entity> RemoveEntityList = new();

        public Entity CreateEntity(Entity entity)
        {
            GameData.AddEntity(entity);
            CurrentGrapic.RenderObjects.Add(entity.RenderProperty.Sprite);
            return entity;
        }

        public bool RemoveEntity(Entity entity)
        {
            if (!GameData.GameEntities.Contains(entity))
            {
                return false;
            }
            CurrentGrapic.RenderObjects.Remove(entity.RenderProperty.Sprite);
            return GameData.RemoveEntity(entity);
        }
    }
}
