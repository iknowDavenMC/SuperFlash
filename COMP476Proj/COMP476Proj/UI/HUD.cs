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
        //Hud Components
        private List<SpriteComponent> hudComponents;
        SpriteComponent bannerComponent;
        SpriteComponent healthBarComponent;
        SpriteComponent healthBarContainerComponent;
        SpriteComponent superFlashIconComponent;

        //Font Components
        private List<FontComponent> fontComponents;
        FontComponent replayText;
        FontComponent timerText;


        //Game Over Buttons
        Button replayButton;
        //Health Variables
        private float health;

        //Fade to Black contents
        private Texture2D fadeToBlack;
        private Vector2 fadeToBlackPosition;
        private float fadeToBlackAlpha;
        private float fadeToBlackDesiredAlpha;
        private float fadeToBlackCurrentTime; 

        //Game Over contents
        private bool isGameOver;
        private Texture2D gameOverText;
        private Vector2 gameOverTextPosition;
        private Rectangle gameOverTextSize;
        private float gameOverTextScale;
        private float gameOverCurrentTime; 

        //Score and Timer
        private int scoreIncrement;
        private int displayedScore;
        private Vector2 positionScore;
        private float scoreScale;
        private float maxScoreScale;

        //Window size 
        private int windowHeight;
        private int windowWidth;
        //private int currentScore;

        //Timer Variables
        private float minutes;
        private float timer;
        private float timerInterval;
        private float seconds;
        private string displaySeconds;           //Used to display the 0 when timer < 10 seconds

        //Animate Variables
        private float animationSpeed;           //Used to animate objects on the HUD
        private float timeSoFar;
        private float TimeToAnimate;

        //Boolean variables
        private bool playGameOverMusic;

        private SpriteFont spriteFont;

        private ParticleSpewer particleBar;

        

        public float Height { get { return bannerComponent.getSize().Y; } }
        #endregion

        /*-------------------------------------------------------------------------*/
        #region Constructor + Load methods

        private HUD()
        {
            //Set window properties
            windowWidth = Game1.SCREEN_WIDTH;
            windowHeight = Game1.SCREEN_HEIGHT;

            hudComponents = new List<SpriteComponent>();
            //Components
            bannerComponent = new SpriteComponent(new Vector2(windowWidth / 2, windowHeight - 20), new Vector2(800, 45));
            healthBarComponent = new SpriteComponent(new Vector2(bannerComponent.getPosition().X-280, bannerComponent.getPosition().Y), new Vector2(580, 12));
            healthBarComponent.setOriginLeft();
            healthBarContainerComponent = new SpriteComponent(new Vector2(bannerComponent.getPosition().X + -284, bannerComponent.getPosition().Y + 10), new Vector2(590, 19));
            healthBarContainerComponent.setOriginBottomLeft();
            superFlashIconComponent = new SpriteComponent(new Vector2((bannerComponent.getPosition().X - bannerComponent.getSize().X/2) - 25, bannerComponent.getPosition().Y-2), new Vector2(45.0f, 45.0f));


            
            
            //Score Positions
            positionScore = new Vector2(bannerComponent.getPosition().X - (bannerComponent.getSize().X / 2) + 10, bannerComponent.getPosition().Y + 4);

            
            
            
            //Initialize current score
            //currentScore = 0;
            scoreScale = 1;
            maxScoreScale = 4.0f;
            scoreIncrement = 10;

            //Set up Timers
            timer = 0;
            timerInterval = 500;

            //Set up current health
            health = 100;
            //healthActual = (int)health;

            //Animate timer 
            timeSoFar = 0;
            TimeToAnimate = 0.4f;
            animationSpeed = 0.005f;

            //Game over initializations 
            isGameOver = false; 
            gameOverTextPosition = new Vector2(windowWidth / 2, windowHeight / 2);
            gameOverTextScale = 5.0f;
            gameOverTextSize = new Rectangle(0, 0, 556, 126);
            gameOverCurrentTime = 0.0f; 

            //Fade To Black initialization 
            fadeToBlackAlpha = 0.0f;
            fadeToBlackDesiredAlpha = 0.8f;
            fadeToBlackPosition = new Vector2(0.0f, 0.0f);
            fadeToBlackCurrentTime = 0.0f;

            playGameOverMusic = true;

            particleBar = new ParticleSpewer(
                healthBarComponent.getPosition().X + health / 100f * healthBarComponent.getSize().X - 1, healthBarComponent.getPosition().Y,
                50, 20, MathHelper.ToRadians(-90), MathHelper.ToRadians(90),
                50, 200, 2, 200, 90, 90, 0.5f, 1, 1, 1, true, 0.75f);
            particleBar.Absolute = true;

            //Test for a font component
            replayText = new FontComponent(new Vector2(gameOverTextPosition.X-250, gameOverTextPosition.Y + 40), new Vector2(100, 100));
            timerText = new FontComponent(new Vector2(bannerComponent.getPosition().X + 355, bannerComponent.getPosition().Y - 12), new Vector2(100, 100));

            //Button Initializations
            replayButton = new Button(replayText.getPosition(), replayText.getSize());
        }

        public void loadContent(Texture2D banner, 
            Texture2D notorietyBar, 
            Texture2D notorietyMeter, 
            SpriteFont spriteFont, 
            Texture2D fadeToBlack, 
            Texture2D gameOverText,
            Texture2D superFlashIcon)
        {
            this.spriteFont = spriteFont;
            this.fadeToBlack = fadeToBlack;
            this.gameOverText = gameOverText;

            //Now using HUDComponents
            this.bannerComponent.LoadContent(banner);
            this.healthBarComponent.LoadContent(notorietyBar);
            this.healthBarContainerComponent.LoadContent(notorietyMeter);
            this.superFlashIconComponent.LoadContent(superFlashIcon);
            hudComponents.Add(bannerComponent);
            hudComponents.Add(healthBarComponent);
            hudComponents.Add(healthBarContainerComponent);
            hudComponents.Add(superFlashIconComponent);

            //Font components
            this.replayText.LoadContent(spriteFont);
            this.timerText.LoadContent(spriteFont);
        }

        public static HUD getInstance()
        {
            if (instance == null)
            {
                instance = new HUD();
            }
            return instance;
        }

        public void resetHud()
        {
            //Initialize current score
            //currentScore = 0;
            scoreScale = 1;
            maxScoreScale = 4.0f;
            scoreIncrement = 10;

            //Set up Timers
            timer = 0;
            timerInterval = 500;

            //Set up current health
            health = 100;
            //healthActual = (int)health;

            //Animate timer 
            timeSoFar = 0;
            TimeToAnimate = 0.4f;
            animationSpeed = 0.005f;

            //Game over initializations 
            isGameOver = false;
            gameOverTextPosition = new Vector2(windowWidth / 2, windowHeight / 2);
            gameOverTextScale = 5.0f;
            gameOverTextSize = new Rectangle(0, 0, 556, 126);
            gameOverCurrentTime = 0.0f;

            //Fade To Black initialization 
            fadeToBlackAlpha = 0.0f;
            fadeToBlackDesiredAlpha = 0.8f;
            fadeToBlackPosition = new Vector2(0.0f, 0.0f);
            fadeToBlackCurrentTime = 0.0f;

            playGameOverMusic = true;
            seconds = 0;
            minutes = 0;
            displayedScore = 0;
            displaySeconds = "0";
            
        }
        #endregion
        


        /*-------------------------------------------------------------------------*/
        #region Update & Draw
        public void Update(GameTime gameTime)
        {
            //Update the time variables
            seconds = ((int)DataManager.GetInstance().time / 1000)%60;
            minutes = (int)(DataManager.GetInstance().time / 60000);
            if (seconds < 10)
            {
                displaySeconds = "0";
            }
            else
            {
                displaySeconds = "";
            }
            //Updating the displayed score
            if (displayedScore < DataManager.GetInstance().score)
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
            updateSuperFlashIcon();
            
            //Update health bar size
            healthBarComponent.setXScale((int)updateHealthBar(gameTime));
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            float scale = 1 / Camera.Scale;
            Vector2 offset = new Vector2(-Camera.Width / 2, -Camera.Height / 2);
            offset *= scale;
            offset.X += Camera.X;
            offset.Y += Camera.Y;

            //Components
            foreach (SpriteComponent component in hudComponents)
            {
                component.Draw(gameTime, spriteBatch, scale, offset);
            }

            float scoreOffset = spriteFont.MeasureString(displayedScore.ToString()).X / 2;
            spriteBatch.DrawString(spriteFont, displayedScore.ToString(), (positionScore + new Vector2(scoreOffset, 0)) * scale + offset, Color.White, 0f, new Vector2(scoreOffset, 15), scoreScale * scale, SpriteEffects.None, 0f);
            
            particleBar.Draw(gameTime, spriteBatch);
            timerText.setText(minutes + ":" + displaySeconds + seconds);
            timerText.Draw(gameTime, spriteBatch, scale, offset);

            drawGameOver(gameTime, spriteBatch, offset, scale);
            
            
            
        }
        
        public void drawGameOver(GameTime gameTime, SpriteBatch spriteBatch, Vector2 offset, float scale)
        {
            //If the game is over, draw the sprites
            if (isGameOver)
            {
                Game1.world.streaker.Kill();

                if (playGameOverMusic)
                {
                    SoundManager.GetInstance().PlaySong("Game Over");
                    playGameOverMusic = false;
                }
                //Draw black Alpha
                animateFadeToBlack(gameTime);
                spriteBatch.Draw(fadeToBlack, new Rectangle((int)(0 + offset.X), (int)(0 + offset.Y), (int)(windowWidth * scale + offset.X), (int)(windowHeight * scale + offset.Y)), Color.Black * fadeToBlackAlpha);

                //Draw Game Over Text
                interpolate(ref gameOverTextScale, 1.0f, ref gameOverCurrentTime, 0.7f, gameTime);
                spriteBatch.Draw(gameOverText, gameOverTextPosition * scale + offset, gameOverTextSize, Color.White, 0.0f, new Vector2(gameOverTextSize.Right / 2, gameOverTextSize.Bottom / 2), gameOverTextScale, SpriteEffects.None, 1.0f);
            
                //Place the Replay Button
                replayText.setText("Replay?");
                replayText.setFontScale(20.0f);
                replayText.Draw(gameTime, spriteBatch, scale, offset);

                MouseState mouse = Mouse.GetState();
                
                replayButton.SetPosition(replayText.getPosition());
                replayButton.setSize(replayText.getSize());
                replayButton.Update(mouse);
                if (replayButton.isClicked)
                {
 
                    Game1.LoadContentReset();
                    Game1.currentGameState = Game1.GameState.PLAY;
                    resetHud();
                    replayButton.isClicked = false;
                }
            }
        }
        #endregion

        /*-------------------------------------------------------------------------*/
        #region Hud Manipulation Functions
        
        //Increases the score by the desired amount
        public void increaseScore(int amount)
        {
            scoreScale = maxScoreScale;
        }

        public void updateSuperFlashIcon()
        {
            if (Game1.world.streaker.hasSuperFlash())
            {
                superFlashIconComponent.setAlpha(1.0f);
            }
            else
            {
                superFlashIconComponent.setAlpha(0.3f);
            }
        }
        
        //Decreases the score by the desired amount 
        public void decreaseHealth(int amount)
        {
            
            if (amount <= 100 &&
                amount > 0)
            {
                particleBar.Start();
                DataManager.GetInstance().DecreaseHealth(amount);
                if (DataManager.GetInstance().health <= 0)
                {
                    isGameOver = true;
                }
            }
            else
            {
                Console.WriteLine("INVALID HEALTH DECREMENT");
            }
        }
            

        
        
        #endregion

        /*-------------------------------------------------------------------------*/
        #region Animation Functions
        
        //Fade the Screen to Black
        private void animateFadeToBlack(GameTime gameTime)
        {
            interpolate(ref fadeToBlackAlpha, fadeToBlackDesiredAlpha, ref fadeToBlackCurrentTime, 2.0f, gameTime);
        }

        //Update the Health Bar Length
        public float updateHealthBar(GameTime gameTime)
        {
            particleBar.Stop();
            interpolate(ref health, DataManager.GetInstance().health, ref timeSoFar, TimeToAnimate, gameTime);
            particleBar.X = healthBarComponent.getPosition().X + (health / 100.0f) * healthBarComponent.getSize().X - 1;
            return ((health / 100.0f) * healthBarComponent.getSize().X);
        }

        //Update the score size 
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
        #endregion
        
        /*-------------------------------------------------------------------------*/
        #region HelperMethods

        /// <summary>
        /// Interpolates between two numbers, automatically adjusts the first one passed
        /// </summary>
        /// <param name="v1">Current state of value</param>
        /// <param name="v2">Final state of value</param>
        /// <param name="currentTime">the current timer</param>
        /// <param name="totalTime">How long the animation should take place for</param>
        /// <param name="gameTime">The current Game time</param>
        private void interpolate(ref float v1, float v2, ref float currentTime, float totalTime, GameTime gameTime)
        {
            if (v2 != v1)
            {
                updateAnimationTimer(gameTime, ref currentTime, totalTime);
                float t = (float)currentTime / totalTime;
                v1 = MathHelper.Lerp(v1, v2, t);
                if (Math.Abs(v1 - v2) <= 0.01f)
                {
                    v1 = v2;
                    currentTime = 0.0f;
                }
            }
        }

        //Use this to update the timer used for interpolation
        private void updateAnimationTimer(GameTime gameTime, ref float currentTime, float timeToAnimate)
        {
            if (currentTime < timeToAnimate)
            {
                currentTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                currentTime /= 1000.0f;
            }
            if (currentTime > timeToAnimate)
            {
                currentTime = timeToAnimate;
            }
        }
        #endregion
    }
}
