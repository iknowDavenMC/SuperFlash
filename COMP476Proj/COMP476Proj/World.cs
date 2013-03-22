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
        //public Camera camera;
        #endregion

        #region Init
        public World()
        {
            streaker = EntityBuilder.buildStreaker();
            //camera = new Camera();
        }
        #endregion

        #region Update & Draw
        public void Update(GameTime gameTime)
        {
            streaker.Update(gameTime);
            Camera.X = (int)streaker.physics.position.X - Camera.Width/2;
            Camera.Y = (int)streaker.physics.position.Y - Camera.Height/2;
            if (Camera.X < 0)
                Camera.X = 0;
            if (Camera.Y < 0)
                Camera.Y = 0;
            //for each enemy --> update
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 drawPos = new Vector2(-Camera.X, -Camera.Y);
            spriteBatch.Draw(SpriteDatabase.GetAnimation("level1").Texture, drawPos, Color.White);
            streaker.Draw(gameTime, spriteBatch);
        }
        #endregion
    }
}
