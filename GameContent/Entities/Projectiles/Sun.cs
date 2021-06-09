using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using BoffXNA;
using BoffXNA.Base;
using BoffXNA.Graphics;

namespace MiniJam61Egypt.GameContent.Entities.Projectiles
{
    public class Sun : Projectile
    {
        public override bool Friendly => true;

        private float _rotateSpeed;

        public Sun(Vector2 p, Vector2 v, Vector2 size) : base(p, v, size)
        {
            Rotation = Main.GameRandom.NextFloat(MathHelper.TwoPi);
            _rotateSpeed = Main.GameRandom.Next(2) == 0 ? Main.GameRandom.NextFloat(-12f, -8f) : Main.GameRandom.NextFloat(8f, 12f);
        }

        public override void PreUpdate(TimeManager time)
        {
            Rotation += time.DeltaTime * _rotateSpeed;

            base.PreUpdate(time);
        }

        public override void Damage(Entity e)
        {
            e.Damage(3f);

            Destroy = true;
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            spriteBatch.Draw(Main.SunTexture, Body.Center, null, Color.White, Rotation, Main.SunTexture.Bounds.Size.ToVector2() * 0.5f, 1f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
        }
    }
}
