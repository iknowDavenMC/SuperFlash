using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StreakerLibrary;
using UnityEngine;
using Assets.Code._XNA;

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
        public DrawOscillate(CustomAnimation anim, Vector2 pos, float depth, bool startOscillating)
            : base(anim)
        {
            Origin = Vector2.zero;
            this.position = pos;
            this.depth = depth;
            oscillateAlpha = startOscillating;
            oscillateSize = false;
        }

        public DrawOscillate(CustomAnimation anim, float depth)
            : base(anim)
        {
            Origin = Vector2.zero;
            this.depth = depth;

        }
        //public DrawOscillate(Animation anim, Vector2 origin) : base(anim) {
        //    Origin = Vector2.zero;
        //}

        //public DrawOscillate(Animation anim, Vector2 origin, float depth)
        //    : base(anim)
        //{
        //    Origin = Vector2.zero;
        //    this.depth = depth;
        //}

        #endregion

        #region Update & Draw
        public override void Update()
        {
            float ratio =  (float)Mathf.Sin(Time.time * 1000f / 155);;
            if (oscillateSize)
            {
                scale.x = (ratio + 1 + lowerBound) / 5;
                scale.y = scale.x;
            }
            if (oscillateAlpha)
            {
                alpha = ratio*.5f+.7f;
            }

            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch, float offsetX, float offsetY)
        {
            base.Draw(spriteBatch, offsetX, offsetY);
        }
        #endregion
    }
}
