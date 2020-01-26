using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroByterGames.BlockBuilder
{
	public class ColorPaletteManager : MonoBehaviour
	{
		private static ColorPaletteManager Singleton;

		public static HashSet<PaletteColor> GetPaletteColors()
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

		public Sprite currentPalette;

		private int paletteWidth;
		private int paletteHeight;

		private HashSet<PaletteColor> colors = new HashSet<PaletteColor>();

		private void Awake()
		{
			Singleton = this;

			var texture = currentPalette.texture;

			paletteWidth = texture.width;
			paletteHeight = texture.height;

			for (int y = 0; y < texture.height; y++)
			{
				for (int x = 0; x < texture.width; x++)
				{
					var color = texture.GetPixel(x, y);
					colors.Add(new PaletteColor(x, y, color.r, color.g, color.b));
				}
			}
		}
	}
}