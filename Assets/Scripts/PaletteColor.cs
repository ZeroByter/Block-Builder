using UnityEngine;

namespace ZeroByterGames.BlockBuilder
{
	public class PaletteColor
	{
		public int x;
		public int y;
		public float r;
		public float g;
		public float b;

		public PaletteColor(int x, int y, float r, float g, float b)
		{
			this.x = x;
			this.y = y;
			this.r = r;
			this.g = g;
			this.b = b;
		}

		public Color GetColor()
		{
			return new Color(r, g, b);
		}

		public override bool Equals(object obj)
		{
			return obj is PaletteColor color &&
				   x == color.x &&
				   y == color.y &&
				   r == color.r &&
				   g == color.g &&
				   b == color.b;
		}

		public override int GetHashCode()
		{
			var hashCode = 1715251579;
			hashCode = hashCode * -1521134295 + x.GetHashCode();
			hashCode = hashCode * -1521134295 + y.GetHashCode();
			hashCode = hashCode * -1521134295 + r.GetHashCode();
			hashCode = hashCode * -1521134295 + g.GetHashCode();
			hashCode = hashCode * -1521134295 + b.GetHashCode();
			return hashCode;
		}
	}
}