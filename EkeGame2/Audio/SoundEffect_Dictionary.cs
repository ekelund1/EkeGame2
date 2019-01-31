using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;



namespace EkeGame2.Audio
{
    class SoundEffect_Dictionary
    {
        static Dictionary<SoundEffects,SoundEffect> SoundEffectDictionary;
        
        public SoundEffect_Dictionary(ContentManager content)
        {
            SoundEffectDictionary = new Dictionary<SoundEffects, SoundEffect>();

            var allSounds = (SoundEffects[])Enum.GetValues(typeof(SoundEffects));
            foreach (var sound in allSounds)
            {
                SoundEffectDictionary[sound] = content.Load<SoundEffect>("SoundEffects/"+sound.ToString());
            }
        }
        public SoundEffect GetSound(SoundEffects getSound)
        {
            return SoundEffectDictionary[getSound];
        }
    }
}
