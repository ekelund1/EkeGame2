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
        private Vector2 Position;
        public int AliveTimer_Milliseconds {get; set;}
        public bool Active { get; set; }

        public SpawnableEffect(ContentManager Content, SpawnableEffect_Type type)
        {
            Type = type;
            AliveTimer_Milliseconds = 0;
            Position = new Vector2(0, -500);
            SpawnableEffectAnimation = new Animation(Position, Vector2.One, Vector2.One);
            SpawnableEffectAnimation.AnimationImage = Content.Load<Texture2D>("SpawnableEffects/" + type.ToString());
        }
        public void Update(GameTime gt)
        {
            if(Active)
                if (gt.TotalGameTime.TotalMilliseconds >= AliveTimer_Milliseconds)
                {
                    SpawnableEffectAnimation.Update(gt, true);
                }
                else
                {
                    Active = false;
                }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if(Active)
                SpawnableEffectAnimation.Draw(spriteBatch, Position);
        }
        public void SpawnEffect(Vector2 spawnPosition, int aliveTimer_Milliseconds, GameTime gt)
        {
            Position = spawnPosition;
            AliveTimer_Milliseconds = aliveTimer_Milliseconds+ (int)gt.TotalGameTime.TotalMilliseconds;
            Active = true;
        }

    }
}
