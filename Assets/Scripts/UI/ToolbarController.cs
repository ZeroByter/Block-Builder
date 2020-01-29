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
			Select = 0,
			Create = 1,
			Destroy = 2,
			Paint = 3,
			Colorpicker = 4
		}
		private Tool currentTool = Tool.Create;

		private void Awake()
		{
			Singleton = this;

			UpdateUI();
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				SetNewTool(Tool.Create);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				SetNewTool(Tool.Destroy);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				SetNewTool(Tool.Paint);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha4))
			{
				SetNewTool(Tool.Colorpicker);
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

		public void SetNewTool(int index)
		{
			SetNewTool((Tool)index);
		}

		private void SetNewTool(Tool tool)
		{
			currentTool = tool;

			NewToolSelected?.Invoke(currentTool);

			UpdateUI();
		}
	}
}