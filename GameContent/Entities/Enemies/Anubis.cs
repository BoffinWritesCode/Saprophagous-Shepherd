using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using BoffXNA;
using BoffXNA.Base;
using BoffXNA.Graphics;

using MiniJam61Egypt.Physics;

namespace MiniJam61Egypt.GameContent.Entities.Enemies
{
    public class Anubis : Enemy
    {
        public Anubis(Vector2 pos) : base(pos, new Vector2(52, 52), 100 * (1f + (Main.RentsPaid + 1) * 0.2f))
        {
            _phase = -1;
        }

        public bool _flippedLeft;

        public float _teleportProgress;
        private bool _doneTP;
        private bool _spawnedObs;

        private int _phase;
        private float _phaseCounter;

        private int _frame;
        private float _frameCounter;

        private Animals.Animal _target;

        private void SetPhase(int p)
        {
            _target = null;
            _phase = p;
            _phaseCounter = 0f;
            _spawnedObs = false;
            _doneTP = false;
            _teleportProgress = 0;

            if (_phase == 2)
            {
                float furthest = 0;
                foreach(Animals.Animal an in Main.Instance.animals)
                {
                    float d = Vector2.Distance(Body.Center, an.Body.Center);
                    if (!an.Following && d > furthest)
                    {
                        furthest = d;
                        _target = an;
                    }
                }
                if (_target != null)
                {
                    _target.OutOfControl = true;
                }

                Main.SFXManager.PlaySound("Anubis_1", 0.15f);
            }
        }

        private void TryFireObelisks()
        {
            foreach (Enemy e in Main.Instance.enemies)
            {
                if (e is Obelisk ob)
                {
                    Main.SpawnProjectile(new Projectiles.RedBlot(e.Body.Center, Vector2.Normalize(Body.Center - e.Body.Center) * 300f, new Vector2(8)));
                }
            }
        }

        public override void PreUpdate(TimeManager time)
        {
            Main.Intense = true;

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

            _phaseCounter += time.DeltaTime;

            if (_phase == -1)
            {
                _teleportProgress += time.DeltaTime;
                if (!_doneTP)
                {
                    if (_teleportProgress > 0.5f)
                    {
                        _doneTP = true;

                        PuffSmoke();
                        Body.Center = new Vector2(Main.GameRandom.Next(Main.ANIMAL_PEN.X, Main.ANIMAL_PEN.Right), Main.GameRandom.Next(Main.ANIMAL_PEN.Y, Main.ANIMAL_PEN.Bottom));
                        _flippedLeft = Main.GameRandom.Next(2) == 0;
                        TryFireObelisks();
                        PuffSmoke();
                    }
                }
                else if (_teleportProgress >= 1f)
                {
                    //new attack
                    SetPhase(Main.GameRandom.Next(3));
                }
            }

            if (_phase == 0)
            {
                _teleportProgress += time.DeltaTime;
                if (_teleportProgress > 1f)
                {
                    _teleportProgress = 1f;
                }
                if (!_doneTP)
                {
                    if (_teleportProgress > 0.5f)
                    {
                        _doneTP = true;

                        PuffSmoke();
                        Body.Center = Main.ANIMAL_PEN.Center();
                        _flippedLeft = Main.GameRandom.Next(2) == 0;
                        TryFireObelisks();
                        PuffSmoke();

                        Main.SpawnProjectile(new Projectiles.AnubisSun(Body.Center - Vector2.UnitY * 20f));

                        Main.SFXManager.PlaySound("Anubis_2", 0.15f);

                        _phaseCounter = 0f;
                    }
                }
                else if (_phaseCounter > 7f)
                {
                    SetPhase(-1);
                }
            }

            if (_phase == 1)
            {
                if (!_spawnedObs)
                {
                    _spawnedObs = true;
                    Main.SpawnEnemy(new Obelisk(new Vector2(Main.ANIMAL_PEN.X + 32, Main.ANIMAL_PEN.Y + 32)));
                    Main.SpawnEnemy(new Obelisk(new Vector2(Main.ANIMAL_PEN.Right - 32, Main.ANIMAL_PEN.Y + 32)));
                    Main.SpawnEnemy(new Obelisk(new Vector2(Main.ANIMAL_PEN.X + 32, Main.ANIMAL_PEN.Bottom - 32)));
                    Main.SpawnEnemy(new Obelisk(new Vector2(Main.ANIMAL_PEN.Right - 32, Main.ANIMAL_PEN.Bottom - 32)));

                    Main.SFXManager.PlaySound("Anubis_0", 0.15f);
                }

                if (_phaseCounter > 4f)
                {
                    SetPhase(-1);
                }
            }

            if (_phase == 2)
            {
                if (_target == null || _target.Health <= 0f || _target.Following)
                {
                    if (_target != null)
                    {
                        _target.Velocity = Vector2.Zero;
                    }
                    SetPhase(-1);
                }
                else
                {
                    _target.Velocity = Vector2.Normalize(Body.Center - _target.Body.Center) * 110f;

                    Vector2 between = _target.Body.Center - Body.Center;

                    int particles = (int)(between.Length() / 4f);

                    Vector2 pos = Body.Center;
                    for (int i = 0; i < particles; i++)
                    {
                        if (Main.GameRandom.Next(7) != 0)
                        {
                            pos += Vector2.Normalize(between) * 4f;
                            continue;
                        }

                        Main.SummonParticlesColorRamp(Main.BloodTexture, 1, new Vector2(pos.X + Main.GameRandom.NextFloat(-3f, 3f), pos.Y + Main.GameRandom.NextFloat(-3f, 3f)), Main.GameRandom.NextFloat(-MathHelper.Pi, MathHelper.Pi).ToVector2() * Main.GameRandom.NextFloat(5f, 20f), new Vector2(0, 32), Color.White, new Color(170, 170, 240), Main.GameRandom.NextFloat(-3f, 3f), 0.5f, 0.8f, -MathHelper.Pi, MathHelper.Pi, 0.7f, 1.2f, 0, true);

                        pos += Vector2.Normalize(between) * 4f;
                    }

                    if (between.Length() < 10f)
                    {
                        _target.Kill();

                        _target = null;

                        float angle = -MathHelper.Pi;
                        for (int i = 0; i < 16; i++)
                        {
                            Vector2 direction = angle.ToVector2();

                            Main.SpawnProjectile(new Projectiles.RedBlot(Body.Center, direction * 200f, new Vector2(4)));

                            angle += MathHelper.PiOver4 * 0.5f;
                        }
                    }
                }
            }

            base.PreUpdate(time);
        }

        private void PuffSmoke()
        {
            Rectangle space = GetDrawBody();
            space.Inflate(-10, -10);
            for (int i = 0; i < Main.GameRandom.Next(15, 24); i++)
            {
                Main.SummonParticlesColorRamp(Main.SmokeTexture, 1, new Vector2(space.X + Main.GameRandom.NextFloat(space.Width), space.Y + Main.GameRandom.NextFloat(space.Height)), Main.GameRandom.NextFloat(MathHelper.TwoPi).ToVector2() * Main.GameRandom.NextFloat(0f, 20f), Vector2.Zero, new Color(160, 160, 160), Color.White, Main.GameRandom.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4), 0.2f, 0.5f, 0, MathHelper.TwoPi, 0.6f, 1f, 10f, true);
            }
        }

        public override void Kill()
        {
            base.Kill();

            Main.SFXManager.PlaySound("Anubis_Die", 0.15f);

            if (_target != null)
            {
                _target.OutOfControl = false;
            }

            Main.stats.AnubisKilled++;

            Main.Coins += 500;
            Main.stats.MoneyEarnt += 500;
            //Main.Food += 1;

            Main.Instance.entities.Add(new Entities.Other.XGain(Main.SmallCoinTexture, Body.Center - new Vector2(0, 12), 500));
        }

        public override Rectangle GetDrawBody()
        {
            Rectangle body = Body.ToRectangle();
            return new Rectangle(body.X - 15, body.Top - 29, 84, 88);
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            Color c = HitColor;

            if (_phase == 0)
            {
                if (_teleportProgress < 0.5f)
                {
                    c = Color.Lerp(HitColor, Color.White * 0f, _teleportProgress / 0.5f);
                }
                else
                {
                    c = Color.Lerp(Color.White * 0f, HitColor, (_teleportProgress - 0.5f) / 0.5f);
                }
            }

            spriteBatch.Draw(Main.AnubisTexture, new Vector2(Body.TopLeft.X - 15, Body.TopLeft.Y - 29), new Rectangle(0, 88 * _frame, 84, 88), c, 0f, Vector2.Zero, 1f, _flippedLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

            base.Draw(spriteBatch);
        }
    }
}
