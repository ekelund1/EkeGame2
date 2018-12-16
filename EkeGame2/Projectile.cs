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
    class Projectile : AbstractGameObject
    {

        public Projectile(ContentManager c, string objektName, int updateDelay, Vector2 spawnPosition) : base(c, objektName, updateDelay, spawnPosition)
        {
            LoadAnimation(objektName);
            active = false;
        }
        protected void Movement()
        {
            if (Velocity.X < 0)
            {
                Velocity.X+=0.1f;
            }
            else if (Velocity.X > 0)
            {
                Velocity.X -= 0.1f;
            }
        }
        public void Shoot(Vector2 spawnPosition, Vector2 bonus_vel, bool direction)
        {
            GameObjectState = GameObject_State.Air;
            active = true;
            Position = spawnPosition;
            goingRight = direction;
            if (goingRight)
                Velocity = new Vector2(20, -15)+bonus_vel;
            else if (!goingRight)
                Velocity = new Vector2(-20, -15) + bonus_vel;
        }
        public override void Update(Level lvl, GameTime gt)
        {
            if (active)
            {
                Movement();
                Gravity();                
                LevelCollsion(lvl);
                Position += Velocity;
                if (Velocity == Vector2.Zero)
                    active = false;
            }            
        }
        public void DrawProjectile(SpriteBatch s,GameTime gt)
        {
            if (active)
                Animations[Animation_State.idle].Draw(s, Position, new Vector2(2, 2));
        }
        protected override void LoadAnimation(string name)
        {
            Animations[Animation_State.idle] = new Animation(Position, new Vector2(1, 1), 1000);
        }

    }
}
