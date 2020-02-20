using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace ZeroByterGames.BlockBuilder
{
	public class CameraController : MonoBehaviour
	{
		private int i;

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Mouse0))
			{
				/*ModelManager.AddCube(i++, 0, 0);
				ModelManager.AddCube(i++, 0, 0);
				ModelManager.AddCube(i++, 0, 0);*/

				int width = 16;
				int height = 32;
				int length = 64;

				var watch = new Stopwatch();
				watch.Start();
				var list = new List<Vector3Int>();
				list.Capacity = width * height * length;
				for (int y = 0; y < height; y++)
				{
					for (int x = 0; x < width; x++)
					{
						for (int z = 0; z < length; z++)
						{
							list.Add(new Vector3Int(x, y, z));
						}
					}
				}
				ModelManager.AddCubes(list);
				watch.Stop();
				print($"took {watch.ElapsedMilliseconds}ms to create mesh");
			}
		}
	}
}
