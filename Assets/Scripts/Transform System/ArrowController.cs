using UnityEngine;

namespace ZeroByterGames.BlockBuilder.TransformSystem
{
	public class ArrowController : MonoBehaviour
	{
		public Vector3 normal;

		new private MeshRenderer renderer;

		private Color color;

		private void Awake()
		{
			renderer = GetComponentInChildren<MeshRenderer>();

			color = Color.grey;
			if (normal == Vector3.right) color = Color.red;
			if (normal == Vector3.forward) color = Color.blue;
			if (normal == Vector3.up) color = Color.green;

			transform.right = normal;

			renderer.material.color = color * .6f;
		}

		private void OnMouseEnter()
		{
			renderer.material.color = color;
		}

		private void OnMouseExit()
		{
			renderer.material.color = color * .6f;
		}
	}
}