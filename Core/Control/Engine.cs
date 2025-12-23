using DotAge.Core.Model;
using DotAge.Core.Model.Delegates;
using DotAge.Core.Model.Dialogue;
using DotAge.Core.Tools;
using DotAge.Core.View;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        public const int GameTick = 20;//per 1000 / 20 = 50 Frames/s
        public int GameTime = 0;
        public double LastLoopTime = 0;

        public List<Script> Scripts = new List<Script>();
        public List<Sprite> FrameSprites = new List<Sprite>();

        public Engine()
        {
            
        }

        public bool Initialize()
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

            Texture2D MainTexture = CurrentGrapic.Content.Load<Texture2D>("Character");
            TextureManager.LoadTexture(MainTexture, 32, 32);
            string[] chanames = {
                "MissingTexture" , "Block_White" ,"Turret_Gun" ,"Human_Engineer" , "Bullet_Yellow"
            };
            TextureManager.LoadedTextures["Character"].LoadNames(chanames);
            TextureManager.LoadedTextures["Character"].LoadName("TV_Normal", new Point(0, 7));
            TextureManager.LoadedTextures["Character"].LoadName("TV_Happy", new Point(0, 8));
            TextureManager.LoadedTextures["Character"].LoadName("TV_Bad", new Point(0, 9));
            TextureManager.LoadedTextures["Character"].LoadName("TV_Leg", new Point(1, 7));
            TextureManager.LoadedTextures["Character"].LoadName("Steel_Ground", new Point(2, 7));
            TextureManager.LoadedTextures["Character"].LoadName("Steel_Material", new Point(3, 7));
            TextureManager.LoadedTextures["Character"].LoadName("Gate", new Point(2, 8));
            TextureManager.LoadedTextures["Character"].LoadName("Blocker", new Point(2, 9));

            TerrainChunk tc = new TerrainChunk();
            for (int y = 0; y < 50; y++)
            {
                for (int x = 0; x < 50; x++)
                {
                    if (x ==3)
                    {
                        Water ground = new Water()
                        {
                            PhysicProperty = new LocationEntity()
                            {
                                Position = new Vector2(x * 32, y * 32),
                                Size = new Vector2(32, 32),
                            },
                        };
                        ground.UpdateRender();
                        tc.Terrains.Add(ground);
                    }
                    else
                    {
                        Ground ground = new Ground()
                        {
                            PhysicProperty = new LocationEntity()
                            {
                                Position = new Vector2(x * 32, y * 32),
                                Size = new Vector2(0, 0),
                            },
                        };
                        ground.UpdateRender();
                        tc.Terrains.Add(ground);
                    }

                }
            }
            AddTerrainChunk(tc);

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



            EntityControler entityControler = new(item1);
            entityControler.EngineAccess = CurrentEngineAccessor;
            GameData.AddControler(entityControler);

            Door door = (Door)CreateEntity(new Door(CurrentEngineAccessor)
            {
            });
            door.PhysicProperty.Position = new Vector2(200, 30);
            door.RenderProperty.Sprite.Position = door.PhysicProperty.Position;
            entityControler.RegisterKey(new ControlerFunc()
            {
                BindKey = Keys.E,
                TriggerStat = TwoStatus.ActiveToFreeze,
                ControlerFuncDelegate = (ClickStat, ClickPos) => {
                    door.Use();
                    return ItemInformation.NullItem;
                },
                ControlerFuncescription = "opendoor"
            });

            ZoneEntity _moveLeft = new ZoneEntity(Controlers.MouseEntity, TwoStatus.Any)
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

            Scripts.Add(new Script(CurrentEngineAccessor));

            GameData.MainPlayer.ControlEntity = item1;

            return true;
        }


        public bool MainLoop(string[] args)
        {
            while (true)
            {
                DateTime StartExcute = DateTime.Now;

                foreach (var script in Scripts)
                {
                    script.Update();
                }

                //START EXCUTE
                if (GraphicEventAble)
                {
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

                    GameData.MainPlayer.ControlEntity.PhysicProperty.UpdateWish();
                    GameData.GameIndex.UpdateIndex(GameData.MainPlayer.ControlEntity);
                    List<Entity> LoadRangeEntity = GameData.MainPlayer.ControlEntity.GetRangeEntity();

                    foreach (Entity item in LoadRangeEntity)
                    {
                        item.PhysicProperty.UpdateWish();
                        GameData.GameIndex.UpdateIndex(item);
                    }
                    for (int i = 0; i < LoadRangeEntity.Count; i++)
                    {
                        if (i >= LoadRangeEntity.Count || LoadRangeEntity.Count <= 0)
                        {
                            break;
                        }
                        Entity UpdateEntity = LoadRangeEntity[i];
                        if (UpdateEntity.PhysicProperty.Size == Vector2.Zero)
                        {
                            continue;
                        }
                        List<IPhysicEntity> RangeCrash = UpdateEntity.GetRangeCrashBox();
                        for (int j = i + 1; j < RangeCrash.Count; j++)
                        {
                            if (j >= RangeCrash.Count || RangeCrash.Count <= 0)
                            {
                                break;
                            }
                            IPhysicEntity BeingUpdateEntity = RangeCrash[j];
                            if (BeingUpdateEntity.PhysicProperty.Size == Vector2.Zero || BeingUpdateEntity == UpdateEntity)
                            {
                                continue;
                            }

                            if (RectF.IsContain(UpdateEntity.PhysicProperty.WishRange , BeingUpdateEntity.PhysicProperty.WishRange))
                            {
                                var crosszone = RectF.CrossZone(UpdateEntity.PhysicProperty.WishRange , BeingUpdateEntity.PhysicProperty.CrashBox);
                                var crossvec = crosszone.Size / 2;
                                var side = RectF.RectDirect(UpdateEntity.PhysicProperty.CrashBox , BeingUpdateEntity.PhysicProperty.CrashBox);
                                var realvec = crossvec * side;
                                UpdateEntity.PhysicProperty.WishForward = MathTool.Project(UpdateEntity.PhysicProperty.WishForward , -realvec);  
                                BeingUpdateEntity.PhysicProperty.WishForward = MathTool.Project(BeingUpdateEntity.PhysicProperty.WishForward , realvec);
                                UpdateEntity.OnCrash(BeingUpdateEntity);
                                BeingUpdateEntity.OnCrash(UpdateEntity);
                            }
                        }
                        UpdateEntity.PhysicProperty.UpdatePosition(); 
                    }
                    
                    foreach (var item in LoadRangeEntity)
                    {
                        item.Update();
                    }   
                    GameTime++;
                }
                
                //END EXCUTE
                DateTime EndExcute = DateTime.Now;
                TimeSpan ts = EndExcute - StartExcute;
                LastLoopTime = ts.TotalMilliseconds;
                CurrentGrapic.CoreTime = LastLoopTime;
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

        public bool AddRenderProperty(IRenderEntity renderEntity)
        {
            if (renderEntity == null)
            {
                return false;
            }
            GameData.AddRenderEntity(renderEntity);
            CurrentGrapic.RenderObjects.Add(renderEntity.RenderProperty.Sprite);
            return true;
        }

        public bool RemoveRenderProperty(IRenderEntity renderEntity)
        {
            if (renderEntity == null)
            {
                return false;
            }
            GameData.RemoveRenderEntity(renderEntity);
            CurrentGrapic.RenderObjects.Remove(renderEntity.RenderProperty.Sprite);
            return true;
        }

        //public bool 

        public Entity CreateEntity(Entity entity)
        {
            if (entity.Parent != null)
            {
                entity.BindParent(entity.Parent);
            }
            entity.EngineAccess = CurrentEngineAccessor;
            entity.Update();
            GameData.AddEntity(entity);
            AddRenderProperty(entity);
            return entity;
        }

        public bool RemoveEntity(Entity entity)
        {
            entity.EngineAccess = null;
            RemoveRenderProperty(entity);
            return GameData.RemoveEntity(entity);
        }

        public bool AddTerrainChunk(TerrainChunk terrainChunk)
        {
            if (terrainChunk == null)
            {
                return false;
            }
            GameData.AddTerrainChunk(terrainChunk);
            foreach (var terrain in terrainChunk.Terrains)
            {
                AddRenderProperty(terrain);
            }
            return true;
        }

        public bool RemoveTerrainChunk(TerrainChunk terrainChunk)
        {
            if (terrainChunk == null)
            {
                return false;
            }
            foreach (var terrain in terrainChunk.Terrains)
            {
                RemoveRenderProperty(terrain);
            }
            return GameData.RemoveTerrainChunk(terrainChunk);
        }

        public TextSprite CreateMessage(MessageEntity messageEntity)
        {
            TextSprite sprite = new TextSprite();
            sprite.TintColor = Color.White;
            sprite.TextSource = messageEntity;
            CurrentGrapic.RenderObjects.Add(sprite);
            return sprite;
        }

        public List<BaseIndex> GetRangeIndex(RectF Range)
        {
            return GameData.GameIndex.GetRangeIndex(Range);
        }
    }

    public class EngineAccessor//将引擎访问器分配给需要访问其他类的类
    {
        public Func<Entity> CreateEntity;
        public Func<Entity , bool> AddEntity;
        public Func<Entity , bool> RemoveEntity;
        public Func<string, TextureRegion> GetTextureRegion;
        public Func<int> GetGameTime;  
        public Func<MessageEntity , TextSprite> AddMessageEntity;
        public Func<RectF, List<BaseIndex>> GetRangeIndex;

        public bool Initialize(Engine engine)
        {
            CreateEntity = () =>
            {
                Entity entity = new Entity(this);
                return entity;
            };
            AddEntity = (entity) =>
            {
                engine.CreateEntity(entity);
                return true;
            };
            RemoveEntity = (entity) =>
            {
                engine.RemoveEntity(entity);
                return true;
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
            GetRangeIndex = (range) =>
            {
                return engine.GetRangeIndex(range);
            };
            return true;
        }
    }
}
