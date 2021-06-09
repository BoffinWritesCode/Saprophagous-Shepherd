using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BoffXNA.Graphics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniJam61Egypt.GameContent.Entities.Other
{
    public class Particle : Entity
    {
        public override bool Solid => false;

        private Texture2D _texture;
        public float Rotation;
        public float Scale;
        public float TimeAlive;
        public float MaxTime;
        public Color Color;
        public Vector2 Acceleration;
        public bool FadeOut;
        public float RotationVelocity;
        public bool OnTop;

        public Particle(Vector2 pos, float timeToLive, Texture2D texture) : base()
        {
            Body.Center = pos;
            _texture = texture;
            MaxTime = timeToLive;

            Color = Color.White;
            FadeOut = true;
        }

        public override void PreUpdate(BoffXNA.Base.TimeManager time)
        {
            TimeAlive += time.DeltaTime;
            Velocity += Acceleration * time.DeltaTime;
            Rotation += RotationVelocity * time.DeltaTime;
            if (TimeAlive >= MaxTime)
            {
                Destroy = true;
            }
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            Color color = FadeOut ? Color * (1f - (TimeAlive / MaxTime)) : Color;
            spriteBatch.Draw(_texture, Body.Center, null, color, Rotation, _texture.Bounds.Size.ToVector2() * 0.5f, Scale, SpriteEffects.None, 0f);
        }

        public override float GetOrderHeight()
        {
            if (OnTop) return 9999999;
            return base.GetOrderHeight();
        }
    }
}
