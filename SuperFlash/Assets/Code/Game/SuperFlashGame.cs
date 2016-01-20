using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Assets.Code._XNA;
using StreakerLibrary;

namespace COMP476Proj
{
    public enum Difficulty { EASY, MEDIUM, HARD, IMPOSSIBLE };
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class SuperFlashGame : Game
    {
        public static System.Random random = new System.Random();

        public const int SCREEN_WIDTH = 1280;
        public const int SCREEN_HEIGHT = 1024;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        public static World world;
        public HUD hud;
        public static float elapsedTime;
        FrameRate frameRate;
        public static bool reset;
        public static Difficulty difficulty = Difficulty.HARD;

        Menu mainMenu;

        public enum GameState
        {
            MAIN, PLAY,
        }
        public static GameState currentGameState;
        public SuperFlashGame()
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
            InputManager.GetInstance();

            frameRate = new FrameRate(this, 1);
            mainMenu = new Menu();
            currentGameState = GameState.MAIN;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            SoundManager.GetInstance().LoadContent(Content);
            SoundManager.GetInstance().PlaySong("Level");
            reset = false;
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
#if(!DEBUG)
            this.graphics.IsFullScreen = true;
#endif
            graphics.ApplyChanges();
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            spriteFont = Content.Load<SpriteFont>("Fonts/Game Over");
            FontManager fontMan = FontManager.getInstance();
            fontMan.addFont("GameOver", spriteFont);
            fontMan.addFont("AchieveTitle", Content.Load<SpriteFont>("Fonts/AchievementTitle"));
            fontMan.addFont("AchieveText", Content.Load<SpriteFont>("Fonts/AchievementText"));
            SpriteDatabase.loadSprites(Content);
            Texture2D blank = new Texture2D(GraphicsDevice, 1, 1);
            blank.SetData(new[] { Color.white });
            SpriteDatabase.AddAnimation(new CustomAnimation("blank", blank, 1, 1, 1, 0, 1));
            //Create World

            world = new World();
            world.LoadMap("level.txt");
            Texture2D level = SpriteDatabase.GetAnimation("level_1").Texture;

            CustomCamera.MaxX = level.Width;
            CustomCamera.MaxY = level.Height;
            CustomCamera.Target = world.streaker;
            CustomCamera.Scale = 1f;

            Debugger.getInstance();
            //Hud

            Texture2D banner = Content.Load<Texture2D>("Hud/banner");
            Texture2D notorietyBar = Content.Load<Texture2D>("Hud/notorietyBar");
            Texture2D notorietyMeter = Content.Load<Texture2D>("Hud/notorietyMeter");
            Texture2D gameOverText = Content.Load<Texture2D>("Hud/gameOverText");
            Texture2D superFlashIcon = Content.Load<Texture2D>("Hud/superFlashButton");
            HUD.getInstance().loadContent(banner, notorietyBar, notorietyMeter, spriteFont, blank, gameOverText, superFlashIcon);

            IsMouseVisible = true;

            mainMenu.LoadContent(Content.Load<Texture2D>("Menu"));
        }

        public static void LoadContentReset()
        {
            NPC.copsWhoSeeTheStreaker = 0;
            SmartCop.closest = null;
            SmartCop.closestDistSq = float.MaxValue;
            SmartCop.StreakerSeen = false;

            foreach (TriggerEntity trigger in world.map.triggers)
            {
                trigger.clearTriggered();
            }

            AchievementManager.getInstance().Reset();
            DataManager.GetInstance().Reset();

            elapsedTime = 0.0f;
            world = new World();
            world.LoadMap("level.txt");
            Texture2D level = SpriteDatabase.GetAnimation("level_1").Texture;
            //Reset data in datamanager
            SoundManager.GetInstance().PlaySong("Level");
            CustomCamera.Target = world.streaker;

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

            if (currentGameState == GameState.MAIN)
            {
                mainMenu.Update(gameTime);
            }
            //GamePlay
            if (currentGameState == GameState.PLAY)
            {
                world.Update(gameTime);
                CustomCamera.Update(gameTime);
#if (DEBUG)
                {
                    this.Window.Title = frameRate.CurrentFramesPerSecond.ToString() + " frames per second";
                }
#endif
                elapsedTime += Time.deltaTime * 1000f;
                HUD.getInstance().Update(gameTime);
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            frameRate.Update(gameTime);

            GraphicsDevice.Clear(Color.ForestGreen);

            if (currentGameState == GameState.MAIN)
            {
                spriteBatch.Begin();
                mainMenu.Draw(gameTime, spriteBatch);
                spriteBatch.End();
            }
            if (currentGameState == GameState.PLAY)
            {
                Vector3 center = new Vector3(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2, 0);
                Matrix transform = Matrix.CreateTranslation(-(CustomCamera.X), -(CustomCamera.Y), 0) * Matrix.CreateScale(CustomCamera.Scale) * Matrix.CreateTranslation(center);
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, transform);
                world.Draw(gameTime, spriteBatch);
                HUD.getInstance().Draw(gameTime, spriteBatch);
                //Debugger.getInstance().Draw(spriteBatch);

                foreach (NPC ped in world.npcs)
                {
                    ped.BoundingRectangle.Draw(spriteBatch);
                }

                world.streaker.BoundingRectangle.Draw(spriteBatch);
                spriteBatch.End();
            }



            base.Draw(gameTime);
        }
    }
}
