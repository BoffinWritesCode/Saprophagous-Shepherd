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
    public class Mark : Projectile
    {
        public override bool Friendly => false;

        private float _timeAlive;
        private float _warningScale;

        public Mark(Vector2 p, Vector2 size) : base(p, Vector2.Zero, size)
        {
        }

        public override void PreUpdate(TimeManager time)
        {
            _timeAlive += time.DeltaTime;

            if (_timeAlive >= 4f)
            {
                float angle = -MathHelper.Pi;
                for (int i = 0; i < 8; i++)
                {
                    Vector2 direction = angle.ToVector2();

                    Main.SpawnProjectile(new RedBlot(Body.Center, direction * 200f, new Vector2(5)));

                    angle += MathHelper.PiOver4;
                }
                Destroy = true;
            }

            _warningScale = 0.6f + (((float)Math.Sin(time.TotalTime * 10f) + 1f) / 2f) * 0.4f;

            base.PreUpdate(time);
        }

        public override void Damage(Entity e)
        {
        }

        public override float GetOrderHeight()
        {
            return -99999f;
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            spriteBatch.Draw(Main.MarkWarningTexture, Body.Center, null, Color.White * _warningScale, Rotation, Main.MarkWarningTexture.Bounds.Size.ToVector2() * 0.5f, _warningScale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);

            spriteBatch.Draw(Main.MarkTexture, Body.Center, null, Color.White, Rotation, Main.MarkTexture.Bounds.Size.ToVector2() * 0.5f, 1f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
        }
    }
}
