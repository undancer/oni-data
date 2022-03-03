using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ScheduleBlockPainter")]
public class ScheduleBlockPainter : KMonoBehaviour
{
	[SerializeField]
	private KButtonDrag button;

	private Action<float> blockPaintHandler;

	[MyCmpGet]
	private RectTransform rectTransform;

	public void Setup(Action<float> blockPaintHandler)
	{
		this.blockPaintHandler = blockPaintHandler;
		button.onPointerDown += OnPointerDown;
		button.onDrag += OnDrag;
	}

	private void OnPointerDown()
	{
		Transmit();
	}

	private void OnDrag()
	{
		Transmit();
	}

	private void Transmit()
	{
		float num = (base.transform.InverseTransformPoint(KInputManager.GetMousePos()).x - rectTransform.rect.x) / rectTransform.rect.width;
		blockPaintHandler(num);
	}
}
