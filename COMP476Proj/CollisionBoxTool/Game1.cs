using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace CollisionBoxTool
{
    public enum DrawModes { Box, Node, Edge }
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        const int WIDTH = 800;
        const int HEIGHT = 600;

        int maxW, maxH, camSpeed;

        Rectangle Camera;

        Texture2D blank;
        Texture2D circle;
        Texture2D scene;
        SpriteFont font;

        List<Rectangle> boxes;
        List<Node> nodes;
        List<Edge> edges;

        Color boxColor;
        Color nodeColor;
        Color edgeColor;
        Vector2 startPos, endPos;
        Rectangle mouseRect;
        Node selected;
        Node mouseNode;
        Edge mouseEdge;

        DrawModes mode = DrawModes.Box;

        public int circler = 16;
        bool zpressed = false;
        bool mpressed = false;

        bool drawing = false;
        bool saving = false;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();
            this.IsMouseVisible = true;

            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Font");
            blank = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            blank.SetData(new[] { Color.White });
            circle = new Texture2D(GraphicsDevice, circler * 2, circler * 2, false, SurfaceFormat.Color);
            Color[] circleData = new Color[circler * 2 * circler * 2];
            for (int i = 0; i != circler * 2 * circler * 2; i++)
            {
                int x = (i % (circler * 2)) - circler;
                int y = (i / (circler * 2)) - circler;
                if (x * x + y * y <= circler * circler)
                    circleData[i] = Color.White;
                else
                    circleData[i] = Color.Transparent;
            }
            circle.SetData(circleData);
            scene = Content.Load<Texture2D>("scene");

            maxW = scene.Width;
            maxH = scene.Height;
            camSpeed = 5;

            startPos = new Vector2();
            endPos = new Vector2();
            mouseRect = new Rectangle();
            selected = null;
            mouseNode = new Node(0, 0);
            mouseEdge = new Edge(null, null);

            boxes = new List<Rectangle>();
            nodes = new List<Node>();
            edges = new List<Edge>();

            boxColor = new Color(0.35f, 0.5f, 0.85f, 0.5f);
            nodeColor = new Color(0f, 0.2f, 1f, 0.75f);
            edgeColor = new Color(0.33f, 1f, 0f, 1f);

            Camera = new Rectangle(0, 0, 800, 600);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState ks = Keyboard.GetState();
            MouseState ms = Mouse.GetState();
            // Allows the game to exit
            if (ks.IsKeyDown(Keys.Escape))
                this.Exit();

            int dx = 0, dy = 0;
            if (ks.IsKeyDown(Keys.Left))
            {
                dx = -camSpeed;
            }
            if (ks.IsKeyDown(Keys.Right))
            {
                dx = camSpeed;
            }
            if (ks.IsKeyDown(Keys.Up))
            {
                dy = -camSpeed;
            }
            if (ks.IsKeyDown(Keys.Down))
            {
                dy = camSpeed;
            }
            if (ks.IsKeyDown(Keys.Z) && !zpressed)
            {
                if (boxes.Count > 0)
                    boxes.RemoveAt(boxes.Count - 1);
                zpressed = true;
            }
            if (ks.IsKeyDown(Keys.M) && !mpressed)
            {
                if (mode == DrawModes.Box)
                    mode = DrawModes.Node;
                else
                    mode = DrawModes.Box;
                mpressed = true;
            }
            if (ks.IsKeyDown(Keys.C))
            {
                boxes.Clear();
            }
            if (ks.IsKeyDown(Keys.S) && !saving)
            {
                save();
                saving = true;
            }
            if (ks.IsKeyUp(Keys.S))
                saving = false;
            if (ks.IsKeyUp(Keys.Z))
                zpressed = false;
            if (ks.IsKeyUp(Keys.M))
                mpressed = false;

            if (ms.LeftButton == ButtonState.Pressed)
            {
                if (mode == DrawModes.Box)
                {
                    if (!drawing)
                    {
                        drawing = true;
                        startPos.X = ms.X + Camera.X;
                        startPos.Y = ms.Y + Camera.Y;
                    }
                    endPos.X = ms.X + Camera.X;
                    endPos.Y = ms.Y + Camera.Y;
                    mouseRect.X = (int)Math.Min(startPos.X, endPos.X);
                    mouseRect.Y = (int)Math.Min(startPos.Y, endPos.Y);
                    mouseRect.Width = (int)Math.Abs(startPos.X - endPos.X);
                    mouseRect.Height = (int)Math.Abs(startPos.Y - endPos.Y);
                }
                else
                {
                    mouseNode.position = new Vector2(ms.X + Camera.X, ms.Y + Camera.Y);
                    if (!drawing)
                    {
                        foreach (Node node in nodes)
                        {
                            if (node.pointInside(ms.X + Camera.X, ms.Y + Camera.Y))
                            {
                                selected = node;
                                break;
                            }
                        }
                        drawing = true;
                    }
                    else if (selected != null)
                    {
                        mouseEdge.start = selected;
                        mouseEdge.end = mouseNode;
                    }

                }
            }
            if (ms.LeftButton == ButtonState.Released && drawing)
            {
                if (mode == DrawModes.Box)
                {
                    drawing = false;
                    boxes.Add(new Rectangle(mouseRect.X, mouseRect.Y, mouseRect.Width, mouseRect.Height));
                }
                else
                {
                    drawing = false;
                    if (selected != null)
                    {
                        foreach (Node node in nodes)
                        {
                            if (node != selected && node.pointInside(ms.X + Camera.X, ms.Y + Camera.Y))
                            {
                                mouseEdge.end = node;
                                edges.Add(mouseEdge);
                                mouseEdge = new Edge(null, null);
                                selected = null;
                                break;
                            }
                        }
                    }
                    else 
                    {
                        nodes.Add(mouseNode);
                        mouseNode = new Node(ms.X + Camera.X, ms.Y + Camera.Y);
                    }

                }
            }

            if (ms.RightButton == ButtonState.Pressed && !drawing)
            {
                if (mode == DrawModes.Box)
                {
                    for (int i = 0; i != boxes.Count; i++)
                    {
                        Rectangle r = boxes[i];
                        if (r.Contains(ms.X + Camera.X, ms.Y + Camera.Y))
                        {
                            boxes.Remove(r);
                            i--;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i != nodes.Count; i++)
                    {
                        Node n = nodes[i];
                        if (n.pointInside(ms.X + Camera.X, ms.Y + Camera.Y))
                        {
                            nodes.Remove(n);
                            for (int j = 0; j != edges.Count; j++)
                            {
                                Edge e = edges[j];
                                if (e.start == n || e.end == n)
                                {
                                    edges.Remove(e);
                                    j--;
                                }
                            }
                            i--;
                        }
                    }
                }
            }

            Camera.X += dx;
            Camera.Y += dy;

            if (Camera.X < 0)
                Camera.X = 0;
            if (Camera.Y < 0)
                Camera.Y = 0;
            if (Camera.X + Camera.Width > maxW)
                Camera.X = maxW - Camera.Width;
            if (Camera.Y + Camera.Height > maxH)
                Camera.Y = maxH - Camera.Height;

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            spriteBatch.Draw(scene, new Vector2(-Camera.X, -Camera.Y), Color.White);
            foreach (Rectangle box in boxes)
            {
                Rectangle draw = new Rectangle(box.X - Camera.X, box.Y - Camera.Y, box.Width, box.Height);
                spriteBatch.Draw(blank, draw, boxColor);
            }
            foreach (Node node in nodes)
            {
                node.draw(spriteBatch, circle, nodeColor, Camera);
            }

            foreach (Edge edge in edges)
            {
                edge.draw(spriteBatch, blank, edgeColor, Camera);
            }
            if (drawing)
            {
                if (mode == DrawModes.Box)
                {
                    Rectangle mouseDraw = new Rectangle(mouseRect.X - Camera.X, mouseRect.Y - Camera.Y, mouseRect.Width, mouseRect.Height);
                    spriteBatch.Draw(blank, mouseDraw, boxColor);
                }
                else
                {
                    mouseEdge.draw(spriteBatch, blank, Color.Blue, Camera);
                }
            }
            String modestr = "MODE: " + (mode == DrawModes.Box ? "BOX" : "NODE");
            spriteBatch.DrawString(font, modestr, new Vector2(12, 12), Color.Black);
            spriteBatch.DrawString(font, modestr, new Vector2(11, 12), Color.Black);
            spriteBatch.DrawString(font, modestr, new Vector2(10, 12), Color.Black);
            spriteBatch.DrawString(font, modestr, new Vector2(10, 11), Color.Black);
            spriteBatch.DrawString(font, modestr, new Vector2(10, 10), Color.Black);
            spriteBatch.DrawString(font, modestr, new Vector2(11, 10), Color.Black);
            spriteBatch.DrawString(font, modestr, new Vector2(12, 10), Color.Black);
            spriteBatch.DrawString(font, modestr, new Vector2(12, 11), Color.Black);
            spriteBatch.DrawString(font, modestr, new Vector2(11, 11), Color.White);
            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        protected void save()
        {
            Stream fs = File.Open("out.txt", FileMode.Create);
            StreamWriter writer = new StreamWriter(fs);
            writer.WriteLine("RECTANGLES");
            foreach (Rectangle box in boxes)
            {
                string line = box.X + " " + box.Y + " " + box.Width + " " + box.Height;

                writer.WriteLine(line);
            }
            writer.WriteLine("NODES");
            foreach (Node node in nodes)
            {
                string line = node.ID + " " + node.position.X + " " + node.position.Y;
                writer.WriteLine(line);
            }
            writer.WriteLine("EDGES");
            foreach (Edge edge in edges)
            {
                string line = edge.start.ID + " " + edge.end.ID;
                writer.WriteLine(line);
            }
            writer.Close();
            fs.Close();
        }
    }
}
