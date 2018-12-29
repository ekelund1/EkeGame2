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
    class Enemy : AbstractGameObject
    {
        readonly EnemyType Type;
        int counterEnemyCycle, EnemyCycleTimer;
        
        public Enemy(EnemyType et, ContentManager c, string objectName, int updateDelay, Vector2 spawnPosition, int timerOffset=0) : base(c, objectName ,updateDelay, spawnPosition)
        {
            counterEnemyCycle=0+timerOffset;
            EnemyCycleTimer = 2000;
            Type = et;
            LoadAnimation(objectName);
            Health = 1;
        }
        public void Behaviour(GameTime gt)
        {
            if (PreviousAnimation == Animation_State.death)
                Active = false;
            if (Active)
                counterEnemyCycle += (int)gt.ElapsedGameTime.Milliseconds;
            if (counterEnemyCycle >= EnemyCycleTimer)
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
                        //Walker
                    case EnemyType.Orange:
                        counterEnemyCycle = 0;
                        if (GoingRight && Velocity.X <= 5)
                            Velocity.X += 0.5f;
                        else if (!GoingRight && Velocity.X >= -5)
                            Velocity.X -= 0.5f;
                        break;
                }
            }
        }
        public void EnemyUpdate(Level lvl, GameTime gt)
        {
            if (Active)
            {
                Behaviour(gt);
                Update(lvl, gt);
            }
            
        }
    }
}
