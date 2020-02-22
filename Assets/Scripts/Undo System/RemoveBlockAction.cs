using UnityEngine;

namespace ZeroByterGames.BlockBuilder.UndoSystem
{
	public class RemoveBlockAction : UndoAction
	{
		public int color;

		public RemoveBlockAction(int x, int y, int z, int color) : base(x, y, z)
		{
			this.color = color;
		}

		public override void RedoAction()
		{
			//ModelManager.RemoveCube(x, y, z);
		}

		public override void ReverseAction()
		{
			//ModelManager.AddCube(x, y, z, color);
		}
	}
}