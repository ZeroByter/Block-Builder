using System;
using System.IO;
using UnityEngine;

namespace ZeroByterGames.BlockBuilder
{
	public static class ExportController
	{
		public static void ExportTest(Mesh mesh)
		{
			string data = "";

			data += $"o test\n";
			data += $"\n";
			data += $"g test\n";
			data += $"\n";
			foreach (var vertex in mesh.vertices)
			{
				data += $"v {vertex.x} {vertex.y} {vertex.z}\n";
			}
			data += $"\n";
			foreach (var uv in mesh.uv)
			{
				data += $"vt {uv.x} {uv.y} {uv}\n";
			}
			data += $"\n";
			foreach (var normal in mesh.normals)
			{
				data += $"vn {normal.x} {normal.y} {normal.z}\n";
			}
			data += $"\n";
			for (int i = 0; i < mesh.triangles.Length / 3; i++)
			{
				data += string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n", mesh.triangles[i * 3] + 1, mesh.triangles[i * 3 + 1] + 1, mesh.triangles[i * 3 + 2] + 1);
			}

			File.WriteAllText(@"C:\Users\User\Unity Projects\Block Builder Tool\Assets\test.obj", data);
		}
	}
}