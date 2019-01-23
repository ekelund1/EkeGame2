using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EkeGame2.SpawnableEffects
{
    public sealed class SpawnableEffect
    {
        private Animation SpawnableEffectAnimation;
        public SpawnableEffect_Type Type { get; }
        private Vector2 Velocity;
        private Vector2 Position;
        public int AliveTimer_Milliseconds {get; set;}
        public bool Active { get; set; }
        private int AliveCounter;
        private double ImageScale;
        //private bool RandomPosition;
        public bool OnLowestLayer { get; set; }

        public SpawnableEffect(ContentManager Content, SpawnableEffect_Type type)
        {
            Velocity = Vector2.Zero;
            Type = type;
            AliveTimer_Milliseconds = 0;
            Position = new Vector2(0, -500);
            SpawnableEffectAnimation = new Animation(Position, Vector2.One, new Vector2(1,1));
            SpawnableEffectAnimation.AnimationImage = Content.Load<Texture2D>("SpawnableEffects/" + type.ToString());
            OnLowestLayer = true;
            
        }
        public void Update(GameTime gt)
        {
            
            if (Active)
            {
                AliveCounter += (int)gt.ElapsedGameTime.TotalMilliseconds;
                SpawnableEffectAnimation.Update(gt);
                ImageScale -= (AliveCounter/AliveTimer_Milliseconds);
                Position += Velocity;
            }        

            if (AliveCounter > AliveTimer_Milliseconds)
            {
                Active = false;
                AliveCounter = 0;
                AliveTimer_Milliseconds = 0;
                SpawnableEffectAnimation.Active = false;
            }           
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if(Active)
                SpawnableEffectAnimation.Draw(spriteBatch, Position,0,ImageScale);
        }
        public void SpawnCopyOfEffect(Vector2 spawnPosition, int aliveTimer_Milliseconds, bool onLowestLayer = true,bool randomPosition=false, float VelocityX=0, float VelocityY=0, double extraScale=0)
        {
            Velocity.X = VelocityX;
            Velocity.Y = VelocityY;
            Position = spawnPosition;
            OnLowestLayer = onLowestLayer;
            AliveTimer_Milliseconds = aliveTimer_Milliseconds;
            Active = true;
            SpawnableEffectAnimation.Active = true;
            ImageScale = extraScale;           
        }
        public SpawnableEffect GetCopy()
        {
            return (SpawnableEffect)this.MemberwiseClone();
        }
    }
}
