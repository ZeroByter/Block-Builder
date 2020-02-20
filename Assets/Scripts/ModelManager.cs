using System.Collections.Generic;
using UnityEngine;
using static ZeroByterGames.BlockBuilder.ChunkController;

namespace ZeroByterGames.BlockBuilder
{
	public class ModelManager : MonoBehaviour
	{
		private static ModelManager Singleton;

        public static void AddCube(int x, int y, int z)
        {
            if (Singleton == null) return;

            int chunkX = Mathf.FloorToInt(x / 16f);
            int chunkY = Mathf.FloorToInt(y / 16f);
            int chunkZ = Mathf.FloorToInt(z / 16f);
            int chunkKey = Singleton.Vector3ToInt(chunkX, chunkY, chunkZ);

            ChunkController chunk = Singleton.GetChunkByBlock(x, y, z);

            if (chunk == null)
            {
                chunk = Singleton.CreateChunk(chunkX, chunkY, chunkZ);
                Singleton.chunks.Add(chunkKey, chunk);
            }

            chunk.AddCube(Mathf.Abs(x - chunkX * 16), Mathf.Abs(y - chunkY * 16), Mathf.Abs(z - chunkZ * 16));
        }

        public static void AddCubes(List<Vector3Int> cubes)
        {
            var chunks = new HashSet<ChunkController>();
            
            foreach(var cube in cubes)
            {
                int chunkX = Mathf.FloorToInt(cube.x / 16f);
                int chunkY = Mathf.FloorToInt(cube.y / 16f);
                int chunkZ = Mathf.FloorToInt(cube.z / 16f);
                int chunkKey = Singleton.Vector3ToInt(chunkX, chunkY, chunkZ);

                ChunkController chunk = Singleton.GetChunkByKey(chunkKey);

                if (chunk == null)
                {
                    chunk = Singleton.CreateChunk(chunkX, chunkY, chunkZ);
                    Singleton.chunks.Add(chunkKey, chunk);
                }

                chunks.Add(chunk);

                chunk.AddCube(Mathf.Abs(cube.x - chunkX * 16), Mathf.Abs(cube.y - chunkY * 16), Mathf.Abs(cube.z - chunkZ * 16), false);
            }

            foreach(var chunk in chunks)
            {
                chunk.UpdateMesh();
            }
        }

        public static void RemoveCube(int x, int y, int z)
        {
            if (Singleton == null) return;

            int chunkX = Mathf.FloorToInt(x / 16f);
            int chunkY = Mathf.FloorToInt(y / 16f);
            int chunkZ = Mathf.FloorToInt(z / 16f);

            ChunkController chunk = Singleton.GetChunkByBlock(x, y, z);

            if (chunk == null) return;

            chunk.RemoveCube(Mathf.Abs(x - chunkX * 16), Mathf.Abs(y - chunkY * 16), Mathf.Abs(z - chunkZ * 16));
        }

        public static void RemoveCubes(List<Vector3Int> cubes)
        {
            var chunks = new HashSet<ChunkController>();

            foreach (var cube in cubes)
            {
                int chunkX = Mathf.FloorToInt(cube.x / 16f);
                int chunkY = Mathf.FloorToInt(cube.y / 16f);
                int chunkZ = Mathf.FloorToInt(cube.z / 16f);
                int chunkKey = Singleton.Vector3ToInt(chunkX, chunkY, chunkZ);

                ChunkController chunk = Singleton.GetChunk(chunkX, chunkY, chunkZ);

                if (chunk == null)
                {
                    chunk = Singleton.CreateChunk(chunkX, chunkY, chunkZ);
                    Singleton.chunks.Add(chunkKey, chunk);
                }

                chunks.Add(chunk);

                chunk.RemoveCube(Mathf.Abs(cube.x - chunkX * 16), Mathf.Abs(cube.y - chunkY * 16), Mathf.Abs(cube.z - chunkZ * 16), false);
            }

            foreach (var chunk in chunks)
            {
                chunk.UpdateMesh();
            }
        }

        public static Cube.Side GetCube(int x, int y, int z, int side)
        {
            if (Singleton == null) return null;

            int chunkX = Mathf.FloorToInt(x / 16f);
            int chunkY = Mathf.FloorToInt(y / 16f);
            int chunkZ = Mathf.FloorToInt(z / 16f);
            int chunkKey = Singleton.Vector3ToInt(chunkX, chunkY, chunkZ);

            ChunkController chunk;

            if (!Singleton.chunks.TryGetValue(chunkKey, out chunk))
            {
                return null;
            }

            return chunk.GetCubeSide(Mathf.Abs(x - chunkX * 16), Mathf.Abs(y - chunkY * 16), Mathf.Abs(z - chunkZ * 16), side);
        }

        public static void RemoveChunk(int x, int y, int z)
        {
            if (Singleton == null) return;

            ChunkController chunk = Singleton.GetChunk(x, y, z);

            if (chunk == null) return;

            Destroy(chunk.gameObject);
            Singleton.chunks.Remove(Singleton.Vector3ToInt(x, y, z));
        }

        public static void UpdateChunk(int x, int y, int z)
        {
            if (Singleton == null) return;

            var chunk = Singleton.GetChunk(x, y, z);
            if (chunk != null) chunk.UpdateMesh();
        }

        private Dictionary<int, ChunkController> chunks = new Dictionary<int, ChunkController>();

		private void Awake()
		{
			Singleton = this;
		}

		private ChunkController CreateChunk(int x, int y, int z)
		{
			var chunkObject = new GameObject($"Chunk({x},{y},{z})");
			var chunkTransform = chunkObject.transform;
			chunkTransform.position = new Vector3(x * 16, y * 16, z * 16);
			chunkTransform.parent = transform;
			var chunk = chunkObject.AddComponent<ChunkController>();
			chunk.SetMaterial(new Material(Shader.Find("Diffuse")));
			return chunk;
		}

        private ChunkController GetChunkByBlock(int x, int y, int z)
        {
            int chunkX = Mathf.FloorToInt(x / 16f);
            int chunkY = Mathf.FloorToInt(y / 16f);
            int chunkZ = Mathf.FloorToInt(z / 16f);
            int chunkKey = Vector3ToInt(chunkX, chunkY, chunkZ);

            ChunkController chunk;

            if (!chunks.TryGetValue(chunkKey, out chunk))
            {
                return null;
            }

            return chunk;
        }

        private ChunkController GetChunk(int x, int y, int z)
        {
            int chunkKey = Vector3ToInt(x, y, z);

            ChunkController chunk;

            if (!chunks.TryGetValue(chunkKey, out chunk))
            {
                return null;
            }

            return chunk;
        }

        private ChunkController GetChunkByKey(int key)
        {
            ChunkController chunk;

            if (!chunks.TryGetValue(key, out chunk))
            {
                return null;
            }

            return chunk;
        }

        private int Vector3ToInt(int x, int y, int z)
        {
            return x * 16777216 + y * 4096 + z;
        }
    }
}
