using UnityEngine;

namespace ZeroByterGames.BlockBuilder.UndoSystem
{
	public abstract class UndoAction
	{
		public int x;
		public int y;
		public int z;

		protected UndoAction(int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		/// <summary>
		/// Can't be named `UndoAction` due to conflicting with class name, but this basically should be called 'UndoAction'
		/// </summary>
		public abstract void ReverseAction();
		public abstract void RedoAction();
	}
}