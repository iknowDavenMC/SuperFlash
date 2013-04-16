using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using StreakerLibrary;

namespace COMP476Proj
{
    public class Map
    {
        public const float WIDTH = 3436;
        public const float HEIGHT = 1122;
        
        public List<NPC> startingNPCs;
        public List<Wall> walls;
        public List<Node> nodes;
        public List<Trigger> triggers;
        public Vector2 playerStart;
        public Map()
        {
            startingNPCs = new List<NPC>();
            walls = new List<Wall>();
            nodes = new List<Node>();
            triggers = new List<Trigger>();
        }

        public void Load(string filename)
        {
            walls.Clear();
            nodes.Clear();
            startingNPCs.Clear();
            playerStart = new Vector2();

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
                    if (w > 0 && h > 0)
                        walls.Add(new Wall(new Vector2(x,y), new BoundingRectangle(x, y, w, h), seeThrough));
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
                    bool key;
                    int spacei = line.IndexOf(' ');
                    id = int.Parse(line.Substring(0, spacei));
                    line = line.Substring(spacei + 1);
                    spacei = line.IndexOf(' ');
                    x = int.Parse(line.Substring(0, spacei));
                    line = line.Substring(spacei + 1);
                    spacei = line.IndexOf(' ');
                    y = int.Parse(line.Substring(0, spacei));
                    line = line.Substring(spacei + 1);
                    key = bool.Parse(line);
                    nodes.Add(new Node(x, y, key, id));
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
                    Node n1 = nodes.Find(n => n.ID == sid);
                    Node n2 = nodes.Find(n => n.ID == eid);
                    n1.Edges.Add(new Edge(n1, n2));
                    n2.Edges.Add(new Edge(n2, n1));
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
                    string type, mode;
                    int spacei = line.IndexOf(' ');
                    x = int.Parse(line.Substring(0, spacei));
                    line = line.Substring(spacei + 1);
                    spacei = line.IndexOf(' ');
                    y = int.Parse(line.Substring(0, spacei));
                    line = line.Substring(spacei + 1);
                    spacei = line.IndexOf(' ');
                    type = line.Substring(0, spacei);
                    line = line.Substring(spacei + 1);
                    spacei = line.IndexOf(' ');
                    mode = line.Substring(0, spacei);
                    line = line.Substring(spacei + 1);
                    spacei = line.IndexOf(' ');
                    sid = int.Parse(line.Substring(0, spacei));
                    line = line.Substring(spacei + 1);
                    eid = int.Parse(line);
                    Node pStart = null, pEnd = null;
                    if (sid > 0 && eid > 0)
                    {
                        pStart = nodes.Find(n => n.ID == sid);
                        pEnd = nodes.Find(n => n.ID == sid);
                    }
                    NPC npc = null;
                    Animation animation;
                    if (type.StartsWith("Civilian"))
                    {
                        int pnum = Game1.random.Next(1, 4);
                        string pedAnim = "student" + pnum + "_static";
                        //Animation a = SpriteDatabase.GetAnimation("cop_static");
                        PedestrianState pstate = PedestrianState.WANDER;
                        if (mode.StartsWith("Static"))
                        {
                            pstate = PedestrianState.STATIC;
                            animation = SpriteDatabase.GetAnimation("student" + pnum + "_static");
                        }
                        else
                        {
                            animation = SpriteDatabase.GetAnimation("student" + pnum + "_walk");
                        }
                        npc = new Pedestrian(
                            new PhysicsComponent2D(new Vector2(x, y), 0, new Vector2(20, 20), 115, 750, 75, 1000, 8, 40, 0f, true),
                            new MovementAIComponent2D(),
                            new DrawComponent(animation, Color.White, Vector2.Zero, new Vector2(.4f, .4f), .5f),
                            pstate
                            );
                    }
                    // The cops are not in, so they are ignored for now

                    if (type.StartsWith("DumbCop"))
                    {
                        DumbCopState dcState = DumbCopState.WANDER;
                        if (mode.StartsWith("Static"))
                        {
                            dcState = DumbCopState.STATIC;
                            animation = SpriteDatabase.GetAnimation("cop_static");
                            
                        }
                        else if (mode.StartsWith("Wander"))
                        {
                            dcState = DumbCopState.WANDER;
                            animation = SpriteDatabase.GetAnimation("cop_walk");
                        }
                        else
                        {
                            //TODO
                            dcState = DumbCopState.WANDER;
                            animation = SpriteDatabase.GetAnimation("cop_walk");
                        }
                        npc = new DumbCop(
                            new PhysicsComponent2D(new Vector2(x, y), 0, new Vector2(20, 20), 135, 750, 75, 1000, 8, 50, 0f, true),
                            new MovementAIComponent2D(),
                            new DrawComponent(animation, Color.White, Vector2.Zero, new Vector2(.4f, .4f), .5f), dcState);

                    }
                    if (type.StartsWith("SmartCop"))
                    {
                        SmartCopState dcState = SmartCopState.WANDER;
                        if (mode.StartsWith("Static"))
                        {
                            dcState = SmartCopState.STATIC;
                            animation = SpriteDatabase.GetAnimation("smartCop_static");

                        }
                        else if (mode.StartsWith("Wander"))
                        {
                            dcState = SmartCopState.WANDER;
                            animation = SpriteDatabase.GetAnimation("smartCop_walk");
                        }
                        else
                        {
                            //TODO
                            dcState = SmartCopState.WANDER;
                            animation = SpriteDatabase.GetAnimation("smartCop_walk");
                        }
                        npc = new SmartCop(
                            new PhysicsComponent2D(new Vector2(x, y), 0, new Vector2(20, 20), 145, 750, 75, 1000, 8, 50, 0f, true),
                            new MovementAIComponent2D(),
                            new DrawComponent(animation, Color.White, Vector2.Zero, new Vector2(.4f, .4f), .5f), dcState);
                    }
                    if (type.StartsWith("RoboCop"))
                    {
                        npc = new RoboCop(
                            new PhysicsComponent2D(new Vector2(x, y), 0, new Vector2(20, 20), 100, 750, 75, 1000, 8, 70, 0.25f, true),
                            new MovementAIComponent2D(),
                            new DrawComponent(SpriteDatabase.GetAnimation("roboCop_static"), Color.White, 
                                              Vector2.Zero, new Vector2(.4f, .4f), .5f));
                    }
                    if (npc != null)
                    {
                        if (pStart != null && pEnd != null)
                        {
                            npc.patrolStart = pStart;
                            npc.patrolEnd = pEnd;
                        }
                        startingNPCs.Add(npc);
                    }
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
                    && !line.StartsWith("NODES")
                    && !line.StartsWith("EDGES")
                    && !line.StartsWith("NPCS")
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
                    playerStart = new Vector2(x, y);
                    line = reader.ReadLine();
                } while (reader.Peek() != -1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        public List<Trigger> getTriggerList()       //Needed for achievements by triggers
        {
            return triggers;
        }
    }
}
