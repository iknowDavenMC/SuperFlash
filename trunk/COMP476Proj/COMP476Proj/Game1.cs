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
using StreakerLibrary;

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

        FrameRate frameRate;

        ParticleSpewer ps;
        ParticleSpewer ps2;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.Window.Title = "Superflash";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            InputManager.GetInstance(InputManager.ControllerType.Keyboard);

            frameRate = new FrameRate(this, 1);

            Components.Add(frameRate);

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

            SpriteDatabase.loadSprites(Content);
            //Create World
            world = new World();

            Texture2D level = SpriteDatabase.GetAnimation("level_1").Texture;

            Camera.MaxX = level.Width;
            Camera.MaxY = level.Height;
            Camera.Target = world.streaker;
            Camera.Scale = 1f;

            Debugger.getInstance();
            //Hud

            Texture2D banner = Content.Load<Texture2D>("Hud/banner");
            Texture2D notorietyBar = Content.Load<Texture2D>("Hud/notorietyBar");
            Texture2D notorietyMeter = Content.Load<Texture2D>("Hud/notorietyMeter");
            hud = new HUD(this, spriteBatch, spriteFont, banner, notorietyBar, notorietyMeter);

            Texture2D blank = new Texture2D(GraphicsDevice, 1, 1);
            blank.SetData(new[] { Color.White });
            SpriteDatabase.AddAnimation(new Animation("blank", blank, 1, 1, 1, 0, 1));

            // Two example particle spewers. One is stuck to the HUD and always on, 
            // the other will follow the streaker and only turn on when he dances
            ps = new ParticleSpewer(
                150, 555, // Start position
                1000, 2, // Max particles and emitters
                MathHelper.ToRadians(247.5f), // Angle range to spew at
                MathHelper.ToRadians(292.5f), // This will be a narrow cone upward
                50, 1000, // Lifespan range (up to one second of life)
                3, 100, // Particle size and speed (3-pixel particles at 100 pixels/s
                0, 360, 1, 1, 1, 1, // Min and max Hue, Saturation and Value (This uses any colour at full saturation and value)
                true, 0.5f); // Fade out after half the lifespan
            ps.Absolute = true; // The camera does not affect this particle spewer
            ps.Start();

            ps2 = new ParticleSpewer(
                100, 75, // Start at the streaker's chest
                1000, 36, // Shoot a lot of particles at once
                MathHelper.ToRadians(0), // Spew in every direction
                MathHelper.ToRadians(360),
                0, 500, // Live for up to half a second
                2, 200, // 2-pixel particles moving at 200 pixels/s
                150, 240, 0, 1, 1, 1, // Colours range from cyan to blue, with and saturation and full value
                true, 0.9f); // Only fade at the end

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

            // Start the spewer while dancing
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                ps2.Start();
            }
            else ps2.Stop();
            world.Update(gameTime);
            hud.Update(gameTime);
            ps.Update(gameTime);
            ps2.X = world.streaker.X;
            ps2.Y = world.streaker.Y - 25;
            ps2.Update(gameTime);
            Camera.Update(gameTime);
#if (DEBUG)
            {
                this.Window.Title = frameRate.CurrentFramesPerSecond.ToString() + " frames per second";
            }
#endif

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            Vector3 center = new Vector3(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2, 0);
            Matrix transform = Matrix.CreateTranslation(-(Camera.X), -(Camera.Y), 0) * Matrix.CreateScale(Camera.Scale) * Matrix.CreateTranslation(center);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, transform);
            world.Draw(gameTime, spriteBatch);
            hud.Draw(gameTime);
            ps2.Draw(gameTime, spriteBatch);
            ps.Draw(gameTime, spriteBatch);
            //Debugger.getInstance().Draw(spriteBatch);

            foreach (Pedestrian ped in world.pedestrians)
            {
                ped.BoundingRectangle.Draw(graphics.GraphicsDevice, spriteBatch);
            }

            world.streaker.BoundingRectangle.Draw(graphics.GraphicsDevice, spriteBatch);
            spriteBatch.End();




            base.Draw(gameTime);
        }
    }
}
