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
            if (rp.Init == false)
            {
                return -1;
            }
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

        public bool AddTexture(string _textureName , int position)
        {
            _texture[position] = Content.Load<Texture2D>(_textureName);
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
            _texture = new Texture2D[_texture.Length + 1];
            int count = -1;
            foreach (var item in TextureManager.BufferTextureNames)
            {
                count ++;
                AddTexture(item, count);
                TextureManager.TextureLoadPosition[item] = count;
            }
            TextureManager.BufferTextureNames.Clear();
            _font = Content.Load<SpriteFont>("Default");
            Engine.TextureLoadedFlag = true;
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



        protected override void Draw(GameTime gameTime)
        {
            
            //GraphicsDevice.Clear(Color.Black);
            DynamicSprite.Begin(transformMatrix:ViewCamera.View);
            StaticSprite.Begin();
            for (int i = 0; i < LoadRP.Count(); i++)
            {
                RenderProperty TmpRP = BufferRP[LoadRP[i]];
                if (TmpRP.Visibility == false)
                {
                    continue;
                }
                if (TmpRP.Visibility == true)
                {
                    if (TmpRP.RenderTexture.Item1 == -1)
                    {
                        continue;
                    }
                    if (TmpRP.IsFixed == false)
                    {
                        if (TmpRP.IsShowTexture == true)
                        {
                            
                            DynamicSprite.Draw(_texture[TmpRP.RenderTexture.Item1], new Rectangle(TmpRP.RenderPosition.ToPoint(), TmpRP.Size.ToPoint()), TmpRP.RenderTexture.Item2, TmpRP.TintColor);
                        }
                        if (TmpRP.IsShowText == true)
                        {
                            DynamicSprite.DrawString(_font , TmpRP.Text , TmpRP.RenderPosition, TmpRP.TintColor);
                        }
                    }
                    else
                    {
                        if (TmpRP.IsShowTexture == true)
                        {
                            StaticSprite.Draw(_texture[TmpRP.RenderTexture.Item1], new Rectangle(TmpRP.RenderPosition.ToPoint(), TmpRP.Size.ToPoint()), TmpRP.RenderTexture.Item2, TmpRP.TintColor);
                        }
                        if (TmpRP.IsShowText == true)
                        {
                            StaticSprite.DrawString(_font, TmpRP.Text, TmpRP.RenderPosition, TmpRP.TintColor);
                        }
                    }
                }
            }

            if (Engine.tessta.Length < 900)
            {
                //StaticSprite.DrawString(_font, city1.GetAllProductInfo + "\n" + city2.GetAllProductInfo + "\n" + GameData.GameEntities[Engine.Code].GameEntity.Money + "\n" + GameData.GameEntities[Engine.Code].GameEntity.GetProductCount, Vector2.Zero, Color.Red);
            }
            DynamicSprite.End();
            StaticSprite.End();
            base.Draw(gameTime);
        }

    }
}
