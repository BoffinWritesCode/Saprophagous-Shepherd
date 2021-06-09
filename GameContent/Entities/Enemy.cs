using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using BoffXNA;
using BoffXNA.Base;
using BoffXNA.Graphics;

namespace MiniJam61Egypt.GameContent.Entities
{
    public class Enemy : Entity
    {
        public override bool Solid => false;
        public override bool HitsWalls => true;

        public Enemy(Vector2 pos, Vector2 size, float health)
        {
            Body.Width = size.X;
            Body.Height = size.Y;
            Body.Center = pos;
            Health = MaxHealth = health;
        }

        public virtual bool Hittable() { return true; }

        public override void Damage(float health)
        {
            base.Damage(health);
        }

        public override void Kill()
        {
            Destroy = true;

            Rectangle space = GetDrawBody();
            for (int i = 0; i < Main.GameRandom.Next(9, 15); i++)
            {
                Main.SummonParticlesColorRamp(Main.SmokeTexture, 1, new Vector2(space.X + Main.GameRandom.NextFloat(space.Width), space.Y + Main.GameRandom.NextFloat(space.Height)), Main.GameRandom.NextFloat(MathHelper.TwoPi).ToVector2() * Main.GameRandom.NextFloat(0f, 20f), Vector2.Zero, new Color(160, 160, 160), Color.White, Main.GameRandom.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4), 0.2f, 0.5f, 0, MathHelper.TwoPi, 0.6f, 1f, 10f);
            }
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            int maxWidth = (int)Body.Width;
            int height = 3;
            int width = (int)((Health / MaxHealth) * maxWidth);

            int x = (int)Body.Center.X - maxWidth / 2;
            int y = (int)Body.Bottom + 1;

            spriteBatch.Draw(spriteBatch.Pixel, new Rectangle(x, y, maxWidth, height), new Color(70, 0, 0));
            spriteBatch.Draw(spriteBatch.Pixel, new Rectangle(x, y, width, height), new Color(230, 0, 0));
        }
    }
}
