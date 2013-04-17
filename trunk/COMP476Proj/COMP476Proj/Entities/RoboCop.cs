using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StreakerLibrary;
namespace COMP476Proj
{
    
    public enum RoboCopState { STATIC, PURSUE, HIT, PATHFIND };

    public class RoboCop : NPC
    {
        #region Attributes

        private RoboCopState state;

        List<Node> path;

        float pathTimer = 0;
        float pathDelay = 5000;

        private bool canSee = false;
        private bool canReach = false;

        private bool withinHitRadius = false;

        private const int HIT_DISTANCE_X = 40;
        private const int HIT_DISTANCE_Y = 15;
        #endregion

        #region Constructors
        public RoboCop(PhysicsComponent2D phys, MovementAIComponent2D move, DrawComponent draw)
        {
            detectRadius = 400;
            movement = move;
            physics = phys;
            this.draw = draw;
            state = RoboCopState.STATIC;
            this.BoundingRectangle = new COMP476Proj.BoundingRectangle(phys.Position, 16, 6);
            draw.Play();
        }

        public RoboCop(PhysicsComponent2D phys, MovementAIComponent2D move, DrawComponent draw, RoboCopState pState,
            float radius)
        {
            movement = move;
            physics = phys;
            this.draw = draw;
            state = pState;
            detectRadius = radius;
            this.BoundingRectangle = new COMP476Proj.BoundingRectangle(phys.Position, 16, 6);
            draw.Play();
        }
        #endregion

        #region Private Methods
        private void transitionToState(RoboCopState pState)
        {
            if (state == pState)
            {
                return;
            }
            switch (pState)
            {
                case RoboCopState.STATIC:
                    state = RoboCopState.STATIC;
                    draw.animation = SpriteDatabase.GetAnimation("roboCop_static");
                    physics.SetSpeed(false);
                    physics.SetAcceleration(false);
                    draw.Reset();
                    break;
                case RoboCopState.HIT:
                    state = RoboCopState.HIT;
                    draw.animation = SpriteDatabase.GetAnimation("roboCop_attack");
                    physics.SetSpeed(false);
                    physics.SetAcceleration(false);
                    draw.Reset();
                    break;
                case RoboCopState.PURSUE:
                    if (state == RoboCopState.STATIC)
                    {
                        playSound("Activation");
                    }
                    if (state != RoboCopState.PATHFIND)
                    {
                        DataManager.GetInstance().numberOfRoboCopsChasing++;
                        draw.Reset();
                    }
                    state = RoboCopState.PURSUE;
                    draw.animation = SpriteDatabase.GetAnimation("roboCop_walk");
                    physics.SetSpeed(true);
                    physics.SetAcceleration(true);
                    
                    break;
                case RoboCopState.PATHFIND:
                    if (state != RoboCopState.PURSUE)
                    {
                        draw.Reset();
                    }
                    state = RoboCopState.PATHFIND;
                    draw.animation = SpriteDatabase.GetAnimation("roboCop_walk");
                    physics.SetSpeed(true);
                    physics.SetAcceleration(true);
                    
                    
                    
                    break;
            }
        }

        public void updateState(GameTime gameTime)
        {
            IsVisible(Game1.world.streaker.Position, out canSee, out canReach);

            withinHitRadius = Math.Abs(Game1.world.streaker.Position.X - pos.X) <= HIT_DISTANCE_X &&
                              Math.Abs(Game1.world.streaker.Position.Y - pos.Y) <= HIT_DISTANCE_Y;

            if (state == RoboCopState.STATIC)
            {
                if (Vector2.Distance(Game1.world.streaker.Position, pos) < detectRadius && canSee)
                {
                    transitionToState(RoboCopState.PURSUE);
                }
            }
            //--------------------------------------------------------------------------
            //               AWARE BEHAVIOR TRANSITION --> Knows about streaker
            //--------------------------------------------------------------------------
            else if (state == RoboCopState.PURSUE)
            {
                float distance = Vector2.Distance(Game1.world.streaker.Position, pos);
                if (withinHitRadius)
                {
                    transitionToState(RoboCopState.HIT);
                }
                else if (distance < detectRadius && canReach)
                {
                    // Don't change anything
                }
                else
                {
                    path = AStar.GetPath(Position, Game1.world.streaker.Position, Game1.world.map.nodes, Game1.world.qTree, true, false);

                    // Optimize
                    while (path.Count > 1 && canReach)
                    {
                        path.RemoveAt(0);
                    }

                    transitionToState(RoboCopState.PATHFIND);
                }
            }
            else if (state == RoboCopState.PATHFIND)
            {
                pathTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                // If sees, chase
                if (Vector2.Distance(Game1.world.streaker.Position, pos) < detectRadius && canSee)
                {
                    transitionToState(RoboCopState.PURSUE);
                }
                // If timer is up, update path
                else if (pathTimer > pathDelay)
                {
                    pathTimer = 0;

                    path = AStar.GetPath(Position, Game1.world.streaker.Position, Game1.world.map.nodes, Game1.world.qTree, true, false);

                    // Optimize
                    while (path.Count > 1 && canReach)
                    {
                        path.RemoveAt(0);
                    }
                }
                // Else, continue along path
                else
                {
                    // If done path, create a new path
                    if (path.Count == 1 && (Position - path[0].Position).Length() <= movement.ArrivalRadius)
                    {
                        pathTimer = 0;

                        path = AStar.GetPath(Position, Game1.world.streaker.Position, Game1.world.map.nodes, Game1.world.qTree, true, false);

                        // Optimize
                        while (path.Count > 1 && canReach)
                        {
                            path.RemoveAt(0);
                        }
                    }
                    // If at next node, update node to seek
                    else if (path.Count > 0 && (Position - path[0].Position).Length() <= movement.ArrivalRadius)
                    {
                        path.RemoveAt(0);
                    }
                    // else, Do no updating
                }
            }
            //--------------------------------------------------------------------------
            //        STATIC BEHAVIOR TRANSITIONS --> Before aware of streaker
            //-------------------------------------------------------------------------- 
            else if (state == RoboCopState.HIT)
            {
                if (!withinHitRadius)
                {
                    transitionToState(RoboCopState.PURSUE);
                }
            }

            //--------------------------------------------------------------------------
            //       CHAR STATE --> ACTION
            //--------------------------------------------------------------------------

            switch (state)
            {
                case RoboCopState.STATIC:
                    movement.Stop(ref physics);
                    break;
                case RoboCopState.PURSUE:
                    movement.SetTarget(Game1.world.streaker.Position);
                    movement.Pursue(ref physics);
                    break;
                case RoboCopState.HIT:
                    movement.Stop(ref physics);
                    break;
                case RoboCopState.PATHFIND:
                    if (path.Count > 0)
                        movement.SetTarget(path[0].Position);
                    movement.Arrive(ref physics);
                    break;
                default:
                    break;
            }
        }

        private void playSound(string soundName)
        {
            SoundManager.GetInstance().PlaySound("RoboCop", soundName, Game1.world.streaker.Position, Position, false);
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
            if (state ==RoboCopState.HIT)
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
            else if (state != RoboCopState.HIT)
            {
                wallCollision = testWallCollide();
            }

            if (!wallCollision)
            {
                updateState(gameTime);
            }

            movement.Look(ref physics);
            physics.UpdatePosition(gameTime.ElapsedGameTime.TotalSeconds, out pos);
            physics.UpdateOrientation(gameTime.ElapsedGameTime.TotalSeconds);
            if (physics.Orientation > 0)
            {
                draw.SpriteEffect = SpriteEffects.None;
            }
            else if (physics.Orientation < 0)
            {
                draw.SpriteEffect = SpriteEffects.FlipHorizontally;
            }

            draw.Update(gameTime);
            if (draw.animComplete && state == RoboCopState.HIT &&
                Math.Abs(Game1.world.streaker.Position.X - pos.X) <= HIT_DISTANCE_X &&
                Math.Abs(Game1.world.streaker.Position.Y - pos.Y) <= HIT_DISTANCE_Y)
            {
                Game1.world.streaker.GetHit();
                Game1.world.streaker.ResolveCollision(this);
            }
            base.Update(gameTime);

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            draw.Draw(gameTime, spriteBatch, physics.Position);
            base.Draw(gameTime, spriteBatch);
        }

        #endregion

    }
}
