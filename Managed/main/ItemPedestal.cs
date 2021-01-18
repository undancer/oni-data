using Klei.AI;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ItemPedestal")]
public class ItemPedestal : KMonoBehaviour
{
	[MyCmpReq]
	private SingleEntityReceptacle receptacle;

	[MyCmpReq]
	private DecorProvider decorProvider;

	private const float MINIMUM_DECOR = 5f;

	private const float STORED_DECOR_MODIFIER = 2f;

	private const int RADIUS_BONUS = 2;

	private AttributeModifier decorModifier;

	private AttributeModifier decorRadiusModifier;

	private static readonly EventSystem.IntraObjectHandler<ItemPedestal> OnOccupantChangedDelegate = new EventSystem.IntraObjectHandler<ItemPedestal>(delegate(ItemPedestal component, object data)
	{
		component.OnOccupantChanged(data);
	});

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(-731304873, OnOccupantChangedDelegate);
		if ((bool)receptacle.Occupant)
		{
			KBatchedAnimController component = receptacle.Occupant.GetComponent<KBatchedAnimController>();
			if ((bool)component)
			{
				component.enabled = true;
				component.sceneLayer = Grid.SceneLayer.Move;
			}
			OnOccupantChanged(receptacle.Occupant);
		}
	}

	private void OnOccupantChanged(object data)
	{
		Attributes attributes = this.GetAttributes();
		if (decorModifier != null)
		{
			attributes.Remove(decorModifier);
			attributes.Remove(decorRadiusModifier);
			decorModifier = null;
			decorRadiusModifier = null;
		}
		if (data != null)
		{
			GameObject gameObject = (GameObject)data;
			DecorProvider component = gameObject.GetComponent<DecorProvider>();
			float value = 5f;
			float value2 = 3f;
			if (component != null)
			{
				value = Mathf.Max(Db.Get().BuildingAttributes.Decor.Lookup(gameObject).GetTotalValue() * 2f, 5f);
				value2 = Db.Get().BuildingAttributes.DecorRadius.Lookup(gameObject).GetTotalValue() + 2f;
			}
			string description = string.Format(BUILDINGS.PREFABS.ITEMPEDESTAL.DISPLAYED_ITEM_FMT, gameObject.GetComponent<KPrefabID>().PrefabTag.ProperName());
			decorModifier = new AttributeModifier(Db.Get().BuildingAttributes.Decor.Id, value, description);
			decorRadiusModifier = new AttributeModifier(Db.Get().BuildingAttributes.DecorRadius.Id, value2, description);
			attributes.Add(decorModifier);
			attributes.Add(decorRadiusModifier);
		}
	}
}
