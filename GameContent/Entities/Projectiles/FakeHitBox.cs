using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using BoffXNA;
using BoffXNA.Base;
using BoffXNA.Graphics;

namespace MiniJam61Egypt.GameContent.Entities.Projectiles
{
    public class FakeHitBox : Projectile
    {
        public override bool HitOneAtTime => false;
        public override bool Friendly => false;

        private float _damage;
        private float _timeAlive;

        public FakeHitBox(float damage, float timeAlive, Vector2 p, Vector2 v, Vector2 size) : base(p, v, size)
        {
            _damage = damage;
            _timeAlive = timeAlive;
        }

        public override void PreUpdate(TimeManager time)
        {
            _timeAlive -= time.DeltaTime;

            if (_timeAlive <= 0f)
            {
                Destroy = true;
            }

            base.PreUpdate(time);
        }

        public override void Damage(Entity e)
        {
            e.Damage(_damage);

            Destroy = true;
        }
    }
}
