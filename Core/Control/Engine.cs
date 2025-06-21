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
        private readonly Version Engine_Version = new (0, 1, 0);

        public delegate int AddRP(ref RenderProperty rp);
        public event AddRP AddRenderProperty;
        public delegate bool RemoveRP(int Pos);
        public event RemoveRP RemoveRenderProperty;
        public delegate bool ModifyRP(int Pos, RenderProperty rp);
        public event ModifyRP ModifyRenderProperty;

        public bool GraphicEventAble = false;
        public bool OverModifyRenderProperty = false;
        public static bool TextureLoadedFlag = false;

        public const int GameTick = 20;
        public int GameTime = 0;

        public static int LoadRange = 1000;

        public Engine()
        {
            if (GameData.GameGroups.Count == 0)
            {
                Group DefaultGroup = new()
                {
                    Name = "DefaultGroup",
                    TintColor = Color.White,
                    ID = 0
                };
                Group RedGroup = new()
                {
                    Name = "RedGroup",
                    TintColor = Color.Red,
                    ID = 1
                };
                Group BlueGroup = new()
                {
                    Name = "BlueGroup",
                    TintColor = Color.Blue,
                    ID = 2
                };
                GameData.AddGroup(DefaultGroup);
                GameData.AddGroup(RedGroup);
                GameData.AddGroup(BlueGroup);
            }
        }

        public bool MainLoop(string[] args)
        {
            while (true)
            {
                DateTime StartExcute = DateTime.Now;
                //START EXCUTE
                if (GraphicEventAble && TextureLoadedFlag)
                {
                    if (GameTime == 0)
                    {
                        ZoneEntity _moveLeft = new ZoneEntity()
                        {
                            Description = "Screen Move To Left",
                            Condition = TwoStat.Any,
                            TriggerZone = new Margin(0, 0, GameSetting.ScreenWidth - 100, 0).ToRectF(),
                        };
                        _moveLeft.BindDelegate((p) => { Graphic.ViewCamera.Move(new Vector2(1, 0)); return true; });
                        ZoneEntity _moveRight = new ZoneEntity()
                        {
                            Description = "Screen Move To Right",
                            Condition = TwoStat.Any,
                            TriggerZone = new Margin(GameSetting.ScreenWidth - 100, 0, 0, 0).ToRectF(),
                        };
                        _moveRight.BindDelegate((p) => { Graphic.ViewCamera.Move(new Vector2(-1, 0)); return true; });
                        ZoneEntity _moveTop = new ZoneEntity()
                        {
                            Description = "Screen Move To Top",
                            Condition = TwoStat.Any,
                            TriggerZone = new Margin(0, 0, 0, GameSetting.ScreenHeight - 100).ToRectF(),
                        };
                        _moveTop.BindDelegate((p) => { Graphic.ViewCamera.Move(new Vector2(0, 1)); return true; });
                        ZoneEntity _moveBottom = new ZoneEntity()
                        {
                            Description = "Screen Move To Bottom",
                            Condition = TwoStat.Any,
                            TriggerZone = new Margin(0, GameSetting.ScreenHeight - 100, 0, 0).ToRectF(),
                        };
                        _moveBottom.BindDelegate((p) => { Graphic.ViewCamera.Move(new Vector2(0, -1)); return true; });
                        GameData.ZoneEntities.Add(_moveLeft);
                        GameData.ZoneEntities.Add(_moveRight);
                        GameData.ZoneEntities.Add(_moveTop);
                        GameData.ZoneEntities.Add(_moveBottom);

                        HealthBar healthBar = new HealthBar();
                        AddRenderGroups(healthBar);
                        Shelf shelf = new Shelf(5,10);
                        AddRenderGroups(shelf);
                        GameData.AddUIElement(healthBar); 
                        GameData.AddUIElement(shelf);
                        shelf.AddItem(0, 0, "Bullet_Yellow" , 100);
                        shelf.AddItem(4, 0, "Bullet_Yellow" , 0);
                        shelf.AddItem(3, 6, "Bullet_Yellow" , 10);
                        List<Terrain> terrainList = new();
                        for (int i = -1; i <= 40; i++)
                        {
                            for (int j = -1; j <= 40; j++)
                            {
                                Terrain tmp;
                                if (i == -1 || j == -1 || i == 40 || j == 40)
                                {
                                    Boundary boundary = new();
                                    boundary._localPhysicEntity.Position = new Vector2(i * 32, j * 32);
                                    tmp = boundary;
                                }
                                else
                                {
                                    Grass grass = new();
                                    grass._localPhysicEntity.Position = new Vector2(i * 32, j * 32);
                                    tmp = grass;
                                }
                                
                                tmp.UpdatePosition();
                                terrainList.Add(tmp);

                                AddRenderGroups(tmp._renderEntity.Canvas);
                            }
                        }
                        GameData.GameMaps[Point.Zero] = terrainList;
                    }
                    //((Shelf)GameData.GameEffects[_id]).AddItem(3, 3, "Bullet_Yellow" , 10);
                    //测试一：将创建与销毁实体事件放在前面执行，而不是后面
                    foreach (var item in RemoveEntityList)
                    {
                        RemoveEntity(item);
                    }
                    RemoveEntityList.Clear();
                    foreach (var item in CreateEntityList)
                    {
                        CreateEntity(item);
                        GameData.GameEntities[item.ID].BindParent(item.ParentID);
                        //在实体中的创建子子实体的方法中就赋值了父ID
                        //由于在创建实体后，才会分配ID，所以在这里之前进行父ID的赋值
                    }
                    CreateEntityList.Clear();
                    //这个引擎大量使用宏定义的设计 例如实体控制器的设计
                    //并且依赖宏去实现大多数功能
                    Controlers.Update();//先更新各个控制器的状态//再在下文中更新对应相机，实体控制器发送更新的数据
                    if (GameTime == 0)
                    {
                        Soildre soildre1 = new()
                        {
                            _physicEntity = new PhysicEntity()
                            {
                                Position = new Vector2(640, 320),
                                Size = new Vector2(32, 32),
                                PathNodes = new Path()
                                {
                                    Cycle = false,
                                    ForceRay = new Vector2(0, 0)
                                }
                            },
                        };
                        soildre1.AddSpeaker();
                        int _Code = CreateEntity(soildre1);
                        InformationBox informationBox = new InformationBox(soildre1._physicEntity);
                        AddRenderGroups(informationBox);
                        GameData.AddUIElement(informationBox);
                        EntityControler entityControler = new()
                        {
                            BindEntity = _Code
                        };
                        GameData.AddControler(entityControler);

                        int seed = (int)Random.Shared.NextInt64(0, 65536);
                        Point Center = new();
                        for (int _y = 0; _y < 3; _y++)
                        {
                            for (int _x = 0; _x < 3; _x++)
                            {
                                CityEntity cityEntity = new()
                                {
                                    _physicEntity = new()
                                    {
                                        Position = new Vector2(200 + _x * 150 + Random.Shared.NextInt64(-60 , 60), 300 + _y * 150 + Random.Shared.NextInt64(-60 , 60))
                                    }
                                };
                                CreateEntity(cityEntity);
                            }
                        }
                    }
                    
                    //在同一帧内进行鼠标状态读取和鼠标处理操作，避免操作遗漏与延迟（<=tick）
                    foreach (var item in GameData.EntityControlers)
                    {
                        foreach (var key in item.KeyDelegate.Keys)
                        {
                            if (Controlers.ChangeKeyStat[key] == item.KeyDelegate[key].TriggerStat || item.KeyDelegate[key].TriggerStat == TwoStat.Any)
                            {
                                item.FrameKeys.Add((key , Controlers.ChangeKeyStat[key]));
                            }
                        }
                        foreach (var mouse in item.MouseDelegate.Keys)
                        {
                            if (Controlers.ChangeMouseStat[mouse] == item.MouseDelegate[mouse].TriggerStat || item.MouseDelegate[mouse].TriggerStat == TwoStat.Any)
                            {
                                item.FrameMouse.Add((mouse , Controlers.ChangeMouseStat[mouse]));
                            }
                        }

                        item.EndFrame();
                        GameData.GameEntities[item.BindEntity].InvokeDelegates();
                    }
                    
                    if (GameData.GameEntities.Count != 0)
                    {
                        Dictionary<Point, BaseIndex> LoadEntityIndex = GameIndex.GetRangeIndex(GameData.GameEntities[GameData.MainControler.BindEntity]._physicEntity.Position, LoadRange);
                        List<int> AllLoadEntity = new();
                        foreach (var item in LoadEntityIndex.Values)
                        {
                            AllLoadEntity.AddRange(item.EntityIndex);
                        }
                        AllLoadEntity = AllLoadEntity.Distinct().ToList();
                        foreach (var SingleEntity in AllLoadEntity)
                        {
                            var _singleEntity = GameData.GameEntities[SingleEntity];
                            _singleEntity._physicEntity.UpdateWish();
                            GameIndex.UpdateEntity(SingleEntity);
                            foreach (var item in _singleEntity.MapIndexes)
                            {
                                if (LoadEntityIndex.ContainsKey(new Point(item.X, item.Y)) == false)
                                {
                                    continue;
                                }
                                var SingleIndex = LoadEntityIndex[new Point(item.X , item.Y)];
                                foreach (var CompareEntity in SingleIndex.EntityIndex)//碰撞不完善
                                {
                                    if (CompareEntity == SingleEntity)
                                    {
                                        continue;
                                    }
                                    var _compareEntity = GameData.GameEntities[CompareEntity];
                                    if (RectF.IsContain(_singleEntity._physicEntity.CrashBox, _compareEntity._physicEntity.CrashBox))
                                    {
                                        _singleEntity.Crash(CompareEntity);//不计算另一个实体的碰撞
                                    }
                                }
                            }
                            _singleEntity._physicEntity.Update();
                            _singleEntity.UpdatePosition();
                            _singleEntity.UpdateSize();
                            ModifyRenderGroups(_singleEntity._renderEntity.Canvas);
                        }
                    }

                    foreach (var item in GameData.UIElements.Values)
                    {
                        item.Update();

                        ModifyRenderGroups(item);
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

        public static string tessta = "";

        public static List<Entity> CreateEntityList = new();
        public static List<int> RemoveEntityList = new();

        public int CreateEntity(Entity entity)
        {
            AddRenderGroups(entity._renderEntity.Canvas);
            return GameData.AddEntity(entity);
        }

        public int CreateEntity(Entity entity, int ID = -1, int _parentID = -1)
        {
            entity.ID = ID;
            entity.ParentID = _parentID;
            AddRenderGroups(entity._renderEntity.Canvas);
            return GameData.AddEntity(entity);
        }

        public bool RemoveEntity(int _entityID)
        {
            if (!GameData.GameEntities.ContainsKey(_entityID))
            {
                return false;
            }
            RemoveRenderGroups(GameData.GameEntities[_entityID]._renderEntity.Canvas);
            return GameData.RemoveEntity(_entityID);
        }

        private bool AddRenderGroups(RenderPropertyGroup _renderTtem)
        {
            for (int j = 0; j < _renderTtem.RenderProperties.Length; j++)
            {
                AddRenderProperty(ref _renderTtem.RenderProperties[j]);
            }
            return true;
        }

        private bool ModifyRenderGroups(RenderPropertyGroup _renderTtem)
        {
            for (int i = 0; i < _renderTtem.RenderProperties.Length; i++)
            {
                ModifyRenderProperty(_renderTtem.RenderProperties[i].RenderOrder, _renderTtem.RenderProperties[i]);
            }
            return true;
        }

        private bool RemoveRenderGroups(RenderPropertyGroup _renderTtem)
        {
            for (int i = 0; i < _renderTtem.RenderProperties.Length; i++)
            {
                if (_renderTtem.RenderProperties[i].Init == false)
                {
                    continue;
                }
                RemoveRenderProperty(_renderTtem.RenderProperties[i].RenderOrder);
            }
            return true;
        }


        private bool AddRenderGroups(UIElement _ui)
        {
            for (int j = 0; j < _ui.RenderGroup.RenderProperties.Length; j++)
            {
                AddRenderProperty(ref _ui.RenderGroup.RenderProperties[j]);
            }
            return true;
        }

        private bool ModifyRenderGroups(UIElement _ui)
        {

            for (int i = 0; i < _ui.RenderGroup.RenderProperties.Length; i++)
            {
                ModifyRenderProperty(_ui.RenderGroup.RenderProperties[i].RenderOrder, _ui.RenderGroup.RenderProperties[i]);
            }
            
            return true;
        }

        private bool RemoveRenderGroups(UIElement _ui)
        {
            for (int i = 0; i < _ui.RenderGroup.RenderProperties.Length; i++)
            {
                if (_ui.RenderGroup.RenderProperties[i].Init == false)
                {
                    continue;
                }
                RemoveRenderProperty(_ui.RenderGroup.RenderProperties[i].RenderOrder);
            }
            return true;
        }
    }
}
