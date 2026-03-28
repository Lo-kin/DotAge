using DotAge.Core.View;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using DotAge.Core.Model.Delegates;
using DotAge.Core.Model;
using System.Security.Cryptography.X509Certificates;

namespace DotAge.Core.Control
{
    public class EntityControler
    {
        public int ID = -1;
        public EngineAccessor EngineAccess = null;
        public Entity ExcuteEntity = null;
        public List<(Keys , TwoStatus)> FrameKeys = new();
        public List<(MouseButton , TwoStatus)> FrameMouse = new();
        public Dictionary<Keys, ControlerFunc> KeyDelegate = new();
        public Dictionary<MouseButton , ControlerFunc> MouseDelegate = new();
        public Dictionary<int , ZoneEntity> ZoneDelegate = new();
        public Vector2 MoveOprate = new Vector2();

        public EntityControler(Entity _excuteEntity)
        {
            if (_excuteEntity == null)
            {
                throw new ArgumentNullException(nameof(_excuteEntity), "EntityControler must bind a valid Entity.");
            }
            else
            {
                ExcuteEntity = _excuteEntity;
            }
            ControlerFunc _esc = new()
            {
                BindKey = Keys.Escape,
                TriggerStat = TwoStatus.FreezeToActive,
                ControlerFuncDelegate = (ClickStat , ClickPos) => { return ItemInformation.NullItem; },
                ControlerFuncescription = "Exit temp"
            };
            ControlerFunc _up = new()
            {
                BindKey = Keys.W,
                TriggerStat = TwoStatus.Active,
                ControlerFuncDelegate = (ClickStat, ClickPos) => {
                    Move(-Vector2.UnitY);
                    ExcuteEntity.PhysicProperty.FaceForward = -Vector2.UnitY;
                    return ItemInformation.NullItem; 
                },
                ControlerFuncescription = "MoveUP"
            };
            ControlerFunc _down = new()
            {
                BindKey = Keys.S,
                TriggerStat = TwoStatus.Active,
                ControlerFuncDelegate = (ClickStat, ClickPos) => {
                    Move(Vector2.UnitY);
                    ExcuteEntity.PhysicProperty.FaceForward = Vector2.UnitY;
                    return ItemInformation.NullItem; },
                ControlerFuncescription = "MoveDOWN"
            };
            ControlerFunc _left = new()
            {
                BindKey = Keys.A,
                TriggerStat = TwoStatus.Active,
                ControlerFuncDelegate = (ClickStat, ClickPos) => { 
                    Move(-Vector2.UnitX);
                    ExcuteEntity.PhysicProperty.FaceForward = -Vector2.UnitX;
                    return ItemInformation.NullItem; },
                ControlerFuncescription = "MoveLEFT"
            };
            ControlerFunc _right = new()
            {
                BindKey = Keys.D,
                TriggerStat = TwoStatus.Active,
                ControlerFuncDelegate = (ClickStat, ClickPos) => {
                    Move(Vector2.UnitX); 
                    ExcuteEntity.PhysicProperty.FaceForward = Vector2.UnitX;
                    return ItemInformation.NullItem; },
                ControlerFuncescription = "MoveRIGHT"
            };
            ControlerFunc _build = new()
            {
                BindKey = Keys.B,
                TriggerStat = TwoStatus.ActiveToFreeze,
                ControlerFuncDelegate = (ClickStat, ClickPos) =>
                {
                    if (EngineAccess != null)
                    {
                        if (ExcuteEntity.GameProperty.Money >= 1)
                        {
                            EngineAccess.AddEntity(new Turret(EngineAccess)
                            {
                                Parent = ExcuteEntity,
                                PhysicProperty = new PhysicEntity()
                                {
                                    Position = ExcuteEntity.PhysicProperty.Position + ((ExcuteEntity.PhysicProperty as PhysicEntity).Pioneer.Direct * 16),
                                    Size = new Vector2(32, 32),
                                }
                            });
                            ExcuteEntity.GameProperty.Money--;
                        }

                    }


                    ItemInformation? ReturnMessage = null;
                    return ReturnMessage;
                },
                ControlerFuncescription = "Build At Position"
            };
            ControlerFunc _PskillUp = new()
            {
                BindKey = Keys.X,
                TriggerStat = TwoStatus.ActiveToFreeze,
                ControlerFuncDelegate = (ClickStat, ClickPos) =>
                {
                    if (ExcuteEntity.GameProperty.SkillPoint != 0)
                    {
                        ExcuteEntity.GameProperty.PierceCount++;
                        foreach (var item in ExcuteEntity.Children)
                        {
                            if (item is Turret)
                            {
                                (item as Turret).BulletPCount = ExcuteEntity.GameProperty.PierceCount;
                            }
                        }
                        ExcuteEntity.GameProperty.SkillPoint--;
                    }
                    return null;
                }
            };
            ControlerFunc _DskillUp = new()
            {
                BindKey = Keys.C,
                TriggerStat = TwoStatus.ActiveToFreeze,
                ControlerFuncDelegate = (ClickStat, ClickPos) =>
                {
                    if (ExcuteEntity.GameProperty.SkillPoint != 0)
                    {
                        ExcuteEntity.GameProperty.Damage+=9;
                        foreach (var item in ExcuteEntity.Children)
                        {
                            if (item is Turret)
                            {
                                (item as Turret).BullectDamage = ExcuteEntity.GameProperty.Damage;
                            }
                        }
                        ExcuteEntity.GameProperty.SkillPoint--;
                    }
                    return null;
                }
            };

            ControlerFunc _eUse = new()
            {
                BindKey = Keys.E,
                TriggerStat = TwoStatus.ActiveToFreeze,
                ControlerFuncDelegate = (ClickStat, ClickPos) => {
                    ExcuteEntity.Use();
                    return ItemInformation.NullItem;
                },
                ControlerFuncescription = "opendoor"
            };
            ControlerFunc _leftmouse = new()
            {
                BindMouseButton = MouseButton.Left,
                TriggerStat = TwoStatus.Any,
                ControlerFuncDelegate = (ClickStat, ClickPos) => {
                    ItemInformation? ReturnMessage = null;
                    return ReturnMessage; 
                },
                ControlerFuncescription = "Target To Position"
            };
            ControlerFunc _rightmouse = new()
            {
                BindMouseButton = MouseButton.Right,
                TriggerStat = TwoStatus.ActiveToFreeze,
                ControlerFuncDelegate = (ClickStat, ClickPos) => { 
                    ExcuteEntity.GameProperty.ModifyMoney(-0.01f); return ItemInformation.NullItem; },
                ControlerFuncescription = "Target To Position"
            };

            RegisterKey(_up);
            RegisterKey(_down);
            RegisterKey(_left);
            RegisterKey(_right); 
            RegisterMouse(_leftmouse);
            RegisterMouse(_rightmouse);
            RegisterKey(_esc);
            RegisterKey(_build);
            RegisterKey(_PskillUp);
            RegisterKey(_DskillUp);
            RegisterKey(_eUse);
        }

        public bool RegisterKey(dynamic _controlerFunc)
        {
            KeyDelegate[_controlerFunc.BindKey] = _controlerFunc;
            return false;
        }

        public bool RegisterMouse(dynamic _controlerFunc)
        {
            MouseDelegate[_controlerFunc.BindMouseButton] = _controlerFunc;
            return false;
        }

        public bool EndFrame()//实体控制器的所有操作都会在Engine类的一帧内进行
        {
            string log = "EC_id:" + ID + "Bind_id:" + ExcuteEntity.ID + " , " + FrameKeys.Count + " Keys Excuted , " + FrameMouse.Count + " Mouse Excuted [";
            foreach (var _keyboardKey in FrameKeys)
            {
                if (KeyDelegate.Keys.Contains(_keyboardKey.Item1))
                {
                    KeyDelegate[_keyboardKey.Item1].Invoke(_keyboardKey.Item2 , Vector2.Zero);
                }

            }
            foreach (var _mouseKey in FrameMouse)
            {
                if (MouseDelegate.Keys.Contains(_mouseKey.Item1))
                {
                    MouseDelegate[_mouseKey.Item1].Invoke(_mouseKey.Item2, Controlers.CurrentMousePosition.ToVector2());
                }
            }
            (ExcuteEntity.PhysicProperty as PhysicEntity).Pioneer.UpdateDirect(MoveOprate);
            MoveOprate = new Vector2(0, 0);
            FrameKeys.Clear();
            FrameMouse.Clear();
            return true;
        }

        public bool CheckVaild()
        {
            if (ExcuteEntity != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Move(Vector2 _moveVec)
        {
            if (CheckVaild() == true)
            {
                MoveOprate += _moveVec;
                return true;
            }
            return false;
        }


        public bool Target(Vector2 _targetVec)
        {
            if (CheckVaild() == true)
            {
                (ExcuteEntity.PhysicProperty as PhysicEntity).PathNodes.IsFollowForceRay = true;
                (ExcuteEntity.PhysicProperty as PhysicEntity).PathNodes.ForceRay = _targetVec;
                return true;
            }
            return false;
        }

        public bool ExcuteFunc(Func<bool> func)
        {
            if (CheckVaild() == true)
            {
                func.Invoke();
                return true;
            }
            
            return false;
        }
    }
}
