using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;

namespace EkeGame2.Audio
{
    class MySoundEffect
    {
        public SoundEffect SoundEffect;
        float Volume, Pitch, Pan;

        public MySoundEffect(SoundEffect soundEffect, float volume = 1, float pitch = 0, float pan = 0)
        {
            SoundEffect = soundEffect;
            Volume = volume;
            Pitch = pitch;
            Pan = pan;
        }

        public void Play()
        {
            
            SoundEffect.Play(Volume, Pitch, Pan);
        }
    }
}
