using UnityEngine;

namespace ZeroByterGames.BlockBuilder.TransformSystem
{
	public class RingController : MonoBehaviour
	{
		public Vector3 normal;

		new private MeshRenderer renderer;

		private Color color;

		private Transform parentTransform;
		private Quaternion dragStartRotation;
		private bool isDragging;
		private float lastMouseX;
		private float virtualRotationValue;

		private void Awake()
		{
			renderer = GetComponentInChildren<MeshRenderer>();

			color = Color.grey;
			if (normal == Vector3.right) color = Color.red;
			if (normal == Vector3.forward) color = Color.blue;
			if (normal == Vector3.up) color = Color.green;

			transform.forward = normal;
			parentTransform = transform.parent.parent.GetChild(2);

			renderer.material.color = color * .6f;
		}

		private void Update()
		{
			if (isDragging)
			{
				virtualRotationValue += Input.mousePosition.x - lastMouseX;

				//parentTransform.rotation = dragStartRotation * Quaternion.Euler(0, 0, Mathf.Round(virtualRotationValue / 90) * 90);
				if (normal.x == 1) parentTransform.rotation = dragStartRotation * Quaternion.Euler(-virtualRotationValue, 0, 0);
				if (normal.y == 1) parentTransform.rotation = dragStartRotation * Quaternion.Euler(0, virtualRotationValue, 0);
				if (normal.z == 1) parentTransform.rotation = dragStartRotation * Quaternion.Euler(0, 0, virtualRotationValue);

				lastMouseX = Input.mousePosition.x;
			}
		}

		private void OnMouseEnter()
		{
			renderer.material.color = color;
		}

		private void OnMouseExit()
		{
			if (isDragging) return;
			renderer.material.color = color * .6f;
		}

		private void OnMouseDown()
		{
			virtualRotationValue = 0;
			dragStartRotation = parentTransform.rotation;
			lastMouseX = Input.mousePosition.x;
			isDragging = true;
		}

		private void OnMouseUp()
		{
			isDragging = false;
			renderer.material.color = color * .6f;
		}
	}
}
