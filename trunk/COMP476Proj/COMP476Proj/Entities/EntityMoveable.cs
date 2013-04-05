using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COMP476Proj
{
    public class EntityMoveable : EntityVisible
    {
        #region Fields
        protected PhysicsComponent2D physics;
        #endregion

        #region Properties
        public PhysicsComponent2D ComponentPhysics
        {
            get { return physics; }
        }
        #endregion

    }
}
