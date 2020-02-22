using System;
using System.Collections.Generic;
using UnityEngine;
using ZeroByterGames.BlockBuilder.UI;

namespace ZeroByterGames.BlockBuilder
{
    public class ChunkController : MonoBehaviour
    {
        private const int Forward = 0;
        private const int Right = 1;
        private const int Backward = 2;
        private const int Left = 3;
        private const int Up = 4;
        private const int Down = 5;

        public class Cube
        {
            public class Side
            {
                public Color color = Color.white;
                public bool Transparent
                {
                    get
                    {
                        return color.a != 1;
                    }
                }

                public override bool Equals(object obj)
                {
                    return obj is Side side &&
                           color.Equals(side.color) &&
                           Transparent == side.Transparent;
                }

                public override int GetHashCode()
                {
                    var hashCode = -511261207;
                    hashCode = hashCode * -1521134295 + color.GetHashCode();
                    hashCode = hashCode * -1521134295 + Transparent.GetHashCode();
                    return hashCode;
                }
            }

            public Side[] sides = new Side[6];

            public Cube()
            {
                for (int i = 0; i < 6; i++)
                {
                    sides[i] = new Side();
                }
            }
        }

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private MeshCollider meshCollider;

        private Mesh mesh;

        private List<Vector3> vertices = new List<Vector3>();
        private List<int> triangles = new List<int>();

        private Cube[,,] cubes = new Cube[16, 16, 16];
        /// <summary>
        /// Only temporarily used for mesh generation
        /// </summary>
        private bool[,] filledCubes = new bool[16, 16];

        private int cubesCount;

        [HideInInspector]
        public Vector3Int position;

        private void Awake()
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshCollider = gameObject.AddComponent<MeshCollider>();

            mesh = new Mesh();
            meshFilter.sharedMesh = mesh;
            meshRenderer.material = new Material(Shader.Find("Diffuse"));
            meshCollider.sharedMesh = mesh;

            UpdateMesh();

            var pos = transform.position;
            position = new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z);

            WorkspaceSizeManager.NewSizeSet += OnNewSizeSet;
        }

        private void Start()
        {
            var size = WorkspaceSizeManager.GetSize();
            cubes = new Cube[size.x, size.y, size.z];

            position = new Vector3Int(size.x, size.y, size.z);
        }

        private void OnDestroy()
        {
            WorkspaceSizeManager.NewSizeSet -= OnNewSizeSet;
        }

        private void OnNewSizeSet(Vector3Int newSize)
        {
            transform.position = (Vector3)newSize / -2f;
            position = new Vector3Int(newSize.x / -2, newSize.y / -2, newSize.z / -2);

            Cube[,,] newCubes = new Cube[newSize.x, newSize.y, newSize.z];

            for (int x = 0; x < newSize.x; x++)
            {
                for (int y = 0; y < newSize.y; y++)
                {
                    for (int z = 0; z < newSize.z; z++)
                    {
                        newCubes[x, y, z] = cubes[x, y, z];
                    }
                }
            }

            this.cubes = newCubes;

            UpdateMesh();
        }

        #region Mesh generation
        public void UpdateMesh()
        {
            mesh.Clear();
            vertices.Clear();
            triangles.Clear();

            for (int currentSide = 0; currentSide < 6; currentSide++)
            {
                filledCubes = new bool[cubes.GetLength(0), cubes.GetLength(1)];

                if (currentSide == Up || currentSide == Down)
                {
                    for (int y = 0; y < cubes.GetLength(1); y++)
                    {
                        for (int z = 0; z < cubes.GetLength(2); z++)
                        {
                            for (int x = 0; x < cubes.GetLength(0); x++)
                            {
                                var renderSide = GetCubeSide(x, y, z, currentSide);

                                if (renderSide != null)
                                {
                                    if (GetFacingCubeSide(x, y, z, currentSide) == null)
                                    {
                                        int count = vertices.Count;

                                        if (!filledCubes[x, z])
                                        {
                                            int[] biggestRectangle = GetBiggestRectangle(x, y, z, currentSide);

                                            if (currentSide == Up)
                                            {
                                                vertices.Add(new Vector3(x, y + 1, z));
                                                vertices.Add(new Vector3(x + biggestRectangle[0], y + 1, z));
                                                vertices.Add(new Vector3(x + biggestRectangle[0], y + 1, z + biggestRectangle[1]));
                                                vertices.Add(new Vector3(x, y + 1, z + biggestRectangle[1]));

                                                for (int filledX = x; filledX < x + biggestRectangle[0]; filledX++)
                                                {
                                                    for (int filledZ = z; filledZ < z + biggestRectangle[1]; filledZ++)
                                                    {
                                                        filledCubes[filledX, filledZ] = true;
                                                    }
                                                }
                                            }
                                            else //down
                                            {
                                                vertices.Add(new Vector3(x, y, z + biggestRectangle[1]));
                                                vertices.Add(new Vector3(x + biggestRectangle[0], y, z + biggestRectangle[1]));
                                                vertices.Add(new Vector3(x + biggestRectangle[0], y, z));
                                                vertices.Add(new Vector3(x, y, z));

                                                for (int filledX = x; filledX < x + biggestRectangle[0]; filledX++)
                                                {
                                                    for (int filledZ = z; filledZ < z + biggestRectangle[1]; filledZ++)
                                                    {
                                                        filledCubes[filledX, filledZ] = true;
                                                    }
                                                }
                                            }

                                            triangles.Add(count + 2);
                                            triangles.Add(count + 1);
                                            triangles.Add(count + 0);
                                            triangles.Add(count + 3);
                                            triangles.Add(count + 2);
                                            triangles.Add(count + 0);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (currentSide == Forward || currentSide == Backward)
                {
                    for (int z = 0; z < cubes.GetLength(2); z++)
                    {
                        for (int y = 0; y < cubes.GetLength(1); y++)
                        {
                            for (int x = 0; x < cubes.GetLength(0); x++)
                            {
                                var renderSide = GetCubeSide(x, y, z, currentSide);

                                if (renderSide != null)
                                {
                                    if (GetFacingCubeSide(x, y, z, currentSide) == null || true)
                                    {
                                        int count = vertices.Count;

                                        if (!filledCubes[y, x] || true)
                                        {
                                            int[] biggestRectangle = GetBiggestRectangle(x, y, z, currentSide);

                                            if (currentSide == Forward)
                                            {
                                                vertices.Add(new Vector3(x, y + biggestRectangle[1], z + 1));
                                                vertices.Add(new Vector3(x + biggestRectangle[0], y + biggestRectangle[1], z + 1));
                                                vertices.Add(new Vector3(x + biggestRectangle[0], y, z + 1));
                                                vertices.Add(new Vector3(x, y, z + 1));

                                                for (int filledX = x; filledX < x + biggestRectangle[0]; filledX++)
                                                {
                                                    for (int filledY = y; filledY < y + biggestRectangle[1]; filledY++)
                                                    {
                                                        filledCubes[filledY, filledX] = true;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                vertices.Add(new Vector3(x, y, z));
                                                vertices.Add(new Vector3(x + biggestRectangle[0], y, z));
                                                vertices.Add(new Vector3(x + biggestRectangle[0], y + biggestRectangle[1], z));
                                                vertices.Add(new Vector3(x, y + biggestRectangle[1], z));

                                                for (int filledX = x; filledX < x + biggestRectangle[0]; filledX++)
                                                {
                                                    for (int filledY = y; filledY < y + biggestRectangle[1]; filledY++)
                                                    {
                                                        filledCubes[filledY, filledX] = true;
                                                    }
                                                }
                                            }

                                            triangles.Add(count + 2);
                                            triangles.Add(count + 1);
                                            triangles.Add(count + 0);
                                            triangles.Add(count + 3);
                                            triangles.Add(count + 2);
                                            triangles.Add(count + 0);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else //left or right
                {
                    for (int x = 0; x < cubes.GetLength(0); x++)
                    {
                        for (int y = 0; y < cubes.GetLength(1); y++)
                        {
                            for (int z = 0; z < cubes.GetLength(2); z++)
                            {
                                var renderSide = GetCubeSide(x, y, z, currentSide);

                                if (renderSide != null)
                                {
                                    if (GetFacingCubeSide(x, y, z, currentSide) == null)
                                    {
                                        int count = vertices.Count;

                                        if (!filledCubes[y, z])
                                        {
                                            int[] biggestRectangle = GetBiggestRectangle(x, y, z, currentSide);

                                            if (currentSide == Right)
                                            {
                                                //Left and right vertex filling is a little weird...
                                                vertices.Add(new Vector3(x + 1, y + biggestRectangle[1], z + biggestRectangle[0]));
                                                vertices.Add(new Vector3(x + 1, y, z));
                                                vertices.Add(new Vector3(x + 1, y, z + biggestRectangle[0]));
                                                vertices.Add(new Vector3(x + 1, y + biggestRectangle[1], z));

                                                for (int filledZ = z; filledZ < z + biggestRectangle[0]; filledZ++)
                                                {
                                                    for (int filledY = y; filledY < y + biggestRectangle[1]; filledY++)
                                                    {
                                                        filledCubes[filledY, filledZ] = true;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                vertices.Add(new Vector3(x, y + biggestRectangle[1], z));
                                                vertices.Add(new Vector3(x, y, z + biggestRectangle[0]));
                                                vertices.Add(new Vector3(x, y, z));
                                                vertices.Add(new Vector3(x, y + biggestRectangle[1], z + biggestRectangle[0]));

                                                for (int filledZ = z; filledZ < z + biggestRectangle[0]; filledZ++)
                                                {
                                                    for (int filledY = y; filledY < y + biggestRectangle[1]; filledY++)
                                                    {
                                                        filledCubes[filledY, filledZ] = true;
                                                    }
                                                }
                                            }

                                            triangles.Add(count + 2);
                                            triangles.Add(count + 1);
                                            triangles.Add(count + 0);
                                            triangles.Add(count + 0);
                                            triangles.Add(count + 1);
                                            triangles.Add(count + 3);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            mesh.RecalculateNormals();
        }

        private int[] GetBiggestRectangle(int startX, int startY, int startZ, int side)
        {
            //return new int[] { 1, 1 };

            var array = new int[2];
            array[1]++;

            if (side == Up || side == Down)
            {
                for (int z = startZ; z < Mathf.Min(cubes.GetLength(2), startZ + cubes.GetLength(2)); z++)
                {
                    bool expandZ = true;
                    for (int x = startX; x < Mathf.Min(cubes.GetLength(0), startX + cubes.GetLength(0)); x++)
                    {
                        if (GetFacingCubeSide(x, startY, z + 1, side) != null) expandZ = false;
                        if (GetIsCubeFilled(x, z + 1)) expandZ = false;

                        if (!IsInChunk(x, startY, z + 1) || GetCubeSide(x, startY, z, side) == null || GetFacingCubeSide(x, startY, z, side) != null)
                        {
                            if (x == startX) expandZ = false;
                            break;
                        }
                        if (GetCubeSide(x, startY, z + 1, side) == null) expandZ = false;
                        if (filledCubes[x, z]) break;

                        if (z == startZ) array[0]++;
                    }

                    if (!expandZ) break;//if we don't have to expand z, there is no reason to keep looking furter
                    array[1]++;
                }
            }
            else if (side == Forward || side == Backward)
            {
                //return new int[] { 1, 1 };

                for (int y = startY; y < Mathf.Min(cubes.GetLength(1), startY + cubes.GetLength(1)); y++)
                {
                    bool expandY = true;
                    for (int x = startX; x < Mathf.Min(cubes.GetLength(0), startX + cubes.GetLength(0)); x++)
                    {
                        if (GetFacingCubeSide(x, y + 1, startZ, side) != null) expandY = false;
                        if (GetIsCubeFilled(y + 1, x)) expandY = false;

                        if (!IsInChunk(x, y + 1, startZ) || GetCubeSide(x, y, startZ, side) == null || GetFacingCubeSide(x, y, startZ, side) != null)
                        {
                            if (x == startX) expandY = false;
                            break;
                        }
                        if (GetCubeSide(x, y + 1, startZ, side) == null) expandY = false;
                        if (filledCubes[y, x]) break;

                        if (y == startY) array[0]++;
                    }

                    if (!expandY) break;//if we don't have to expand y, there is no reason to keep looking furter
                    array[1]++;
                }
            }
            else //left or right
            {
                for (int y = startY; y < Mathf.Min(cubes.GetLength(1), startY + cubes.GetLength(1)); y++)
                {
                    bool expandY = true;
                    for (int z = startZ; z < Mathf.Min(cubes.GetLength(2), startZ + cubes.GetLength(2)); z++)
                    {
                        if (GetFacingCubeSide(startX, y + 1, z, side) != null) expandY = false;
                        if (GetIsCubeFilled(y + 1, z)) expandY = false;

                        if (!IsInChunk(startX, y + 1, z) || GetCubeSide(startX, y, z, side) == null || GetFacingCubeSide(startX, y, z, side) != null)
                        {
                            if (z == startZ) expandY = false;
                            break;
                        }
                        if (GetCubeSide(startX, y + 1, z, side) == null) expandY = false;
                        if (filledCubes[y, z]) break;

                        if (y == startY) array[0]++;
                    }


                    if (!expandY) break;//if we don't have to expand y, there is no reason to keep looking furter
                    array[1]++;
                }
            }

            return array;
        }
        #endregion

        public void AddCube(int x, int y, int z, bool updateMesh = true)
        {
            if (!IsInChunk(x, y, z)) return;

            cubes[x, y, z] = new Cube();
            cubesCount++;

            if (updateMesh)
            {
                UpdateMesh();
            }
        }

        public void RemoveCube(int x, int y, int z, bool updateMesh = true)
        {
            if (!IsInChunk(x, y, z)) return;

            cubes[x, y, z] = null;
            cubesCount--;

            if (cubesCount <= 0)
            {
                ModelManager.RemoveChunk();
                return;
            }

            if(updateMesh) UpdateMesh();
        }

        private bool IsInChunk(int x, int y, int z)
        {
            /*if (x < cubes.GetLength(0) / -2 || x > cubes.GetLength(0) / 2 - 1) return false;
            if (y < cubes.GetLength(1) / -2 || y > cubes.GetLength(1) / 2 - 1) return false;
            if (z < cubes.GetLength(2) / -2 || z > cubes.GetLength(2) / 2 - 1) return false;*/
            if (x < 0 || x > cubes.GetLength(0) - 1) return false;
            if (y < 0 || y > cubes.GetLength(1) - 1) return false;
            if (z < 0 || z > cubes.GetLength(2) - 1) return false;

            return true;
        }

        public Cube.Side GetCubeSide(int x, int y, int z, int side)
        {
            if (!IsInChunk(x, y, z)) return null;// ModelManager.GetCube(Mathf.FloorToInt(x + transform.position.x), Mathf.FloorToInt(y + transform.position.y), Mathf.FloorToInt(z + transform.position.z), side);
            if (side < 0 || side > 5) return null;

            var cube = cubes[x, y, z];
            if (cube == null) return null;

            return cubes[x, y, z].sides[side];
        }

        private Cube.Side GetFacingCubeSide(int x, int y, int z, int side)
        {
            if (side == Forward)
            {
                return GetCubeSide(x, y, z + 1, side);
            }
            else if (side == Right)
            {
                return GetCubeSide(x + 1, y, z, side);
            }
            else if (side == Backward)
            {
                return GetCubeSide(x, y, z - 1, side);
            }
            else if (side == Left)
            {
                return GetCubeSide(x - 1, y, z, side);
            }
            else if (side == Up)
            {
                return GetCubeSide(x, y + 1, z, side);
            }
            else
            {
                return GetCubeSide(x, y - 1, z, side);
            }
        }

        private bool GetIsCubeFilled(int a, int b)
        {
            if (a < 0 || a > 15) return false;
            if (b < 0 || b > 15) return false;
            return filledCubes[a, b];
        }

        public void SetMaterial(Material material)
        {
            meshRenderer.material = material;
        }

        public int GetVerticesCount()
        {
            return vertices.Count;
        }

        public int GetTrianglesCount()
        {
            return triangles.Count;
        }

        private void OnDrawGizmosSelected()
        {
            Debug.DrawLine(transform.position + new Vector3(0, 0, 0), transform.position + new Vector3(cubes.GetLength(0), 0, 0));
            Debug.DrawLine(transform.position + new Vector3(0, 0, cubes.GetLength(2)), transform.position + new Vector3(cubes.GetLength(0), 0, cubes.GetLength(2)));
            Debug.DrawLine(transform.position + new Vector3(0, 0, 0), transform.position + new Vector3(0, 0, cubes.GetLength(2)));
            Debug.DrawLine(transform.position + new Vector3(cubes.GetLength(0), 0, 0), transform.position + new Vector3(cubes.GetLength(0), 0, cubes.GetLength(2)));
            Debug.DrawLine(transform.position + new Vector3(0, cubes.GetLength(1), 0), transform.position + new Vector3(cubes.GetLength(0), cubes.GetLength(1), 0));
            Debug.DrawLine(transform.position + new Vector3(0, cubes.GetLength(1), cubes.GetLength(2)), transform.position + new Vector3(cubes.GetLength(0), cubes.GetLength(1), cubes.GetLength(2)));
            Debug.DrawLine(transform.position + new Vector3(0, cubes.GetLength(1), 0), transform.position + new Vector3(0, cubes.GetLength(1), cubes.GetLength(2)));
            Debug.DrawLine(transform.position + new Vector3(cubes.GetLength(0), cubes.GetLength(1), 0), transform.position + new Vector3(cubes.GetLength(0), cubes.GetLength(1), cubes.GetLength(2)));
            Debug.DrawLine(transform.position + new Vector3(0, 0, 0), transform.position + new Vector3(0, cubes.GetLength(1), 0));
            Debug.DrawLine(transform.position + new Vector3(cubes.GetLength(0), 0, 0), transform.position + new Vector3(cubes.GetLength(0), cubes.GetLength(1), 0));
            Debug.DrawLine(transform.position + new Vector3(0, 0, cubes.GetLength(2)), transform.position + new Vector3(0, cubes.GetLength(1), cubes.GetLength(2)));
            Debug.DrawLine(transform.position + new Vector3(cubes.GetLength(0), 0, cubes.GetLength(2)), transform.position + new Vector3(cubes.GetLength(0), cubes.GetLength(1), cubes.GetLength(2)));
        }
    }
}
