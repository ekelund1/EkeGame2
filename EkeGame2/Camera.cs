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
    public class Camera
    {
        public float Zoom { get; set; }
        public Vector2 Position { get; set; }
        public Rectangle Bounds { get; protected set; }
        public Rectangle VisibleArea { get; protected set; }
        public Matrix Transform { get; protected set; }

        private Player PlayerObject;

        private float currentMouseWheelValue, previousMouseWheelValue, zoom, previousZoom, maxX,minX,maxY,minY;

        public Camera(Viewport viewport, Level lvl, ref Player player)
        {
            Bounds = viewport.Bounds;
            Zoom = 1f;
            Position = Vector2.Zero;
            PlayerObject = player;


            minX = 700;
            minY = 400;

            maxX = lvl.Hitbox.Width - minX;            
            maxY = lvl.Hitbox.Height - minY;
            
        }


        private void UpdateVisibleArea()
        {
            var inverseViewMatrix = Matrix.Invert(Transform);

            var tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
            var tr = Vector2.Transform(new Vector2(Bounds.X, 0), inverseViewMatrix);
            var bl = Vector2.Transform(new Vector2(0, Bounds.Y), inverseViewMatrix);
            var br = Vector2.Transform(new Vector2(Bounds.Width, Bounds.Height), inverseViewMatrix);

            var min = new Vector2(
                MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
                MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));
            var max = new Vector2(
                MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
                MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));
            VisibleArea = new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

        private void UpdateMatrix()
        {
            Transform = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                    Matrix.CreateScale(Zoom) *
                    Matrix.CreateTranslation(new Vector3(Bounds.Width * 0.5f, Bounds.Height * 0.5f, 0));
            UpdateVisibleArea();
        }

        public void MoveCamera(Vector2 movePosition)
        {
            
            Vector2 newPosition = Position + movePosition;
            Position = newPosition;
        }

        public void AdjustZoom(float zoomAmount)
        {
            Zoom += zoomAmount;
            if (Zoom < .35f)
            {
                Zoom = .35f;
            }
            if (Zoom > 2f)
            {
                Zoom = 2f;
            }
        }
        private void CameraLogic()
        {
            if (Position.X < minX)
                Position = new Vector2(minX, Position.Y);
            else if(Position.X > maxX)
                Position = new Vector2(maxX, Position.Y);
            if(Position.Y < minY)
                Position = new Vector2(Position.X, minY);
            else if(Position.Y > maxY)
                Position = new Vector2(Position.X, maxY);
        }
        public void UpdateCamera(Viewport bounds)
        {
            Bounds = bounds.Bounds;
            UpdateMatrix();

            Vector2 cameraMovement = Vector2.Zero;
            int moveSpeed;

            if (Zoom > .8f)
            {
                moveSpeed = 10;
            }
            else if (Zoom < .8f && Zoom >= .6f)
            {
                moveSpeed = 15;
            }
            else if (Zoom < .6f && Zoom > .35f)
            {
                moveSpeed = 20;
            }
            else if (Zoom <= .35f)
            {
                moveSpeed = 25;
            }
            else
            {
                moveSpeed = 10;
            }

            
            
                
            
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                cameraMovement.Y = -moveSpeed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                cameraMovement.Y = moveSpeed;
            }            
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                cameraMovement.X = -moveSpeed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                cameraMovement.X = moveSpeed;
            }

            previousMouseWheelValue = currentMouseWheelValue;
            currentMouseWheelValue = Mouse.GetState().ScrollWheelValue;

            if (currentMouseWheelValue > previousMouseWheelValue)
            {
                AdjustZoom(.05f);
                Console.WriteLine(moveSpeed);
            }

            if (currentMouseWheelValue < previousMouseWheelValue)
            {
                AdjustZoom(-.05f);
                Console.WriteLine(moveSpeed);
            }

            previousZoom = zoom;
            zoom = Zoom;
            if (previousZoom != zoom)
            {
                Console.WriteLine(zoom);
            }

            

            ///////////////////
            ///Follow player///
            ///////////////////
            Vector2 PlayerPos = Vector2.Transform(PlayerObject.GetPosition, this.Transform);
            if (PlayerPos.X > 700)
                cameraMovement.X = moveSpeed;
            else if (PlayerPos.X < 600)
                cameraMovement.X = -moveSpeed;
            if (PlayerPos.Y < 400)
                cameraMovement.Y = -moveSpeed;
            else if (PlayerPos.Y > 600)
                cameraMovement.Y = moveSpeed;
            CameraLogic();

            MoveCamera(cameraMovement);
        }
        private void FollowPlayer()
        {

        }
    }
}
