using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using StreakerLibrary;

namespace COMP476Proj.Entities
{
    public abstract class NPC : EntityMoveable
    {
        #region Fields
        protected MovementAIComponent2D movement;
        protected float detectRadius = 40f;
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
