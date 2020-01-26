using UnityEngine;

namespace ZeroByterGames.BlockBuilder.TransformSystem
{
	public class ArrowController : MonoBehaviour
	{
		public Vector3 normal;

		new private MeshRenderer renderer;

		private Color color;

		private Plane plane;
		private Transform parentTransform;
		private bool isDragging;
		private Vector3 draggingStartPosition;

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

			plane = new Plane(Vector3.right, 2);
		}

		private void Update()
		{
			if (isDragging)
			{
				float enter;
				var ray = CameraController.GetCamera().ScreenPointToRay(Input.mousePosition);
				if(plane.Raycast(ray, out enter))
				{
					if (normal.x == 1) parentTransform.position = new Vector3(Mathf.Round(ray.GetPoint(enter).x), draggingStartPosition.y, draggingStartPosition.z);
					if (normal.y == 1) parentTransform.position = new Vector3(draggingStartPosition.x, Mathf.Round(ray.GetPoint(enter).y), draggingStartPosition.z);
					if (normal.z == 1) parentTransform.position = new Vector3(draggingStartPosition.x, draggingStartPosition.y, Mathf.Round(ray.GetPoint(enter).z));
					plane.SetNormalAndPosition(transform.right, transform.position);
				}
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
			draggingStartPosition = transform.position;
			isDragging = true;
		}

		private void OnMouseUp()
		{
			isDragging = false;
			renderer.material.color = color * .6f;
		}
	}
}