using DotAge.Core.Model;
using DotAge.Core.Model.Delegates;
using DotAge.Core.Model.Dialogue;
using DotAge.Core.Tools;
using DotAge.Core.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DotAge.Core.Control
{
    public class Engine
    {
        private readonly Version Engine_Version = new(0, 2, 0);
        public EngineAccessor CurrentEngineAccessor = new EngineAccessor();

        public Graphic CurrentGrapic = null;
        public GameData GameData = new GameData();

        public bool GraphicEventAble = false;
        public bool OverModifyRenderProperty = false;

        public const int GameTick = 20;//per 1000 / 20 = 50 Frames/s
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime LastTickTime { get; set; } = DateTime.Now;
        public DateTime CurrentTime { get { return DateTime.Now; } }
        public int GameTime = 0;
        public double TickDurationPercent { get { return (CurrentTime - LastTickTime).TotalMilliseconds / GameTick; } }
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
            for (int y = 0; y < 40; y++)
            {
                for (int x = 0; x < 30; x++)
                {
                    if (x == 5 || y == 5 || x == 25 || y == 15 && x >= 5 && x <= 25 && y <= 15 && y >= 5)
                    {
                        Wall ground = new Wall()
                        {
                            PhysicProperty = new LocationEntity()
                            {
                                Position = new Vector2(x * 32, y * 32),
                                Size = new Vector2(32, 32),
                                IsSoild = true,
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

            GameData.MainPlayer.LoadEntity((Soildre)CreateEntity(new Soildre(CurrentEngineAccessor)
            {
                PhysicProperty = new PhysicEntity()
                {
                    SpeedLength = 0.1f,
                    Position = new Vector2(15 * 32 + 60, 10 * 32),
                    Size = new Vector2(32, 32),
                    IsSoild = true,
                    PathNodes = new Path()
                    {
                        Cycle = false,
                        ForceRay = new Vector2(0, 0)
                    }
                },
            }));

            EntityControler entityControler = GameData.MainPlayer.Controler;
            entityControler.EngineAccess = CurrentEngineAccessor;
            GameData.AddControler(entityControler);

            Door door = (Door)CreateEntity(new Door(CurrentEngineAccessor)
            {
            });
            door.PhysicProperty.Position = new Vector2(32 * 3, 32 * 4);
            door.RenderProperty.UpdatePosition(door.PhysicProperty.Position);
            RemoveEntity(door);


            ZoneEntity _moveLeft = new ZoneEntity(Controlers.MouseEntity, TwoStatus.Any)
            {
                Description = "Screen Move To Left",
                TriggerLoacation = new Location(new Margin(0, 0, GameSetting.ScreenWidth - 100, 0).ToRectF()),
            };
            _moveLeft.BindDelegate((p) => { Graphic.ViewCamera.Move(new Vector2(1, 0)); return true; });
            ZoneEntity _moveRight = new ZoneEntity(Controlers.MouseEntity, TwoStatus.Any)
            {
                Description = "Screen Move To Right",

                TriggerLoacation = new Location(new Margin(GameSetting.ScreenWidth - 100, 0, 0, 0).ToRectF()),
            };
            _moveRight.BindDelegate((p) => { Graphic.ViewCamera.Move(new Vector2(-1, 0)); return true; });
            ZoneEntity _moveTop = new ZoneEntity(Controlers.MouseEntity, TwoStatus.Any)
            {
                Description = "Screen Move To Top",
                TriggerLoacation = new Location(new Margin(0, 0, 0, GameSetting.ScreenHeight - 100).ToRectF()),
            };
            _moveTop.BindDelegate((p) =>
            {
                Graphic.ViewCamera.Move(new Vector2(0, 1)); return true;
            });
            ZoneEntity _moveBottom = new ZoneEntity(Controlers.MouseEntity, TwoStatus.Any)
            {
                Description = "Screen Move To Bottom",
                TriggerLoacation = new Location(new Margin(0, GameSetting.ScreenHeight - 100, 0, 0).ToRectF()),
            };
            _moveBottom.BindDelegate((p) => { Graphic.ViewCamera.Move(new Vector2(0, -1)); return true; });
            GameData.ZoneEntities.Add(_moveLeft);
            GameData.ZoneEntities.Add(_moveRight);
            GameData.ZoneEntities.Add(_moveTop);
            GameData.ZoneEntities.Add(_moveBottom);

            Scripts.Add(new Script(CurrentEngineAccessor));



            StartTime = DateTime.Now;
            LastTickTime = DateTime.Now;


            CurrentGrapic.LoadSoundEffect("hit");
            CurrentGrapic.LoadSoundEffect("shoot");
            CurrentGrapic.LoadSoundEffect("wood");
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
                    Graphic.ScriptInfo.Message = "Day " + (GameTick / 2000f) +
                        "\nHealth : " + GameData.MainPlayer.ControlEntity.GameProperty.Health +
                        "\nSkill Point : " + GameData.MainPlayer.ControlEntity.GameProperty.SkillPoint +
                        "\nDmg : " + GameData.MainPlayer.ControlEntity.GameProperty.Damage +
                        "\nPir : " + GameData.MainPlayer.ControlEntity.GameProperty.PierceCount
                        ;

                    Controlers.Update();
                    foreach (var item in GameData.EntityControlers)
                    {
                        foreach (var key in item.KeyDelegate.Keys)
                        {
                            if (Controlers.ChangeKeyStat[key] == item.KeyDelegate[key].TriggerStat || item.KeyDelegate[key].TriggerStat == TwoStatus.Any)
                            {
                                item.FrameKeys.Add((key, Controlers.ChangeKeyStat[key]));
                            }
                        }
                        foreach (var mouse in item.MouseDelegate.Keys)
                        {
                            if (Controlers.ChangeMouseStat[mouse] == item.MouseDelegate[mouse].TriggerStat || item.MouseDelegate[mouse].TriggerStat == TwoStatus.Any)
                            {
                                item.FrameMouse.Add((mouse, Controlers.ChangeMouseStat[mouse]));
                            }
                        }
                        item.EndFrame();
                    }
                    foreach (var item in GameData.ZoneEntities)
                    {
                        item.Invoke(Controlers.CurrentMousePosition.ToVector2());
                    }

                    Entity Updator = GameData.MainPlayer.ControlEntity;
                    Updator.PhysicProperty.UpdateWish();
                    GameData.GameIndex.UpdateIndex(Updator);

                    Graphic.ViewCamera.Position = MathTool.Floor((GameSetting.ScreenSize / 2) - Updator.RenderProperty.Sprite.BaseProperty.RenderBox.Center);

                    List<Entity> LoadRangeEntity = Updator.GetRangeEntity();
                    List<IRenderEntity> LoadRangeTerrain = Updator.GetRangeRender();

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
                        List<IPhysicEntity> RangeCrash = UpdateEntity.GetIndexRangeCrashBox();
                        for (int j = i + 1; j < RangeCrash.Count; j++)
                        {
                            if (j >= RangeCrash.Count || RangeCrash.Count <= 0)
                            {
                                break;
                            }
                            var BeingUpdateEntity = RangeCrash[j];

                            if (BeingUpdateEntity.PhysicProperty.Size == Vector2.Zero || BeingUpdateEntity == UpdateEntity)
                            {
                                continue;
                            }

                            if (RectF.IsContain(UpdateEntity.PhysicProperty.WishRange, BeingUpdateEntity.PhysicProperty.WishRange))
                            {
                                if (RectF.IsContain(UpdateEntity.PhysicProperty.WishDestination, BeingUpdateEntity.PhysicProperty.WishDestination))
                                {
                                    UpdateEntity.OnCrash(BeingUpdateEntity);
                                    BeingUpdateEntity.OnCrash(UpdateEntity);
                                }
                                if (UpdateEntity.PhysicProperty.IsSoild && BeingUpdateEntity.PhysicProperty.IsSoild)
                                {
                                    RectF CrossZone = RectF.CrossZone(UpdateEntity.PhysicProperty.WishRange, BeingUpdateEntity.PhysicProperty.WishRange);
                                    Vector2 CrossVec = CrossZone.Size;
                                    float uLength = UpdateEntity.PhysicProperty.WishForward.Length();
                                    float bLength = BeingUpdateEntity.PhysicProperty.WishForward.Length();
                                    float rate = uLength / (uLength + bLength);
                                    if (float.IsNaN(rate))
                                    {
                                        rate = 0.5f;
                                    }

                                    Vector2 b_sign = MathTool.VectorSign(UpdateEntity.PhysicProperty.Position - BeingUpdateEntity.PhysicProperty.Position);
                                    float XYFlag = MathF.Abs(UpdateEntity.PhysicProperty.Position.X + UpdateEntity.PhysicProperty.Size.X - BeingUpdateEntity.PhysicProperty.Position.X - BeingUpdateEntity.PhysicProperty.Size.Y) - MathF.Abs(UpdateEntity.PhysicProperty.Position.Y + UpdateEntity.PhysicProperty.Size.Y - BeingUpdateEntity.PhysicProperty.Position.Y - BeingUpdateEntity.PhysicProperty.Size.Y);
                                    if (XYFlag > 0)
                                    {
                                        UpdateEntity.PhysicProperty.WishForward.X += (rate * b_sign.X * CrossVec.X);
                                        BeingUpdateEntity.PhysicProperty.WishForward.X += ((1 - rate) * b_sign.X * CrossVec.X);
                                    }
                                    else if (XYFlag < 0)
                                    {
                                        UpdateEntity.PhysicProperty.WishForward.Y += (rate * b_sign.Y * CrossVec.Y);
                                        BeingUpdateEntity.PhysicProperty.WishForward.Y += ((1 - rate) * b_sign.Y * CrossVec.Y);
                                    }
                                    else
                                    {
                                        UpdateEntity.PhysicProperty.WishForward += (rate * b_sign * CrossVec);
                                        BeingUpdateEntity.PhysicProperty.WishForward += ((1 - rate) * b_sign * CrossVec);
                                    }

                                }

                            }
                        }
                        if (UpdateEntity is Soildre)
                        {
                            CurrentGrapic.WishForce = UpdateEntity.PhysicProperty.WishForward;
                        }
                        UpdateEntity.PhysicProperty.UpdatePosition();
                    }

                    foreach (var item in LoadRangeTerrain)
                    {
                        if (MathF.Ceiling(RectF.DistanceToPoint(item.RenderProperty.Sprite.BaseProperty.RenderBox, Updator.RenderProperty.Sprite.BaseProperty.RenderBox.Center).Length()) > Updator.GameProperty.Sight)
                        {
                            item.RenderProperty.Sprite.BaseProperty.IsVisible = false;
                        }
                        else
                        {
                            item.RenderProperty.Sprite.BaseProperty.IsVisible = true;
                        }
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
                LastTickTime = DateTime.Now;

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
            sprite.BaseProperty.TintColor = Color.White;
            (sprite.BaseProperty as TextRenderProperty).TextSource = messageEntity;
            CurrentGrapic.RenderObjects.Add(sprite);
            return sprite;
        }

        public List<BaseIndex> GetRangeIndex(RectF Range)
        {
            return GameData.GameIndex.GetRangeIndex(Range);
        }

        public bool PlaySound(string name)
        {
            return CurrentGrapic.SoundEffects[name].Play();
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
        public Func<RayF, float, List<IGameEntity>> GetLineIndex;
        public Func<string , bool> PlaySoundEffect;

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
            GetLineIndex = (line , length) =>
            {
                return engine.GameData.GetRayTrigger(line , length);
            };
            PlaySoundEffect = (name) =>
            {
                return engine.PlaySound(name);
             };
            return true;
        }
    }
}
