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
    public class AnubisSun : Projectile
    {
        public override bool Friendly => false;

        private float _timeAlive;
        private float _scale;

        private List<Entity> _iveHit;
        private bool _startedMoving;

        public AnubisSun(Vector2 p) : base(p, new Vector2(0, -18f), new Vector2(60))
        {
            _iveHit = new List<Entity>();
        }

        public override void PreUpdate(TimeManager time)
        {
            _timeAlive += time.DeltaTime;
            if (_timeAlive > 4.5f)
            {
                if (!_startedMoving)
                {
                    _startedMoving = true;
                    Velocity = Vector2.Normalize(Main.Instance.player.Body.Center - Body.Center) * 300f;
                }
            }

            _scale = MathHelper.Lerp(0f, 0.75f, _timeAlive / 4.5f);
            _scale += Main.GameRandom.NextFloat(-0.03f, 0.03f);
            if (_scale < 0f) _scale = 0f;
            if (_scale > 0.75f) _scale = 0.75f;

            base.PreUpdate(time);
        }

        public override void Damage(Entity e)
        {
            if (!_iveHit.Contains(e) && _timeAlive > 4.5f)
            {
                e.Damage(25f);
                _iveHit.Add(e);
            }
        }

        public override float GetOrderHeight()
        {
            return 99999f;
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            spriteBatch.Draw(Main.CircleTexture, Body.Center, null, Color.Lerp(Color.White, new Color(255, 197, 153), Main.GameRandom.NextFloat(1f)), 0f, Main.CircleTexture.Bounds.Size.ToVector2() * 0.5f, _scale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
        }
    }
}
