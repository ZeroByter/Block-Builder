using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZeroByterGames.BlockBuilder.UI;
using static ZeroByterGames.BlockBuilder.SaveOpenManager.SaveData;

namespace ZeroByterGames.BlockBuilder
{
	public class ModelManager : MonoBehaviour
	{
        private static ModelManager Singleton;

        public Material defaultMaterial;

        public static Mesh GetCompleteMesh()
        {
            if (Singleton == null) return null;

            var chunks = Singleton.chunks.Values.ToArray();
            var combines = new CombineInstance[chunks.Length];

            for (int i = 0; i < chunks.Length; i++)
            {
                ChunkController chunk = chunks[i];

                combines[i].mesh = chunk.GetMesh();
                combines[i].transform = chunk.transform.localToWorldMatrix;
            }

            var wholeMesh = new Mesh();

            wholeMesh.CombineMeshes(combines);

            wholeMesh.RecalculateNormals();
            wholeMesh.RecalculateTangents();
            wholeMesh.RecalculateBounds();

            return wholeMesh;
        }

        public static List<BlockData> GetAllBlocks()
        {
            if (Singleton == null) return new List<BlockData>();

            List<BlockData> blocks = new List<BlockData>();

            foreach(var chunk in Singleton.chunks.Values)
            {
                blocks.AddRange(chunk.GetAllBlocks());
            }
            
            return blocks;
        }

        /// <summary>
        /// Creates a new block using the current closestColor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
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

            var closestColor = ColorpickerController.GetClosestColor();
            chunk.AddCube(Mathf.Abs(x - chunkX * 16), Mathf.Abs(y - chunkY * 16), Mathf.Abs(z - chunkZ * 16), closestColor.x, closestColor.y);
        }

        /// <summary>
        /// Creates a new block with a specified color value index
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="color"></param>
        public static void AddCube(int x, int y, int z, int color)
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

            int paletteWidth = ColorPaletteManager.GetPaletteWidth();
            int colorX = color % paletteWidth;
            int colorY = Mathf.FloorToInt(color / paletteWidth);
            chunk.AddCube(Mathf.Abs(x - chunkX * 16), Mathf.Abs(y - chunkY * 16), Mathf.Abs(z - chunkZ * 16), colorX, colorY);
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

        public static int GetCubeColor(int x, int y, int z)
        {
            if (Singleton == null) return 0;

            int chunkX = Mathf.FloorToInt(x / 16f);
            int chunkY = Mathf.FloorToInt(y / 16f);
            int chunkZ = Mathf.FloorToInt(z / 16f);
            int chunkKey = Singleton.Vector3ToInt(chunkX, chunkY, chunkZ);

            ChunkController chunk;

            if (!Singleton.chunks.TryGetValue(chunkKey, out chunk))
            {
                return 0;
            }

            return chunk.GetCubeColor(Mathf.Abs(x - chunkX * 16), Mathf.Abs(y - chunkY * 16), Mathf.Abs(z - chunkZ * 16));
        }

        public static bool GetCube(int x, int y, int z)
        {
            if (Singleton == null) return false;

            int chunkX = Mathf.FloorToInt(x / 16f);
            int chunkY = Mathf.FloorToInt(y / 16f);
            int chunkZ = Mathf.FloorToInt(z / 16f);
            int chunkKey = Singleton.Vector3ToInt(chunkX, chunkY, chunkZ);

            ChunkController chunk;

            if (!Singleton.chunks.TryGetValue(chunkKey, out chunk))
            {
                return false;
            }

            return chunk.DoesCubeExist(Mathf.Abs(x - chunkX * 16), Mathf.Abs(y - chunkY * 16), Mathf.Abs(z - chunkZ * 16));
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
            chunk.SetMaterial(defaultMaterial);
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

        private int Vector3ToInt(int x, int y, int z)
        {
            return x * 16777216 + y * 4096 + z;
        }
	}
}

//https://forum.minetest.net/viewtopic.php?f=7&t=20337#p322489