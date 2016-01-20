using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Assets.Code._XNA;
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
        float pathDelay = 5000;

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
                        if (!chasing)
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

        public void updateState()
        {
            IsVisible(SuperFlashGame.world.streaker.Position, out canSee, out canReach);

            //--------------------------------------------------------------------------
            //        DEFAULT BEHAVIOR TRANSITIONS --> Before aware of streaker
            //--------------------------------------------------------------------------
            if (behavior == SmartCopBehavior.DEFAULT)
            {
                if (Vector2.Distance(SuperFlashGame.world.streaker.Position, pos) < detectRadius && canSee)
                {
                    playSound("Exclamation");
                    behavior = SmartCopBehavior.AWARE;
                    transitionToState(SmartCopState.PURSUE);
                }
                else if (copsWhoSeeTheStreaker > 0)
                {
                    behavior = SmartCopBehavior.AWARE;

                    path = AStar.GetPath(Position, SuperFlashGame.world.streaker.Position, SuperFlashGame.world.map.nodes, SuperFlashGame.world.qTree, true, false);

                    // Optimize
                    OptimizePath(ref path);


                    transitionToState(SmartCopState.PATHFIND);
                }
            }
            //--------------------------------------------------------------------------
            //               AWARE BEHAVIOR TRANSITION --> Knows about streaker
            //--------------------------------------------------------------------------
            else if (behavior == SmartCopBehavior.AWARE)
            {
                float distance = Vector2.Distance(SuperFlashGame.world.streaker.Position, pos);

                switch (state)
                {
                    case SmartCopState.PATHFIND:

                        pathTimer += Time.deltaTime * 1000f;

                        // If sees, chases
                        if (canReach && distance <= detectRadius)
                        {
                            if (Math.Abs(SuperFlashGame.world.streaker.Position.x - pos.x) <= HIT_DISTANCE_X &&
                                Math.Abs(SuperFlashGame.world.streaker.Position.y - pos.y) <= HIT_DISTANCE_Y)
                            {
                                transitionToState(SmartCopState.HIT);
                            }
                            else
                            {
                                transitionToState(SmartCopState.PURSUE);
                            }
                        }
                        // If you or anyone else sees the streaker
                        else if ((canSee && distance <= detectRadius) || copsWhoSeeTheStreaker > 0)
                        {
                            StreakerSeen = true;
                            if (pathTimer > pathDelay)
                            {
                                pathTimer = 0;

                                path = AStar.GetPath(Position, SuperFlashGame.world.streaker.Position, SuperFlashGame.world.map.nodes, SuperFlashGame.world.qTree, true, false);

                                OptimizePath(ref path);
                            }
                            bool canReachNode = false;
                            bool canSeeNode = false;

                            // Optimize
                            //OptimizePath(ref path);

                            if (hasSeenTheStreaker)
                            {
                                hasSeenTheStreaker = false;
                                --copsWhoSeeTheStreaker;
                            }
                        }
                        // Else, continue along path
                        else
                        {
                            // If done current path and haven't seeked key point, do so
                            if (path.Count == 1 && (Position - path[0].Position).magnitude <= movement.ArrivalRadius && !isSeekingKeyNode)
                            {

                                isSeekingKeyNode = true;

                                Vector2 positionOfKeyNode = GetKeyNode();

                                path = AStar.GetPath(Position, positionOfKeyNode, SuperFlashGame.world.map.nodes, SuperFlashGame.world.qTree, true, false);

                                // Optimize
                                OptimizePath(ref path);
                            }
                            // If done current path and have sought key point, go back to normal
                            else if (path.Count == 1 && (Position - path[0].Position).magnitude <= movement.ArrivalRadius && isSeekingKeyNode)
                            {
                                if (StreakerSeen)
                                {
                                    DataManager.GetInstance().IncreaseScore(DataManager.Points.LoseAllCops, true,
                                        SuperFlashGame.world.streaker.Position.x, SuperFlashGame.world.streaker.Position.y - 64);
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
                            else if (path.Count > 0 && (Position - path[0].Position).magnitude <= movement.ArrivalRadius)
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
                                Math.Abs(SuperFlashGame.world.streaker.Position.x - pos.x) <= HIT_DISTANCE_X &&
                                Math.Abs(SuperFlashGame.world.streaker.Position.y - pos.y) <= HIT_DISTANCE_Y)
                            {
                                SuperFlashGame.world.streaker.GetHit();
                                playSound("Hit");
                            }

                            transitionToState(SmartCopState.PURSUE);
                        }

                        break;

                    case SmartCopState.PURSUE:

                        // If sees, chases or hits
                        if (canSee && distance <= detectRadius)
                        {
                            if (Math.Abs(SuperFlashGame.world.streaker.Position.x - pos.x) <= HIT_DISTANCE_X &&
                                Math.Abs(SuperFlashGame.world.streaker.Position.y - pos.y) <= HIT_DISTANCE_Y)
                            {
                                transitionToState(SmartCopState.HIT);
                            }
                        }
                        // If anyone else sees, path find
                        else if (copsWhoSeeTheStreaker > 0)
                        {
                            path = AStar.GetPath(Position, SuperFlashGame.world.streaker.Position, SuperFlashGame.world.map.nodes, SuperFlashGame.world.qTree, true, false);

                            // Optimize
                            OptimizePath(ref path);

                            transitionToState(SmartCopState.PATHFIND);
                        }
                        // Else, path find to key node
                        else
                        {
                            Vector2 positionOfKeyNode = GetKeyNode();

                            path = AStar.GetPath(Position, positionOfKeyNode, SuperFlashGame.world.map.nodes, SuperFlashGame.world.qTree, true, false);

                            // Optimize
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
                            SoundManager.GetInstance().PlaySound("Common", "Fall", SuperFlashGame.world.streaker.Position, Position);
                            transitionToState(SmartCopState.GET_UP);
                        }
                        break;
                    case SmartCopState.GET_UP:
                        if (draw.animComplete && Vector2.Distance(SuperFlashGame.world.streaker.Position, pos) < detectRadius)
                        {
                            behavior = SmartCopBehavior.AWARE;
                            transitionToState(SmartCopState.PURSUE);
                        }
                        else if (draw.animComplete && Vector2.Distance(SuperFlashGame.world.streaker.Position, pos) >= detectRadius)
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
                    movement.SetTarget(SuperFlashGame.world.streaker.Position, this);
                    movement.SetTargetVelocity(SuperFlashGame.world.streaker.ComponentPhysics.Velocity);
                    if (closest == this)
                        movement.Seek(ref physics);
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
                    movement.Seek(ref physics);
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
            SoundManager.GetInstance().PlaySound("SmartCop", soundName, SuperFlashGame.world.streaker.Position, Position);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Update
        /// </summary>
        public void Update(World w)
        {
            pos = physics.Position;

            bool wallCollision = false;
            if (state != SmartCopState.FALL || state != SmartCopState.GET_UP || state != SmartCopState.HIT)
            {
                wallCollision = testWallCollide();
            }

            if (!wallCollision)
            {
                updateState();
            }

            if (closest == this)
            {
                closestDistSq = (SuperFlashGame.world.streaker.ComponentPhysics.Position - physics.Position).sqrMagnitude;
            }
            else
            {
                float distSq = (SuperFlashGame.world.streaker.ComponentPhysics.Position - physics.Position).sqrMagnitude;
                if (distSq < closestDistSq)
                {
                    closest = this;
                    closestDistSq = distSq;
                }
            }

            movement.Look(ref physics);
            physics.UpdatePosition(Time.time, out pos, this);
            physics.UpdateOrientation(Time.time);

            if (state == SmartCopState.HIT)
            {
                if (pos.x < SuperFlashGame.world.streaker.Position.x)
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

            draw.Update();
            if (draw.animComplete && (state == SmartCopState.FALL || state == SmartCopState.GET_UP))
            {
                draw.GoToPrevFrame();
            }
            if (draw.animComplete && state == SmartCopState.HIT &&
                Math.Abs(SuperFlashGame.world.streaker.Position.x - pos.x) <= HIT_DISTANCE_X &&
                Math.Abs(SuperFlashGame.world.streaker.Position.y - pos.y) <= HIT_DISTANCE_Y)
            {
                SuperFlashGame.world.streaker.GetHit();
                playSound("Hit");
            }
            base.Update();

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            draw.Draw(spriteBatch, physics.Position);
            base.Draw(spriteBatch);
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

