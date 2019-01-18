using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;



namespace EkeGame2.Graphics
{
    class FPSCounter
    {
        private SpriteFont FPSCounterFont;
        private Vector2 AnchorPosition;
        private double FrameRate;
        private int FrameCounter;

        public FPSCounter(ContentManager c)
        {
            FPSCounterFont = c.Load<SpriteFont>("SuperMarioFont");
            AnchorPosition = new Vector2(1000, 70);
            FrameRate = 0;
        }
        public void Update(GameTime gt)
        {
            FrameRate = 1 / gt.TotalGameTime.TotalSeconds;
        }
        public void Draw(SpriteBatch s)
        {
            s.DrawString(FPSCounterFont, "FPS: " + FrameRate.ToString(), AnchorPosition, Color.Black);
            s.DrawString(FPSCounterFont, "FPS: " + FrameRate.ToString(), AnchorPosition +Vector2.One*3, Color.Wheat);
        }
    }
}
