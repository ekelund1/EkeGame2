using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace EkeGame2.UnitObjects
{
    class Enemy_Library
    {
        public Dictionary<EnemyType, Enemy> TheEnemyLibrary;

        public Enemy_Library(ContentManager content)
        {
            TheEnemyLibrary = new Dictionary<EnemyType, Enemy>();

            var allEnemyTypes = (EnemyType[])Enum.GetValues(typeof(EnemyType));
            foreach (var enemyType in allEnemyTypes)
            {
                TheEnemyLibrary[enemyType] = new Enemy(enemyType, content, "Enemy/" + enemyType.ToString()+"/", 15, new Vector2(-1000 - 1000));
                TheEnemyLibrary[enemyType].Active = false;
            }
        }
        public Enemy GetEnemy(EnemyType type)
        {
            return TheEnemyLibrary[type].GetCopy();
        }
    }
}
