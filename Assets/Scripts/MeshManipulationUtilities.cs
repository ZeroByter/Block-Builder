using System.Collections.Generic;
using UnityEngine;

namespace ZeroByterGames.BlockBuilder
{
	/// <summary>
	/// Class has been created to avoid duplicate code...
	/// </summary>
	public class MeshManipulationUtilities
	{
        private List<int> triangles = new List<int>();
        private List<Vector3> vertices = new List<Vector3>();
        private List<Vector2> uvs = new List<Vector2>();

        public void Clear()
        {
            triangles.Clear();
            vertices.Clear();
            uvs.Clear();
        }

        public int[] GetTrianglesArray()
        {
            return triangles.ToArray();
        }

        public Vector3[] GetVerticesArray()
        {
            return vertices.ToArray();
        }

        public Vector2[] GetUVsArray()
        {
            return uvs.ToArray();
        }

        public void AddTriangle(int offset, int vertex1, int vertex2, int vertex3)
        {
            triangles.Add(offset + vertex1);
            triangles.Add(offset + vertex2);
            triangles.Add(offset + vertex3);
        }

        public void AddQuad(Vector3 origin, Vector3 normal, int uvTileX, int uvTileY)
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

            float materialTextureWidth = ColorPaletteManager.GetPaletteWidth();
            float materialTextureHeight = ColorPaletteManager.GetPaletteHeight();

            uvs.Add(new Vector2(uvTileX / materialTextureWidth, uvTileY / materialTextureHeight));
            uvs.Add(new Vector2(uvTileX / materialTextureWidth, uvTileY / materialTextureHeight));
            uvs.Add(new Vector2((uvTileX + 1) / materialTextureWidth, (uvTileY + 1) / materialTextureHeight));
            uvs.Add(new Vector2((uvTileX + 1) / materialTextureWidth, (uvTileY + 1) / materialTextureHeight));
        }

        public void AddCubeQuads(Vector3 origin, Vector3[] faces, int uvTileX, int uvTileY)
        {
            foreach (var face in faces)
            {
                Vector3 offset = Vector3.zero;

                if (face == Vector3.up || face == Vector3.right || face == Vector3.forward) offset = face;

                AddQuad(origin + offset, face, uvTileX, uvTileY);
            }
        }
    }
}
