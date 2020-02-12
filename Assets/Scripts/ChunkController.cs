using System.Collections.Generic;
using UnityEngine;

namespace ZeroByterGames.BlockBuilder
{
	public class ChunkController : MonoBehaviour
	{
		//https://github.com/roboleary/GreedyMesh/blob/master/src/mygame/Main.java

		private const int Forward = 0;
		private const int Right = 1;
		private const int Backward = 2;
		private const int Left = 3;
		private const int Up = 4;
		private const int Down = 5;

		public class Cube
		{
			public class Side
			{
				public Color color = Color.white;
				public bool Transparent {
					get {
						return color.a != 1;
					}
				}

				public override bool Equals(object obj)
				{
					return obj is Side side &&
						   color.Equals(side.color) &&
						   Transparent == side.Transparent;
				}

				public override int GetHashCode()
				{
					var hashCode = -511261207;
					hashCode = hashCode * -1521134295 + color.GetHashCode();
					hashCode = hashCode * -1521134295 + Transparent.GetHashCode();
					return hashCode;
				}
			}

			public Side[] sides = new Side[6];

			public Cube()
			{
				for (int i = 0; i < 6; i++)
				{
					sides[i] = new Side();
				}
			}
		}

		private MeshFilter meshFilter;
		private MeshRenderer meshRenderer;
		private MeshCollider meshCollider;

		private Mesh mesh;

		private List<Vector3> vertices = new List<Vector3>();
		private List<int> triangles = new List<int>();

		private Cube[,,] cubes = new Cube[16, 16, 16];

		private void Awake()
		{
			meshFilter = gameObject.AddComponent<MeshFilter>();
			meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshCollider = gameObject.AddComponent<MeshCollider>();

			mesh = new Mesh();
			meshFilter.sharedMesh = mesh;
			meshRenderer.material = new Material(Shader.Find("Diffuse"));
			meshCollider.sharedMesh = mesh;

			cubes[0, 2, 0] = new Cube();
			cubes[1, 2, 0] = new Cube();
			cubes[1, 1, 0] = new Cube();
			cubes[1, 0, 0] = new Cube();
			cubes[2, 2, 0] = new Cube();

			UpdateMesh();
		}

		private void UpdateMesh()
		{
			mesh.Clear();
			vertices.Clear();
			triangles.Clear();

			int z = 0;

			for (int y = 0; y < 16; y++)
			{
				for (int x = 0; x < 16; x++)
				{
					var side = GetCubeSide(x, y, z, Up);

					if (side != null)
					{
						var facingSide = GetFacingCubeSide(x, y, z, Up);

						if(facingSide == null)
						{
							int count = vertices.Count;

							vertices.Add(new Vector3(x, y + 1, z));
							vertices.Add(new Vector3(x + 1, y + 1, z));
							vertices.Add(new Vector3(x + 1, y + 1, z + 1));
							vertices.Add(new Vector3(x, y + 1, z + 1));

							//TODO: Implemented culling, now need to figure out how to implement greedy!

							//Idea:
							//Loop through X until cube is null
							//In each X we loop to, check if there if we can also expand 'up' (y + 1). If we can't in even just one X loop, then dont add 'height' to rectangle
							//If we can expand 'up', then do the same X loop thing but with Y++, if not, that's our final rectangle.

							triangles.Add(count + 2);
							triangles.Add(count + 1);
							triangles.Add(count + 0);
							triangles.Add(count + 3);
							triangles.Add(count + 2);
							triangles.Add(count + 0);
						}
					}
				}
			}

			mesh.vertices = vertices.ToArray();
			mesh.triangles = triangles.ToArray();

			mesh.RecalculateNormals();
		}

		private bool IsInChunk(int x, int y, int z)
		{
			if (x < 0 || x > 15) return false;
			if (y < 0 || y > 15) return false;
			if (z < 0 || z > 15) return false;

			return true;
		}

		private Cube.Side GetCubeSide(int x, int y, int z, int side)
		{
			if (!IsInChunk(x, y, z)) return null; //TODO: get cube in adjacent chunk instead of null
			if (side < 0 || side > 5) return null;

			var cube = cubes[x, y, z];
			if (cube == null) return null;

			return cubes[x, y, z].sides[side];
		}

		private Cube.Side GetFacingCubeSide(int x, int y, int z, int side)
		{
			if(side == Forward)
			{
				return GetCubeSide(x + 1, y, z, side);
			}
			else if(side == Right)
			{
				return GetCubeSide(x, y, z + 1, side);
			}
			else if(side == Backward)
			{
				return GetCubeSide(x - 1, y, z, side);
			}
			else if(side == Left)
			{
				return GetCubeSide(x, y, z - 1, side);
			}
			else if(side == Up)
			{
				return GetCubeSide(x, y + 1, z, side);
			}
			else
			{
				return GetCubeSide(x, y - 1, z, side);
			}
		}
	}
}
