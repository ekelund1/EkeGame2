using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkeGame2.Audio
{
    static class StaticSoundPlayer
    {
        private static SoundPlayer soundPlayer;

        public static void SetSoundPlayer(ref SoundPlayer soundplayer)
        {
            soundPlayer = soundplayer;
        }
        public static void PlaySound(SoundEffects playThis, float volume = 1, float pitch = 0, float pan = 0)
        {
            soundPlayer.PlaySound(playThis,volume,pitch,pan);
        }
    }
}
