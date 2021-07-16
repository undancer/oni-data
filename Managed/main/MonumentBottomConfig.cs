using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class MonumentBottomConfig : IBuildingConfig
{
	public const string ID = "MonumentBottom";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("MonumentBottom", 5, 5, "victory_monument_base_kanim", 1000, 60f, new float[2]
		{
			7500f,
			2500f
		}, new string[2]
		{
			SimHashes.Steel.ToString(),
			SimHashes.Obsidian.ToString()
		}, 9999f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER2, decor: BUILDINGS.DECOR.BONUS.MONUMENT.INCOMPLETE);
		BuildingTemplates.CreateMonumentBuildingDef(obj);
		obj.SceneLayer = Grid.SceneLayer.BuildingFront;
		obj.OverheatTemperature = 2273.15f;
		obj.Floodable = false;
		obj.AttachmentSlotTag = "MonumentBottom";
		obj.ObjectLayer = ObjectLayer.Building;
		obj.PermittedRotations = PermittedRotations.FlipH;
		obj.attachablePosition = new CellOffset(0, 0);
		obj.RequiresPowerInput = false;
		obj.CanMove = false;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<LoopingSounds>();
		go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
		{
			new BuildingAttachPoint.HardPoint(new CellOffset(0, 5), "MonumentMiddle", null)
		};
		go.AddOrGet<MonumentPart>().part = MonumentPart.Part.Bottom;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<KBatchedAnimController>().initialAnim = "option_a";
		go.GetComponent<KPrefabID>().prefabSpawnFn += delegate(GameObject game_object)
		{
			MonumentPart monumentPart = game_object.AddOrGet<MonumentPart>();
			monumentPart.part = MonumentPart.Part.Bottom;
			monumentPart.selectableStatesAndSymbols = new List<Tuple<string, string>>();
			monumentPart.stateUISymbol = "base";
			monumentPart.selectableStatesAndSymbols.Add(new Tuple<string, string>("option_a", "straight_legs"));
			monumentPart.selectableStatesAndSymbols.Add(new Tuple<string, string>("option_b", "wide_stance"));
			monumentPart.selectableStatesAndSymbols.Add(new Tuple<string, string>("option_c", "hmmm_legs"));
			monumentPart.selectableStatesAndSymbols.Add(new Tuple<string, string>("option_d", "sitting_stool"));
			monumentPart.selectableStatesAndSymbols.Add(new Tuple<string, string>("option_e", "wide_stance2"));
			monumentPart.selectableStatesAndSymbols.Add(new Tuple<string, string>("option_f", "posing1"));
			monumentPart.selectableStatesAndSymbols.Add(new Tuple<string, string>("option_g", "knee_kick"));
			monumentPart.selectableStatesAndSymbols.Add(new Tuple<string, string>("option_h", "step_on_hatches"));
			monumentPart.selectableStatesAndSymbols.Add(new Tuple<string, string>("option_i", "sit_on_tools"));
			monumentPart.selectableStatesAndSymbols.Add(new Tuple<string, string>("option_j", "water_pacu"));
			monumentPart.selectableStatesAndSymbols.Add(new Tuple<string, string>("option_k", "sit_on_eggs"));
			if (DlcManager.IsExpansion1Active())
			{
				monumentPart.selectableStatesAndSymbols.Add(new Tuple<string, string>("option_l", "rocketnosecone"));
				monumentPart.selectableStatesAndSymbols.Add(new Tuple<string, string>("option_m", "rocketsugarengine"));
				monumentPart.selectableStatesAndSymbols.Add(new Tuple<string, string>("option_n", "rocketnCO2"));
				monumentPart.selectableStatesAndSymbols.Add(new Tuple<string, string>("option_o", "rocketpetro"));
				monumentPart.selectableStatesAndSymbols.Add(new Tuple<string, string>("option_p", "rocketnoseconesmall"));
				monumentPart.selectableStatesAndSymbols.Add(new Tuple<string, string>("option_q", "rocketradengine"));
				monumentPart.selectableStatesAndSymbols.Add(new Tuple<string, string>("option_r", "sweepyoff"));
				monumentPart.selectableStatesAndSymbols.Add(new Tuple<string, string>("option_s", "sweepypeek"));
				monumentPart.selectableStatesAndSymbols.Add(new Tuple<string, string>("option_t", "sweepy"));
			}
		};
	}
}
