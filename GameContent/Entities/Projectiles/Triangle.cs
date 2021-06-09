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
    public class Triangle : Projectile
    {
        public override bool Friendly => false;

        private float _waitTime;
        private Vector2 _eventualVelocity;

        public Triangle(float waitTime, Vector2 p, Vector2 v, Vector2 size) : base(p, v, size)
        {
            _waitTime = waitTime;
            _eventualVelocity = v;
            Rotation = _eventualVelocity.ToAngle() + MathHelper.PiOver2;
        }

        public override void PreUpdate(TimeManager time)
        {
            _waitTime -= time.DeltaTime;
            if (_waitTime <= 0)
            {
                Velocity = _eventualVelocity;
            }
            else
            {
                Velocity = Vector2.Zero;
            }

            Rotation = _eventualVelocity.ToAngle() + MathHelper.PiOver2;

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

            spriteBatch.Draw(Main.TriangleTexture, Body.Center, null, Color.White, Rotation, Main.TriangleTexture.Bounds.Size.ToVector2() * 0.5f, 1f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
        }
    }
}
