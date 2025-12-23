using DotAge.Core.Control;
using DotAge.Core.Model;
using DotAge.Core.Model.Dialogue;
using DotAge.Core.Model.Economy;
using DotAge.Core.Model.Region;
using DotAge.Core.Tools;
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
        public static SpriteFont _font;
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
            DrawInfo.TextSource = CoreInfo;
            DrawInfo.Font = _font;
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
            DateTime start = DateTime.Now;
            GraphicsDevice.Clear(Color.Black);
            DynamicSprite.Begin(transformMatrix:ViewCamera.View);
            StaticSprite.Begin();
            try
            {
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
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Render Error:" + ex.Message);
            }

            DateTime end = DateTime.Now;
            CoreInfo.Message = "Render Time: " + (end - start).TotalMilliseconds + " ms\n" + "Core Time: " + CoreTime + " ms\n";
            DrawInfo.Draw(StaticSprite);

            DynamicSprite.End();
            StaticSprite.End();

            base.Draw(gameTime);



        }
        public double CoreTime = 0;
        public MessageEntity CoreInfo = new MessageEntity();
        public TextSprite DrawInfo = new TextSprite()
        {
            IsStatic = true,
            Position = new Vector2(10, 10),
            TintColor = Color.Yellow,
            Font = _font,
        };

    }
    

    public abstract class Sprite
    {
        public Vector2 _position = Vector2.Zero;
        public Color _tintColor = Color.White;
        public float _rotation = 0f;
        public Vector2 _origin = Vector2.Zero;
        public SpriteEffects _effect  = SpriteEffects.None;
        public float _layerDepth  = 0f;
        public bool _isVisible = true;
        public bool _isStatic = false;
        public bool _isChanged = false;
        public Vector2 Position { get { return _position; } set { if (!_position.Equals(value)) _position = value; IsChanged = true; } } 
        public Color TintColor { get { return _tintColor; } set { if (!_tintColor.Equals(value)) _tintColor = value;IsChanged = true; } } 
        public float Rotation { get { return _rotation; } set {if (_rotation != value) _rotation = value; IsChanged = true; } } 
        public Vector2 Origin { get { return _origin; } set {if (!_origin.Equals(value)) _origin = value;IsChanged = true; } }
        public SpriteEffects Effect { get { return _effect; } set {if(_effect != value) _effect = value;IsChanged = true; } } 
        public float LayerDepth { get { return _layerDepth; } set {if (_layerDepth != value) _layerDepth = value;IsChanged = true; } } 
        public bool IsVisible { get { return _isVisible; } set {if (_isVisible != value) _isVisible = value; IsChanged = true; } } 
        public bool IsStatic { get { return _isStatic; } set {if (_isStatic != value) _isStatic = value;IsChanged = true; } }
        public bool IsChanged { get { return _isChanged; } set { _isChanged = value;} }
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
        public TextureRegion _region = null;
        public Vector2 _size = Vector2.Zero;
        public Vector2 _scale = Vector2.One;

        public TextureRegion Region { get { return _region; } set {if (_region != value) _region = value; IsChanged = true; } }
        public Vector2 Size { get { return _size; } set { if (!_size.Equals(value))_size = value; IsChanged = true; } }
        public Vector2 Scale { get { return _scale; } set {if (!_scale.Equals(value)) _scale = value; IsChanged = true; } }

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
                //Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
                //spriteBatch.Draw(Region.Texture ,Position , (Rectangle)Region.TextureRect , TintColor, Rotation, Origin,Scale ,  Effect, LayerDepth);
                spriteBatch.Draw(Region.Texture, destinationRectangle: new Rectangle(Position.ToPoint(), Size.ToPoint()), sourceRectangle: Region.TextureRect, TintColor, Rotation, Origin, Effect, LayerDepth);
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
        public MessageEntity _textSource = null;
        
        public SpriteFont _font = Graphic._font;
        public float _scale = 1.0f;

        public MessageEntity TextSource { get { return _textSource; } set {if (_textSource != value) _textSource = value; IsChanged = true; } }
        public SpriteFont Font { get { return _font; } set {if (_font != value) _font = value; IsChanged = true; } }
        public float Scale { get { return _scale; } set {if (_scale != value) _scale = value; IsChanged = true; } }
        public TextSprite()
        {

        }
        public TextSprite(MessageEntity text, SpriteFont font, Vector2 position, Color color, float rotation, Vector2 origin, SpriteEffects effect, float layerDepth, float scale)
        {
            TextSource = text;
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
                spriteBatch.DrawString(Font, TextSource.Message, Position, TintColor, Rotation, Origin, Scale, Effect, LayerDepth);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class AnimationSprite : Sprite
    {
        public Animation Animation = null;
        public Vector2 Size = Vector2.Zero;
        public Vector2 Scale = Vector2.One;

        public override bool CheckVaild()
        {
            if (Animation == null)
            {
                return false;
            }
            return base.CheckVaild();
        }

        public AnimationSprite()
        {
        }

        public AnimationSprite(TextureRegion texture, Vector2 position, Vector2 size, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effect, float layerDepth)
        {
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
                spriteBatch.Draw(Animation.CurrentFrame.Texture, destinationRectangle: new Rectangle(Position.ToPoint(), Size.ToPoint()), sourceRectangle: Animation.CurrentFrame.TextureRect, TintColor, Rotation, Origin, Effect, LayerDepth);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
