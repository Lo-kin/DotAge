using DotAge.Core.Control;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DotAge.Core.View
{
    public class Graphic : Game
    {
        private GraphicsDeviceManager _graphics;
        public static Camera2D ViewCamera = new Camera2D();
        private SpriteBatch spb;
        private SpriteFont _font;
        public Texture2D[] _texture = new Texture2D[6];
        public RenderProperty[] BufferRP = new RenderProperty[65536];//0,0位置显示错误推测：后端tps低导致在添加对象后还未更新对象导致 , 0位置不使用
        public List<int> EmptyRP = Enumerable.Range(0, 65536).ToList();
        public List<int> ExistRP = new List<int>();
        //public 
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

        public bool ModifyRPBuffer(int Pos, RenderProperty rp)
        {
            if (rp.Init == false)
            {
                return false;
            }
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
            var tmp = new RenderProperty();
            AddRPBuffer(ref tmp);
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
            spb = new SpriteBatch(GraphicsDevice);
            _texture[0] = Content.Load<Texture2D>("default_texture");
            _texture[1] = Content.Load<Texture2D>("Character");
            _texture[2] = Content.Load<Texture2D>("Status");
            _font = Content.Load<SpriteFont>("Default");
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            // TODO: Add your update logic here
            if (IsRPModified == true)//简单的添加绘图事件
            {
                LoadRP = ExistRP.ToArray();
                IsRPModified = false;
            }
            
            base.Update(gameTime);
        }
        int a = 0;
        protected override void Draw(GameTime gameTime)
        {
            a++;
            GraphicsDevice.Clear(Color.Black);
            spb.Begin(transformMatrix:ViewCamera.View);

            for (int i = 0; i < LoadRP.Count(); i++)
            {
                RenderProperty TmpRP = BufferRP[LoadRP[i]];
                if (TmpRP.Visibility == false)
                {
                    continue;
                }
                if (TmpRP.Visibility == true)
                {
                    spb.Draw(_texture[1],new Rectangle( TmpRP.RenderPosition.ToPoint() , TmpRP.Size.ToPoint()), TextureIndex.GetTextureX(TmpRP.RenderTexture), TmpRP.TintColor);
                }

            }
            spb.End();
            spb.Begin();
            if (Engine.tessta.Length < 900)
            {
                string text = "";
                for (int i = 4; i >= 1; i--)
                {
                    text += Controlers.Log[^i] + "\n";
                }
                text += Controlers.NowMouseStat.Position.ToString() + "\n";
                if (!(MouseZone.Log.Count < 3))
                {
                    for (int i = 2; i >= 1; i--)
                    {
                        text += MouseZone.Log[^i] + "\n";
                    }
                }
                text += "Mouse Postion : " + Controlers.CurrentMapMousePosition.ToString() + "\n";

                spb.DrawString(_font, text, Vector2.Zero, Color.Red);
                //Thread.Sleep(100000);
            }

            spb.End();
            base.Draw(gameTime);
        }
    }
}
