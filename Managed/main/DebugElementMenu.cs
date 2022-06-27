using UnityEngine;

public class DebugElementMenu : KButtonMenu
{
	public static DebugElementMenu Instance;

	public GameObject root;

	protected override void OnPrefabInit()
	{
		Instance = this;
		base.OnPrefabInit();
		base.ConsumeMouseScroll = true;
	}

	protected override void OnForcedCleanUp()
	{
		Instance = null;
		base.OnForcedCleanUp();
	}

	public void Turnoff()
	{
		root.gameObject.SetActive(value: false);
	}
}
