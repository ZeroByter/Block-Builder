using System;
using TMPro;
using UnityEngine;

namespace ZeroByterGames.BlockBuilder.UI
{
	public class WorkspaceSizeManager : MonoBehaviour
	{
		public static Vector3Int GetSize()
		{
			if (Singleton == null) return Vector3Int.zero;

			return Singleton.size;
		}

		public static Action<Vector3Int> NewSizeSet;

		private static WorkspaceSizeManager Singleton;

		private TMP_InputField inputField;

		private Vector3Int size = new Vector3Int(16, 16, 16);

		private void Awake()
		{
			Singleton = this;

			inputField = GetComponent<TMP_InputField>();
		}

		private void Start()
		{
			NewSize("16 16 16");
		}

		private void ResetInputField()
		{
			inputField.text = $"{size.x} {size.y} {size.z}";
		}

		public void NewSize(string str)
		{
			string[] stringData = str.Split(' ');

			if(stringData.Length == 3)
			{
				int[] data = new int[3];
				for (int i = 0; i < 3; i++)
				{
					int number;
					if(int.TryParse(stringData[i], out number))
					{
						data[i] = number;
					}
					else
					{
						ResetInputField();
						return;
					}
				}
				
				//succesfully got new size data
				size.x = data[0];
				size.y = data[1];
				size.z = data[2];

				NewSizeSet?.Invoke(new Vector3Int(size.x, size.y, size.z));
			}
			else
			{
				ResetInputField();
			}
		}
	}
}
