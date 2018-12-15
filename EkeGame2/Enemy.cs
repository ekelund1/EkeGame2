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
    public class Enemy : GameObject
    {
        int counterEnemyCycle, EnemyCycleTimer;
        public Enemy()
        { }
        public Enemy(ContentManager c, string objektName, int updateDelay, Vector2 spawnPosition, int timerOffset=0)
        {
            ActiveAnimation = Animation_State.idle;
            GameObjectState = GameObject_State.Air;
            Position = spawnPosition;
            Velocity = Vector2.Zero;
            Content = c;
            Hitbox = Content.Load<Texture2D>(objektName+"/test_hitbox");
            UpdateDelay = updateDelay;
            UpdateCounter = 0;
            AnimationChanged = false;

            PositionRectangle = new Rectangle((int)Position.X, (int)Position.Y, Hitbox.Width, Hitbox.Height);


            LoadHitboxData();
            LoadAnimation(objektName);
            
            goingRight = false;

            active = true;
            counterEnemyCycle=0+timerOffset;
            EnemyCycleTimer = 2000;
        }
        public void Behaviour(GameTime gt)
        {
            if (active)
                counterEnemyCycle += (int)gt.ElapsedGameTime.Milliseconds;
            if (counterEnemyCycle >= EnemyCycleTimer)
            {
                counterEnemyCycle = 0;
                Jump();
                if (!goingRight)
                    Velocity.X += 5;
                else if (goingRight)
                    Velocity.X -= 5;
                goingRight = !goingRight;
            }
        }
        public void EnemyUpdate(Level lvl, GameTime gt)
        {
            Behaviour(gt);
            Update(lvl, gt);
        }
    }
}
