using UnityEngine;
using System.Collections;

public abstract class BaseMovement : MonoBehaviour
{
		public float m_speed = 250f;
		public int m_orderInLayerAccuracy = 100;

		protected SpriteRenderer m_spriteRenderer;
		protected Animator m_animator;
		protected Rigidbody2D m_rigidBody;

		protected Vector2 m_movementDirection;
		protected bool m_isMoving;

		protected static string S_LOCKED_TAG_NAME = "Locked";
		protected static int S_LOCKED_TAG_HASH;
		protected static string S_UNLOCKED_TAG_NAME = "Unlocked";
		protected static int S_UNLOCKED_TAG_HASH;

		protected static string S_IS_MOVING_PARAMETER_NAME = "IsMoving";
		protected static int S_IS_MOVING_PARAMETER_HASH;
		protected static string S_FALL_PARAMETER_NAME = "Fall";
		protected static int S_FALL_PARAMTER_HASH;

		protected virtual void Start()
		{
				m_spriteRenderer = GetComponent<SpriteRenderer>();
				m_animator = GetComponent<Animator>();
				m_rigidBody = GetComponent<Rigidbody2D>();

				S_LOCKED_TAG_HASH = Animator.StringToHash(S_LOCKED_TAG_NAME);
				S_UNLOCKED_TAG_HASH = Animator.StringToHash(S_UNLOCKED_TAG_NAME);
				S_IS_MOVING_PARAMETER_HASH = Animator.StringToHash(S_IS_MOVING_PARAMETER_NAME);
		}

		protected void Update()
		{
				UpdateState();
				UpdateMovementDirection();
		}

		protected abstract void UpdateState();

		protected abstract void UpdateMovementDirection();

		protected void FixedUpdate()
		{
				UpdatePosition();
				UpdateOrientation();
		}

		private void UpdatePosition()
		{
				Vector3 movement = m_movementDirection * m_speed * Time.deltaTime;

				if (movement.sqrMagnitude > 0f)
				{
						m_rigidBody.AddForce(movement);
				}

				m_spriteRenderer.sortingOrder = (int)(-transform.position.y * m_orderInLayerAccuracy);
		}

		private void UpdateOrientation()
		{
				if (m_movementDirection.x != 0f)
				{
						m_spriteRenderer.flipX = (m_movementDirection.x < 0f);
				}
		}
}