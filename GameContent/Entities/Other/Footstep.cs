using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BoffXNA.Base;
using BoffXNA.Graphics;

using Microsoft.Xna.Framework;

namespace MiniJam61Egypt.GameContent.Entities.Other
{
    public class Footstep : Entity
    {
        public override bool Static => true;

        private float _timeAlive;
        private bool _flipped;

        public Footstep(Vector2 position)
        {
            Body.Center = position;
            _flipped = new Random().Next(2) == 0;
        }

        public override void PreUpdate(TimeManager time)
        {
            _timeAlive += time.DeltaTime;
            if (_timeAlive > 1f)
            {
                Kill();
            }

            base.PreUpdate(time);
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Main.FootstepTexture, Body.Center - Vector2.One * 4f, null, Color.White * MathHelper.Lerp(0.7f, 0f, _timeAlive / 1f), 0f, Vector2.Zero, 1f, _flipped ? Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally : Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
        }

        public override float GetOrderHeight()
        {
            return -100000f;
        }
    }
}
