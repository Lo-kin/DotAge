using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotAge.Core.Tools;
using Microsoft.Xna.Framework;

namespace DotAge.Core.Model
{
    public class Path
    {
        private List<Vector2> Nodes = new List<Vector2>();
        public List<Vector2> GetNodes { get { return Nodes; } }
        public Vector2 ForceRay = new Vector2(0, 0);
        public bool IsFollowForceRay = false;
        public bool ManualMode { get; set; } = false;
        public bool Cycle { get; set; } = true;
        public int ProcessNodeIndex { get; private set; } = 0;

        public bool IsInTarget { get; private set; } = false;

        public Vector2 CurrentPosition { get; private set; }
        public float RemainLength
        { 
            get
            {
                return (CurrentTarget - CurrentPosition).Length();
            }
        }
        public Vector2 RemainTarget
        {
            get
            {
                return CurrentTarget - CurrentPosition;
            }
        }

        public Vector2 CurrentDirect 
        {
            get
            {
                if (CurrentTarget == CurrentPosition)
                {
                    return Vector2.Zero;
                }
                return Vector2.Normalize(CurrentTarget - CurrentPosition);
            }
        }

        public Vector2 CurrentTarget
        {
            get
            {
                if (Nodes.Count != 0 && ProcessNodeIndex <= Nodes.Count - 1)
                {
                    return Nodes[ProcessNodeIndex];
                }
                else
                {
                    return CurrentPosition;
                }
            }   
        }

        public Path()
        {

        }

        public Vector2 Update(Vector2 Position)//ms
        {
            CurrentPosition = Position;
            if (CurrentPosition == CurrentTarget)
            {
                IsInTarget = true;
                if (ManualMode == false)
                {
                    PushNode();
                }
            }
            else
            {
                IsInTarget = false;
            }
            return CurrentDirect;
        }

        public bool PushNode()
        {
            if (ProcessNodeIndex >= 0 && ProcessNodeIndex <= Nodes.Count - 1)
            {
                if (ProcessNodeIndex == Nodes.Count - 1)
                {
                    if (Cycle == true)
                    {
                        ProcessNodeIndex = 0;
                    }
                }
                else
                {
                    ProcessNodeIndex++;

                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckNodeIndexVaild(int Index)
        {
            if (Nodes.Count == 0)
            {
                return false;
            }
            if (Index >= 0 || Index <= Nodes.Count - 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AddNode(Vector2 Position)
        {
            if (!float.IsNaN(Position.X) && !float.IsNaN(Position.Y))
            {
                Nodes.Add(Position);
            }
            return true;
        }

        public bool DeleteNode(int Index)
        {
            if (Nodes.Count == 0)
            {
                return false;
            }
            if (Index >= 0 && Index <= Nodes.Count - 1)
            {
                Nodes.RemoveAt(Index);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class Pioneer
    {
        public Vector2 Direct = Vector2.Zero;
        public Vector2 Update(int Tick , float Speed)//ms
        {
            return Direct * Speed * Tick;
        }
        public bool UpdateDirect(Vector2 NewDirect)
        {
            if (NewDirect.X != float.NaN && NewDirect.Y != float.NaN)
            {
                if (NewDirect.X == float.NegativeInfinity || NewDirect.X == float.PositiveInfinity || NewDirect.Y == float.NegativeInfinity || NewDirect.Y == float.PositiveInfinity)
                {
                    Vector2 tmp = Vector2.Zero;
                    if (NewDirect.X == float.NegativeInfinity || NewDirect.X == float.PositiveInfinity)
                    {
                        tmp.X = MathF.Sign(NewDirect.X);
                    }
                    else
                    {
                        tmp.X = 0;
                    }
                    if (NewDirect.Y == float.NegativeInfinity || NewDirect.Y == float.PositiveInfinity)
                    {
                        tmp.Y = MathF.Sign(NewDirect.Y);
                    }
                    else
                    {
                        tmp.Y = 0;
                    }
                    Direct = Vector2.Normalize(NewDirect);
                }
                else
                {
                    if (NewDirect.X == 0 && NewDirect.Y == 0)
                    {
                        Direct = NewDirect;
                    }
                    else
                    {
                        Direct = Vector2.Normalize(NewDirect);
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
