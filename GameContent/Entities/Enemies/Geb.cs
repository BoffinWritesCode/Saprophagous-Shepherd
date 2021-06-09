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
    public class Geb : Enemy
    {
        private static readonly Vector2[] TRIANGLE_POINTS = new Vector2[]
        {
            new Vector2(50, 0),
            new Vector2(16.6666f, 16.6666f),
            new Vector2(16.6666f, -16.6666f),
            new Vector2(-16.6666f, -33.3333f),
            new Vector2(-16.6666f, 33.3333f),
            new Vector2(-50, 50),
            new Vector2(-50, 16.6666f),
            new Vector2(-50, -16.6666f),
            new Vector2(-50, -50),
        };

        public Geb(Vector2 pos) : base(pos, new Vector2(82, 58), 40 * (1f + (Main.RentsPaid + 1) * 0.2f))
        {
            SetPhase(Main.GameRandom.Next(3));
        }

        private float _frameCounter;
        private int _frame;

        private float _phaseCounter;
        private int _phase;
        
        private int _trisSpawned;
        private float _rotation;

        private bool _markSpawned;
        
        private void SetPhase(int p)
        {
            _phase = p;
            _phaseCounter = 0;
            switch (_phase)
            {
                case 0:
                    _rotation = (Main.Instance.player.Body.Center - Body.Center).ToAngle();
                    _trisSpawned = 0;
                    break;
                case 1:
                case 2:
                    _markSpawned = false;
                    break;
            }
        }

        public override void PreUpdate(TimeManager time)
        {
            Main.Intense = true;

            _phaseCounter += time.DeltaTime;

            if (_phase == -1)
            {
                if (_phaseCounter > 1.5f)
                {
                    SetPhase(Main.GameRandom.Next(3));
                }
            }
            if (_phase == -2)
            {
                if (_phaseCounter > 6f)
                {
                    SetPhase(Main.GameRandom.Next(3));
                }
            }
            if (_phase == 0)
            {
                if (_phaseCounter > 0.1f)
                {
                    if (_trisSpawned < 9)
                    {
                        _phaseCounter -= 0.1f;
                        Vector2 point = TRIANGLE_POINTS[_trisSpawned];
                        float speed = point.Length();
                        float rot = point.ToAngle() + _rotation;
                        Vector2 newPoint = rot.ToVector2() * speed;
                        Main.SpawnProjectile(new Projectiles.Triangle(2f + (9 - _trisSpawned) * 0.1f, Body.Center + newPoint, _rotation.ToVector2() * 400f, new Vector2(8)));
                        _trisSpawned++;
                    }
                    else
                    {
                        Main.SFXManager.PlaySound("Geb_0", 0.05f);
                        SetPhase(-2);
                        _trisSpawned = 0;
                        _rotation = 0f;
                    }
                }
            }
            if (_phase == 1)
            {
                if (!_markSpawned)
                {
                    List<Entity> valid = new List<Entity>();
                    valid.Add(Main.Instance.player);
                    foreach(Entity e in Main.Instance.animals)
                    {
                        valid.Add(e);
                    }

                    Entity choice = valid[Main.GameRandom.Next(valid.Count)];

                    Main.SpawnProjectile(new Projectiles.Mark(new Vector2(choice.Body.Center.X, choice.Body.Bottom - 8), new Vector2(24, 8)));

                    _markSpawned = true;
                    Main.SFXManager.PlaySound("Geb_1", 0.05f);
                }

                if (_phaseCounter > 6f)
                {
                    SetPhase(-1);
                }
            }
            if (_phase == 2)
            {
                if (!_markSpawned)
                {
                    Vector2 player = Main.Instance.player.Body.Center;
                    Vector2 position = player;
                    do
                    {
                        position = new Vector2(Main.GameRandom.Next(Main.ANIMAL_PEN.X, Main.ANIMAL_PEN.Right), Main.GameRandom.Next(Main.ANIMAL_PEN.Y, Main.ANIMAL_PEN.Bottom));
                    }
                    while (Vector2.Distance(position, player) < 160f);

                    Main.SpawnProjectile(new Projectiles.Whirlpool(position));

                    _markSpawned = true;
                    Main.SFXManager.PlaySound("Geb_0", 0.05f);
                }

                if (_phaseCounter > 5f)
                {
                    SetPhase(-1);
                }
            }

            _frameCounter += time.DeltaTime;
            if (_frameCounter > 0.09f)
            {
                _frameCounter -= 0.09f;
                _frame++;
                if (_frame > 5)
                {
                    _frame = 0;
                }
            }

            base.PreUpdate(time);
        }

        public override void Kill()
        {
            base.Kill();

            Main.stats.GebKilled++;

            Main.SFXManager.PlaySound("Geb_Death", 0.08f);

            Main.Coins += 220;
            Main.stats.MoneyEarnt += 220;
            //Main.Food += 1;

            Main.Instance.entities.Add(new Entities.Other.XGain(Main.SmallCoinTexture, Body.Center - new Vector2(0, 12), 200));
        }

        public override Rectangle GetDrawBody()
        {
            Rectangle body = Body.ToRectangle();
            return new Rectangle(body.X - 8, body.Top - 42, 96, 100);
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            Rectangle source = new Rectangle(0, _frame * 100, 96, 100);
            spriteBatch.Draw(Main.PyramidTexture, new Vector2(Body.TopLeft.X - 8, Body.TopLeft.Y - 42), source, HitColor);

            base.Draw(spriteBatch);
        }
    }
}
