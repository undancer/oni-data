using UnityEngine;
using UnityEngine.UI;

public class ClippyPanel : KScreen
{
	public Text title;

	public Text detailText;

	public Text flavorText;

	public Image topicIcon;

	private KButton okButton;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		SpeedControlScreen.Instance.Pause();
		Game.Instance.Trigger(1634669191);
	}

	public void OnOk()
	{
		SpeedControlScreen.Instance.Unpause();
		Object.Destroy(base.gameObject);
	}
}
