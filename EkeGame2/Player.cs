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
        Projectile PlayerProjectile;

        public Player(ContentManager c, string objektName, int updateDelay, Vector2 spawnPosition, ref Projectile playerProjectile) : base(c,objektName,updateDelay,spawnPosition)
        {
            Health = 1;
            LoadAnimation(objektName);
            PlayerProjectile = playerProjectile;
        }
        public override void Update(Level lvl, GameTime gt)
        {
            PlayerProjectile.Update(lvl,gt);
            base.Update(lvl, gt);
        }
        public void Movement(bool a, bool d, bool w, bool r, bool h)
        {
            if (w && GameObjectState == GameObject_State.onGround)
                Jump();
            if (r)
                this.Respawn();

            if (h)
                Shoot();

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
            DrawGameObject(s);
        }
        
        private void Shoot()
        {
            if (!PlayerProjectile.Active)
            {
                PlayerProjectile.Shoot(Position, Velocity, GoingRight);
                ChangeAnimationState(Animation_State.throwing);
                WaitForAnimation(Animations[ActiveAnimation]);
            }
        }
        
        
    }
}
