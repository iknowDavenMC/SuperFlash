using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace COMP476Proj
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public const int SCREEN_WIDTH = 800;
        public const int SCREEN_HEIGHT = 600;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont; 
        World world;
        public HUD hud;  

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
            // TODO: Add your initialization logic here

            InputManager.GetInstance(InputManager.ControllerType.Keyboard);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.ApplyChanges();
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>("Fonts/Game Over");
            //Populate Sprite Database
            Texture2D streaker = Content.Load<Texture2D>("streaker");
            Texture2D level1 = Content.Load<Texture2D>("level1");
            //Hud elements
            Texture2D banner = Content.Load<Texture2D>("Hud/banner");
            Texture2D notorietyBar = Content.Load<Texture2D>("Hud/notorietyBar");
            Texture2D notorietyMeter = Content.Load<Texture2D>("Hud/notorietyMeter");
            //Animation elements 
            SpriteDatabase.AddAnimation(new Animation("streaker_static", streaker, 2, 143, 184, 0));
            SpriteDatabase.AddAnimation(new Animation("streaker_walk", streaker, 5, 143, 184, 184));
            SpriteDatabase.AddAnimation(new Animation("streaker_fall", streaker, 7, 143, 184, 368));
            SpriteDatabase.AddAnimation(new Animation("streaker_getup", streaker, 7, 143, 184, 552));
            SpriteDatabase.AddAnimation(new Animation("streaker_dance", streaker, 5, 143, 184, 736));
            SpriteDatabase.AddAnimation(new Animation("level1",level1));


            //Create World
            world = new World();

            Debugger.getInstance();
            //Hud
            hud = new HUD(this, spriteBatch, spriteFont, banner, notorietyBar, notorietyMeter);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            InputManager.GetInstance().Update(gameTime);

            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();
            //Debugger.getInstance().Clear();
            world.Update(gameTime);
            hud.Update(gameTime);
            // TODO: Add your update logic here
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            world.Draw(gameTime, spriteBatch);
            hud.Draw(gameTime);
            //Debugger.getInstance().Draw(spriteBatch);
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
