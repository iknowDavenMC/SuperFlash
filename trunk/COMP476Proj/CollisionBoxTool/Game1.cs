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
using Microsoft.Xna.Framework.Storage;
using StreakerLibrary;

namespace CollisionBoxTool
{
    public enum DrawModes { Box, Node, NPC, Trigger, Player }
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        const int WIDTH = 1024;
        const int HEIGHT = 768;

        int maxW, maxH, camSpeed;

        Rectangle Camera;

        Texture2D blank;
        Texture2D circle;
        Texture2D scene;
        SpriteFont font;

        List<Box> boxes;
        List<Node> nodes;
        List<Edge> edges;
        List<NPC> NPCs;
        List<Trigger> triggers;
        NPC player;

        Color boxColor;
        Color wallColor;
        Color triggerColor;
        Color nodeColor;
        Color edgeColor;
        Vector2 startPos, endPos;
        Box mouseBox;
        Node selected;
        NPC selectedNPC;
        Node selectedStart;
        Node selectedEnd;
        Node mouseNode;
        Edge mouseEdge;
        NPC mouseNPC;
        NPC mousePlayer;

        NPC.Type npcType = NPC.Type.Civilian;
        NPC.Mode npcMode = NPC.Mode.Static;

        DrawModes mode = DrawModes.Box;

        public int circler = 16;
        bool mpressed = false;
        bool wpressed = false;
        bool lpressed = false;

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
            graphics.PreferredBackBufferWidth = WIDTH;
            graphics.PreferredBackBufferHeight = HEIGHT;
            //graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            this.IsMouseVisible = true;

            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Fonts/AchievementText");
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
            scene = Content.Load<Texture2D>("levelDesign_Full");

            maxW = scene.Width;
            maxH = scene.Height;
            camSpeed = 5;

            startPos = new Vector2();
            endPos = new Vector2();
            mouseBox = new Box(0, 0, 0, 0, false);
            selected = null;
            mouseNode = new Node(0, 0);
            mouseEdge = new Edge(null, null);
            mouseNPC = new NPC(0, 0, npcType, npcMode);
            mousePlayer = new NPC(0, 0, NPC.Type.Streaker, NPC.Mode.Static);

            boxes = new List<Box>();
            nodes = new List<Node>();
            edges = new List<Edge>();
            NPCs = new List<NPC>();
            triggers = new List<Trigger>();
            player = null;

            boxColor = new Color(1f, 0.5f, 0.05f, 0.5f);
            wallColor = new Color(1f, 0.0f, 0.0f, 0.5f);
            triggerColor = new Color(0.5f, 1.0f, 0.0f, 0.5f);
            nodeColor = new Color(0f, 0.2f, 1f, 0.75f);
            edgeColor = Color.LightCyan;

            SpriteDatabase.loadSprites(Content);

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

            if (ks.IsKeyDown(Keys.D1) && mode == DrawModes.NPC)
            {
                npcType = NPC.Type.Civilian;
                mouseNPC.type = NPC.Type.Civilian;
            }
            if (ks.IsKeyDown(Keys.D2) && mode == DrawModes.NPC)
            {
                npcType = NPC.Type.DumbCop;
                mouseNPC.type = NPC.Type.DumbCop;
            }
            if (ks.IsKeyDown(Keys.D3) && mode == DrawModes.NPC)
            {
                npcType = NPC.Type.SmartCop;
                mouseNPC.type = NPC.Type.SmartCop;
            }
            if (ks.IsKeyDown(Keys.D4) && mode == DrawModes.NPC)
            {
                npcType = NPC.Type.RoboCop;
                mouseNPC.type = NPC.Type.RoboCop;
            }

            if (ks.IsKeyDown(Keys.L) && !lpressed)
            {
                load("level.txt");
                lpressed = true;
            }

            if (ks.IsKeyDown(Keys.M) && !mpressed)
            {
                if (mode == DrawModes.Box)
                    mode = DrawModes.Node;
                else if (mode == DrawModes.Node)
                    mode = DrawModes.NPC;
                else if (mode == DrawModes.NPC)
                    mode = DrawModes.Trigger;
                else if (mode == DrawModes.Trigger)
                    mode = DrawModes.Player;
                else if (mode == DrawModes.Player)
                    mode = DrawModes.Box;
                mpressed = true;
            }

            if (ks.IsKeyDown(Keys.S) && !saving)
            {
                save();
                saving = true;
            }

            if (ks.IsKeyDown(Keys.W) && !wpressed)
            {
                if (mode == DrawModes.Box)
                {
                    mouseBox.seeThrough = !mouseBox.seeThrough;
                }
                else if (mode == DrawModes.NPC)
                {
                    if (npcMode == NPC.Mode.Static)
                        npcMode = NPC.Mode.Wander;
                    else if (npcMode == NPC.Mode.Wander)
                        npcMode = NPC.Mode.Patrol;
                    else if (npcMode == NPC.Mode.Patrol)
                        npcMode = NPC.Mode.Static;
                    mouseNPC.mode = npcMode;
                }
                wpressed = true;
            }

            if (ks.IsKeyUp(Keys.L))
                lpressed = false;
            if (ks.IsKeyUp(Keys.M))
                mpressed = false;
            if (ks.IsKeyUp(Keys.S))
                saving = false;
            if (ks.IsKeyUp(Keys.W))
                wpressed = false;

            if (ms.LeftButton == ButtonState.Pressed)
            {
                if (mode == DrawModes.Box || mode == DrawModes.Trigger)
                {
                    if (!drawing)
                    {
                        drawing = true;
                        startPos.X = ms.X + Camera.X;
                        startPos.Y = ms.Y + Camera.Y;
                    }
                    endPos.X = ms.X + Camera.X;
                    endPos.Y = ms.Y + Camera.Y;
                    mouseBox.rect.X = (int)Math.Min(startPos.X, endPos.X);
                    mouseBox.rect.Y = (int)Math.Min(startPos.Y, endPos.Y);
                    mouseBox.rect.Width = (int)Math.Abs(startPos.X - endPos.X);
                    mouseBox.rect.Height = (int)Math.Abs(startPos.Y - endPos.Y);
                }
                else if (mode == DrawModes.Node)
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
                else if (mode == DrawModes.NPC)
                {
                    mouseNPC.position = new Vector2(ms.X + Camera.X, ms.Y + Camera.Y);
                    if (!drawing)
                    {
                        foreach (NPC npc in NPCs)
                        {
                            if (npc.pointInside(ms.X + Camera.X, ms.Y + Camera.Y))
                            {
                                selectedNPC = npc;
                                break;
                            }
                        }
                        drawing = true;
                    }
                    else if (selectedNPC != null)
                    {

                        foreach (Node node in nodes)
                        {
                            if (node.pointInside(ms.X + Camera.X, ms.Y + Camera.Y))
                            {
                                selectedStart = node;
                                break;
                            }
                        }
                    }
                }
                else if (mode == DrawModes.Player)
                {
                    mousePlayer.position = new Vector2(ms.X + Camera.X, ms.Y + Camera.Y);
                    if (!drawing)
                    {
                        drawing = true;
                    }
                }
            }

            if (ms.LeftButton == ButtonState.Released && drawing)
            {
                if (mode == DrawModes.Box)
                {
                    drawing = false;
                    if (mouseBox.rect.Width > 0 && mouseBox.rect.Height > 0)
                        boxes.Add(new Box(mouseBox.rect.X, mouseBox.rect.Y, mouseBox.rect.Width, mouseBox.rect.Height, mouseBox.seeThrough));
                }
                else if (mode == DrawModes.Node)
                {
                    drawing = false;
                    if (selectedNPC != null)
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
                        selected = null;
                        mouseEdge = new Edge(null, null);
                    }
                    else
                    {
                        nodes.Add(mouseNode);
                        mouseNode = new Node(ms.X + Camera.X, ms.Y + Camera.Y);
                    }
                }
                else if (mode == DrawModes.NPC && ms.RightButton == ButtonState.Released)
                {
                    drawing = false;
                    if (ms.RightButton == ButtonState.Released && selectedEnd == null)
                    {
                        if (selectedNPC != null)
                        {
                            foreach (Node node in nodes)
                            {
                                if (node == selectedStart && node.pointInside(ms.X + Camera.X, ms.Y + Camera.Y))
                                {
                                    selectedNPC.patrolStart = node;
                                    break;
                                }
                            }
                            selectedStart = null;
                            mouseEdge = new Edge(null, null);
                        }
                        else
                        {
                            NPCs.Add(mouseNPC);
                            mouseNPC = new NPC(ms.X + Camera.X, ms.Y + Camera.Y, npcType, npcMode);
                        }
                    }

                }
                else if (mode == DrawModes.Trigger)
                {
                    drawing = false;
                    if (mouseBox.rect.Width > 0 && mouseBox.rect.Height > 0)
                        triggers.Add(new Trigger(mouseBox.rect.X, mouseBox.rect.Y, mouseBox.rect.Width, mouseBox.rect.Height));
                }
                else if (mode == DrawModes.Player)
                {
                    drawing = false;
                    player = mousePlayer;
                    mousePlayer = new NPC(ms.X + Camera.X, ms.Y + Camera.Y, NPC.Type.Streaker, NPC.Mode.Static);

                }
            }

            if (ms.RightButton == ButtonState.Pressed)
            {
                if (mode == DrawModes.NPC)
                {
                    mouseNPC.position = new Vector2(ms.X + Camera.X, ms.Y + Camera.Y);
                    if (!drawing)
                    {
                        foreach (NPC npc in NPCs)
                        {
                            if (npc.pointInside(ms.X + Camera.X, ms.Y + Camera.Y))
                            {
                                selectedNPC = npc;
                                break;
                            }
                        }
                        drawing = true;
                    }
                    else if (selectedNPC != null)
                    {
                        foreach (Node node in nodes)
                        {
                            if (node.pointInside(ms.X + Camera.X, ms.Y + Camera.Y))
                            {
                                selectedEnd = node;
                                break;
                            }
                        }
                    }
                }
                else if (!drawing)
                {
                    if (mode == DrawModes.Box)
                    {
                        for (int i = 0; i != boxes.Count; i++)
                        {
                            Box r = boxes[i];
                            if (r.rect.Contains(ms.X + Camera.X, ms.Y + Camera.Y))
                            {
                                boxes.Remove(r);
                                i--;
                            }
                        }
                    }
                    else if (mode == DrawModes.Node)
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
                    //else if (mode == DrawModes.NPC)
                    //{
                    //    for (int i = 0; i != NPCs.Count; i++)
                    //    {
                    //        NPC n = NPCs[i];
                    //        if (n.pointInside(ms.X + Camera.X, ms.Y + Camera.Y))
                    //        {
                    //            NPCs.Remove(n);
                    //            i--;
                    //        }
                    //    }
                    //}
                    else if (mode == DrawModes.Trigger)
                    {
                        for (int i = 0; i != triggers.Count; i++)
                        {
                            Trigger t = triggers[i];
                            if (t.rect.Contains(ms.X + Camera.X, ms.Y + Camera.Y))
                            {
                                triggers.Remove(t);
                                i--;
                            }
                        }
                    }
                    else if (mode == DrawModes.Player)
                    {
                        if (player != null && player.pointInside(ms.X + Camera.X, ms.Y + Camera.Y))
                        {
                            player = null;
                        }
                    }
                }
            }

            if (ms.RightButton == ButtonState.Released)
            {
                if (mode == DrawModes.NPC && ms.LeftButton == ButtonState.Released)
                {
                    if (selectedNPC != null)
                    {
                        for (int i = 0; i != NPCs.Count; i++)
                        {
                            NPC n = NPCs[i];
                            if (n.pointInside(ms.X + Camera.X, ms.Y + Camera.Y))
                            {
                                NPCs.Remove(n);
                                i--;
                            }
                        }
                        foreach (Node node in nodes)
                        {
                            if (node == selectedEnd && node.pointInside(ms.X + Camera.X, ms.Y + Camera.Y))
                            {
                                selectedNPC.patrolEnd = node;
                                break;
                            }
                        }
                        selectedEnd = null;
                        selectedNPC= null;
                        mouseEdge = new Edge(null, null);
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
            foreach (Box box in boxes)
            {
                Rectangle draw = new Rectangle(box.rect.X - Camera.X, box.rect.Y - Camera.Y, box.rect.Width, box.rect.Height);
                spriteBatch.Draw(blank, draw, box.seeThrough ? boxColor : wallColor);
            }
            foreach (Trigger trigger in triggers)
            {
                Rectangle draw = new Rectangle(trigger.rect.X - Camera.X, trigger.rect.Y - Camera.Y, trigger.rect.Width, trigger.rect.Height);
                spriteBatch.Draw(blank, draw, triggerColor);
                drawBordered(spriteBatch,
                    trigger.rect.X + trigger.rect.Width / 2 - Camera.X,
                    trigger.rect.Y + trigger.rect.Height / 2 - Camera.Y,
                    "" + trigger.id, Color.Black, Color.White);
            }
            foreach (Node node in nodes)
            {
                node.draw(spriteBatch, circle, nodeColor, Camera);
            }

            foreach (Edge edge in edges)
            {
                edge.draw(spriteBatch, blank, edgeColor, Camera);
            }
            foreach (NPC npc in NPCs)
            {
                npc.draw(spriteBatch, circle, Camera);
                drawBordered(spriteBatch, npc.X - Camera.X, npc.Y - Camera.Y, npc.mode.ToString().Substring(0,1), Color.Black, Color.White);
                //switch (npc.mode)
                //{
                //    case NPC.Mode.Static:
                //        drawBordered(spriteBatch, npc.X - Camera.X, npc.Y - Camera.Y, "S", Color.Black, Color.White);
                //        break;
                //    case NPC.Mode.Wander:
                //        drawBordered(spriteBatch, npc.X - Camera.X, npc.Y - Camera.Y, "W", Color.Black, Color.White);
                //        break;
                //}
                if (npc.patrolStart != null)
                {
                    drawLine(spriteBatch, npc.position, npc.patrolStart.position, Color.LightGreen);
                }
                if (npc.patrolEnd != null)
                {
                    drawLine(spriteBatch, npc.position, npc.patrolEnd.position, Color.Pink);
                }
            }
            if (player != null)
                player.draw(spriteBatch, circle, Camera);
            if (drawing)
            {
                if (mode == DrawModes.Box || mode == DrawModes.Trigger)
                {
                    Rectangle mouseDraw = new Rectangle(mouseBox.rect.X - Camera.X, mouseBox.rect.Y - Camera.Y, mouseBox.rect.Width, mouseBox.rect.Height);
                    spriteBatch.Draw(blank, mouseDraw, (mode == DrawModes.Box ? (mouseBox.seeThrough ? boxColor : wallColor) : triggerColor));
                }
                else if (mode == DrawModes.Node)
                {
                    mouseEdge.draw(spriteBatch, blank, Color.Blue, Camera);
                }
                else if (mode == DrawModes.NPC)
                {
                    if (selectedNPC != null)
                        drawLine(spriteBatch, mouseNPC.position, selectedNPC.position, Color.LightBlue);
                    else 
                    mouseNPC.draw(spriteBatch, circle, Camera);
                }
                else if (mode == DrawModes.Player)
                {
                    mousePlayer.draw(spriteBatch, circle, Camera);
                }
            }
            String modestr = "MODE: " + mode.ToString();
            drawBordered(spriteBatch, 10, 10, modestr, Color.Black, Color.White);
            if (mode == DrawModes.Box)
            {
                drawBordered(spriteBatch, 10, 28, "[W]    " + (mouseBox.seeThrough ? "See Through" : "Wall"), Color.Black, Color.White);
            }
            if (mode == DrawModes.NPC)
            {
                drawBordered(spriteBatch, 10, 28, "[1,2,3,4]", Color.Black, Color.White);
                drawBordered(spriteBatch, 110, 28, "Civilian", Color.Black, (npcType == NPC.Type.Civilian ? Color.LimeGreen : Color.White));
                drawBordered(spriteBatch, 210, 28, "Dumb Cop", Color.Black, (npcType == NPC.Type.DumbCop ? Color.Crimson : Color.White));
                drawBordered(spriteBatch, 310, 28, "SmartCop", Color.Black, (npcType == NPC.Type.SmartCop ? Color.Orange : Color.White));
                drawBordered(spriteBatch, 410, 28, "RoboCop", Color.Black, (npcType == NPC.Type.RoboCop ? Color.Coral : Color.White));
                drawBordered(spriteBatch, 10, 46, "[W]", Color.Black, Color.White);
                drawBordered(spriteBatch, 50, 46, npcMode.ToString(), Color.Black, Color.White);
            }
            drawBordered(spriteBatch, 575, 10, "[S] Save to out.txt", Color.Black, Color.White);
            drawBordered(spriteBatch, 575, 28, "[L] Load level.txt", Color.Black, Color.White);
            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        public void drawBordered(SpriteBatch spriteBatch, int x, int y, string str, Color border, Color text)
        {
            spriteBatch.DrawString(font, str, new Vector2(x - 1, y - 1), border);
            spriteBatch.DrawString(font, str, new Vector2(x - 1, y), border);
            spriteBatch.DrawString(font, str, new Vector2(x - 1, y + 1), border);
            spriteBatch.DrawString(font, str, new Vector2(x, y + 1), border);
            spriteBatch.DrawString(font, str, new Vector2(x + 1, y + 1), border);
            spriteBatch.DrawString(font, str, new Vector2(x + 1, y), border);
            spriteBatch.DrawString(font, str, new Vector2(x + 1, y - 1), border);
            spriteBatch.DrawString(font, str, new Vector2(x, y - 1), border);
            spriteBatch.DrawString(font, str, new Vector2(x, y), text);
        }

        public void drawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color)
        {
            float dx = start.X - end.X;
            float dy = start.Y - end.Y;
            double length = Math.Sqrt(dx * dx + dy * dy);
            Vector2 mid = new Vector2(end.X + dx / 2, end.Y + dy / 2);
            float angle = (float)Math.Atan2(dy, dx);
            Rectangle drawRect = new Rectangle(
                (int)end.X - Camera.X,
                (int)end.Y - Camera.Y,
                (int)length, 3);
            //sb.Draw(tex, drawRect, color);
            spriteBatch.Draw(blank, drawRect, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
        }

        protected void save()
        {
            Stream fs = File.Open("out.txt", FileMode.Create);
            StreamWriter writer = new StreamWriter(fs);
            writer.WriteLine("RECTANGLES");
            foreach (Box box in boxes)
            {
                string line = box.rect.X + " " + box.rect.Y + " " + box.rect.Width + " " + box.rect.Height + " " + (box.seeThrough ? "true" : "false");

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
            writer.WriteLine("NPCS");
            foreach (NPC npc in NPCs)
            {
                string line = npc.X + " " + npc.Y + " " + npc.type.ToString() + " " + npc.mode.ToString() + " " + (npc.patrolStart == null ? 0 : npc.patrolStart.ID ) + " "  + (npc.patrolEnd == null ? 0 : npc.patrolEnd.ID);
                writer.WriteLine(line);
            }
            writer.WriteLine("TRIGGER");
            foreach (Trigger trigger in triggers)
            {
                string line = trigger.rect.X + " " + trigger.rect.Y + " " + trigger.rect.Width + " " + trigger.rect.Height + " " + trigger.id;

                writer.WriteLine(line);
            }
            writer.WriteLine("PLAYER");
            writer.WriteLine(player.X + " " + player.Y);
            writer.Close();
            fs.Close();
        }

        protected void load(string filename)
        {
            boxes.Clear();
            nodes.Clear();
            edges.Clear();
            NPCs.Clear();
            player = null;

            StreamReader reader = new StreamReader(filename);
            try
            {
                string line = reader.ReadLine();
                if (line.StartsWith("RECTANGLES"))
                    line = reader.ReadLine();
                do
                {
                    int x, y, w, h;
                    bool seeThrough;
                    int spacei = line.IndexOf(' ');
                    x = int.Parse(line.Substring(0, spacei));
                    line = line.Substring(spacei + 1);
                    spacei = line.IndexOf(' ');
                    y = int.Parse(line.Substring(0, spacei));
                    line = line.Substring(spacei + 1);
                    spacei = line.IndexOf(' ');
                    w = int.Parse(line.Substring(0, spacei));
                    line = line.Substring(spacei + 1);
                    spacei = line.IndexOf(' ');
                    h = int.Parse(line.Substring(0, spacei));
                    line = line.Substring(spacei + 1);
                    seeThrough = bool.Parse(line);
                    boxes.Add(new Box(x, y, w, h, seeThrough));
                    line = reader.ReadLine();
                } while (reader.Peek() != -1
                    && !line.StartsWith("NODES")
                    && !line.StartsWith("EDGES")
                    && !line.StartsWith("NPCS")
                    && !line.StartsWith("TRIGGER")
                    && !line.StartsWith("PLAYER")
                    );

                if (line.StartsWith("NODES"))
                    line = reader.ReadLine();
                do
                {
                    int id, x, y;
                    int spacei = line.IndexOf(' ');
                    id = int.Parse(line.Substring(0, spacei));
                    line = line.Substring(spacei + 1);
                    spacei = line.IndexOf(' ');
                    x = int.Parse(line.Substring(0, spacei));
                    line = line.Substring(spacei + 1);
                    y = int.Parse(line);
                    nodes.Add(new Node(x, y, id));
                    line = reader.ReadLine();
                } while (reader.Peek() != -1
                    && !line.StartsWith("EDGES")
                    && !line.StartsWith("NPCS")
                    && !line.StartsWith("TRIGGER")
                    && !line.StartsWith("PLAYER")
                    );

                if (line.StartsWith("EDGES"))
                    line = reader.ReadLine();
                do
                {
                    int sid, eid;
                    int spacei = line.IndexOf(' ');
                    sid = int.Parse(line.Substring(0, spacei));
                    line = line.Substring(spacei + 1);
                    eid = int.Parse(line);
                    edges.Add(new Edge(nodes.Find(n => n.ID == sid), nodes.Find(n => n.ID == eid)));
                    line = reader.ReadLine();
                } while (reader.Peek() != -1
                    && !line.StartsWith("NPCS")
                    && !line.StartsWith("TRIGGER")
                    && !line.StartsWith("PLAYER")
                    );

                if (line.StartsWith("NPCS"))
                    line = reader.ReadLine();
                do
                {
                    int x, y, sid, eid;
                    NPC.Type type;
                    NPC.Mode mode;
                    int spacei = line.IndexOf(' ');
                    x = int.Parse(line.Substring(0, spacei));
                    line = line.Substring(spacei + 1);
                    spacei = line.IndexOf(' ');
                    y = int.Parse(line.Substring(0, spacei));
                    line = line.Substring(spacei + 1);
                    spacei = line.IndexOf(' ');
                    type = (NPC.Type)Enum.Parse(typeof(NPC.Type), line.Substring(0, spacei));
                    line = line.Substring(spacei + 1);
                    spacei = line.IndexOf(' ');
                    mode = (NPC.Mode)Enum.Parse(typeof(NPC.Mode), line.Substring(0, spacei));
                    line = line.Substring(spacei + 1);
                    spacei = line.IndexOf(' ');
                    sid = int.Parse(line.Substring(0, spacei));
                    line = line.Substring(spacei + 1);
                    eid = int.Parse(line);
                    NPC npc = new NPC(x, y, type, mode);
                    if (sid > 0 && eid > 0)
                    {
                        npc.patrolStart = nodes.Find(n => n.ID == sid);
                        npc.patrolEnd = nodes.Find(n => n.ID == sid); 
                    }
                    NPCs.Add(npc);
                    line = reader.ReadLine();
                }
                while (reader.Peek() != -1
                    && !line.StartsWith("TRIGGER")
                    && !line.StartsWith("PLAYER")
                    );

                if (line.StartsWith("TRIGGER"))
                    line = reader.ReadLine();
                do
                {
                    int x, y, w, h, id;
                    int spacei = line.IndexOf(' ');
                    x = int.Parse(line.Substring(0, spacei));
                    line = line.Substring(spacei + 1);
                    spacei = line.IndexOf(' ');
                    y = int.Parse(line.Substring(0, spacei));
                    line = line.Substring(spacei + 1);
                    spacei = line.IndexOf(' ');
                    w = int.Parse(line.Substring(0, spacei));
                    line = line.Substring(spacei + 1);
                    spacei = line.IndexOf(' ');
                    h = int.Parse(line.Substring(0, spacei));
                    line = line.Substring(spacei + 1);
                    id = int.Parse(line);
                    triggers.Add(new Trigger(x, y, w, h, id));
                    line = reader.ReadLine();
                } while (reader.Peek() != -1
                    && !line.StartsWith("PLAYER")
                    );

                if (line.StartsWith("PLAYER"))
                    line = reader.ReadLine();
                do
                {
                    int x, y;
                    int spacei = line.IndexOf(' ');
                    x = int.Parse(line.Substring(0, spacei));
                    line = line.Substring(spacei + 1);
                    spacei = line.IndexOf(' ');
                    y = int.Parse(line);
                    player = new NPC(x, y, NPC.Type.Streaker, NPC.Mode.Static);
                    line = reader.ReadLine();
                } while (reader.Peek() != -1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
