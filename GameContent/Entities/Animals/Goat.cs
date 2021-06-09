using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BoffXNA.Base;
using BoffXNA.Graphics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniJam61Egypt.GameContent.Entities.Animals
{
    public class Goat : Animal
    {
        public override int Type => 1;

        public Goat(Rectangle explore) : base(explore) 
        {
            Body = new Util.BoundingBox2D(0, 0, 29, 18);
            Health = MaxHealth = 55f;
            _alt = Main.GameRandom.Next(3);
            if (Main.GameRandom.Next(16) == 0)
            {
                _alt = 3;
            }
            Main.SFXManager.PlaySound("Goat_" + Main.GameRandom.Next(3), 0.07f);
        }

        public override Rectangle GetMouseHitbox()
        {
            Rectangle hitBox = Body.ToRectangle();
            return new Rectangle(hitBox.X - (_facingLeft ? 8 : 4), hitBox.Y - 17, 41, 42);
        }

        public override Rectangle GetDrawBody()
        {
            return GetMouseHitbox();
        }

        public override void RightClicked()
        {
            base.RightClicked();

            Main.SFXManager.PlaySound("Goat_" + Main.GameRandom.Next(3), 0.07f);
        }

        public override void Kill()
        {
            base.Kill();

            Main.SFXManager.PlaySound("Goat_" + Main.GameRandom.Next(3), 0.07f);
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            SpriteEffects effect = _facingLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Rectangle source = new Rectangle(41 * _alt, 0, 41, 42);
            spriteBatch.Draw(IsBaby ? Main.BabyGoatTexture : Main.GoatTexture, Body.TopLeft - new Vector2(_facingLeft ? 8 : 4, 17), source, HitColor, 0f, Vector2.Zero, 1f, effect, 0f);

            base.Draw(spriteBatch);
        }

        public override void DrawHover(ExtendedSpriteBatch spriteBatch)
        {
            Color c = GOOD_HOVER;
            if (Vector2.Distance(Body.Center, Main.Instance.player.Body.Center) > CLICK_DISTANCE)
            {
                c = BAD_HOVER;
            }

            SpriteEffects effect = _facingLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(IsBaby ? Main.BabyGoatOutlineTexture : Main.GoatOutlineTexture, Body.TopLeft - new Vector2(_facingLeft ? 8 : 4, 17), null, c, 0f, Vector2.Zero, 1f, effect, 0f);

            Vector2 size = Main.MainFont.MeasureString(Name);
            int offsetY = IsBaby ? 12 : 22;
            Main.MainFont.Draw(spriteBatch.Batch, Name, new Vector2(Body.Center.X, Body.Top - offsetY + 1) - size * 0.5f, Color.Black);
            Main.MainFont.Draw(spriteBatch.Batch, Name, new Vector2(Body.Center.X + 1, Body.Top - offsetY) - size * 0.5f, Color.Black);
            Main.MainFont.Draw(spriteBatch.Batch, Name, new Vector2(Body.Center.X - 1, Body.Top - offsetY) - size * 0.5f, Color.Black);
            Main.MainFont.Draw(spriteBatch.Batch, Name, new Vector2(Body.Center.X, Body.Top - offsetY - 1) - size * 0.5f, Color.Black);
            Main.MainFont.Draw(spriteBatch.Batch, Name, new Vector2(Body.Center.X, Body.Top - offsetY) - size * 0.5f, Color.White);

            base.DrawHover(spriteBatch);
        }
    }
}
