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
    public class XGain : Entity
    {
        public override bool Solid => false;

        private int _amount;
        private float _timeAlive;
        private Texture2D _sprite;

        public XGain(Texture2D sprite, Vector2 pos, int amount) : base()
        {
            _sprite = sprite;
            Body.Center = pos;
            _amount = amount;
        }

        public override void PreUpdate(BoffXNA.Base.TimeManager time)
        {
            _timeAlive += time.DeltaTime;
            if (_timeAlive >= 1f)
            {
                Destroy = true;
            }
            Velocity.Y = -8f;
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            Color color = Color.White * (1f - _timeAlive);
            spriteBatch.Draw(_sprite, Body.Center - new Vector2(16, 4), color);
            Main.DrawBorderText(spriteBatch.Batch, _amount.ToString(), Body.Center - new Vector2(2, 2), color, Color.Black * (1f - _timeAlive));
        }

        public override float GetOrderHeight()
        {
            return 999999;
        }
    }
}
