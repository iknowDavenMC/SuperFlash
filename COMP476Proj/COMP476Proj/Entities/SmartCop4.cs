using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StreakerLibrary;

namespace COMP476Proj
{
    public enum SmartCopState { STATIC, WANDER, PATHFIND, PURSUE, FALL, GET_UP, HIT, PATROL };
    public enum SmartCopBehavior { DEFAULT, AWARE, KNOCKEDUP };

    public class SmartCop : NPC
    {
        #region Attributes
        public static SmartCop closest = null;
        public static float closestDistSq = float.MaxValue;
        public static bool StreakerSeen = false;
        bool hasSeenTheStreaker = false;
        bool isSeekingKeyNode = false;

        List<Node> path;

        float pathTimer = 0;
        float pathDelay = 2500;

        private bool canSee = false;
        private bool canReach = false;

        private SmartCopState state;
        private SmartCopBehavior behavior;
        private SmartCopState defaultState;
        private const int HIT_DISTANCE_X = 40;
        private const int HIT_DISTANCE_Y = 23;
        private bool chasing = false;
        public SmartCopState State { get { return state; } }
        #endregion

        #region Constructors

        public SmartCop(PhysicsComponent2D phys, MovementAIComponent2D move, DrawComponent draw, SmartCopState pState)
        {
            detectRadius = 400;
            movement = move;
            physics = phys;
            this.draw = draw;
            behavior = SmartCopBehavior.DEFAULT;
            state = defaultState = pState;
            this.BoundingRectangle = new COMP476Proj.BoundingRectangle(phys.Position, 16, 6);
            draw.Play();
        }

        public SmartCop(PhysicsComponent2D phys, MovementAIComponent2D move, DrawComponent draw, SmartCopState pState,
            float radius)
        {
            movement = move;
            physics = phys;
            this.draw = draw;
            state = defaultState = pState;
            detectRadius = radius;
            this.BoundingRectangle = new COMP476Proj.BoundingRectangle(phys.Position, 16, 6);
            draw.Play();
        }
        #endregion

        #region Private Methods
        private void transitionToState(SmartCopState pState)
        {
            if (state == pState)
            {
                return;
            }
            switch (pState)
            {
                case SmartCopState.STATIC:
                    if (hasSeenTheStreaker)
                    {
                        hasSeenTheStreaker = false;
                        --copsWhoSeeTheStreaker;
                    }
                    behavior = SmartCopBehavior.DEFAULT;
                    state = SmartCopState.STATIC;
                    draw.animation = SpriteDatabase.GetAnimation("smartCop_static");
                    physics.SetSpeed(false);
                    physics.SetAcceleration(false);
                    draw.Reset();
                    break;
                case SmartCopState.WANDER:
                    if (hasSeenTheStreaker)
                    {
                        hasSeenTheStreaker = false;
                        --copsWhoSeeTheStreaker;
                    }
                    behavior = SmartCopBehavior.DEFAULT;
                    state = SmartCopState.WANDER;
                    draw.animation = SpriteDatabase.GetAnimation("smartCop_walk");
                    physics.SetSpeed(false);
                    physics.SetAcceleration(false);
                    draw.Reset();
                    break;
                case SmartCopState.FALL:
                    behavior = SmartCopBehavior.KNOCKEDUP;
                    state = SmartCopState.FALL;
                    draw.animation = SpriteDatabase.GetAnimation("smartCop_fall");
                    physics.SetSpeed(false);
                    physics.SetAcceleration(false);
                    draw.Reset();
                    break;
                case SmartCopState.GET_UP:
                    behavior = SmartCopBehavior.KNOCKEDUP;
                    state = SmartCopState.GET_UP;
                    draw.animation = SpriteDatabase.GetAnimation("smartCop_getup");
                    physics.SetSpeed(false);
                    physics.SetAcceleration(false);
                    draw.Reset();
                    break;
                case SmartCopState.PATROL:
                    if (hasSeenTheStreaker)
                    {
                        hasSeenTheStreaker = false;
                        --copsWhoSeeTheStreaker;
                    }
                    state = SmartCopState.PATROL;
                    draw.animation = SpriteDatabase.GetAnimation("smartCop_walk");
                    physics.SetSpeed(false);
                    physics.SetAcceleration(false);
                    draw.Reset();
                    break;
                case SmartCopState.PATHFIND:
                    state = SmartCopState.PATHFIND;
                    draw.animation = SpriteDatabase.GetAnimation("smartCop_walk");
                    physics.SetSpeed(true);
                    physics.SetAcceleration(true);
                    draw.Reset();
                    break;
                case SmartCopState.HIT:
                    behavior = SmartCopBehavior.AWARE;
                    state = SmartCopState.HIT;
                    draw.animation = SpriteDatabase.GetAnimation("smartCop_attack");
                    physics.SetSpeed(false);
                    physics.SetAcceleration(false);
                    draw.Reset();
                    break;
                case SmartCopState.PURSUE:
                    if (!hasSeenTheStreaker)
                    {
                        hasSeenTheStreaker = true;
                        ++copsWhoSeeTheStreaker;
                        if(!chasing)
                            DataManager.GetInstance().numberOfCopsChasing++;
                        chasing = true;
                    }
                    behavior = SmartCopBehavior.AWARE;
                    state = SmartCopState.PURSUE;
                    draw.animation = SpriteDatabase.GetAnimation("smartCop_walk");
                    physics.SetSpeed(true);
                    physics.SetAcceleration(true);
                    draw.Reset();
                    break;
            }
        }

        public void updateState(GameTime gameTime)
        {
            IsVisible(Game1.world.streaker.Position, out canSee, out canReach);

            //--------------------------------------------------------------------------
            //        DEFAULT BEHAVIOR TRANSITIONS --> Before aware of streaker
            //--------------------------------------------------------------------------
            if (behavior == SmartCopBehavior.DEFAULT)
            {
                // If can reach, chase
                if (Vector2.Distance(Game1.world.streaker.Position, pos) < detectRadius && canReach)
                {
                    playSound("Exclamation");
                    behavior = SmartCopBehavior.AWARE;
                    transitionToState(SmartCopState.PURSUE);
                }
                // If can see, path find
                else if (Vector2.Distance(Game1.world.streaker.Position, pos) < detectRadius && canSee)
                {
                    playSound("Exclamation");

                    path = AStar.GetPath(Position, Game1.world.streaker.Position, Game1.world.map.nodes, Game1.world.qTree, true, false);

                    OptimizePath(ref path);

                    behavior = SmartCopBehavior.AWARE;
                    transitionToState(SmartCopState.PATHFIND);
                }
                // If anyone else sees, path find
                else if (copsWhoSeeTheStreaker > 0)
                {
                    behavior = SmartCopBehavior.AWARE;

                    path = AStar.GetPath(Position, Game1.world.streaker.Position, Game1.world.map.nodes, Game1.world.qTree, true, false);
                    
                    OptimizePath(ref path);

                    transitionToState(SmartCopState.PATHFIND);
                }
            }
            //--------------------------------------------------------------------------
            //               AWARE BEHAVIOR TRANSITION --> Knows about streaker
            //--------------------------------------------------------------------------
            else if (behavior == SmartCopBehavior.AWARE)
            {
                float distance = Vector2.Distance(Game1.world.streaker.Position, pos);

                switch (state)
                {
                    case SmartCopState.PATHFIND:

                        pathTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                        // If sees, chases
                        if (canReach && distance <= detectRadius)
                        {
                            transitionToState(SmartCopState.PURSUE);
                        }
                        // If you or anyone else sees the streaker
                        else if (!isSeekingKeyNode && ((canSee && distance <= detectRadius) || copsWhoSeeTheStreaker > 0))
                        {
                            StreakerSeen = true;
                            if (pathTimer > pathDelay)
                            {
                                pathTimer = 0;

                                path = AStar.GetPath(Position, Game1.world.streaker.Position, Game1.world.map.nodes, Game1.world.qTree, true, false);

                                OptimizePath(ref path);
                            }

                            if (!canSee && hasSeenTheStreaker)
                            {
                                hasSeenTheStreaker = false;
                                --copsWhoSeeTheStreaker;
                            }
                        }
                        // Else, continue along path
                        else
                        {
                            // If done current path and haven't seeked key point, do so
                            if (path.Count == 1 && (Position - path[0].Position).Length() <= movement.ArrivalRadius && !isSeekingKeyNode)
                            {

                                isSeekingKeyNode = true;

                                Vector2 positionOfKeyNode = GetKeyNode();

                                path = AStar.GetPath(Position, positionOfKeyNode, Game1.world.map.nodes, Game1.world.qTree, true, false);

                                OptimizePath(ref path);
                            }
                            // If done current path and have seeked key point, go back to normal
                            else if (path.Count == 1 && (Position - path[0].Position).Length() <= movement.ArrivalRadius && isSeekingKeyNode)
                            {
                                if (StreakerSeen)
                                {
                                    DataManager.GetInstance().IncreaseScore(DataManager.Points.LoseAllCops, true,
                                        Game1.world.streaker.Position.X, Game1.world.streaker.Position.Y - 64);
                                    StreakerSeen = false;
                                    chasing = false;
                                    DataManager.GetInstance().numberOfCopsChasing--;
                                }
                                isSeekingKeyNode = false;
                                path.Clear();
                                behavior = SmartCopBehavior.DEFAULT;
                                transitionToState(SmartCopState.STATIC);
                            }
                            // If at next node, update node to seek
                            else if (path.Count > 0 && (Position - path[0].Position).Length() <= movement.ArrivalRadius)
                            {
                                path.RemoveAt(0);
                            }
                            // else, Do no updating
                        }

                        break;

                    case SmartCopState.HIT:

                        if (draw.animComplete)
                        {
                            if (state == SmartCopState.HIT &&
                                Math.Abs(Game1.world.streaker.Position.X - pos.X) <= HIT_DISTANCE_X &&
                                Math.Abs(Game1.world.streaker.Position.Y - pos.Y) <= HIT_DISTANCE_Y)
                            {
                                Game1.world.streaker.GetHit();
                                playSound("Hit");
                            }

                            transitionToState(SmartCopState.PURSUE);
                        }

                        break;

                    case SmartCopState.PURSUE:

                        // If sees, chases or hits
                        if (canReach && distance <= detectRadius)
                        {
                            if (Math.Abs(Game1.world.streaker.Position.X - pos.X) <= HIT_DISTANCE_X &&
                                Math.Abs(Game1.world.streaker.Position.Y - pos.Y) <= HIT_DISTANCE_Y)
                            {
                                transitionToState(SmartCopState.HIT);
                            }
                        }
                        // If anyone else sees, path find
                        else if (copsWhoSeeTheStreaker > 0)
                        {
                            path = AStar.GetPath(Position, Game1.world.streaker.Position, Game1.world.map.nodes, Game1.world.qTree, true, false);

                            OptimizePath(ref path);

                            transitionToState(SmartCopState.PATHFIND);
                        }
                        // Else, path find to key node
                        else
                        {
                            Vector2 positionOfKeyNode = GetKeyNode();

                            path = AStar.GetPath(Position, positionOfKeyNode, Game1.world.map.nodes, Game1.world.qTree, true, false);

                            OptimizePath(ref path); 

                            transitionToState(SmartCopState.PATHFIND);
                        }

                        break;
                }
            }
            //--------------------------------------------------------------------------
            //               COLLIDE BEHAVIOR TRANSITION
            //--------------------------------------------------------------------------
            else if (behavior == SmartCopBehavior.KNOCKEDUP)
            {
                switch (state)
                {
                    case SmartCopState.FALL:
                        if (draw.animComplete)
                        {
                            SoundManager.GetInstance().PlaySound("Common", "Fall", Game1.world.streaker.Position, Position);
                            transitionToState(SmartCopState.GET_UP);
                        }
                        break;
                    case SmartCopState.GET_UP:
                        if (draw.animComplete && Vector2.Distance(Game1.world.streaker.Position, pos) < detectRadius)
                        {
                            behavior = SmartCopBehavior.AWARE;
                            transitionToState(SmartCopState.PURSUE);
                        }
                        else if (draw.animComplete && Vector2.Distance(Game1.world.streaker.Position, pos) >= detectRadius)
                        {
                            behavior = SmartCopBehavior.DEFAULT;
                            transitionToState(defaultState);
                        }
                        break;
                }
            }

            //--------------------------------------------------------------------------
            //       CHAR STATE --> ACTION
            //--------------------------------------------------------------------------

            switch (state)
            {
                case SmartCopState.STATIC:
                    movement.Stop(ref physics);
                    break;
                case SmartCopState.WANDER:
                    movement.Wander(ref physics);
                    break;
                case SmartCopState.PURSUE:
                    movement.SetTarget(Game1.world.streaker.Position, this);
                    movement.SetTargetVelocity(Game1.world.streaker.ComponentPhysics.Velocity);
                    if (closest == this)
                        movement.Arrive(ref physics);
                    else
                        movement.Pursue(ref physics);
                    break;
                case SmartCopState.PATROL:
                    if (path.Count > 0)
                        movement.SetTarget(path[0].Position);
                    movement.Arrive(ref physics);
                    break;
                case SmartCopState.PATHFIND:
                    if (path.Count > 0)
                        movement.SetTarget(path[0].Position);
                    movement.Arrive(ref physics);
                    break;
                case SmartCopState.FALL:
                    //movement.Stop(ref physics);
                    break;
                case SmartCopState.HIT:
                    movement.Stop(ref physics);
                    break;
                case SmartCopState.GET_UP:
                    movement.Stop(ref physics);
                    break;
                default:
                    break;
            }
        }

        private void playSound(string soundName)
        {
            SoundManager.GetInstance().PlaySound("SmartCop", soundName, Game1.world.streaker.Position, Position);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Update
        /// </summary>
        public void Update(GameTime gameTime, World w)
        {
            pos = physics.Position;

            bool wallCollision = false;
            if (state != SmartCopState.FALL || state != SmartCopState.GET_UP || state != SmartCopState.HIT )
            {
                wallCollision = testWallCollide();
            }

            if (!wallCollision)
            {
                updateState(gameTime);
            }

            if (closest == this)
            {
                closestDistSq = (Game1.world.streaker.ComponentPhysics.Position - physics.Position).LengthSquared();
            }
            else
            {
                float distSq = (Game1.world.streaker.ComponentPhysics.Position - physics.Position).LengthSquared();
                if (distSq < closestDistSq)
                {
                    closest = this;
                    closestDistSq = distSq;
                }
            }

            movement.Look(ref physics);
            physics.UpdatePosition(gameTime.ElapsedGameTime.TotalSeconds, out pos, this);
            physics.UpdateOrientation(gameTime.ElapsedGameTime.TotalSeconds);

            if (state == SmartCopState.HIT)
            {
                if (pos.X < Game1.world.streaker.Position.X)
                {
                    draw.SpriteEffect = SpriteEffects.None;
                }
                else
                {
                    draw.SpriteEffect = SpriteEffects.FlipHorizontally;
                }
            }
            else if (physics.Orientation > 0)
            {
                draw.SpriteEffect = SpriteEffects.None;
            }
            else if (physics.Orientation < 0)
            {
                draw.SpriteEffect = SpriteEffects.FlipHorizontally;
            }

            draw.Update(gameTime);
            if (draw.animComplete && (state == SmartCopState.FALL || state == SmartCopState.GET_UP))
            {
                draw.GoToPrevFrame();
            }
            if (draw.animComplete && state == SmartCopState.HIT &&
                Math.Abs(Game1.world.streaker.Position.X - pos.X) <= HIT_DISTANCE_X &&
                Math.Abs(Game1.world.streaker.Position.Y - pos.Y) <= HIT_DISTANCE_Y)
            {
                Game1.world.streaker.GetHit();
                playSound("Hit");
            }
            base.Update(gameTime);

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            draw.Draw(gameTime, spriteBatch, physics.Position);
            base.Draw(gameTime, spriteBatch);
        }

        public override void Fall(bool isSuperFlash)
        {
            if (state != SmartCopState.FALL)
            {
                if (isSuperFlash)
                {
                    playSound("SuperFlash");
                }

                behavior = SmartCopBehavior.KNOCKEDUP;

                transitionToState(SmartCopState.FALL);

                movement.Stop(ref physics);
            }
        }

        #endregion

    }
}

