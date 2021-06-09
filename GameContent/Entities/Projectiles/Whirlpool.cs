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
    public class Whirlpool : Projectile
    {
        public override bool Friendly => false;

        private float _w1Rot;
        private float _w2Rot;
        private float _w3Rot;

        private bool _dying;
        private float _damageDealt;

        private float _timeAlive;
        private float _scale = 0f;

        public Whirlpool(Vector2 p) : base(p, Vector2.Zero, new Vector2(32))
        {
        }

        public override void PreUpdate(TimeManager time)
        {
            _w1Rot += time.DeltaTime;
            _w2Rot -= time.DeltaTime * 1.3f;
            _w3Rot += time.DeltaTime * 1.8f;

            _timeAlive += time.DeltaTime;

            if (_timeAlive > 10f)
            {
                _dying = true;
            }

            if (!_dying)
            {
                TrySuckIn(Main.Instance.player);
                foreach(Animals.Animal an in Main.Instance.animals)
                {
                    if (!an.Following)
                        TrySuckIn(an);
                }

                _scale += time.DeltaTime * 0.8f;
                if (_scale > 1f)
                {
                    _scale = 1f;
                }
            }
            else
            {
                _scale -= time.DeltaTime * 0.6f;
                if (_scale <= 0f)
                {
                    Destroy = true;
                }
            }

            base.PreUpdate(time);
        }

        private void TrySuckIn(Entity e)
        {
            float dist = Vector2.Distance(Body.Center, e.Body.Center);
            if (!_dying && dist < 128f)
            {
                float strength = 1f - (dist / 128f);
                Vector2 toMe = Body.Center - e.Body.Center;
                e.Velocity += Vector2.Normalize(toMe) * strength * 128f;
            }
        }

        public override void Damage(Entity e)
        {
            if (!_dying)
            {
                e.Damage(0.2f);
                _damageDealt += 0.2f;
                if (_damageDealt >= 35f)
                {
                    _dying = true;
                }
            }
        }

        public override float GetOrderHeight()
        {
            return -99999f;
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            spriteBatch.Draw(Main.WhirlpoolTextures[0], Body.Center, null, Color.White, _w1Rot, new Vector2(29), _scale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
            spriteBatch.Draw(Main.WhirlpoolTextures[1], Body.Center, null, Color.White, _w2Rot, new Vector2(29), _scale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
            spriteBatch.Draw(Main.WhirlpoolTextures[2], Body.Center, null, Color.White, _w3Rot, new Vector2(29), _scale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
        }
    }
}
