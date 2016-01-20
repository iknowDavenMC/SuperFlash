using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.Code._XNA
{
		public enum SpriteEffects { FlipHorizontally, FlipVertically, None };
		public enum PlayerIndex { One };

		public class SpriteBatch
		{
				public SpriteBatch(GraphicsDeviceManager GraphicsDevice)
				{

				}

				public void Draw(Texture2D texture, Vector2 drawPos, Rect? sourceRect, Color color, float unkown, Vector2 origin, Vector2 scale, SpriteEffects spriteEffects, float depth)
				{

				}

				public void Draw(Texture2D texture, Vector2 drawPos, Rect? sourceRect, Color color, float unkown, Vector2 origin, float scale, SpriteEffects spriteEffects, float depth)
				{

				}

				public void Draw(Texture2D texture, Vector2 drawPos, Color color)
				{

				}

				public void DrawString(SpriteFont font, string value, Vector2 position, Color color)
				{

				}
		}

		public class SpriteFont
		{
				public Vector2 MeasureString(string text)
				{
						return Vector2.zero;
				}
		}

		public class Game
		{
				public ContentManager Content;
				public WindowClass Window;
				public GraphicsDeviceManager GraphicsDevice;

				public void Run()
				{

				}
		}

		public class ContentManager
		{
				public string RootDirectory;

				public SpriteFont Load<T>(string name) where T : SpriteFont
				{
						return new SpriteFont();
				}
		}

		public class WindowClass
		{
				public string Title;
		}

		public class GraphicsDeviceManager
		{
				public int PreferredBackBufferWidth;
				public int PreferredBackBufferHeight;

				public GraphicsDeviceManager(Game game)
				{

				}

				public void ApplyChanges()
				{

				}
		}
}
