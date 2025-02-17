using DotAge.Core;
using DotAge.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DotAge
{
    public class Graphic : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch spb;
        private SpriteFont _font;
        public Texture2D[] _texture = new Texture2D[6];
        public RenderProperty[] BufferRP = new RenderProperty[65536];
        public List<int> EmptyRP = Enumerable.Range(0,65536).ToList();
        public List<int> ExistRP = new List<int>();
        public int[] LoadRP = Array.Empty<int>();
        private bool IsRPModified = false;

        public int AddRPBuffer(ref RenderProperty rp)
        {
            if (rp.Size != Vector2.Zero)
            {
                BufferRP[EmptyRP[0]] = rp;
                rp.RenderOrder = EmptyRP[0];
                ExistRP.Add(EmptyRP[0]);
                EmptyRP.RemoveAt(0);
                IsRPModified = true;
                return rp.RenderOrder;
            }
            return -1;  
        }

        public bool ModifyRPBuffer(int Pos , RenderProperty rp)
        {
            if (Pos < 0 || Pos > 65535 || !ExistRP.Contains(Pos))
            {
                return false;
            }
            if (ExistRP.Contains(Pos))
            {
                BufferRP[Pos] = rp;
                return true;
            }
            return true;
        }

        public bool RemoveRPBuffer(int Pos)
        {
            if (Pos < 0 || Pos > 65535 || !ExistRP.Contains(Pos))
            {
                return false;
            }
            if (ExistRP.Contains(Pos))
            {
                ExistRP.Remove(Pos);
                EmptyRP.Add(Pos);
                BufferRP[Pos] = new RenderProperty();
                IsRPModified = true;
                return true;
            }
            return true;
        }

        public Graphic()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = 640;
            _graphics.PreferredBackBufferHeight = 480;
            _graphics.ApplyChanges();
            base.Initialize();

        }

        protected override void LoadContent()
        {
            spb = new SpriteBatch(GraphicsDevice);
            _texture[0] = Content.Load<Texture2D>("default_texture");
            _texture[1] = Content.Load<Texture2D>("Texture");
            _font = Content.Load<SpriteFont>("Default");
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                foreach (var item in GameData.GameCreatures.Values.ToList())
                {
                    if (item.GetType() == typeof(Soildre))
                    {
                        item.PathNode.AddNode(new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
                    }
                    
                }
            }
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                foreach (var item in GameData.GameCreatures.Values.ToList())
                {
                    if (item.GetType() == typeof(Soildre))
                    {
                        item.PathNode.DeleteNode(0);
                    }

                }
            }
            // TODO: Add your update logic here
            if (IsRPModified == true)
            {
                LoadRP = ExistRP.ToArray();
                IsRPModified = false;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            spb.Begin();

            for (int  i = 0; i < LoadRP.Count();i++)
            {
                RenderProperty TmpRP = BufferRP[LoadRP[i]];
                if (TmpRP.Visibility == false)
                {
                    continue;
                }
                if (GameData.GameCreatures.Count > 40)
                {
                    spb.DrawString(_font, GameData.GameCreatures.Values.ToList<Creature>()[30].PathNode.CurrentDirect.ToString() + "/" + GameData.GameCreatures.Values.ToList<Creature>()[30].PathNode.IsInTarget + "/" + GameData.GameCreatures.Values.ToList<Creature>()[30].PathNode.RemainLength, new Vector2(0, 400), Color.Red);
                }
                
                spb.Draw(_texture[1], TmpRP.RenderPosition , TextureIndex.GetTextureX(TmpRP.RenderTexture) , TmpRP.TintColor);
            }
            spb.End();
            base.Draw(gameTime);
        }
    }
}
