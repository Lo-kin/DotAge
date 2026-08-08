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

    public abstract class BaseRenderProperty
    {
        public Vector2 _position = Vector2.Zero;
        public Color _tintColor = Color.White;
        public float _rotation = 0f;
        public Vector2 _origin = Vector2.Zero;
        public SpriteEffects _effect = SpriteEffects.None;
        public float _layerDepth = 0f;
        public bool _isVisible = true;
        public bool _isStatic = false;
        public bool _isChanged = false;
        public virtual RectF RenderBox { get { return new RectF(Position, Vector2.One); } }
        public Vector2 Position { get { return _position; } set { if (!_position.Equals(value)) _position = value; IsChanged = true; } }
        public Color TintColor { get { return _tintColor; } set { if (!_tintColor.Equals(value)) _tintColor = value; IsChanged = true; } }
        public float Rotation { get { return _rotation; } set { if (_rotation != value) _rotation = value; IsChanged = true; } }
        public Vector2 Origin { get { return _origin; } set { if (!_origin.Equals(value)) _origin = value; IsChanged = true; } }
        public SpriteEffects Effect { get { return _effect; } set { if (_effect != value) _effect = value; IsChanged = true; } }
        public float LayerDepth { get { return _layerDepth; } set { if (_layerDepth != value) _layerDepth = value; IsChanged = true; } }
        public bool IsVisible { get { return _isVisible; } set { if (_isVisible != value) _isVisible = value; IsChanged = true; } }
        public bool IsStatic { get { return _isStatic; } set { if (_isStatic != value) _isStatic = value; IsChanged = true; } }
        public bool IsChanged { get { return _isChanged; } set { _isChanged = value; } }
        public virtual bool CheckVaild()
        {
            if (IsStatic == true)
            {
                if (IsChanged == true)
                {
                    return true;
                }
                else
                {
                    IsChanged = false;
                    return false;
                }
            }
            return true;
        }
    }

    public class TextureRenderProperty : BaseRenderProperty
    {
        public TextureRegion _region = null;
        public Vector2 _size = Vector2.Zero;
        public Vector2 _scale = Vector2.One;
        public override RectF RenderBox { get { return new RectF(Position, Size); } }
        public TextureRegion Region { get { return _region; } set { if (_region != value) _region = value; IsChanged = true; } }
        public Vector2 Size { get { return _size; } set { if (!_size.Equals(value)) _size = value; IsChanged = true; } }
        public Vector2 Scale { get { return _scale; } set { if (!_scale.Equals(value)) _scale = value; IsChanged = true; } }

        public TextureRenderProperty()
        {

        }
        public TextureRenderProperty(TextureRegion texture, Vector2 position, Vector2 size, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effect, float layerDepth)
        {
            Region = texture;
            Position = position;
            Size = size;
            TintColor = color;
            Rotation = rotation;
            Origin = origin;
            Effect = effect;
            LayerDepth = layerDepth;
            Scale = scale;
        }
    }


    public class TextRenderProperty : BaseRenderProperty
    {
        public MessageEntity TextSource = null;
        public SpriteFont _font = null;
        public int _size = 12;
        public Vector2 _scale = Vector2.One;
        public override RectF RenderBox { get { return new RectF(Position, Font.MeasureString(Text.Message)); } }
        public MessageEntity Text { get { return TextSource; } set { if (TextSource != value) TextSource = value; IsChanged = true; } }
        public SpriteFont Font { get { return _font; } set { if (_font != value) _font = value; IsChanged = true; } }
        public int Size { get { return _size; } set { if (_size != value) _size = value; IsChanged = true; } }
        public Vector2 Scale { get { return _scale; } set { if (!_scale.Equals(value)) _scale = value; IsChanged = true; } }

        public TextRenderProperty()
        {
        }
        public TextRenderProperty(MessageEntity text, SpriteFont font, Vector2 position, int size, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effect, float layerDepth)
        {
            Text = text;
            Font = font;
            Position = position;
            Size = size;
            TintColor = color;
            Rotation = rotation;
            Origin = origin;
            Effect = effect;
            LayerDepth = layerDepth;
            Scale = scale;
        }
    }
}
