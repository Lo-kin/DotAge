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
    public class Engine
    {
        private readonly Version Engine_Version = new (0, 2, 0);
        public EngineAccessor CurrentEngineAccessor = new EngineAccessor();

        public Graphic CurrentGrapic = null;
        public GameData GameData = new GameData();

        public bool GraphicEventAble = false;
        public bool OverModifyRenderProperty = false;

        public const int GameTick = 20;
        public int GameTime = 0;

        public Engine()
        {
            CurrentEngineAccessor.Initialize(this);
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

            Soildre item1 = (Soildre)CreateEntity(new Soildre(CurrentEngineAccessor)
            {
                PhysicProperty = new PhysicEntity()
                {
                    SpeedLength = 0.5f,
                    Position = new Vector2(640, 320),
                    Size = new Vector2(32, 32),
                    PathNodes = new Path()
                    {
                        Cycle = false,
                        ForceRay = new Vector2(0, 0)
                    }
                },
            });


            Zombie item2 = (Zombie)CreateEntity(new Zombie(CurrentEngineAccessor)
            {
                PhysicProperty = new PhysicEntity()
                {
                    SpeedLength= 0.1f,
                    Position = new Vector2(0, 0),
                    Size = new Vector2(32, 32),
                },
                GameProperty = new GameEntity()
                {
                    Health = 500f,
                }
            });

            Turret item3 = (Turret)CreateEntity(new Turret(CurrentEngineAccessor)
            {
                Parent = item1,
                PhysicProperty = new PhysicEntity()
                {
                    Position = new Vector2(800, 320),
                    Size = new Vector2(32, 32),
                },
            });
            EntityControler entityControler = new(item1);
            entityControler.EngineAccess = CurrentEngineAccessor;
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
            Script script = new Script(CurrentEngineAccessor);
            while (true)
            {
                DateTime StartExcute = DateTime.Now;
                if (CurrentEngineAccessor != null)
                {
                    script.Update();
                }
                
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
                        foreach (var item2 in GameData.GameEntities)
                        {
                            if (item != item2)
                            {
                                if (RectF.IsContain(item.PhysicProperty.CrashBox,item2.PhysicProperty.CrashBox))
                                {
                                    item.Crash(item2);
                                }
                            }
                        }
                        if (item.GameProperty.IsAlive == false)
                        {
                            RemoveEntityList.Add(item);
                        }
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

        public List<Entity> CreateEntityList = new();
        public List<Entity> RemoveEntityList = new();

        public Entity CreateEntity(Entity entity)
        {
            entity.EngineAccess = CurrentEngineAccessor;
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
            entity.EngineAccess = null;
            CurrentGrapic.RenderObjects.Remove(entity.RenderProperty.Sprite);
            return GameData.RemoveEntity(entity);
        }

        public bool CreateMessage(MessageEntity messageEntity)
        {
            TextSprite sprite = new TextSprite();
            sprite.TintColor = Color.White;
            sprite.TextSource = messageEntity;
            CurrentGrapic.RenderObjects.Add(sprite);

            return true;
        }
    }

    public class EngineAccessor//将引擎访问器分配给需要访问其他类的类
    {
        public Func<Entity> CreateEntity;
        public Func<Entity , bool> AddEntity;
        public Func<Entity , bool> RemoveEntity;
        public Func<List<Entity>> GetAllEntities;
        public Func<string, TextureRegion> GetTextureRegion;
        public Func<int> GetGameTime;
        public Func<MessageEntity , bool> AddMessageEntity;

        public bool Initialize(Engine engine)
        {
            CreateEntity = () =>
            {
                Entity entity = new Entity(this);
                return entity;
            };
            AddEntity = (entity) =>
            {
                engine.CreateEntityList.Add(entity);
                return true;
            };
            RemoveEntity = (entity) =>
            {
                engine.RemoveEntityList.Add(entity);
                return true;
            };
            GetAllEntities = () =>
            {
                return engine.GameData.GameEntities;
            };
            GetTextureRegion = (name) =>
            {
                return TextureManager.GetTextureRegionByName(name);
            };
            GetGameTime = () =>
            {
                return engine.GameTime;
            };
            AddMessageEntity = (message) =>
            {
                return engine.CreateMessage(message);
            };
            return true;
        }
    }
}
