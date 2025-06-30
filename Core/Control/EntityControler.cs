using DotAge.Core.View;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using DotAge.Core.Model.Delegates;
using DotAge.Core.Model;

namespace DotAge.Core.Control
{
    class EntityControler
    {
        public int ID = -1;
        public Entity ExcuteEntity = null;
        public List<(Keys , TwoStat)> FrameKeys = new();
        public List<(MouseButton , TwoStat)> FrameMouse = new();
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
                TriggerStat = TwoStat.FreezeToActive,
                ControlerFuncDelegate = (ClickStat , ClickPos) => { return ItemInformation.NullItem; },
                ControlerFuncescription = "Exit temp"
            };
            ControlerFunc _up = new()
            {
                BindKey = Keys.W,
                TriggerStat = TwoStat.Active,
                ControlerFuncDelegate = (ClickStat, ClickPos) => {
                    Move(-Vector2.UnitY); 
                    return ItemInformation.NullItem; 
                },
                ControlerFuncescription = "MoveUP"
            };
            ControlerFunc _down = new()
            {
                BindKey = Keys.S,
                TriggerStat = TwoStat.Active,
                ControlerFuncDelegate = (ClickStat, ClickPos) => {Move(Vector2.UnitY); return ItemInformation.NullItem; },
                ControlerFuncescription = "MoveDOWN"
            };
            ControlerFunc _left = new()
            {
                BindKey = Keys.A,
                TriggerStat = TwoStat.Active,
                ControlerFuncDelegate = (ClickStat, ClickPos) => { Move(-Vector2.UnitX); return ItemInformation.NullItem; },
                ControlerFuncescription = "MoveLEFT"
            };
            ControlerFunc _right = new()
            {
                BindKey = Keys.D,
                TriggerStat = TwoStat.Active,
                ControlerFuncDelegate = (ClickStat, ClickPos) => {Move(Vector2.UnitX); return ItemInformation.NullItem; },
                ControlerFuncescription = "MoveRIGHT"
            };
            ControlerFunc _leftmouse = new()
            {
                BindMouseButton = MouseButton.Left,
                TriggerStat = TwoStat.Any,
                ControlerFuncDelegate = (ClickStat, ClickPos) => {
                    ItemInformation? ReturnMessage = null;
                    foreach (var item in GameData.ZoneEntities)
                    {
                        if (item.IsFixedToMap == true)
                        {
                            item.Trigger(ClickPos, ClickStat);
                        }
                        else
                        {
                            item.Trigger(ClickPos - Graphic.ViewCamera.Position, ClickStat);
                        }
                        ReturnMessage = item.InformationSource.MessageItem;
                    }
                    return ReturnMessage; 
                },
                ControlerFuncescription = "Target To Position"
            };
            ControlerFunc _rightmouse = new()
            {
                BindMouseButton = MouseButton.Right,
                TriggerStat = TwoStat.ActiveToFreeze,
                ControlerFuncDelegate = (ClickStat, ClickPos) => { ExcuteEntity.GameFrame.ModifyMoney(-0.01f); return ItemInformation.NullItem; },
                ControlerFuncescription = "Target To Position"
            };
            RegisterKey(_up);
            RegisterKey(_down);
            RegisterKey(_left);
            RegisterKey(_right); 
            RegisterMouse(_leftmouse);
            RegisterMouse(_rightmouse);
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
                log += _keyboardKey.ToString() + " ";
            }
            foreach (var _mouseKey in FrameMouse)
            {
                if (MouseDelegate.Keys.Contains(_mouseKey.Item1))
                {
                    MouseDelegate[_mouseKey.Item1].Invoke(_mouseKey.Item2, Controlers.CurrentMousePosition.ToVector2());
                }
                log += _mouseKey.ToString() + " ";
            }
            ExcuteEntity.PhysicFrame.WishForward += MoveOprate;
            MoveOprate = new Vector2(0, 0);
            FrameKeys.Clear();
            FrameMouse.Clear();
            Controlers.Log.Add(log);
            return true;
        }

        public bool CheckVaild()
        {
            if (GameData.GameEntities.ContainsKey(ExcuteEntity.ID))
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
                ExcuteEntity.PhysicFrame.PathNodes.IsFollowForceRay = true;
                ExcuteEntity.PhysicFrame.PathNodes.ForceRay = _targetVec;
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
