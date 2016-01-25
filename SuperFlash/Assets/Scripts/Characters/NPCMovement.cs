using UnityEngine;
using System.Collections;

public abstract class NPCMovement : BaseMovement
{
		public float m_wanderAngle = 10f;
		public float m_wanderDirectionSmoothness = 5f;

		private float normallyDistributedRandomNumber()
		{
				float a = 2f * Random.value - 1f;
				float b = 2f * Random.value - 1f;

				return a * b;
		}

		public Vector2 Wander()
		{
				Vector2 direction;

				if (m_movementDirection == Vector2.zero)
				{
						direction = new Vector2(Random.value * m_wanderDirectionSmoothness - m_wanderDirectionSmoothness / 2f,
								Random.value * m_wanderDirectionSmoothness - m_wanderDirectionSmoothness / 2f);
				}
				else
				{
						float angle = Mathf.Deg2Rad * m_wanderAngle * normallyDistributedRandomNumber();
						float sinAngle = Mathf.Sin(angle);
						float cosAngle = Mathf.Sin(angle);

						direction.x = cosAngle * m_movementDirection.x - sinAngle * m_movementDirection.y;
						direction.y = sinAngle * m_movementDirection.x + cosAngle * m_movementDirection.y;
				}

				direction.Normalize();
				return direction;
		}
}
