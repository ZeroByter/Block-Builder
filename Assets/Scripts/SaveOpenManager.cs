using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ZeroByterGames.BlockBuilder
{
	public class SaveOpenManager : MonoBehaviour
	{
		[Serializable]
		public class SaveData
		{
			[Serializable]
			public class BlockData
			{
				public int x;
				public int y;
				public int z;

				public int color;

				public BlockData(int x, int y, int z, int color)
				{
					this.x = x;
					this.y = y;
					this.z = z;
					this.color = color;
				}
			}

			public List<BlockData> blocks = new List<BlockData>();

			public SaveData(List<BlockData> blocks)
			{
				this.blocks = blocks;
			}
		}

		public static void Save(string path)
		{
			FileStream fs = File.Create(path);// new FileStream(path, FileMode.Create);

			BinaryFormatter bf = new BinaryFormatter();
			bf.Serialize(fs, new SaveData(ModelManager.GetAllBlocks()));

			fs.Close();
		}

		public static void Open(string path)
		{
			if (!File.Exists(path)) return;

			openFilePath = path;
			SceneManager.LoadScene(0);
		}

		private static string openFilePath = "";

		private void Awake()
		{
			if (openFilePath == "") return;

			FileStream fs = File.OpenRead(openFilePath);
			openFilePath = "";

			BinaryFormatter bf = new BinaryFormatter();
			var saveData = (SaveData)bf.Deserialize(fs);

			fs.Close();

			foreach (var block in saveData.blocks)
			{
				ModelManager.AddCube(block.x, block.y, block.z, block.color);
			}
		}
	}
}