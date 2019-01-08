using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using EkeGame2;


namespace EkeGame2
{
    class Enemy : AbstractGameObject
    {
        readonly EnemyType Type;
        int counterEnemyCycle, EnemyCycleTimer;
        public Projectile EnemyProjectile;


        public Enemy(EnemyType et, ContentManager c, string objectName, int updateDelay, Vector2 spawnPosition, int timerOffset=0) : base(c, objectName ,updateDelay, spawnPosition)
        {
            counterEnemyCycle=0+timerOffset;
            EnemyCycleTimer = 2000;
            Type = et;
            LoadAnimation(c, objectName);
            Health = 1;
            EnemyProjectile = new Projectile(c, "Fireball", 15, Vector2.Zero, ProjectileOwner.ENEMY);


        }
        public void Behaviour(GameTime gt, Player ThePlayer)
        {
            if (Active && GameObjectState!=GameObject_State.Death)
                counterEnemyCycle += (int)gt.ElapsedGameTime.Milliseconds;
            if (counterEnemyCycle >= EnemyCycleTimer && GameObjectState != GameObject_State.Death)
            {
                switch (Type)
                { 
                    //Jumper
                    case EnemyType.Purple:
                        counterEnemyCycle = 0;
                        Jump();
                        if (!GoingRight && GameObjectState==GameObject_State.Air)
                            Velocity.X += 5;
                        else if (GoingRight && GameObjectState == GameObject_State.Air)
                            Velocity.X -= 5;
                        GoingRight = !GoingRight;
                        break;
                        //Shooter
                    case EnemyType.Orange:
                        counterEnemyCycle = 0;
                        FacePlayer(ThePlayer);
                        ShootProjectile();
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
                Behaviour(gt,ThePlayer);
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
    }
}
