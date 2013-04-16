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

    public class HUD
    {

        /*-------------------------------------------------------------------------*/
        #region Attributes

        private static volatile HUD instance = null;

        //Black Bar
        private Texture2D banner;
        private Vector2 positionBanner;
        private Vector2 bannerScale;

        //Health Container
        private Texture2D healthBar;
        private Vector2 positionHealthBar;
        private Vector2 healthBarScale;
        private const int healthBarLengthMax = 580;

        //Green Meter
        private Texture2D healthBarContainer;
        private Vector2 positionHealthBarContainer;
        private Vector2 healthBarContainerScale;

        //Health Variables
        private float health;
        private int healthActual;

        //Score and Timer
        private int scoreIncrement;
        private int displayedScore;
        private Vector2 positionScore;
        private Vector2 positionTime;
        private float scoreScale;
        private float maxScoreScale;

        //Window size 
        private int windowHeight;
        private int windowWidth;
        private int currentScore;

        //Timer Variables
        private float minutes;
        private float timer;
        private float timerInterval;
        private float seconds;
        private string displaySeconds;           //Used to display the 0 when timer < 10 seconds

        private float animationSpeed;           //Used to animate objects on the HUD
        private float timeSoFar;
        private float TimeToAnimate;

        private SpriteFont spriteFont;

        private ParticleSpewer particleBar;

        public float Height { get { return bannerScale.Y; } }
        #endregion

        /*-------------------------------------------------------------------------*/
        #region Constructor + Load methods

        private HUD()
        {
            //Set window size 
            windowWidth = Game1.SCREEN_WIDTH;
            windowHeight = Game1.SCREEN_HEIGHT;

            //Set scale of the textures
            bannerScale = new Vector2(800, 45);
            healthBarScale = new Vector2(580, 12);
            healthBarContainerScale = new Vector2(590, 19);

            //Set positions of Textures 
            positionBanner = new Vector2(windowWidth / 2, windowHeight - 20);
            positionHealthBar = new Vector2(positionBanner.X - 280, positionBanner.Y);
            positionHealthBarContainer = new Vector2(positionBanner.X + -284, positionBanner.Y + 10);
            positionScore = new Vector2(positionBanner.X - (bannerScale.X / 2) + 10, positionBanner.Y + 4);
            positionTime = new Vector2(positionBanner.X + 355, positionBanner.Y - 12);

            //Initialize current score
            currentScore = 0;
            scoreScale = 1;
            maxScoreScale = 4.0f;
            scoreIncrement = 10;

            //Set up Timers
            timer = 0;
            timerInterval = 500;

            //Set up current health
            health = 100;
            healthActual = (int)health;

            timeSoFar = 0;
            TimeToAnimate = 0.4f;

            animationSpeed = 0.005f;

            particleBar = new ParticleSpewer(
                positionHealthBar.X + health / 100f * healthBarScale.X - 1, positionHealthBar.Y,
                50, 3, MathHelper.ToRadians(-90), MathHelper.ToRadians(90),
                50, 200, 2, 200, 90, 90, 0.5f, 1, 1, 1, true, 0.75f);
            particleBar.Absolute = true;
        }

        public void loadContent(Texture2D banner, Texture2D notorietyBar, Texture2D notorietyMeter, SpriteFont spriteFont)
        {
            this.spriteFont = spriteFont;
            this.banner = banner;
            this.healthBar = notorietyBar;
            this.healthBarContainer = notorietyMeter;
        }

        public static HUD getInstance()
        {
            if (instance == null)
            {
                instance = new HUD();
            }
            return instance;
        }
        #endregion



        /*-------------------------------------------------------------------------*/
        #region Update & Draw
        public void Update(GameTime gameTime)
        {
            //Update the time variables
            seconds = (float)gameTime.TotalGameTime.Seconds;
            minutes = (float)gameTime.TotalGameTime.Minutes;
            if (seconds < 10)
            {
                displaySeconds = "0";
            }
            else
            {
                displaySeconds = "";
            }
            //Updating the displayed score
            if (displayedScore < currentScore)
            {
                timer += Game1.elapsedTime;
                if (timer > timerInterval)
                {
                    timer = 0;
                    displayedScore += scoreIncrement;
                }
            }
            particleBar.Update(gameTime);
            UpdateScoreSize(gameTime);
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            float scale = 1 / Camera.Scale;
            Vector2 offset = new Vector2(-Camera.Width / 2, -Camera.Height / 2);
            offset *= scale;
            offset.X += Camera.X;
            offset.Y += Camera.Y;
            spriteBatch.Draw(banner, positionBanner * scale + offset, null, Color.White, 0.0f, new Vector2(bannerScale.X / 2, bannerScale.Y / 2), scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(healthBar, positionHealthBar * scale + offset, new Rectangle(0, 0, (int)updateHealthBar(gameTime), (int)healthBarScale.Y), Color.White, 0.0f, new Vector2(0, healthBarScale.Y / 2), scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(healthBarContainer, positionHealthBarContainer * scale + offset, null, Color.White, 0.0f, new Vector2(0.0f, healthBarContainerScale.Y), scale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(spriteFont, displayedScore.ToString(), positionScore * scale + offset, Color.White, 0f, new Vector2(0, 15), scoreScale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(spriteFont, minutes + ":" + displaySeconds + seconds, positionTime * scale + offset, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            particleBar.Draw(gameTime, spriteBatch);
        }
        #endregion

        /*-------------------------------------------------------------------------*/
        #region Functions
        public void increaseScore(int amount)
        {
            scoreScale = maxScoreScale;
            currentScore += amount;
        }

        public float updateHealthBar(GameTime gameTime)
        {
            particleBar.Stop();
            if (health > healthActual)
            {
                particleBar.Start();
                if (timeSoFar < TimeToAnimate)
                {
                    timeSoFar += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    timeSoFar /= 1000.0f;
                }
                if (timeSoFar > TimeToAnimate)
                {
                    timeSoFar = TimeToAnimate;
                }
                float t = (float)timeSoFar / TimeToAnimate;
                health = MathHelper.Lerp(health, healthActual, t);

                if (health - healthActual < 0.25f)
                {
                    health = healthActual;
                }
            }
            else if (health < healthActual)
            {
                health = healthActual;
                timeSoFar = 0.0f;
            }
            particleBar.X = positionHealthBar.X + (health / 100.0f) * healthBarScale.X - 1;
            return ((health / 100.0f) * healthBarScale.X);
        }

        public void UpdateScoreSize(GameTime gameTime)
        {
            if (scoreScale > 1.0f)
            {
                scoreScale -= animationSpeed * gameTime.ElapsedGameTime.Milliseconds;
            }
            else if (scoreScale < 1.0f)
            {
                scoreScale = 1.0f;
            }
        }
        public float lerp(float v0, float v1, float t)
        {
            return v0 + (v1 - v0) * t;
        }
        public void decreaseHealth(int amount)
        {
            //Check for valid amount
            if (amount <= 100 &&
                amount > 0)
            {
                healthActual -= amount;
                if (healthActual < 0)
                {
                    healthActual = 0;
                }
            }
            else
            {
                Console.WriteLine("INVALID HEALTH DECREMENT");
            }
        }
        #endregion
    }
}
