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
    public sealed class Projectile : UnitObject
    {
        public ProjectileOwner Owner { get;set; }
        private Projectile_State ProjectileState;
        private Projectile_Trajectory ProjectileTrajectory;
        public Projectile(ContentManager c, string objektName, int updateDelay, Vector2 spawnPosition, ProjectileOwner owner) : base(c, objektName, updateDelay, spawnPosition)
        {
            Owner = owner;
            LoadAnimation(c,objektName);
            Active = false;
        }
        void Movement()
        {
            if (Velocity.X < 0)
            {
                Velocity.X+=0.3f;
            }
            else if (Velocity.X > 0)
            {
                Velocity.X -= 0.3f;
            }
        }
        public void Shoot(Vector2 spawnPosition, bool direction, Projectile_Trajectory pt=Projectile_Trajectory.MEDIUM)
        {
            GameObjectState = GameObject_State.Air;
            ActiveAnimation = Animation_State.jumping;
            Active = true;
            Position = spawnPosition+new Vector2(50,15);
            ProjectileTrajectory = pt;
            Velocity = Vector2.Zero;
            GoingRight = direction;
            ProjectileState = Projectile_State.CHARGING;
            PositionRectangle.Location = Position.ToPoint();            
            Wait = 600;
            WaitCounter = 0;
        }
        public void Update(Level lvl, GameTime gt, Vector2 OwnerPosition)
        {
            if (Active)
            { 
                UpdateCounter += gt.ElapsedGameTime.Milliseconds;
                WaitCounter += gt.ElapsedGameTime.Milliseconds;
            }
           
            if (Active && UpdateCounter >= UpdateDelay)
            {
                UpdateCounter = 0;

                switch (ProjectileState)
                {
                    case Projectile_State.IN_MOTION:
                        Movement();
                        Gravity();
                        LevelCollision(lvl, gt);
                        if (lvl.LevelProjectileObjectCollision(this,gt))
                        {
                            Active = false;
                            Position = Vector2.Zero;
                        }
                        Position += Velocity;
                        if (WaitCounter >= Wait*3)
                            Active = false;
                        break;

                    case Projectile_State.CHARGING:
                        Position = OwnerPosition;
                    if (WaitCounter >= Wait)
                    {
                        WaitCounter = 0;
                        ProjectileState = Projectile_State.IN_MOTION;
                        SetProjectileTrajectory();
                        if (!GoingRight)
                            Velocity.X = -Velocity.X;
                    }
                    break;
                }
                
                
            }
            UpdatePositionRectangle();
            UpdateAnimations(gt);
        }

        private void SetProjectileTrajectory()
        {
            switch (ProjectileTrajectory)
            {
                case Projectile_Trajectory.LOW:
                    Velocity = new Vector2(25, -5);
                    break;
                case Projectile_Trajectory.MEDIUM:
                    Velocity = new Vector2(20, -10);
                    break;
                case Projectile_Trajectory.HIGH:
                    Velocity = new Vector2(10, -20);
                    break;
            }
        }

        public override void LoadAnimation(ContentManager Content, string name)
        {
            var animations = (Animation_State[])Enum.GetValues(typeof(Animation_State));
            foreach (var animation in animations)
            {
                var animationCount = 8;
                var animationDirections = 2;
                var frameUpdateDelay = 50;
                var imagePath = name + "/" + name.ToLower();
                
                try
                {
                    var animationImage = Content.Load<Texture2D>(imagePath);
                    Animations[animation] = new Animation(Position, new Vector2(animationCount, animationDirections), new Vector2(3,3), frameUpdateDelay);

                    Animations[animation].AnimationImage = animationImage;
                }
                catch (Exception ex)
                {
                    Animations[animation] = new Animation(Position, new Vector2(8, 2), new Vector2(3, 3), 10);
                    Animations[animation].AnimationImage = Content.Load<Texture2D>(name + "/fireball");
                }
            }
        }
    
        
        

    }
}
