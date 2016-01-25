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

		private Transform m_streakerTransform;
		private RaycastHit2D m_rayCastHit;
		private int m_streakerVisibilityMask;
		private int m_movementObstacleMask;

		void OnCollisionEnter2D(Collision2D other)
		{
				// The student falls when bumped by the streaker or a cop if he isn't already on the ground
				if (!m_isFallen && (other.gameObject.tag == UnityEditorConstants.S_PLAYER_TAG_NAME || other.gameObject.tag == UnityEditorConstants.S_COP_TAG_NAME))
				{
						Fall();
				}
		}

		protected override void Start()
		{
				m_isFleeing = false;

				m_streakerTransform = GameObject.FindGameObjectWithTag(UnityEditorConstants.S_PLAYER_TAG_NAME).transform;

				m_streakerVisibilityMask = LayerMask.GetMask(UnityEditorConstants.S_VISION_OBSTACLE_LAYER_NAME, UnityEditorConstants.S_PLAYER_LAYER_NAME);
				m_movementObstacleMask = LayerMask.GetMask(UnityEditorConstants.S_MOVEMENT_OBSTACLE_LAYER_NAME);

				S_IS_FLEEING_PARAMETER_HASH = Animator.StringToHash(S_IS_FLEEING_PARAMETER_NAME);

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
						m_animator.SetBool(S_IS_FLEEING_PARAMETER_HASH, m_isFleeing);
				}

				return true;
		}
}