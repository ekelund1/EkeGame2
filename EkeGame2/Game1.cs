﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using EkeGame2.Graphics;
using EkeGame2.Audio;
using System;
using System.Collections.Generic;
using EkeGame2.SpawnableEffects;

namespace EkeGame2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch_LayerLowest;
        SpriteBatch spriteBatch_LayerMiddle;
        SpriteBatch spriteBatch_LayerHighest;
        SpriteBatch SpriteBatch_HUD;

        SoundPlayer TheSoundPlayer;
        SpawnableEffect_List TheSpawnableEffect_List;

        Level lvl;
        Player ThePlayer;
        bool drawHitboxes = false;
        Camera MyCamera;
        int CurrentLevel = 0;
        GameHud TheGameHud;

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
            graphics.PreferredBackBufferWidth = 1380;
            graphics.PreferredBackBufferHeight = 820;
            graphics.ApplyChanges();

            spriteBatch_LayerLowest = new SpriteBatch(GraphicsDevice);
            spriteBatch_LayerMiddle = new SpriteBatch(GraphicsDevice);
            spriteBatch_LayerHighest = new SpriteBatch(GraphicsDevice);

            SpriteBatch_HUD = new SpriteBatch(GraphicsDevice);

            lvl = new Level(Content, CurrentLevel);
            MyCamera = new Camera(GraphicsDevice.Viewport, lvl, ref ThePlayer);
            ThePlayer = new Player(Content, "Player/", 15, lvl.PlayerSpawnPosition);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.

            spriteBatch_LayerLowest = new SpriteBatch(GraphicsDevice);
            SpriteBatch_HUD = new SpriteBatch(GraphicsDevice);

            lvl = new Level(Content, CurrentLevel);

            ThePlayer = new Player(Content, "Player/", 15, lvl.PlayerSpawnPosition);

            MyCamera = new Camera(GraphicsDevice.Viewport, lvl, ref ThePlayer);

            ThePlayer.SetSpawnPosition(lvl.PlayerSpawnPosition);
            ThePlayer.Respawn();

            TheSoundPlayer = new SoundPlayer(Content);
            StaticSoundPlayer.SetSoundPlayer(ref TheSoundPlayer);

            TheSpawnableEffect_List = new SpawnableEffect_List(Content);
            StaticSpawnableEffect.SetSpawnableEffect_List(ref TheSpawnableEffect_List);

            TheGameHud = new GameHud(Content);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            MyCamera.UpdateCamera(GraphicsDevice.Viewport);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.T))
                drawHitboxes = !drawHitboxes;

            ThePlayer.Movement(gameTime,
                Keyboard.GetState().IsKeyDown(Keys.A),
                Keyboard.GetState().IsKeyDown(Keys.D),
                Keyboard.GetState().IsKeyDown(Keys.W),
                Keyboard.GetState().IsKeyDown(Keys.R),
                Keyboard.GetState().IsKeyDown(Keys.H),
                Keyboard.GetState().IsKeyDown(Keys.S),
                Keyboard.GetState().IsKeyDown(Keys.J));

            if(Keyboard.GetState().CapsLock && !graphics.IsFullScreen)
                graphics.ToggleFullScreen();
            ThePlayer.Update(lvl, gameTime);
            lvl.Update(gameTime, ref ThePlayer);
            
            //This one should probably check collision inside LevelUpdate. And then instead set a flag for going to the next level.
            if (lvl.LevelGoalObjectCollision(ref ThePlayer) ||Keyboard.GetState().IsKeyDown(Keys.L))
            {
                CurrentLevel++;
                this.UnloadContent();
                this.LoadContent();
            }
            
            TheSoundPlayer.Update();
            TheSpawnableEffect_List.Update(gameTime);
            TheGameHud.Update(gameTime, ThePlayer, lvl);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (drawHitboxes)
            {
                spriteBatch_LayerLowest.Begin(SpriteSortMode.Texture, null, null, null, null, null, MyCamera.Transform);

                lvl.DrawHitbox(spriteBatch_LayerLowest);
                ThePlayer.DrawHitbox(spriteBatch_LayerLowest);
                spriteBatch_LayerLowest.End();
            }
            else
            {
                spriteBatch_LayerLowest.Begin(SpriteSortMode.Texture,null,null,null,null,null,MyCamera.Transform);
                lvl.DrawLevel_LowestLayer(spriteBatch_LayerLowest);
                TheSpawnableEffect_List.DrawLowest(spriteBatch_LayerLowest);
                spriteBatch_LayerLowest.End();

                spriteBatch_LayerMiddle.Begin(SpriteSortMode.Texture, null, null, null, null, null, MyCamera.Transform);
                ThePlayer.DrawPlayer(spriteBatch_LayerMiddle);
                lvl.DrawLevelGameObjects(spriteBatch_LayerMiddle);
                spriteBatch_LayerMiddle.End();

                spriteBatch_LayerHighest.Begin(SpriteSortMode.Texture, null, null, null, null, null, MyCamera.Transform);
                lvl.DrawLevel_HighestLayer(spriteBatch_LayerHighest);
                TheSpawnableEffect_List.DrawHighest(spriteBatch_LayerHighest);
                spriteBatch_LayerHighest.End();



                SpriteBatch_HUD.Begin();
                TheGameHud.Draw(SpriteBatch_HUD, gameTime);                
                SpriteBatch_HUD.End();
            }
            

            base.Draw(gameTime);
        }
    }
}
