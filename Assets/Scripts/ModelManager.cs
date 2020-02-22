using System.Collections.Generic;
using UnityEngine;
using ZeroByterGames.BlockBuilder.UI;
using static ZeroByterGames.BlockBuilder.ChunkController;

namespace ZeroByterGames.BlockBuilder
{
	public class ModelManager : MonoBehaviour
	{
		private static ModelManager Singleton;

        public static void AddCube(int x, int y, int z)
        {
            if (Singleton == null) return;
            if (Singleton.chunk == null) Singleton.chunk = Singleton.CreateChunk();

            Singleton.chunk.AddCube(x - Singleton.chunk.position.x, y - Singleton.chunk.position.y, z - Singleton.chunk.position.z);
        }

        public static void AddCubes(List<Vector3Int> cubes)
        {
            if (Singleton.chunk == null) Singleton.chunk = Singleton.CreateChunk();
            
            foreach(var cube in cubes)
            {
                Singleton.chunk.AddCube(cube.x, cube.y, cube.z, false);
            }

            Singleton.chunk.UpdateMesh();
        }

        public static void RemoveCube(int x, int y, int z)
        {
            if (Singleton == null) return;

            int chunkX = Mathf.FloorToInt(x / 16f);
            int chunkY = Mathf.FloorToInt(y / 16f);
            int chunkZ = Mathf.FloorToInt(z / 16f);

            ChunkController chunk = Singleton.chunk;

            if (chunk == null) return;

            chunk.RemoveCube(Mathf.Abs(x - chunkX * 16), Mathf.Abs(y - chunkY * 16), Mathf.Abs(z - chunkZ * 16));
        }

        public static void RemoveCubes(List<Vector3Int> cubes)
        {
            if (Singleton.chunk == null) Singleton.chunk = Singleton.CreateChunk();

            foreach (var cube in cubes)
            {
                Singleton.chunk.RemoveCube(cube.x, cube.y, cube.z, false);
            }

            Singleton.chunk.UpdateMesh();
        }

        public static Cube.Side GetCube(int x, int y, int z, int side)
        {
            if (Singleton == null) return null;
            if (Singleton.chunk == null) return null;

            return Singleton.chunk.GetCubeSide(x, y, z, side);
        }

        public static void RemoveChunk()
        {
            if (Singleton == null) return;
            if (Singleton.chunk == null) return;

            Destroy(Singleton.chunk.gameObject);
            Singleton.chunk = null;
        }

        public static void UpdateChunk(int x, int y, int z)
        {
            if (Singleton == null) return;

            var chunk = Singleton.chunk;
            if (chunk != null) chunk.UpdateMesh();
        }

        private ChunkController chunk;

		private void Awake()
		{
			Singleton = this;
		}

		private ChunkController CreateChunk()
		{
			var chunkObject = new GameObject($"Chunk");
			var chunkTransform = chunkObject.transform;
			chunkTransform.position = (Vector3)WorkspaceSizeManager.GetSize() / -2f;
			chunkTransform.parent = transform;
			var chunk = chunkObject.AddComponent<ChunkController>();
			chunk.SetMaterial(new Material(Shader.Find("Diffuse")));
			return chunk;
		}
    }
}
