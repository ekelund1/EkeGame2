using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;


namespace EkeGame2
{
    class Platform
    {
        Vector2 Position;
        Vector2 Velocity;
        Vector2 SpawnPoint;

        Texture2D Hitbox;
        Rectangle PositionRectangle;
        Dictionary<Animation_State, Animation> Animations;

        int UpdateDelay;

        public Platform(ContentManager c, string objectName, Vector2 spawnPosition)
        {

        }
    }
}
