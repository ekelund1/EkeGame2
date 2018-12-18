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

            Active = true;
        }
        
        public virtual void DrawHitbox(SpriteBatch s)
        {
            if(Active)
                s.Draw(Hitbox, Position);             
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
        public void Kill()
        {
            GameObjectState = GameObject_State.Death;
            ActiveAnimation=Animation_State.death;
            WaitForAnimation(Animations[Animation_State.death]);
            this.Active = false;
            Velocity = Vector2.Zero;
        }
        
        public virtual void Update(Level lvl, GameTime gt)
        {
            if (Active)
            {
                UpdateCounter += (int)gt.ElapsedGameTime.Milliseconds;
            }
            if (GameObjectState == GameObject_State.Death && UpdateCounter >= Wait)
            {
                Active = false;
                /*Velocity = new Vector2(0, 0);
                GameObjectState = GameObject_State.Air;
                Wait = 0;*/
            }
            else if (UpdateCounter >= UpdateDelay && UpdateCounter >= Wait)
            {
                UpdateCounter = 0;
                Wait = 0;

                LevelCollsion(lvl);
                Position += Velocity;

                //Update states
                switch (GameObjectState)
                {
                    case GameObject_State.Air:
                        if (Velocity.Y < 0)
                            ChangeAnimationState(Animation_State.jumping);
                        else if (Velocity.Y > 0)
                            ChangeAnimationState(Animation_State.falling);
                        Gravity();
                        break;
                    case GameObject_State.onGround:
                        if (Velocity.X != 0)
                            ChangeAnimationState(Animation_State.running);
                        else
                            ChangeAnimationState(Animation_State.idle);

                        if (Velocity.X > 0)
                            GoingRight = true;
                        else if (Velocity.X < 0)
                            GoingRight = false;

                        break;
                    case GameObject_State.Flying:
                        break;
                }
                PositionRectangle.Location = Position.ToPoint();
            }

            UpdateAnimations(gt);
        }
        protected void UpdateAnimations(GameTime gt)
        {
            if (PreviousAnimation != ActiveAnimation)
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
        protected void LevelCollsion(Level lvl)
        {
            int newX = (int)Math.Floor((Position.X + Velocity.X));
            int newY = (int)Math.Floor((Position.Y + Velocity.Y));
            Vector2 new_pos = Vector2.Zero;

            if (new_pos.X < 0 || new_pos.X > Hitbox.Width || new_pos.Y < 0 || new_pos.Y > Hitbox.Height)
                Velocity = Vector2.Zero;

            //Checks y
            if (Velocity.Y < 0 && lvl.Hitbox_Colors[newX, newY] == Color.Black)
            {
                for (int i = (int)Velocity.Y; i < 0; i++)
                {
                    if (lvl.Hitbox_Colors[newX, (int)Position.Y + i] != Color.Black)
                    {
                        new_pos.Y = i;
                        i = 0;
                    }
                }
                Velocity.Y = 0;
            }
            else if (Velocity.Y > 0 && lvl.Hitbox_Colors[newX, newY] == Color.Black && GameObjectState == GameObject_State.Air)
            {
                for (int i = (int)Velocity.Y; i > 0; i--)
                {
                    if (lvl.Hitbox_Colors[newX, (int)Position.Y + i] != Color.Black)
                    {
                        new_pos.Y = i;
                        i = 0;
                    }
                }
                Velocity.Y = 0;
                ChangeAnimationState(Animation_State.falldamage);
                WaitForAnimation(Animations[ActiveAnimation]);
                GameObjectState = GameObject_State.onGround;
            }
            else if (GameObjectState == GameObject_State.onGround && lvl.Hitbox_Colors[(int)Position.X, (int)Position.Y + 1] != Color.Black)
            {
                GameObjectState = GameObject_State.Air;
                ChangeAnimationState(Animation_State.falling);
            }
            //Checks X
            if (GoingRight && lvl.Hitbox_Colors[newX, newY] == Color.Black)
            {
                for (int i = (int)Velocity.X; i > 0; i--)
                {
                    if (lvl.Hitbox_Colors[(int)Position.X + i, newY] != Color.Black)
                    {
                        new_pos.X = i;
                        i = 0;
                    }
                }
                Velocity.X = 0;
                ChangeAnimationState(Animation_State.byWall);

            }
            else if (!GoingRight && lvl.Hitbox_Colors[newX, newY] == Color.Black)
            {
                for (int i = (int)Velocity.X; i < 0; i++)
                {
                    if (lvl.Hitbox_Colors[(int)Position.X + i, newY] != Color.Black)
                    {
                        new_pos.X = i;
                        i = 0;
                    }
                }
                Velocity.X = 0;
                ChangeAnimationState(Animation_State.byWall);
            }
            Position += new_pos;

        }

        public bool SpriteCollision(AbstractGameObject go)
        {
            if (PositionRectangle.Intersects(go.PositionRectangle))
                return true;
            return false;
        }
        protected virtual void Gravity()
        {
            if (Velocity.Y < 20 && (GameObjectState == GameObject_State.Air))
                Velocity.Y = (Velocity.Y + 1);
        }
        protected void Jump()
        {
            Velocity.Y = -20;
            GameObjectState = GameObject_State.Air;
            ActiveAnimation = Animation_State.jumpSquat;
            WaitForAnimation(Animations[ActiveAnimation]);
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
            Wait = a.AnimationLenght * a.AmountOfFrames;
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

    }

}
