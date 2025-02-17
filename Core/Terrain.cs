using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DotAge.Core
{
    class Terrain
    {

    }

    class VoidBlock : Terrain
    {
        
    }

    class BaseBlock
    {
        public Rectangle CrashBox = new Rectangle();
        public Rectangle RenderBox
        {
            get
            {
                return new Rectangle(Position, RenderSize);
            }
        }
            
        public Size RenderSize { get; set; } = new Size(16, 16);
        public Point Position { get; set; } = new Point();

        public BaseBlock()
        { 
            
        }
    }
}
