
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace EkeGame2
{
    class Goal : UnitObject
    {
        public int CollectibleRequirement;
        public int CurrentCollectibleAmount;

        private SpriteFont GoalpriteFont;


        public Goal(ContentManager c, string objektName, int updateDelay, Vector2 spawnPosition) : base(c, objektName, updateDelay, spawnPosition)
        {
            GameObjectState = GameObject_State.Flying;
            Position = spawnPosition;
            PositionRectangle.Location = Position.ToPoint();
            LoadAnimation(c,objektName,8);
            Animations[ActiveAnimation].AnimationScale = new Vector2(2, 2);

            CollectibleRequirement = 75;
            CurrentCollectibleAmount = 0;

            GoalpriteFont = c.Load<SpriteFont>("SuperMarioFont");
            UpdateDelay = 15;
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
        public override void DrawGameObject(SpriteBatch s)
        {
            if (Active)
            {
                s.DrawString(GoalpriteFont, CurrentCollectibleAmount.ToString() + "/" + CollectibleRequirement, Position, Color.Black);
                s.DrawString(GoalpriteFont, CurrentCollectibleAmount.ToString() + "/" + CollectibleRequirement, Position+Vector2.One*3, Color.Beige);
                base.DrawGameObject(s);
            }
        }
        public void TransferCollectible(ref Player thePlayer)
        {
            if (thePlayer.CollectedAmount > 0)
            {
                thePlayer.ChangeCollectibles(-1);
                CurrentCollectibleAmount += 1;
            }
        }

    }
}
