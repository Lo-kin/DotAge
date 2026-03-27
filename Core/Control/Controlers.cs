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
        public static Dictionary<MouseButton , TwoStatus> ChangeMouseStat = new Dictionary<MouseButton, TwoStatus>();
        public static Dictionary<Keys , TwoStatus> ChangeKeyStat = new Dictionary<Keys, TwoStatus>();

        public static PhysicBase MouseEntity = new PhysicEntity()
        {
            Position = Vector2.Zero,
            Size = new Vector2(1, 1),
        };
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
            MouseEntity.Position = CurrentMousePosition.ToVector2();
            foreach (var item in Enum.GetValues(typeof(MouseButton)))
            {
                ChangeMouseStat[(MouseButton)item] = GetMouseChangeStat((MouseButton)item);
            }

            foreach (var item in Enum.GetValues(typeof(Keys)))
            {
                LastKeyStat[(Keys)item] = NowKeyStat.ContainsKey((Keys)item) ? NowKeyStat[(Keys)item] : KeyState.Up;
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
                ChangeKeyStat[(Keys)item] = GetKeyChangeStat((Keys)item);
            }
        }

        public static TwoStatus GetMouseChangeStat(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    if (NowMouseStat.LeftButton - LastMouseStat.LeftButton == -1)
                    {
                        return TwoStatus.ActiveToFreeze;
                    }
                    else if (NowMouseStat.LeftButton - LastMouseStat.LeftButton == 1)
                    {
                        return TwoStatus.FreezeToActive;
                    }
                    else
                    {
                        return (TwoStatus)NowMouseStat.LeftButton;
                    }
                case MouseButton.Right:
                    if (NowMouseStat.RightButton - LastMouseStat.RightButton == -1)
                    {
                        return TwoStatus.ActiveToFreeze;
                    }
                    else if (NowMouseStat.RightButton - LastMouseStat.RightButton == 1)
                    {
                        return TwoStatus.FreezeToActive;
                    }
                    else
                    {
                        return (TwoStatus)NowMouseStat.RightButton;
                    }
                case MouseButton.Middle:
                    if (NowMouseStat.MiddleButton - LastMouseStat.MiddleButton == -1)
                    {
                        return TwoStatus.ActiveToFreeze;
                    }
                    else if (NowMouseStat.MiddleButton - LastMouseStat.MiddleButton == 1)
                    {
                        return TwoStatus.FreezeToActive;
                    }
                    else
                    {
                        return (TwoStatus)NowMouseStat.MiddleButton;
                    }
                default:
                    return TwoStatus.None;
            }
        }
        
        public static TwoStatus GetKeyChangeStat(Keys key)
        {
            if (NowKeyStat[key] == KeyState.Down && LastKeyStat[key] == KeyState.Up)
            {
                return TwoStatus.FreezeToActive;
            }
            else if (NowKeyStat[key] == KeyState.Up && LastKeyStat[key] == KeyState.Down)
            {
                return TwoStatus.ActiveToFreeze;
            }
            else if (NowKeyStat[key] == KeyState.Down && LastKeyStat[key] == KeyState.Down)
            {
                return TwoStatus.Active;
            }
            else if (NowKeyStat[key] == KeyState.Up && LastKeyStat[key] == KeyState.Up)
            {
                return TwoStatus.Freeze;
            }
            else
            {
                return TwoStatus.None;
            }
        }
    }

    public enum MouseButton
    {
        None = 0,
        Left = 1,
        Right = 2,
        Middle = 3,
        X1 = 4,
        X2 = 5
    }

    public enum TwoStatus//双状态在此为：冻结和激活；延伸出的状态有：冻结到激活和激活到冻结
    {
        None = -1,
        Freeze = 0,
        Active = 1,
        FreezeToActive = 2,
        ActiveToFreeze = 3,
        Any = 4,
    }
}
