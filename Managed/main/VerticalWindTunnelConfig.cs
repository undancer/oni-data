using TUNING;
using UnityEngine;

public class VerticalWindTunnelConfig : IBuildingConfig
{
	public const string ID = "VerticalWindTunnel";

	private const float DISPLACEMENT_AMOUNT = 3f;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("VerticalWindTunnel", 5, 6, "wind_tunnel_kanim", 30, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER6, MATERIALS.PLASTICS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.BONUS.TIER1);
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.Floodable = true;
		buildingDef.AudioCategory = "Metal";
		buildingDef.Overheatable = true;
		buildingDef.RequiresPowerInput = true;
		buildingDef.PowerInputOffset = new CellOffset(0, 0);
		buildingDef.EnergyConsumptionWhenActive = 1200f;
		buildingDef.SelfHeatKilowattsWhenActive = 2f;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RecBuilding);
		VerticalWindTunnel verticalWindTunnel = go.AddOrGet<VerticalWindTunnel>();
		verticalWindTunnel.specificEffect = "VerticalWindTunnel";
		verticalWindTunnel.trackingEffect = "RecentlyVerticalWindTunnel";
		verticalWindTunnel.basePriority = RELAXATION.PRIORITY.TIER4;
		verticalWindTunnel.displacementAmount_DescriptorOnly = 3f;
		ElementConsumer elementConsumer = go.AddComponent<ElementConsumer>();
		elementConsumer.configuration = ElementConsumer.Configuration.AllGas;
		elementConsumer.consumptionRate = 3f;
		elementConsumer.storeOnConsume = false;
		elementConsumer.showInStatusPanel = false;
		elementConsumer.consumptionRadius = 2;
		elementConsumer.sampleCellOffset = new Vector3(0f, -2f, 0f);
		elementConsumer.showDescriptor = false;
		elementConsumer = go.AddComponent<ElementConsumer>();
		elementConsumer.configuration = ElementConsumer.Configuration.AllGas;
		elementConsumer.consumptionRate = 3f;
		elementConsumer.storeOnConsume = false;
		elementConsumer.showInStatusPanel = false;
		elementConsumer.consumptionRadius = 2;
		elementConsumer.sampleCellOffset = new Vector3(0f, 6f, 0f);
		elementConsumer.showDescriptor = false;
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.RecRoom.Id;
		roomTracker.requirement = RoomTracker.Requirement.Recommended;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
