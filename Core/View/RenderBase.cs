using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotAge.Core.Model;
using Microsoft.Xna.Framework.Content;
using DotAge.Core.Tools;
using DotAge.Core.Control;
using System.Diagnostics.SymbolStore;

namespace DotAge.Core.View
{
    public class TextureProperty
    {
        public Texture2D Texture = null;
        public string Description = "";
        public int UnitWidth = 0;
        public int UnitHeight = 0;
        public int WidthCount { get { return (Texture.Width - 1) / (UnitWidth + 1); } }
        public int HeightCount { get { return (Texture.Height - 1) / (UnitHeight + 1); } }
        public Dictionary<Point , TextureRegion> TextureRegions = new Dictionary<Point, TextureRegion>();

        public TextureProperty(int _unitWidth , int _unitHeight , Texture2D loadTexture)
        {
            UnitHeight = _unitHeight;
            UnitWidth = _unitWidth;
            Texture = loadTexture;
        }

        public TextureRegion? GetTextureRegion(string name)
        {
            return TextureRegions.Values.ToList().Find(x => x.Name == name);
        }

        public TextureRegion? GetTextureRegion(Point pos)
        {
            return TextureRegions[pos];//不需要保护？
        }

        public bool CheckNameVaild(string name)
        {
            if (TextureRegions.Values.ToList().Find(x => x.Name == name) == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool LoadNames(string[] _names)
        {
            var offset = 0;
            var start = TextureRegions.Count;

            Point startPos = MathTool.HorizonLayout(start, WidthCount);
            for (int i = 0; i < _names.Length;i++ )
            {
                Point _tileTexturePoint = MathTool.HorizonLayout(i + offset + start, WidthCount);
                LoadName(_names[i] , _tileTexturePoint);
            }
            return true;
        }

        public bool LoadName(string _name , Point _position)
        {
            if (_position.X >= 0 && _position.Y >= 0 && _position.X < WidthCount && _position.Y < HeightCount)
            {
                TextureRegions[_position] = new TextureRegion()
                {
                    Texture = Texture,
                    TextureRect = new Rectangle(1 + (_position.X * (UnitWidth + 1)), 1 + (_position.Y * (UnitHeight + 1)), UnitWidth, UnitHeight),
                    Name = _name
                };
            }
            return true;
        }
    }

    public class TextureRegion
    {
        public Texture2D Texture;
        public Rectangle? TextureRect;
        public string Name = "None";
    }
}
