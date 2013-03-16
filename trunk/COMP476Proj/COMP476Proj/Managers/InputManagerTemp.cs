#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
#endregion

namespace COMP476Proj
{
    public class InputManagerTemp
    {
        KeyboardState prevKeyState;
        World world; 

        public InputManagerTemp(World w) {
            prevKeyState = new KeyboardState();
            world = w;
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Up))
            {
                world.streaker.moveUp();
            }
            if (keyState.IsKeyDown(Keys.Down))
            {
                world.streaker.moveDown();
            }
            if (keyState.IsKeyDown(Keys.Left))
            {
                world.streaker.moveLeft();
            }
            if (keyState.IsKeyDown(Keys.Right))
            {
                world.streaker.moveRight();
            }

            if (keyState.IsKeyDown(Keys.F) && prevKeyState.IsKeyUp(Keys.F))
            {
                world.streaker.GetIntelligenceComponent().charState = CharacterState.FALL;
            }

            if (keyState.IsKeyDown(Keys.G) && prevKeyState.IsKeyUp(Keys.G))
            {
                world.streaker.GetIntelligenceComponent().charState = CharacterState.GET_UP;
            }

            if (keyState.IsKeyDown(Keys.D) && prevKeyState.IsKeyUp(Keys.D))
            {
                world.streaker.GetIntelligenceComponent().charState = CharacterState.DANCE;
            }

            if (keyState.IsKeyDown(Keys.S) && prevKeyState.IsKeyUp(Keys.S))
            {
                world.streaker.GetIntelligenceComponent().charState = CharacterState.STATIC;
            }

            prevKeyState = keyState;
        }


    }
}
