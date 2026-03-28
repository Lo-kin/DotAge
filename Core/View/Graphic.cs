using DotAge.Core.Control;
using DotAge.Core.Model;
using DotAge.Core.Model.Dialogue;
using DotAge.Core.Model.Economy;
using DotAge.Core.Model.Region;
using DotAge.Core.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
            (DrawInfo.BaseProperty as TextRenderProperty).TextSource = CoreInfo;
            (DrawInfo.BaseProperty as TextRenderProperty).Font = _font;
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
                    if (item.BaseProperty.IsVisible == true)
                    {
                        if (item.BaseProperty.IsStatic == true)
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
            CoreInfo.Message = "Render Time: " + (end - start).TotalMilliseconds + " ms\n" + "Core Time: " + CoreTime + " ms\n" + "Force : " + WishForce;
            DrawInfo.Draw(StaticSprite);

            DynamicSprite.End();
            StaticSprite.End();
            base.Draw(gameTime);
        }
        public Vector2 WishForce = Vector2.Zero;
        public double CoreTime = 0;
        public MessageEntity CoreInfo = new MessageEntity();
        public TextSprite DrawInfo = new TextSprite()
        {
            BaseProperty = new TextRenderProperty()
            {
                IsStatic = true,
                Position = new Vector2(10, 10),
                TintColor = Color.Yellow,
                Font = _font,
            },

        };
    }

    public abstract class Sprite
    {
        public BaseRenderProperty BaseProperty = null;
        public Sprite()
        {

        }

        public virtual bool CheckVaild()
        {
            if (BaseProperty == null)
            {
                return false;
            }
            if (BaseProperty.IsStatic == true)
            {
                if (BaseProperty.IsChanged == true)
                {
                    return true;
                }
                else
                {
                    BaseProperty.IsChanged = false;
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
            BaseProperty = null;

            return true;
        }

        public virtual bool UpdatePosition(Vector2 position)
        {
            BaseProperty.Position = position;
            return true;
        }

        public virtual bool UpdateSize(Vector2 size)
        {
            if (BaseProperty is TextureRenderProperty)
            {
                (BaseProperty as TextureRenderProperty).Size = size;
                return true;
            }
            return false;
        }
    }

    public class TextureSprite : Sprite
    {
        public override bool CheckVaild()
        {
            if (BaseProperty is not TextureRenderProperty)
            {
                return false;
            }
            if ((BaseProperty as TextureRenderProperty).Region == null)
            {
                return false;
            }
            return base.CheckVaild();
        }

        public TextureSprite()
        {
            BaseProperty = new TextureRenderProperty();
        }

        public TextureSprite(TextureRenderProperty renderProperty)
        {
            BaseProperty = renderProperty;
        }

        public override bool Draw(SpriteBatch spriteBatch)
        {
            if (CheckVaild() == true)
            {
                var property = BaseProperty as TextureRenderProperty;
                spriteBatch.Draw(
                    property.Region.Texture,
                    destinationRectangle: new Rectangle(property.Position.ToPoint(), property.Size.ToPoint()), 
                    sourceRectangle: property.Region.TextureRect,
                    property.TintColor,
                    property.Rotation,
                    property.Origin,
                    property.Effect,
                    property.LayerDepth
                );
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
        public TextSprite()
        {
            BaseProperty = new TextRenderProperty();
        }

        public override bool CheckVaild()
        {
            if (BaseProperty is not TextRenderProperty)
            {
                return false;
            }
            if ((BaseProperty as TextRenderProperty).Font == null)
            {
                return false;
            }
            return true;
        }

        public override bool Draw(SpriteBatch spriteBatch)
        {
            if (CheckVaild() == true)
            {
                var property = BaseProperty as TextRenderProperty;
                spriteBatch.DrawString(
                    property.Font,
                    property.TextSource.Message,
                    property.Position, 
                    property.TintColor, 
                    property.Rotation, 
                    property.Origin, 
                    property.Scale, 
                    property.Effect,
                    property.LayerDepth
                );
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


        public override bool Draw(SpriteBatch spriteBatch)
        {
            if (CheckVaild() == true)
            {
                var property = BaseProperty as TextureRenderProperty;
                spriteBatch.Draw(
                    Animation.CurrentFrame.Texture,
                    destinationRectangle: new Rectangle(property.Position.ToPoint(), property.Size.ToPoint()), 
                    sourceRectangle: Animation.CurrentFrame.TextureRect,
                    property.TintColor,
                    property.Rotation,
                    property.Origin,
                    property.Effect,
                    property.LayerDepth
                );
                Animation.Update(1);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class LerpSprite : TextureSprite
    {
        public TextureRenderProperty StartSprite { get; set; } = null;
        public TextureRenderProperty EndSprite { get; set; } = null;
        public float LerpDurationFrames { get; set; } = 0;
        public float CurrentFrame { get; set; } = 0;
        public float LerpFactor { get {return 1 / LerpDurationFrames; } }
        public float LerpProgress { get { return CurrentFrame / LerpDurationFrames; } }

        public bool AutoLoop { get; set; } = false;

        public int LoadEngineTick(int Tick)
        {
            int Duration = 60 * Tick;
            LerpDurationFrames = Duration;
            ResetLerp();
            return Duration;
        }

        public bool InitLerp(TextureRenderProperty Start , TextureRenderProperty End , int Ticks)
        {
            if (Start == null || End == null || Ticks <= 0)
            {
                return false;
            }
            StartSprite = Start;
            EndSprite = End;
            LoadEngineTick(Ticks);
            return true;
        }

        public bool PushPostion(Vector2 position )
        {
            StartSprite.Position = EndSprite.Position;
            EndSprite.Position = position;
            ResetLerp();
            return true;
        }

        public bool PushSize(Vector2 size )
        {
            StartSprite.Size = EndSprite.Size;
            EndSprite.Size = size;
            ResetLerp();
            return true;
        }

        public bool PushTicks(int Ticks)
        {
            LoadEngineTick(Ticks);
            return true;
        }

        public bool PushFrame()
        {
            if (CurrentFrame < LerpDurationFrames)
            {
                CurrentFrame++;
                BaseProperty.Position = (EndSprite.Position - StartSprite.Position) * LerpProgress + StartSprite.Position;
                (BaseProperty as TextureRenderProperty).Size = (EndSprite.Size - StartSprite.Size) * LerpProgress + StartSprite.Size;
                return true;
            }
            else if (AutoLoop == true)
            {
                ReverseLerp();
            }
            else if (AutoLoop == true && CurrentFrame >= LerpDurationFrames)
            {
                ResetLerp();
            }
            return false;
        }

        public bool ReverseLerp()
        {
            var LastPosition = StartSprite.Position;
            var LastSize = StartSprite.Size;
            StartSprite.Position = EndSprite.Position;
            StartSprite.Size = EndSprite.Size;
            EndSprite.Position = LastPosition;
            EndSprite.Size = LastSize;
            ResetLerp();
            return true;
        }

        public bool ResetLerp()
        {
            CurrentFrame = 0;
            return true;
        }

        public override bool CheckVaild()
        {
            if (StartSprite == null || EndSprite == null)
            {
                return false;
            }
            return base.CheckVaild();
        }

        public LerpSprite()
        {

        }

        public LerpSprite(TextureRenderProperty startSprite, TextureRenderProperty endSprite , int Ticks)
        {
            var _ = InitLerp(startSprite, endSprite , Ticks) ? BaseProperty.IsChanged = true : BaseProperty.IsChanged = false;
        }

        public override bool Draw(SpriteBatch spriteBatch)
        {
            if (CheckVaild() == true)
            {
                spriteBatch.Draw(
                    (BaseProperty as TextureRenderProperty).Region.Texture,
                    destinationRectangle: new Rectangle(
                        BaseProperty.Position.ToPoint(),
                        (BaseProperty as TextureRenderProperty).Size.ToPoint()),
                    sourceRectangle: (BaseProperty as TextureRenderProperty).Region.TextureRect,
                    BaseProperty.TintColor,
                    BaseProperty.Rotation,
                    BaseProperty.Origin,
                    BaseProperty.Effect,
                    BaseProperty.LayerDepth
                );
                PushFrame();

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}