using UnityEngine;

namespace ZeroByterGames.BlockBuilder.TransformSystem
{
	public class ObjectContainerController : MonoBehaviour
	{
		private static ObjectContainerController Singleton;

		

		public static MoveTarget GetMoveTarget()
		{
			if (Singleton == null) return MoveTarget.CubeSelection;

			return Singleton.moveTarget;
		}

		public enum MoveTarget
		{
			CubeSelection,
			GuideSprite
		}
		private MoveTarget moveTarget;

		private void Awake()
		{
			Singleton = this;
		}
	}
}
