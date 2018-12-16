﻿using System;
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
        EnemyType Type;
        int counterEnemyCycle, EnemyCycleTimer;
        
        public Enemy(EnemyType et, ContentManager c, string objectName, int updateDelay, Vector2 spawnPosition, int timerOffset=0) : base(c, objectName, updateDelay, spawnPosition)
        {
            counterEnemyCycle=0+timerOffset;
            EnemyCycleTimer = 2000;
            Type = et;
        }
        public void Behaviour(GameTime gt)
        {
            if (active)
                counterEnemyCycle += (int)gt.ElapsedGameTime.Milliseconds;
            if (counterEnemyCycle >= EnemyCycleTimer)
            {
                switch (Type)
                { 
                    case EnemyType.Purple:
                        counterEnemyCycle = 0;
                        Jump();
                        if (!goingRight)
                            Velocity.X += 5;
                        else if (goingRight)
                            Velocity.X -= 5;
                        goingRight = !goingRight;
                        break;    
                }
            }
        }
        public void EnemyUpdate(Level lvl, GameTime gt)
        {
            Behaviour(gt);
            Update(lvl, gt);
        }
    }
}
