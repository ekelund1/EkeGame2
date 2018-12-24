using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    class Goal : AbstractGameObject
    {
        public Goal(ContentManager c, string objektName, int updateDelay, Vector2 spawnPosition) : base(c, objektName, updateDelay, spawnPosition)
        {
            GameObjectState = GameObject_State.Flying;
            Position = spawnPosition;
            PositionRectangle.Location = Position.ToPoint();
            LoadAnimation(objektName,8);
        }
        public override void Update(Level lvl, GameTime gt)
        {            
            UpdateAnimations(gt);
        }

    }
}
