using UnityEngine;
using UnityEngine.UI;

namespace ZeroByterGames.BlockBuilder.UI
{
	public class ForceSliderMovement : MonoBehaviour
	{
		private Transform canvas;
		private Slider slider;
		private RectTransform rect;

		private void Awake()
		{
			slider = GetComponent<Slider>();
			rect = GetComponent<RectTransform>();

			canvas = GetComponentInParent<Canvas>().transform;
		}

		private void Update()
		{
			if (Input.GetKey(KeyCode.Mouse0))
			{
				var mouse = Input.mousePosition;

				float width = rect.sizeDelta.x * canvas.localScale.x;
				float height = rect.sizeDelta.y * canvas.localScale.x;
				float x = rect.position.x;
				float y = rect.position.y;

				if (mouse.x > x - width && mouse.x < x && mouse.y > y - height && mouse.y < y)
				{
					slider.value = Mathf.InverseLerp(x - width, x, mouse.x);
				}
			}
		}
	}
}
