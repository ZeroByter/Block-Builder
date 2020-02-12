using System.Collections.Generic;
using UnityEngine;

namespace ZeroByterGames.BlockBuilder
{
	public class TestChunkController : MonoBehaviour
	{
		//https://github.com/roboleary/GreedyMesh/blob/master/src/mygame/Main.java

		class Face
		{
			public enum FaceSide
			{
				Forward = 0,
				Right = 1,
				Backwards = 2,
				Left = 3,
				Up = 4,
				Down = 5,
			}

			public bool transparent;
			public int color;
			public FaceSide side;

			public override bool Equals(object obj)
			{
				return obj is Face face &&
					   transparent == face.transparent &&
					   color == face.color;
			}

			public override int GetHashCode()
			{
				var hashCode = 1188517494;
				hashCode = hashCode * -1521134295 + color.GetHashCode();
				hashCode = hashCode * -1521134295 + side.GetHashCode();
				return hashCode;
			}
		}

		private Face[,,] faces = new Face[16, 16, 16];

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private MeshCollider meshCollider;

        private Mesh mesh;

        private List<Vector3> vertices = new List<Vector3>();
        private List<int> triangles = new List<int>();
        private List<Color> colors = new List<Color>();

		private void Awake()
		{
            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshCollider = gameObject.AddComponent<MeshCollider>();

            mesh = new Mesh();
            meshFilter.sharedMesh = mesh;
            meshRenderer.material = new Material(Shader.Find("Diffuse"));
            meshCollider.sharedMesh = mesh;

            Face face;

            for (int i = 0; i < 16; i++)
            {

                for (int j = 0; j < 16; j++)
                {

                    for (int k = 0; k < 16; k++)
                    {

                        if (i > 16 / 2 && i < 16 * 0.75 &&
                            j > 16 / 2 && j < 16 * 0.75 &&
                            k > 16 / 2 && k < 16 * 0.75)
                        {

                            /*
                             * We add a set of voxels of type 1 at the top-right of the chunk.
                             * 
                             */
                            face = new Face();
                            face.color = 1;

                            /*
                             * To see an example of face culling being used in combination with 
                             * greedy meshing, you could set the trasparent attribute to true.
                             */
                            //                        face.transparent = true;

                        }
                        else if (i == 0)
                        {

                            /*
                             * We add a set of voxels of type 2 on the left of the chunk. 
                             */
                            face = new Face();
                            face.color = 2;

                        }
                        else
                        {

                            /*
                             * And the rest are set to type 3.
                             */
                            face = new Face();
                            face.color = 3;
                        }

                        faces[i,j,k] = face;
                    }
                }
            }

            DoGreedyTest();
        }

        private void DoGreedyTest()
        {
            /*
         * These are just working variables for the algorithm - almost all taken 
         * directly from Mikola Lysenko's javascript implementation.
         */
            int i, j, k, l, w, h, u, v, n, side = 0;

            int[] x = new int[] { 0, 0, 0 };
            int[] q = new int[] { 0, 0, 0 };
            int[] du = new int[] { 0, 0, 0 };
            int[] dv = new int[] { 0, 0, 0 };

            /*
             * We create a mask - this will contain the groups of matching voxel faces 
             * as we proceed through the chunk in 6 directions - once for each face.
             */
            Face[] mask = new Face[16 * 16];

            /*
             * These are just working variables to hold two faces during comparison.
             */
            Face voxelFace, voxelFace1;

            /**
             * We start with the lesser-spotted bool for-loop (also known as the old flippy floppy). 
             * 
             * The variable backFace will be TRUE on the first iteration and FALSE on the second - this allows 
             * us to track which direction the indices should run during creation of the quad.
             * 
             * This loop runs twice, and the inner loop 3 times - totally 6 iterations - one for each 
             * voxel face.
             */
            for (bool backFace = true, b = false; b != backFace; backFace = backFace && b, b = !b)
            {

                /*
                 * We sweep over the 3 dimensions - most of what follows is well described by Mikola Lysenko 
                 * in his post - and is ported from his Javascript implementation.  Where this implementation 
                 * diverges, I've added commentary.
                 */
                for (int d = 0; d < 3; d++)
                {

                    u = (d + 1) % 3;
                    v = (d + 2) % 3;

                    x[0] = 0;
                    x[1] = 0;
                    x[2] = 0;

                    q[0] = 0;
                    q[1] = 0;
                    q[2] = 0;
                    q[d] = 1;

                    /*
                     * Here we're keeping track of the side that we're meshing.
                     */
                    if (d == 0) { side = backFace ? (int)Face.FaceSide.Left : (int)Face.FaceSide.Right; }
                    else if (d == 1) { side = backFace ? (int)Face.FaceSide.Down : (int)Face.FaceSide.Up; }
                    else if (d == 2) { side = backFace ? (int)Face.FaceSide.Backwards : (int)Face.FaceSide.Forward; }

                    /*
                     * We move through the dimension from front to back
                     */
                    for (x[d] = -1; x[d] < 16;)
                    {

                        /*
                         * -------------------------------------------------------------------
                         *   We compute the mask
                         * -------------------------------------------------------------------
                         */
                        n = 0;

                        for (x[v] = 0; x[v] < 16; x[v]++)
                        {

                            for (x[u] = 0; x[u] < 16; x[u]++)
                            {

                                /*
                                 * Here we retrieve two voxel faces for comparison.
                                 */
                                voxelFace = (x[d] >= 0) ? getFace(x[0], x[1], x[2], side) : null;
                                voxelFace1 = (x[d] < 16 - 1) ? getFace(x[0] + q[0], x[1] + q[1], x[2] + q[2], side) : null;

                                /*
                                 * Note that we're using the Equals function in the voxel face class here, which lets the faces 
                                 * be compared based on any number of attributes.
                                 * 
                                 * Also, we choose the face to add to the mask depending on whether we're moving through on a backface or not.
                                 */
                                mask[n++] = ((voxelFace != null && voxelFace1 != null && voxelFace.Equals(voxelFace1)))
                                            ? null
                                            : backFace ? voxelFace1 : voxelFace;
                            }
                        }

                        x[d]++;

                        /*
                         * Now we generate the mesh for the mask
                         */
                        n = 0;

                        for (j = 0; j < 16; j++)
                        {

                            for (i = 0; i < 16;)
                            {

                                if (mask[n] != null)
                                {

                                    /*
                                     * We compute the width
                                     */
                                    for (w = 1; i + w < 16 && mask[n + w] != null && mask[n + w].Equals(mask[n]); w++) { }

                                    /*
                                     * Then we compute height
                                     */
                                    bool done = false;

                                    for (h = 1; j + h < 16; h++)
                                    {

                                        for (k = 0; k < w; k++)
                                        {

                                            if (mask[n + k + h * 16] == null || !mask[n + k + h * 16].Equals(mask[n])) { done = true; break; }
                                        }

                                        if (done) { break; }
                                    }

                                    /*
                                     * Here we check the "transparent" attribute in the Face class to ensure that we don't mesh 
                                     * any culled faces.
                                     */
                                    if (!mask[n].transparent)
                                    {
                                        /*
                                         * Add quad
                                         */
                                        x[u] = i;
                                        x[v] = j;

                                        du[0] = 0;
                                        du[1] = 0;
                                        du[2] = 0;
                                        du[u] = w;

                                        dv[0] = 0;
                                        dv[1] = 0;
                                        dv[2] = 0;
                                        dv[v] = h;

                                        /*
                                         * And here we call the quad function in order to render a merged quad in the scene.
                                         * 
                                         * We pass mask[n] to the function, which is an instance of the Face class containing 
                                         * all the attributes of the face - which allows for variables to be passed to shaders - for 
                                         * example lighting values used to create ambient occlusion.
                                         */
                                        quad(new Vector3(x[0], x[1], x[2]),
                                             new Vector3(x[0] + du[0], x[1] + du[1], x[2] + du[2]),
                                             new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]),
                                             new Vector3(x[0] + dv[0], x[1] + dv[1], x[2] + dv[2]),
                                             w,
                                             h,
                                             mask[n],
                                             backFace);
                                    }

                                    /*
                                     * We zero out the mask
                                     */
                                    for (l = 0; l < h; ++l)
                                    {

                                        for (k = 0; k < w; ++k) { mask[n + k + l * 16] = null; }
                                    }

                                    /*
                                     * And then finally increment the counters and continue
                                     */
                                    i += w;
                                    n += w;

                                }
                                else
                                {

                                    i++;
                                    n++;
                                }
                            }
                        }
                    }
                }
            }
        }

        Face getFace(int x, int y, int z, int side)
        {

            Face voxelFace = faces[x,y,z];

            voxelFace.side = (Face.FaceSide)side;

            return voxelFace;
        }

        void quad(Vector3 bottomLeft,
              Vector3 topLeft,
              Vector3 topRight,
              Vector3 bottomRight,
              int width,
              int height,
              Face voxel,
              bool backFace)
        {

            Vector3[] vertices = new Vector3[4];

            vertices[2] = topLeft * 16;
            vertices[3] = topRight * 16;
            vertices[0] = bottomLeft * 16;
            vertices[1] = bottomRight * 16;

            int[] indexes = backFace ? new int[] { 2, 0, 1, 1, 3, 2 } : new int[] { 2, 3, 1, 1, 0, 2 };

            float[] colorArray = new float[4 * 4];

            for (int i = 0; i < colorArray.Length; i += 4)
            {

                /*
                 * Here I set different colors for quads depending on the "type" attribute, just 
                 * so that the different groups of voxels can be clearly seen.
                 * 
                 */
                if (voxel.color == 1)
                {

                    colorArray[i] = 1.0f;
                    colorArray[i + 1] = 0.0f;
                    colorArray[i + 2] = 0.0f;
                    colorArray[i + 3] = 1.0f;

                }
                else if (voxel.color == 2)
                {

                    colorArray[i] = 0.0f;
                    colorArray[i + 1] = 1.0f;
                    colorArray[i + 2] = 0.0f;
                    colorArray[i + 3] = 1.0f;

                }
                else
                {

                    colorArray[i] = 0.0f;
                    colorArray[i + 1] = 0.0f;
                    colorArray[i + 2] = 1.0f;
                    colorArray[i + 3] = 1.0f;
                }
            }

            int count = this.vertices.Count;
            this.vertices.AddRange(vertices);
            foreach (var index in indexes)
            {
                triangles.Add(count + index);
            }
            if (voxel.color == 1)
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    colors.Add(Color.red);
                }
            }else if (voxel.color == 2)
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    colors.Add(Color.green);
                }
            } else
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    colors.Add(Color.blue);
                }
            }

            mesh.vertices = this.vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.colors = colors.ToArray();

            mesh.RecalculateNormals();

            /*Mesh mesh = new Mesh();

            mesh.setBuffer(Type.Position, 3, BufferUtils.createFloatBuffer(vertices));
            mesh.setBuffer(Type.Color, 4, colorArray);
            mesh.setBuffer(Type.Index, 3, BufferUtils.createIntBuffer(indexes));
            mesh.updateBound();

            Geometry geo = new Geometry("ColoredMesh", mesh);
            Material mat = new Material(assetManager, "Common/MatDefs/Misc/Unshaded.j3md");
            mat.setBoolean("VertexColor", true);

            mat.getAdditionalRenderState().setWireframe(true);

            geo.setMaterial(mat);

            rootNode.attachChild(geo);*/

            //TODO: Finish add all vertices, triangles, and color variables and check it out!
        }
    }
}
