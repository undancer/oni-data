using System;

public class DebugOverlays : KScreen
{
	public static DebugOverlays instance { get; private set; }

	protected override void OnPrefabInit()
	{
		instance = this;
		KPopupMenu componentInChildren = GetComponentInChildren<KPopupMenu>();
		componentInChildren.SetOptions(new string[5] { "None", "Rooms", "Lighting", "Style", "Flow" });
		componentInChildren.OnSelect = (Action<string, int>)Delegate.Combine(componentInChildren.OnSelect, new Action<string, int>(OnSelect));
		base.gameObject.SetActive(value: false);
	}

	private void OnSelect(string str, int index)
	{
		switch (str)
		{
		case "None":
			SimDebugView.Instance.SetMode(OverlayModes.None.ID);
			break;
		case "Flow":
			SimDebugView.Instance.SetMode(SimDebugView.OverlayModes.Flow);
			break;
		case "Lighting":
			SimDebugView.Instance.SetMode(OverlayModes.Light.ID);
			break;
		case "Rooms":
			SimDebugView.Instance.SetMode(OverlayModes.Rooms.ID);
			break;
		default:
			Debug.LogError("Unknown debug view: " + str);
			break;
		}
	}
}
