using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace EkeGame2
{
    class Collectible : AbstractGameObject
    {
        bool Collected;
        int CollectibleValue;
        double ScaleOscillation;

        public Collectible(ContentManager c, string objectName, Vector2 spawnPosition) : base(c, objectName, 15, spawnPosition)
        {
            GameObjectState = GameObject_State.Flying;
            ActiveAnimation = Animation_State.idle;
            GoingRight = true;
            Active = true;
            Collected = false;
            UpdatePositionRectangle();
            ScaleOscillation = 0;

            CollectibleValue = 10;

            var animationImage = c.Load<Texture2D>(objectName + "/idle");
            Animations[Animation_State.idle] = new Animation(Position, new Vector2(8, 1), Vector2.One, 500);
            Animations[Animation_State.idle].AnimationImage = animationImage;

            animationImage = c.Load<Texture2D>(objectName + "/death");
            Animations[Animation_State.death] = new Animation(Position, new Vector2(6, 1), Vector2.One, 100);
            Animations[Animation_State.death].AnimationImage = animationImage;
        }
        public override void Update(Level lvl, GameTime gt)
        {
            if (Active)
            {
                ScaleOscillation = 0.3+Math.Sin(gt.TotalGameTime.TotalSeconds) / 10;

                switch (GameObjectState)
                {
                    case GameObject_State.Flying:
                        ActiveAnimation = Animation_State.idle;
                        break;
                    case GameObject_State.Death:
                        if (gt.TotalGameTime.TotalMilliseconds >= DeathDelayTimer)
                            Active = false;
                        break;
                }
            }
            
            UpdateAnimations(gt);
        }
        public int TriggerCollect(GameTime gt)
        {
            ChangeGameObjectState(GameObject_State.Death);
            DeathDelayTimer = (int)gt.TotalGameTime.TotalMilliseconds + 590;
            ChangeAnimationState(Animation_State.death);
            Collected = true;
            return CollectibleValue;
        }
        public override void DrawGameObject(SpriteBatch s)
        {
            if (Active)
                Animations[ActiveAnimation].Draw(s, Position, 0, ScaleOscillation);
        }
        public bool IsCollected
        {
            get
            {
                return Collected;
            }
        }

    }
}
