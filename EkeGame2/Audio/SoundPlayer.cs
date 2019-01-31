using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace EkeGame2.Audio
{
    public class SoundPlayer
    {
        private static SoundEffect_Dictionary SoundEffect_Dictionary;
        List<MySoundEffect> PlayQueue;

        public SoundPlayer(ContentManager content)
        {
            SoundEffect_Dictionary = new SoundEffect_Dictionary(content);
            PlayQueue = new List<MySoundEffect>();
        }
        public void PlaySound(SoundEffects playThis, float volume = 1, float pitch = 0, float pan = 0)
        {
            PlayQueue.Add(new MySoundEffect(SoundEffect_Dictionary.GetSound(playThis), volume, pitch, pan));
        }
        public void Update()
        {
            foreach (MySoundEffect sound in PlayQueue)
            {
                sound.Play();
            }
            PlayQueue.RemoveAll(sound => true);
        }

    }
}
    
