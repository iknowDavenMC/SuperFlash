using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StreakerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace COMP476Proj
{
    public enum DumbCopState { STATIC, WANDER, PATROL, SEEK, FALL, GET_UP, HIT, PATHFIND };
    public enum DumbCopBehavior { DEFAULT, AWARE, KNOCKEDUP };

    public class DumbCop : NPC
    {
        #region Attributes

        bool hasSeenTheStreaker = false;

        Node startNode;
        Node endNode;
        Node targetNode;

        Vector2? lastStreakerPosition;
        List<Node> path;

        private DumbCopState state;
        private DumbCopBehavior behavior;
        private DumbCopState defaultState;
        private const int HIT_DISTANCE_X = 40;
        private const int HIT_DISTANCE_Y = 15;
        #endregion

        #region Constructors
        public DumbCop(PhysicsComponent2D phys, MovementAIComponent2D move, DrawComponent draw, DumbCopState pState)
        {
            detectRadius = 400;
            movement = move;
            physics = phys;
            this.draw = draw;
            behavior = DumbCopBehavior.DEFAULT;
            state = defaultState = pState;
            this.BoundingRectangle = new COMP476Proj.BoundingRectangle(phys.Position, 16, 6);
            draw.Play();
        }

        public DumbCop(PhysicsComponent2D phys, MovementAIComponent2D move, DrawComponent draw, DumbCopState pState,
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
        private void transitionToState(DumbCopState pState)
        {
            if (state == pState)
            {
                return;
            }
            switch (pState)
            {
                case DumbCopState.STATIC:
                    if (hasSeenTheStreaker)
                    {
                        hasSeenTheStreaker = false;
                        --copsWhoSeeTheStreaker;
                    }
                    behavior = DumbCopBehavior.DEFAULT;
                    state = DumbCopState.STATIC;
                    draw.animation = SpriteDatabase.GetAnimation("cop_static");
                    physics.SetSpeed(false);
                    physics.SetAcceleration(false);
                    draw.Reset();
                    break;
                case DumbCopState.WANDER:
                    if (hasSeenTheStreaker)
                    {
                        hasSeenTheStreaker = false;
                        --copsWhoSeeTheStreaker;
                    }
                    behavior = DumbCopBehavior.DEFAULT;
                    state = DumbCopState.WANDER;
                    draw.animation = SpriteDatabase.GetAnimation("cop_walk");
                    physics.SetSpeed(false);
                    physics.SetAcceleration(false);
                    draw.Reset();
                    break;
                case DumbCopState.FALL:
                    behavior = DumbCopBehavior.KNOCKEDUP;
                    state = DumbCopState.FALL;
                    draw.animation = SpriteDatabase.GetAnimation("cop_fall");
                    physics.SetSpeed(false);
                    physics.SetAcceleration(false);
                    draw.Reset();
                    break;
                case DumbCopState.GET_UP:
                    behavior = DumbCopBehavior.KNOCKEDUP;
                    state = DumbCopState.GET_UP;
                    draw.animation = SpriteDatabase.GetAnimation("cop_getup");
                    physics.SetSpeed(false);
                    physics.SetAcceleration(false);
                    draw.Reset();
                    break;
                case DumbCopState.PATROL:
                    if (hasSeenTheStreaker)
                    {
                        hasSeenTheStreaker = false;
                        --copsWhoSeeTheStreaker;
                    }
                    state = DumbCopState.PATROL;
                    draw.animation = SpriteDatabase.GetAnimation("cop_walk");
                    physics.SetSpeed(false);
                    physics.SetAcceleration(false);
                    draw.Reset();
                    break;
                case DumbCopState.PATHFIND:
                    if (hasSeenTheStreaker)
                    {
                        hasSeenTheStreaker = false;
                        --copsWhoSeeTheStreaker;
                    }
                    state = DumbCopState.PATHFIND;
                    draw.animation = SpriteDatabase.GetAnimation("cop_walk");
                    physics.SetSpeed(true);
                    physics.SetAcceleration(true);
                    draw.Reset();
                    break;
                case DumbCopState.HIT:
                    behavior = DumbCopBehavior.AWARE;
                    state = DumbCopState.HIT;
                    draw.animation = SpriteDatabase.GetAnimation("cop_attack");
                    physics.SetSpeed(false);
                    physics.SetAcceleration(false);
                    draw.Reset();
                    break;
                case DumbCopState.SEEK:
                    if (!hasSeenTheStreaker)
                    {
                        hasSeenTheStreaker = true;
                        ++copsWhoSeeTheStreaker;
                    }
                    behavior = DumbCopBehavior.AWARE;
                    state = DumbCopState.SEEK;
                    draw.animation = SpriteDatabase.GetAnimation("cop_walk");
                    physics.SetSpeed(true);
                    physics.SetAcceleration(true);
                    draw.Reset();
                    break;
            }
        }

        public void updateState()
        {

            //--------------------------------------------------------------------------
            //        DEFAULT BEHAVIOR TRANSITIONS --> Before aware of streaker
            //--------------------------------------------------------------------------
            if (behavior == DumbCopBehavior.DEFAULT)
            {
                if (Vector2.Distance(Game1.world.streaker.Position, pos) < detectRadius && LineOfSight())
                {
                    playSound("Exclamation");
                    behavior = DumbCopBehavior.AWARE;
                    transitionToState(DumbCopState.SEEK);
                }
            }
            //--------------------------------------------------------------------------
            //               AWARE BEHAVIOR TRANSITION --> Knows about streaker
            //--------------------------------------------------------------------------
            else if (behavior == DumbCopBehavior.AWARE)
            {
                bool canSee = LineOfSight();
                float distance = Vector2.Distance(Game1.world.streaker.Position, pos);

                switch (state)
                {
                    case DumbCopState.PATHFIND:

                        // If sees, chases
                        if (canSee && distance <= detectRadius)
                        {
                            lastStreakerPosition = Game1.world.streaker.Position;

                            if (Math.Abs(Game1.world.streaker.Position.X - pos.X) <= HIT_DISTANCE_X &&
                                Math.Abs(Game1.world.streaker.Position.Y - pos.Y) <= HIT_DISTANCE_Y)
                            {
                                transitionToState(DumbCopState.HIT);
                            }
                            else
                            {
                                transitionToState(DumbCopState.SEEK);
                            }
                        }
                        // Path
                        else
                        {
                            // If done path, go back to default
                            if (path.Count == 1 && (Position - path[0].Position).Length() <= movement.ArrivalRadius)
                            {
                                DataManager.GetInstance().IncreaseScore(DataManager.Points.LoseCop,
                                    true, Game1.world.streaker.Position.X, Game1.world.streaker.Position.Y - 64);
                                path.Clear();
                                behavior = DumbCopBehavior.DEFAULT;
                                transitionToState(defaultState);
                            }
                            // If at next node, update node to seek
                            else if (path.Count > 0 && (Position - path[0].Position).Length() <= movement.ArrivalRadius)
                            {
                                path.RemoveAt(0);
                            }
                            // else, Do no updating
                        }

                        break;

                    case DumbCopState.HIT:
                    case DumbCopState.SEEK:

                        // If sees, chases or hits
                        if (canSee && distance <= detectRadius)
                        {
                            lastStreakerPosition = Game1.world.streaker.Position;

                            if (Math.Abs(Game1.world.streaker.Position.X - pos.X) <= HIT_DISTANCE_X &&
                                Math.Abs(Game1.world.streaker.Position.Y - pos.Y) <= HIT_DISTANCE_Y)
                            {
                                transitionToState(DumbCopState.HIT);
                            }
                            else
                            {
                                transitionToState(DumbCopState.SEEK);
                            }
                        }
                        // Can't see and hasn't path found already, path find to last known position
                        else if (!canSee && lastStreakerPosition != null)
                        {
                            path = AStar.GetPath(Position, (Vector2)lastStreakerPosition, Game1.world.map.nodes, Game1.world.qTree, true, false);

                            // Optimize
                            while (path.Count > 1 && IsVisible(path[1].Position))
                            {
                                path.RemoveAt(0);
                            }

                            transitionToState(DumbCopState.PATHFIND);
                            lastStreakerPosition = null;
                        }
                        // Else, go back to default
                        else
                        {
                            DataManager.GetInstance().IncreaseScore(DataManager.Points.LoseCop,
                                true, Game1.world.streaker.Position.X, Game1.world.streaker.Position.Y - 64);
                            behavior = DumbCopBehavior.DEFAULT;
                            transitionToState(defaultState);
                        }

                        break;
                }
            }
            //--------------------------------------------------------------------------
            //               COLLIDE BEHAVIOR TRANSITION
            //--------------------------------------------------------------------------
            else if (behavior == DumbCopBehavior.KNOCKEDUP)
            {
                switch (state)
                {
                    case DumbCopState.FALL:
                        if (draw.animComplete)
                        {
                            SoundManager.GetInstance().PlaySound("Common", "Fall", Game1.world.streaker.Position, Position);
                            transitionToState(DumbCopState.GET_UP);
                        }
                        break;
                    case DumbCopState.GET_UP:
                        if (draw.animComplete && Vector2.Distance(Game1.world.streaker.Position, pos) < detectRadius)
                        {
                            behavior = DumbCopBehavior.AWARE;
                            transitionToState(DumbCopState.SEEK);
                        }
                        else if (draw.animComplete && Vector2.Distance(Game1.world.streaker.Position, pos) >= detectRadius)
                        {
                            behavior = DumbCopBehavior.DEFAULT;
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
                case DumbCopState.STATIC:
                    movement.Stop(ref physics);
                    break;
                case DumbCopState.WANDER:
                    movement.Wander(ref physics);
                    break;
                case DumbCopState.SEEK:
                    movement.SetTarget(Game1.world.streaker.Position);
                    movement.Seek(ref physics);
                    break;
                case DumbCopState.PATROL:
                    if (path.Count > 0)
                        movement.SetTarget(path[0].Position);
                    movement.Arrive(ref physics);
                    break;
                case DumbCopState.PATHFIND:
                    if (path.Count > 0)
                        movement.SetTarget(path[0].Position);
                    movement.Arrive(ref physics);
                    break;
                case DumbCopState.FALL:
                    //movement.Stop(ref physics);
                    break;
                case DumbCopState.HIT:
                    movement.Stop(ref physics);
                    break;
                case DumbCopState.GET_UP:
                    movement.Stop(ref physics);
                    break;
                default:
                    break;
            }
        }

        private void playSound(string soundName)
        {
            SoundManager.GetInstance().PlaySound("DumbCop", soundName, Game1.world.streaker.Position, Position);
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
            if(state != DumbCopState.FALL || state != DumbCopState.GET_UP || state != DumbCopState.HIT){
                wallCollision = testWallCollide();
            }
            
            if(!wallCollision){
                updateState();
            }

            movement.Look(ref physics);
            physics.UpdatePosition(gameTime.ElapsedGameTime.TotalSeconds, out pos);
            physics.UpdateOrientation(gameTime.ElapsedGameTime.TotalSeconds);

            if (state == DumbCopState.HIT)
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
            if (draw.animComplete && (state == DumbCopState.FALL || state == DumbCopState.GET_UP))
            {
                draw.GoToPrevFrame();
            }
            if (draw.animComplete && state == DumbCopState.HIT &&
                Math.Abs(Game1.world.streaker.Position.X - pos.X) <= HIT_DISTANCE_X &&
                Math.Abs(Game1.world.streaker.Position.Y - pos.Y) <= HIT_DISTANCE_Y)
            {
                Game1.world.streaker.GetHit();
                Game1.world.streaker.ResolveCollision(this);
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
            if (state != DumbCopState.FALL)
            {
                if (isSuperFlash)
                {
                    playSound("SuperFlash");
                }

                behavior = DumbCopBehavior.KNOCKEDUP;

                transitionToState(DumbCopState.FALL);

                movement.Stop(ref physics);
            }
        }

        #endregion
    }
}
