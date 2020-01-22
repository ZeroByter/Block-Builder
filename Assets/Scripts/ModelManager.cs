using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZeroByterGames.BlockBuilder
{
	public class ModelManager : MonoBehaviour
	{
        private static ModelManager Singleton;

        public static Mesh GetCompleteMesh()
        {
            if (Singleton == null) return null;

            var vertices = new List<Vector3>();
            var triangles = new List<int>();

            foreach(var chunk in Singleton.chunks.Values)
            {
                var mesh = chunk.GetMesh();

                vertices.AddRange(mesh.vertices);
                triangles.AddRange(mesh.triangles);
            }

            var wholeMesh = new Mesh();

            wholeMesh.vertices = vertices.ToArray();
            wholeMesh.triangles = triangles.ToArray();

            return wholeMesh;
        }

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

        public static bool GetCube(int x, int y, int z)
        {
            if (Singleton == null) return false;

            return Singleton._GetCube(x, y, z);
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
            if(chunk != null) chunk.UpdateMesh();
        }

        public static void UpdateChunkDelayed(int x, int y, int z)
        {
            if (Singleton == null) return;

            Singleton.StartCoroutine(Singleton.UpdateMeshCoroutine(x, y, z));
        }

        private Dictionary<int, ChunkController> chunks = new Dictionary<int, ChunkController>();

        private void Awake()
        {
            Singleton = this;
            
            AddCube(0, 0, 0);
        }

        private IEnumerator UpdateMeshCoroutine(int x, int y, int z)
        {
            yield return new WaitForSeconds(1);

            UpdateChunk(x, y, z);
        }

        private ChunkController CreateChunk(int x, int y, int z)
        {
            var newChunk = new GameObject($"Chunk({x},{y},{z})");
            var chunkTransform = newChunk.transform;
            chunkTransform.position = new Vector3(x * 16, y * 16, z * 16);
            chunkTransform.parent = transform;
            return newChunk.AddComponent<ChunkController>();
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

        private bool _GetCube(int x, int y, int z)
        {
            int chunkX = Mathf.FloorToInt(x / 16f);
            int chunkY = Mathf.FloorToInt(y / 16f);
            int chunkZ = Mathf.FloorToInt(z / 16f);
            int chunkKey = Vector3ToInt(chunkX, chunkY, chunkZ);

            ChunkController chunk;

            if (!chunks.TryGetValue(chunkKey, out chunk))
            {
                return false;
            }

            return chunk.GetCube(Mathf.Abs(x - chunkX * 16), Mathf.Abs(y - chunkY * 16), Mathf.Abs(z - chunkZ * 16));
        }

        private int Vector3ToInt(int x, int y, int z)
        {
            return x * 16777216 + y * 4096 + z;
        }
	}
}

//https://forum.minetest.net/viewtopic.php?f=7&t=20337#p322489