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
        public Vector2 Position;
        protected Vector2 Velocity;
        protected Vector2 SpawnPoint;
        public Texture2D Hitbox;
        public Rectangle PositionRectangle;
        
        protected Animation_State ActiveAnimation;
        protected Animation_State PreviousAnimation;
        protected GameObject_State GameObjectState;
        protected Dictionary<Animation_State, Animation> Animations;

        protected int GameTimer;
        protected bool GoingRight;
        protected float Wait, WaitCounter;
        public bool Active { get; set; }
        protected int UpdateDelay, UpdateCounter, DeathTimerCounter, DeathTimerDelay;
        protected float LockAnimationState, LockAnimationStateCounter;
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
            Hitbox = c.Load<Texture2D>(objektName + "/test_hitbox");
            UpdateDelay = updateDelay;
            UpdateCounter = 0;
            WaitCounter = 0;
            LockAnimationStateCounter = 0;
            AnimationChanged = false;
            PositionRectangle = new Rectangle((int)Position.X, (int)Position.Y, Hitbox.Width, Hitbox.Height);

            
            Active = true;
        }
        public void SetVelocity(Vector2 velocity)
        {
            Velocity = velocity;
        }
        public virtual void DrawHitbox(SpriteBatch s)
        {
            if (Active)
            {
                s.Draw(Hitbox, PositionRectangle, Color.White);
            }
        }
        public virtual void DrawGameObject(SpriteBatch s)
        {
            if (Active)
            { 
                Animations[ActiveAnimation].Draw(s, Position);
            }
        }
        public virtual void DrawGameObject(SpriteBatch s, GameTime gt)
        {
            if(Active)
                Animations[ActiveAnimation].Draw(s, Position,gt.ElapsedGameTime.Milliseconds);
        }
        

        
        protected void ChangeGameObjectState(GameObject_State GO_S)
        {
            if (GameObjectState != GameObject_State.Death)
                GameObjectState = GO_S;
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
            if (!AnimationChanged && LockAnimationStateCounter >= LockAnimationState && ActiveAnimation!=Animation_State.death)
            {
                ActiveAnimation = a;
                AnimationChanged = true;
                LockAnimationStateCounter = 0;
                LockAnimationState = 0;
            }
        }
        protected void LockAnimation(Animation a)
        {
            LockAnimationState = a.AnimationLenght * a.AmountOfFrames - 1;
        }
        protected void WaitForAnimation(Animation a)
        {
            Wait = a.AnimationLenght * a.AmountOfFrames - 1;
        }
        
        public virtual void LoadAnimation(ContentManager Content, string name, int animationCount_idle=6, 
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
                        frameUpdateDelay = 60;
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
                    Animations[animation] = new Animation(Position, new Vector2(animationCount, animationDirections), Vector2.One, frameUpdateDelay);

                    Animations[animation].AnimationImage = animationImage;
                }
                catch (Exception ex)
                {
                    Animations[animation] = new Animation(Position, new Vector2(6, 2), Vector2.One, 150);
                    Animations[animation].AnimationImage = Content.Load<Texture2D>(name + "/idle");
                }
            }
        }
        public virtual void LoadAnimation(ContentManager Content,string name)
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
                    Animations[animation] = new Animation(Position, new Vector2(animationCount, animationDirections), new Vector2(2,2), frameUpdateDelay);

                    Animations[animation].AnimationImage = animationImage;
                }
                catch (Exception ex)
                {
                    Animations[animation] = new Animation(Position, new Vector2(6, 2), new Vector2(2, 2), 150);
                    Animations[animation].AnimationImage = Content.Load<Texture2D>(name + "/idle");
                }
            }
        }

        public virtual void Update(Level lvl, GameTime gt)
        {
            if(Active)
                UpdateCounter += (int)gt.ElapsedGameTime.Milliseconds;
            if (UpdateCounter >= UpdateDelay)
                Position += Velocity;
            UpdateAnimations(gt);
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
