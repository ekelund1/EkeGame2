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
    public class SpawnableEffect_Library
    {
        public Dictionary<SpawnableEffect_Type, SpawnableEffect> TheSpawnableEffect_Library;

        public SpawnableEffect_Library(ContentManager content)
        {
            TheSpawnableEffect_Library = new Dictionary<SpawnableEffect_Type, SpawnableEffect>();

            var allEffectTypes = (SpawnableEffect_Type[])Enum.GetValues(typeof(SpawnableEffect_Type));
            foreach (var effectType in allEffectTypes)
            {
                TheSpawnableEffect_Library[effectType] = new SpawnableEffect(content, effectType);
            }
        }
        public SpawnableEffect GetSpawnableEffect(SpawnableEffect_Type type)
        {
            return TheSpawnableEffect_Library[type];
        }
    }
}
