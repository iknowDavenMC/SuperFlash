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
        /// 0 points to the right of the screen
        /// Pi points to the left of the screen
        /// Pi/2 points to the bottom of the screen
        /// -Pi/2 points to the top of the screen
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

        /// <summary>
        /// Bounding Rectangle for collisions
        /// </summary>
        protected BoundingRectangle boundingBox;

        #region Movement-related Bounds

        /// <summary>
        /// Max Velocity
        /// </summary>
        protected float maxVelocity = 6.25f; // Average running speed

        /// <summary>
        /// Max Acceleration
        /// </summary>
        protected float maxAcceleration = 2.5f; // Assuming it takes 2.5 seconds to reach max speed

        #endregion

        #region Orientation-related Bounds

        /// <summary>
        /// Max rotation
        /// </summary>
        protected float maxRotation = MathHelper.ToRadians(240); // It takes about 1.5 seconds to do a full turn

        /// <summary>
        /// Max angular acceleration
        /// </summary>
        protected float maxAngularAcceleration = MathHelper.ToRadians(480); // Assuming it takes 0.5 seconds to reach max rotation speed

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

        public BoundingRectangle BoundingBox
        {
            get { return boundingBox; }
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
        /// <param name="dimensions">Dimensions of the object</param>
        /// <param name="isSteering">Is steering being employed, or is kinematic movement being used</param>
        public PhysicsComponent2D(Vector2 position, float orientation, Vector2 dimensions,
            bool isSteering = false)
        {
            // Specified
            this.position = position;
            this.orientation = orientationDirection = orientation;
            this.isSteering = isSteering;
            boundingBox = new BoundingRectangle(position, (int)dimensions.X, (int)dimensions.Y);

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
                acceleration = velocity * -1;

                // Capped by max acceleration
                if (acceleration.Length() > maxAcceleration)
                {
                    acceleration.Normalize();
                    acceleration *= maxAcceleration;
                }

                // Update velocity
                velocity += acceleration * (float)time;

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
        /// Sets the character to move right
        /// </summary>
        public void MoveRight()
        {
            movementDirection = new Vector2(1, 0);
        }

        /// <summary>
        /// Sets the character to move left
        /// </summary>
        public void MoveLeft()
        {
            movementDirection = new Vector2(-1, 0);
        }

        /// <summary>
        /// Sets the character to move up
        /// </summary>
        public void MoveUp()
        {
            movementDirection = new Vector2(0, -1);
        }

        /// <summary>
        /// Sets the character to move down
        /// </summary>
        public void MoveDown()
        {
            movementDirection = new Vector2(0, 1);
        }

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

            // Updates bounding box
            boundingBox.Update(position);
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
        /// Toggle steering
        /// </summary>
        public void ToggleSteering()
        {
            isSteering = !isSteering;

            #if (DEBUG)
            {
                Console.WriteLine("Steer is " + (isSteering ? "on" : "off") + ".");
            }
            #endif
        }

        #endregion
    }
}
