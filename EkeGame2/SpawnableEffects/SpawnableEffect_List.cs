using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace EkeGame2.SpawnableEffects
{
    public class SpawnableEffect_List
    {
        public List<SpawnableEffect> TheSpawnableEffectList { get; set; }
        public SpawnableEffect_List()
        {
            TheSpawnableEffectList = new List<SpawnableEffect>();
        }
        public void Update(GameTime gt)
        {
            foreach (SpawnableEffect se in TheSpawnableEffectList)
            {
                if (se.Active)
                    se.Update(gt);
                else
                    TheSpawnableEffectList.Remove(se);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (SpawnableEffect se in TheSpawnableEffectList)
            {
                if (se.Active)
                    se.Draw(spriteBatch);
            }
        }

    }
}
