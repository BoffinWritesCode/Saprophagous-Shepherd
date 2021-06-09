using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using BoffXNA.Base;
using BoffXNA.Base.Input;
using BoffXNA.Graphics;
using BoffXNA.Maths;

using MiniJam61Egypt.Util;
using MiniJam61Egypt.Environment;

namespace MiniJam61Egypt.GameContent.Entities
{
    public class Player : Entity
    {
        public override bool Solid => false;
        public const float MaxSpeed = 300f;

        private InputManager _input;

        private int _frame;
        private float _frameCounter;
        private int _facing;
        private float _moveCounter;

        public bool IsAlive;

        public Player(InputManager input)
        {
            _input = input;
            Body = new BoundingBox2D(0, 0, 29, 31);
            IsAlive = true;
        }

        public override void PreUpdate(TimeManager time)
        {
            if (!IsAlive)
            {
                Velocity = Vector2.Zero;
                return;
            }

            float deltaTime = time.DeltaTime;

            //float accel = 18f * deltaTime;
            float accel = 100f;
            float oppositeWayMult = 5f;
            float slowDown = 0.64f;

            bool keyPressed = false;
            bool keyPressedV = false;
            bool keyPressedH = false;

            if (_input.KeyDown(Keys.W))
            {
                keyPressed = true;
                _facing = 3;
                keyPressedV = true;
                if (Velocity.Y > 0f)
                {
                    accel *= oppositeWayMult;
                }
                Velocity.Y -= accel;
                if (Velocity.Y < -MaxSpeed)
                {
                    Velocity.Y = -MaxSpeed;
                }
            }
            if (_input.KeyDown(Keys.S))
            {
                keyPressed = true;
                _facing = 0;
                keyPressedV = true;
                if (Velocity.Y < 0f)
                {
                    accel *= oppositeWayMult;
                }
                Velocity.Y += accel;
                if (Velocity.Y > MaxSpeed)
                {
                    Velocity.Y = MaxSpeed;
                }
            }

            if (_input.KeyDown(Keys.A))
            {
                keyPressed = true;
                _facing = 2;
                keyPressedH = true;
                if (Velocity.X > 0f)
                {
                    accel *= oppositeWayMult;
                }
                Velocity.X -= accel;
                if (Velocity.X < -MaxSpeed)
                {
                    Velocity.X = -MaxSpeed;
                }
            }
            if (_input.KeyDown(Keys.D))
            {
                keyPressed = true;
                _facing = 1;
                keyPressedH = true;
                if (Velocity.X < 0f)
                {
                    accel *= oppositeWayMult;
                }
                Velocity.X += accel;
                if (Velocity.X > MaxSpeed)
                {
                    Velocity.X = MaxSpeed;
                }
            }

            if (!keyPressedH)
            {
                Velocity.X *= slowDown;
                if (Math.Abs(Velocity.X) < 0.1f) Velocity.X = 0f;
            }

            if (!keyPressedV)
            {
                Velocity.Y *= slowDown;
                if (Math.Abs(Velocity.Y) < 0.1f) Velocity.Y = 0f;
            }

            if (Velocity.Length() > MaxSpeed)
            {
                Velocity.Normalize();
                Velocity *= MaxSpeed;
            }

            if (!keyPressed)
            {
                _frame = 0;
            }
            else
            {
                _moveCounter += deltaTime;
                if (_moveCounter > 0.18f)
                {
                    Vector2 playerWorld = Main.Instance.player.Body.Center;
                    Point tile = (playerWorld / 32f).ToPoint();
                    if (tile.X >= 0 && tile.X < Main.Instance.world.Width && tile.Y >= 0 && tile.Y < Main.Instance.world.Height)
                    {
                        if (Main.Instance.world[tile.X, tile.Y].HasFootsteps)
                        {
                            Main.Instance.entities.Add(new Other.Footstep(new Vector2(Body.Center.X + Main.GameRandom.Next(-2, 3), Body.Bottom - 4f + Main.GameRandom.Next(-2, 3))));
                            _moveCounter = 0f;
                        }
                    }
                }
                float frameTimer = 0.09f;
                _frameCounter += deltaTime;
                if (_frameCounter > frameTimer)
                {
                    _frameCounter -= frameTimer;
                    _frame++;
                    if (_frame > 3)
                        _frame = 0;
                }
            }
        }

        public override Rectangle GetDrawBody()
        {
            Rectangle body = Body.ToRectangle();
            return new Rectangle(body.Center.X - 14, body.Bottom - 42, 29, 42);
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            Rectangle body = Body.ToRectangle();

            foreach (Animals.Animal a in Main.Instance.animals)
            {
                if (a.Following)
                {
                    Vector2 myCenter = (Body.Center - Vector2.UnitY * 16f);
                    Vector2 between = myCenter - Body.Center;
                    BezierCurve curve = new BezierCurve(myCenter, myCenter + between * 0.5f + Vector2.UnitY * (between.Length() * 2f), a.Body.Center);
                    List<Vector2> points = curve.GetPoints(8);
                    for (int i = 0; i < points.Count - 1; i++)
                    {
                        Vector2 start = points[i];
                        Vector2 end = points[i + 1];
                        spriteBatch.DrawLine(start, end, new Color(196, 151, 109), 1f);
                    }
                }
            }

            spriteBatch.Draw(Main.PlayerTexture, new Rectangle(body.Center.X - 14, body.Bottom - 42, 29, 42), new Rectangle(_facing * 29, 42 * _frame, 29, 42), HitColor, 0f, Vector2.Zero, SpriteEffects.None, 0f);
        }
    }
}
