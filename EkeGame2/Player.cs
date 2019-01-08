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

        public Player(ContentManager c, string objektName, int updateDelay, Vector2 spawnPosition) : base(c,objektName,updateDelay,spawnPosition)
        {
            Health = 3;
            LoadAnimation(c,objektName);
            PlayerProjectile = new Projectile(c, "Fireball", 15, Vector2.Zero, ProjectileOwner.PLAYER);
        }
        public void SetSpawnPosition(Vector2 newSpawn)
        {
            SpawnPoint = newSpawn;
        }
        public override void Respawn()
        {
            GameObjectState = GameObject_State.Air;
            PreviousAnimation = Animation_State.falling;
            ActiveAnimation = Animation_State.falling;
            Position = SpawnPoint;
            Active = true;
            Health = 3;
            Velocity = Vector2.Zero;
        }

        public void Movement(bool a, bool d, bool w, bool r, bool h, bool s)
        {
            if (r)
                this.Respawn();
            if (GameObjectState != GameObject_State.Death)
            {
                if (w && GameObjectState == GameObject_State.onGround)
                    Jump();


                if (h)
                { 
                    if(w)
                        ShootProjectile(Projectile_Trajectory.HIGH);
                    else if(s)
                        ShootProjectile(Projectile_Trajectory.LOW);
                    else
                        ShootProjectile();
                }

                if (a && Velocity.X > -10)
                {
                    Velocity.X -= 0.5f;
                }
                else if (d && Velocity.X < 10)
                {
                    Velocity.X += 0.5f; ;
                }
                else
                {
                    if (Velocity.X < 0.5 && Velocity.X > -0.5)
                        Velocity.X = 0;
                    else if (Velocity.X <= -0.5)
                    {
                        Velocity.X += 0.5f;
                    }
                    else if (Velocity.X >= 0.5)
                    {
                        Velocity.X -= 0.5f; ;
                    }
                }
            }
        }
        
        public override void Update(Level lvl, GameTime gt)
        {
            base.Update(lvl, gt);
            if (PlayerProjectile.Active)
                PlayerProjectile.Update(lvl, gt,Position);
        }
        public void DrawPlayer(SpriteBatch s, GameTime gt)
        {            
            DrawGameObject(s);
            if (PlayerProjectile.Active)
                PlayerProjectile.DrawGameObject(s, gt);
        }
        
        
        public override void DrawHitbox(SpriteBatch s)
        {
            if (PlayerProjectile.Active)
                PlayerProjectile.DrawHitbox(s);
            base.DrawHitbox(s);
        }
        private void ShootProjectile(Projectile_Trajectory pt = Projectile_Trajectory.MEDIUM)
        {
            if (!PlayerProjectile.Active)
            {
                PlayerProjectile.Shoot(Position, GoingRight,pt);
                ChangeAnimationState(Animation_State.throwing);
                LockAnimation(Animations[ActiveAnimation]);
            }
        }
    }
}
