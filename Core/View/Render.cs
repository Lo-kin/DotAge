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

namespace DotAge.Core.View
{
    public struct RenderProperty
    {
        public Vector2 Offset = new Vector2();
        public Vector2 ActualPosition = new Vector2();
        public Vector2 RenderPosition { get { return Offset + ActualPosition; } }

        public Vector2 Size = new Vector2();
        public Vector2 _RBPosition = new Vector2();
        public Color TintColor = Color.White;
        public int RenderOrder = -1;
        public TextureName RenderTexture = TextureName.Empty;
        public bool Visibility = false;
        public bool Init = false;
        public bool PropertyChanged = false;

        public RenderProperty(Vector2 _position, Vector2 _size, TextureName _texture)
        {
            Offset = _position;
            Size = _size;
            _RBPosition = Offset + Size;
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

        public Rectangle? GetTextureX(TextureName name)
        {
            int _x = (int)name;
            return GetTextureXY(_x % WidthCount, (int)MathF.Floor(_x / WidthCount));
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

        public enum TextureName
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
            White_EightSIde = 11,
            Crash_Frame = 12,
            Wihte_Ball = 13,
        }

    }


    public static class TextureIndex
    {
        private static Dictionary<string , TextureProperty> LoadedTextures = new Dictionary<string, TextureProperty>();
        private static Dictionary<(Type, int), int> GroupTexureIndex = new Dictionary<(Type, int), int>();

        static TextureIndex()
        {
            LoadTexture("Character", 2048, 2048, 32, 32);
            LoadTexture("Status", 512, 512, 32, 32);
            SetGroupUniformTexture(0, new Soildre().GetType(), 8, true);
            SetGroupUniformTexture(0, new Turret().GetType(), 7, true);
        }

        public static bool LoadTexture(string TextureName, int TextureWidth, int TextureHeight, int UnitWidth , int UnitHeight)
        {
            if (LoadedTextures.ContainsKey(TextureName))
            {
                return false;
            }
            else
            {
                if (TextureWidth % UnitWidth != 0 || TextureHeight % UnitHeight != 0)
                {
                    return false;
                }

                var _tmpTextureProperty = new TextureProperty(TextureWidth, TextureHeight, UnitWidth, UnitHeight);
                _tmpTextureProperty.Description = "Texture Name : " + TextureName;
                _tmpTextureProperty.InitTextures();
                LoadedTextures[TextureName] = _tmpTextureProperty;
                for (int i = 0; i < LoadedTextures[TextureName].HeightCount; i++)
                {
                    for (int j = 0; j < LoadedTextures[TextureName].WidthCount; j++)
                    {
                        LoadedTextures[TextureName].Textures[j, i] = new Rectangle(UnitWidth * j, UnitHeight * i, TextureWidth, TextureHeight);
                    }
                }
                return true;
            }
        }

        public static bool SetGroupUniformTexture(int GroupID, Type ObjType, int TextureIndex, bool ForceOverwrite)
        {
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


        public static bool AddAnimation()
        {

            return true;
        }

    }

    public enum TextureName
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
        White_EightSIde = 11,
        Crash_Frame = 12,
        Wihte_Ball = 13,
    }
}
