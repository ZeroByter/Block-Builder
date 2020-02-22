using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroByterGames.BlockBuilder
{
	public class ColorPaletteManager : MonoBehaviour
	{
		private static ColorPaletteManager Singleton;

		public static PaletteColor[] GetPaletteColors()
		{
			if (Singleton == null) return null;

			return Singleton.colors;
		}

		public static int GetPaletteWidth()
		{
			if (Singleton == null) return 0;

			return Singleton.paletteWidth;
		}

		public static int GetPaletteHeight()
		{
			if (Singleton == null) return 0;

			return Singleton.paletteHeight;
		}

		public static Texture GetPaletteTexture()
		{
			if (Singleton == null) return null;

			return Singleton.currentPalette.texture;
		}

		public static PaletteColor GetColorFromPalette(int x, int y)
		{
			if (Singleton == null) return null;

			return Singleton.colors[x + y * Singleton.paletteWidth];
		}

		public Sprite currentPalette;

		private int paletteWidth;
		private int paletteHeight;

		private PaletteColor[] colors;

		private void Awake()
		{
			Singleton = this;

			var texture = currentPalette.texture;

			paletteWidth = texture.width;
			paletteHeight = texture.height;

			HashSet<PaletteColor> uniqueColors = new HashSet<PaletteColor>();

			for (int y = 0; y < texture.height; y++)
			{
				for (int x = 0; x < texture.width; x++)
				{
					var color = texture.GetPixel(x, y);
					uniqueColors.Add(new PaletteColor(x, y, color.r, color.g, color.b));
				}
			}

			colors = uniqueColors.ToArray();
		}
	}
}