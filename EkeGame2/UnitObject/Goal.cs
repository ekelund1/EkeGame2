
using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Content;

namespace EkeGame2
{
    class Goal : UnitObject
    {
        public Goal(ContentManager c, string objektName, int updateDelay, Vector2 spawnPosition) : base(c, objektName, updateDelay, spawnPosition)
        {
            GameObjectState = GameObject_State.Flying;
            Position = spawnPosition;
            PositionRectangle.Location = Position.ToPoint();
            LoadAnimation(c,objektName,8);
            Animations[ActiveAnimation].AnimationScale = new Vector2(2, 2);
            UpdateDelay = 200;
        }
        public override void Update(Level lvl, GameTime gt)
        {
            if (Active)
                UpdateCounter += (int)gt.ElapsedGameTime.Milliseconds;
            if (UpdateCounter > UpdateDelay)
            {
                lvl.AddSpawnableEffect(SpawnableEffect_Type.TINY_STARS, Position, 50, false);
                UpdatePositionRectangle();
                UpdateAnimations(gt);
                UpdateCounter = 0;
            }
        }

    }
}
