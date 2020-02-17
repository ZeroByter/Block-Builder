using UnityEditor;
using UnityEngine;

namespace ZeroByterGames.BlockBuilder
{
	[CustomEditor(typeof(ChunkController))]
	public class ChunkControllerEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var controller = (ChunkController)target;

			GUILayout.Label($"Vertices: {controller.GetVerticesCount()}");
			GUILayout.Label($"Triangles: {controller.GetTrianglesCount()}");
			if(GUILayout.Button("Update Mesh"))
			{
				controller.UpdateMesh();
			}
		}
	}
}
