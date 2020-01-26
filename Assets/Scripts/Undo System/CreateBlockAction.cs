using UnityEngine;

namespace ZeroByterGames.BlockBuilder.UndoSystem
{
	public class CreateBlockAction : UndoAction
	{
		public int color;

		public CreateBlockAction(int x, int y, int z, int color) : base(x, y, z)
		{
			this.color = color;
		}

		public override void RedoAction()
		{
			ModelManager.AddCube(x, y, z, color);
		}

		public override void ReverseAction()
		{
			ModelManager.RemoveCube(x, y, z);
		}
	}
}