using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace COMP476Proj
{
    class Menu
    {
        SpriteComponent menuBackground;
        Button playButton;

        public Menu()
        {
            menuBackground = new SpriteComponent(new Vector2(0.0f, 0.0f), new Vector2(SuperFlashGame.SCREEN_WIDTH, SuperFlashGame.SCREEN_HEIGHT));
            menuBackground.setOriginTopLeft();
            playButton = new Button(new Vector2(508.0f, 903.0f), new Vector2(240.0f, 75.0f));
        }
        public void LoadContent(Texture2D menuImage)
        {
            menuBackground.LoadContent(menuImage);
        }
        public void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            menuBackground.Update(gameTime);
            playButton.Update(mouse);
            if (playButton.buttonPressed())
            {
                SuperFlashGame.currentGameState = SuperFlashGame.GameState.PLAY;
            }
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            menuBackground.Draw(gameTime, spriteBatch);
        }
    }
}
