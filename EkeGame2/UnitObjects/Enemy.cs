using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;


namespace EkeGame2.UnitObjects
{
    class Enemy : UnitObject
    {
        readonly EnemyType Type;
        int counterEnemyCycle, EnemyCycleTimer;
        public Projectile EnemyProjectile;

        public Enemy(EnemyType et, ContentManager c, string objectName, int updateDelay, Vector2 spawnPosition, int timerOffset = 0) : base(c, objectName, updateDelay, spawnPosition)
        {
            counterEnemyCycle = 0 + timerOffset;
            EnemyCycleTimer = 2000;
            Type = et;
            LoadAnimation(c, objectName);
            Health = 1;
            EnemyProjectile = new Projectile(c, "Fireball/", 15, Vector2.Zero, ProjectileOwner.ENEMY);
        }
        public void Behaviour(GameTime gt, Player ThePlayer, Level lvl)
        {
            if (Active && GameObjectState != GameObject_State.Death)
                counterEnemyCycle += (int)gt.ElapsedGameTime.Milliseconds;
            if (GameObjectState != GameObject_State.Death)
            {
                switch (Type)
                {
                    //Jumper
                    case EnemyType.Purple:
                        if (counterEnemyCycle >= EnemyCycleTimer)
                        {
                            Jump(gt);
                            counterEnemyCycle = 0;

                            if (!GoingRight && GameObjectState == GameObject_State.Air)
                                Velocity.X = 5;
                            else if (GoingRight && GameObjectState == GameObject_State.Air)
                                Velocity.X = -5;
                            GoingRight = !GoingRight;
                        }
                        break;

                    //Shooter
                    case EnemyType.Orange:
                        FacePlayer(ThePlayer);
                        if (counterEnemyCycle >= EnemyCycleTimer)
                        {
                            ShootProjectile();
                            counterEnemyCycle = 0;
                        }
                        break;

                    //Thwomp
                    case EnemyType.DarkGray:
                        FacePlayer(ThePlayer);
                        if (GameObjectState == GameObject_State.onGround)
                        {
                            lvl.AddSpawnableEffect(SpawnableEffect_Type.SMOKE_PUFF_RIGHT, new Vector2(Position.X, Position.Y + Hitbox.Height / 2), 300, true, false, 3, 0);
                            lvl.AddSpawnableEffect(SpawnableEffect_Type.SMOKE_PUFF_LEFT, new Vector2(Position.X, Position.Y + Hitbox.Height / 2), 300, true, false, -3, 0);

                            ChangeAnimationState(Animation_State.falldamage);
                            LockAnimation(Animations[ActiveAnimation]);
                            Velocity.Y = -4;
                            GameObjectState = GameObject_State.Jumping;
                            Wait = 1000;
                        }
                        else if (GameObjectState == GameObject_State.Flying)
                        {
                            Velocity.Y = 0;

                            if (Position.X - ThePlayer.Position.X < 50 && Position.X - ThePlayer.Position.X > -50)
                            {
                                GameObjectState = GameObject_State.Air;
                            }
                        }
                        else if (GameObjectState == GameObject_State.Air)
                        {
                            Velocity.Y += 1;
                        }
                        else if (GameObjectState == GameObject_State.Jumping)
                        {
                            ChangeAnimationState(Animation_State.jumping);
                            Velocity.Y = -4;
                            if (SpawnPoint.Y - Position.Y > 3)
                            {
                                GameObjectState = GameObject_State.Flying;
                            }
                        }
                        break;

                    //FireWalker
                    case EnemyType.DarkRed:                        
                        if (counterEnemyCycle >= EnemyCycleTimer)
                        {
                            Velocity.X = 4;
                            counterEnemyCycle = 0;
                        }
                        break;
                        //Yoyo fiende. Pappas idé.
                }
            }
        }
        private void FacePlayer(Player ThePlayer)
        {
            if (ThePlayer.Position.X > Position.X)
            {
                GoingRight = true;
            }
            else
                GoingRight = false;
        }
        private void ShootProjectile()
        {
            if (!EnemyProjectile.Active)
            {
                EnemyProjectile.Shoot(Position, GoingRight);
                ChangeAnimationState(Animation_State.throwing);
                LockAnimation(Animations[ActiveAnimation]);
            }
        }
        public void Update(Level lvl, GameTime gt, Player ThePlayer)
        {
            if (Active)
            {
                Behaviour(gt, ThePlayer, lvl);
                if (EnemyProjectile.Active)
                    EnemyProjectile.Update(lvl, gt, Position);
                base.Update(lvl, gt);
            }
        }

        public override void DrawGameObject(SpriteBatch s)
        {
            if (EnemyProjectile.Active)
                EnemyProjectile.DrawGameObject(s);
            base.DrawGameObject(s);
        }
        public override void DrawHitbox(SpriteBatch s)
        {
            if (EnemyProjectile.Active)
                EnemyProjectile.DrawHitbox(s);
            base.DrawHitbox(s);
        }
        public void SetNewEnemyCopy_Properties(Vector2 position, int timerOffset=0)
        {
            Active = true;
            Position = position;
            UpdatePositionRectangle();
            counterEnemyCycle += timerOffset;

            switch (Type)
            {
                default:
                    ChangeGameObjectState(GameObject_State.Air);
                    Health = 1;
                    break;
                case EnemyType.DarkGray:
                    ChangeGameObjectState(GameObject_State.Flying);
                    Health = 2;
                    break;
            }
        }

        public Enemy GetCopy()
        {
            return (Enemy)this.MemberwiseClone();
        }
    } 
}
