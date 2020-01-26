using System;
using UnityEngine;
using UnityEngine.UI;

namespace ZeroByterGames.BlockBuilder.UI
{
	public class ToolbarController : MonoBehaviour
	{
		public static Action<Tool> NewToolSelected;

		private static ToolbarController Singleton;

		public static Tool GetCurrentTool()
		{
			if (Singleton == null) return Tool.Create;

			return Singleton.currentTool;
		}

		public enum Tool
		{
			Translate = 0,
			Rotate = 1,
			Select = 2,
			Create = 3,
			Destroy = 4,
			Paint = 5,
			Colorpick = 6
		}
		private Tool currentTool = Tool.Create;

		private void Awake()
		{
			Singleton = this;

			UpdateUI();
		}

		private void Update()
		{
			int offset = 3;
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				SetCurrentTool(offset);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				SetCurrentTool(offset + 1);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				SetCurrentTool(offset + 2);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha4))
			{
				SetCurrentTool(offset + 3);
			}
		}

		private void UpdateUI()
		{
			UpdateSelectedTransformTool();

			UpdateSelectedTool();
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

		private void UpdateSelectedTransformTool()
		{
			int index = -1;
			foreach (Transform tool in transform)
			{
				index++;

				if (index > 1) break;

				if (index == 0) //translate
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

			NewToolSelected?.Invoke(currentTool);

			UpdateUI();
		}
	}
}