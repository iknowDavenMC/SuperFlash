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
        private LevelManager instance;
        #endregion

        #region Init
        private LevelManager()
        {
            lvl = Level.MAIN_MENU;
        }
        #endregion

        #region Methods
        public LevelManager getInstance()
        {
            if (instance == null)
                instance = new LevelManager();
            return instance;
        }
        public void SwitchToSchool()
        {

        }
        #endregion
    }
}
