using UnityEngine;
using ZeroByterGames.BlockBuilder.UI;

namespace ZeroByterGames.BlockBuilder
{
	public class WorkspaceWallController : MonoBehaviour
	{
		public Vector3 facingNormal;

		private void Awake()
		{
			transform.forward = facingNormal;

			transform.position = facingNormal * -0.5f;

			WorkspaceSizeManager.NewSizeSet += OnNewSizeSet;
		}

		private void OnDestroy()
		{
			WorkspaceSizeManager.NewSizeSet -= OnNewSizeSet;
		}

		private void OnNewSizeSet(Vector3Int size)
		{
			if (facingNormal.x == 1 || facingNormal.x == -1)
			{
				transform.position = new Vector3(size.x / (2f * -facingNormal.x), 0, 0);
				transform.localScale = new Vector3(-size.z, size.y, size.x);
			}
			if (facingNormal.z == 1 || facingNormal.z == -1)
			{
				transform.position = new Vector3(0, 0, size.z / (2f * -facingNormal.z));
				transform.localScale = new Vector3(-size.x, size.y, size.z);
			}
			if (facingNormal.y == 1 || facingNormal.y == -1)
			{
				transform.position = new Vector3(0, size.y / (2f * -facingNormal.y), 0);
				transform.localScale = new Vector3(-size.x, size.y, size.z);
			}
		}
	}
}
