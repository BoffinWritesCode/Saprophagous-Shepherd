using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using BoffXNA;
using BoffXNA.Base;
using BoffXNA.Graphics;

using MiniJam61Egypt.Physics;

namespace MiniJam61Egypt.GameContent.Entities.Enemies
{
    public class Obelisk : Enemy
    {
        public Obelisk(Vector2 pos) : base(pos, new Vector2(22, 44), 10 * (1f + (Main.RentsPaid + 1) * 0.2f))
        {
        }

        public override void Kill()
        {
            base.Kill();

            Main.Coins += 10;
            Main.stats.MoneyEarnt += 10;

            Main.Instance.entities.Add(new Entities.Other.XGain(Main.SmallCoinTexture, Body.Center - new Vector2(0, 12), 10));
        }

        public override Rectangle GetDrawBody()
        {
            Rectangle body = Body.ToRectangle();
            return new Rectangle(body.X, body.Top - 20, 22, 64);
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Main.ObeliskTexture, new Vector2(Body.TopLeft.X, Body.TopLeft.Y - 20 + (float)Math.Sin(Main.Instance.TimeManager.TotalTime * 0.7f) * 4f), null, HitColor);

            base.Draw(spriteBatch);
        }
    }
}
