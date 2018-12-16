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
    public class GameObject
    {
        protected Vector2 Position;
        protected Vector2 Velocity;
        protected Vector2 SpawnPoint;
        public Texture2D Hitbox;
        protected Color[,] HitboxData;
        protected Texture2D PlayerModel;
        protected ContentManager Content;
        protected Animation_State ActiveAnimation;
        protected Animation_State PreviousAnimation;
        protected GameObject_State GameObjectState;
        Animation[] Animations;
        protected bool goingRight;
        protected float Wait;
        protected bool active { get; set; }
        protected int UpdateDelay,UpdateCounter;
        protected bool AnimationChanged;
        public Rectangle PositionRectangle;

        public GameObject()
        { }
        public GameObject(ContentManager c, string objektName, int updateDelay)
        {
            ActiveAnimation = Animation_State.idle;
            GameObjectState = GameObject_State.Air;
            SpawnPoint = new Vector2(100, 100);
            Position = SpawnPoint;
            Velocity = Vector2.Zero;
            Content = c;
            Hitbox = Content.Load<Texture2D>(objektName+"/test_hitbox");
            UpdateDelay = updateDelay;
            UpdateCounter = 0;
            AnimationChanged = false;
            PositionRectangle = new Rectangle((int)Position.X, (int)Position.Y, Hitbox.Width, Hitbox.Height);
            LoadHitboxData();
            LoadAnimation(objektName);            

            goingRight = true;
            
            active = true;
        }
        public void DrawHitbox(SpriteBatch s)
        {
            //s.Draw(new Rectangle((int)Position.X,(int)Position.Y, Hitbox_Size,Hitbox_Size);            
            s.Draw(Hitbox, Position);
        }
        public void DrawGameObject(SpriteBatch s)
        {
            Animations[(int)ActiveAnimation].Draw(s, Position,new Vector2(2,2));
        }
        public void Movement(bool a, bool d, bool w,bool r, bool h)
        {
            if (w && GameObjectState == GameObject_State.onGround)            
                Jump();
            if (r)
                this.Respawn();

            if (a && Velocity.X > -10)
            {
                Velocity.X--;
            }
            else if (d && Velocity.X < 10)
            {
                Velocity.X++;
            }
            else
            {
                if (Velocity.X < 0)
                {
                    Velocity.X++;
                }
                else if (Velocity.X > 0)
                {
                    Velocity.X--;
                }
            }
        }
        public void Respawn()
        {
            GameObjectState = GameObject_State.Death;
            ChangeAnimationState(Animation_State.death);
            WaitForAnimation(Animations[(int)Animation_State.death]);            
        }
        public void Update(Level lvl, GameTime gt)
        {
            if (active)
            {
                UpdateCounter += (int)gt.ElapsedGameTime.Milliseconds;
            }
            if (GameObjectState == GameObject_State.Death && UpdateCounter >= Wait)
            {

                Position = SpawnPoint;
                Velocity = new Vector2(0, 0);
                GameObjectState = GameObject_State.Air;
                Wait = 0;
            }

            if (UpdateCounter >= UpdateDelay && UpdateCounter >= Wait)
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
                            goingRight = true;
                        else if (Velocity.X < 0)
                            goingRight = false;

                        break;
                    case GameObject_State.Flying:
                        break;
                }
                PositionRectangle.Location = Position.ToPoint();
            }
            
            UpdateAnimations(gt);
        }
        private void UpdateAnimations(GameTime gt)
        {
            if (PreviousAnimation != ActiveAnimation)
            {
                Animations[(int)PreviousAnimation].Deactivate();
                PreviousAnimation = ActiveAnimation;
            }
            AnimationChanged = false;
            Animations[(int)ActiveAnimation].Update(gt, ActiveAnimation, goingRight);
        }
        private void ChangeAnimationState(Animation_State a)
        {
            if (!AnimationChanged)
            {
                ActiveAnimation = a;
                AnimationChanged = true;
            }
        }

        public void LevelCollsion(Level lvl)
        {
            //Check OOB
           /* Vector2 a = (Position + Velocity);
            if (a.X < 0 || a.X > lvl.GetHitbox().Width || a.Y < 0 || a.Y > lvl.GetHitbox().Height)
                Velocity = Vector2.Zero; */
            
            int newX = (int)Math.Floor((Position.X + Velocity.X));
            int newY = (int)Math.Floor((Position.Y + Velocity.Y));
            Vector2 new_pos = Vector2.Zero;            

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
                WaitForAnimation(Animations[(int)ActiveAnimation]);
                GameObjectState = GameObject_State.onGround;
            }
            else if ( GameObjectState == GameObject_State.onGround && lvl.Hitbox_Colors[(int)Position.X, (int)Position.Y + 1] != Color.Black)
            {
                GameObjectState = GameObject_State.Air;
                ChangeAnimationState(Animation_State.falling);
            }
            //Checks X
            if (goingRight && lvl.Hitbox_Colors[newX, newY] == Color.Black)
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
            else if (!goingRight && lvl.Hitbox_Colors[newX, newY] == Color.Black)
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

        
        public bool SpriteCollision(GameObject go)
        {
            if (PositionRectangle.Intersects(go.PositionRectangle))
                return true;
            return false;
        }
        
        public float GetX
        {
            get
            {
                return Position.X;
            }
        }
        public float getY
        {
            get
            {
                return Position.Y;
            } 
        }
        private void Gravity()
        {
            if (Velocity.Y < 20 && (GameObjectState == GameObject_State.Air))
                Velocity.Y = (Velocity.Y + 1);            
        }
        public void Jump()
        {
            Velocity.Y = -20;
            GameObjectState = GameObject_State.Air;
            ActiveAnimation = Animation_State.jumpSquat;
            WaitForAnimation(Animations[(int)ActiveAnimation]);
        }

        ///Loads the animations.
        ///Rules for animation. 
        ///
        public void LoadAnimation(string name)
        {
            Animations = new Animation[12];

            //Tim tipsar!
            //var _animations = new Dictionary<string, Animation>();
            //_animations.Add("idle", new Animation(Position, new Vector2(6, 2), 150));
            //_animations["idle"].AnimationImage = Content.Load<Texture2D>(name + "/idle");

            //idle=0, running=1, jumping=2, falling=3, landing=4, hurt=5, jumpSquat=6;
            Animations[0] = new Animation(Position, new Vector2(6, 2), 150);
            Animations[0].AnimationImage = Content.Load<Texture2D>(name+"/idle");

            Animations[1] = new Animation(Position, new Vector2(8, 2), 150);
            Animations[1].AnimationImage = Content.Load<Texture2D>(name +"/running");

            Animations[2] = new Animation(Position, new Vector2(3, 2), 150);
            Animations[2].AnimationImage = Content.Load<Texture2D>(name + "/jumping");

            Animations[3] = new Animation(Position, new Vector2(2, 2), 50);
            Animations[3].AnimationImage = Content.Load<Texture2D>(name + "/falling");

            Animations[4] = new Animation(Position, new Vector2(3, 2), 300);
            Animations[4].AnimationImage = Content.Load<Texture2D>(name + "/landing");

            Animations[6] = new Animation(Position, new Vector2(3, 2), 125);
            Animations[6].AnimationImage = Content.Load<Texture2D>(name + "/landing");

            Animations[7] = new Animation(Position, new Vector2(2, 2), 250);
            Animations[7].AnimationImage = Content.Load<Texture2D>(name + "/falldamage");

            Animations[8] = new Animation(Position, new Vector2(6, 2), 100);
            Animations[8].AnimationImage = Content.Load<Texture2D>(name + "/bywall");

            Animations[9] = new Animation(Position, new Vector2(7, 2), 400);
            Animations[9].AnimationImage = Content.Load<Texture2D>(name + "/death");

            Animations[10] = new Animation(Position, new Vector2(2, 2), 400);
            Animations[10].AnimationImage = Content.Load<Texture2D>(name + "/wallcling");

            Animations[11] = new Animation(Position, new Vector2(4, 2), 200);
            Animations[11].AnimationImage = Content.Load<Texture2D>(name + "/throwing");
        }
        private void WaitForAnimation(Animation a)
        {
            Wait = a.AnimationLenght * a.AmountOfFrames;
        }
        protected void WaitTime(int ms)
        {
            Wait = ms;
        }
        public void LoadHitboxData()
        {
            Color[] colors1D = new Color[Hitbox.Width * Hitbox.Height];
            Hitbox.GetData(colors1D);

            Color[,] colors2D = new Color[Hitbox.Width, Hitbox.Height];
            for (int x = 0; x < Hitbox.Width; x++)
            {
                for (int y = 0; y < Hitbox.Height; y++)
                {
                    colors2D[x, y] = colors1D[x + y * Hitbox.Width];
                }
            }
            HitboxData = colors2D;
        }
        public Color[,] GetHitboxData
        {
            get
            {
                return GetHitboxData;
            }
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
