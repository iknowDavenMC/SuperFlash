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
        
        //Variables
        private int health;

        //Score and Timer
        private int scoreIncrement;
        private int displayedScore;
        private Vector2 positionScore;
        private Vector2 positionTime;
        
        //Window size 
        private int windowHeight;
        private int windowWidth;
        private int currentScore;
        
        //Timer Variables
        private float minutes;
        private float timer;
        private float timerInterval;
        private float seconds;

        private SpriteFont spriteFont;

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
            positionBanner = new Vector2(windowWidth/2, windowHeight - 20);
            positionHealthBar = new Vector2(positionBanner.X - 280, positionBanner.Y);
            positionHealthBarContainer = new Vector2(positionBanner.X + -284, positionBanner.Y+10);
            positionScore = new Vector2(.0f, positionBanner.Y -12);
            positionTime = new Vector2(positionBanner.X +355, positionBanner.Y-12);
            
            //Initialize current score
            currentScore = 0;
            scoreIncrement = 10;

            //Set up Timers
            timer = 0;
            timerInterval = 500;
            
            //Set up current health
            health = 100;
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
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            float scale = 1 / Camera.Scale;
            Vector2 offset = new Vector2(-Camera.Width / 2, -Camera.Height / 2);
            offset *= scale;
            offset.X += Camera.X;
            offset.Y += Camera.Y;
            spriteBatch.Draw(banner, positionBanner * scale + offset, null, Color.White, 0.0f, new Vector2(bannerScale.X/2, bannerScale.Y/2), scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(healthBar, positionHealthBar * scale + offset, new Rectangle(0, 0, (int)updateHealthBar(), (int)healthBarScale.Y), Color.White, 0.0f, new Vector2(0, healthBarScale.Y / 2), scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(healthBarContainer, positionHealthBarContainer * scale + offset, null, Color.White, 0.0f, new Vector2(0.0f, healthBarContainerScale.Y), scale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(spriteFont, displayedScore.ToString(), positionScore * scale + offset, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(spriteFont, minutes+":"+seconds, positionTime * scale + offset, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
        #endregion

        /*-------------------------------------------------------------------------*/
        #region Functions
        public void increaseScore(int amount)
        {
            currentScore += amount;
        }

        public float updateHealthBar()
        {
            return ((health / 100.0f) * healthBarScale.X);
        }

        public void decreaseHealth(int amount)
        {
            //Check for valid amount
            if (amount <= 100 &&
                amount > 0)
            {
                health -= amount;
                if (health < 0)
                {
                    health = 0;
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
