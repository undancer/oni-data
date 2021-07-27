using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public abstract class SubstanceSource : KMonoBehaviour
{
	private bool enableRefresh;

	private static readonly float MaxPickupTime = 8f;

	[MyCmpReq]
	public Pickupable pickupable;

	[MyCmpReq]
	private PrimaryElement primaryElement;

	protected override void OnPrefabInit()
	{
		pickupable.SetWorkTime(MaxPickupTime);
	}

	protected override void OnSpawn()
	{
		pickupable.SetWorkTime(10f);
	}

	protected abstract CellOffset[] GetOffsetGroup();

	protected abstract IChunkManager GetChunkManager();

	public SimHashes GetElementID()
	{
		return primaryElement.ElementID;
	}

	public Tag GetElementTag()
	{
		Tag invalid = Tag.Invalid;
		if (base.gameObject != null && primaryElement != null && primaryElement.Element != null)
		{
			invalid = primaryElement.Element.tag;
		}
		return invalid;
	}

	public Tag GetMaterialCategoryTag()
	{
		Tag result = Tag.Invalid;
		if (base.gameObject != null && primaryElement != null && primaryElement.Element != null)
		{
			result = primaryElement.Element.GetMaterialCategoryTag();
		}
		return result;
	}
}
