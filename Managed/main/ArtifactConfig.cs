using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class ArtifactConfig : IMultiEntityConfig
{
	public delegate void PostInitFn(GameObject gameObject);

	public static Dictionary<ArtifactType, List<string>> artifactItems = new Dictionary<ArtifactType, List<string>>();

	public List<GameObject> CreatePrefabs()
	{
		List<GameObject> list = new List<GameObject>();
		artifactItems.Add(ArtifactType.Terrestrial, new List<string>());
		artifactItems.Add(ArtifactType.Space, new List<string>());
		artifactItems.Add(ArtifactType.Any, new List<string>());
		list.Add(CreateArtifact("Sandstone", UI.SPACEARTIFACTS.SANDSTONE.NAME, UI.SPACEARTIFACTS.SANDSTONE.DESCRIPTION, "idle_layered_rock", "ui_layered_rock", DECOR.SPACEARTIFACT.TIER0, DlcManager.AVAILABLE_ALL_VERSIONS));
		list.Add(CreateArtifact("Sink", UI.SPACEARTIFACTS.SINK.NAME, UI.SPACEARTIFACTS.SINK.DESCRIPTION, "idle_kitchen_sink", "ui_sink", DECOR.SPACEARTIFACT.TIER0, DlcManager.AVAILABLE_ALL_VERSIONS, "artifacts_kanim", null, SimHashes.Creature, ArtifactType.Terrestrial));
		list.Add(CreateArtifact("RubiksCube", UI.SPACEARTIFACTS.RUBIKSCUBE.NAME, UI.SPACEARTIFACTS.RUBIKSCUBE.DESCRIPTION, "idle_rubiks_cube", "ui_rubiks_cube", DECOR.SPACEARTIFACT.TIER0, DlcManager.AVAILABLE_ALL_VERSIONS, "artifacts_kanim", null, SimHashes.Creature, ArtifactType.Terrestrial));
		list.Add(CreateArtifact("OfficeMug", UI.SPACEARTIFACTS.OFFICEMUG.NAME, UI.SPACEARTIFACTS.OFFICEMUG.DESCRIPTION, "idle_coffee_mug", "ui_coffee_mug", DECOR.SPACEARTIFACT.TIER0, DlcManager.AVAILABLE_ALL_VERSIONS));
		list.Add(CreateArtifact("Obelisk", UI.SPACEARTIFACTS.OBELISK.NAME, UI.SPACEARTIFACTS.OBELISK.DESCRIPTION, "idle_tallstone", "ui_tallstone", DECOR.SPACEARTIFACT.TIER1, DlcManager.AVAILABLE_ALL_VERSIONS, "artifacts_kanim", null, SimHashes.Creature, ArtifactType.Space));
		list.Add(CreateArtifact("OkayXray", UI.SPACEARTIFACTS.OKAYXRAY.NAME, UI.SPACEARTIFACTS.OKAYXRAY.DESCRIPTION, "idle_xray", "ui_xray", DECOR.SPACEARTIFACT.TIER1, DlcManager.AVAILABLE_ALL_VERSIONS, "artifacts_kanim", null, SimHashes.Creature, ArtifactType.Terrestrial));
		list.Add(CreateArtifact("Blender", UI.SPACEARTIFACTS.BLENDER.NAME, UI.SPACEARTIFACTS.BLENDER.DESCRIPTION, "idle_blender", "ui_blender", DECOR.SPACEARTIFACT.TIER1, DlcManager.AVAILABLE_ALL_VERSIONS, "artifacts_kanim", null, SimHashes.Creature, ArtifactType.Terrestrial));
		list.Add(CreateArtifact("Moldavite", UI.SPACEARTIFACTS.MOLDAVITE.NAME, UI.SPACEARTIFACTS.MOLDAVITE.DESCRIPTION, "idle_moldavite", "ui_moldavite", DECOR.SPACEARTIFACT.TIER1, DlcManager.AVAILABLE_ALL_VERSIONS, "artifacts_kanim", null, SimHashes.Creature, ArtifactType.Space));
		list.Add(CreateArtifact("VHS", UI.SPACEARTIFACTS.VHS.NAME, UI.SPACEARTIFACTS.VHS.DESCRIPTION, "idle_vhs", "ui_vhs", DECOR.SPACEARTIFACT.TIER1, DlcManager.AVAILABLE_ALL_VERSIONS, "artifacts_kanim", null, SimHashes.Creature, ArtifactType.Terrestrial));
		list.Add(CreateArtifact("Saxophone", UI.SPACEARTIFACTS.SAXOPHONE.NAME, UI.SPACEARTIFACTS.SAXOPHONE.DESCRIPTION, "idle_saxophone", "ui_saxophone", DECOR.SPACEARTIFACT.TIER1, DlcManager.AVAILABLE_ALL_VERSIONS, "artifacts_kanim", null, SimHashes.Creature, ArtifactType.Space));
		list.Add(CreateArtifact("ModernArt", UI.SPACEARTIFACTS.MODERNART.NAME, UI.SPACEARTIFACTS.MODERNART.DESCRIPTION, "idle_abstract_blocks", "ui_abstract_blocks", DECOR.SPACEARTIFACT.TIER1, DlcManager.AVAILABLE_ALL_VERSIONS));
		list.Add(CreateArtifact("HoneyJar", UI.SPACEARTIFACTS.HONEYJAR.NAME, UI.SPACEARTIFACTS.HONEYJAR.DESCRIPTION, "idle_honey_jar", "ui_honey_jar", DECOR.SPACEARTIFACT.TIER1, DlcManager.AVAILABLE_EXPANSION1_ONLY, "artifacts_2_kanim", null, SimHashes.Creature, ArtifactType.Terrestrial));
		list.Add(CreateArtifact("AmeliasWatch", UI.SPACEARTIFACTS.AMELIASWATCH.NAME, UI.SPACEARTIFACTS.AMELIASWATCH.DESCRIPTION, "idle_earnhart_watch", "ui_earnhart_watch", DECOR.SPACEARTIFACT.TIER2, DlcManager.AVAILABLE_ALL_VERSIONS, "artifacts_kanim", null, SimHashes.Creature, ArtifactType.Space));
		list.Add(CreateArtifact("TeaPot", UI.SPACEARTIFACTS.TEAPOT.NAME, UI.SPACEARTIFACTS.TEAPOT.DESCRIPTION, "idle_teapot", "ui_teapot", DECOR.SPACEARTIFACT.TIER2, DlcManager.AVAILABLE_ALL_VERSIONS, "artifacts_kanim", null, SimHashes.Creature, ArtifactType.Space));
		list.Add(CreateArtifact("BrickPhone", UI.SPACEARTIFACTS.BRICKPHONE.NAME, UI.SPACEARTIFACTS.BRICKPHONE.DESCRIPTION, "idle_brick_phone", "ui_brick_phone", DECOR.SPACEARTIFACT.TIER2, DlcManager.AVAILABLE_ALL_VERSIONS, "artifacts_kanim", null, SimHashes.Creature, ArtifactType.Terrestrial));
		list.Add(CreateArtifact("RobotArm", UI.SPACEARTIFACTS.ROBOTARM.NAME, UI.SPACEARTIFACTS.ROBOTARM.DESCRIPTION, "idle_robot_arm", "ui_robot_arm", DECOR.SPACEARTIFACT.TIER2, DlcManager.AVAILABLE_ALL_VERSIONS, "artifacts_kanim", null, SimHashes.Creature, ArtifactType.Space));
		list.Add(CreateArtifact("ShieldGenerator", UI.SPACEARTIFACTS.SHIELDGENERATOR.NAME, UI.SPACEARTIFACTS.SHIELDGENERATOR.DESCRIPTION, "idle_hologram_generator_loop", "ui_hologram_generator", DECOR.SPACEARTIFACT.TIER2, DlcManager.AVAILABLE_ALL_VERSIONS, "artifacts_kanim", delegate(GameObject go)
		{
			go.AddOrGet<LoopingSounds>();
		}));
		list.Add(CreateArtifact("BioluminescentRock", UI.SPACEARTIFACTS.BIOLUMINESCENTROCK.NAME, UI.SPACEARTIFACTS.BIOLUMINESCENTROCK.DESCRIPTION, "idle_bioluminescent_rock", "ui_bioluminescent_rock", DECOR.SPACEARTIFACT.TIER2, DlcManager.AVAILABLE_ALL_VERSIONS, "artifacts_kanim", delegate(GameObject go)
		{
			Light2D light2D3 = go.AddOrGet<Light2D>();
			light2D3.overlayColour = LIGHT2D.BIOLUMROCK_COLOR;
			light2D3.Color = LIGHT2D.BIOLUMROCK_COLOR;
			light2D3.Range = 2f;
			light2D3.Angle = 0f;
			light2D3.Direction = LIGHT2D.BIOLUMROCK_DIRECTION;
			light2D3.Offset = LIGHT2D.BIOLUMROCK_OFFSET;
			light2D3.shape = LightShape.Cone;
			light2D3.drawOverlay = true;
		}, SimHashes.Creature, ArtifactType.Space));
		list.Add(CreateArtifact("GrubStatue", UI.SPACEARTIFACTS.GRUBSTATUE.NAME, UI.SPACEARTIFACTS.GRUBSTATUE.DESCRIPTION, "idle_grub_statue", "ui_grub_statue", DECOR.SPACEARTIFACT.TIER2, DlcManager.AVAILABLE_EXPANSION1_ONLY, "artifacts_2_kanim"));
		list.Add(CreateArtifact("Stethoscope", UI.SPACEARTIFACTS.STETHOSCOPE.NAME, UI.SPACEARTIFACTS.STETHOSCOPE.DESCRIPTION, "idle_stethocope", "ui_stethoscope", DECOR.SPACEARTIFACT.TIER3, DlcManager.AVAILABLE_ALL_VERSIONS, "artifacts_kanim", null, SimHashes.Creature, ArtifactType.Terrestrial));
		list.Add(CreateArtifact("EggRock", UI.SPACEARTIFACTS.EGGROCK.NAME, UI.SPACEARTIFACTS.EGGROCK.DESCRIPTION, "idle_egg_rock_light", "ui_egg_rock_light", DECOR.SPACEARTIFACT.TIER3, DlcManager.AVAILABLE_ALL_VERSIONS));
		list.Add(CreateArtifact("HatchFossil", UI.SPACEARTIFACTS.HATCHFOSSIL.NAME, UI.SPACEARTIFACTS.HATCHFOSSIL.DESCRIPTION, "idle_fossil_hatch", "ui_fossil_hatch", DECOR.SPACEARTIFACT.TIER3, DlcManager.AVAILABLE_ALL_VERSIONS));
		list.Add(CreateArtifact("RockTornado", UI.SPACEARTIFACTS.ROCKTORNADO.NAME, UI.SPACEARTIFACTS.ROCKTORNADO.DESCRIPTION, "idle_whirlwind_rock", "ui_whirlwind_rock", DECOR.SPACEARTIFACT.TIER3, DlcManager.AVAILABLE_ALL_VERSIONS));
		list.Add(CreateArtifact("PacuPercolator", UI.SPACEARTIFACTS.PACUPERCOLATOR.NAME, UI.SPACEARTIFACTS.PACUPERCOLATOR.DESCRIPTION, "idle_percolator", "ui_percolator", DECOR.SPACEARTIFACT.TIER3, DlcManager.AVAILABLE_ALL_VERSIONS, "artifacts_kanim", null, SimHashes.Creature, ArtifactType.Space));
		list.Add(CreateArtifact("MagmaLamp", UI.SPACEARTIFACTS.MAGMALAMP.NAME, UI.SPACEARTIFACTS.MAGMALAMP.DESCRIPTION, "idle_lava_lamp", "ui_lava_lamp", DECOR.SPACEARTIFACT.TIER3, DlcManager.AVAILABLE_ALL_VERSIONS, "artifacts_kanim", delegate(GameObject go)
		{
			Light2D light2D2 = go.AddOrGet<Light2D>();
			light2D2.overlayColour = LIGHT2D.MAGMALAMP_COLOR;
			light2D2.Color = LIGHT2D.MAGMALAMP_COLOR;
			light2D2.Range = 2f;
			light2D2.Angle = 0f;
			light2D2.Direction = LIGHT2D.MAGMALAMP_DIRECTION;
			light2D2.Offset = LIGHT2D.MAGMALAMP_OFFSET;
			light2D2.shape = LightShape.Cone;
			light2D2.drawOverlay = true;
		}));
		list.Add(CreateArtifact("Oracle", UI.SPACEARTIFACTS.ORACLE.NAME, UI.SPACEARTIFACTS.ORACLE.DESCRIPTION, "idle_oracle", "ui_oracle", DECOR.SPACEARTIFACT.TIER3, DlcManager.AVAILABLE_EXPANSION1_ONLY, "artifacts_2_kanim", null, SimHashes.Creature, ArtifactType.Terrestrial));
		list.Add(CreateArtifact("DNAModel", UI.SPACEARTIFACTS.DNAMODEL.NAME, UI.SPACEARTIFACTS.DNAMODEL.DESCRIPTION, "idle_dna", "ui_dna", DECOR.SPACEARTIFACT.TIER4, DlcManager.AVAILABLE_ALL_VERSIONS, "artifacts_kanim", null, SimHashes.Creature, ArtifactType.Terrestrial));
		list.Add(CreateArtifact("RainbowEggRock", UI.SPACEARTIFACTS.RAINBOWEGGROCK.NAME, UI.SPACEARTIFACTS.RAINBOWEGGROCK.DESCRIPTION, "idle_egg_rock_rainbow", "ui_egg_rock_rainbow", DECOR.SPACEARTIFACT.TIER4, DlcManager.AVAILABLE_ALL_VERSIONS));
		list.Add(CreateArtifact("PlasmaLamp", UI.SPACEARTIFACTS.PLASMALAMP.NAME, UI.SPACEARTIFACTS.PLASMALAMP.DESCRIPTION, "idle_plasma_lamp_loop", "ui_plasma_lamp", DECOR.SPACEARTIFACT.TIER4, DlcManager.AVAILABLE_ALL_VERSIONS, "artifacts_kanim", delegate(GameObject go)
		{
			go.AddOrGet<LoopingSounds>();
			Light2D light2D = go.AddOrGet<Light2D>();
			light2D.overlayColour = LIGHT2D.PLASMALAMP_COLOR;
			light2D.Color = LIGHT2D.PLASMALAMP_COLOR;
			light2D.Range = 2f;
			light2D.Angle = 0f;
			light2D.Direction = LIGHT2D.PLASMALAMP_DIRECTION;
			light2D.Offset = LIGHT2D.PLASMALAMP_OFFSET;
			light2D.shape = LightShape.Circle;
			light2D.drawOverlay = true;
		}));
		list.Add(CreateArtifact("MoodRing", UI.SPACEARTIFACTS.MOODRING.NAME, UI.SPACEARTIFACTS.MOODRING.DESCRIPTION, "idle_moodring", "ui_moodring", DECOR.SPACEARTIFACT.TIER4, DlcManager.AVAILABLE_EXPANSION1_ONLY, "artifacts_2_kanim"));
		list.Add(CreateArtifact("SolarSystem", UI.SPACEARTIFACTS.SOLARSYSTEM.NAME, UI.SPACEARTIFACTS.SOLARSYSTEM.DESCRIPTION, "idle_solar_system_loop", "ui_solar_system", DECOR.SPACEARTIFACT.TIER5, DlcManager.AVAILABLE_ALL_VERSIONS, "artifacts_kanim", delegate(GameObject go)
		{
			go.AddOrGet<LoopingSounds>();
		}, SimHashes.Creature, ArtifactType.Space));
		list.Add(CreateArtifact("Moonmoonmoon", UI.SPACEARTIFACTS.MOONMOONMOON.NAME, UI.SPACEARTIFACTS.MOONMOONMOON.DESCRIPTION, "idle_moon", "ui_moon", DECOR.SPACEARTIFACT.TIER5, DlcManager.AVAILABLE_ALL_VERSIONS, "artifacts_kanim", null, SimHashes.Creature, ArtifactType.Space));
		list.Add(CreateArtifact("ReactorModel", UI.SPACEARTIFACTS.REACTORMODEL.NAME, UI.SPACEARTIFACTS.REACTORMODEL.DESCRIPTION, "idle_model", "ui_model", DECOR.SPACEARTIFACT.TIER5, DlcManager.AVAILABLE_EXPANSION1_ONLY, "artifacts_2_kanim"));
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (list[num] == null)
			{
				list.RemoveAt(num);
			}
		}
		foreach (GameObject item in list)
		{
			SpaceArtifact component = item.GetComponent<SpaceArtifact>();
			ArtifactType key = (DlcManager.IsExpansion1Active() ? component.artifactType : ArtifactType.Any);
			artifactItems[key].Add(item.name);
		}
		return list;
	}

	public static GameObject CreateArtifact(string id, string name, string desc, string initial_anim, string ui_anim, ArtifactTier artifact_tier, string[] dlcIDs, string animFile = "artifacts_kanim", PostInitFn postInitFn = null, SimHashes element = SimHashes.Creature, ArtifactType artifact_type = ArtifactType.Any)
	{
		if (!DlcManager.IsDlcListValidForCurrentContent(dlcIDs))
		{
			return null;
		}
		GameObject gameObject = EntityTemplates.CreateLooseEntity("artifact_" + id.ToLower(), name, desc, 25f, unitMass: true, Assets.GetAnim(animFile), initial_anim, Grid.SceneLayer.Ore, EntityTemplates.CollisionShape.RECTANGLE, 1f, 1f, isPickupable: true, SORTORDER.BUILDINGELEMENTS, element, new List<Tag> { GameTags.MiscPickupable });
		gameObject.AddOrGet<OccupyArea>().OccupiedCellsOffsets = EntityTemplates.GenerateOffsets(1, 1);
		DecorProvider decorProvider = gameObject.AddOrGet<DecorProvider>();
		decorProvider.SetValues(artifact_tier.decorValues);
		decorProvider.overrideName = gameObject.name;
		SpaceArtifact spaceArtifact = gameObject.AddOrGet<SpaceArtifact>();
		spaceArtifact.SetUIAnim(ui_anim);
		spaceArtifact.SetArtifactTier(artifact_tier);
		spaceArtifact.uniqueAnimNameFragment = initial_anim;
		spaceArtifact.artifactType = artifact_type;
		gameObject.AddOrGet<KSelectable>();
		gameObject.GetComponent<KPrefabID>().prefabSpawnFn += delegate(GameObject instance)
		{
			instance.GetComponent<SpaceArtifact>().SetArtifactTier(artifact_tier);
		};
		gameObject.GetComponent<KBatchedAnimController>().initialMode = KAnim.PlayMode.Loop;
		postInitFn?.Invoke(gameObject);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddTag(GameTags.PedestalDisplayable);
		component.AddTag(GameTags.Artifact);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
