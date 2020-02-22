using System.Collections.Generic;
using UnityEngine;

namespace ZeroByterGames.BlockBuilder.UndoSystem
{
	public class UndoManager : MonoBehaviour
	{
		private static UndoManager Singleton;

		public static void AddAction(UndoAction action)
		{
			if (Singleton == null) return;

			if (Singleton.redoIndex > 0)
			{
				//Singleton.actions.RemoveRange(0, Singleton.redoIndex);
				Singleton.actions.RemoveRange(Singleton.actions.Count - Singleton.redoIndex, Singleton.redoIndex);
				Singleton.redoIndex = 0;
			}

			Singleton.actions.Add(action);
		}

		private List<UndoAction> actions = new List<UndoAction>();
		private int redoIndex = 0;

		private void Awake()
		{
			Singleton = this;
		}

		private void Update()
		{
#if UNITY_EDITOR
			if(Input.GetKeyDown(KeyCode.Z))
#else
			if(Input.GetKeyDown(KeyCode.Z) && Input.GetKey(KeyCode.LeftControl))
#endif
			{
				if (actions.Count > 0)
				{
					redoIndex = Mathf.Min(actions.Count, redoIndex + 1);
					var action = actions[actions.Count - Mathf.Clamp(redoIndex, 1, actions.Count)];

					action.ReverseAction();

					//actions.RemoveAt(actions.Count - 1);
				}
			}
#if UNITY_EDITOR
			if (Input.GetKeyDown(KeyCode.Y))
#else
			if(Input.GetKeyDown(KeyCode.Y) && Input.GetKey(KeyCode.LeftControl))
#endif
			{
				if (actions.Count > 0)
				{
					var action = actions[actions.Count - Mathf.Clamp(redoIndex, 1, actions.Count)];

					action.RedoAction();

					redoIndex = Mathf.Max(0, redoIndex - 1);
					//actions.RemoveAt(actions.Count - 1);
				}
			}
		}
	}
}