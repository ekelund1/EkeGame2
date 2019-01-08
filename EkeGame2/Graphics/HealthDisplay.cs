﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace EkeGame2.Graphics
{
    class HealthDisplay
    {
        private Vector2 AnchorPosition;

        private SpriteFont HealthText;        
        //0 is background, 1 is health1, 2 is health2, 3 is health3.
        private Animation[] HealthMeter;
        private Animation[] HealthMeterNumber;

        private Vector2 HealthNumberOffset;
        int UpdateCounter, UpdateDelay;
        private double NumberScaleOscillation;
        bool Active;

        public HealthDisplay(ContentManager c)
        {
            HealthMeter = new Animation[4];
            HealthMeterNumber = new Animation[4];
            HealthNumberOffset = new Vector2(20, 20);

            Active = true;
            UpdateCounter = 0;
            UpdateDelay = 30;
            AnchorPosition = new Vector2(700, 120);
            for (int i = 0; i < 4; i++)
            {
                HealthMeter[i] = new Animation(AnchorPosition, Vector2.One, new Vector2(1.5f,1.5f));
                var img = c.Load<Texture2D>("HUD/HealthDisplay/HealthDisplay/HealthDisplay" + i.ToString());
                HealthMeter[i].AnimationImage = img;

                HealthMeterNumber[i] = new Animation(AnchorPosition, Vector2.One, new Vector2(0.2f,0.2f));
                img = c.Load<Texture2D>("HUD/HealthDisplay/" + i.ToString());
                HealthMeterNumber[i].AnimationImage = img;
            }
        }
        public void Draw(SpriteBatch s, GameTime gt)
        {
            if (Active)
            {
                for (int i = 0; i <= 3; i++)
                {
                    if (HealthMeter[i].Active)
                        HealthMeter[i].Draw(s, AnchorPosition);
                    if (HealthMeterNumber[i].Active)
                        HealthMeterNumber[i].Draw(s, AnchorPosition+HealthNumberOffset);
                }
            }
        }
        public void Update(GameTime gt, Player ThePlayer, Level TheLvl)
        {
            if (Active)
            {
                UpdateCounter += (int)gt.ElapsedGameTime.Milliseconds;
                NumberScaleOscillation = 0.4 + Math.Sin(gt.TotalGameTime.TotalSeconds*2.33)/18;
            }
                        
            if (UpdateCounter >= UpdateDelay)
            {
                UpdateCounter = 0;
                SetActive(ThePlayer);
                for (int i = 0; i < 4; i++)
                {
                    if (HealthMeter[i].Active)
                        HealthMeter[i].Update(gt, Animation_State.idle, true);
                    if (HealthMeterNumber[i].Active)
                    {
                        HealthMeterNumber[i].AnimationScale = new Vector2((float)NumberScaleOscillation, (float)NumberScaleOscillation);
                        HealthMeterNumber[i].Update(gt, Animation_State.idle, true);
                    }
                }
            }
        }
        private void SetActive(Player ThePlayer)
        {
            switch (ThePlayer.Health)
            {
                case 3:
                    HealthMeter[0].Active = true;
                    HealthMeter[1].Active = true;
                    HealthMeter[2].Active = true;
                    HealthMeter[3].Active = true;
                    HealthMeterNumber[0].Active = false;
                    HealthMeterNumber[1].Active = false;
                    HealthMeterNumber[2].Active = false;
                    HealthMeterNumber[3].Active = true;
                    break;
                case 2:
                    HealthMeter[0].Active = true;
                    HealthMeter[1].Active = true;
                    HealthMeter[2].Active = true;
                    HealthMeter[3].Active = false;
                    HealthMeterNumber[0].Active = false;
                    HealthMeterNumber[1].Active = false;
                    HealthMeterNumber[2].Active = true;
                    HealthMeterNumber[3].Active = false;
                    break;
                case 1:
                    HealthMeter[0].Active = true;
                    HealthMeter[1].Active = true;
                    HealthMeter[2].Active = false;
                    HealthMeter[3].Active = false;
                    HealthMeterNumber[0].Active = false;
                    HealthMeterNumber[1].Active = true;
                    HealthMeterNumber[2].Active = false;
                    HealthMeterNumber[3].Active = false;
                    break;
                case 0:
                    HealthMeter[0].Active = true;
                    HealthMeter[1].Active = false;
                    HealthMeter[2].Active = false;
                    HealthMeter[3].Active = false;
                    HealthMeterNumber[0].Active = true;
                    HealthMeterNumber[1].Active = false;
                    HealthMeterNumber[2].Active = false;
                    HealthMeterNumber[3].Active = false;
                    break;
                default:
                    HealthMeter[0].Active = true;
                    HealthMeter[1].Active = false;
                    HealthMeter[2].Active = false;
                    HealthMeter[3].Active = false;
                    HealthMeterNumber[0].Active = true;
                    HealthMeterNumber[1].Active = false;
                    HealthMeterNumber[2].Active = false;
                    HealthMeterNumber[3].Active = false;
                    break;
            }
        }
    }
}
