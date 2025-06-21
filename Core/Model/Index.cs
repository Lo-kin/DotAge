using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotAge.Core.Model
{
    struct BaseIndex
    {
        public List<int> EntityIndex { get; set; } = new List<int>();
        public List<int> MineIndex { get; set; } = new List<int>();
        public List<int> BuildingIndex { get; set; } = new List<int>();
        public List<int> CrashBoxIndex { get; set; } = new List<int>();

        public BaseIndex()
        {

        }

        public bool CheckCreature(int _creatureID)
        {
            if (EntityIndex.Contains(_creatureID))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
