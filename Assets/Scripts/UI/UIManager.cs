using Ookii.Dialogs;
using SFB;
using UnityEngine;

namespace ZeroByterGames.BlockBuilder.UI
{
	public class UIManager : MonoBehaviour
	{
		public void ExportObj()
		{
			var savePath = StandaloneFileBrowser.SaveFilePanel("Save Model", "", "myObject.obj", "obj");

			if(savePath != "")
			{
				ExportController.Export(savePath, ModelManager.GetCompleteMesh());
			}
		}
	}
}