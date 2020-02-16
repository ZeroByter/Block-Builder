using UnityEngine;

namespace ZeroByterGames.BlockBuilder
{
	public class CameraController : MonoBehaviour
	{
		private int i;

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Mouse0))
			{
				ModelManager.AddCube(i++, 0, 0);
				ModelManager.AddCube(i++, 0, 0);
				ModelManager.AddCube(i++, 0, 0);
			}
		}
	}
}
