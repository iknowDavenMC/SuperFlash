using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;

namespace COMP476Proj
{
    public class MovementAIComponent2D
    {
        #region Attributes

        #region Arrival Attributes

        /// <summary>
        /// Radius in which to slow down when arriving
        /// </summary>
        private float slowDownRadius;

        /// <summary>
        /// Radius in which the player has arrived
        /// </summary>
        private float arrivalRadius;

        /// <summary>
        /// NOT CURRENTLY USED
        /// Speed under which a player can continue moving
        /// </summary>
        private float thresholdSpeed;

        /// <summary>
        /// NOT CURRENTLY USED
        /// Distance under which a player will perform a certain action
        /// </summary>
        private float thresholdDistance;

        /// <summary>
        /// Time in which to arrive at the target
        /// </summary>
        private float timeToTarget;

        #endregion

        #region Allign Attributes

        /// <summary>
        /// Difference in angle at which to slow down rotation
        /// </summary>
        private float slowDownRadiusRotation;

        /// <summary>
        /// Difference in angle at which the orientation is correct
        /// </summary>
        private float arrivalRadiusRotation;

        #endregion

        #region Rotation Attributes

        /// <summary>
        /// Angle in which the player sees another player
        /// </summary>
        private float perceptionAngle;

        /// <summary>
        /// Constant by which the change of perception angle relative to speed can be adjusted
        /// </summary>
        private float perceptionConstant;

        #endregion

        #region Target Attributes

        /// <summary>
        /// Desired position
        /// </summary>
        private Vector2 targetPosition;

        /// <summary>
        /// Desired velocity
        /// </summary>
        private Vector2 targetVelocity;

        /// <summary>
        /// Desired orientation
        /// </summary>
        private float targetOrientation;

        /// <summary>
        /// Desired rotation
        /// </summary>
        private float targetRotation;

        #endregion

        #region Random Number Attributes

        /// <summary>
        /// Random number
        /// </summary>
        static private Random random;

        #endregion

        #endregion

        #region Properties

        public float SlowDownRadius
        {
            get { return slowDownRadius; }
        }

        public float ArrivalRadius
        {
            get { return arrivalRadius; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MovementAIComponent2D()
        {
            slowDownRadius = 3;
            arrivalRadius = 0.5f;

            slowDownRadiusRotation = MathHelper.ToRadians(45);
            arrivalRadiusRotation = MathHelper.ToRadians(5);

            thresholdSpeed = 3;
            thresholdDistance = 2;

            perceptionAngle = MathHelper.ToRadians(45);
            perceptionConstant = 0.5f;

            timeToTarget = 0.1f;

            targetPosition = Vector2.Zero;
            targetVelocity = Vector2.Zero;

            if (random == null)
            {
                random = new Random();
            }
        }

        /// <summary>
        /// Basic Construtor
        /// </summary>
        /// <param name="thresholdSpeed">Speed under which a player can continue moving</param>
        /// <param name="thresholdDistance">Distance under which a player will perform a certain action</param>
        /// <param name="perceptionAngle">Angle in which the player sees another player</param>
        /// <param name="perceptionConstant">Constant by which the change of perception angle relative to speed can be adjusted</param>
        /// <param name="slowDownRadius">Radius in which to slow down when arriving</param>
        /// <param name="arrivalRadius">Radius in which the player has arrived</param>
        /// <param name="position">Target position</param>
        /// <param name="velocity">Target velocity</param>
        /// <param name="timeToTarget">Time in which to arrive at the target</param>
        public MovementAIComponent2D(float thresholdSpeed, float thresholdDistance, float perceptionAngle, float perceptionConstant,
            float slowDownRadius, float arrivalRadius, Vector2 position, Vector2 velocity,
            float timeToTarget)
        {
            this.thresholdSpeed = thresholdSpeed;
            this.thresholdDistance = thresholdDistance;

            this.perceptionAngle = perceptionAngle;
            this.perceptionConstant = perceptionConstant;

            this.slowDownRadius = slowDownRadius;
            this.arrivalRadius = arrivalRadius;

            slowDownRadiusRotation = MathHelper.ToRadians(50);
            arrivalRadiusRotation = MathHelper.ToRadians(10);

            targetPosition = position;
            targetVelocity = velocity;

            this.timeToTarget = timeToTarget;

            if (random == null)
            {
                random = new Random();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Generates a roughly normally distributed random number around 0
        /// </summary>
        /// <returns>Approximately normally distributed number around 0</returns>
        private double normallyDistributedRandomNumber()
        {
            double a = 2 * random.NextDouble() - 1;
            double b = 2 * random.NextDouble() - 1;

            return a * b;
        }

        /// <summary>
        /// Kinematic arrive to the target
        /// Code is based on the code in the AI book
        /// </summary>
        /// <param name="physics">Physics component of the target</param>
        private void kinematicArrive(ref PhysicsComponent2D physics)
        {
            Vector2 velocity = targetPosition - physics.Position;

            if (velocity.Length() < arrivalRadius)
            {
                return;
            }

            velocity /= timeToTarget;

            if (velocity.Length() > physics.MaxVelocity)
            {
                velocity.Normalize();
                velocity *= physics.MaxVelocity;
            }

            physics.SetTargetValues(false, velocity, velocity, null);
        }

        /// <summary>
        /// Steering arrive to the target
        /// Code is based on the code in the AI book
        /// </summary>
        /// <param name="physics">Physics component of the target</param>
        private void steeringArrive(ref PhysicsComponent2D physics)
        {
            Vector2 direction = targetPosition - physics.Position;
            float distance = direction.Length();
            float speed;

            if (distance < arrivalRadius)
            {
                return;
            }

            if (distance > slowDownRadius)
            {
                speed = physics.MaxVelocity;
            }
            else
            {
                speed = physics.MaxVelocity * distance / slowDownRadius;
            }

            Vector2 velocity = direction;
            velocity.Normalize();
            velocity *= speed;

            Vector2 targetAcceleration = velocity - physics.Velocity;
            targetAcceleration /= timeToTarget;

            if (targetAcceleration.Length() > physics.MaxAcceleration)
            {
                targetAcceleration.Normalize();
                targetAcceleration *= physics.MaxAcceleration;
            }

            physics.SetTargetValues(false, velocity, null, targetAcceleration);
        }

        /// <summary>
        /// Allign to the desired orientation
        /// Code is based on the code in the AI book
        /// </summary>
        /// <param name="physics">Physics component of the target</param>
        private void allign(ref PhysicsComponent2D physics)
        {
            float rotation = MathHelper.WrapAngle(targetOrientation - physics.Orientation);
            float rotationSize = Math.Abs(rotation);

            if (rotationSize < arrivalRadiusRotation)
            {
                physics.SetTargetValues(targetOrientation, -Math.Sign(physics.Rotation) * physics.MaxAngularAcceleration);
                return;
            }

            if (rotationSize > slowDownRadiusRotation)
            {
                targetRotation = physics.MaxRotation;
            }
            else
            {
                targetRotation = physics.MaxRotation * rotationSize / slowDownRadiusRotation;
            }

            targetRotation *= rotation / rotationSize;

            float angular = (targetRotation - physics.Rotation) / timeToTarget;

            float angularAcceleration = Math.Abs(angular);

            if (angularAcceleration > physics.MaxAngularAcceleration)
            {
                angular /= angularAcceleration;
                angular *= physics.MaxAngularAcceleration;
            }

            physics.SetTargetValues(targetOrientation, angular);
        }

        /// <summary>
        /// Checks to see whether or not the target is in a cone of perception
        /// Perception is based on angle and velocity (as velocity increases,
        /// cone size decreases)
        /// </summary>
        /// <param name="physics">Physics component of the target</param>
        /// <returns>Whether or not the target can be seen</returns>
        private bool isInPerceptionZone(ref PhysicsComponent2D physics)
        {
            Vector2 direction = targetPosition - physics.Position;

            if (direction.Length() == 0)
            {
                return true;
            }

            float angle = (float)Math.Atan2(direction.X, direction.Y);

            return (Math.Abs(angle - physics.Orientation) < (perceptionAngle / (perceptionConstant * physics.Velocity.Length() + 1)));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the position to target
        /// </summary>
        /// <param name="targetPosition">Desired position</param>
        public void SetTarget(Vector2 targetPosition)
        {
            this.targetPosition = targetPosition;
        }

        /// <summary>
        /// Seek movement
        /// </summary>
        /// <param name="physics">The physics component of the thinking character</param>
        public void Seek(ref PhysicsComponent2D physics)
        {
            // Seek normally
            physics.SetTargetValues(false, targetPosition - physics.Position, null, null);
        }

        /// <summary>
        /// Arrive movement
        /// </summary>
        /// <param name="physics">The physics component of the thinking character</param>
        public void Arrive(ref PhysicsComponent2D physics)
        {
            // Call appropriate behaviour
            if (physics.IsSteering)
            {
                steeringArrive(ref physics);
            }
            else
            {
                kinematicArrive(ref physics);
            }
        }

        /// <summary>
        /// Pursue movement
        /// Code is based on the code in the AI book
        /// </summary>
        /// <param name="physics">The physics component of the thinking character</param>
        public void Pursue(ref PhysicsComponent2D physics)
        {
            if (targetVelocity.Length() == 0)
            {
                Flee(ref physics);
                return;
            }

            float maxPrediction = 0.5f;
            float prediction;

            Vector2 direction = targetPosition - physics.Position;
            float distance = direction.Length();

            float speed = physics.Velocity.Length();

            if (speed <= distance / maxPrediction)
            {
                prediction = maxPrediction;
            }
            else
            {
                prediction = distance / speed;
            }

            Vector2 targetPos = targetPosition;
            targetPos += targetVelocity * prediction;
            physics.SetTargetValues(false, targetPos, null, null);
        }

        /// <summary>
        /// Flee movement
        /// </summary>
        /// <param name="physics">The physics component of the thinking character</param>
        public void Flee(ref PhysicsComponent2D physics)
        {
            // Flee normally
            Vector2 toFlee = physics.Position + (physics.Position - targetPosition);
            physics.SetTargetValues(false, toFlee, Vector2.Zero, Vector2.Zero);
        }

        /// <summary>
        /// Evade movement
        /// Code is based on the code in the AI book
        /// </summary>
        /// <param name="physics">The physics component of the thinking character</param>
        public void Evade(ref PhysicsComponent2D physics)
        {
            if (targetVelocity.Length() == 0)
            {
                Flee(ref physics);
                return;
            }

            float maxPrediction = 0.1f;
            float prediction;

            Vector2 direction = targetPosition - physics.Position;
            float distance = direction.Length();

            float speed = physics.Velocity.Length();

            if (speed <= distance / maxPrediction)
            {
                prediction = maxPrediction;
            }
            else
            {
                prediction = distance / speed;
            }

            Vector2 targetPos = targetPosition;
            targetPos += targetVelocity * prediction;

            // Flee normally
            targetPos = physics.Position + (physics.Position - targetPos);
            physics.SetTargetValues(false, targetPos, null, null);
        }

        /// <summary>
        /// Wander movement
        /// </summary>
        /// <param name="physics">The physics component of the thinking character</param>
        public void Wander(ref PhysicsComponent2D physics)
        {
            Vector2 direction;
            float angle;

            if (physics.Direction.Length() == 0)
            {
                Random random = new Random();
                float x, y;

                do
                {
                    x = random.Next(-1, 2);
                } while (x == 0);

                do
                {
                    y = random.Next(-1, 2);
                } while (y == 0);

                direction = new Vector2(x, y);
                direction.Normalize();
            }
            else
            {
                do
                {
                    angle = MathHelper.ToRadians(135 * (float)normallyDistributedRandomNumber());
                    direction.X = (float)Math.Cos(physics.Orientation + angle);
                    direction.Y = -(float)Math.Sin(physics.Orientation + angle);
                    direction.Normalize();
                }
                while (direction.Length() == 0);
            }

            // Seek normally
            physics.SetTargetValues(false, direction, null, null);
        }

        /// <summary>
        /// Stop movement
        /// </summary>
        /// <param name="physics">The physics component of the thinking character</param>
        public void Stop(ref PhysicsComponent2D physics)
        {
            physics.SetTargetValues(true, null, null, null);
        }

        /// <summary>
        /// Look where you're going, delegated to allign
        /// Code is based on the code in the AI book
        /// </summary>
        /// <param name="physics">The physics component of the thinking character</param>
        public void Look(ref PhysicsComponent2D physics)
        {
            if (physics.Velocity.Length() == 0)
            {
                return;
            }

            targetOrientation = (float)Math.Atan2(physics.Velocity.X, physics.Velocity.Y);

            allign(ref physics);
        }

        /// <summary>
        /// Look at a point, delegated to allign
        /// Code is based on the code in the AI book
        /// </summary>
        /// <param name="physics">The physics component of the thinking character</param>
        public void Face(ref PhysicsComponent2D physics)
        {
            Vector2 direction = targetPosition - physics.Position;

            if (direction.Length() == 0)
            {
                return;
            }

            targetOrientation = (float)Math.Atan2(direction.X, direction.Y);

            allign(ref physics);
        }

        /// <summary>
        /// Look away from a point, delegated to allign
        /// Code is based on the code in the AI book
        /// </summary>
        public void FaceAway(ref PhysicsComponent2D physics)
        {
            Vector2 direction = physics.Position - targetPosition;

            if (direction.Length() == 0)
            {
                return;
            }

            targetOrientation = (float)Math.Atan2(direction.X, direction.Y);

            allign(ref physics);
        }

        #endregion
    }
}
