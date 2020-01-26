using UnityEngine;
using ZeroByterGames.BlockBuilder.UI;
using static ZeroByterGames.BlockBuilder.UI.ToolbarController;

namespace ZeroByterGames.BlockBuilder.TransformSystem
{
	public class TransformController : MonoBehaviour
	{
		private GameObject translateParent;
		private GameObject rotateParent;

		private void Awake()
		{
			translateParent = transform.GetChild(0).gameObject;
			rotateParent = transform.GetChild(1).gameObject;

			translateParent.SetActive(false);
			rotateParent.SetActive(false);

			ToolbarController.NewToolSelected += OnNewToolSelected;
		}

		private void OnNewToolSelected(Tool tool)
		{
			translateParent.SetActive(tool == Tool.Translate);
			rotateParent.SetActive(tool == Tool.Rotate);
		}
	}
}