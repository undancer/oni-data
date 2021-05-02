using System.Collections.Generic;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/InOrbitRequired")]
public class InOrbitRequired : KMonoBehaviour, IGameObjectEffectDescriptor
{
	[MyCmpReq]
	private Building building;

	[MyCmpReq]
	private Operational operational;

	public static readonly Operational.Flag inOrbitFlag = new Operational.Flag("in_orbit", Operational.Flag.Type.Requirement);

	private CraftModuleInterface craftModuleInterface;

	protected override void OnSpawn()
	{
		WorldContainer myWorld = this.GetMyWorld();
		craftModuleInterface = myWorld.GetComponent<CraftModuleInterface>();
		base.OnSpawn();
		bool newInOrbit = craftModuleInterface.HasTag(GameTags.RocketNotOnGround);
		UpdateFlag(newInOrbit);
		craftModuleInterface.Subscribe(-1582839653, OnTagsChanged);
	}

	protected override void OnCleanUp()
	{
		if (craftModuleInterface != null)
		{
			craftModuleInterface.Unsubscribe(-1582839653, OnTagsChanged);
		}
	}

	private void OnTagsChanged(object data)
	{
		TagChangedEventData tagChangedEventData = (TagChangedEventData)data;
		if (tagChangedEventData.tag == GameTags.RocketNotOnGround)
		{
			UpdateFlag(tagChangedEventData.added);
		}
	}

	private void UpdateFlag(bool newInOrbit)
	{
		operational.SetFlag(inOrbitFlag, newInOrbit);
		GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.InOrbitRequired, !newInOrbit, this);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		list.Add(new Descriptor(UI.BUILDINGEFFECTS.IN_ORBIT_REQUIRED, UI.BUILDINGEFFECTS.TOOLTIPS.IN_ORBIT_REQUIRED, Descriptor.DescriptorType.Requirement));
		return list;
	}
}
