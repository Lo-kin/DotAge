using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotAge.Core.Control;
using DotAge.Core.Tools;
using DotAge.Core.View;
using Microsoft.Xna.Framework;

namespace DotAge.Core.Model
{
    public class Stator
    {
        public bool LastState { get; set; } = false;
        public bool CurrentState { get; set; } = false;
        public TwoStatus TriggerCondition = TwoStatus.Any;
        public TwoStatus CurrentStatus
        {
            get
            {
                if (LastState == false && CurrentState == false)
                {
                    return TwoStatus.Freeze;
                }
                else if (LastState == false && CurrentState == true)
                {
                    return TwoStatus.FreezeToActive;
                }
                else if (LastState == true && CurrentState == false)
                {
                    return TwoStatus.ActiveToFreeze;
                }
                else
                {
                    return TwoStatus.Active;
                }
            }
        }
        public Stator()
        {

        }

        public Stator(TwoStatus triggerCondition)
        {
            TriggerCondition = triggerCondition;
        }
        public TwoStatus Update(bool newState)
        {
            LastState = CurrentState;
            CurrentState = newState;
            return CurrentStatus;
        }

        public bool IsTriggered()
        {
            if (TriggerCondition == TwoStatus.Any || 
                TriggerCondition == CurrentStatus || 
                (TriggerCondition == TwoStatus.Freeze && CurrentStatus == TwoStatus.ActiveToFreeze) || 
                (TriggerCondition == TwoStatus.Active && CurrentStatus == TwoStatus.FreezeToActive))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }

    public struct ItemInformation
    {
        public static ItemInformation NullItem => new("Null", "This is a null item information.", "This is a null item content.");

        public string Title = "Item Information";
        public string Description = "This is a item information.";
        public string Content  = "This is a item content."; 
        public (int , Rectangle?) Icon = (-1, null);
        public ItemInformation(string title, string description, string content)
        {
            Title = title;
            Description = description;
            Content = content;
        }
    }

    public class Group
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public Color TintColor { get; set; } = Color.White;
        public int FlagTexutre;
        public List<int> IncludeCreatures { get; set; } = new List<int>();

        public bool JoinCreature(ref Creature creature)
        {
            if (creature == null)
            {
                return false;
            }
            else
            {
                IncludeCreatures.Add(IncludeCreatures.Count);
                return true;
            }

        }
    }

    struct Margin
    {
        public RectF Container
        {
            get
            {
                return new RectF(new Vector2(0, 0), new Vector2(GameSetting.ScreenWidth, GameSetting.ScreenHeight));
                //用匿名函数来实现类似于矩形的数据绑定如何呢
            }
            set
            {

            }
        }
        public float Left { get; set; } = 0;
        public float Right { get; set; } = 0;
        public float Top { get; set; } = 0;
        public float Bottom { get; set; } = 0;

        public Margin(float _left, float _top, float _right, float _bottom)
        {
            Left = _left;
            Top = _top;
            Right = _right;
            Bottom = _bottom;
        }

        public RectF ToRectF()
        {
            RectF retRectF = new RectF();
            var newLeft = Container.Left + Left;
            var newRight = Container.Right - Right;
            var newTop = Container.Top + Top;
            var newBottom = Container.Bottom - Bottom;
            retRectF = new RectF(newLeft, newTop, newRight, newBottom);
            return retRectF;
        }

        public static RectF ToRectF(Margin margin)
        {
            RectF retRectF = new RectF();
            var newLeft = margin.Container.Left + margin.Left;
            var newRight = margin.Container.Right - margin.Right;
            var newTop = margin.Container.Top + margin.Top;
            var newBottom = margin.Container.Bottom - margin.Bottom;
            retRectF = new RectF(newLeft, newTop, newRight, newBottom);
            return retRectF;
        }

    }

    public struct Ray
    {
        public Vector2 Position;
        public Vector2 Direct;
    }

    public struct Line
    {
        public Vector2 Start;
        public Vector2 End;
    }

    public struct RectF
    {
        public float Right { get { return Position.X + Size.X; } }
        public float Left { get { return Position.X; } }
        public float Top { get { return Position.Y; } }
        public float Bottom { get { return Position.Y + Size.Y; } }
        public Vector2 Center { get { return Position + (Size / 2); } }
        public Vector2 Position { get; set; } = new Vector2();
        public Vector2 Size { get; set; } = new Vector2();

        public Vector2 GetAngle(Vector2 Pointer)
        {
            Vector2 retAngle = Vector2.Zero;
            if (Pointer.X >= 0)
            {
                retAngle.X = Right;
            }
            else
            {
                retAngle.X = Left;
            }
            if (Pointer.Y >= 0)
            {
                retAngle.Y = Top;
            }
            else
            {
                retAngle.Y = Bottom;
            }
            return retAngle;
        }

        public static RectF CombineRectangel(RectF rect1, RectF rect2)
        {
            float _xL = MathF.Min(rect1.Left, rect2.Left);
            float _xR = MathF.Max(rect1.Right, rect2.Right);
            float _yT = MathF.Min(rect1.Top, rect2.Top);
            float _yB = MathF.Max(rect1.Bottom, rect2.Bottom);
            return new RectF(_xL, _yT, _xR, _yB);
        }

        public static RectF ExpandRectangel(RectF rect, Vector2 expandVec)
        {
            float _xL = Math.Min(rect.Position.X, rect.Position.X + expandVec.X);
            float _xR = Math.Max(rect.Position.X + rect.Size.X, rect.Position.X + rect.Size.X + expandVec.X);
            float _yT = Math.Min(rect.Position.Y, rect.Position.Y + expandVec.Y);
            float _yB = Math.Max(rect.Position.Y + rect.Size.Y, rect.Position.Y + rect.Size.Y + expandVec.Y);
            return new RectF(_xL, _yT, _xR, _yB);
        }

        public RectF(Vector2 _position, Vector2 _size)
        {
            Position = _position;
            Size = _size;
        }

        public RectF(float _left, float _top, float _right, float _bottom)
        {
            Position = new Vector2(_left, _top);
            Size = new Vector2(_right - _left, _bottom - _top);
        }

        public Vector2 Cross(RectF rect)
        {
            Vector2 xyContain = new Vector2();
            if (IsContain(this, rect) == true)
            {
                RectF TwoRect = CombineRectangel(this, rect);
                xyContain.X = (TwoRect.Size.X - this.Size.X - rect.Size.X);
                if (this.Center.X < rect.Center.X)
                {
                    xyContain.X *= -1;
                }
                xyContain.Y = (TwoRect.Size.Y - this.Size.Y - rect.Size.Y);
                if (this.Center.Y < rect.Center.Y)
                {
                    xyContain.Y *= -1;
                }
            }
            return xyContain;
        }

        public static Vector2 ContainCount(RectF rect1, RectF rect2)
        {
            Vector2 xyContain = new Vector2();
            if (IsContain(rect1, rect2) == true)
            {
                RectF TwoRect = CombineRectangel(rect1, rect2);
                xyContain.X = -TwoRect.Size.X + rect1.Size.X + rect2.Size.X;
                
                xyContain.Y = -TwoRect.Size.Y + rect1.Size.Y + rect2.Size.Y;
            }
            return xyContain;
        }

        public Vector2 RectDirect(RectF DetectRect)
        {
            return MathTool.VectorDirect(DetectRect.Center - Center);
        }

        public static Vector2 RectDirect(RectF CheckLoacation , RectF BeingCheckLocation)
        {
            return MathTool.VectorDirect(BeingCheckLocation.Center - CheckLoacation.Center);
        }

        public static Vector2 CrashSide(PhysicBase CheckLoacation , PhysicBase BeingCheckLocation)
        {
            var realDistance = MathTool.AbsVector(CheckLoacation.CrashBox.Center - BeingCheckLocation.CrashBox.Center) - (CheckLoacation.Size / 2) - (BeingCheckLocation.Size / 2);
            var direct = new Vector2(MathF.Sign(realDistance.X), MathF.Sign(realDistance.Y));
            if (IsContain(CheckLoacation.CrashBox , BeingCheckLocation.CrashBox))
            {
                return direct;
            }
            else
            {
                if (IsContain(CheckLoacation.WishRange, BeingCheckLocation.WishRange))
                {
                    Vector2 twoDistance = Distance(CheckLoacation.CrashBox, BeingCheckLocation.CrashBox);
                    Vector2 wishmix = CheckLoacation.WishForward - BeingCheckLocation.WishForward;
                    Vector2 twoWish = new Vector2(MathF.Abs(wishmix.X), MathF.Abs(wishmix.Y));
                    Vector2 distanceArg = twoDistance + twoWish;
                    if (distanceArg.X < distanceArg.Y)
                    {
                        return new Vector2(0, 1) * direct;
                    }
                    else if (distanceArg.X > distanceArg.Y)
                    {
                        return new Vector2(1, 0) * direct;
                    }
                    else
                    {
                        return new Vector2(1, 1) * direct;
                    }
                 }
                else
                {
                    return Vector2.Zero;
                }
            }
            return Vector2.Zero;
        }

        public static Vector2 Distance(RectF CheckRect , RectF BeingCheckRect)
        {
            return new Vector2(MathF.Min(MathF.Abs(CheckRect.Left - BeingCheckRect.Right), MathF.Abs(CheckRect.Right - BeingCheckRect.Left)) , MathF.Min(MathF.Abs(CheckRect.Top - BeingCheckRect.Bottom) , MathF.Abs(CheckRect.Bottom - BeingCheckRect.Top)));
        }

        public static bool IsContain(RectF rect1, Vector2 point)
        {
            bool stat = false;
            if (point.X > rect1.Left && point.X < rect1.Right && point.Y > rect1.Top && point.Y < rect1.Bottom)
            {
                stat = true;
            }
            return stat;
        }

        public static bool IsContain(RectF rect1, RectF rect2)
        {
            bool stat = false;
            if (rect1.Right > rect2.Left && rect1.Left < rect2.Right && rect1.Bottom > rect2.Top && rect1.Top < rect2.Bottom)
            {
                stat = true;
            }
            return stat;
        }

        public bool Contains(RectF rect2)
        {
            bool stat = false;
            if (Right > rect2.Left && Left < rect2.Right && Bottom > rect2.Top && Top < rect2.Bottom)
            {
                stat = true;
            }
            return stat;
        }

        public static RectF CrossZone(RectF rect1, RectF rect2)
        {
            if (IsContain(rect1, rect2) == false)
            {
                return new RectF(0, 0, 0, 0);
            }
            float xL = MathF.Max(rect1.Left,  rect2.Left);
            float xR = MathF.Min(rect1.Right, rect2.Right);
            float yT = MathF.Max(rect1.Top,   rect2.Top);
            float yB = MathF.Min(rect1.Bottom,rect2.Bottom);
            if (xR >= xL && yB >= yT)
            {
                return new RectF(xL, yT, xR, yB);
            }
            else
            {
                return new RectF(0, 0, 0, 0);
            }
        }

        public RectF TestMove(Vector2 Vec)
        {
            RectF tmp = this;
            tmp.Move(Vec);
            return tmp;
        }

        public RectF TestMove(Vector2 Speed, int Time)
        {
            RectF tmp = this;
            tmp.Move(Speed * Time);
            return tmp;
        }

        public bool Move(Vector2 Vec)
        {
            Position += Vec;
            return true;
        }

        public bool ChangeSize(Vector2 Vec)
        {
            Size += Vec;
            return true;
        }

        public RectF Copy()
        {
            return new RectF(this.Position , this.Size);
        }
    }
}
