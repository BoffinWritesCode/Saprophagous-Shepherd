using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using BoffXNA.Graphics;

namespace MiniJam61Egypt.GameContent.Entities.Other
{
    public class WorldDecoration : Entity
    {
        public override bool Solid => true;
        public override bool Static => true;

        private Texture2D _texture;
        private Vector2 _drawOffset;

        public WorldDecoration(Rectangle hitbox, Vector2 drawOffset, Texture2D texture)
        {
            _texture = texture;
            Body = new Util.BoundingBox2D(hitbox);
            _drawOffset = drawOffset;

            Main.Instance.physicsManager.AddBody(Body.ToRectangle());
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Body.TopLeft + _drawOffset, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
