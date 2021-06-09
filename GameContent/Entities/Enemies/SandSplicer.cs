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
    public class SandSplicer : Enemy
    {
        public static readonly List<Rectangle> EXTRA_WALLS = new List<Rectangle>()
        {
            new Rectangle(20 * 32, 4 * 32, 16, 128),
            new Rectangle(39 * 32 + 16, 4 * 32, 16, 128)
        };

        private const float MAX_SPEED = 128f;
        private const float ACCELERATION = 300f;
        private const float ROTATION_ACCELERATION = 10f;
        private const float MAX_ROTATION_SPEED = MathHelper.PiOver4;

        private int _frame;
        private float _frameCounter;
        private int _currentPhase;
        private int _prevPhase;
        private float _phaseTimer;
        private float _rotationSpeed;
        private bool _rotateLeft;
        private bool _wander;
        private Animals.Animal _target;
        private float _shootTimer;
        private float _sfxTimer;

        public SandSplicer(Vector2 pos) : base(pos, new Vector2(56, 18), 10 * (1f + (Main.RentsPaid + 1) * 0.2f))
        {
            _currentPhase = 0;
            _frame = -1;
        }

        private void PickNewAttackPhase()
        {
            SetPhase(Main.GameRandom.Next(3));
        }

        private void SetPhase(int phase)
        {
            _prevPhase = _currentPhase;
            _currentPhase = phase;
            _phaseTimer = 0;
            switch(phase)
            {
                default:
                    break;
                case 2:
                    List<Animals.Animal> choosable = new List<Animals.Animal>();
                    foreach(Animals.Animal a in Main.Instance.animals)
                    {
                        if (Main.ANIMAL_PEN.Contains(a.Body.Center) && a.Health > a.MaxHealth * 0.5f)
                        {
                            choosable.Add(a);
                        }
                    }
                    if (choosable.Count == 0)
                    {
                        SetPhase(0);
                        break;
                    }
                    _target = choosable[Main.GameRandom.Next(choosable.Count)];
                    break;
                case 4:
                    _rotationSpeed = 0;
                    _frame = 10;
                    _frameCounter = 0;
                    _wander = false;
                    break;
                case 5:
                case 6:
                    _wander = false;
                    _frame = 6;
                    _frameCounter = 0;
                    break;
            }
        }

        public override void OnHorizontalCollide()
        {
            Velocity.X *= -1f;
        }

        public override void OnVerticalCollide()
        {
            Velocity.Y *= -1f;
        }

        public override void PreUpdate(TimeManager time)
        {
            Main.Intense = true;

            if (_currentPhase == -1)
            {
                PickNewAttackPhase();
            }

            _phaseTimer += time.DeltaTime;

            if (_currentPhase == 0) //swim randomly ("wander")
            {
                if (_phaseTimer < 3f)
                {
                    _frame = -1;

                    if (!_wander)
                    {
                        Velocity = Main.GameRandom.NextFloat(-MathHelper.Pi, MathHelper.Pi).ToVector2() * 0.5f;
                        _rotateLeft = Main.GameRandom.Next(2) == 0;
                        _wander = true;
                    }
                    else
                    {
                        _rotationSpeed += _rotateLeft ? -ROTATION_ACCELERATION * time.DeltaTime : ROTATION_ACCELERATION * time.DeltaTime;
                        if (_rotationSpeed < -MAX_ROTATION_SPEED) _rotationSpeed = -MAX_ROTATION_SPEED;
                        if (_rotationSpeed > MAX_ROTATION_SPEED) _rotationSpeed = MAX_ROTATION_SPEED;

                        float speed = Velocity.Length();
                        float rotation = Velocity.ToAngle();
                        rotation += _rotationSpeed * time.DeltaTime;
                        speed += ACCELERATION * time.DeltaTime;
                        if (speed > MAX_SPEED)
                        {
                            speed = MAX_SPEED;
                        }
                        Velocity = rotation.ToVector2() * speed;
                    }

                    if (Main.GameRandom.Next(80) == 0)
                    {
                        _rotateLeft = !_rotateLeft;
                    }
                }
                else
                {
                    _wander = false;
                    SetPhase(5);
                }
            }
            else if (_currentPhase == 1) //chase player
            {
                if (_phaseTimer < 3f)
                {
                    SmoothMove(time, 256f, Main.Instance.player.Body.Center);
                }
                else
                {
                    SetPhase(6);
                }
            }
            else if (_currentPhase == 2) //chase animal
            {
                if (_phaseTimer < 5f)
                {
                    if (_target == null)
                    {
                        SetPhase(0);
                    }
                    else
                    {
                        SmoothMove(time, MAX_SPEED, _target.Body.Center);

                        if (Vector2.Distance(Body.Center, _target.Body.Center) < 4f)
                        {
                            Velocity *= 0.8f;
                            _phaseTimer = 5f;
                        }
                    }
                }
                else
                {
                    Velocity *= 0.85f;
                    if (Velocity.Length() < 4f)
                    {
                        Velocity = Vector2.Zero;
                    }
                    if (_phaseTimer > 7.5f)
                    {
                        SetPhase(6);
                    }
                }
            }

            //going back underground
            if (_currentPhase == 4)
            {
                _frameCounter += time.DeltaTime;
                if (_frameCounter > 0.07f)
                {
                    _frameCounter -= 0.07f;
                    _frame--;
                    if (_frame == 5)
                    {
                        _frame = -1;
                        _currentPhase = -1;
                    }
                }
            }

            //popping up then attacking
            if (_currentPhase == 5 || _currentPhase == 6)
            {
                Velocity *= 0.85f;
                if (Velocity.Length() < 4f)
                {
                    Velocity = Vector2.Zero;
                }

                _frameCounter += time.DeltaTime;
                if (_frameCounter > 0.07f)
                {
                    _frameCounter -= 0.07f;
                    _frame++;
                    if (_frame >= 10)
                    {
                        _frame = 0;
                        if (_currentPhase == 5) SetPhase(7);
                        if (_currentPhase == 6) SetPhase(8);
                    }
                }
            }

            if (_currentPhase == 7 || _currentPhase == 8 || _currentPhase == 9)
            {
                Velocity *= 0.85f;
                if (Velocity.Length() < 4f)
                {
                    Velocity = Vector2.Zero;
                }

                _frameCounter += time.DeltaTime;
                if (_frameCounter > 0.07f)
                {
                    _frameCounter -= 0.07f;
                    _frame++;
                    if (_frame > 5)
                    {
                        _frame = 0;
                    }
                }
            }

            if (_currentPhase == 7)
            {
                //shoot projectiles
                Vector2 spawnPos = Body.Center - new Vector2(0, 20);
                _shootTimer += time.DeltaTime;
                if (_shootTimer > 0.5f)
                {
                    _shootTimer = 0f;
                    Main.SpawnProjectile(new Projectiles.RedBlot(spawnPos, new Vector2(300, 0), new Vector2(8)));
                    Main.SpawnProjectile(new Projectiles.RedBlot(spawnPos, new Vector2(-300, 0), new Vector2(8)));
                    Main.SpawnProjectile(new Projectiles.RedBlot(spawnPos, new Vector2(0, 300), new Vector2(8)));
                    Main.SpawnProjectile(new Projectiles.RedBlot(spawnPos, new Vector2(0, -300), new Vector2(8)));
                }
                if (_phaseTimer > 4f)
                {
                    SetPhase(4);
                }
            }

            if (_currentPhase == 8)
            {
                Main.SpawnProjectile(new Projectiles.FakeHitBox(25f, 1f, Body.Center, Vector2.Zero, new Vector2(50, 16)));
                _currentPhase = 9;

                Main.SFXManager.PlaySound("Sand_Splicer_0", 0.15f);
            }

            if (_currentPhase == 9)
            {
                if (_phaseTimer > 4f)
                {
                    SetPhase(4);
                }
            }

            if (_frame == -1)
            {
                Main.Instance.entities.Add(new Entities.Other.BigFootstep(Body.Center + new Vector2(Main.GameRandom.NextFloat(-5f, 5f), Main.GameRandom.NextFloat(-5, 5f))));

                _sfxTimer += time.DeltaTime;
                if (_sfxTimer > 0.18f)
                {
                    Main.SFXManager.PlaySound("Sand_Splicer_Dig", 0.07f, 0f, Main.GameRandom.NextFloat(-0.2f, 0.05f));
                    _sfxTimer = 0f;
                }
            }

            base.PreUpdate(time);
        }

        public override bool Hittable()
        {
            return _frame >= 0 && _frame != 6;
        }

        public override void Kill()
        {
            base.Kill();

            Main.stats.SplicersKilled++;

            Main.SFXManager.PlaySound("Sand_Splicer_Death", 0.05f);

            Main.Coins += 80;
            Main.stats.MoneyEarnt += 80;
            //Main.Food += 1;

            Main.Instance.entities.Add(new Entities.Other.XGain(Main.SmallCoinTexture, Body.Center - new Vector2(0, 12), 50));
        }

        private void SmoothMove(TimeManager time, float max, Vector2 target)
        {
            Vector2 toDest = target - Body.Center;
            float dist = toDest.Length();
            toDest.Normalize();

            float slowDownDist = (-(max * max)) / (2f * -ACCELERATION);

            float maxSpeed = max;
            if (dist < slowDownDist)
            {
                maxSpeed *= (dist / slowDownDist);
                if (maxSpeed == float.NaN || maxSpeed < 0.1f)
                {
                    maxSpeed = 0.1f;
                }
            }

            Velocity += toDest * ACCELERATION * time.DeltaTime;

            if (Velocity.Length() > maxSpeed)
            {
                Velocity.Normalize();
                Velocity *= maxSpeed;
            }

            if (dist < 1f)
            {
                Body.Center = target;
                Velocity = Vector2.Zero;
            }
        }

        public override void Update(TimeManager time, PhysicsManager physics)
        {
            PreUpdate(time);

            physics.UpdateMe(this, EXTRA_WALLS);

            PostUpdate(time);
        }

        public override Rectangle GetDrawBody()
        {
            Rectangle body = Body.ToRectangle();
            return new Rectangle(body.X - 4, body.Top - 39, 62, 63);
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            if (_frame == -1) return;

            Rectangle source = new Rectangle(0, _frame * 63, 62, 63);
            spriteBatch.Draw(Main.SandSplicerTexture, new Vector2(Body.TopLeft.X - 4, Body.TopLeft.Y - 39), source, HitColor);

            base.Draw(spriteBatch);
        }
    }
}
