using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using MiniJam61Egypt.GameContent.Entities.Animals;

namespace MiniJam61Egypt.GameContent.Entities
{
    public class Projectile : Entity
    {
        public override bool Solid => false;
        public override bool HitsWalls => false;

        public virtual bool HitOneAtTime => true;
        public virtual bool Friendly => false;

        public float Rotation;

        public Projectile(Vector2 pos, Vector2 velocity, Vector2 size)
        {
            Body = new Util.BoundingBox2D(pos - size * 0.5f, size);
            Velocity = velocity;
        }

        public virtual void Damage(Entity e) { }

        public override void PostUpdate(BoffXNA.Base.TimeManager time)
        {
            if (Body.X < -100f || Body.Y < -100f || Body.X > 2020f || Body.Y > 780f)
            {
                Destroy = true;
            }
        }

        public override float GetOrderHeight()
        {
            return 999999f;
        }
    }
}
