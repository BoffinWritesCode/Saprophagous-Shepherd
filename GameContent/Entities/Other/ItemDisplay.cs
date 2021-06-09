using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BoffXNA.Base;
using BoffXNA.Graphics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniJam61Egypt.GameContent.Entities.Other
{
    public class ItemDisplay : Entity
    {
        public override bool Static => true;
        public override bool CanHover => true;
        public override bool ConsumeMouse => true;

        public Texture2D ItemTexture;
        public Vector2 DrawOffset;
        public Action LeftClick;
        public string Name;
        public string Description;
        public Rectangle Source;
        public int Cost;

        private float _yOffset;
        private bool _playedSound;

        public ItemDisplay(Vector2 position, Texture2D item, Vector2 offset, string name, string desc, int cost, Action leftClick)
        {
            Body.Width = 50;
            Body.Height = 50f;
            Body.Center = position;

            Source = item.Bounds;
            Cost = cost;
            DrawOffset = offset;
            Description = desc;
            ItemTexture = item;
            LeftClick = leftClick;
            Name = name;
        }

        public override bool MouseOver()
        {
            bool yes = Body.ToRectangle().Contains(Main.Instance.MouseInWorld());
            if (yes)
            {
                if (!_playedSound)
                {
                    _playedSound = true;
                    Main.SFXManager.PlaySound("Tick", 0.1f, 0f, 0.5f);
                }
            }
            else
            {
                _playedSound = false;
            }
            return yes;
        }

        public override void LeftClicked()
        {
            Main.CanShoot = false;
            LeftClick?.Invoke();
        }

        public override void PreUpdate(TimeManager time)
        {
            _yOffset = (float)Math.Sin(time.TotalTime * 2f) * 2 - 6;
            base.PreUpdate(time);
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            Rectangle body = Body.ToRectangle();
            if (MouseOver())
            {
                Main.Instance.shopkeeper.DisplayText(Description);
                if (Main.Instance.InputManager.MouseLeftDown)
                {
                    body.Inflate(-2, -2);
                }
                spriteBatch.Draw(spriteBatch.Pixel, body, Color.Black * 0.4f);
            }

            spriteBatch.Draw(ItemTexture, Body.Center - Source.Size.ToVector2() * 0.5f + Vector2.UnitY * _yOffset + DrawOffset, Source, Color.White);
            Vector2 size = Main.MainFont.MeasureString(Name);
            Main.DrawBorderText(spriteBatch.Batch, Name, new Vector2(Body.Center.X, Body.Bottom - 8 + _yOffset) - size * 0.5f, Color.White, Color.Black);
            if (Cost >= 0)
            {
                string text = Cost.ToString() + " coins";
                size = Main.MainFont.MeasureString(text);
                Main.DrawBorderText(spriteBatch.Batch, text, new Vector2(Body.Center.X, Body.Bottom + 4 + _yOffset) - size * 0.5f, Color.White, Color.Black);
            }
        }
    }
}
