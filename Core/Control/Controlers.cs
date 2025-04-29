using DotAge.Core.View;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using DotAge.Core.Model;

namespace DotAge.Core.Control
{
    static class Controlers
    {
        public static Dictionary<Keys , KeyState> LastKeyStat = new Dictionary<Keys , KeyState>();
        public static Dictionary<Keys , KeyState> NowKeyStat = new Dictionary<Keys , KeyState>();
        public static Dictionary<Keys , bool> ChangeKeyStat = new Dictionary<Keys, bool>();
        public static MouseState LastMouseStat = new MouseState();
        public static MouseState NowMouseStat = new MouseState();
        public static Dictionary<MouseButton , TwoStat> ChangeMouseStat = new Dictionary<MouseButton, TwoStat>();
        public static List<string> Log = new List<string>();
        public static Point CurrentMapMousePosition
        { 
            get
            {
                return NowMouseStat.Position - Graphic.ViewCamera.Position.ToPoint();
            }
               
        }

        static Controlers()
        {
            Update();
        }

        public static void Update()
        {
            LastMouseStat = NowMouseStat;
            NowMouseStat = Mouse.GetState();
            LastKeyStat = NowKeyStat;
            foreach (var item in Enum.GetValues(typeof(MouseButton)))
            {
                ChangeMouseStat[(MouseButton)item] = GetMouseStat((MouseButton)item);
            }
            NowKeyStat.Clear();
            foreach (var item in Enum.GetValues(typeof(Keys)))
            {
                if (Keyboard.GetState().IsKeyDown((Keys)item))
                {
                    NowKeyStat[(Keys)item] = KeyState.Down;

                }
                else
                {
                    NowKeyStat[(Keys)item] = KeyState.Up;
                }
                IsChangeStat((Keys)item);
            }
        }

        public static bool IsChangeStat(Keys key)
        {
            if (LastKeyStat[key] != NowKeyStat[key])
            {
                ChangeKeyStat[key] = true;
                return true;
            }
            else
            {
                ChangeKeyStat[key] = false;
                return false;
            }
            
        }

        public static TwoStat GetMouseStat(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    if (NowMouseStat.LeftButton - LastMouseStat.LeftButton == -1)
                    {
                        return TwoStat.ActiveToFreeze;
                    }
                    else if (NowMouseStat.LeftButton - LastMouseStat.LeftButton == 1)
                    {
                        return TwoStat.FreezeToActive;
                    }
                    else
                    {
                        return (TwoStat)NowMouseStat.LeftButton;
                    }
                case MouseButton.Right:
                    if (NowMouseStat.RightButton - LastMouseStat.RightButton == -1)
                    {
                        return TwoStat.ActiveToFreeze;
                    }
                    else if (NowMouseStat.RightButton - LastMouseStat.RightButton == 1)
                    {
                        return TwoStat.FreezeToActive;
                    }
                    else
                    {
                        return (TwoStat)NowMouseStat.RightButton;
                    }
                case MouseButton.Middle:
                    if (NowMouseStat.MiddleButton - LastMouseStat.MiddleButton == -1)
                    {
                        return TwoStat.ActiveToFreeze;
                    }
                    else if (NowMouseStat.MiddleButton - LastMouseStat.MiddleButton == 1)
                    {
                        return TwoStat.FreezeToActive;
                    }
                    else
                    {
                        return (TwoStat)NowMouseStat.MiddleButton;
                    }
                default:
                    return TwoStat.None;
            }
        }
        //public static bool BindKey
    }

    static class MouseZone
    {
        public static List<PositionTrigger> Trigger = new List<PositionTrigger>();
        public delegate bool MouseZoneDelegate();
        public static List<string> Log = new List<string>();

        static MouseZone()
        {
            Trigger.Clear();
            PositionTrigger _moveLeft = new PositionTrigger()
            {
                Description = "Screen Move To Left",
                TriggerZone = new Margin(0,0,GameSetting.ScreenWidth - 100,0).ToRectF(),
                TriggerDelegate = () => { Graphic.ViewCamera.Move(new Vector2(1, 0)) ; return true; }

            };
            PositionTrigger _moveRight = new PositionTrigger()
            {
                Description = "Screen Move To Right",
                TriggerZone = new Margin(GameSetting.ScreenWidth - 100, 0, 0, 0).ToRectF(),
                TriggerDelegate = () => { Graphic.ViewCamera.Move(new Vector2(-1, 0)) ; return true; }

            };
            PositionTrigger _moveTop = new PositionTrigger()
            {
                Description = "Screen Move To Top",
                TriggerZone = new Margin(0, 0, 0 ,GameSetting.ScreenHeight - 100).ToRectF(),
                TriggerDelegate = () => { Graphic.ViewCamera.Move(new Vector2(0, 1)) ; return true; }

            };
            PositionTrigger _moveBottom = new PositionTrigger()
            {
                Description = "Screen Move To Bottom",
                TriggerZone = new Margin(0, GameSetting.ScreenHeight - 100, 0 , 0).ToRectF(),
                TriggerDelegate = () => { Graphic.ViewCamera.Move(new Vector2(0, -1)) ; return true; }

            };
            Trigger.Add(_moveTop);
            Trigger.Add(_moveBottom);
            Trigger.Add(_moveLeft);
            Trigger.Add(_moveRight);
        }

        public static bool Update()
        {
            
            foreach (var item in Trigger)
            {
                if (RectF.IsContain(item.TriggerZone,Controlers.NowMouseStat.Position.ToVector2()))
                {
                    Log.Add(item.Description);
                    item.Invoke();
                }
            }
            return true;
        }
    }

    class PositionTrigger
    {
        public RectF TriggerZone = new RectF();
        public MouseZone.MouseZoneDelegate TriggerDelegate;
        public string Description;
        public int invokeCount = 0;

        public bool Invoke()
        {
            if (TriggerDelegate != null)
            {
                invokeCount++;
                return TriggerDelegate();
            }
            return false;
        }
    }

    enum MouseButton
    {
        None = 0,
        Left = 1,
        Right = 2,
        Middle = 3,
        X1 = 4,
        X2 = 5
    }

    enum TwoStat//双状态在此为：冻结和激活；延伸出的状态有：冻结到激活和激活到冻结
    {
        None = -1,
        Freeze = 0,
        Active = 1,
        FreezeToActive = 2,
        ActiveToFreeze = 3,
    }
}
