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
    public class Level
    {
        protected ContentManager Content;
        Texture2D Background;
        Texture2D Foreground;
        public Texture2D Hitbox;
        public Color[,] Hitbox_Colors;
        Texture2D PlayArea;

        private Stack<Enemy> EnemyStack;
        private Stack<Platform> PlatformStack;
        private Stack<Collectible> CollectibleStack;

        public Vector2 PlayerSpawnPosition;
        private Goal TheGoal;
        Player RefThePlayer;
        
        public Level(ContentManager g, int levelID)
        {
            Content = g;
            Background = Content.Load<Texture2D>("Level"+levelID.ToString()+"/"+levelID.ToString() + "background");
            //Foreground = Content.Load<Texture2D>("Level" + id.ToString() + "/" + id.ToString() + "foreground");
            Hitbox = Content.Load<Texture2D>("Level" + levelID.ToString() + "/" + levelID.ToString() + "hitbox");
            PlayArea = Content.Load<Texture2D>("Level" + levelID.ToString() + "/" + levelID.ToString() + "playarea");
            
            LoadLevel(levelID);            
        }

        private void LoadLevel(int levelID)
        {
            Color[] colors1D = new Color[Hitbox.Width * Hitbox.Height];
            Hitbox.GetData(colors1D);

            Color[,] colors2D = new Color[Hitbox.Width, Hitbox.Height];
            EnemyStack = new Stack<Enemy>();
            PlatformStack = new Stack<Platform>();
            CollectibleStack = new Stack<Collectible>();

            for (int x = 0; x < Hitbox.Width; x++)
            {
                for (int y = 0; y < Hitbox.Height; y++)
                {
                    colors2D[x, y] = colors1D[x + y * Hitbox.Width];
                    if (colors2D[x, y] == Color.Purple)
                    {
                        //foreach purple pixel in hitbox image. 
                        //spawn new enemy at purple pixel position
                        EnemyStack.Push(new Enemy(EnemyType.Purple, Content, "Enemy/Purple/", 15, new Vector2(x, y), y * 5));
                    }
                    else if (colors2D[x, y] == Color.Orange)
                    {
                        EnemyStack.Push(new Enemy(EnemyType.Orange, Content, "Enemy/Orange/", 15, new Vector2(x, y), y * 5));
                    }
                    else if (colors2D[x, y] == Color.Brown)
                    {
                        PlayerSpawnPosition = new Vector2(x, y);
                    }
                    else if (colors2D[x, y] == Color.Yellow) //255 255 0
                        TheGoal = new Goal(Content, "Goal", 15, new Vector2(x, y));
                    else if (colors2D[x, y] == Color.Blue) //0 0 255
                        PlatformStack.Push(new Platform(Platform_Type.MOVING_PLATFORM_UPnDOWN, Content, "Platform", new Vector2(x, y)));
                    else if (colors2D[x, y] == Color.CadetBlue) //95 158 160
                        PlatformStack.Push(new Platform(Platform_Type.MOVING_PLATFORM_CIRCLE, Content, "Platform", new Vector2(x, y)));
                    else if (colors2D[x, y] == Color.YellowGreen) //154 205 50 Collectible
                        CollectibleStack.Push(new Collectible(Content, "Collectible", new Vector2(x, y)));

                }
            }
            Hitbox_Colors = colors2D;
        }
        public Texture2D GetHitbox()
        {
            return Hitbox;
        }
        public Color HitboxColor(int x, int y)
        {
            if (x >= 0 && x < Hitbox.Width && y >= 0 && y < Hitbox.Height)
                return Hitbox_Colors[x, y];
            return Color.Black;
        }
        public Color HitboxColor(Point objectPoint)
        {
            if(objectPoint.X>=0 && objectPoint.X<Hitbox.Width && objectPoint.Y>=0 && objectPoint.Y<Hitbox.Height)
                return Hitbox_Colors[objectPoint.X, objectPoint.Y];
            return Color.Black;
        }
        public Color HitboxColor(Vector2 objectPoint)
        {
            if (objectPoint.X >= 0 && objectPoint.X < Hitbox.Width && objectPoint.Y >= 0 && objectPoint.Y < Hitbox.Height)
                return Hitbox_Colors[(int)objectPoint.X, (int)objectPoint.Y];
            return Color.Black;
        }
        public void DrawBackground(SpriteBatch s)
        {
            s.Draw(Background, Vector2.Zero);
        }
        public void DrawPlayArea(SpriteBatch s)
        {
            s.Draw(PlayArea, Vector2.Zero);
        }
        public void DrawHitbox(SpriteBatch s)
        {
            foreach (Enemy e in EnemyStack)
            { 
                if(e.Active)
                    e.DrawHitbox(s);
            }
            foreach (Platform p in PlatformStack)
            {
                if (p.Active)
                    p.DrawHitbox(s);
            }
            foreach (Collectible c in CollectibleStack)
            {
                if (c.Active)
                    c.DrawHitbox(s);
            }
            TheGoal.DrawHitbox(s);
            s.Draw(Hitbox, Vector2.Zero);
        }
        public void DrawForeground(SpriteBatch s)
        {
            s.Draw(Foreground, Vector2.Zero);
        }
        public void Update(GameTime gt, ref Player ThePlayer)
        {
            foreach (Enemy e in EnemyStack)
                e.Update(this, gt, ThePlayer);
            foreach (Collectible c in CollectibleStack)
            { 
                c.Update(this, gt);
                if (c.SpriteCollision(ThePlayer) && !c.IsCollected)
                {                    
                    ThePlayer.ChangeCollectibles(c.TriggerCollect(gt));
                }
            }
            foreach (Platform p in PlatformStack)
            { 
                p.Update(this, gt);
                p.PlatformUnitCollision(ref ThePlayer);
            }
            if (this.LevelEnemyObjectCollision(ThePlayer))
                ThePlayer.ChangeHealth(-1, gt);
            

            TheGoal.Update(this, gt);
            RefThePlayer = ThePlayer;
        }
        
        public void DrawLevelGameObjects(SpriteBatch s)
        {
            foreach (Enemy e in EnemyStack)
                if(e.Active)
                    e.DrawGameObject(s);
            foreach (Platform p in PlatformStack)
                if (p.Active)
                    p.DrawGameObject(s);
            foreach (Collectible c in CollectibleStack)
                if (c.Active)
                    c.DrawGameObject(s);
            TheGoal.DrawGameObject(s);
        }

        public bool LevelGoalObjectCollision(UnitObject player)
        {
            if (TheGoal.SpriteCollision(player))
                return true;
            return false;
        }
        public bool LevelEnemyObjectCollision(UnitObject player)
        {
            foreach (Enemy e in EnemyStack)
            {
                if (e.Active && player.SpriteCollision(e))
                    return true;
            }
            return false;
        }
        
        public bool LevelProjectileObjectCollision(Projectile proj, GameTime gt)
        {
            if (proj.Owner == ProjectileOwner.PLAYER)
            {
                foreach (Enemy e in EnemyStack)
                {
                    if (e.Active && e.Health>0 && proj.SpriteCollision(e))
                    {
                        e.SetVelocity(proj.GetVelocity()/2);
                        e.ChangeHealth(-1,gt);
                        return true;
                    }
                }
            }
            else if (proj.Owner == ProjectileOwner.ENEMY)
            {
                //Check collision with player. if collision. Kill player
                foreach (Enemy e in EnemyStack)
                {
                    if (e.Active && e.EnemyProjectile.Active && e.Health > 0 
                        && e.EnemyProjectile.SpriteCollision(RefThePlayer))
                    {
                        RefThePlayer.SetVelocity(proj.GetVelocity() / 2);
                        RefThePlayer.ChangeHealth(-1,gt);
                        return true;
                    }
                }
            }
            return false;
        }
        public void ChangeHitboxColorGrid(Rectangle objectPositionRectangle, Color newColor, Color oldColor)
        {
            for (int i = 0; i < objectPositionRectangle.Width; i++)
            {
                for (int j = 0; j < objectPositionRectangle.Height; j++)
                {
                    if (Hitbox_Colors[objectPositionRectangle.Location.X + i, objectPositionRectangle.Location.Y + j] == oldColor)
                        Hitbox_Colors[objectPositionRectangle.Location.X + i, objectPositionRectangle.Location.Y + j] = newColor;
                }
            }
        }
    }
}
