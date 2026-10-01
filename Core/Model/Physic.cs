using DotAge.Core.Control;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Text;

namespace DotAge.Core.Model
{
    public interface ILocation
    {
        public Func<Vector2, bool> OnUpdatePosition { get; set; }
        public Func<Vector2, bool> OnUpdateSize { get; set; }
        
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 Scale { get; set; }
        public float Rotation { get; set; }
        public RectF CrashBox { get { return new RectF(Position, Size); } }

        public bool UpdatePosition(Vector2 NewPosition)
        {
            if (NewPosition == Position || NewPosition.X == float.NaN || NewPosition.Y == float.NaN)
            {
                return false;
            }
            else
            {
                Position = NewPosition;
                OnUpdatePosition?.Invoke(Position);
                return true;
            }
        }

        public bool DeltaPosition(Vector2 delta)
        {
            if (delta == Vector2.Zero || delta.X == float.NaN || delta.Y == float.NaN)
            {
                return false;
            }
            else
            {
                Position += delta;
                OnUpdatePosition?.Invoke(Position);
                return true;
            }
        }

        public bool UpdateSize(Vector2 NewSize)
        {
            if (NewSize == Size || NewSize.X == float.NaN || NewSize.Y == float.NaN)
            {
                return false;
            }
            else
            {
                Size = NewSize;
                OnUpdateSize?.Invoke(Size);
                return true;
            }
        }

        public static Location GetLerpLocation(ILocation start , ILocation end , float perc)
        {
            return new Location()
            {
                Position = Vector2.LerpPrecise(start.Position , end.Position , perc),
                Size = Vector2.LerpPrecise(start.Size , end.Size , perc),
                Scale = Vector2.LerpPrecise(start.Scale , end.Scale , perc),
                Rotation = (start.Rotation + end.Rotation ) * perc
            };
        }
    }

    public interface IPhysic : ILocation
    {
        public Func<Vector2, bool> OnUpdateForce { get; set; }
        public Func<IPhysic, bool> OnCrash { get; set; }
        public Vector2 LastPosition { get; set; }
        public Vector2 Speed { get; set; }
        public Vector2 Accelerate { get; set; }
        public float Mass { get; set; }
        public Vector2 FaceForward { get; set; }

        public Vector2 WishForward { get; set; }
        public RectF WishRange
        {
            get
            {
                return RectF.ExpandRectangel(CrashBox, WishForward);
            }
        }
        public RectF WishDestination
        {
            get
            {
                return new RectF(Position + WishForward, Size);
            }
        }
        public Vector2 UpdateWish()
        {
            Speed += Accelerate * Engine.GameTickSecend;
            if (Speed.X != float.NaN && Speed.Y != float.NaN && Speed != Vector2.Zero && Speed != Vector2.Zero)
            {
                Vector2 TickMove = Speed * Engine.GameTickSecend;
                OnUpdatePosition?.Invoke(Position + TickMove);
                return TickMove;
            }
            return Vector2.Zero;
        }

        public bool AddForce(Vector2 Force)
        {
            if (Force == Vector2.Zero || float.IsNaN(Force.X) || float.IsNaN(Force.Y))
            {
                return false;
            }
            else
            {
                Accelerate += Force / Mass;
                OnUpdateForce?.Invoke(Force);
                return true;
            }
        }

        public bool Crash(IPhysic physic)
        {
            if (RectF.IsContain(this.CrashBox, physic.CrashBox))
            {
                OnCrash?.Invoke(physic);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public struct Location : ILocation
    {
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Vector2 Size { get; set; } = new Vector2(32, 32);
        public RectF CrashBox
        {
            get
            {
                return new RectF(Position, Size);
            }
        }

        public Func<Vector2, bool> OnUpdatePosition { get; set; }
        public Func<Vector2, bool> OnUpdateSize { get; set; }
        public Vector2 Scale { get; set; } = Vector2.One;
        public float Rotation { get; set; } = 0f;
        public Location(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;
        }

        public Location(RectF rect)
        {
            Position = rect.Position;
            Size = rect.Size;
        }
    }

    public abstract class PhysicEntity : IPhysic
    {
        public Vector2 _position = Vector2.Zero;
        public Vector2 _faceForward = Vector2.One;
        public Vector2 LastPosition { get; set; } = Vector2.Zero;
        public Vector2 Position { get { return _position; } set { LastPosition = _position; _position = value; } }
        public Vector2 Size { get; set; } = new Vector2(32, 32);
        public Vector2 Scale { get; set; } = Vector2.One;
        public float Rotation { get; set; } = 0f;
        public RectF CrashBox
        {
            get
            {
                return new RectF(Position, Size);
            }
        }
        public Vector2 WishForward { get; set; } = Vector2.Zero;
        public RectF WishRange
        {
            get
            {
                return RectF.ExpandRectangel(CrashBox, WishForward);
            }
        }
        public RectF WishDestination
        {
            get
            {
                return new RectF(Position + WishForward, Size);
            }
        }
        public bool IsMoving { get; set; } = false;
        public bool IsSizeChanging { get; set; } = false;
        public bool IsSoild { get; set; } = false;
        public Vector2 FaceForward
        {
            get { return _faceForward; }
            set
            {
                if (value != Vector2.Zero)
                {
                    _faceForward = Vector2.Normalize(value);
                }
            }
        }
        public Vector2 Speed { get; set; }
        public Vector2 Accelerate { get; set; }
        public float Mass { get; set; }
        public Func<IPhysic, bool> OnCrashEvent { get; set; }
        public Func<Vector2, bool> OnUpdatePosition { get; set; }
        public Func<Vector2, bool> OnUpdateSize { get; set; }
        public Func<Vector2, bool> OnDeltaPosition { get; set; }
        public Func<Vector2, bool> OnUpdateForce { get; set; }
        public Func<IPhysic, bool> OnCrash { get; set; }

        public PhysicEntity()
        {
            OnDeltaPosition += (vec) => { WishForward += vec; return true; };
        }

        public virtual bool UpdatePosition()
        {
            Position += WishForward;
            if (WishForward != Vector2.Zero)
            {
                IsMoving = true;
            }
            else
            {
                IsMoving = false;
            }
            WishForward = Vector2.Zero;
            return true;
        }
    }
}
