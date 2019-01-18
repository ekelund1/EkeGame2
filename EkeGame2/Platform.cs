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
    class Platform : AbstractGameObject
    {
        Color PlatformTypeColor;
        Platform_Type PlatformType;
        Platform_State PlatformState;

        public Platform(Platform_Type platformType, ContentManager c, string objectName, Vector2 spawnPosition) : base(c, objectName, 15, spawnPosition)
        {
            PlatformType = platformType;
            GoingRight = true;

            UpdatePositionRectangle();
            switch (platformType)
            {
                case Platform_Type.MOVING_PLATFORM_UPnDOWN:
                    PlatformTypeColor = Color.Blue;
                    break;
                case Platform_Type.MOVING_PLATFORM_LEFTnRIGHT:
                    PlatformTypeColor = Color.CadetBlue;
                    break;
            }


            var animationImage = c.Load<Texture2D>(objectName + "/idle");
            Animations[Animation_State.idle] = new Animation(Position, new Vector2(1, 1), Vector2.One, 15);
            Animations[Animation_State.idle].AnimationImage = animationImage;

        }

        public override void Update(Level lvl, GameTime gt)
        {
            if (Active)
                UpdateCounter += (int)gt.ElapsedGameTime.Milliseconds;

            if (UpdateCounter >= UpdateDelay)
            {
                UpdateCounter = 0;
                switch (PlatformType)
                {
                    case Platform_Type.MOVING_PLATFORM_CIRCLE:
                    case Platform_Type.MOVING_PLATFORM_UPnDOWN:
                    case Platform_Type.MOVING_PLATFORM_LEFTnRIGHT:
                        MovingPlatformBehaviour(gt);
                        Position += Velocity;
                        UpdatePositionRectangle();
                        break;
                }
            }
            UpdateAnimations(gt);
        }
        public void PlatformUnitCollision(ref Player playerObject)
        {
            if (this.SpriteCollision(playerObject) && playerObject.GetVelocity().Y >= 0)
            {
                playerObject.Position.Y = this.PositionRectangle.Top- playerObject.Hitbox.Height/2;
                playerObject.Velocity.Y = 0;
                playerObject.Position.X += this.Velocity.X;
                playerObject.ChangeGameObjectState(GameObject_State.onGround);
            }
        }
        public void PlatformUnitCollision(ref Enemy enemyObject)
        {
            if (this.SpriteCollision(enemyObject) && enemyObject.GetVelocity().Y >= 0)
            {
                enemyObject.Position.Y = this.PositionRectangle.Top - enemyObject.Hitbox.Height;
                enemyObject.Velocity.Y = 0;
                enemyObject.Position.X = this.Velocity.X;
                enemyObject.ChangeGameObjectState(GameObject_State.onGround);
                
            }
        }
        private void MovingPlatformBehaviour(GameTime gt)
        {

            if (PlatformState == Platform_State.MOVING)
            {
                if (PlatformType == Platform_Type.MOVING_PLATFORM_UPnDOWN)
                    Velocity.Y = (float)Math.Sin(gt.TotalGameTime.TotalSeconds) * 1.5f;
                else if (PlatformType == Platform_Type.MOVING_PLATFORM_LEFTnRIGHT)
                    Velocity.X = (float)Math.Sin(gt.TotalGameTime.TotalSeconds) * 1.5f;
                else if (PlatformType == Platform_Type.MOVING_PLATFORM_CIRCLE)
                {
                    Velocity.X = (float)Math.Cos(gt.TotalGameTime.TotalSeconds) * 1.5f;
                    Velocity.Y = (float)Math.Sin(gt.TotalGameTime.TotalSeconds) * 1.5f;
                }

            }
            else if (PlatformState == Platform_State.PAUSE)
                Velocity = Vector2.Zero;
        }
        public void ChangePlatformState(Platform_State pt)
        {
            PlatformState = pt;
        }
    }
}
