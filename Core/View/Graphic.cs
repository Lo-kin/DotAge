using DotAge.Core.Control;
using DotAge.Core.Model;
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
        private EngineAccessor engineAccessor;
        private GraphicsDeviceManager _graphics;
        public static Camera2D ViewCamera = new Camera2D();
        private SpriteBatch DynamicSprite;
        private SpriteBatch StaticSprite;
        public static SpriteFont _font;
        public bool IsLoaded = false;
        public Dictionary<string, SoundEffect> SoundEffects = new();

        public Graphic(EngineAccessor ea)
        {
            engineAccessor = ea;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
            IsMouseVisible = true;
        }

        public bool LoadSoundEffect(string name)
        {
            try
            {
                SoundEffects[name] = Content.Load<SoundEffect>(name);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Load Sound Effect Error: " + ex.Message);
                return false;
            }
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
        }
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            base.Update(gameTime);
        }

        public static StringBuilder sb = new StringBuilder();

        protected override void Draw(GameTime gameTime)
        {
            DateTime start = DateTime.Now;
            GraphicsDevice.Clear(Color.Black);
            DynamicSprite.Begin(transformMatrix:ViewCamera.View);
            StaticSprite.Begin();
            try
            {
                foreach (var item in engineAccessor.GetAllRenderObjs)
                {
                    if (item.RenderProp.IsVisible == true)
                    {
                        if (item.RenderProp.IsStatic == true)
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

            DrawScriptInfo.Draw(StaticSprite);

            DynamicSprite.End();
            StaticSprite.End();
            base.Draw(gameTime);
        }
        public Vector2 WishForce = Vector2.Zero;
        public double CoreTime = 0;
        public static MessageEntity CoreInfo = new MessageEntity("");
        public TextScroller DrawInfo = new TextScroller(CoreInfo)
        {
            RenderProperties = [(new TextRenderProperty()
            {
                TextSource = CoreInfo,
                IsStatic = true,
                Position = new Vector2(10, 10),
                TintColor = Color.Yellow,
                Font = _font,
            } , 0)],

        };

        public static MessageEntity ScriptInfo = new MessageEntity();
        public Sprite DrawScriptInfo = new Sprite()
        {
            RenderProperties = [(new TextRenderProperty()
            {
                TextSource = ScriptInfo,
                IsStatic = true,
                Position = new Vector2(10, 100),
                TintColor = Color.Blue,
                Font = _font,
            } , 0)],

        };
    }
}