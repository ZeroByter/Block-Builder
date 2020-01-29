using Facepunch;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZeroByterGames.BlockBuilder.TransformSystem;
using static ZeroByterGames.BlockBuilder.SaveOpenManager.SaveData;

namespace ZeroByterGames.BlockBuilder
{
	public class CubeSelectionController : MonoBehaviour
	{
		private static CubeSelectionController Singleton;

        public static bool DoesCubeExist(int x, int y, int z)
        {
            if (Singleton == null) return false;

            x -= (int)Singleton.transform.position.x;
            y -= (int)Singleton.transform.position.y;
            z -= (int)Singleton.transform.position.z;

            return Singleton.blocks.ContainsKey(Vector3ToInt(x, y, z));
        }

        public static void AddCube(int x, int y, int z, int color)
        {
            if (Singleton == null) return;

            Singleton.blocks.Add(Vector3ToInt(x, y, z), new BlockData(x, y, z, color));

            Singleton.UpdateMesh();
        }

        public static void RemoveCube(int x, int y, int z)
        {
            if (Singleton == null) return;

            Singleton.blocks.Remove(Vector3ToInt(x, y, z));

            Singleton.UpdateMesh();
        }

        public static BlockData[] GetAllCubes()
        {
            if (Singleton == null) return new BlockData[0];

            return Singleton.blocks.Values.ToArray();
        }

        public static Vector3 GetPosition()
        {
            if (Singleton == null) return Vector3.zero;

            return Singleton.transform.position;
        }

        public static void Clear()
        {
            if (Singleton == null) return;

            Singleton.blocks.Clear();

            Singleton.UpdateMesh();
        }
        
        public static void ResetPosition()
        {
            if (Singleton == null) return;

            Singleton.transform.position = Vector3.zero;

            /*if (Singleton == null) return;

            Dictionary<int, BlockData> blocks = new Dictionary<int, BlockData>();
            var controllerPosition = Singleton.transform.position;

            foreach (var cube in Singleton.blocks.Values)
            {
                int x = cube.x - (int)controllerPosition.x;
                int y = cube.y - (int)controllerPosition.y;
                int z = cube.z - (int)controllerPosition.z;

                blocks[Vector3ToInt(x, y, z)] = new BlockData(x, y, z, cube.color);
            }

            Singleton.blocks = blocks;*/
        }

        public static int Vector3ToInt(int x, int y, int z)
        {
            return x * 16777216 + y * 4096 + z;
        }

        public Material material;

		private MeshFilter meshFilter;
		private MeshRenderer meshRenderer;
        private MeshCollider meshCollider;

		private Mesh mesh;

		private Dictionary<int, BlockData> blocks = new Dictionary<int, BlockData>();

		private MeshManipulationUtilities meshManipulation = new MeshManipulationUtilities();

		private void Awake()
		{
			Singleton = this;

			meshFilter = gameObject.AddComponent<MeshFilter>();
			meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshCollider = gameObject.AddComponent<MeshCollider>();
            meshRenderer.material = material;

            mesh = new Mesh();

			meshFilter.sharedMesh = mesh;
            meshCollider.sharedMesh = mesh;

            StartCoroutine(AddToHighlight());
        }

        private void Update()
        {
            if(blocks.Count > 0)
            {
                TransformController.SetToolsPosition(meshCollider.bounds.center);
            }
        }

        private IEnumerator AddToHighlight()
        {
            yield return new WaitForEndOfFrame();

            Highlight.ClearAll();
            Highlight.AddRenderer(meshRenderer);
            Highlight.Rebuild(true);
        }

        public void UpdateMesh()
        {
            mesh.Clear();
            meshManipulation.Clear();

            var controllerPosition = transform.position;

            foreach (var blockData in blocks.Values)
            {
                int x = blockData.x - (int)controllerPosition.x;
                int y = blockData.y - (int)controllerPosition.y;
                int z = blockData.z - (int)controllerPosition.z;

                List<Vector3> faces = new List<Vector3>();

                if (!DoesCubeExist(x, y, z + 1)) faces.Add(Vector3.forward);
                if (!DoesCubeExist(x, y, z - 1)) faces.Add(Vector3.back);
                if (!DoesCubeExist(x + 1, y, z)) faces.Add(Vector3.right);
                if (!DoesCubeExist(x - 1, y, z)) faces.Add(Vector3.left);
                if (!DoesCubeExist(x, y + 1, z)) faces.Add(Vector3.up);
                if (!DoesCubeExist(x, y - 1, z)) faces.Add(Vector3.down);

                int paletteWidth = ColorPaletteManager.GetPaletteWidth();

                meshManipulation.AddCubeQuads(new Vector3(x, y, z), faces.ToArray(), blockData.color % paletteWidth, Mathf.FloorToInt(blockData.color / (float)paletteWidth) + 1);
            }

            mesh.vertices = meshManipulation.GetVerticesArray();
            mesh.SetTriangles(meshManipulation.GetTrianglesArray(), 0);
            mesh.uv = meshManipulation.GetUVsArray();

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            meshCollider.enabled = false;
            meshCollider.enabled = true;
        }
    }
}
