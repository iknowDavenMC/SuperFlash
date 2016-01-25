using UnityEngine;
using System.Collections;

public class StudentMovement : NPCMovement
{
		public float m_walkingSpeed = 100f;
		public float m_runningSpeed = 150f;
		public float m_sightRange = 3f;

		protected bool m_isFleeing; 

		protected static string S_IS_FLEEING_PARAMETER_NAME = "IsFleeing";
		protected static int S_IS_FLEEING_PARAMETER_HASH;

		protected static string S_PLAYER_TAG_NAME = "Player";

		protected static string S_VISION_OBSTACLE_LAYER_NAME = "VisionObstacle";
		protected static string S_MOVEMENT_OBSTACLE_LAYER_NAME = "MovementObstacle";
		protected static string S_PLAYER_LAYER_NAME = "Player";

		private Transform m_streakerTransform;
		private RaycastHit2D m_rayCastHit;
		private int m_streakerVisibilityMask;
		private int m_movementObstacleMask;

		protected override void Start()
		{
				m_isFleeing = false;

				m_streakerTransform = GameObject.FindGameObjectWithTag(S_PLAYER_TAG_NAME).transform;

				m_streakerVisibilityMask = LayerMask.GetMask(S_VISION_OBSTACLE_LAYER_NAME, S_PLAYER_LAYER_NAME);
				m_movementObstacleMask = LayerMask.GetMask(S_MOVEMENT_OBSTACLE_LAYER_NAME);

				base.Start();
		}

		protected override void UpdateState()
		{
				// Check if the streaker is visible
				m_rayCastHit = Physics2D.Raycast(transform.position, (m_streakerTransform.position - transform.position).normalized, m_sightRange, m_streakerVisibilityMask);

				bool shouldFlee = false;

				if (m_rayCastHit.collider != null)
				{
						Rigidbody2D streakerRigidBody = m_rayCastHit.collider.GetComponent<Rigidbody2D>();

						// If the streaker is visible
						if (streakerRigidBody != null)
						{
								shouldFlee = true;
						}
				}

				SetFleeingState(shouldFlee);


// 				if (Physics2D.Raycast(m_ray, out m_rayCastHit, m_sightRange, m_streakerVisibilityMask))
// 				{
// 						
// 				}
// 
// 				// Check if the streaker is visible
// 				if (Physics2D.Raycast(m_ray, out m_rayCastHit, m_sightRange, LayerMask.GetMask("Characters")))
// 				{
// 						Rigidbody2D streakerRigidBody = m_rayCastHit.collider.GetComponent<Rigidbody2D>();
// 
// 						// If the streaker is visible
// 						if (streakerRigidBody != null)
// 						{
// 								SetFleeingState(true);
// 						}
// 				}
		}

		protected override void UpdateMovementDirection()
		{
				if (m_isFleeing)
				{
						m_movementDirection = (transform.position - m_streakerTransform.position).normalized;
				}
				else
				{
						m_movementDirection = Wander();
				}
		}

		private bool SetFleeingState(bool isFleeing)
		{
				// Set the fleeing state if it is new
				if (isFleeing != m_isFleeing)
				{
						m_isFleeing = isFleeing;

						if (m_isFleeing)
						{
								// Play sound
								
						}

						m_speed = m_isFleeing ? m_runningSpeed : m_walkingSpeed;

						m_animator.SetBool(S_IS_MOVING_PARAMETER_HASH, m_movementDirection != Vector2.zero);
						m_animator.SetBool(S_IS_FLEEING_PARAMETER_NAME, m_isFleeing);
				}

				return true;
		}
}