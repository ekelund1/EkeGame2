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
    class Animation
    {
        int frameCounter;
        float switchFrame;
        bool active;
        Vector2 position, amountOfFrames, currentFrame;
        Texture2D image;
        Rectangle sourceRect;        

        public Animation(Vector2 pos, Vector2 frames, float _switchFrame=200)
        {
            active = true;
            switchFrame = _switchFrame;
            position = pos;
            amountOfFrames = frames;
        }
        public void Initialize(Vector2 position, Vector2 frames)
        {
            active = true;
            switchFrame = 200;
            this.position = position;
            this.amountOfFrames = frames;
        }
        public void Update(GameTime gameTime, Animation_State state, bool goingRight)
        {
            active = true;
            frameCounter += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            
            if (frameCounter >= switchFrame)
            {
                frameCounter = 0;
                currentFrame.X += FrameWidth;
                if (currentFrame.X >= image.Width)
                    currentFrame.X = 0;
            }
            if (goingRight)
                currentFrame.Y = 0 * (image.Height / 2);
            else
                currentFrame.Y = 1 * (image.Height / 2);
            
            sourceRect = new Rectangle((int)currentFrame.X, (int)currentFrame.Y, FrameWidth, FrameHeight);
        }
        public void Deactivate()
        {
            active = false;
            currentFrame.X = 0;
            frameCounter = 0;
        }
        public void Draw(SpriteBatch s, Vector2 pos)
        {
            Position = pos;
            s.Draw(image, allignAnimation(pos), sourceRect, Color.White);
        }
        public void Draw(SpriteBatch s, Vector2 pos, Vector2 scale, float rotation = 0)
        {
            Position = pos;
            s.Draw(image, allignAnimation(pos), sourceRect, Color.White, rotation, Vector2.Zero, scale, 0, 0);
        }
        private Vector2 allignAnimation(Vector2 pos)
        {
            return new Vector2(pos.X - FrameWidth/2, pos.Y - FrameHeight/2);
        }
        public Vector2 CurrentFrame
        {
            get
            {
                return currentFrame;
            }
            set
            {
                currentFrame = value;
            }
        }
        public float AmountOfFrames
        {
            get
            {
                return amountOfFrames.X;
            }
        }
        public float AnimationLenght
        {
            get
            {
                return SwitchFrame;
            }
        }
        public Rectangle SourceRectangle
        {
            get
            {
                return sourceRect;
            }
            set
            {
                sourceRect = value;
            }
        }

        public float SwitchFrame
        {
            get
            {
                return switchFrame;
            }
            set
            {
                switchFrame = value;
            }
        }

        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        public int FrameWidth
        {
            get
            {
                return image.Width / (int)amountOfFrames.X;
            }
        }

        public int FrameHeight
        {
            get
            {
                return image.Height / (int)amountOfFrames.Y;
            }
        }

        public bool Active
        {
            get
            {
                return active;
            }
            set
            {
                active = value;
            }
        }

        public Texture2D AnimationImage
        {
            set
            {
                image = value;
            }
        }
    }
    
}
