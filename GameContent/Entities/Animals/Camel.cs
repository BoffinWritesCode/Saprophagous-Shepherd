using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BoffXNA.Base;
using BoffXNA.Graphics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using BoffXNA;

namespace MiniJam61Egypt.GameContent.Entities.Animals
{
    public class Camel : Animal
    {
        public override int Type => 2;

        public Camel(Rectangle explore) : base(explore) 
        {
            Body = new Util.BoundingBox2D(0, 0, 37, 24);
            Health = MaxHealth = 90f;
            _alt = Main.GameRandom.Next(2);
            if (Main.GameRandom.Next(20) == 0)
            {
                _alt = 2;
            }

            Main.SFXManager.PlaySound("Camel_" + Main.GameRandom.Next(2), 0.3f, 0f, Main.GameRandom.NextFloat(-0.2f, 0.04f));
        }

        public override Rectangle GetMouseHitbox()
        {
            Rectangle hitBox = Body.ToRectangle();
            return new Rectangle(hitBox.X - (_facingLeft ? 15 : 4), hitBox.Y - 18, 59, 42);
        }

        public override Rectangle GetDrawBody()
        {
            return GetMouseHitbox();
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            SpriteEffects effect = _facingLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Rectangle source = new Rectangle(59 * _alt, 0, 59, 42);
            spriteBatch.Draw(IsBaby ? Main.BabyCamelTexture : Main.CamelTexture, Body.TopLeft - new Vector2(_facingLeft ? 15 : 4, 18), source, HitColor, 0f, Vector2.Zero, 1f, effect, 0f);

            base.Draw(spriteBatch);
        }

        public override void Kill()
        {
            base.Kill();

            Main.SFXManager.PlaySound("Camel_" + Main.GameRandom.Next(2), 0.3f, 0f, Main.GameRandom.NextFloat(-0.2f, 0.04f));
        }

        public override void RightClicked()
        {
            base.RightClicked();

            Main.SFXManager.PlaySound("Camel_" + Main.GameRandom.Next(2), 0.3f, 0f, Main.GameRandom.NextFloat(-0.2f, 0.04f));
        }

        public override void DrawHover(ExtendedSpriteBatch spriteBatch)
        {
            Color c = GOOD_HOVER;
            if (Vector2.Distance(Body.Center, Main.Instance.player.Body.Center) > CLICK_DISTANCE)
            {
                c = BAD_HOVER;
            }

            SpriteEffects effect = _facingLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(IsBaby ? Main.BabyCamelOutlineTexture : Main.CamelOutlineTexture, Body.TopLeft - new Vector2(_facingLeft ? 15 : 4, 18), null, c, 0f, Vector2.Zero, 1f, effect, 0f);

            Vector2 size = Main.MainFont.MeasureString(Name);
            int offsetY = IsBaby ? 14 : 23;
            Main.MainFont.Draw(spriteBatch.Batch, Name, new Vector2(Body.Center.X, Body.Top - offsetY + 1) - size * 0.5f, Color.Black);
            Main.MainFont.Draw(spriteBatch.Batch, Name, new Vector2(Body.Center.X + 1, Body.Top - offsetY) - size * 0.5f, Color.Black);
            Main.MainFont.Draw(spriteBatch.Batch, Name, new Vector2(Body.Center.X - 1, Body.Top - offsetY) - size * 0.5f, Color.Black);
            Main.MainFont.Draw(spriteBatch.Batch, Name, new Vector2(Body.Center.X, Body.Top - offsetY - 1) - size * 0.5f, Color.Black);
            Main.MainFont.Draw(spriteBatch.Batch, Name, new Vector2(Body.Center.X, Body.Top - offsetY) - size * 0.5f, Color.White);

            base.DrawHover(spriteBatch);
        }
    }
}
