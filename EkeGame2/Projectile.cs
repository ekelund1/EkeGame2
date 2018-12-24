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
    public sealed class Projectile : AbstractGameObject
    {
        public ProjectileOwner Owner { get;set; }
        public Projectile(ContentManager c, string objektName, int updateDelay, Vector2 spawnPosition, ProjectileOwner owner) : base(c, objektName, updateDelay, spawnPosition)
        {
            Owner = owner;
            LoadAnimation(objektName);
            Active = false;
        }
        void Movement()
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
            ActiveAnimation = Animation_State.jumping;
            Active = true;
            Position = spawnPosition+new Vector2(50,15);
            GoingRight = direction;
            if (GoingRight)
            {
                Position = spawnPosition + new Vector2(50, 15);
                Velocity = new Vector2(20, -15) + bonus_vel;
            }
            else if (!GoingRight)
            {
                Position = spawnPosition + new Vector2(-50, 15);
                Velocity = new Vector2(-20, -15) + bonus_vel;
            }
            PositionRectangle.Location = Position.ToPoint();

            Wait = 600;
        }
        public override void Update(Level lvl, GameTime gt)
        {
            if (Active)
                UpdateCounter += gt.ElapsedGameTime.Milliseconds;

            if (Active && UpdateCounter >= UpdateDelay && UpdateCounter >= Wait)
            {
                UpdateCounter = 0;
                Wait = 0;
                Movement();
                Gravity();
                LevelCollsion(lvl);
                if (lvl.LevelProjectileObjectCollision(this))
                { 
                    Active = false;
                    Position = Vector2.Zero;
                }
                Position += Velocity;
                if (Velocity == Vector2.Zero)
                    Active = false;
                PositionRectangle.Location = Position.ToPoint();
            }
            UpdateAnimations(gt);

        }

        public override void LoadAnimation(string name)
        {
            var animations = (Animation_State[])Enum.GetValues(typeof(Animation_State));
            foreach (var animation in animations)
            {
                var animationCount = 8;
                var animationDirections = 2;
                var frameUpdateDelay = 50;
                var imagePath = name + "/fireball";
                
                try
                {
                    var animationImage = Content.Load<Texture2D>(imagePath);
                    Animations[animation] = new Animation(Position, new Vector2(animationCount, animationDirections), frameUpdateDelay);

                    Animations[animation].AnimationImage = animationImage;
                }
                catch (Exception ex)
                {
                    Animations[animation] = new Animation(Position, new Vector2(8, 2), 10);
                    Animations[animation].AnimationImage = Content.Load<Texture2D>(name + "/fireball");
                }
            }
        }
    
        
        

    }
}
