using UnityEngine;

namespace ZeroByterGames.BlockBuilder.UI
{
	public class UIManager : MonoBehaviour
	{
		public void ExportObj()
		{
			ExportController.ExportTest(ModelManager.GetTestMesh());
		}
	}
}