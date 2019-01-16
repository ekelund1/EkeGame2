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
    public abstract class UnitObject : AbstractGameObject
    {
        public int Health { get; set; }
        protected double HealthInvunerabilityTimer;
        protected bool ResetVelocityY;

        public UnitObject(ContentManager c, string objectName, int updateDelay, Vector2 spawnPosition) : base(c, objectName, updateDelay, spawnPosition)
        {
            GoingRight = false;
            ResetVelocityY = false;

        }
        public override void Update(Level lvl, GameTime gt)
        {
            if (Active)
            {
                UpdateCounter += (int)gt.ElapsedGameTime.Milliseconds;
                LockAnimationStateCounter += (int)gt.ElapsedGameTime.Milliseconds;
                DeathTimerCounter += (int)gt.ElapsedGameTime.Milliseconds;
            }

            if (Health <= 0)
                this.Kill();

            if (UpdateCounter >= UpdateDelay && UpdateCounter >= Wait && Active)
            {
                Gravity();
                LevelCollision(lvl, gt);
                Position += Velocity;
                PositionRectangle.Location = Position.ToPoint();
                VelocityDecay();
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
                        if (DeathTimerCounter >= DeathTimerDelay)
                            Active = false;
                        break;
                }
                Wait = 0;
                UpdateCounter = 0;
            }
            UpdateAnimations(gt);
        }

        public void ChangeHealth(int value, GameTime gt)
        {
            if (gt.TotalGameTime.TotalMilliseconds > HealthInvunerabilityTimer)
            {
                Health += value;
                HealthInvunerabilityTimer = gt.TotalGameTime.TotalMilliseconds + 1000;
            }
        }
        protected void Kill()
        {
            if (GameObjectState != GameObject_State.Death)
            {
                ChangeGameObjectState(GameObject_State.Death);
                AnimationChanged = false;
                ChangeAnimationState(Animation_State.death);
                LockAnimation(Animations[Animation_State.death]);
                DeathTimerCounter = 0;
                DeathTimerDelay = 1100;
                Velocity.X = Velocity.X / 2;
            }
        }
        protected void VelocityDecay(float decayAlteration = 0)
        {
            if (Velocity.X < 0.5 && Velocity.X > -0.5)
                Velocity.X = 0;
            Velocity.X *= 0.9f + decayAlteration;
        }
        protected void Jump()
        {
            Velocity.Y = -20;
            ChangeGameObjectState(GameObject_State.Air);
            ChangeAnimationState(Animation_State.jumpSquat);
            WaitForAnimation(Animations[ActiveAnimation]);
        }
        protected void Gravity()
        {
            if (Velocity.Y < 20 && GameObjectState != GameObject_State.Flying)
                Velocity.Y++;
        }
        public virtual void Respawn()
        {
            GameObjectState = GameObject_State.Air;
            PreviousAnimation = Animation_State.falling;
            ActiveAnimation = Animation_State.falling;
            Position = SpawnPoint;
            Active = true;
            Health = 1;
            Velocity = Vector2.Zero;
        }
        public bool SpriteCollision(AbstractGameObject go)
        {
            if (PositionRectangle.Intersects(go.PositionRectangle))
                return true;
            return false;
        }
        public virtual void LevelCollision(Level lvl, GameTime gt)
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
                    Color PixelColor = lvl.HitboxColor(newPositionRectangle.Left + i, newPositionRectangle.Bottom);
                    //If collision with ground. Snap to ground.
                    if (PixelColor == Color.Black)
                    {
                        Velocity.Y = 0;
                        ChangeGameObjectState(GameObject_State.onGround);
                        i = Hitbox.Width + 1;
                    }
                    //Collision with Lava
                    else if (PixelColor == Color.Red)
                    {
                        Velocity.Y = -15;
                        ChangeGameObjectState(GameObject_State.Air);
                        i = Hitbox.Width + 1;
                        ChangeHealth(-1, gt);
                    }
                    //Collision with bounce pad.
                    else if (PixelColor == Color.Green)
                    {
                        Velocity.Y = -30;
                        ChangeGameObjectState(GameObject_State.Air);
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
                        i = Hitbox.Width + 1;
                    }
                    else if (i > 1 && lvl.HitboxColor(newPositionRectangle.Right, newPositionRectangle.Bottom - i) == Color.Black)
                    {
                        Velocity.X = 0;
                        i = Hitbox.Width + 1;
                    }


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
                        Velocity.Y = -Math.Max(1, Math.Abs(Velocity.X));
                        ResetVelocityY = true;
                        i = Hitbox.Width + 1;

                    }
                    else if (i > 1 && lvl.HitboxColor(newPositionRectangle.Left, newPositionRectangle.Bottom - i) == Color.Black)
                    {
                        Velocity.X = 0;
                        i = Hitbox.Width + 1;
                    }
                }
            }

        }

    }
}
