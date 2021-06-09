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
    public class Arrow : Projectile
    {
        public override bool Friendly => true;

        public Arrow(Vector2 p, Vector2 v, Vector2 size) : base(p, v, size)
        {
            Rotation = v.ToAngle() + MathHelper.PiOver4;
        }

        public override void PreUpdate(TimeManager time)
        {
            Rotation = Velocity.ToAngle() + MathHelper.PiOver4;

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

            spriteBatch.Draw(Main.ArrowTexture, Body.Center, null, Color.White, Rotation, Main.RockTexture.Bounds.Size.ToVector2() * 0.5f, 1f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
        }
    }
}
