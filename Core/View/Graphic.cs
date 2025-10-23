using DotAge.Core.Control;
using DotAge.Core.Model.Dialogue;
using DotAge.Core.Model.Economy;
using DotAge.Core.Model.Region;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace DotAge.Core.View
{
    public class Graphic : Game
    {
        private GraphicsDeviceManager _graphics;
        public static Camera2D ViewCamera = new Camera2D();
        private SpriteBatch DynamicSprite;
        private SpriteBatch StaticSprite;
        public SpriteFont _font;
        public List<Sprite> RenderObjects = new List<Sprite>();
        public bool IsLoaded = false;

        public Graphic()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            ViewCamera.View = Matrix.CreateTranslation(new Vector3(0, 0, 0));
            _graphics.PreferredBackBufferWidth = GameSetting.ScreenWidth;
            _graphics.PreferredBackBufferHeight = GameSetting.ScreenHeight;
            _graphics.ApplyChanges();
            base.Initialize();

        }

        protected override void LoadContent()
        {

            DynamicSprite = new SpriteBatch(GraphicsDevice);
            StaticSprite = new SpriteBatch(GraphicsDevice);

            _font = Content.Load<SpriteFont>("Default");
            IsLoaded = true;
            // TODO: use this.Content to load your game content here
        }
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            // TODO: Add your update logic here

            
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            
            //GraphicsDevice.Clear(Color.Black);
            DynamicSprite.Begin(transformMatrix:ViewCamera.View);
            StaticSprite.Begin();
            foreach (var item in RenderObjects)
            {
                if (item.IsVisible == true)
                {
                    if (item.IsStatic == true)
                    {
                        item.Draw(StaticSprite);
                    }
                    else
                    {
                        item.Draw(DynamicSprite);
                    }
                }
            }
            DynamicSprite.End();
            StaticSprite.End();
            base.Draw(gameTime);
        }

    }

    public abstract class Sprite
    {
        public Vector2 Position = Vector2.Zero;
        public Color TintColor = Color.White;
        public float Rotation = 0f;
        public Vector2 Origin = Vector2.Zero;
        public SpriteEffects Effect = SpriteEffects.None;
        public float LayerDepth = 0f;
        public bool IsVisible = true;
        public bool IsStatic = false;
        public Sprite()
        {
        }

        public Sprite( float rotation, Vector2 origin, SpriteEffects effect, float layerDepth)
        {
            Rotation = rotation;
            Origin = origin;
            Effect = effect;
            LayerDepth = layerDepth;
        }

        public virtual bool CheckVaild()
        {
            return true;
        }

        public virtual bool Draw(SpriteBatch spriteBatch)
        {
            return true;
        }

        public virtual bool Dispose()
        {
            return true;
        }
    }

    public class TextureSprite : Sprite
    {
        public TextureRegion Region = null;
        public Vector2 Size = Vector2.Zero;
        public Vector2 Scale = Vector2.One;

        public override bool CheckVaild()
        {
            if (Region == null)
            {
                return false;
            }
            return base.CheckVaild();
        }

        public TextureSprite()
        {
        }

        public TextureSprite(TextureRegion texture, Vector2 position, Vector2 size, Color color, float rotation, Vector2 origin,Vector2 scale ,  SpriteEffects effect, float layerDepth)
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

        public override bool Draw(SpriteBatch spriteBatch)
        {
            
            if (CheckVaild() == true)
            {
                spriteBatch.Draw(Region.Texture,Position, Region.TextureRect, TintColor, Rotation, Origin, Scale, Effect, LayerDepth);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class TextSprite : Sprite
    {
        public string Text = string.Empty;
        public SpriteFont Font = null;
        public float Scale = 1.0f;
        public TextSprite()
        {
        }
        public TextSprite(string text, SpriteFont font, Vector2 position, Color color, float rotation, Vector2 origin, SpriteEffects effect, float layerDepth, float scale)
        {
            Text = text;
            Font = font;
            Position = position;
            TintColor = color;
            Rotation = rotation;
            Origin = origin;
            Effect = effect;
            LayerDepth = layerDepth;
            Scale = scale;
        }
        public override bool CheckVaild()
        {
            if (Font == null)
            {
                return false;
            }
            return true;
        }

        public override bool Draw(SpriteBatch spriteBatch)
        {
            if (CheckVaild() == true)
            {
                spriteBatch.DrawString(Font, Text, Position, TintColor, Rotation, Origin, Scale, Effect, LayerDepth);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
