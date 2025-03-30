using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace DotAge.Core
{
    internal class Path
    {
        private List<Vector2> Nodes = new List<Vector2>();
        public List<Vector2> GetNodes {  get { return Nodes; } }
        public bool ManualMode { get; set; } = false;
        public bool Cycle { get; set; } = true;
        public int ProcessNodeIndex { get; private set; } = 0;
        public bool IsInTarget { get; private set; } = false;
        public float RemainLength { get; private set; } = 0;
        public Vector2 RemainTarget { get; private set; } = new Vector2();

        public Vector2 CurrentDirect { get; private set; } = new Vector2();
        
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
                    if (ProcessNodeIndex > Nodes.Count - 1)
                    {
                        ProcessNodeIndex = Nodes.Count;
                    }
                    return Vector2.Zero;
                }
            }
        } 

        public Path()
        {

        }

        public bool Update(Vector2 Position)
        {
            if (Position == CurrentTarget)
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
            CurrentDirect = Vector2.Normalize(CurrentTarget - Position);
            RemainLength = (CurrentTarget - Position).Length();
            RemainTarget = CurrentTarget - Position;
            return false;
        }

        public bool PushNode()
        {
            if (ProcessNodeIndex >=0 && ProcessNodeIndex <= Nodes.Count - 1)
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
            if (Position.X != float.NaN && Position.Y != float.NaN)
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
}
