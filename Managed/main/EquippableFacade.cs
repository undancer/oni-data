using KSerialization;

public class EquippableFacade : KMonoBehaviour
{
	[Serialize]
	private string _facadeID;

	[Serialize]
	public string BuildOverride;

	public string FacadeID
	{
		get
		{
			return _facadeID;
		}
		private set
		{
			_facadeID = value;
			OverrideName();
		}
	}

	public static void AddFacadeToEquippable(Equippable equippable, string facadeID)
	{
		EquippableFacade equippableFacade = equippable.gameObject.AddOrGet<EquippableFacade>();
		equippableFacade.FacadeID = facadeID;
		equippableFacade.BuildOverride = Db.Get().EquippableFacades.Get(facadeID).BuildOverride;
		equippableFacade.ApplyAnimOverride();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		OverrideName();
		ApplyAnimOverride();
	}

	public void ApplyAnimOverride()
	{
		if (!FacadeID.IsNullOrWhiteSpace())
		{
			GetComponent<KBatchedAnimController>().SwapAnims(new KAnimFile[1] { Db.Get().EquippableFacades.Get(FacadeID).AnimFile });
		}
	}

	private void OverrideName()
	{
		GetComponent<KSelectable>().SetName(GetNameOverride(GetComponent<Equippable>().def.Id, FacadeID));
	}

	public static string GetNameOverride(string defID, string facadeID)
	{
		if (facadeID.IsNullOrWhiteSpace())
		{
			return Strings.Get("STRINGS.EQUIPMENT.PREFABS." + defID.ToUpper() + ".NAME");
		}
		return Strings.Get("STRINGS.EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES." + facadeID.ToUpper());
	}
}
