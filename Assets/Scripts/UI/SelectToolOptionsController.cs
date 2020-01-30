using System;
using UnityEngine;
using UnityEngine.UI;
using static ZeroByterGames.BlockBuilder.UI.ToolbarController;

namespace ZeroByterGames.BlockBuilder.UI
{
	public class SelectToolOptionsController : MonoBehaviour
	{
		public static Action<SelectTool> NewSelectTool;

		private static SelectToolOptionsController Singleton;

		public static SelectTool GetCurrentTool()
		{
			if (Singleton == null) return SelectTool.Translate;

			return Singleton.currentTool;
		}

		public enum SelectTool
		{
			Translate = 0,
			Rotate = 1,
			Scale = 2
		}
		private SelectTool currentTool;

		private CanvasGroup group;

		private void Awake()
		{
			Singleton = this;

			group = GetComponent<CanvasGroup>();

			group.alpha = 0;

			ToolbarController.NewToolSelected += OnNewToolSelected;

			SetNewTool(SelectTool.Translate);
		}

		public void SetNewTool(int index)
		{
			SetNewTool((SelectTool)index);
		}

		private void SetNewTool(SelectTool tool)
		{
			currentTool = tool;

			NewSelectTool?.Invoke(tool);

			UpdateSelectedTransformTool();
		}

		private void OnNewToolSelected(Tool newTool)
		{
			group.alpha = newTool == Tool.Select ? 1 : 0;
		}

		private void UpdateSelectedTransformTool()
		{
			int index = -1;
			foreach (Transform tool in transform)
			{
				index++;

				if (index == (int)currentTool) //translate
				{
					tool.GetComponent<Image>().color = Color.white;
				}
				else
				{
					tool.GetComponent<Image>().color = Color.black;
				}
			}
		}
	}
}