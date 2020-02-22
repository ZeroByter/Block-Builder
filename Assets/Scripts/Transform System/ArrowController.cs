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
		private Vector3 dragStartPosition;
		private Vector3 dragStartRayPoint;

		private void Awake()
		{
			renderer = GetComponentInChildren<MeshRenderer>();

			color = Color.grey;
			if (normal == Vector3.right) color = Color.red;
			if (normal == Vector3.forward) color = Color.blue;
			if (normal == Vector3.up) color = Color.green;

			transform.forward = normal;
			parentTransform = transform.parent.parent.GetChild(3);

			renderer.material.color = color * .6f;

			plane = new Plane(Vector3.right, 1);
		}

		private void Update()
		{
			if (isDragging)
			{
				if (normal.x == 1) parentTransform.position = dragStartPosition + new Vector3(Mathf.Round(GetPlaneRayPoint().x - dragStartRayPoint.x), 0, 0);
				if (normal.y == 1) parentTransform.position = dragStartPosition + new Vector3(0, Mathf.Round(GetPlaneRayPoint().y - dragStartRayPoint.y), 0);
				if (normal.z == 1) parentTransform.position = dragStartPosition + new Vector3(0, 0, Mathf.Round(GetPlaneRayPoint().z - dragStartRayPoint.z));
			}
		}

		private void OnMouseEnter()
		{
			renderer.material.color = color;
			TransformController.AddSelectedTranformComponents();
		}

		private void OnMouseExit()
		{
			TransformController.MinusSelectedTranformComponents();
			if (isDragging) return;
			renderer.material.color = color * .6f;
		}

		private void OnMouseDown()
		{
			plane.SetNormalAndPosition(transform.right, transform.position);
			dragStartPosition = parentTransform.position;
			dragStartRayPoint = GetPlaneRayPoint();
			isDragging = true;
		}

		private void OnMouseUp()
		{
			//if (isDragging) TransformController.MinusSelectedTranformComponents();
			isDragging = false;
			renderer.material.color = color * .6f;
		}

		private void OnDisable()
		{
			//TransformController.MinusSelectedTranformComponents();
		}

		private Vector3 GetPlaneRayPoint()
		{
			float enter;
			var ray = CameraController.GetCamera().ScreenPointToRay(Input.mousePosition);
			if (!plane.Raycast(ray, out enter))
			{
				return Vector3.zero;
			}

			return ray.GetPoint(enter);
		}
	}
}