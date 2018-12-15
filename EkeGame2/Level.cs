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
                        EnemyStack.Push(new Enemy(Content, "Enemy", 15, new Vector2(x, y), y * 5));
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
                e.DrawHitbox(s);
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
                e.DrawGameObject(s);
        }
        public bool CheckCollsion(GameObject go)
        {
            if (Hitbox_Colors[(int)go.GetX, (int)go.getY] == Color.Black)
                return true;
            return false;
        }

        public bool EnemyCollision(GameObject player)
        {
            foreach (Enemy e in EnemyStack)
            {
                if (player.SpriteCollision(e))
                    return true;
            }
            return false;
        }
        public int WallSnapping(Vector2 pos, Vector2 vel)
        {
            if (vel.X > 0)
            {
                for (int i = (int)vel.X; i > 0 ; i--)
                {                    
                    if (Hitbox_Colors[(int)pos.X+i, (int)pos.Y] != Color.Black)
                        return i;
                }
            }
            else
            {
                for (int i = (int)vel.X; i < 0; i++)
                {
                    if (Hitbox_Colors[(int)pos.X+i, (int)pos.Y] != Color.Black)
                        return i;
                }
            }
            return 0;
        }
        public int FloorSnapping(Vector2 pos, Vector2 vel)
        {
            if (vel.Y > 0)
            {
                for (int i = (int)vel.Y; i > 0 ; i--)
                {
                    if (Hitbox_Colors[(int)pos.X, (int)pos.Y+i] != Color.Black)
                        return i;
                }
            }
            else
            {
                for (int i = (int)vel.Y; i < 0; i++)
                {
                    if (Hitbox_Colors[(int)pos.X, (int)pos.Y+i] != Color.Black)
                        return i;
                }
            }
            
            return 0;
        }
    }
}
