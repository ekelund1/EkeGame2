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
    class GameHud
    {
        private HealthDisplay PlayerHealthDisplay;
        private CollectibleDisplay PlayerCollectibleDisplay;
        private FPSCounter PlayerFPSCounter;

        public GameHud(ContentManager c)
        {
            PlayerHealthDisplay = new HealthDisplay(c);
            PlayerCollectibleDisplay = new CollectibleDisplay(c);
            PlayerFPSCounter = new FPSCounter(c);
        }
        public void Draw(SpriteBatch s, GameTime gt)
        {
            PlayerHealthDisplay.Draw(s, gt);
            PlayerCollectibleDisplay.Draw(s);
           // PlayerFPSCounter.Draw(s);
        }
        public void Update(GameTime gt, Player ThePlayer, Level TheLvl)
        {
            PlayerHealthDisplay.Update(gt, ThePlayer, TheLvl);
            PlayerCollectibleDisplay.Update(gt, ThePlayer);
           // PlayerFPSCounter.Update(gt);
        }


    }
}
