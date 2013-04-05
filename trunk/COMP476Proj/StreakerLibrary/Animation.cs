using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace StreakerLibrary
{
    public class Animation
    {
        /*-------------------------------------------------------------------------*/
        #region Fields

        public string animationId;
        Texture2D texture;
        protected int numOfColumns;
        protected int frameWidth;
        protected int frameHeight;
        protected int yPos = 0;
        protected int timePerFrame = -1;

        #endregion

        /*-------------------------------------------------------------------------*/
        #region Properties

        public string AnimationId
        {
            get { return animationId; }
        }

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public int FrameWidth
        {
            get { return frameWidth; }
        }

        public int FrameHeight
        {
            get { return frameHeight; }
        }

        public int YPos
        {
            get { return yPos; }
        }

        public int NumOfColumns
        {
            get { return numOfColumns; }
        }

        public int TimePerFrame
        {
            get { return timePerFrame; }
        }

        #endregion

        /*-------------------------------------------------------------------------*/
        #region Init

        public Animation(string animationId, Texture2D texture)
        {
            this.animationId = animationId;
            this.texture = texture;
            numOfColumns = 1;
            frameWidth = texture.Width;
            frameHeight = texture.Height;
        }

        public Animation(string animationID, Texture2D texture, int numOfCol, int frameWid, int frameHt, int y, int timePerFrame)
        {
            this.timePerFrame = timePerFrame;
            this.animationId = animationID;
            this.texture = texture;
            numOfColumns = numOfCol;
            frameWidth = frameWid;
            frameHeight = frameHt;
            yPos = y;
        }

        #endregion
    }
}
