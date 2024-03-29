using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/CharacterOverlay")]
public class CharacterOverlay : KMonoBehaviour
{
	public bool shouldShowName;

	private bool registered;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Register();
	}

	public void Register()
	{
		if (!registered)
		{
			registered = true;
			NameDisplayScreen.Instance.AddNewEntry(base.gameObject);
		}
	}
}
