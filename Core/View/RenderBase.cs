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
    public struct RenderProperty
    {
        public Vector2 Offset = new Vector2();
        public Vector2 ActualPosition = new Vector2();
        public Vector2 RenderPosition { get { return Offset + ActualPosition; } }

        public string Text = string.Empty;
        public int FontSize = 16;

        public Vector2 Size = new Vector2();
        public Vector2 _RBPosition = new Vector2();
        public Color TintColor = Color.White;
        public int RenderOrder = -1;
        public (int , Rectangle?) RenderTexture = (0 , null);
        public bool Visibility = false;
        public bool Init = false;
        public bool PropertyChanged = false;
        public bool IsFixed = false;

        public bool IsShowText = false;
        public bool IsShowTexture = false;

        public RenderProperty(Vector2 _position, Vector2 _size)
        {
            Offset = _position;
            Size = _size;
            _RBPosition = Offset + Size;
            Init = true;
        }

        public RenderProperty(bool _init = true,bool _isShowTexture = true , bool _isShowText = true , bool _visibility = true , Color? _color = null)
        {
            Init = _init;
            Visibility = _visibility;
            IsShowTexture = _isShowTexture;
            IsShowText = _isShowText;
            if (_color != null)
            {
                TintColor = (Color)_color;
            }
            else
            {
                TintColor = Color.White;
            }
        }
    }

    public class RenderPropertyGroup
    {
        public RenderProperty[] RenderProperties = new RenderProperty[16];

        public RenderPropertyGroup(int _renderCanvasCount = 16 , Vector2? _allCanvasSize = null , Vector2? _allCanvasPosition = null , int[] _initCanvas = null)
        {
            RenderProperties = new RenderProperty[_renderCanvasCount];
            if (_initCanvas == null)
            {
                InitialAllCanvas();
            }
            else
            {
                InitialSpecificCanvas(_initCanvas);
            }
            
            if (_allCanvasPosition != null)
            {
                SetAllPosition((Vector2)_allCanvasPosition);
            }
            if (_allCanvasSize != null)
            {
                SetAllSize((Vector2)_allCanvasSize);
            }
        }

        public bool CheckVaild(int _canvasPosition)
        {
            if (_canvasPosition < 0 || _canvasPosition >= RenderProperties.Length)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool InitialAllCanvas()
        {
            for (int i = 0; i < RenderProperties.Length; i++)
            {
                InitialCanvas(i);
            }
            return true;
        }

        public bool InitialSpecificCanvas(int[] _canvasPositon)
        {
            foreach (var _canvasPosition in _canvasPositon)
            {
                if (CheckVaild(_canvasPosition) == false)
                {
                }
                else
                {
                    InitialCanvas(_canvasPosition);
                }
            }
            return true;
        }

        public bool InitialCanvas(int _canvasPosition)
        {
            if (CheckVaild(_canvasPosition) == false)
            {
                return false;
            }
            else
            {
                RenderProperties[_canvasPosition] = new RenderProperty()
                {
                    Init = true,
                    Visibility = true,
                    Text = "",
                    TintColor = Color.White,
                };
                return true;
            }
            
        }

        public bool SetAllPosition(Vector2 _position)
        {
            for (int i = 0; i < RenderProperties.Length; i++)
            {
                SetPosition(i, _position);
            }
            return true;
        }

        public bool SetPosition(int _canvasPosition, Vector2 _position)
        {
            if (CheckVaild(_canvasPosition) == false)
            {
                return false;
            }
            else
            {
                RenderProperties[_canvasPosition].ActualPosition = _position;
                RenderProperties[_canvasPosition].PropertyChanged = true;
                return true;
            }
        }

        public bool SetAllSize(Vector2 _size)
        {
            for (int i = 0; i < RenderProperties.Length; i++)
            {
                SetSize(i, _size);
            }
            return true;
        }

        public bool SetSize(int _canvasPosition, Vector2 _size)
        {
            if (CheckVaild(_canvasPosition) == false)
            {
                return false;
            }
            else
            {
                RenderProperties[_canvasPosition].Size = _size;
                RenderProperties[_canvasPosition].PropertyChanged = true;
                return true;
            }
        }

        public bool SetOffset(int _canvasPosition, Vector2 _position)
        {
            if (CheckVaild(_canvasPosition) == false)
            {
                return false;
            }
            else
            {
                RenderProperties[_canvasPosition].Offset = _position;
                RenderProperties[_canvasPosition].PropertyChanged = true;
                return true;
            }
        }

        public bool SetProperty(int _canvasPosition, RenderProperty _renderProperty)
        {
            if (CheckVaild(_canvasPosition) == false)
            {
                return false;
            }
            else
            {
                RenderProperties[_canvasPosition] = _renderProperty;
                RenderProperties[_canvasPosition].PropertyChanged = true;
                return true;
            }
        }

        public bool SetFixed(int _canvasPosition , bool _isFixed)
        {
            if (CheckVaild(_canvasPosition) == false)
            {
                return false;
            }
            else
            {
                RenderProperties[_canvasPosition].IsFixed = _isFixed;
                RenderProperties[_canvasPosition].PropertyChanged = true;
                return true;
            }
        }

        public bool SetAllFixed(bool _isFixed)
        {
            for (int i = 0; i < RenderProperties.Length; i++)
            {
                SetFixed(i, _isFixed);
            }
            return true;
        }

        public bool SetText(int _canvasPosition, string _text)
        {
            if (CheckVaild(_canvasPosition) == false)
            {
                return false;
            }
            else
            {
                RenderProperties[_canvasPosition].Text = _text;
                RenderProperties[_canvasPosition].PropertyChanged = true;
                return true;
            }
        }

        public bool SetShowTextState(int _canvasPosition , bool _stat)
        {
            if (CheckVaild(_canvasPosition) == false)
            {
                return false;
            }
            else
            {
                RenderProperties[_canvasPosition].IsShowText = _stat;
                return true;
            }
        }

        public bool SetShowTextureState(int _canvasPosition, bool _stat)
        {
            if (CheckVaild(_canvasPosition) == false)
            {
                return false;
            }
            else
            {
                RenderProperties[_canvasPosition].IsShowTexture = _stat;
                return true;
            }
        }

        public bool SetAllTextState(bool _stat)
        {
            for (int i = 0; i < RenderProperties.Length; i++)
            {
                SetShowTextState(i, _stat);
            }
            return true;
        }

        public bool SetAllTextureState(bool _stat)
        {
            for (int i = 0; i < RenderProperties.Length; i++)
            {
                SetShowTextureState(i, _stat);
            }
            return true;
        }

        public bool SetTint(int _canvasPosition , Color _color)
        {
            if (CheckVaild(_canvasPosition) == false)
            {
                return false;
            }
            else
            {
                RenderProperties[_canvasPosition].TintColor = _color;
                RenderProperties[_canvasPosition].PropertyChanged = true;
                return true;
            }
        }

        public bool SetAllTint(Color _color)
        {
            for (int i = 0; i < RenderProperties.Length; i++)
            {
                SetTint(i, _color);
            }
            return true;
        }

        public bool SetVisibility(int _canvasPosition, bool _visibility)
        {
            if (CheckVaild(_canvasPosition) == false)
            {
                return false;
            }
            else
            {
                RenderProperties[_canvasPosition].Visibility = _visibility;
                RenderProperties[_canvasPosition].PropertyChanged = true;
                return true;
            }
        }

        public bool SetAllVisibility(bool _visibility)
        {
            for (int i = 0; i < RenderProperties.Length; i++)
            {
                SetVisibility(i, _visibility);
            }
            return true;
        }
    }

    public struct ChainRenderProperty
    {
        public Vector2 StartChainpoint = new Vector2();
        public Vector2 EndChainpoint = new Vector2();
        public Vector2 ChainBox { get { return EndChainpoint - StartChainpoint; } }
        public Vector2 ChainBoxSize { get { return new Vector2(ChainBox.X, ChainBox.Y); } }
            
        public ChainRenderProperty() 
        {

        }
    }

    public struct TextureProperty
    {
        public string Description = "";
        public int TextureWidth = 0;
        public int TextureHeight = 0;
        public int UnitWidth = 0;
        public int UnitHeight = 0;
        public int WidthCount { get { return TextureWidth / UnitWidth; } }
        public int HeightCount { get { return TextureHeight / UnitHeight; } }
        public Rectangle[,] Textures = new Rectangle[,] { };
        public Dictionary<string , Point> TextureNames = new Dictionary<string, Point>();
        public TextureProperty(int _textureWidth , int _textureHeight , int _unitWidth , int _unitHeight)
        {
            TextureHeight = _textureHeight;
            TextureWidth = _textureWidth;
            UnitHeight = _unitHeight;
            UnitWidth = _unitWidth;
        }

        public bool InitTextures()
        {
            Textures = new Rectangle[WidthCount, HeightCount];
            return true;
        }

        public Rectangle? GetTextureXY(int _width, int _height)
        {
            if (_height < 0 || _width < 0 || _height >= HeightCount || _width >= WidthCount)
            {
                return null;
            }
            else
            {
                return Textures[_width, _height];
            }
        }

        public Rectangle? GetTextureX(int _x)
        {
            return GetTextureXY(_x % WidthCount, (int)MathF.Floor(_x / WidthCount));
        }

        public Rectangle? GetTextureByName(string name)
        {
            if (TextureNames.ContainsKey(name))
            {
                return GetTextureXY(TextureNames[name].X, TextureNames[name].Y);
            }
            else
            {
                return null;
            }
        }

        public bool CheckIndexVaild(int Index)
        {
            if (Index < 0 || Index > WidthCount * HeightCount - 1)
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
            var start = TextureNames.Count;

            Point startPos = MathTool.LengthTransToPointX(TextureNames.Count , WidthCount);
            for (int i = 0; i < _names.Length;i++ )
            {
                Point _tileTexturePoint = MathTool.LengthTransToPointX(i + offset + start, WidthCount);
                if (TextureNames.Values.Contains(_tileTexturePoint))
                {
                    offset++;
                    _tileTexturePoint = MathTool.LengthTransToPointX(i + offset + start, WidthCount);
                }
                TextureNames[_names[i]] = _tileTexturePoint;
            }
            return true;
        }
    }
}
