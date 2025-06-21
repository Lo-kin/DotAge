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
using DotAge.Core.Model.Delegates;
using System.Runtime.CompilerServices;

namespace DotAge.Core.Control
{
    static class Controlers
    {
        public static Dictionary<Keys , KeyState> LastKeyStat = new Dictionary<Keys , KeyState>();
        public static Dictionary<Keys , KeyState> NowKeyStat = new Dictionary<Keys , KeyState>();
        public static MouseState LastMouseStat = new MouseState();
        public static MouseState NowMouseStat = new MouseState();
        public static Dictionary<MouseButton , TwoStat> ChangeMouseStat = new Dictionary<MouseButton, TwoStat>();
        public static Dictionary<Keys , TwoStat> ChangeKeyStat = new Dictionary<Keys, TwoStat>();

        public static List<string> Log = new List<string>();
        public static Point CurrentMapMousePosition
        { 
            get
            {
                return NowMouseStat.Position - Graphic.ViewCamera.Position.ToPoint();
            }
               
        }

        public static Point CurrentMousePosition
        {
            get
            {
                return NowMouseStat.Position;
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
            foreach (var item in Enum.GetValues(typeof(MouseButton)))
            {
                ChangeMouseStat[(MouseButton)item] = GetMouseChangeStat((MouseButton)item);
            }

            LastKeyStat = NowKeyStat;
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
                ChangeKeyStat[(Keys)item] = GetKeyChangeStat((Keys)item);
            }
        }

        public static TwoStat GetMouseChangeStat(MouseButton button)
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
        
        public static TwoStat GetKeyChangeStat(Keys key)
        {
            if (NowKeyStat[key] == KeyState.Down && LastKeyStat[key] == KeyState.Up)
            {
                return TwoStat.FreezeToActive;
            }
            else if (NowKeyStat[key] == KeyState.Up && LastKeyStat[key] == KeyState.Down)
            {
                return TwoStat.ActiveToFreeze;
            }
            else if (NowKeyStat[key] == KeyState.Down && LastKeyStat[key] == KeyState.Down)
            {
                return TwoStat.Active;
            }
            else
            {
                return TwoStat.Freeze;
            }
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
        Any = 4,
    }
}
