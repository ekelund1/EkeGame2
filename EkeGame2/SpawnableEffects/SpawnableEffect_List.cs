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
        List<SpawnableEffect> TheSpawnableEffectList { get; set; }
        private static SpawnableEffect_Library SpawnableEffect_Library;

        public SpawnableEffect_List(ContentManager content)
        {
            TheSpawnableEffectList = new List<SpawnableEffect>();
            SpawnableEffect_Library = new SpawnableEffect_Library(content);
        }
        public void Update(GameTime gt)
        {
            foreach (SpawnableEffect se in TheSpawnableEffectList)
            {
                if (se.Active)
                    se.Update(gt);
            }
            TheSpawnableEffectList.RemoveAll(se => !se.Active);            
        }
        public void DrawLowest(SpriteBatch spriteBatch)
        {
            foreach (SpawnableEffect se in TheSpawnableEffectList)
            {
                if (se.Active && se.OnLowestLayer)
                    se.Draw(spriteBatch);
            }
        }
        public void DrawHighest(SpriteBatch spriteBatch)
        {
            foreach (SpawnableEffect se in TheSpawnableEffectList)
            {
                if (se.Active && !se.OnLowestLayer)
                    se.Draw(spriteBatch);
            }
        }
        public void AddSpawnableEffect(SpawnableEffect_Type type, Vector2 position, int timer, double extraScale = 0)
        {
            var effect = SpawnableEffect_Library.GetSpawnableEffect(type);
            effect.SpawnCopyOfEffect(position, timer, true, false, 0, 0, extraScale);
            TheSpawnableEffectList.Add(effect);
        }
        public void AddSpawnableEffect(SpawnableEffect_Type type, Vector2 position, int timer, bool onLowestLayer = true, bool randomPosition = false, float velocityX = 0, float velocityY = 0, double extraScale = 0)
        {
            var effect = SpawnableEffect_Library.GetSpawnableEffect(type);
            effect.SpawnCopyOfEffect(position, timer, onLowestLayer, randomPosition, velocityX, velocityY, extraScale);
            TheSpawnableEffectList.Add(effect);
        }
    }
}
