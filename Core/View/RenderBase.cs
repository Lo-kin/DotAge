using DotAge.Core.Control;
using DotAge.Core.Model;
using DotAge.Core.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace DotAge.Core.View
{
    public class TextScroller : IRender
    {
        public RenderProperty RenderProp { get; set; }

        public TextScroller(string text)
        {
            var me = new MessageEntity(text);
            RenderProp = new TextRenderProperty(me , Graphic._font);
        }
        public TextScroller(MessageEntity me)
        {
            RenderProp = new TextRenderProperty(me, Graphic._font);
        }
    }
    public interface IRender
    {
        public RenderProperty RenderProp { get; set; }

        public virtual bool CheckVaild()
        {
            return true;
        }

        public bool Draw(SpriteBatch spriteBatch)
        {
            RenderProp.Draw(spriteBatch);
            return true;
        }

        public bool Dispose()
        {
            RenderProp = null;
            return true;
        }
    }

    public abstract class RenderProperty
    {
        public ValueLerp<ILocation> Location { get; set; } = new();
        public Color TintColor { get; set; } = Color.White;
        public Vector2 Origin { get { return Location.CurrentFrame.CrashBox.Center; } }
        public SpriteEffects Effect { get; set; } = SpriteEffects.None;
        public float LayerDepth { get; set; } = 0.0f;
        public bool IsVisible { get; set; } = true;
        public bool IsStatic { get; set; } = false;

        public RenderProperty()
        {

        }

        public virtual bool CheckVaild()
        {
            return true;
        }
        public virtual bool Update(int time = 1)
        {
            return Location.UpdateTimer();

        }
        public virtual bool Draw(SpriteBatch spriteBatch)
        {
            return false;
        }
    }

    public class TextureRenderProperty : RenderProperty
    {
        public ValueLerp<TextureRegion> Frames { get; set; } = new();
        public override bool Update(int time = 1)
        {
            return Frames.UpdateTimer() && base.Update();
        }
        public override bool Draw(SpriteBatch spriteBatch)
        {
            if (CheckVaild() == true)
            {
                Update();
                Location lc = ILocation.GetLerpLocation(Location.CurrentFrame, Location.NextFrame , Location.FramePerc);
                spriteBatch.Draw(
                    Frames.CurrentFrame.Texture,
                    destinationRectangle: new Rectangle(
                        lc.Position.ToPoint(),
                        lc.Size.ToPoint()),
                    sourceRectangle: Frames.CurrentFrame.TextureRect,
                    TintColor,
                    lc.Rotation,
                    Origin,
                    Effect,
                    LayerDepth
                );
                return true;
            }
            return base.Draw(spriteBatch);
        }
    }

    public class TextRenderProperty : RenderProperty
    {
        public MessageEntity TextSource { get; set; }
        public SpriteFont Font { get; set; }
        public override bool CheckVaild()
        {
            return Font != null & base.CheckVaild();
        }
        public override bool Update(int time = 1)
        {
            TextSource.Update();
            return base.Update(time);
        }
        public TextRenderProperty(MessageEntity text, SpriteFont font)
        {
            TextSource = text;
            Font = font;
        }
        public override bool Draw(SpriteBatch spriteBatch)
        {
            if (CheckVaild() == true)
            {
                Update();
                Location lc = ILocation.GetLerpLocation(Location.CurrentFrame, Location.NextFrame, Location.FramePerc);
                spriteBatch.DrawString(
                    Font,
                    TextSource.ScrollMsg,
                    lc.Position,
                    TintColor,
                    lc.Rotation,
                    Origin,
                    lc.Scale,
                    Effect,
                    LayerDepth
                );
                return true;
            }
            return base.Draw(spriteBatch);
        }
    }
}

