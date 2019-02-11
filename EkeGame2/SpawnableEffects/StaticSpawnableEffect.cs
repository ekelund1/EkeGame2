using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;


namespace EkeGame2.SpawnableEffects
{
    class StaticSpawnableEffect
    {
        private static SpawnableEffect_List SpawnableEffect_List;

        public static void SetSpawnableEffect_List(ref SpawnableEffect_List spawnableEffect_List)
        {
            SpawnableEffect_List = spawnableEffect_List;
        }
        public static void AddSpawnableEffect(SpawnableEffect_Type type, Vector2 position, int timer, double extraScale = 0)
        {
            SpawnableEffect_List.AddSpawnableEffect(type, position, timer, extraScale);
        }
        public static void AddSpawnableEffect(SpawnableEffect_Type type, Vector2 position, int timer, bool onLowestLayer = true, bool randomPosition = false, float velocityX = 0, float velocityY = 0, double extraScale = 0)
        {
            SpawnableEffect_List.AddSpawnableEffect(type, position, timer, onLowestLayer, randomPosition, velocityX, velocityY, extraScale);
        }
    }
}
