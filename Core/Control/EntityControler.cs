using DotAge.Core.View;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Win32;
using DotAge.Core.Model;

namespace DotAge.Core.Control
{
    class EntityControler
    {
        public int ID = -1;
        public int BindEntity = -1;
        public List<Keys> FrameKeys = new List<Keys>();
        public List<MouseButton> FrameMouse = new List<MouseButton>();
        public delegate bool TriggerDelegate();
        public Dictionary<Keys, ControlerFunc> KeyDelegate = new Dictionary<Keys, ControlerFunc>();
        public Dictionary<MouseButton , ControlerFunc> MouseDelegate = new Dictionary<MouseButton, ControlerFunc>();
        public Vector2 MoveOprate = new Vector2();

        public EntityControler()
        {
            ControlerFunc _up = new ControlerFunc() {
                BindKey = Keys.W,
                ControlerFuncDelegate = () => { return Move(new Vector2(0, -1)); },
                ControlerFuncescription = "MoveUP"
            };
            ControlerFunc _down = new ControlerFunc()
            {
                BindKey = Keys.S,
                ControlerFuncDelegate = () => { return Move(new Vector2(0, 1)); },
                ControlerFuncescription = "MoveDOWN"
            };
            ControlerFunc _left = new ControlerFunc()
            {
                BindKey = Keys.A,
                ControlerFuncDelegate = () => { return Move(new Vector2(-1, 0)); },
                ControlerFuncescription = "MoveLEFT"
            };
            ControlerFunc _right = new ControlerFunc()
            {
                BindKey = Keys.D,
                ControlerFuncDelegate = () => { return Move(new Vector2(1, 0)); },
                ControlerFuncescription = "MoveRIGHT"
            };
            ControlerFunc _leftmouse = new ControlerFunc()
            {
                BindMouseButton = MouseButton.Left,
                ControlerFuncDelegate = () => { return Target(Controlers.CurrentMapMousePosition.ToVector2()); },
                ControlerFuncescription = "Target To Position"
            };
            ControlerFunc _rightmouse = new ControlerFunc()
            {
                BindMouseButton = MouseButton.Right,
                ControlerFuncDelegate = () => { return ExcuteFunc(() => { return GameData.GameEntities[BindEntity].CreateChild(new Bullet()); }); },
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
                if (KeyDelegate.Keys.Contains(_keyboardKey))
                {
                    KeyDelegate[_keyboardKey].Invoke();
                }
                log += _keyboardKey.ToString() + " ";
            }
            foreach (var _mouseKey in FrameMouse)
            {
                if (MouseDelegate.Keys.Contains(_mouseKey))
                {
                    MouseDelegate[_mouseKey].Invoke();
                }
                log += _mouseKey.ToString() + " ";
            }
            
            GameData.GameEntities[BindEntity].PhysicEntity.Position += MoveOprate;
            MoveOprate = new Vector2(0, 0);
            FrameKeys.Clear();
            FrameMouse.Clear();
            Controlers.Log.Add(log);
            return true;
        }

        public bool GetKeyboard(Keys key)
        {
            FrameKeys.Add(key);
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
                GameData.GameEntities[BindEntity].PhysicEntity.PathNodes.IsFollowForceRay = true;
                GameData.GameEntities[BindEntity].PhysicEntity.PathNodes.ForceRay = _targetVec;
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

    class ControlerFunc
    {
        public Keys BindKey = Keys.None;
        public MouseButton BindMouseButton = MouseButton.None;
        public EntityControler.TriggerDelegate ControlerFuncDelegate;
        public string ControlerFuncescription;
        public int invokeCount;

        public bool Invoke()
        {
            if (ControlerFuncDelegate != null)
            {
                var ExcuteStaat = ControlerFuncDelegate();
                invokeCount++;
                return ExcuteStaat;
            }
            return false;
        }
    }
}
