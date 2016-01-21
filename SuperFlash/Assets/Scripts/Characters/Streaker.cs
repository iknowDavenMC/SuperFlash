using UnityEngine;
using System.Collections;

public class Streaker : MonoBehaviour
{
		public float m_speed = 250f;

		private SpriteRenderer m_spriteRenderer;
		private Animator m_animator;
		private Rigidbody2D m_rigidBody;

		private Vector2 m_movementDirection;
		private bool m_isMoving;

		private static int S_LOCKED_TAG_HASH;
		private static int S_UNLOCKED_TAG_HASH;
		private static int S_IS_MOVING_PARAMETER_HASH;

		private static string S_LOCKED_TAG_NAME = "Locked";
		private static string S_UNLOCKED_TAG_NAME = "Unlocked";
		private static string S_IS_MOVING_PARAMETER_NAME = "IsMoving";
		private static string S_HORIZONTAL_INPUT_AXIS = "Horizontal";
		private static string S_VERTICAL_INPUT_AXIS = "Vertical";

		// Use this for initialization
		void Start()
		{
				m_spriteRenderer = GetComponent<SpriteRenderer>();
				m_animator = GetComponent<Animator>();
				m_rigidBody = GetComponent<Rigidbody2D>();

				S_LOCKED_TAG_HASH = Animator.StringToHash(S_LOCKED_TAG_NAME);
				S_UNLOCKED_TAG_HASH = Animator.StringToHash(S_UNLOCKED_TAG_NAME);
				S_IS_MOVING_PARAMETER_HASH = Animator.StringToHash(S_IS_MOVING_PARAMETER_NAME);
		}

		// Update is called once per frame
		void Update()
		{
				AnimatorStateInfo currentAnimatorStateInfo = m_animator.GetCurrentAnimatorStateInfo(0);

				if (currentAnimatorStateInfo.tagHash == S_UNLOCKED_TAG_HASH)
				{
						HandleUserInput();
				}
		}

		void FixedUpdate()
		{
				UpdatePosition();
				UpdateOrientation();
		}

		private void HandleUserInput()
		{
				// Handle the user input
				m_movementDirection.x = Input.GetAxisRaw(S_HORIZONTAL_INPUT_AXIS);
				m_movementDirection.y = Input.GetAxisRaw(S_VERTICAL_INPUT_AXIS);
				m_movementDirection.Normalize();

				// Handle the animation update
				m_isMoving = (m_movementDirection != Vector2.zero);
				m_animator.SetBool(S_IS_MOVING_PARAMETER_HASH, m_isMoving);
		}

		private void UpdatePosition()
		{
				Vector3 movement = m_movementDirection * m_speed * Time.deltaTime;

				if (movement.sqrMagnitude > 0)
				{
						m_rigidBody.AddForce(movement);
				}
		}

		private void UpdateOrientation()
		{
				if (m_movementDirection.x != 0f)
				{
						m_spriteRenderer.flipX = (m_movementDirection.x < 0f);
				}
		}
}
