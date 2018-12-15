using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;


namespace EkeGame2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteBatch SpriteBatch_FG;
        Level lvl;
        GameObject player;
        bool drawHitboxes = false;
        Camera cam;
        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.PreferredBackBufferHeight = 820;
            graphics.PreferredBackBufferWidth = 1380;
            
            

            graphics.ApplyChanges();
            

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            SpriteBatch_FG = new SpriteBatch(GraphicsDevice);
            lvl = new Level(Content, 0);
            player = new GameObject(Content,"Player", 15);
            cam = new Camera(GraphicsDevice.Viewport, lvl, ref player);



            

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            cam.UpdateCamera(GraphicsDevice.Viewport);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.T))
                drawHitboxes = !drawHitboxes;

            player.Movement(
                Keyboard.GetState().IsKeyDown(Keys.A),
                Keyboard.GetState().IsKeyDown(Keys.D),
                Keyboard.GetState().IsKeyDown(Keys.W),
                Keyboard.GetState().IsKeyDown(Keys.R));

            if(Keyboard.GetState().CapsLock && !graphics.IsFullScreen)
                graphics.ToggleFullScreen();
            player.Update(lvl, gameTime);
            lvl.Update(gameTime);

            if (lvl.EnemyCollision(player))
                player.Respawn();

            /*if (player.SpriteCollision(lvl.))
                drawHitboxes = !drawHitboxes;*/

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (drawHitboxes)
            {
                spriteBatch.Begin(SpriteSortMode.Texture, null, null, null, null, null, cam.Transform);

                lvl.DrawHitbox(spriteBatch);
                player.DrawHitbox(spriteBatch);
                //enemy.DrawHitbox(spriteBatch);                
                spriteBatch.End();
            }
            else
            {
                //spriteBatch.Begin(SpriteSortMode.Texture,null,null,null,null,null,cam.Transform);
                
                lvl.DrawBackground(spriteBatch);                
                lvl.DrawPlayArea(spriteBatch);
                player.DrawGameObject(spriteBatch);
                lvl.DrawEnemies(spriteBatch);
                spriteBatch.End();

                SpriteBatch_FG.Begin();
                //lvl.DrawForeground(SpriteBatch_FG);
                SpriteBatch_FG.End();
            }
            

            
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
