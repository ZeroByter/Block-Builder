using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ZeroByterGames.BlockBuilder.UI
{
	public class ColorSliderController : MonoBehaviour
	{
		[Serializable]
		public class ColorValueChangedEvent : UnityEvent<float> { }

		public ColorValueChangedEvent onValueChanged;

		private TMP_InputField inputField;
		private Slider slider;

		private void Awake()
		{
			inputField = transform.GetComponentInChildren<TMP_InputField>();
			slider = transform.GetComponentInChildren<Slider>();

			inputField.onValueChanged.AddListener( delegate { OnInputFieldValueChanged(); } );
			slider.onValueChanged.AddListener( delegate { OnSliderValueChanged(); } );
		}

		private void OnInputFieldValueChanged()
		{
			float newValue;

			if(float.TryParse(inputField.text, out newValue))
			{
				slider.value = newValue / 255f;

				onValueChanged.Invoke(newValue);
			}
		}

		private void OnSliderValueChanged()
		{
			float newValue = slider.value;

			inputField.text = Mathf.Round(newValue * 255).ToString();

			onValueChanged.Invoke(newValue);
		}
	}
}