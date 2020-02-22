using UnityEngine;

namespace ZeroByterGames.BlockBuilder.UndoSystem
{
	public class PaintBlockAction : UndoAction
	{
		private int previousColor;
		private int newColor;

		public PaintBlockAction(int x, int y, int z, int previousColor, int newColor) : base(x, y, z)
		{
			this.previousColor = previousColor;
			this.newColor = newColor;
		}

		public override void RedoAction()
		{
			ModelManager.AddCube(x, y, z, newColor);
		}

		public override void ReverseAction()
		{
			ModelManager.AddCube(x, y, z, previousColor);
		}
	}
}