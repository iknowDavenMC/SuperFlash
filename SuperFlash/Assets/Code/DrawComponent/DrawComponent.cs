using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StreakerLibrary;

namespace COMP476Proj
{
    public class DrawComponent
    {
        /*-------------------------------------------------------------------------*/
        #region Fields

        public Animation animation;

        //Draw Logic
        protected bool visible = true;
        public Vector2 Origin = Vector2.Zero;
        protected Vector2 scale = Vector2.One;
        protected Vector2 position = Vector2.Zero;
        protected Color color = Color.White;
        protected SpriteEffects spriteEffects = SpriteEffects.None;
        protected float depth = 0.5f;
        protected float alpha = 1.0f;


        //Animation
        protected bool paused = false;
        protected int currentFrame = 0;
        protected float timePerFrame = -1;
        protected float timeElapsed = 0;
        public bool animComplete = true;

        private const int PIXELS_HEAD_TO_TOE = 149;
        private const int PIXELS_LEFT_TO_CENTER = 88;

        #endregion

        /*-------------------------------------------------------------------------*/
        #region Properties

        public bool IsPaused
        {
            get { return paused; }
        }
        public SpriteEffects SpriteEffect
        {
            set { spriteEffects = value; }
        }
        public Vector2 Position
        {
            get { return position; }
        }
        public int CurrentFrame { get { return currentFrame; } }

        public float Alpha
        {
            set { alpha = value; }
        }
        #endregion

        /*-------------------------------------------------------------------------*/
        #region Init

        public DrawComponent(Animation defaultAnimation) {
            animation = defaultAnimation;
            Pause();
        }

        /*
        public DrawComponent(Animation defaultAnimation, int timePerFrame)
        {
            animation = defaultAnimation;
            this.timePerFrame = timePerFrame;
            Pause();
        }

        public DrawComponent(Animation defaultAnimation, Color col, Vector2 orig, Vector2 scale, float depth)
       {
           animation = defaultAnimation;
           color = col;
           Origin = orig;
           this.scale = scale;
           this.depth = depth;
           Pause();
       }
         * */

        public DrawComponent(Animation defaultAnimation, Color col, Vector2 orig, Vector2 scale, float depth)
       {
           animation = defaultAnimation;
           color = col;
           Origin = orig;
           this.scale = scale;
           this.depth = depth;
           Pause();
       }
        #endregion

       /*-------------------------------------------------------------------------*/
        #region Update and Draw

        public virtual void Update(GameTime gameTime)
        {
            if (animation != null)
                timePerFrame = animation.TimePerFrame;
            else
                timePerFrame = 0;
            if (!paused && timePerFrame > 0)
            {
                timeElapsed += (float)gameTime.ElapsedGameTime.Milliseconds;

                if (timeElapsed > timePerFrame)
                {
                    currentFrame++;
                    
                    timeElapsed = 0;
                }
                if (currentFrame >= animation.NumOfColumns)
                {
                    animComplete = true;
                    //currentFrame = 0;
                    Reset();
                }
                else
                {
                    animComplete = false;
                }
                
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 pos)
        {
            if (visible)
            {
                Rectangle sourceRect = new Rectangle(animation.FrameWidth * currentFrame, animation.YPos, animation.FrameWidth, animation.FrameHeight);
                Vector2 offset;

                if (spriteEffects == SpriteEffects.FlipHorizontally)
                {
                    offset = new Vector2((animation.FrameWidth - PIXELS_LEFT_TO_CENTER) * scale.X, PIXELS_HEAD_TO_TOE * scale.Y);
                }
                else
                {
                    offset = new Vector2(PIXELS_LEFT_TO_CENTER * scale.X, PIXELS_HEAD_TO_TOE * scale.Y);
                }
                
                Vector2 drawPos = new Vector2(pos.X, pos.Y) - offset;
                spriteBatch.Draw(animation.Texture, drawPos, sourceRect, color *alpha, 0, Origin, scale, spriteEffects, depth);
                
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, float offsetX, float offsetY)
        {
            if (visible)
            {
                Rectangle sourceRect = new Rectangle(animation.FrameWidth * currentFrame, animation.YPos, animation.FrameWidth, animation.FrameHeight);
                Vector2 offset;

                if (spriteEffects == SpriteEffects.FlipHorizontally)
                {
                    offset = new Vector2((animation.FrameWidth - PIXELS_LEFT_TO_CENTER) * scale.X, PIXELS_HEAD_TO_TOE * scale.Y);
                }
                else
                {
                    offset = new Vector2(PIXELS_LEFT_TO_CENTER * scale.X, PIXELS_HEAD_TO_TOE * scale.Y);
                }

                Vector2 drawPos = new Vector2(Position.X, Position.Y) - offset + new Vector2(offsetX,offsetY);
                spriteBatch.Draw(animation.Texture, drawPos, sourceRect, color*alpha, 0, Origin, scale, spriteEffects, depth);

            }
        }

        #endregion

        /*-------------------------------------------------------------------------*/
        #region Animation

        public void Reset()
        {
            currentFrame = 0;
            timeElapsed = 0f;
        }

        public void Stop()
        {
            Pause();
            Reset();
        }

        public void Play()
        {
            paused = false;
        }

        public void Pause()
        {
            paused = true;
        }

        public void GoToPrevFrame()
        {
            currentFrame = (currentFrame - 1) % animation.NumOfColumns;
            while (currentFrame < 0)
            {
                currentFrame += animation.NumOfColumns;
            }
        }

        #endregion
    }
}
