public class RepairableEquipment : KMonoBehaviour
{
	public DefHandle defHandle;

	public EquipmentDef def
	{
		get
		{
			return defHandle.Get<EquipmentDef>();
		}
		set
		{
			defHandle.Set(value);
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (def.AdditionalTags != null)
		{
			Tag[] additionalTags = def.AdditionalTags;
			foreach (Tag tag in additionalTags)
			{
				GetComponent<KPrefabID>().AddTag(tag);
			}
		}
	}
}
