using Ookii.Dialogs;
using SFB;
using UnityEngine;

namespace ZeroByterGames.BlockBuilder.UI
{
	public class UIManager : MonoBehaviour
	{
		public void Save()
		{
			var savePath = StandaloneFileBrowser.SaveFilePanel("Save", "", "myObject.3da", "3da");

			if (savePath != "")
			{
				SaveOpenManager.Save(savePath);
			}
		}

		public void Open()
		{
			var openPath = StandaloneFileBrowser.OpenFilePanel("Open", "", "3da", false);

			if (openPath.Length > 0)
			{
				SaveOpenManager.Open(openPath[0]);
			}
		}

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