using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotAge.Core;

namespace DotAge.Render
{
    public struct RenderProperty
    {
        public Vector2 RalativePosition = new Vector2();
        public Vector2 RenderPosition = new Vector2();
        public Vector2 Size = new Vector2();
        public Vector2 _RBPosition = new Vector2();
        public Color TintColor = Color.White;
        public int RenderOrder = 0;
        public int RenderTexture = 2;
        public bool Visibility = false;
        public bool Init = false;

        public RenderProperty(Vector2 _position, Vector2 _size, int _texture)
        {
            RalativePosition = _position;
            Size = _size;
            _RBPosition = RalativePosition + Size;
            RenderTexture = _texture;
            Init = true;
        }
    }

    public static class RenderModifier
    {
        static RenderModifier()
        {
            Vector2 vector2 = new Vector2();
            var c = vector2 * 2;
        }


    }


    public static class TextureIndex
    {
        private static Rectangle[,] Textures = new Rectangle[Width, Height];
        private const int Height = 128;
        private const int Width = 128;
        private const int TextureHeight = 16;
        private const int TextureWidth = 16;
        private static Dictionary<(Type, int), int> GroupTexureIndex = new Dictionary<(Type, int), int>();
        public const int ShadowIndex = 10;

        static TextureIndex()
        {
            for (int i = 0; i < Textures.GetLength(1); i++)
            {
                for (int j = 0; j < Textures.GetLength(0); j++)
                {
                    Textures[j, i] = new Rectangle(j * TextureWidth, i * TextureHeight, TextureWidth, TextureHeight);
                }
            }
            SetGroupUniformTexture(0, new Soildre().GetType(), 8, true);
            SetGroupUniformTexture(0, new Turret().GetType(), 7, true);
        }

        public static Rectangle? GetTextureXY(int _width = Width - 1, int _height = Height - 1)
        {
            if (_height < 0 || _width < 0 || _height >= Height || _width >= Width)
            {
                return null;
            }
            else
            {
                return Textures[_width, _height];
            }
        }

        public static Rectangle? GetTextureX(int _x)
        {
            return GetTextureXY(_x % Width, (int)MathF.Floor(_x / Width));
        }

        public static bool SetGroupUniformTexture(int GroupID, Type ObjType, int TextureIndex, bool ForceOverwrite)
        {
            if (!CheckIndexVaild(TextureIndex))
            {
                return false;
            }
            if (GroupTexureIndex.ContainsKey((ObjType, GroupID)) && ForceOverwrite == true || !GroupTexureIndex.ContainsKey((ObjType, GroupID)))
            {
                GroupTexureIndex[(ObjType, GroupID)] = TextureIndex;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int GetGroupUniformTexture(int GroupID, Type ObjType)
        {
            if (GroupTexureIndex.ContainsKey((ObjType, GroupID)))
            {
                return GroupTexureIndex[(ObjType, GroupID)];
            }
            else
            {
                return 0;
            }
        }

        public static bool CheckIndexExist(Type type, int GroupID)
        {
            if (GroupTexureIndex.ContainsKey((type, GroupID)))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public static bool CheckIndexVaild(int Index)
        {
            if (Index < 0 || Index > Width * Height - 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool AddAnimation()
        {

            return true;
        }

    }

    enum TextureName
    {
        MissingTexture = 0,
        Block_White = 1,
        Empty = 2,
        Mine_Stone = 3,
        Mine_Gold = 4,
        Mine_Coal = 5,
        Tree = 6,
        Turret_Gun = 7,
        Human_Engineer = 8,
        Bullet_Yellow = 9,
        Shadow_White = 10,
    }
}
