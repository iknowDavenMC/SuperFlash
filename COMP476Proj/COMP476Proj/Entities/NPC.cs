using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using StreakerLibrary;

namespace COMP476Proj
{
    public abstract class NPC : EntityMoveable
    {
        #region Fields
        protected MovementAIComponent2D movement;
        protected float detectRadius = 300f;
        protected float farRadius = 500f;
        public Node patrolStart = null, patrolEnd = null;
        public Flock flock;

        public static int copsWhoSeeTheStreaker = 0;

        #endregion

        #region Properties
        public MovementAIComponent2D ComponentMovement
        {
            get { return movement; }
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

    }
}
