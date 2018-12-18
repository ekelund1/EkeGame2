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
        public Vector2 PlayerSpawnPosition;
        

        //Constructor
        public Level(ContentManager g, int id)
        {
            Content = g;
            Background = Content.Load<Texture2D>("Level"+id.ToString()+"/"+id.ToString() + "background");
            Foreground = Content.Load<Texture2D>("Level" + id.ToString() + "/" + id.ToString() + "foreground");
            Hitbox = Content.Load<Texture2D>("Level" + id.ToString() + "/" + id.ToString() + "hitbox");
            PlayArea = Content.Load<Texture2D>("Level" + id.ToString() + "/" + id.ToString() + "playarea");

            Color[] colors1D = new Color[Hitbox.Width * Hitbox.Height];
            Hitbox.GetData(colors1D);

            Color[,] colors2D = new Color[Hitbox.Width, Hitbox.Height];
            EnemyStack = new Stack<Enemy>();
            for (int x = 0; x < Hitbox.Width; x++)
            {
                for (int y = 0; y < Hitbox.Height; y++)
                {
                    colors2D[x, y] = colors1D[x + y * Hitbox.Width];
                    if (colors2D[x, y] == Color.Purple)
                    {
                        //foreach purple pixel in hitbox image. 
                        //spawn new enemy at purple pixel position
                        EnemyStack.Push(new Enemy(EnemyType.Purple, Content, "Enemy", 15, new Vector2(x, y), y * 5));
                    }
                    if (colors2D[x, y] == Color.Brown)
                    {
                         PlayerSpawnPosition = new Vector2(x, y);
                    }

                }
            }
            Hitbox_Colors = colors2D;
        }
        public Texture2D GetHitbox()
        {
            return Hitbox;
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
            s.Draw(Hitbox, Vector2.Zero);
        }
        public void DrawForeground(SpriteBatch s)
        {
            s.Draw(Foreground, Vector2.Zero);
        }
        public void Update(GameTime gt)
        {
            foreach (Enemy e in EnemyStack)
                e.EnemyUpdate(this, gt);            
        }
        public void DrawEnemies(SpriteBatch s)
        {
            foreach (Enemy e in EnemyStack)
                if(e.Active)
                    e.DrawGameObject(s);
        }
        

        public bool LevelGameObjectCollision(AbstractGameObject player)
        {
            foreach (Enemy e in EnemyStack)
            {
                if (e.Active && player.SpriteCollision(e))
                    return true;
            }
            return false;
        }
        public bool LevelGameObjectCollision(Projectile proj)
        {
            if (proj.Owner == ProjectileOwner.PLAYER)
            {
                foreach (Enemy e in EnemyStack)
                {
                    if (e.Active && proj.SpriteCollision(e))
                    {
                        e.Kill();
                        return true;
                    }
                }
            }
            else if (proj.Owner == ProjectileOwner.ENEMY)
            {
                foreach (Enemy e in EnemyStack)
                {
                    if (e.Active && proj.SpriteCollision(e))
                    {                        
                        return true;
                    }
                }
            }
            return false;
        }
        
    }
}
