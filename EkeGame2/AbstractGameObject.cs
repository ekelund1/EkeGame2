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

    public abstract class AbstractGameObject
    {
        protected ContentManager Content;

        protected Vector2 Position;
        protected Vector2 Velocity;
        protected Vector2 SpawnPoint;
        public Texture2D Hitbox;
        public Rectangle PositionRectangle;

        protected Animation_State ActiveAnimation;
        protected Animation_State PreviousAnimation;
        protected GameObject_State GameObjectState;
        protected Dictionary<Animation_State, Animation> Animations;

        protected bool GoingRight;
        protected float Wait;
        public bool Active { get; set; }
        protected int UpdateDelay, UpdateCounter;
        protected bool AnimationChanged;
        public int Health { get; set; }
        private bool ResetVelocityY;

        private string ObjectName { get; }
        
        protected AbstractGameObject(ContentManager c, string objektName, int updateDelay, Vector2 spawnPosition)
        {
            ObjectName = objektName;
            Animations = new Dictionary<Animation_State, Animation>();

            ActiveAnimation = Animation_State.idle;
            GameObjectState = GameObject_State.Air;
            Position = spawnPosition;
            Velocity = Vector2.Zero;
            Content = c;
            Hitbox = Content.Load<Texture2D>(objektName + "/test_hitbox");
            UpdateDelay = updateDelay;
            UpdateCounter = 0;
            AnimationChanged = false;
            PositionRectangle = new Rectangle((int)Position.X, (int)Position.Y, Hitbox.Width, Hitbox.Height);

            GoingRight = false;
            ResetVelocityY=false;
            Active = true;
        }
        public void SetVelocity(Vector2 velocity)
        {
            Velocity = velocity;
        }
        public virtual void DrawHitbox(SpriteBatch s)
        {
            if (Active)
                s.Draw(Hitbox, PositionRectangle, Color.Red);             
        }
        public virtual void DrawGameObject(SpriteBatch s)
        {
            if (Active)
                Animations[ActiveAnimation].Draw(s, Position, new Vector2(2, 2));
        }
        public virtual void DrawGameObject(SpriteBatch s, GameTime gt)
        {
            if(Active)
                Animations[ActiveAnimation].Draw(s, Position, new Vector2(4, 4),gt.ElapsedGameTime.Milliseconds);
        }
        protected void Kill()
        {
            if (GameObjectState != GameObject_State.Death)
            {
                GameObjectState = GameObject_State.Death;
                AnimationChanged = false;
                ChangeAnimationState(Animation_State.death);
                WaitForAnimation(Animations[Animation_State.death]);
                UpdateCounter = 0;
            }
        }

        public virtual void Update(Level lvl, GameTime gt)
        {
            if (Active)            
                UpdateCounter += (int)gt.ElapsedGameTime.Milliseconds;
            
            if (Health <= 0)            
                this.Kill();            

            if (UpdateCounter >= UpdateDelay && UpdateCounter >= Wait && Active)
            {
                Gravity();
                LevelCollision(lvl);
                Position += Velocity;
                PositionRectangle.Location = Position.ToPoint();
                if (ResetVelocityY)
                {
                    Velocity.Y = 0;
                    ResetVelocityY = false;
                }

                //Update states
                switch (GameObjectState)
                {
                    case GameObject_State.Air:
                        if (Velocity.Y < 0)
                            ChangeAnimationState(Animation_State.jumping);
                        else if (Velocity.Y > 0)
                            ChangeAnimationState(Animation_State.falling);
                        break;
                    case GameObject_State.onGround:
                        if (Velocity.X != 0)
                            ChangeAnimationState(Animation_State.running);
                        else if (Velocity.X == 0 && (Keyboard.GetState().IsKeyDown(Keys.A) || Keyboard.GetState().IsKeyDown(Keys.D)))
                            ChangeAnimationState(Animation_State.byWall);
                        else
                            ChangeAnimationState(Animation_State.idle);
                        if (Velocity.X > 0)
                            GoingRight = true;
                        else if (Velocity.X < 0)
                            GoingRight = false;
                        break;
                    case GameObject_State.Flying:
                        break;
                    case GameObject_State.Death:
                        if (PreviousAnimation == Animation_State.death)
                            Active = false;
                        break;
                }
                Wait = 0;
                UpdateCounter = 0;
            }
            UpdateAnimations(gt);
        }
        protected void UpdateAnimations(GameTime gt)
        {
            if (PreviousAnimation != ActiveAnimation && UpdateCounter >= Wait)
            {
                Animations[PreviousAnimation].Deactivate();
                PreviousAnimation = ActiveAnimation;
            }
            AnimationChanged = false;
            Animations[ActiveAnimation].Update(gt, ActiveAnimation, GoingRight);
        }
        protected void ChangeAnimationState(Animation_State a)
        {
            if (!AnimationChanged)
            {
                ActiveAnimation = a;
                AnimationChanged = true;
            }
        }
        
        
        protected void LevelCollision(Level lvl)
        {
            Rectangle newPositionRectangle = PositionRectangle;
            newPositionRectangle.Offset(Velocity.X, Velocity.Y);

            //Check top
            if (Velocity.Y < 0)
            {
                for (int i = 0; i < Hitbox.Width; i++)
                {
                    if (lvl.HitboxColor(newPositionRectangle.Left + i, newPositionRectangle.Top) == Color.Black)
                    {
                        Velocity.Y = 0;
                    }
                }
            }
            else if (Velocity.Y >= 0)
            {
                //Check bottom
                for (int i = 0; i < Hitbox.Width; i++)
                {
                    //If collision with ground. Snap to ground.
                    if (lvl.HitboxColor(newPositionRectangle.Left + i, newPositionRectangle.Bottom) == Color.Black)
                    {
                        Velocity.Y = 0;
                        GameObjectState = GameObject_State.onGround;
                        i = Hitbox.Width + 1;
                    }
                }
            }
            newPositionRectangle = PositionRectangle;
            newPositionRectangle.Offset(Velocity.X, Velocity.Y);

            //Check right
            if (GoingRight)
            {
                for (int i = 0; i < Hitbox.Width; i++)
                {
                    if (i <= 1 && Velocity.X > 0 && lvl.HitboxColor(newPositionRectangle.Right, newPositionRectangle.Bottom - i) == Color.Black
                       && lvl.HitboxColor(newPositionRectangle.Right, newPositionRectangle.Bottom - (int)Velocity.X) != Color.Black)
                    {
                        Velocity.Y = -Math.Max(1, Math.Abs(Velocity.X));
                        ResetVelocityY = true;
                    }
                    else if (i > 1 && lvl.HitboxColor(newPositionRectangle.Right, newPositionRectangle.Bottom - i) == Color.Black)
                        Velocity.X = 0;
                    
                }
            }
            else
            {
                //Check left
                for (int i = 0; i < Hitbox.Width; i++)
                {
                    if (i <= 1 && Velocity.X < 0 && lvl.HitboxColor(PositionRectangle.Left, newPositionRectangle.Bottom - i) == Color.Black
                        && lvl.HitboxColor(newPositionRectangle.Left, newPositionRectangle.Bottom + (int)Velocity.X) != Color.Black)
                    {
                        Velocity.Y = -Math.Max(1,Math.Abs(Velocity.X));
                        ResetVelocityY = true;
                    }
                    else if (i > 1 && lvl.HitboxColor(newPositionRectangle.Left, newPositionRectangle.Bottom - i) == Color.Black)
                        Velocity.X = 0;

                }
            }
            
        }
      
        public void Respawn()
        {
            
            GameObjectState = GameObject_State.Air;
            Position = SpawnPoint;
            Active = true;
            Health = 1;
            Velocity=Vector2.Zero;
            
        }
        public bool SpriteCollision(AbstractGameObject go)
        {
            if (PositionRectangle.Intersects(go.PositionRectangle))
                return true;
            return false;
        }
        protected virtual void Gravity()
        {
            if (Velocity.Y < 20 && GameObjectState != GameObject_State.Flying)
                Velocity.Y++;
        }
        protected void Jump()
        {
            Velocity.Y = -20;
            GameObjectState = GameObject_State.Air;
            ActiveAnimation = Animation_State.jumpSquat;
            WaitForAnimation(Animations[ActiveAnimation]);
        }
        public virtual void LoadAnimation(string name, int animationCount_idle=6, 
            int animationCount_running = 8,int animationCount_jumping = 3,
            int animationCount_falling = 2,int animationCount_landing = 3,int animationCount_falldamage = 2,
            int animationCount_jumpSquat = 3,int animationCount_byWall = 6,int animationCount_death = 7,
            int animationCount_wallcling = 2,int animationCount_throwing = 4)
        {
            var animations = (Animation_State[])Enum.GetValues(typeof(Animation_State));

            foreach (var animation in animations)
            {
                var animationCount = 6;
                var animationDirections = 2;
                var frameUpdateDelay = 150;
                var imagePath = name + "/idle";
                switch (animation)
                {
                    case Animation_State.idle:
                        animationCount = animationCount_idle;
                        frameUpdateDelay = 150;
                        imagePath = name + "/idle";
                        break;
                    case Animation_State.running:
                        animationCount= animationCount_running;
                        frameUpdateDelay = 150;
                        imagePath = name + "/running";
                        break;
                    case Animation_State.jumping:
                        animationCount = animationCount_jumping;
                        frameUpdateDelay = 150;
                        imagePath = name + "/jumping";
                        break;
                    case Animation_State.falling:
                        animationCount = animationCount_falling;
                        frameUpdateDelay = 50;
                        imagePath = name + "/falling";
                        break;
                    case Animation_State.landing:
                        animationCount = animationCount_landing;
                        frameUpdateDelay = 300;
                        imagePath = name + "/landing";
                        break;
                    case Animation_State.falldamage:
                        animationCount = animationCount_falldamage;
                        frameUpdateDelay = 250;
                        imagePath = name + "/falldamage";
                        break;
                    case Animation_State.jumpSquat:
                        animationCount = animationCount_jumpSquat;
                        frameUpdateDelay = 150;
                        imagePath = name + "/jumpsquat";
                        break;
                    case Animation_State.byWall:
                        animationCount = animationCount_byWall;
                        frameUpdateDelay = 150;
                        imagePath = name + "/bywall";
                        break;
                    case Animation_State.death:
                        animationCount = animationCount_death;
                        frameUpdateDelay = 200;
                        imagePath = name + "/death";
                        break;
                    case Animation_State.wallcling:
                        animationCount = animationCount_wallcling;
                        frameUpdateDelay = 400;
                        imagePath = name + "/wallcling";
                        break;
                    case Animation_State.throwing:
                        animationCount = animationCount_throwing;
                        frameUpdateDelay = 120;
                        imagePath = name + "/throwing";
                        break;
                }
                try
                {
                    
                    var animationImage = Content.Load<Texture2D>(imagePath);
                    Animations[animation] = new Animation(Position, new Vector2(animationCount, animationDirections), frameUpdateDelay);

                    Animations[animation].AnimationImage = animationImage;
                }
                catch (Exception ex)
                {
                    Animations[animation] = new Animation(Position, new Vector2(6, 2), 150);
                    Animations[animation].AnimationImage = Content.Load<Texture2D>(name + "/idle");
                }
            }
        }
        public virtual void LoadAnimation(string name)
        {
            var animations = (Animation_State[])Enum.GetValues(typeof(Animation_State));

            foreach (var animation in animations)
            {
                var animationCount = 6;
                var animationDirections = 2;
                var frameUpdateDelay = 150;
                var imagePath = name + "/idle";
                switch (animation)
                {
                    case Animation_State.idle:
                        animationCount = 6;
                        frameUpdateDelay = 150;
                        imagePath = name + "/idle";
                        break;
                    case Animation_State.running:
                        animationCount = 8;
                        frameUpdateDelay = 150;
                        imagePath = name + "/running";
                        break;
                    case Animation_State.jumping:
                        animationCount = 3;
                        frameUpdateDelay = 150;
                        imagePath = name + "/jumping";
                        break;
                    case Animation_State.falling:
                        animationCount = 2;
                        frameUpdateDelay = 50;
                        imagePath = name + "/falling";
                        break;
                    case Animation_State.landing:
                        animationCount = 3;
                        frameUpdateDelay = 300;
                        imagePath = name + "/landing";
                        break;
                    case Animation_State.falldamage:
                        animationCount = 2;
                        frameUpdateDelay = 250;
                        imagePath = name + "/falldamage";
                        break;
                    case Animation_State.jumpSquat:
                        animationCount = 3;
                        frameUpdateDelay = 150;
                        imagePath = name + "/jumpsquat";
                        break;
                    case Animation_State.byWall:
                        animationCount = 6;
                        frameUpdateDelay = 150;
                        imagePath = name + "/bywall";
                        break;
                    case Animation_State.death:
                        animationCount = 7;
                        frameUpdateDelay = 200;
                        imagePath = name + "/death";
                        break;
                    case Animation_State.wallcling:
                        animationCount = 2;
                        frameUpdateDelay = 400;
                        imagePath = name + "/wallcling";
                        break;
                    case Animation_State.throwing:
                        animationCount = 4;
                        frameUpdateDelay = 120;
                        imagePath = name + "/throwing";
                        break;
                }
                try
                {

                    var animationImage = Content.Load<Texture2D>(imagePath);
                    Animations[animation] = new Animation(Position, new Vector2(animationCount, animationDirections), frameUpdateDelay);

                    Animations[animation].AnimationImage = animationImage;
                }
                catch (Exception ex)
                {
                    Animations[animation] = new Animation(Position, new Vector2(6, 2), 150);
                    Animations[animation].AnimationImage = Content.Load<Texture2D>(name + "/idle");
                }
            }
        }

        protected void WaitForAnimation(Animation a)
        {
            Wait = a.AnimationLenght * a.AmountOfFrames-1;
        }
        protected void WaitTime(int ms)
        {
            Wait = ms;
        }
        public Vector2 GetPosition
        {
            get
            {
                return Position;
            }
        }
        public Vector2 GetVelocity()
        {
            return Velocity;
        }
        public void ChangeHealth(int value)
        {
            Health += value;
        }

    }

}
