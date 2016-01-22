using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
		public float m_cameraBoundsPadding = 0.25f;
		public Vector3 m_offset;

		private Camera m_camera;
		private Transform m_target;
		private Rect m_cameraBoundsRect;

		// Use this for initialization
		void Start()
		{
				m_camera = Camera.main;
				m_target = GameObject.FindGameObjectWithTag("Player").transform;

				// Determine the camera bounds
				GameObject levelObject = GameObject.FindGameObjectWithTag("Level");
				Debug.Assert(levelObject != null, "[CameraMovement::Start] Level game object not found");
				if (levelObject != null)
				{
						Bounds levelBounds = levelObject.GetComponent<SpriteRenderer>().bounds;

						float minX = levelBounds.min.x + m_camera.orthographicSize * m_camera.aspect - m_cameraBoundsPadding;
						float maxX = levelBounds.max.x - m_camera.orthographicSize * m_camera.aspect + m_cameraBoundsPadding;

						float minY = levelBounds.min.y + m_camera.orthographicSize - m_cameraBoundsPadding;
						float maxY = levelBounds.max.y - m_camera.orthographicSize + m_cameraBoundsPadding;

						m_cameraBoundsRect = new Rect(minX, minY, maxX - minX, maxY - minY);
				}
		}

		// Update is called once per frame
		void FixedUpdate()
		{
				// Follow the target at an offset
				Vector3 desiredPosition = m_target.position + m_offset;

				// Clamp the position within the camera bounds
				desiredPosition.x = Mathf.Clamp(desiredPosition.x, m_cameraBoundsRect.xMin, m_cameraBoundsRect.xMax);
				desiredPosition.y = Mathf.Clamp(desiredPosition.y, m_cameraBoundsRect.yMin, m_cameraBoundsRect.yMax);

				m_camera.transform.position = desiredPosition;
		}
}
