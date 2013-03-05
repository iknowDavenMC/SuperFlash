#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace COMP476Proj
{
    public enum Level { MAIN_MENU, SCHOOL }
 
    public class LevelManager
    {
        #region Fields
        Level lvl;
        #endregion

        #region Init
        public LevelManager()
        {
            lvl = Level.MAIN_MENU;
        }
        #endregion
    }
}
