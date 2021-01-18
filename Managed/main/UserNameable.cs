using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/UserNameable")]
public class UserNameable : KMonoBehaviour
{
	[Serialize]
	public string savedName = "";

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (string.IsNullOrEmpty(savedName))
		{
			SetName(base.gameObject.GetProperName());
		}
		else
		{
			SetName(savedName);
		}
	}

	public void SetName(string name)
	{
		KSelectable component = GetComponent<KSelectable>();
		base.name = name;
		if (component != null)
		{
			component.SetName(name);
		}
		base.gameObject.name = name;
		NameDisplayScreen.Instance.UpdateName(base.gameObject);
		savedName = name;
		Trigger(1102426921, name);
	}
}
