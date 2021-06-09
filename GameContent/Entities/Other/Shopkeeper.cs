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
    public class Shopkeeper : Entity
    {
        public override bool Static => true;

        private string _text;

        private const int BOX_BORDER = 5;

        public Shopkeeper(Vector2 position)
        {
            Body.Width = 33;
            Body.Height = 42f;
            Body.Center = position;
            _text = "";
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Main.ShopkeeperTexture, Body.TopLeft, Color.White);

            Vector2 size = Main.MainFont.MeasureString(_text);
            Vector2 center = Body.Center - Vector2.UnitY * 44f;
            Vector2 textPos = center - size * 0.5f;
            spriteBatch.Draw(spriteBatch.Pixel, new Rectangle((int)textPos.X - BOX_BORDER, (int)textPos.Y - (BOX_BORDER - 1), (int)size.X + BOX_BORDER * 2, (int)size.Y + (BOX_BORDER - 1) * 2), Color.White * 0.6f);
            spriteBatch.Draw(spriteBatch.Pixel, new Rectangle((int)textPos.X - (BOX_BORDER - 1), (int)textPos.Y - BOX_BORDER, (int)size.X + (BOX_BORDER - 1) * 2, (int)size.Y + BOX_BORDER * 2), Color.White * 0.6f);

            Main.DrawBorderText(spriteBatch.Batch, _text, textPos, Color.Black, Color.TransparentBlack);

            _text = "Welcome to my shop friend, mouse over what you need!\nBring animals over the blue carpet and I'll buy them!";
        }

        public void DisplayText(string text)
        {
            _text = text;
        }
    }
}
