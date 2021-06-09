using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BoffXNA.Base;
using BoffXNA.Graphics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniJam61Egypt.Environment
{
    public abstract class Tile
    {
        public bool HasFootsteps { get; set; }
        public bool AnimalSale { get; set; }
        public virtual bool IsSolid => false;
        public virtual void WalkedOver() { }
        public virtual void LeftClicked(int hotbarSlot) { }
        public virtual void Update(TimeManager time) { }
        public virtual void Draw(int x, int y, ExtendedSpriteBatch spriteBatch) { }
    }

    public class NormalTile : Tile
    {
        public Texture2D MyTexture { get; set; }
        public Rectangle Source { get; set; }

        public NormalTile(Texture2D texture)
        {
            HasFootsteps = true;
            MyTexture = texture;
            Source = MyTexture.Bounds;
        }

        public override void Draw(int x, int y, ExtendedSpriteBatch spriteBatch)
        {
            spriteBatch.Draw(MyTexture, new Rectangle(x * 32, y * 32, 32, 32), Source, Color.White);
        }
    }

    public class FarmTile : NormalTile
    {
        private const float MAX_WATER = 22f;
        private int _growthStage;
        private float _growthTimer;
        private float _waterTimer;

        private int _x;
        private int _y;

        public FarmTile(Texture2D texture) : base(texture)
        {
            _growthStage = -1;
            _growthTimer = 0;
            _waterTimer = 0;
        }

        public override void LeftClicked(int hotbarSlot)
        {
            if (hotbarSlot == ItemSlots.WaterBasin)
            {
                _waterTimer = MAX_WATER;
                Main.SFXManager.PlaySound("WaterPour", 0.15f);
            }
            if (hotbarSlot == ItemSlots.Seeds)
            {
                if (_growthStage >= 0) return;

                if (Main.Seeds > 0 )
                {
                    _growthStage = 0;
                    Main.Seeds--;
                    Main.SFXManager.PlaySound("PlantSeed", 0.15f);
                }
            }
        }

        public override void WalkedOver()
        {
            if (_growthStage == 3)
            {
                _growthStage = -1;
                Main.Seeds++;
                int amount = Main.GameRandom.Next(1, 3);
                Main.Food += amount;
                Main.stats.PlantsHarvested++;
                Main.SFXManager.PlaySound("Plant_Gather", 0.15f);
                Main.Instance.entities.Add(new GameContent.Entities.Other.XGain(Main.SmallFoodTexture, new Vector2(_x * 32 + 16, _y * 32 + 16), amount));
            }
        }

        private void UpdateSource()
        {
            int x = (_growthStage + 1) * 32;
            int y = _waterTimer > 0f ? 32 : 0;
            Source = new Rectangle(x, y, 32, 32);
        }

        public override void Update(TimeManager time)
        {
            _waterTimer -= time.DeltaTime;
            if (_growthStage >= 0 && _growthStage < 3)
            {
                if (_waterTimer > 0f)
                {
                    _growthTimer += time.DeltaTime;
                    if (_growthTimer > 10f)
                    {
                        _growthTimer -= 10f;
                        _growthStage++;
                    }
                }
            }
        }

        public override void Draw(int x, int y, ExtendedSpriteBatch spriteBatch)
        {
            UpdateSource();

            _x = x;
            _y = y;

            base.Draw(x, y, spriteBatch);

            int width = (int)((_waterTimer / MAX_WATER) * 32);
            spriteBatch.Draw(spriteBatch.Pixel, new Rectangle(x * 32, y * 32 + 30, width, 2), new Color(102, 174, 255));

            Vector2 mouse = Main.Instance.MouseInWorld();
            if (new Rectangle(x * 32, y * 32, 32, 32).Contains(mouse))
            {
                spriteBatch.Draw(Main.TileHoverTexture, new Rectangle(x * 32, y * 32, 32, 32), Color.White * 0.5f);
            }
        }
    }
}
