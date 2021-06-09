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
using MiniJam61Egypt.Util;

namespace MiniJam61Egypt.GameContent.Entities
{
    public abstract class Entity
    {
        public virtual bool Solid => throw new NotImplementedException();
        public virtual bool Static => false;
        public virtual bool HitsWalls => true;
        public virtual bool Destroy { get; protected set; }
        public virtual bool CanHover => false;
        public virtual bool ConsumeMouse => false;

        public float Health;
        public float MaxHealth;
        public Color HitColor;

        public Vector2 Velocity;
        public BoundingBox2D Body;

        public Entity()
        {
            HitColor = Color.White;
        }

        public virtual Rectangle GetDrawBody()
        {
            return Body.ToRectangle();
        }

        public virtual void Update(TimeManager time, PhysicsManager physics)
        {
            PreUpdate(time);

            physics.UpdateMe(this);

            PostUpdate(time);
        }

        public virtual void Damage(float health)
        {
            Health -= health;
            if (Health <= 0f)
            {
                Kill();
            }

            if (health >= 1f)
            {
                Main.SFXManager.PlaySound("Splat", 0.04f, 0f, Main.GameRandom.NextFloat(-0.3f, 0.3f));
            }

            Rectangle space = GetDrawBody();
            int blood = (int)(Main.GameRandom.Next(15, 22) * Math.Min(1f, (health / 4f)));
            for (int i = 0; i < blood; i++)
            {
                Main.SummonParticlesColorRamp(Main.BloodTexture, 1, new Vector2(space.X + Main.GameRandom.NextFloat(space.Width), space.Y + Main.GameRandom.NextFloat(space.Height)), Main.GameRandom.NextFloat(-MathHelper.Pi, MathHelper.Pi).ToVector2() * Main.GameRandom.NextFloat(5f, 20f), new Vector2(0, 32), Color.White, new Color(170, 170, 240), Main.GameRandom.NextFloat(-3f, 3f), 0.5f, 0.8f, -MathHelper.Pi, MathHelper.Pi, 0.7f, 1.2f, 0, true);
            }
            HitColor = new Color(255, 155, 155);
        }

        public virtual bool InContactWith(Entity e)
        {
            return ((Body.Top < e.Body.Top && Body.Right > e.Body.Left && Body.Left < e.Body.Right) && Body.Bottom + 0.2f >= e.Body.Y) || Body.Intersects(e.Body);
        }

        public virtual void PreUpdate(TimeManager time) { }
        public virtual void PostUpdate(TimeManager time)
        {
            if (HitColor.G < 255) HitColor.G += 10;
            if (HitColor.B < 255) HitColor.B += 10;
        }
        public virtual void OnHorizontalCollide()
        {
            Velocity.X = 0f;
        }
        public virtual void OnVerticalCollide() 
        { 
            Velocity.Y = 0f;
        }
        public virtual void OnSquish() { }
        public virtual void Draw(ExtendedSpriteBatch spriteBatch) 
        {
            Rectangle rect = Body.ToRectangle();
            int extra = 4;
            int width = rect.Width + extra;
            //spriteBatch.Draw(Main.EntityShadow, new Rectangle(rect.X - cameraPosition.X - extra, rect.Y - cameraPosition.Y + 10, width, 10), null, Color.White * 0.45f, 0f, Vector2.Zero, SpriteEffects.None, 0f);
        }
        public virtual float GetOrderHeight()
        {
            return Body.Bottom;
        }
        public virtual void Kill()
        {
            Destroy = true;
        }
        public virtual bool MouseOver()
        {
            return false;
        }
        public virtual void RightClicked()
        {
        }
        public virtual void LeftClicked()
        {
        }

        public virtual void DrawHover(ExtendedSpriteBatch spriteBatch) { }

        public override bool Equals(object obj)
        {
            if (obj is Entity e)
            {
                return e.Body == Body && e.Velocity.Equals(Velocity);
            }

            return false;
        }
    }
}
