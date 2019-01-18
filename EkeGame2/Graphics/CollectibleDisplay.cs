using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace EkeGame2.Graphics
{
    class CollectibleDisplay
    {
        bool Active;
        int UpdateDelay, UpdateCounter;
        private Vector2 AnchorPosition;
        private Vector2 TextOffset;
        private Animation CollectibleIcon;
        private SpriteFont CollectibleSpriteFont;

        private double DisplayScale;

        private int CollectedAmount;

        public CollectibleDisplay(ContentManager c)
        {
            AnchorPosition = new Vector2(100, 70);
            TextOffset = new Vector2(15, 0);
            UpdateCounter = 200;

            var animationImage = c.Load<Texture2D>("Collectible/idle");
            CollectibleIcon = new Animation(AnchorPosition, new Vector2(8, 1), Vector2.One, 200);
            CollectibleIcon.AnimationImage = animationImage;

            CollectibleSpriteFont = c.Load<SpriteFont>("SuperMarioFont");
        }
        public void Update(GameTime gt, Player ThePlayer)
        {
            if (Active)
                UpdateCounter += (int)gt.ElapsedGameTime.TotalMilliseconds;
            if (UpdateCounter >= UpdateDelay)
            {
                UpdateCounter = 0;
                if (CollectedAmount < ThePlayer.CollectedAmount)
                {
                    CollectedAmount++;
                    DisplayScale = 3.5;
                }
                if (DisplayScale > 1)
                    DisplayScale *= 0.85;
                else
                    DisplayScale = 1;
            }
            CollectibleIcon.Update(gt, Animation_State.idle, true);
        }
        public void Draw(SpriteBatch s)
        {
            CollectibleIcon.Draw(s, AnchorPosition,0,DisplayScale);

            s.DrawString(CollectibleSpriteFont, "x " + CollectedAmount.ToString(), AnchorPosition + TextOffset,Color.Black,0,Vector2.Zero,(float)DisplayScale,SpriteEffects.None,0);
            s.DrawString(CollectibleSpriteFont, "x " + CollectedAmount.ToString(), AnchorPosition + TextOffset+Vector2.One*3,Color.Wheat, 0, Vector2.Zero, (float)DisplayScale, SpriteEffects.None, 0);


        }
    }
}
