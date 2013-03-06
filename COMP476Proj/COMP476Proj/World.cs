#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace COMP476Proj
{
    public class World
    {
        #region Fields
        public Streaker streaker;
        public List<BoundingBox> mapBoundingBoxes;
        #endregion

        #region Init
        public World()
        {
            streaker = EntityBuilder.buildStreaker();
        }
        #endregion

        #region Update & Draw
        public void Update(GameTime gameTime)
        {
            streaker.Update(gameTime);
            //for each enemy --> update
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            streaker.Draw(gameTime, spriteBatch);
            
        }
        #endregion
    }
}
