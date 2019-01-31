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
    public class Player : UnitObject
    {
        Projectile PlayerProjectile;
        int CollectedCollectibles;

        public Player(ContentManager c, string objektName, int updateDelay, Vector2 spawnPosition) : base(c, objektName, updateDelay, spawnPosition)
        {
            Health = 3;
            LoadAnimation(c, objektName);
            CollectedCollectibles = 0;
            PlayerProjectile = new Projectile(c, "Fireball/", 15, Vector2.Zero, ProjectileOwner.PLAYER);
        }
        public void ChangeCollectibles(int amount)
        {
            CollectedCollectibles += amount;
        }
        public int CollectedAmount{ get { return CollectedCollectibles; } }
        public void SetSpawnPosition(Vector2 newSpawn)
        {
            SpawnPoint = newSpawn;
        }
        public override void Respawn()
        {
            base.Respawn();
            Health = 3;
        }

        public void Movement(GameTime gt, bool a, bool d, bool w, bool r, bool h, bool s, bool j)
        {
            if (r)
                this.Respawn();
            if (!(GameObjectState == GameObject_State.Death) || !(GameObjectState == GameObject_State.AirRoll))
            {
                if (w && GameObjectState == GameObject_State.onGround)
                    Jump(gt);


                if (h)
                { 
                    if(w)
                        ShootProjectile(Projectile_Trajectory.HIGH);
                    else if(s)
                        ShootProjectile(Projectile_Trajectory.LOW);
                    else
                        ShootProjectile();
                }
                if (j)
                    AirRoll(gt,a,s,d,w);

                if (a && Velocity.X > -10)
                {
                    Velocity.X -= (0.5f - Velocity.X * 0.05f);
                }
                else if (d && Velocity.X < 10)
                {
                    Velocity.X += (0.5f + Velocity.X * 0.05f);
                }
 
           
            }
        }
        
        public override void Update(Level lvl, GameTime gt)
        {
            base.Update(lvl, gt);
            if (PlayerProjectile.Active)
                PlayerProjectile.Update(lvl, gt,Position);
        }
        public void DrawPlayer(SpriteBatch s)
        {            
            DrawGameObject(s);
            if (PlayerProjectile.Active)
                PlayerProjectile.DrawGameObject(s, 0);
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
        private void AirRoll(GameTime gt,bool a, bool s, bool d, bool w)
        {
            if (!UsedAirRoll)
            {
                ChangeGameObjectState(GameObject_State.AirRoll);
                AirRollTimer = (int)gt.TotalGameTime.TotalMilliseconds + 300;
                AirRolled_SavedVelocity = Velocity;
                ChangeAnimationState(Animation_State.airroll);
                UsedAirRoll = true;
                Velocity = Vector2.Zero;
                if (a)
                {
                    Velocity.X = -20;
                    GoingRight = false;
                }
                if (s)
                    Velocity.Y = 20;
                if (d)
                {
                    Velocity.X = 20;
                    GoingRight = true;
                }
                if (w)
                    Velocity.Y = -20;
                Wait = 100;
            }
        }
    }
}
