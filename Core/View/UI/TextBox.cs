using DotAge.Core.Model.Dialogue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace DotAge.Core.View.UI
{
    internal class TextBox
    {
        public Vector2 Position = new Vector2(0, 0);
        public int Width { get; set; } = 100;
        public int Height { get; set; } = 32;
        public Paragraph Paragraph { get; set; } = new Paragraph();

        public TextBox() 
        {

        }
    }
}
