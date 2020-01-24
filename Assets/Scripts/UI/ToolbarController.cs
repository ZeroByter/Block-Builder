using UnityEngine;
using UnityEngine.UI;

namespace ZeroByterGames.BlockBuilder.UI
{
	public class ToolbarController : MonoBehaviour
	{
		private static ToolbarController Singleton;

		public static Tool GetCurrentTool()
		{
			if (Singleton == null) return Tool.Create;

			return Singleton.currentTool;
		}

		public enum Tool
		{
			Create = 0,
			Destroy = 1,
			Paint = 2,
			Colorpick = 3
		}
		private Tool currentTool;

		private void Awake()
		{
			Singleton = this;

			UpdateSelectedTool();
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				SetCurrentTool(0);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				SetCurrentTool(1);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				SetCurrentTool(2);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha4))
			{
				SetCurrentTool(3);
			}
		}

		private void UpdateSelectedTool()
		{
			int index = -1;
			foreach (Transform tool in transform)
			{
				index++;

				if(index == (int)currentTool)
				{
					tool.GetComponent<Image>().color = Color.white;
				}
				else
				{
					tool.GetComponent<Image>().color = Color.black;
				}
			}
		}

		public void SetCurrentTool(int index)
		{
			currentTool = (Tool)index;

			UpdateSelectedTool();
		}
	}
}