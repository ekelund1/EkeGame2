using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using EkeGame2.SpawnableEffects;
using EkeGame2.UnitObjects;


namespace EkeGame2
{
    public class Level
    {
       // protected ContentManager Content;
        Texture2D Background;
        Texture2D Foreground;
        public Texture2D Hitbox;
        public Color[,] Hitbox_Colors;
        Texture2D PlayArea;
        private Enemy_Library TheEnemy_Library;

        private List<Enemy> EnemyList;
        private List<Platform> PlatformList;
        private List<Collectible> CollectibleList;

        public Vector2 PlayerSpawnPosition;
        private Goal TheGoal;
        Player RefThePlayer;
        
        public Level(ContentManager Content, int levelID)
        {
            Background = Content.Load<Texture2D>("Level"+levelID.ToString()+"/"+levelID.ToString() + "background");
            Hitbox = Content.Load<Texture2D>("Level" + levelID.ToString() + "/" + levelID.ToString() + "hitbox");
            PlayArea = Content.Load<Texture2D>("Level" + levelID.ToString() + "/" + levelID.ToString() + "playarea");
            
            LoadLevel(levelID,Content);       
        }

        private void LoadLevel(int levelID, ContentManager Content)
        {
            TheEnemy_Library = new Enemy_Library(Content);

            Color[] colors1D = new Color[Hitbox.Width * Hitbox.Height];
            Hitbox.GetData(colors1D);

            Color[,] colors2D = new Color[Hitbox.Width, Hitbox.Height];
            EnemyList = new List<Enemy>();
            PlatformList = new List<Platform>();
            CollectibleList = new List<Collectible>();

            for (int x = 0; x < Hitbox.Width; x++)
            {
                for (int y = 0; y < Hitbox.Height; y++)
                {
                    colors2D[x, y] = colors1D[x + y * Hitbox.Width];
                    if (colors2D[x, y] == Color.Purple)
                    {
                        //foreach purple pixel in hitbox image. 
                        //spawn new enemy at purple pixel position
                        EnemyList.Add(new Enemy(EnemyType.Purple, Content, "Enemy/Purple/", 15, new Vector2(x, y), y * 5));

                        //////////////////
                        ///Detta fungerade inte riktigt. Eftersom att alla objekten
                        ///som skapas får samma animationer. Inte en kopia utav animationer
                        ///utan exakt samma, med pekare till animationerna.
                        ///Iställer för egna versioner av dem.
                        ///
                        //Enemy enemy = TheEnemy_Library.GetEnemy(EnemyType.Purple);
                        //enemy.SetNewEnemyCopy_Properties(new Vector2(x, y), y * 5);
                        //EnemyList.Add(enemy);
                    }
                    else if (colors2D[x, y] == Color.Orange)
                    {
                        EnemyList.Add(new Enemy(EnemyType.Orange, Content, "Enemy/Orange/", 15, new Vector2(x, y), y * 5));
                    }
                    else if (colors2D[x, y] == Color.DarkGray) //169 169 169
                    {                        
                        EnemyList.Add(new Enemy(EnemyType.DarkGray, Content, "Enemy/DarkGray/", 15, new Vector2(x, y)));
                    }
                    else if (colors2D[x, y] == Color.DarkRed) //139 0 0
                    {
                        Enemy enemy = new Enemy(EnemyType.DarkRed, Content, "Enemy/DarkRed/", 15, new Vector2(x, y));
                        enemy.LoadAnimation(Content, "Enemy/DarkRed/", 8,8,8,8,8,8,8,8,8,8,8,1.5f);
                        
                        EnemyList.Add(enemy);
                    }
                    else if (colors2D[x, y] == Color.Yellow) //255 255 0
                        TheGoal = new Goal(Content, "Goal/", 15, new Vector2(x, y));
                    else if (colors2D[x, y] == Color.Blue) //0 0 255
                        PlatformList.Add(new Platform(Platform_Type.MOVING_PLATFORM_UPnDOWN, Content, "Platform/", new Vector2(x, y)));
                    else if (colors2D[x, y] == Color.CadetBlue) //95 158 160
                        PlatformList.Add(new Platform(Platform_Type.MOVING_PLATFORM_CIRCLE, Content, "Platform/", new Vector2(x, y)));
                    else if (colors2D[x, y] == Color.YellowGreen) //154 205 50 Collectible
                        CollectibleList.Add(new Collectible(Content, "Collectible/", new Vector2(x, y)));
                    else if (colors2D[x, y] == Color.Brown)
                        PlayerSpawnPosition = new Vector2(x, y);
                    
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
            foreach (Enemy e in EnemyList)
            { 
                if(e.Active)
                    e.DrawHitbox(s);
            }
            foreach (Platform p in PlatformList)
            {
                if (p.Active)
                    p.DrawHitbox(s);
            }
            foreach (Collectible c in CollectibleList)
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
        public void DrawLevel_LowestLayer(SpriteBatch lowest)
        {
            DrawBackground(lowest);
            DrawPlayArea(lowest);
        }
        public void DrawLevel_MiddleLayer(SpriteBatch middle)
        {
            DrawLevelGameObjects(middle);
        }
        public void DrawLevel_HighestLayer(SpriteBatch highest)
        {
        }

        public void Update(GameTime gt, ref Player ThePlayer)
        {
            foreach (Enemy e in EnemyList)
                e.Update(this, gt, ThePlayer);
            EnemyList.RemoveAll(effect => !effect.Active);

            foreach (Collectible c in CollectibleList)
            { 
                c.Update(this, gt);
                if (c.SpriteCollision(ThePlayer) && !c.IsCollected)
                {                    
                    ThePlayer.ChangeCollectibles(c.TriggerCollect(gt));                    
                }
            }
            foreach (Platform p in PlatformList)
            { 
                p.Update(this, gt);
                p.PlatformUnitCollision(ref ThePlayer);
            }

            if (this.LevelEnemyObjectCollision(ThePlayer))
            {
                ThePlayer.ChangeHealth(-1, gt);
                if (ThePlayer.Health > 0)
                    ThePlayer.InduceKnockback();
            }
            
            TheGoal.Update(this, gt);
            RefThePlayer = ThePlayer;
        }
        
        public void DrawLevelGameObjects(SpriteBatch s)
        {
            foreach (Enemy e in EnemyList)
                if(e.Active)
                    e.DrawGameObject(s);
            foreach (Platform p in PlatformList)
                if (p.Active)
                    p.DrawGameObject(s);
            foreach (Collectible c in CollectibleList)
                if (c.Active)
                    c.DrawGameObject(s);
            TheGoal.DrawGameObject(s);
        }

        public bool LevelGoalObjectCollision(ref Player player)
        {
            if (TheGoal.SpriteCollision(player))
            {
                TheGoal.TransferCollectible(ref player);
            }
            if (TheGoal.CurrentCollectibleAmount >= TheGoal.CollectibleRequirement)
                return true;
            return false;
        }
        public bool LevelEnemyObjectCollision(UnitObject player)
        {
            foreach (Enemy e in EnemyList)
            {
                if (e.Active && player.SpriteCollision(e))
                {
                    return true;
                }
            }
            return false;
        }
        
        public bool LevelProjectileObjectCollision(Projectile proj, GameTime gt)
        {
            if (proj.Owner == ProjectileOwner.PLAYER)
            {
                foreach (Enemy e in EnemyList)
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
                //Check collision with player. if collision. hurt player
                foreach (Enemy e in EnemyList)
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

        public void AddEnemy(EnemyType type, Vector2 position)
        {
            Enemy enemy = TheEnemy_Library.GetEnemy(type);
            enemy.SetNewEnemyCopy_Properties(position);
            EnemyList.Add(enemy);            
        }
    }
}
