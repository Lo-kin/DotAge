using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotAge.Core.Model;
using DotAge.Core.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DotAge.Core.Control
{
    internal class Engine
    {
        private Version Engine_Version = new Version(0, 1, 0);

        public delegate int AddRP(ref RenderProperty rp);
        public event AddRP AddRenderProperty;
        public delegate bool RemoveRP(int Pos);
        public event RemoveRP RemoveRenderProperty;
        public delegate bool ModifyRP(int Pos, RenderProperty rp);
        public event ModifyRP ModifyRenderProperty;

        public bool GraphicEventAble = false;
        public bool OverModifyRenderProperty = false;

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
                //START EXCUTE
                if (GraphicEventAble)
                {
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
                    Controlers.Update();//先更新各个控制器的状态
                    MouseZone.Update();//再在下文中更新对应相机，实体控制器发送更新的数据
                    if (GameTime == 0)
                    {
                        Soildre soildre1 = new Soildre();

                        soildre1.PhysicEntity.Position = new Vector2(320, 240);
                        soildre1.PhysicEntity.PathNodes.Cycle = false;
                        Soildre soildre2 = new Soildre();

                        soildre2.PhysicEntity.Position = new Vector2(320, 230);
                        soildre2.PhysicEntity.PathNodes.Cycle = false;
                        int _realID1 = CreateEntity(soildre1);
                        CreateEntity(soildre2);
                        EntityControler entityControler = new EntityControler();
                        entityControler.BindEntity = _realID1;
                        GameData.EntityControlers.Add(entityControler);

                    }
                    //在同一帧内进行鼠标状态读取和鼠标处理操作，避免操作遗漏与延迟（<=tick）
                    foreach (var item in GameData.EntityControlers)
                    {
                        foreach (var key in item.KeyDelegate.Keys)
                        {
                            if (Controlers.NowKeyStat[key] == KeyState.Down)
                            {
                                item.FrameKeys.Add(key);
                            }
                        }
                        foreach (var mouse in item.MouseDelegate.Keys)
                        {
                            if (Controlers.GetMouseStat(mouse) == TwoStat.FreezeToActive)
                            {
                                item.FrameMouse.Add(mouse);
                            }
                        }
                        item.EndFrame();
                        GameData.GameEntities[item.BindEntity].InvokeDelegates();
                    }

                    foreach (var item in GameData.GameEntities.Values)
                    {
                        foreach (var item2 in GameData.GameEntities.Values)
                        {
                            if (item != item2 && RectF.IsContain(item.PhysicEntity.CrashBox , item2.PhysicEntity.CrashBox))
                            {
                                item.Crash(item2.ID);
                                item2.Crash(item.ID);
                            }
                        }
                        //处理实体状态先进行期望处理，在经过物理处理，游戏处理之后进行数据处理，最后的实际渲染处理
                        item.PhysicEntity.UpdateWish();
                        item.PhysicEntity.Update();
                        item.UpdatePosition();
                        item.UpdateSize();

                        ModifyRenderGroups(item);
                    }
                    //tessta = GameData.GameEntities.Values.ToArray()[0].GameEntity.Money;



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

        public static List<Entity> CreateEntityList = new List<Entity>();
        public static List<int> RemoveEntityList = new List<int>();

        public int CreateEntity(Entity entity)
        {
            AddRenderGroups(entity);
            return GameData.AddEntity(entity);
        }

        public int CreateEntity(Entity entity, int ID = -1, int _parentID = -1)
        {
            entity.ID = ID;
            entity.ParentID = _parentID;
            AddRenderGroups(entity);
            return GameData.AddEntity(entity);
        }

        public int CreateEntity(PhysicEntity physicEntity , RenderEntity renderEntity , GameEntity gameEntity , int ID = -1 , int _parentID = -1)
        {
            Entity _entity = new Entity();
            _entity.PhysicEntity = physicEntity;
            _entity.RenderEntity = renderEntity;
            _entity.GameEntity = gameEntity;
            _entity.ID = ID;
            _entity.ParentID = _parentID;
            AddRenderGroups(_entity);
            return GameData.AddEntity(_entity);
        }

        public bool RemoveEntity(int _entityID)
        {
            if (!GameData.GameEntities.ContainsKey(_entityID))
            {
                return false;
            }
            RemoveRenderGroups(GameData.GameEntities[_entityID]);
            return GameData.RemoveEntity(_entityID);
        }

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

        private bool RemoveRenderGroups(Entity _entity)
        {
            for (int i = 0; i < _entity.RenderEntity.RenderCanvas.Length; i++)
            {
                if (_entity.RenderEntity.RenderCanvas[i].Init == false)
                {
                    continue;
                }
                RemoveRenderProperty(_entity.RenderEntity.RenderCanvas[i].RenderOrder);
            }
            return true;
        }

        public bool CheckCrash(RectF rectangle1, RectF rectangle2)
        {
            if (RectF.IsContain(rectangle1, rectangle2) == true)
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
