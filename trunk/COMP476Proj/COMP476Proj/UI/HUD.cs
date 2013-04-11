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

    public class HUD : Microsoft.Xna.Framework.DrawableGameComponent
    {

        /*-------------------------------------------------------------------------*/
        #region Fields
        private Texture2D banner;
        private Texture2D notorietyBar;
        private Texture2D notorietyMeter;
        private int notorietyBarLength = 400;
        private Vector2 positionBanner;
        private Vector2 positionNotorietyBar;
        private Vector2 positionNotorietyMeter;
        private Vector2 positionScore;
        private Vector2 positionTime;
        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;
        private int windowHeight = 600;
        private int windowWidth = 800;
        private int score;
        private float seconds, minutes;
        float elapsedTime, timer;
        #endregion

        /*-------------------------------------------------------------------------*/
        #region Constructor
        public HUD(Game game, SpriteBatch spriteBatch, SpriteFont spriteFont, Texture2D banner, Texture2D nororietyBar, Texture2D notorietyMeter)
            : base(game)
        {
            this.spriteBatch = spriteBatch;
            this.spriteFont = spriteFont;
            this.banner = banner;
            this.notorietyBar = nororietyBar;
            this.notorietyMeter = notorietyMeter;
            positionBanner = new Vector2(0, windowHeight - 45);
            positionNotorietyBar = new Vector2(positionBanner.X + 120, positionBanner.Y + 15);
            positionNotorietyMeter = new Vector2(positionBanner.X + 118, positionBanner.Y + 11);
            positionScore = new Vector2(positionBanner.X, positionBanner.Y + 1);
            positionTime = new Vector2(positionBanner.X + 750, positionBanner.Y + 1);
            score = 0;
        }
        #endregion

        /*-------------------------------------------------------------------------*/
        #region Init
        public override void Initialize()
        {


            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }
        #endregion

        /*-------------------------------------------------------------------------*/
        #region Update & Draw
        public override void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.Milliseconds;
            if (elapsedTime >= 60.0f)
            {
                elapsedTime = 0;
                seconds++;
            }
            //seconds += (int)gameTime.ElapsedGameTime.Seconds;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            float scale = 1 / Camera.Scale;

            Vector2 offset = new Vector2(-Camera.Width / 2, -Camera.Height / 2);
            offset *= scale;
            offset.X += Camera.X;
            offset.Y += Camera.Y;
            spriteBatch.Draw(banner, positionBanner * scale + offset, null, Color.White, 0.0f, new Vector2(0.0f, 0.0f), scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(notorietyBar, positionNotorietyBar * scale + offset, new Rectangle(0, 0, notorietyBarLength, 12), Color.White, 0.0f, new Vector2(0.0f, 0.0f), scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(notorietyMeter, positionNotorietyMeter * scale + offset, null, Color.White, 0.0f, new Vector2(0.0f, 0.0f), scale, SpriteEffects.None, 0f);

            spriteBatch.DrawString(spriteFont, "10899", positionScore * scale + offset, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(spriteFont, "" + seconds, positionTime * scale + offset, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            base.Draw(gameTime);
        }
        #endregion
    }
}
