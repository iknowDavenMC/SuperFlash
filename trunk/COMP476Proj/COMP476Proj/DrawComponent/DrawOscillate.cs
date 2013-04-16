using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StreakerLibrary;
using Microsoft.Xna.Framework;

namespace COMP476Proj
{
    public class DrawOscillate : DrawComponent
    {
        #region Fields
        protected bool oscillateOn = true;
        protected float lowerBound = 0.5f;

        #endregion

        #region Constructors
        public DrawOscillate(Animation anim, Vector2 origin) : base(anim) {
            Origin = Vector2.Zero;
        }

        public DrawOscillate(Animation anim, Vector2 origin, float minScale)
            : base(anim)
        {
            Origin = Vector2.Zero;
            this.lowerBound = minScale;
        }

        #endregion

        #region Update & Draw
        public override void Update(GameTime gameTime)
        {
            float ratio = (float)Math.Sin(gameTime.TotalGameTime.Milliseconds/155);
            scale.X = (ratio + 1 + lowerBound)/5;
            scale.Y = scale.X;
            base.Update(gameTime);
        }
        #endregion
    }
}
