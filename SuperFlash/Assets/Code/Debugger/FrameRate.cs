// David Campbell
// 1965387

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Microsoft.Xna.Framework;

/// <summary>
/// FrameRate is used to calculate and control the current frame rate of an application
/// </summary>
namespace COMP476Proj
{
    class FrameRate : GameComponent
    {
        #region Attributes

        /// <summary>
        /// Number of frames that have been displayed
        /// </summary>
        private short numberOfFrames;

        /// <summary>
        /// Time (in seconds) over which the current frame rate should be calculated
        /// </summary>
        private int evaluationPeriod;

        /// <summary>
        /// Seconds stores the numbers of seconds that have passed
        /// (to avoid calling external methods multiple times)
        /// </summary>
        private double seconds;

        /// <summary>
        /// desiredFramesPerSecond stores the desired frame rate,
        /// which can be accessed using DesiredFramesPerSecond
        /// </summary>
        private int desiredFramesPerSecond = 120;

        /// <summary>
        /// currentframesPerSecond stores the last calculated frame rate,
        /// which can be accessed using CurrentFramesPerSecond
        /// </summary>
        private double currentframesPerSecond;

        #endregion

        #region Accessors

        public int EvaluationPeriod
        {
            get { return EvaluationPeriod; }
        }

        public double DesiredFramesPerSecond
        {
            get { return desiredFramesPerSecond; }
        }

        public double CurrentFramesPerSecond
        {
            get { return currentframesPerSecond; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor. By default, time lapse is 2 seconds
        /// </summary>
        /// <param name="game">The current game instance</param>
        /// <param name="evaluationPeriod">The period of time
        /// over which frame rate should be calculated</param>
        public FrameRate(Game game, int evaluationPeriod = 2)
            : base(game)
        {
            this.evaluationPeriod = evaluationPeriod;

            numberOfFrames = 0;

            game.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / desiredFramesPerSecond);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Update method called automatically
        /// </summary>
        /// <param name="gameTime">The current game instance's time object</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Count frame
            ++numberOfFrames;

            // Look at elapsed time
            seconds += gameTime.ElapsedGameTime.TotalSeconds;

            // If the correct time lapse has gone by, calculate frame rate and reset values
            if (seconds >= evaluationPeriod)
            {
                currentframesPerSecond = (int)numberOfFrames / seconds;

                /*
                #if (DEBUG)
                {
                    Console.WriteLine("Frame rate at " + DateTime.Now.ToString("HH:mm:ss tt") +
                        " is " + seconds + (seconds.Equals(1) ? " second" : " seconds")
                        + " : " + currentframesPerSecond + " frames per second.");
                }
                #endif
                 * */

                seconds = 0;
                numberOfFrames = 0;
            }
        }

        /// <summary>
        /// increaseDesiredFramesPerSecond increases the desired frame rate
        /// </summary>
        public void increaseDesiredFramesPerSecond()
        {
            if (desiredFramesPerSecond < 240)
            {
                ++desiredFramesPerSecond;
                base.Game.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / desiredFramesPerSecond);
            }
        }

        /// <summary>
        /// decreaseDesiredFramesPerSecond decreases the desired frame rate
        /// </summary>
        public void decreaseDesiredFramesPerSecond()
        {
            if (desiredFramesPerSecond > 15)
            {
                --desiredFramesPerSecond;
                base.Game.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / desiredFramesPerSecond);
            }
        }

        /// <summary>
        /// Synchronize sets the desired frame rate to the last recorded frame rate
        /// </summary>
        public void synchronize()
        {
            desiredFramesPerSecond = (int)currentframesPerSecond;
        }

        #endregion
    }
}