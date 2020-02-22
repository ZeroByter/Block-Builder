using System.Collections.Generic;
using UnityEngine;

namespace ZeroByterGames.BlockBuilder
{
	public class CameraController : MonoBehaviour
	{
		new private Camera camera;

		private Vector3 dragStartPosition;
		private Vector2 dragStartMousePosition;
		private float dragSpeedMultiplier = 0.025f;

		private void Awake()
		{
			camera = GetComponent<Camera>();
		}

		private void Update()
		{
			/*if (Input.GetKeyDown(KeyCode.Mouse1))
			{
				dragStartPosition = transform.position;
				dragStartMousePosition = Input.mousePosition;
			}

			if (Input.GetKey(KeyCode.Mouse1))
			{
				transform.position = (dragStartPosition + transform.right * ((Input.mousePosition.x - dragStartMousePosition.x) * dragSpeedMultiplier)) + (transform.up * ((Input.mousePosition.y - dragStartMousePosition.y) * dragSpeedMultiplier));
			}*/

			if (Input.GetKeyDown(KeyCode.Mouse0))
			{
				ModelManager.AddCube(0, 0, 0);
			}

			//camera.orthographicSize = Mathf.Clamp(camera.orthographicSize - Input.mouseScrollDelta.y * 0.2f, 0.2f, 10f);
		}
	}
}
