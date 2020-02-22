using UnityEngine;
using UnityEngine.UI;

namespace ZeroByterGames.BlockBuilder.UI {
	public class ColorpickerController : MonoBehaviour
	{
		private static ColorpickerController Singleton;

		public static PaletteColor GetClosestColor()
		{
			if (Singleton == null) return null;

			return Singleton.closestColor;
		}

		public static void SetColor(int x, int y)
		{
			if (Singleton == null) return;

			Singleton.closestColor = ColorPaletteManager.GetColorFromPalette(x, y);
			Singleton.UpdateUI();
		}

		public Image colorPreview;

		private float currentRed = 1;
		private float currentGreen = 1;
		private float currentBlue = 1;

		private PaletteColor closestColor;

		private void Awake()
		{
			Singleton = this;
		}

		private void Start()
		{
			FindClosestColorAndUpdateUI();
		}

		public void ChangedRed(float newValue)
		{
			currentRed = newValue;

			FindClosestColorAndUpdateUI();
		}

		public void ChangedGreen(float newValue)
		{
			currentGreen = newValue;

			FindClosestColorAndUpdateUI();
		}

		public void ChangedBlue(float newValue)
		{
			currentBlue = newValue;

			FindClosestColorAndUpdateUI();
		}

		private void FindClosestColorAndUpdateUI()
		{
			closestColor = FindClosestColor();
			UpdateUI();
		}

		private void UpdateUI()
		{
			colorPreview.color = closestColor.GetColor();
		}

		private PaletteColor FindClosestColor()
		{
			var colors = ColorPaletteManager.GetPaletteColors();

			PaletteColor closestColor = null;
			float closestDistance = 1000;

			foreach(var color in colors)
			{
				if(closestColor == null)
				{
					closestColor = color;
					continue;
				}

				float distance = Mathf.Sqrt(Mathf.Pow(color.r - currentRed, 2) + Mathf.Pow(color.g - currentGreen, 2) + Mathf.Pow(color.b - currentBlue, 2));

				if(distance <= closestDistance)
				{
					closestColor = color;
					closestDistance = distance;
				}
			}

			return closestColor;
		}
	}
}