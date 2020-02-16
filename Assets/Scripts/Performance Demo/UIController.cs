using TMPro;
using UnityEngine;

namespace ZeroByterGames.BlockBuilder.PerformanceDemo
{
	public class UIController : MonoBehaviour
	{
		public TestChunkController controller;

		public void NewDropdownValue(int value)
		{
			controller.currentMode = (TestChunkController.Mode)value;
			controller.UpdateMesh();
		}
	}
}
