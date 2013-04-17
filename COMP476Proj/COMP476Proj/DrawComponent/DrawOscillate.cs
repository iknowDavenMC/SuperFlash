using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StreakerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace COMP476Proj
{
    public class DrawOscillate : DrawComponent
    {
        #region Fields
        protected bool oscillateSize = true;
        protected float lowerBound = 0.5f;
        protected bool oscillateAlpha = false;
        public bool OscillateSize
        {
            set { oscillateSize = value; }
        }
        public bool OscillateAlpha
        {
            set { oscillateAlpha = value; }
        }

        #endregion

        #region Constructors
        public DrawOscillate(Animation anim, Vector2 pos, float depth, bool startOscillating)
            : base(anim)
        {
            Origin = Vector2.Zero;
            this.position = pos;
            this.depth = depth;
            oscillateAlpha = startOscillating;
            oscillateSize = false;
        }

        public DrawOscillate(Animation anim, float depth)
            : base(anim)
        {
            Origin = Vector2.Zero;
            this.depth = depth;

        }
        //public DrawOscillate(Animation anim, Vector2 origin) : base(anim) {
        //    Origin = Vector2.Zero;
        //}

        //public DrawOscillate(Animation anim, Vector2 origin, float depth)
        //    : base(anim)
        //{
        //    Origin = Vector2.Zero;
        //    this.depth = depth;
        //}

        #endregion

        #region Update & Draw
        public override void Update(GameTime gameTime)
        {
            float ratio =  (float)Math.Sin(gameTime.TotalGameTime.Milliseconds / 155);;
            if (oscillateSize)
            {
                scale.X = (ratio + 1 + lowerBound) / 5;
                scale.Y = scale.X;
            }
            if (oscillateAlpha)
            {
                alpha = ratio*.5f+.7f;
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float offsetX, float offsetY)
        {
            base.Draw(gameTime, spriteBatch, offsetX, offsetY);
        }
        #endregion
    }
}
