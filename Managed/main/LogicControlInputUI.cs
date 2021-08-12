using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/LogicRibbonDisplayUI")]
public class LogicControlInputUI : KMonoBehaviour
{
	[SerializeField]
	private Image icon;

	[SerializeField]
	private Image border;

	[SerializeField]
	private LogicModeUI uiAsset;

	private Color32 colourOn;

	private Color32 colourOff;

	private Color32 colourDisconnected;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		colourOn = GlobalAssets.Instance.colorSet.logicOn;
		colourOff = GlobalAssets.Instance.colorSet.logicOff;
		colourOn.a = (colourOff.a = byte.MaxValue);
		colourDisconnected = GlobalAssets.Instance.colorSet.logicDisconnected;
		icon.raycastTarget = false;
		border.raycastTarget = false;
	}

	public void SetContent(LogicCircuitNetwork network)
	{
		Color32 color = ((network == null) ? GlobalAssets.Instance.colorSet.logicDisconnected : (network.IsBitActive(0) ? colourOn : colourOff));
		icon.color = color;
	}
}
