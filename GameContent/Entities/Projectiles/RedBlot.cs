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
    public class RedBlot : Projectile
    {
        public override bool Friendly => false;

        private float _rotateSpeed;

        public RedBlot(Vector2 p, Vector2 v, Vector2 size) : base(p, v, size)
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
            e.Damage(5f);

            Destroy = true;
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            spriteBatch.Draw(Main.RedBlotTexture, Body.Center, null, Color.White, Rotation, Main.RedBlotTexture.Bounds.Size.ToVector2() * 0.5f, 1f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
        }
    }
}
