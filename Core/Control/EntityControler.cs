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
        public int BindEntity = -1;
        public List<(Keys , TwoStat)> FrameKeys = new List<(Keys, TwoStat)>();
        public List<(MouseButton , TwoStat)> FrameMouse = new List<(MouseButton, TwoStat)>();
        public Dictionary<Keys, ControlerFunc> KeyDelegate = new Dictionary<Keys, ControlerFunc>();
        public Dictionary<MouseButton , ControlerFunc> MouseDelegate = new Dictionary<MouseButton, ControlerFunc>();
        public Dictionary<int , ZoneEntity> ZoneDelegate = new Dictionary<int, ZoneEntity>();
        public Vector2 MoveOprate = new Vector2();

        public EntityControler()
        {
            ControlerFunc _esc = new ControlerFunc()
            {
                BindKey = Keys.Escape,
                TriggerStat = TwoStat.FreezeToActive,
                ControlerFuncDelegate = (t) => { return true; },
                ControlerFuncescription = "Exit temp"
            };
            ControlerFunc _up = new ControlerFunc()
            {
                BindKey = Keys.W,
                TriggerStat = TwoStat.FreezeToActive,
                ControlerFuncDelegate = (t) => { return Move(new Vector2(0, -1)); },
                ControlerFuncescription = "MoveUP"
            };
            ControlerFunc _down = new ControlerFunc()
            {
                BindKey = Keys.S,
                TriggerStat = TwoStat.FreezeToActive,
                ControlerFuncDelegate = (t) => { return Move(new Vector2(0, 1)); },
                ControlerFuncescription = "MoveDOWN"
            };
            ControlerFunc _left = new ControlerFunc()
            {
                BindKey = Keys.A,
                TriggerStat = TwoStat.FreezeToActive,
                ControlerFuncDelegate = (t) => { return Move(new Vector2(-1, 0)); },
                ControlerFuncescription = "MoveLEFT"
            };
            ControlerFunc _right = new ControlerFunc()
            {
                BindKey = Keys.D,
                TriggerStat = TwoStat.FreezeToActive,
                ControlerFuncDelegate = (t) => { return Move(new Vector2(1, 0)); },
                ControlerFuncescription = "MoveRIGHT"
            };
            ControlerFunc _leftmouse = new ControlerFunc()
            {
                BindMouseButton = MouseButton.Left,
                TriggerStat = TwoStat.Any,
                ControlerFuncDelegate = (t) => {
                    var fixedMousePos = Controlers.CurrentMapMousePosition.ToVector2();
                    var unfixedMousePos = Controlers.NowMouseStat.Position.ToVector2();
                    foreach (var item in GameData.ZoneEntities)
                    {
                        if (item.IsFixedToMap == true)
                        {
                            if (item.CheckTrigger(fixedMousePos , t))
                            {
                                item.Trigger(fixedMousePos , t);
                            }
                        }
                        else
                        {
                            if (item.CheckTrigger(unfixedMousePos , t))
                            {
                                item.Trigger(unfixedMousePos , t);//缺少特异性的指代，stat与实际stat不符
                            }
                        }

                    }
                    return true; 
                },
                ControlerFuncescription = "Target To Position"
            };
            ControlerFunc _rightmouse = new ControlerFunc()
            {
                BindMouseButton = MouseButton.Right,
                TriggerStat = TwoStat.FreezeToActive,
                ControlerFuncDelegate = (t) => { return ExcuteFunc(() => { return GameData.GameEntities[BindEntity]._physicEntity.PathNodes.AddNode(Controlers.CurrentMapMousePosition.ToVector2()); }); },
                ControlerFuncescription = "Target To Position"
            };
            RegisterKey(_up);
            RegisterKey(_down);
            RegisterKey(_left);
            RegisterKey(_right); 
            RegisterMouse(_leftmouse);
            RegisterMouse(_rightmouse);
        }

        public bool RegisterKey(ControlerFunc _controlerFunc)
        {
            KeyDelegate[_controlerFunc.BindKey] = _controlerFunc;
            return false;
        }

        public bool RegisterMouse(ControlerFunc _controlerFunc)
        {
            MouseDelegate[_controlerFunc.BindMouseButton] = _controlerFunc;
            return false;
        }

        public bool EndFrame()//实体控制器的所有操作都会在Engine类的一帧内进行
        {
            string log = "EC_id:" + ID + "Bind_id:" + BindEntity + " , " + FrameKeys.Count + " Keys Excuted , " + FrameMouse.Count + " Mouse Excuted [";
            foreach (var _keyboardKey in FrameKeys)
            {
                if (KeyDelegate.Keys.Contains(_keyboardKey.Item1))
                {
                    KeyDelegate[_keyboardKey.Item1].Invoke(_keyboardKey.Item2);
                }
                log += _keyboardKey.ToString() + " ";
            }
            foreach (var _mouseKey in FrameMouse)
            {
                if (MouseDelegate.Keys.Contains(_mouseKey.Item1))
                {
                    MouseDelegate[_mouseKey.Item1].Invoke(_mouseKey.Item2);
                }
                log += _mouseKey.ToString() + " ";
            }
            GameData.GameEntities[BindEntity]._physicEntity.WishForward += MoveOprate;
            MoveOprate = new Vector2(0, 0);
            FrameKeys.Clear();
            FrameMouse.Clear();
            Controlers.Log.Add(log);
            return true;
        }

        public bool CheckVaild()
        {
            if (GameData.GameEntities.ContainsKey(BindEntity))
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
                GameData.GameEntities[BindEntity]._physicEntity.PathNodes.IsFollowForceRay = true;
                GameData.GameEntities[BindEntity]._physicEntity.PathNodes.ForceRay = _targetVec;
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
