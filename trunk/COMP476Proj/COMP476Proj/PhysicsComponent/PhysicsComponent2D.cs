// David Campbell
// 1965387

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Microsoft.Xna.Framework;

namespace COMP476Proj
{
    // SI units
    public class PhysicsComponent2D
    {
        #region Attributes

        #region Movement Variables

        /// <summary>
        /// Position
        /// </summary>
        protected Vector2 position;

        /// <summary>
        /// Velocity
        /// </summary>
        protected Vector2 velocity;

        /// <summary>
        /// Acceleration
        /// </summary>
        protected Vector2 acceleration;

        /// <summary>
        /// Direction vector
        /// </summary>
        protected Vector2 movementDirection;

        #endregion

        #region Orientation Variables

        /// <summary>
        /// Orientation in radians.
        /// 0 points to the bottom of the screen
        /// Pi points to the top of the screen
        /// Pi/2 points to the right of the screen
        /// -Pi/2 points to the left of the screen
        /// </summary>
        protected float orientation;

        /// <summary>
        /// Rotation
        /// </summary>
        protected float rotation;

        /// <summary>
        /// Angular acceleration
        /// </summary>
        protected float angularAcceleration;

        /// <summary>
        /// Orientation direction
        /// </summary>
        protected float orientationDirection;

        #endregion

        #region Collision Variables

        /// <summary>
        /// Mass of the object
        /// </summary>
        protected float mass;

        /// <summary>
        /// Coefficient of restitution
        /// </summary>
        protected float coefficientOfRestitution;

        #endregion

        #region Movement-related Bounds

        /// <summary>
        /// Max Velocity
        /// </summary>
        protected float maxVelocity;

        /// <summary>
        /// Max Acceleration
        /// </summary>
        protected float maxAcceleration;

        /// <summary>
        /// Max Velocity for a walk
        /// </summary>
        protected float maxVelocityWalk = 150; // Good for streaker

        /// <summary>
        /// Max Acceleration for a walk
        /// </summary>
        protected float maxAccelerationWalk = 750f; // Good for streaker

        /// <summary>
        /// Max Velocity for a run
        /// </summary>
        protected float maxVelocityRun = 150; // Good for streaker

        /// <summary>
        /// Max Acceleration for a run
        /// </summary>
        protected float maxAccelerationRun = 750f; // Good for streaker

        /// <summary>
        /// Controls how quickly people come to a stop
        /// </summary>
        protected float friction = 8;

        #endregion

        #region Orientation-related Bounds

        /// <summary>
        /// Max rotation
        /// </summary>
        protected float maxRotation = MathHelper.ToRadians(359); // It takes about 1.5 seconds to do a full turn

        /// <summary>
        /// Max angular acceleration
        /// </summary>
        protected float maxAngularAcceleration = MathHelper.ToRadians(359); // Assuming it takes 0.5 seconds to reach max rotation speed

        #endregion

        #region Toggles

        /// <summary>
        /// Whether or not steering movement mechanics are being used
        /// </summary>
        protected bool isSteering;

        /// <summary>
        /// Whether or not the entity is coming to a stop
        /// </summary>
        protected bool isStopping;

        #endregion

        #endregion

        #region Properties

        public float Mass
        {
            get { return mass; }
        }

        public float MaxAcceleration
        {
            get { return maxAcceleration; }
        }

        public float MaxVelocity
        {
            get { return maxVelocity; }
        }

        public float MaxAngularAcceleration
        {
            get { return maxAngularAcceleration; }
        }

        public Vector2 Direction
        {
            get { return movementDirection; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
        }

        public Vector2 Acceleration
        {
            get { return acceleration; }
        }

        public float Orientation
        {
            get { return orientation; }
        }

        public bool IsSteering
        {
            get { return isSteering; }
        }

        public float MaxRotation
        {
            get { return maxRotation; }
        }

        public float AngularAcceleration
        {
            get { return angularAcceleration; }
        }

        public float Rotation
        {
            get { return rotation; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <param name="position">Position of the object</param>
        /// <param name="orientation">Orientation of the object</param>
        /// <param name="dimensions">Dimensions of the object's bounding rectangle</param>
        /// <param name="maxVelocityRun">Max player velocity when running</param>
        /// <param name="maxAccelerationRun">Max player acceleration when running</param>
        /// <param name="maxVelocityWalk">Max player velocity when walking</param>
        /// <param name="maxAccelerationWalk">Max player acceleration when walking</param>
        /// <param name="friction">Sliding factor on stop</param>
        /// <param name="mass">Character mass</param>
        /// <param name="coefficientOfRestitution">How much is momentum preserved</param>
        /// <param name="isSteering">Is steering being employed, or is kinematic movement being used</param>
        public PhysicsComponent2D(Vector2 position, float orientation, Vector2 dimensions, float maxVelocityRun,
            float maxAccelerationRun, float maxVelocityWalk, float maxAccelerationWalk, float friction,
            float mass, float coefficientOfRestitution, bool isSteering = false)
        {
            // Specified
            this.position = position;
            this.orientation = orientationDirection = orientation;
            this.isSteering = isSteering;
            this.maxAccelerationRun = maxAccelerationRun;
            this.maxVelocityRun = maxVelocityRun;
            this.maxAccelerationWalk = maxAccelerationWalk;
            this.maxVelocityWalk = maxVelocityWalk;
            this.friction = friction;
            this.mass = mass;
            this.coefficientOfRestitution = coefficientOfRestitution;

            maxAcceleration = maxAccelerationWalk;
            maxVelocity = maxVelocityWalk;

            // Unspecified
            movementDirection = velocity = acceleration = Vector2.Zero;
            rotation = angularAcceleration = 0;
            isStopping = false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Position update if steering
        /// </summary>
        /// <param name="time">Time since last update call</param>
        private void updatePositionSteering(double time)
        {
            // If stopping
            if (isStopping)
            {
                // Acceleration opposes last movement
                acceleration = velocity * -friction;

                // Capped by max acceleration
                if (acceleration.Length() > maxAcceleration)
                {
                    acceleration.Normalize();
                    acceleration *= maxAcceleration;
                }

                // Update velocity
                int signX = Math.Sign(velocity.X), signY = Math.Sign(velocity.Y);
                velocity += acceleration * (float)time;

                if (Math.Sign(velocity.X) != signX || Math.Abs(velocity.X) < 25)
                {
                    velocity.X = 0;
                }

                if (Math.Sign(velocity.Y) != signY || Math.Abs(velocity.Y) < 25)
                {
                    velocity.Y = 0;
                }

                // Capped by max velocity
                if (velocity.Length() > maxVelocity)
                {
                    velocity.Normalize();
                    velocity *= maxVelocity;
                }
            }
            else
            {
                // Accelerate fully in the direction of the target
                acceleration = movementDirection;
                acceleration *= maxAcceleration;

                velocity += acceleration * (float)time;

                // Capped by max velocity
                if (velocity.Length() > maxVelocity)
                {
                    velocity.Normalize();
                    velocity *= maxVelocity;
                }
            }
        }
        
        /// <summary>
        /// Position update if kinematic
        /// </summary>
        /// <param name="time">Time since last update call</param>
        private void updatePositionKinematic(double time)
        {
            // If stopping
            if (isStopping)
            {
                // Stop
                velocity = Vector2.Zero;
            }
            else
            {
                // Move at max speed in direction of the target
                velocity = movementDirection;
                velocity *= maxVelocity;
            }
        }

        /// <summary>
        /// Orientation update if steering
        /// </summary>
        /// <param name="time">Time since last update call</param>
        private void updateOrientationSteering(double time)
        {
            // Update rotation
            rotation += angularAcceleration * (float)time;

            // Capped by max rotation
            if (Math.Abs(rotation) > maxRotation)
            {
                rotation = maxRotation * Math.Sign(rotation);
            }

            // Calculates orientation
            orientation += rotation * (float)time;
            orientation = MathHelper.WrapAngle(orientation);
        }

        /// <summary>
        /// Orientation update if kinematic
        /// nterpolates the rotation over a few frames
        /// </summary>
        /// <param name="time">Time since last update call</param>
        private void updateOrientationKinematic(double time)
        {
            // Calculate change in angle needed
            float change = MathHelper.WrapAngle(orientationDirection - orientation);

            // If cannot reach angle, do max
            if (Math.Abs(change) > maxRotation * time)
            {
                rotation = maxRotation * Math.Sign(change);

                // Calculates orientation
                orientation += rotation * (float)time;
            }
            // Set the desired angle
            else
            {
                orientation = orientationDirection;
            }

            orientation = MathHelper.WrapAngle(orientation);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the target values (used with AI characters to move based on thinking)
        /// If null, the parameter is not important
        /// </summary>
        /// <param name="isToStop">Whether or not the object should stop</param>
        /// <param name="targetDirection">Direction to travel</param>
        /// <param name="targetVelocity">Velocity at which to travel</param>
        /// <param name="targetAcceleration">Acceleration with which to travel</param>
        public void SetTargetValues(bool? isToStop, Vector2? targetDirection, Vector2? targetVelocity, Vector2? targetAcceleration)
        {
            if (isToStop != null)
            {
                isStopping = (bool)isToStop;
            }

            if (!targetDirection.Equals(null))
            {
                movementDirection = (Vector2)targetDirection;
                movementDirection.Normalize();
            }

            if (!targetVelocity.Equals(null))
            {
                velocity = (Vector2)targetVelocity;
            }

            if (!targetAcceleration.Equals(null))
            {
                acceleration = (Vector2)targetAcceleration;
            }
        }

        /// <summary>
        /// Sets the target value (used with AI characters to rotate based on thinking)
        /// </summary>
        /// <param name="targetOrientation">Desired orientation</param>
        /// <param name="targetAcceleration">Angular acceleration with which to rotate</param>
        public void SetTargetValues(float? targetOrientation, float? targetAcceleration)
        {
            if (targetOrientation != null)
            {
                orientationDirection = (float)targetOrientation;
            }

            if (targetAcceleration != null)
            {
                angularAcceleration = (float)targetAcceleration;
            }
        }

        /// <summary>
        /// Calculates current position based on velocity and acceleration
        /// </summary>
        /// <param name="time">Time elapsed (in seconds) since the last update of position</param>
        public void UpdatePosition(double time)
        {
            // Steering
            if (isSteering)
            {
                updatePositionSteering(time);
            }
            else
            {
                updatePositionKinematic(time);
            }

            // Calculates position
            position += velocity * (float)time;
        }

        /// <summary>
        /// Calculates current position based on velocity and acceleration
        /// </summary>
        /// <param name="time">Time elapsed (in seconds) since the last update of position</param>
        /// <param name="position">New position</param>
        public void UpdatePosition(double time, out Vector2 position)
        {
            UpdatePosition(time);

            position = this.position;
        }

        /// <summary>
        /// Calculates current orientation based on rotation and angular acceleration if steering
        /// Else, orientation is interpolated over a few frames
        /// </summary>
        /// <param name="time">Time elapsed (in seconds) since the last update of position</param>
        public void UpdateOrientation(double time)
        {
            // Steering
            if (isSteering)
            {
                updateOrientationSteering(time);
            }
            else
            {
                updateOrientationKinematic(time);
            }
        }

        /// <summary>
        /// Sets orientation to instantly look where the player is moving
        /// </summary>
        /// <param name="time">Time elapsed (in seconds) since the last update of position</param>
        public void UpdateOrientationInstant(double time)
        {
            orientation = orientationDirection;
        }

        /// <summary>
        /// Resolve collision between two moveable objects
        /// </summary>
        /// <param name="other">Other object colliding</param>
        /// <param name="overlap">Rectangle of intersection between bounding rectangles</param>
        public void ResolveCollision(PhysicsComponent2D other, Rectanglef overlap)
        {
            // Normal
            Vector2 contactNormal = position - overlap.Center;
            contactNormal.Normalize();

            // Steps outlined in the class slides
            float Vs = -(1 + coefficientOfRestitution) * -(Math.Abs(Vector2.Dot(velocity, contactNormal)) + Math.Abs(Vector2.Dot(other.velocity, contactNormal)));
            float deltaPD = (1 / mass) + (1 / other.mass);
            float g = Vs / deltaPD;
            contactNormal *= g;

            velocity += contactNormal / mass;
            movementDirection = velocity;
            movementDirection.Normalize();
        }

        /// <summary>
        /// Resolves a collision with a wall
        /// </summary>
        public void ResolveWallCollision(Rectanglef overlap)
        {
            // Normal
            Vector2 contactNormal = position - overlap.Center;
            contactNormal.Normalize();

            // Steps outlined in the class slides
            float Vs = -(1 + coefficientOfRestitution) * -Math.Abs(Vector2.Dot(velocity, contactNormal));
            float deltaPD = (1 / mass);
            float g = Vs / deltaPD;
            contactNormal *= g;

            velocity += contactNormal / mass;
            movementDirection = velocity;
            movementDirection.Normalize();
        }

        /// <summary>
        /// Resolve inter penetration between two moveable objects
        /// </summary>
        /// <param name="overlap">Area of interpenetration</param>
        /// <param name="playerRectangle">Rectangle of the player</param>
        public void ResolveInterPenetration(Rectanglef overlap, Rectanglef playerRectangle)
        {
            if (overlap.Width < overlap.Height)
            {
                if (Math.Abs(overlap.X - playerRectangle.X) < 0.0001)
                {
                    position.X += (overlap.Width + 1);
                }
                else
                {
                    position.X -= (overlap.Width + 1);
                }
            }
            else
            {
                if (Math.Abs(overlap.Y - playerRectangle.Y) < 0.0001)
                {
                    position.Y += (overlap.Height + 1);
                }
                else
                {
                    position.Y -= (overlap.Height + 1);
                }
            }
        }

        /// <summary>
        /// Toggle steering
        /// </summary>
        /// <param name="isSteering">New steering value</param>
        public void ToggleSteering(bool isSteering)
        {
            this.isSteering = isSteering;

            #if (DEBUG)
            {
                Console.WriteLine("Steer is " + (isSteering ? "on" : "off") + ".");
            }
            #endif
        }

        /// <summary>
        /// Sets the pace of the entity to use walk or run values
        /// </summary>
        /// <param name="isFast">Whether or not to set pace to the fast value</param>
        public void SetPace(bool isFast)
        {
            if (isFast)
            {
                maxAcceleration = maxAccelerationRun;
                maxVelocity = maxVelocityRun;
            }
            else
            {
                maxAcceleration = maxAccelerationWalk;
                maxVelocity = maxVelocityWalk;
            }
        }

        #endregion
    }
}
