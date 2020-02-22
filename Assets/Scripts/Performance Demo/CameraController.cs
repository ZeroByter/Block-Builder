using UnityEngine;

namespace ZeroByterGames.BlockBuilder.PerformanceDemo
{
	public class CameraController : MonoBehaviour
	{
		public TestChunkController controller;

		private float orbit;

		private void Update()
		{
			orbit += 0.01f;

			Vector3 orbitPoint = controller.transform.TransformPoint(controller.mesh.bounds.center);
			transform.position = orbitPoint + new Vector3(Mathf.Sin(orbit) * 5.5f, 2, Mathf.Cos(orbit) * 5.5f);
			transform.LookAt(orbitPoint);
		}

		private void OnPreRender()
		{
			GL.wireframe = true;
		}

		private void OnPostRender()
		{
			GL.wireframe = false;
		}
	}
}
