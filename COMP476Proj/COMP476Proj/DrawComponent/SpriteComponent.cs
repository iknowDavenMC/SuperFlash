using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace COMP476Proj
{
    public abstract class SpriteComponent
    {
        /*-------------------------------------------------------------------------*/
        #region Fields

        public Animation animation;
        public Entity entity;

        //Draw Logic
        protected bool visible = true;
        public Vector2 Origin = Vector2.Zero;
        protected Vector2 scale = Vector2.One;
        protected Color color = Color.White;
        protected SpriteEffects spriteEffects = SpriteEffects.None;
        protected float depth = 0.5f;

        //Animation
        protected bool paused = false;
        protected int currentFrame = 0;
        protected float timePerFrame = -1;
        protected float timeElapsed = 0;

        #endregion

        /*-------------------------------------------------------------------------*/
        #region Properties

        public bool IsPaused
        {
            get { return paused; }
        }

        #endregion

        /*-------------------------------------------------------------------------*/
        #region Init

        public SpriteComponent(Animation defaultAnimation) {
            animation = defaultAnimation;
        }

        public SpriteComponent(Animation defaultAnimation, int timePerFrame)
        {
            animation = defaultAnimation;
            this.timePerFrame = timePerFrame;
        }

       public SpriteComponent(Animation defaultAnimation, Color col, Vector2 orig, Vector2 scale, float depth)
       {
           animation = defaultAnimation;
           color = col;
           Origin = orig;
           this.scale = scale;
           this.depth = depth;
       }

       public SpriteComponent(Animation defaultAnimation, Color col, Vector2 orig, Vector2 scale, float depth, 
            int timePerFrame)
       {
           animation = defaultAnimation;
           color = col;
           Origin = orig;
           this.scale = scale;
           this.depth = depth;
           this.timePerFrame = timePerFrame;
       }
        #endregion

       /*-------------------------------------------------------------------------*/
        #region Update and Draw

        public virtual void Update(Entity gameObj, GameTime gameTime)
        {
            if (!paused && timePerFrame > 0)
            {
                timeElapsed += (float)gameTime.ElapsedGameTime.Milliseconds;

                if (timeElapsed > timePerFrame)
                {
                    currentFrame++;
                    currentFrame = currentFrame % animation.NumOfColumns;
                    timeElapsed -= timePerFrame;
                }
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, Entity gameObj)
        {
            if (visible)
            {
                Rectangle sourceRect = new Rectangle(animation.FrameWidth * currentFrame, animation.YPos, animation.FrameWidth, animation.FrameHeight);
                spriteBatch.Draw(animation.Texture, gameObj.position, sourceRect, color, 0, Origin, scale, spriteEffects, depth);
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

        #endregion
    }
}
