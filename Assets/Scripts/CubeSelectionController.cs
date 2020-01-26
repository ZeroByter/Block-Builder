using Facepunch;
using System.Collections.Generic;
using UnityEngine;
using static ZeroByterGames.BlockBuilder.SaveOpenManager.SaveData;

namespace ZeroByterGames.BlockBuilder
{
	public class CubeSelectionController : MonoBehaviour
	{
		private static CubeSelectionController Singleton;

        public Material material;

		private MeshFilter meshFilter;
		private MeshRenderer meshRenderer;

		private Mesh mesh;

		private Dictionary<int, BlockData> blocks = new Dictionary<int, BlockData>();

		private MeshManipulationUtilities meshManipulation = new MeshManipulationUtilities();

		private void Awake()
		{
			Singleton = this;

			meshFilter = gameObject.AddComponent<MeshFilter>();
			meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = material;

            mesh = new Mesh();

			meshFilter.sharedMesh = mesh;
        }

        private void Start()
        {
            Highlight.ClearAll();
            Highlight.AddRenderer(meshRenderer);
            Highlight.Rebuild();

            AddCube(0, 0, 0, 0, 0);
            AddCube(1, 0, 0, 0, 0);
            AddCube(2, 0, 0, 0, 0);
            AddCube(4, 1, 0, 0, 0);
        }

        public void UpdateMesh()
        {
            mesh.Clear();
            meshManipulation.Clear();

            foreach(var blockData in blocks.Values)
            {
                int x = blockData.x;
                int y = blockData.y;
                int z = blockData.z;

                List<Vector3> faces = new List<Vector3>();

                if (!DoesCubeExist(x, y, z + 1)) faces.Add(Vector3.forward);
                if (!DoesCubeExist(x, y, z - 1)) faces.Add(Vector3.back);
                if (!DoesCubeExist(x + 1, y, z)) faces.Add(Vector3.right);
                if (!DoesCubeExist(x - 1, y, z)) faces.Add(Vector3.left);
                if (!DoesCubeExist(x, y + 1, z)) faces.Add(Vector3.up);
                if (!DoesCubeExist(x, y - 1, z)) faces.Add(Vector3.down);

                int paletteWidth = ColorPaletteManager.GetPaletteWidth();
                int uvY = Mathf.FloorToInt(blockData.color / (float)paletteWidth);

                meshManipulation.AddCubeQuads(new Vector3(x, y, z), faces.ToArray(), blockData.color % paletteWidth, uvY);
            }

            mesh.vertices = meshManipulation.GetVerticesArray();
            mesh.SetTriangles(meshManipulation.GetTrianglesArray(), 0);
            mesh.uv = meshManipulation.GetUVsArray();

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }

        private bool DoesCubeExist(int x, int y, int z)
        {
            return blocks.ContainsKey(Vector3ToInt(x, y, z));
        }

        private void AddCube(int x, int y, int z, int colorTileX, int colorTileY)
        {
            blocks.Add(Vector3ToInt(x, y, z), new BlockData(x, y, z, colorTileX + colorTileY * ColorPaletteManager.GetPaletteWidth()));

            UpdateMesh();
        }

        private int Vector3ToInt(int x, int y, int z)
        {
            return x * 16777216 + y * 4096 + z;
        }
    }
}
