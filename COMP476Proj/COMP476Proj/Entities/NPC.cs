using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COMP476Proj.Entities
{
    public class NPC : EntityMoveable
    {
        #region Fields
        protected MovementAIComponent2D movement;
        #endregion

        #region Properties
        public MovementAIComponent2D ComponentBrain
        {
            get { return movement; }
        }
        #endregion
    }
}
