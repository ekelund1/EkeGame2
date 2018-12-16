using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace EkeGame2
{
    public sealed class Player : AbstractGameObject
    {
        Projectile playerProjectile;
        bool projectileActive;

        public Player(ContentManager c, string objektName, int updateDelay, Vector2 spawnPosition) : base(c,objektName,updateDelay,spawnPosition)
        {
            playerProjectile = new Projectile(c, "Hammer", 15, Position);
            
            
        }
        public override void Update(Level lvl, GameTime gt)
        {
            base.Update(lvl, gt);
            playerProjectile.Update(lvl, gt);
        }
        public void Movement(bool a, bool d, bool w, bool r, bool h)
        {
            if (w && GameObjectState == GameObject_State.onGround)
                Jump();
            if (r)
                this.Respawn();

            if (h)
                this.Shoot();

            if (a && Velocity.X > -10)
            {
                Velocity.X--;
            }
            else if (d && Velocity.X < 10)
            {
                Velocity.X++;
            }
            else
            {
                if (Velocity.X < 0)
                {
                    Velocity.X++;
                }
                else if (Velocity.X > 0)
                {
                    Velocity.X--;
                }
            }
        }
        public void DrawPlayer(SpriteBatch s, GameTime gt)
        {
            playerProjectile.DrawProjectile(s,gt);
            this.DrawGameObject(s);
        }
        protected override void Gravity()
        {
            if (Velocity.Y < 20 && (GameObjectState == GameObject_State.Air))
                Velocity.Y = (Velocity.Y + 1);
        }
        private void Shoot()
        {
            if (!playerProjectile.active)
            {
                playerProjectile.Shoot(Position, Velocity,goingRight);
                projectileActive=true;
            }
        }
        public override void DrawHitbox(SpriteBatch s)
        {
            playerProjectile.DrawHitbox(s);
            base.DrawHitbox(s);
        }
    }
}
