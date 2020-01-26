using UnityEngine;

namespace ZeroByterGames.BlockBuilder.TransformSystem
{
	public class CameraArrowController : MonoBehaviour
	{
		public Vector3 normal;

		new private MeshRenderer renderer;

		private void Awake()
		{
			renderer = GetComponentInChildren<MeshRenderer>();

			var color = Color.grey;
			if (normal == Vector3.right) color = Color.red;
			if (normal == Vector3.forward) color = Color.blue;
			if (normal == Vector3.up) color = Color.green;

			renderer.material.color = color * .6f;
		}

		private void Update()
		{
			transform.forward = CameraController.GetCamera().transform.InverseTransformDirection(normal);
			transform.localPosition = transform.forward;
		}
	}
}
