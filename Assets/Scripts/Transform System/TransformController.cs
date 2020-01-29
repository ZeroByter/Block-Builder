using UnityEngine;
using ZeroByterGames.BlockBuilder.UI;
using static ZeroByterGames.BlockBuilder.UI.SelectToolOptionsController;
using static ZeroByterGames.BlockBuilder.UI.ToolbarController;

namespace ZeroByterGames.BlockBuilder.TransformSystem
{
	public class TransformController : MonoBehaviour
	{
		private static TransformController Singleton;

		public static void UpdateVisibleTools()
		{
			if (Singleton == null) return;

			var tool = ToolbarController.GetCurrentTool();
			var selectTool = SelectToolOptionsController.GetCurrentTool();

			Singleton.translateParent.SetActive(tool == Tool.Select && selectTool == SelectToolOptionsController.SelectTool.Translate && CubeSelectionController.GetAllCubes().Length > 0);
			Singleton.rotateParent.SetActive(tool == Tool.Select && selectTool == SelectToolOptionsController.SelectTool.Rotate && CubeSelectionController.GetAllCubes().Length > 0);
		}

		public static void SetToolsPosition(Vector3 position)
		{
			if (Singleton == null) return;

			Singleton.translateParent.transform.position = position;
			Singleton.rotateParent.transform.position = position;
		}

		public static void AddSelectedTranformComponents()
		{
			if (Singleton == null) return;

			Singleton.selectedTranformComponents++;
		}

		public static void MinusSelectedTranformComponents()
		{
			if (Singleton == null) return;

			Singleton.selectedTranformComponents = Mathf.Max(Singleton.selectedTranformComponents - 1, 0);
		}

		public static int GetSelectedTranformComponents()
		{
			if (Singleton == null) return 0;

			return Singleton.selectedTranformComponents;
		}

		private GameObject translateParent;
		private GameObject rotateParent;

		private int selectedTranformComponents;

		private void Awake()
		{
			Singleton = this;

			translateParent = transform.GetChild(0).gameObject;
			rotateParent = transform.GetChild(1).gameObject;

			translateParent.SetActive(false);
			rotateParent.SetActive(false);

			ToolbarController.NewToolSelected += OnNewToolSelected;
			SelectToolOptionsController.NewSelectTool += OnNewSelectTool;
		}

		private void OnNewToolSelected(Tool tool)
		{
			UpdateVisibleTools();
		}

		private void OnNewSelectTool(SelectTool newTool)
		{
			UpdateVisibleTools();
		}
	}
}