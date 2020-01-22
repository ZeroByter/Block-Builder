using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace ZeroByterGames.BlockBuilder {
    public class ChunkController : MonoBehaviour
    {
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private MeshCollider meshCollider;

        private Mesh mesh;

        private List<Vector3> vertices = new List<Vector3>();
        private List<int> triangles = new List<int>();

        private bool[,,] modelData = new bool[16, 16, 16];
        public int cubesCount;

        private Vector3Int chunkPosition;

        private void Awake()
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshCollider = gameObject.AddComponent<MeshCollider>();

            meshRenderer.material = new Material(Shader.Find("Diffuse"));

            mesh = new Mesh();
            meshFilter.sharedMesh = mesh;
            meshCollider.sharedMesh = mesh;

            var pos = transform.position;
            chunkPosition = new Vector3Int((int)pos.x / 16, (int)pos.y / 16, (int)pos.z / 16);
        }

        public Mesh GetMesh()
        {
            return mesh;
        }

        public void UpdateMesh()
        {
            mesh.Clear();
            vertices.Clear();
            triangles.Clear();

            for (int x = 0; x < modelData.GetLength(0); x++)
            {
                for (int y = 0; y < modelData.GetLength(1); y++)
                {
                    for (int z = 0; z < modelData.GetLength(2); z++)
                    {
                        if (!GetCube(x, y, z)) continue;

                        List<Vector3> faces = new List<Vector3>();

                        if (!GetCube(x, y, z + 1)) faces.Add(Vector3.forward);
                        if (!GetCube(x, y, z - 1)) faces.Add(Vector3.back);
                        if (!GetCube(x + 1, y, z)) faces.Add(Vector3.right);
                        if (!GetCube(x - 1, y, z)) faces.Add(Vector3.left);
                        if (!GetCube(x, y + 1, z)) faces.Add(Vector3.up);
                        if (!GetCube(x, y - 1, z)) faces.Add(Vector3.down);

                        AddCubeQuads(new Vector3(x, y, z), faces.ToArray());
                    }
                }
            }

            mesh.vertices = vertices.ToArray();
            mesh.SetTriangles(triangles.ToArray(), 0);

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            meshCollider.enabled = false;
            meshCollider.enabled = true;
        }

        private void AddTriangle(int offset, int vertex1, int vertex2, int vertex3)
        {
            triangles.Add(offset + vertex1);
            triangles.Add(offset + vertex2);
            triangles.Add(offset + vertex3);
        }

        private void AddQuad(Vector3 origin, Vector3 normal)
        {
            int count = vertices.Count;

            if (normal == Vector3.forward)
            {
                vertices.Add(origin + new Vector3(0, 0, 0));
                vertices.Add(origin + new Vector3(1, 0, 0));
                vertices.Add(origin + new Vector3(1, 1, 0));
                vertices.Add(origin + new Vector3(0, 1, 0));

                AddTriangle(count, 2, 0, 1);
                AddTriangle(count, 2, 3, 0);
            }
            else if (normal == Vector3.back)
            {
                vertices.Add(origin + new Vector3(0, 0, 0));
                vertices.Add(origin + new Vector3(1, 0, 0));
                vertices.Add(origin + new Vector3(1, 1, 0));
                vertices.Add(origin + new Vector3(0, 1, 0));

                AddTriangle(count, 2, 1, 0);
                AddTriangle(count, 2, 0, 3);
            }
            else if (normal == Vector3.right)
            {
                vertices.Add(origin + new Vector3(0, 0, 0));
                vertices.Add(origin + new Vector3(0, 0, 1));
                vertices.Add(origin + new Vector3(0, 1, 1));
                vertices.Add(origin + new Vector3(0, 1, 0));

                AddTriangle(count, 3, 1, 0);
                AddTriangle(count, 2, 1, 3);
            }
            else if (normal == Vector3.left)
            {
                vertices.Add(origin + new Vector3(0, 0, 0));
                vertices.Add(origin + new Vector3(0, 0, 1));
                vertices.Add(origin + new Vector3(0, 1, 1));
                vertices.Add(origin + new Vector3(0, 1, 0));

                AddTriangle(count, 3, 0, 1);
                AddTriangle(count, 2, 3, 1);
            }
            else if (normal == Vector3.up)
            {
                vertices.Add(origin + new Vector3(0, 0, 0));
                vertices.Add(origin + new Vector3(1, 0, 0));
                vertices.Add(origin + new Vector3(1, 0, 1));
                vertices.Add(origin + new Vector3(0, 0, 1));

                AddTriangle(count, 0, 2, 1);
                AddTriangle(count, 0, 3, 2);
            }
            else if (normal == Vector3.down)
            {
                vertices.Add(origin + new Vector3(0, 0, 0));
                vertices.Add(origin + new Vector3(1, 0, 0));
                vertices.Add(origin + new Vector3(1, 0, 1));
                vertices.Add(origin + new Vector3(0, 0, 1));

                AddTriangle(count, 0, 1, 2);
                AddTriangle(count, 0, 2, 3);
            }
        }

        private void AddCubeQuads(Vector3 origin, Vector3[] faces)
        {
            foreach(var face in faces)
            {
                Vector3 offset = Vector3.zero;

                if (face == Vector3.up || face == Vector3.right || face == Vector3.forward) offset = face;

                AddQuad(origin + offset, face);
            }
        }

        public void AddCube(int x, int y, int z)
        {
            if (x < 0 || x >= modelData.GetLength(0) || y < 0 || y >= modelData.GetLength(1) || z < 0 || z >= modelData.GetLength(2)) return;

            modelData[x, y, z] = true;
            cubesCount++;

            UpdateAdjacentChunk(x, y, z);

            UpdateMesh();
        }

        public void RemoveCube(int x, int y, int z)
        {
            if (x < 0 || x >= modelData.GetLength(0) || y < 0 || y >= modelData.GetLength(1) || z < 0 || z >= modelData.GetLength(2)) return;

            modelData[x, y, z] = false;
            cubesCount--;

            UpdateAdjacentChunk(x, y, z);

            if(cubesCount <= 0)
            {
                ModelManager.RemoveChunk(chunkPosition.x, chunkPosition.y, chunkPosition.z);
                return;
            }

            UpdateMesh();
        }

        private void UpdateAdjacentChunk(int x, int y, int z)
        {
            //If we are on the edge of the chunk, update the nearby chunk
            if (x < 1) ModelManager.UpdateChunk(chunkPosition.x - 1, chunkPosition.y, chunkPosition.z);
            if (x >= modelData.GetLength(0) - 1) ModelManager.UpdateChunk(chunkPosition.x + 1, chunkPosition.y, chunkPosition.z);
            if (y < 1) ModelManager.UpdateChunk(chunkPosition.x, chunkPosition.y - 1, chunkPosition.z);
            if (y >= modelData.GetLength(1) - 1) ModelManager.UpdateChunk(chunkPosition.x, chunkPosition.y + 1, chunkPosition.z);
            if (z < 1) ModelManager.UpdateChunk(chunkPosition.x, chunkPosition.y, chunkPosition.z - 1);
            if (z >= modelData.GetLength(2) - 1) ModelManager.UpdateChunk(chunkPosition.x, chunkPosition.y, chunkPosition.z + 1);
        }

        public bool GetCube(int x, int y, int z)
        {
            if (x < 0 || y < 0 || z < 0) return ModelManager.GetCube(Mathf.FloorToInt(x + transform.position.x), Mathf.FloorToInt(y + transform.position.y), Mathf.FloorToInt(z + transform.position.z));
            if (x >= modelData.GetLength(0) || y >= modelData.GetLength(1) || z >= modelData.GetLength(2))
            {
                return ModelManager.GetCube(Mathf.FloorToInt(x + transform.position.x), Mathf.FloorToInt(y + transform.position.y), Mathf.FloorToInt(z + transform.position.z));
            }

            return modelData[x, y, z];
        }

        private void OnDrawGizmosSelected()
        {
            Debug.DrawLine(transform.position + new Vector3(0, 0, 0), transform.position + new Vector3(16, 0, 0));
            Debug.DrawLine(transform.position + new Vector3(0, 0, 16), transform.position + new Vector3(16, 0, 16));
            Debug.DrawLine(transform.position + new Vector3(0, 0, 0), transform.position + new Vector3(0, 0, 16));
            Debug.DrawLine(transform.position + new Vector3(16, 0, 0), transform.position + new Vector3(16, 0, 16));
            Debug.DrawLine(transform.position + new Vector3(0, 16, 0), transform.position + new Vector3(16, 16, 0));
            Debug.DrawLine(transform.position + new Vector3(0, 16, 16), transform.position + new Vector3(16, 16, 16));
            Debug.DrawLine(transform.position + new Vector3(0, 16, 0), transform.position + new Vector3(0, 16, 16));
            Debug.DrawLine(transform.position + new Vector3(16, 16, 0), transform.position + new Vector3(16, 16, 16));
            Debug.DrawLine(transform.position + new Vector3(0, 0, 0), transform.position + new Vector3(0, 16, 0));
            Debug.DrawLine(transform.position + new Vector3(16, 0, 0), transform.position + new Vector3(16, 16, 0));
            Debug.DrawLine(transform.position + new Vector3(0, 0, 16), transform.position + new Vector3(0, 16, 16));
            Debug.DrawLine(transform.position + new Vector3(16, 0, 16), transform.position + new Vector3(16, 16, 16));
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ChunkController))]
    public class ChunkControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var chunk = (ChunkController)target;

            if(GUILayout.Button("Update Mesh")) chunk.UpdateMesh();
        }
    }
#endif
}
