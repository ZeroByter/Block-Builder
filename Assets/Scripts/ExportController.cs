using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ZeroByterGames.BlockBuilder
{
	public static class ExportController
	{
		public static void Export(string path, Mesh mesh)
		{
			if (path.EndsWith(".obj"))
			{
				ExportObj(path, mesh);
			}
		}

		private static void ExportObj(string path, Mesh mesh)
		{
			Vector3 minVertex = Vector3.positiveInfinity;
			Vector3 maxVertex = Vector3.negativeInfinity;

			//we calculate the min and max vertices so that later on we can centerize the mesh
			foreach (var vertex in mesh.vertices)
			{
				if (vertex.x < minVertex.x) minVertex.x = vertex.x;
				if (vertex.y < minVertex.y) minVertex.y = vertex.y;
				if (vertex.z < minVertex.z) minVertex.z = vertex.z;
				if (vertex.x > maxVertex.x) maxVertex.x = vertex.x;
				if (vertex.y > maxVertex.y) maxVertex.y = vertex.y;
				if (vertex.z > maxVertex.z) maxVertex.z = vertex.z;
			}

			string data = "";

			data += $"mtllib ./{Path.GetFileNameWithoutExtension(path)}.mtl\n";
			data += $"o test\n";
			data += $"\n";
			data += $"g test\n";
			data += $"\n";

			List<Vector3> distinctVertices = new List<Vector3>();
			//a map which links the old vertice's positions to the new ones
			Dictionary<int, int> verticesMap = new Dictionary<int, int>();

			for (int i = 0; i < mesh.vertexCount; i++)
			{
				var vertex = mesh.vertices[i];

				int distinctIndex = distinctVertices.IndexOf(vertex);

				if (distinctIndex == -1)
				{
					verticesMap[i] = distinctVertices.Count;
					distinctVertices.Add(vertex);

					data += $"v {vertex.x - (minVertex.x + maxVertex.x) / 2} {vertex.y - (minVertex.y + maxVertex.y) / 2} {vertex.z - (minVertex.z + maxVertex.z) / 2}\n";
				}
				else
				{
					verticesMap[i] = distinctIndex;
				}
			}

			data += $"\n";

			List<Vector2> distinctUvs = new List<Vector2>();
			Dictionary<int, int> uvsMap = new Dictionary<int, int>();

			for (int i = 0; i < mesh.uv.Length; i++)
			{
				var uv = mesh.uv[i];

				int distinctIndex = distinctUvs.IndexOf(uv);

				if (distinctIndex == -1)
				{
					uvsMap[i] = distinctUvs.Count;
					distinctUvs.Add(uv);

					data += $"vt {uv.x} {uv.y}\n";
				}
				else
				{
					uvsMap[i] = distinctIndex;
				}
			}

			data += $"\n";

			List<Vector3> distinctNormals = new List<Vector3>();
			Dictionary<int, int> normalsMap = new Dictionary<int, int>();

			for (int i = 0; i < mesh.normals.Length; i++)
			{
				var normal = mesh.normals[i];

				int distinctIndex = distinctNormals.IndexOf(normal);

				if (distinctIndex == -1)
				{
					normalsMap[i] = distinctNormals.Count;
					distinctNormals.Add(normal);

					data += $"vn {normal.x} {normal.y} {normal.z}\n";
				}
				else
				{
					normalsMap[i] = distinctIndex;
				}
			}

			data += $"\n";
			data += $"g\n";
			data += $"usemtl DefaultColorPalette\n";
			for (int i = 0; i < mesh.triangles.Length / 3; i++)
			{
				data += string.Format("f {0}/{3}/{6} {1}/{4}/{7} {2}/{5}/{8}\n",
					verticesMap[mesh.triangles[i * 3]] + 1, verticesMap[mesh.triangles[i * 3 + 1]] + 1, verticesMap[mesh.triangles[i * 3 + 2]] + 1,
					uvsMap[mesh.triangles[i * 3]] + 1, uvsMap[mesh.triangles[i * 3 + 1]] + 1, uvsMap[mesh.triangles[i * 3 + 2]] + 1,
					normalsMap[mesh.triangles[i * 3]] + 1, normalsMap[mesh.triangles[i * 3 + 1]] + 1, normalsMap[mesh.triangles[i * 3 + 2]] + 1);
			}

			File.WriteAllText(path, data);
			File.WriteAllText(Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path) + ".mtl", GetMaterialData());
			File.WriteAllBytes(Path.GetDirectoryName(path) + "/DefaultColorPalette.png", ((Texture2D)ColorPaletteManager.GetPaletteTexture()).EncodeToJPG());
		}

		private static string GetMaterialData()
		{
			string data = "";

			data += $"newmtl DefaultColorPalette\n";
			data += $"map_Kd DefaultColorPalette.png\n";
			data += $"Kd 1 1 1\n";
			data += $"d 1\n";

			return data;
		}
	}
}