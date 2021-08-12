using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/LogicRibbonDisplayUI")]
public class LogicRibbonDisplayUI : KMonoBehaviour
{
	[SerializeField]
	private Image wire1;

	[SerializeField]
	private Image wire2;

	[SerializeField]
	private Image wire3;

	[SerializeField]
	private Image wire4;

	[SerializeField]
	private LogicModeUI uiAsset;

	private Color32 colourOn;

	private Color32 colourOff;

	private Color32 colourDisconnected = new Color(255f, 255f, 255f, 255f);

	private int bitDepth = 4;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		colourOn = GlobalAssets.Instance.colorSet.logicOn;
		colourOff = GlobalAssets.Instance.colorSet.logicOff;
		colourOn.a = (colourOff.a = byte.MaxValue);
		wire1.raycastTarget = false;
		wire2.raycastTarget = false;
		wire3.raycastTarget = false;
		wire4.raycastTarget = false;
	}

	public void SetContent(LogicCircuitNetwork network)
	{
		Color32 color = colourDisconnected;
		List<Color32> list = new List<Color32>();
		for (int i = 0; i < bitDepth; i++)
		{
			list.Add((network == null) ? color : (network.IsBitActive(i) ? colourOn : colourOff));
		}
		if (wire1.color != list[0])
		{
			wire1.color = list[0];
		}
		if (wire2.color != list[1])
		{
			wire2.color = list[1];
		}
		if (wire3.color != list[2])
		{
			wire3.color = list[2];
		}
		if (wire4.color != list[3])
		{
			wire4.color = list[3];
		}
	}
}
