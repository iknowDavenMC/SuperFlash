using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
		// TODO: Figure out how to determine these values
		public Rect m_cameraBounds;
		public Vector3 m_offset;

		private Camera m_camera;
		private Transform m_target;

		// Use this for initialization
		void Start()
		{
				m_camera = Camera.main;
				m_target = GameObject.FindGameObjectWithTag("Player").transform;
		}

		// Update is called once per frame
		void FixedUpdate()
		{
				Vector3 desiredPosition = m_target.position + m_offset;

				desiredPosition.x = Mathf.Clamp(desiredPosition.x, m_cameraBounds.xMin, m_cameraBounds.xMax);
				desiredPosition.y = Mathf.Clamp(desiredPosition.y, m_cameraBounds.yMin, m_cameraBounds.yMax);

				m_camera.transform.position = desiredPosition;
		}
}
