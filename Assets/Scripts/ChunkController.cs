using System.Collections.Generic;
using UnityEngine;

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

        public bool doGreedy;
        public bool doCulling;

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

        private Vector3Int chunkPosition;

        private void Awake()
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshCollider = gameObject.AddComponent<MeshCollider>();

            mesh = new Mesh();
            meshFilter.sharedMesh = mesh;
            meshRenderer.material = new Material(Shader.Find("Diffuse"));
            meshCollider.sharedMesh = mesh;

            var pos = transform.position;
            chunkPosition = new Vector3Int((int)pos.x / 16, (int)pos.y / 16, (int)pos.z / 16);

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
                filledCubes = new bool[16, 16];

                if (currentSide == Up || currentSide == Down)
                {
                    for (int y = 0; y < 16; y++)
                    {
                        for (int z = 0; z < 16; z++)
                        {
                            for (int x = 0; x < 16; x++)
                            {
                                var renderSide = GetCubeSide(x, y, z, currentSide);

                                if (renderSide != null)
                                {
                                    if (GetFacingCubeSide(x, y, z, currentSide) == null || !doCulling)
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
                    for (int z = 0; z < 16; z++)
                    {
                        for (int y = 0; y < 16; y++)
                        {
                            for (int x = 0; x < 16; x++)
                            {
                                var renderSide = GetCubeSide(x, y, z, currentSide);

                                if (renderSide != null)
                                {
                                    if (GetFacingCubeSide(x, y, z, currentSide) == null || !doCulling)
                                    {
                                        int count = vertices.Count;

                                        if (!filledCubes[y, x])
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
                    for (int x = 0; x < 16; x++)
                    {
                        for (int y = 0; y < 16; y++)
                        {
                            for (int z = 0; z < 16; z++)
                            {
                                var renderSide = GetCubeSide(x, y, z, currentSide);

                                if (renderSide != null)
                                {
                                    if (GetFacingCubeSide(x, y, z, currentSide) == null || !doCulling)
                                    {
                                        int count = vertices.Count;

                                        if (!filledCubes[y, z])
                                        {
                                            int[] biggestRectangle = GetBiggestRectangle(x, y, z, currentSide);

                                            if (currentSide == Right)
                                            {
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
            var array = new int[2];
            array[1]++;

            if (side == Up || side == Down)
            {
                for (int z = startZ; z < 16 - startZ; z++)
                {
                    bool expandZ = true;
                    for (int x = startX; x < 16 - startX; x++)
                    {
                        if (GetFacingCubeSide(x, startY, z + 1, side) != null) expandZ = false;
                        if (GetIsCubeFilled(x, z + 1)) expandZ = false;

                        if (GetCubeSide(x, startY, z, side) == null || GetFacingCubeSide(x, startY, z, side) != null)
                        {
                            if (x == startX) expandZ = false;
                            break;
                        }
                        if (GetCubeSide(x, startY, z + 1, side) == null) expandZ = false;
                        if (filledCubes[x, z]) break;

                        if (z == startZ) array[0]++;
                    }

                    if (expandZ)
                    {
                        array[1]++;
                    }
                    else
                    {
                        break; //if we don't have to expand z, there is no reason to keep looking furter
                    }
                }
            }
            else if (side == Forward || side == Backward)
            {
                for (int y = startY; y < 16 - startY; y++)
                {
                    bool expandY = true;
                    for (int x = startX; x < 16 - startX; x++)
                    {
                        if (GetFacingCubeSide(x + 1, y, startZ, side) != null) expandY = false;
                        if (GetIsCubeFilled(y, x + 1)) expandY = false;

                        if (GetCubeSide(x, y, startZ, side) == null || GetFacingCubeSide(x, y, startZ, side) != null)
                        {
                            if (x == startX) expandY = false;
                            break;
                        }
                        if (GetCubeSide(x + 1, y, startZ, side) == null) expandY = false;
                        if (filledCubes[y, x]) break;

                        if (y == startY) array[0]++;
                    }

                    if (expandY)
                    {
                        array[1]++;
                    }
                    else
                    {
                        break; //if we don't have to expand z, there is no reason to keep looking furter
                    }
                }
            }
            else //left or right
            {
                for (int y = startY; y < 16 - startY; y++)
                {
                    bool expandY = true;
                    for (int z = startZ; z < 16 - startZ; z++)
                    {
                        if (GetFacingCubeSide(startX, y + 1, z, side) != null) expandY = false;
                        if (GetIsCubeFilled(y + 1, z)) expandY = false;

                        if (GetCubeSide(startX, y, z, side) == null || GetFacingCubeSide(startX, y, z, side) != null)
                        {
                            if (z == startZ) expandY = false;
                            break;
                        }
                        if (GetCubeSide(startX, y + 1, z, side) == null) expandY = false;
                        if (filledCubes[y, z]) break;

                        if (y == startY) array[0]++;
                    }

                    if (expandY)
                    {
                        array[1]++;
                    }
                    else
                    {
                        break; //if we don't have to expand z, there is no reason to keep looking furter
                    }
                }
            }

            return array;
        }
        #endregion

        public void AddCube(int x, int y, int z)
        {
            if (x < 0 || x >= cubes.GetLength(0) || y < 0 || y >= cubes.GetLength(1) || z < 0 || z >= cubes.GetLength(2)) return;

            cubes[x, y, z] = new Cube();
            cubesCount++;

            UpdateAdjacentChunk(x, y, z);

            UpdateMesh();
        }

        public void RemoveCube(int x, int y, int z)
        {
            if (x < 0 || x >= cubes.GetLength(0) || y < 0 || y >= cubes.GetLength(1) || z < 0 || z >= cubes.GetLength(2)) return;

            cubes[x, y, z] = null;
            cubesCount--;

            UpdateAdjacentChunk(x, y, z);

            if (cubesCount <= 0)
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
            if (x >= cubes.GetLength(0) - 1) ModelManager.UpdateChunk(chunkPosition.x + 1, chunkPosition.y, chunkPosition.z);
            if (y < 1) ModelManager.UpdateChunk(chunkPosition.x, chunkPosition.y - 1, chunkPosition.z);
            if (y >= cubes.GetLength(1) - 1) ModelManager.UpdateChunk(chunkPosition.x, chunkPosition.y + 1, chunkPosition.z);
            if (z < 1) ModelManager.UpdateChunk(chunkPosition.x, chunkPosition.y, chunkPosition.z - 1);
            if (z >= cubes.GetLength(2) - 1) ModelManager.UpdateChunk(chunkPosition.x, chunkPosition.y, chunkPosition.z + 1);
        }

        private bool IsInChunk(int x, int y, int z)
        {
            if (x < 0 || x > 15) return false;
            if (y < 0 || y > 15) return false;
            if (z < 0 || z > 15) return false;

            return true;
        }

        public Cube.Side GetCubeSide(int x, int y, int z, int side)
        {
            if (!IsInChunk(x, y, z)) return ModelManager.GetCube(Mathf.FloorToInt(x + transform.position.x), Mathf.FloorToInt(x + transform.position.x), Mathf.FloorToInt(x + transform.position.x), side); //TODO: get cube in adjacent chunk instead of null
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
    }
}
