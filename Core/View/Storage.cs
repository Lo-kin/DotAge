using DotAge.Core.Control;
using DotAge.Core.Model;
using DotAge.Core.Model.Delegates;
using DotAge.Core.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotAge.Core.View
{
    public class GameData
    {
        public List<EntityControler> EntityControlers = new List<EntityControler>();
        public EntityControler MainControler = null;
        public EntityControler CurrentEntityControler = null;
        private List<TerrainChunk> MapChunks = new List<TerrainChunk>();

        public Player MainPlayer = new Player();
        private List<IPhysic> PhysicProperties = new List<IPhysic>();
        public List<IRender> RenderProperties = new List<IRender>();
        private List<IGame> GameProperties = new List<IGame>();
        public List<ITrigger> TriggerProperties = new List<ITrigger>();

        static GameData()
        {

        }

        public bool AddControler(EntityControler _controler)
        {
            if (_controler == null)
            {
                return false;
            }
            else
            {
                if (EntityControlers.Count == 0)
                {
                    CurrentEntityControler = _controler;
                }
                EntityControlers.Add(_controler);
                return true;
            }
        }

        public bool AddPhysic(IPhysic physic)
        {
            if (physic != null) 
            {
                PhysicProperties.Add(physic);
                return true;
            }
            return false;
        }
        public bool AddRender(IRender render)
        {
            if (render != null)
            {
                RenderProperties.Add(render);
                return true;
            }
            return false;
        }
        public bool AddGameProp(IGame game)
        {
            if (game != null)
            {
                GameProperties.Add(game);
                return true;
            }
            return false;
        }
    }

    public static class TextureManager
    {
        public static Dictionary<string, TextureProperty> LoadedTextures = new Dictionary<string, TextureProperty>();

        static TextureManager()
        {

        }

        public static bool LoadTexture(Texture2D LoadTexture, int UnitWidth, int UnitHeight)
        {
            if (LoadedTextures.ContainsKey(LoadTexture.Name))
            {
                return false;
            }
            else
            {
                if ((((LoadTexture.Width - 1) % (UnitWidth + 1)) != 0) || ((LoadTexture.Height - 1) % (UnitHeight + 1) != 0))
                {
                    return false;
                }

                var _tmpTextureProperty = new TextureProperty(UnitWidth, UnitHeight , LoadTexture);
                _tmpTextureProperty.Description = "Texture Name : " + LoadTexture.Name;
                LoadedTextures[LoadTexture.Name] = _tmpTextureProperty;
                return true;
            }
        }

        public static TextureRegion GetTextureRegionByName(string TextureName)
        {
            foreach (var textureProperty in LoadedTextures.Values)
            {
                var tmpRegion = textureProperty.GetTextureRegion(TextureName);
                if (tmpRegion != null)
                {
                    return tmpRegion;
                }
            }
            return null;
        }
    }
    public class TextureProperty
    {
        public Texture2D Texture = null;
        public string Description = "";
        public int UnitWidth = 0;
        public int UnitHeight = 0;
        public int WidthCount { get { return (Texture.Width - 1) / (UnitWidth + 1); } }
        public int HeightCount { get { return (Texture.Height - 1) / (UnitHeight + 1); } }
        public Dictionary<Point, TextureRegion> TextureRegions = new Dictionary<Point, TextureRegion>();

        public TextureProperty(int _unitWidth, int _unitHeight, Texture2D loadTexture)
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
            for (int i = 0; i < _names.Length; i++)
            {
                Point _tileTexturePoint = MathTool.HorizonLayout(i + offset + start, WidthCount);
                LoadName(_names[i], _tileTexturePoint);
            }
            return true;
        }

        public bool LoadName(string _name, Point _position)
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
