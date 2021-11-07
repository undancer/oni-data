using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using Klei;
using Klei.AI;
using ProcGen;
using ProcGenGame;
using STRINGS;
using TUNING;
using UnityEngine;

public static class CodexEntryGenerator
{
	private class ConversionEntry
	{
		public string title;

		public GameObject prefab;

		public HashSet<ElementUsage> inSet = new HashSet<ElementUsage>();

		public HashSet<ElementUsage> outSet = new HashSet<ElementUsage>();
	}

	private class CodexElementMap
	{
		public Dictionary<Tag, List<ConversionEntry>> map = new Dictionary<Tag, List<ConversionEntry>>();

		public void Add(Tag t, ConversionEntry ce)
		{
			if (map.TryGetValue(t, out var value))
			{
				value.Add(ce);
				return;
			}
			map[t] = new List<ConversionEntry> { ce };
		}
	}

	public static Dictionary<string, CodexEntry> GenerateBuildingEntries()
	{
		string text = "BUILD_CATEGORY_";
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		foreach (PlanScreen.PlanInfo item in TUNING.BUILDINGS.PLANORDER)
		{
			string text2 = HashCache.Get().Get(item.category);
			string text3 = CodexCache.FormatLinkID(text + text2);
			Dictionary<string, CodexEntry> dictionary2 = new Dictionary<string, CodexEntry>();
			for (int i = 0; i < ((ICollection<string>)item.data).Count; i++)
			{
				BuildingDef buildingDef = Assets.GetBuildingDef(((IList<string>)item.data)[i]);
				if (!buildingDef.DebugOnly)
				{
					List<ContentContainer> list = new List<ContentContainer>();
					List<ICodexWidget> list2 = new List<ICodexWidget>();
					list2.Add(new CodexText(buildingDef.Name, CodexTextStyle.Title));
					Tech tech = Db.Get().Techs.TryGetTechForTechItem(buildingDef.PrefabID);
					if (tech != null)
					{
						list2.Add(new CodexLabelWithIcon(tech.Name, CodexTextStyle.Body, new Tuple<Sprite, Color>(Assets.GetSprite("research_type_alpha_icon"), Color.white)));
					}
					list2.Add(new CodexDividerLine());
					list.Add(new ContentContainer(list2, ContentContainer.ContentLayout.Vertical));
					GenerateImageContainers(buildingDef.GetUISprite(), list);
					GenerateBuildingDescriptionContainers(buildingDef, list);
					GenerateFabricatorContainers(buildingDef.BuildingComplete, list);
					GenerateReceptacleContainers(buildingDef.BuildingComplete, list);
					CodexEntry codexEntry = new CodexEntry(text3, list, Strings.Get("STRINGS.BUILDINGS.PREFABS." + ((IList<string>)item.data)[i].ToUpper() + ".NAME"));
					codexEntry.icon = buildingDef.GetUISprite();
					codexEntry.parentId = text3;
					CodexCache.AddEntry(((IList<string>)item.data)[i], codexEntry);
					dictionary2.Add(codexEntry.id, codexEntry);
				}
			}
			if (dictionary2.Count != 0)
			{
				CategoryEntry categoryEntry = GenerateCategoryEntry(CodexCache.FormatLinkID(text3), Strings.Get("STRINGS.UI.BUILDCATEGORIES." + text2.ToUpper() + ".NAME"), dictionary2);
				categoryEntry.parentId = "BUILDINGS";
				categoryEntry.category = "BUILDINGS";
				categoryEntry.icon = Assets.GetSprite(PlanScreen.IconNameMap[text2]);
				dictionary.Add(text3, categoryEntry);
			}
		}
		PopulateCategoryEntries(dictionary);
		return dictionary;
	}

	public static void GeneratePageNotFound()
	{
		List<ContentContainer> list = new List<ContentContainer>();
		ContentContainer contentContainer = new ContentContainer();
		contentContainer.content.Add(new CodexText(CODEX.PAGENOTFOUND.TITLE, CodexTextStyle.Title));
		contentContainer.content.Add(new CodexText(CODEX.PAGENOTFOUND.SUBTITLE, CodexTextStyle.Subtitle));
		contentContainer.content.Add(new CodexDividerLine());
		contentContainer.content.Add(new CodexImage(312, 312, Assets.GetSprite("outhouseMessage")));
		list.Add(contentContainer);
		CodexEntry codexEntry = new CodexEntry("ROOT", list, CODEX.PAGENOTFOUND.TITLE);
		codexEntry.searchOnly = true;
		CodexCache.AddEntry("PageNotFound", codexEntry);
	}

	public static Dictionary<string, CodexEntry> GenerateCreatureEntries()
	{
		Dictionary<string, CodexEntry> results = new Dictionary<string, CodexEntry>();
		List<GameObject> brains = Assets.GetPrefabsWithComponent<CreatureBrain>();
		Action<Tag, string> obj = delegate(Tag speciesTag, string name)
		{
			bool flag = false;
			List<ContentContainer> list = new List<ContentContainer>();
			CodexEntry codexEntry = new CodexEntry("CREATURES", list, name);
			foreach (GameObject item2 in brains)
			{
				if (item2.GetDef<BabyMonitor.Def>() == null)
				{
					Sprite sprite = null;
					Sprite sprite2 = null;
					GameObject gameObject = Assets.TryGetPrefab(item2.PrefabID().ToString() + "Baby");
					if (gameObject != null)
					{
						sprite2 = Def.GetUISprite(gameObject).first;
					}
					CreatureBrain component = item2.GetComponent<CreatureBrain>();
					if (!(component.species != speciesTag))
					{
						if (!flag)
						{
							flag = true;
							list.Add(new ContentContainer(new List<ICodexWidget>
							{
								new CodexSpacer(),
								new CodexSpacer()
							}, ContentContainer.ContentLayout.Vertical));
							codexEntry.parentId = "CREATURES";
							CodexCache.AddEntry(speciesTag.ToString(), codexEntry);
							results.Add(speciesTag.ToString(), codexEntry);
						}
						List<ContentContainer> list2 = new List<ContentContainer>();
						string symbolPrefix = component.symbolPrefix;
						sprite = Def.GetUISprite(item2, symbolPrefix + "ui").first;
						if ((bool)sprite2)
						{
							GenerateImageContainers(new Sprite[2] { sprite, sprite2 }, list2, ContentContainer.ContentLayout.Horizontal);
						}
						else
						{
							GenerateImageContainers(sprite, list2);
						}
						GenerateCreatureDescriptionContainers(item2, list2);
						SubEntry item = new SubEntry(component.PrefabID().ToString(), speciesTag.ToString(), list2, component.GetProperName())
						{
							icon = sprite,
							iconColor = Color.white
						};
						codexEntry.subEntries.Add(item);
					}
				}
			}
		};
		obj(GameTags.Creatures.Species.PuftSpecies, STRINGS.CREATURES.FAMILY_PLURAL.PUFTSPECIES);
		obj(GameTags.Creatures.Species.PacuSpecies, STRINGS.CREATURES.FAMILY_PLURAL.PACUSPECIES);
		obj(GameTags.Creatures.Species.OilFloaterSpecies, STRINGS.CREATURES.FAMILY_PLURAL.OILFLOATERSPECIES);
		obj(GameTags.Creatures.Species.LightBugSpecies, STRINGS.CREATURES.FAMILY_PLURAL.LIGHTBUGSPECIES);
		obj(GameTags.Creatures.Species.HatchSpecies, STRINGS.CREATURES.FAMILY_PLURAL.HATCHSPECIES);
		obj(GameTags.Creatures.Species.GlomSpecies, STRINGS.CREATURES.FAMILY_PLURAL.GLOMSPECIES);
		obj(GameTags.Creatures.Species.DreckoSpecies, STRINGS.CREATURES.FAMILY_PLURAL.DRECKOSPECIES);
		obj(GameTags.Creatures.Species.MooSpecies, STRINGS.CREATURES.FAMILY_PLURAL.MOOSPECIES);
		obj(GameTags.Creatures.Species.MoleSpecies, STRINGS.CREATURES.FAMILY_PLURAL.MOLESPECIES);
		obj(GameTags.Creatures.Species.SquirrelSpecies, STRINGS.CREATURES.FAMILY_PLURAL.SQUIRRELSPECIES);
		obj(GameTags.Creatures.Species.CrabSpecies, STRINGS.CREATURES.FAMILY_PLURAL.CRABSPECIES);
		obj(GameTags.Robots.Models.ScoutRover, STRINGS.CREATURES.FAMILY_PLURAL.SCOUTROVER);
		obj(GameTags.Creatures.Species.StaterpillarSpecies, STRINGS.CREATURES.FAMILY_PLURAL.STATERPILLARSPECIES);
		obj(GameTags.Creatures.Species.BeetaSpecies, STRINGS.CREATURES.FAMILY_PLURAL.BEETASPECIES);
		obj(GameTags.Creatures.Species.DivergentSpecies, STRINGS.CREATURES.FAMILY_PLURAL.DIVERGENTSPECIES);
		obj(GameTags.Robots.Models.SweepBot, STRINGS.CREATURES.FAMILY_PLURAL.SWEEPBOT);
		return results;
	}

	public static Dictionary<string, CodexEntry> GeneratePlantEntries()
	{
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		List<GameObject> prefabsWithComponent = Assets.GetPrefabsWithComponent<Harvestable>();
		prefabsWithComponent.AddRange(Assets.GetPrefabsWithComponent<WiltCondition>());
		foreach (GameObject item in prefabsWithComponent)
		{
			if (!dictionary.ContainsKey(item.PrefabID().ToString()) && !(item.GetComponent<BudUprootedMonitor>() != null))
			{
				List<ContentContainer> list = new List<ContentContainer>();
				Sprite first = Def.GetUISprite(item).first;
				GenerateImageContainers(first, list);
				GeneratePlantDescriptionContainers(item, list);
				CodexEntry codexEntry = new CodexEntry("PLANTS", list, item.GetProperName());
				codexEntry.parentId = "PLANTS";
				codexEntry.icon = first;
				CodexCache.AddEntry(item.PrefabID().ToString(), codexEntry);
				dictionary.Add(item.PrefabID().ToString(), codexEntry);
			}
		}
		return dictionary;
	}

	public static Dictionary<string, CodexEntry> GenerateFoodEntries()
	{
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		foreach (EdiblesManager.FoodInfo allFoodType in EdiblesManager.GetAllFoodTypes())
		{
			GameObject prefab = Assets.GetPrefab(allFoodType.Id);
			if (!prefab.HasTag(GameTags.DeprecatedContent) && !prefab.HasTag(GameTags.IncubatableEgg))
			{
				List<ContentContainer> list = new List<ContentContainer>();
				GenerateTitleContainers(allFoodType.Name, list);
				Sprite first = Def.GetUISprite(allFoodType.ConsumableId).first;
				GenerateImageContainers(first, list);
				GenerateFoodDescriptionContainers(allFoodType, list);
				GenerateRecipeContainers(allFoodType.ConsumableId.ToTag(), list);
				GenerateUsedInRecipeContainers(allFoodType.ConsumableId.ToTag(), list);
				CodexEntry codexEntry = new CodexEntry("FOOD", list, allFoodType.Name);
				codexEntry.icon = first;
				codexEntry.parentId = "FOOD";
				CodexCache.AddEntry(allFoodType.Id, codexEntry);
				dictionary.Add(allFoodType.Id, codexEntry);
			}
		}
		return dictionary;
	}

	public static Dictionary<string, CodexEntry> GenerateTechEntries()
	{
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		foreach (Tech resource in Db.Get().Techs.resources)
		{
			List<ContentContainer> list = new List<ContentContainer>();
			GenerateTitleContainers(resource.Name, list);
			GenerateTechDescriptionContainers(resource, list);
			GeneratePrerequisiteTechContainers(resource, list);
			GenerateUnlockContainers(resource, list);
			CodexEntry codexEntry = new CodexEntry("TECH", list, resource.Name);
			codexEntry.icon = ((resource.unlockedItems.Count != 0) ? resource.unlockedItems[0] : null)?.getUISprite("ui", arg2: false);
			codexEntry.parentId = "TECH";
			CodexCache.AddEntry(resource.Id, codexEntry);
			dictionary.Add(resource.Id, codexEntry);
		}
		return dictionary;
	}

	public static Dictionary<string, CodexEntry> GenerateRoleEntries()
	{
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		foreach (Skill resource in Db.Get().Skills.resources)
		{
			if (!resource.deprecated)
			{
				List<ContentContainer> list = new List<ContentContainer>();
				Sprite sprite = null;
				sprite = Assets.GetSprite(resource.hat);
				GenerateTitleContainers(resource.Name, list);
				GenerateImageContainers(sprite, list);
				GenerateGenericDescriptionContainers(resource.description, list);
				GenerateSkillRequirementsAndPerksContainers(resource, list);
				GenerateRelatedSkillContainers(resource, list);
				CodexEntry codexEntry = new CodexEntry("ROLES", list, resource.Name);
				codexEntry.parentId = "ROLES";
				codexEntry.icon = sprite;
				CodexCache.AddEntry(resource.Id, codexEntry);
				dictionary.Add(resource.Id, codexEntry);
			}
		}
		return dictionary;
	}

	public static Dictionary<string, CodexEntry> GenerateGeyserEntries()
	{
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		List<GameObject> prefabsWithComponent = Assets.GetPrefabsWithComponent<Geyser>();
		if (prefabsWithComponent != null)
		{
			foreach (GameObject item2 in prefabsWithComponent)
			{
				if (!item2.GetComponent<KPrefabID>().HasTag(GameTags.DeprecatedContent))
				{
					List<ContentContainer> list = new List<ContentContainer>();
					GenerateTitleContainers(item2.GetProperName(), list);
					Sprite first = Def.GetUISprite(item2).first;
					GenerateImageContainers(first, list);
					List<ICodexWidget> list2 = new List<ICodexWidget>();
					string text = item2.PrefabID().ToString().ToUpper();
					string text2 = "GENERICGEYSER_";
					if (text.StartsWith(text2))
					{
						text.Remove(0, text2.Length);
					}
					list2.Add(new CodexText(UI.CODEX.GEYSERS.DESC));
					ContentContainer item = new ContentContainer(list2, ContentContainer.ContentLayout.Vertical);
					list.Add(item);
					CodexEntry codexEntry = new CodexEntry("GEYSERS", list, item2.GetProperName());
					codexEntry.icon = first;
					codexEntry.parentId = "GEYSERS";
					codexEntry.id = item2.PrefabID().ToString();
					CodexCache.AddEntry(codexEntry.id, codexEntry);
					dictionary.Add(codexEntry.id, codexEntry);
				}
			}
			return dictionary;
		}
		return dictionary;
	}

	public static Dictionary<string, CodexEntry> GenerateEquipmentEntries()
	{
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		List<GameObject> prefabsWithComponent = Assets.GetPrefabsWithComponent<Equippable>();
		if (prefabsWithComponent != null)
		{
			foreach (GameObject item2 in prefabsWithComponent)
			{
				bool flag = false;
				Equippable component = item2.GetComponent<Equippable>();
				if (component.def.AdditionalTags != null)
				{
					Tag[] additionalTags = component.def.AdditionalTags;
					for (int i = 0; i < additionalTags.Length; i++)
					{
						if (additionalTags[i] == GameTags.DeprecatedContent)
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag && !component.hideInCodex)
				{
					List<ContentContainer> list = new List<ContentContainer>();
					GenerateTitleContainers(item2.GetProperName(), list);
					Sprite first = Def.GetUISprite(item2).first;
					GenerateImageContainers(first, list);
					List<ICodexWidget> list2 = new List<ICodexWidget>();
					string text = item2.PrefabID().ToString();
					list2.Add(new CodexText(Strings.Get("STRINGS.EQUIPMENT.PREFABS." + text.ToUpper() + ".DESC")));
					ContentContainer item = new ContentContainer(list2, ContentContainer.ContentLayout.Vertical);
					list.Add(item);
					CodexEntry codexEntry = new CodexEntry("EQUIPMENT", list, item2.GetProperName());
					codexEntry.icon = first;
					codexEntry.parentId = "EQUIPMENT";
					codexEntry.id = item2.PrefabID().ToString();
					CodexCache.AddEntry(codexEntry.id, codexEntry);
					dictionary.Add(codexEntry.id, codexEntry);
				}
			}
			return dictionary;
		}
		return dictionary;
	}

	public static Dictionary<string, CodexEntry> GenerateBiomeEntries()
	{
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		ListPool<YamlIO.Error, WorldGen>.PooledList pooledList = ListPool<YamlIO.Error, WorldGen>.Allocate();
		_ = Application.streamingAssetsPath + "/worldgen/worlds/";
		_ = Application.streamingAssetsPath + "/worldgen/biomes/";
		_ = Application.streamingAssetsPath + "/worldgen/subworlds/";
		WorldGen.LoadSettings();
		Dictionary<string, List<WeightedSubworldName>> dictionary2 = new Dictionary<string, List<WeightedSubworldName>>();
		foreach (KeyValuePair<string, ClusterLayout> item5 in SettingsCache.clusterLayouts.clusterCache)
		{
			ClusterLayout value = item5.Value;
			_ = value.filePath;
			foreach (WorldPlacement worldPlacement in value.worldPlacements)
			{
				foreach (WeightedSubworldName subworldFile in SettingsCache.worlds.GetWorldData(worldPlacement.world).subworldFiles)
				{
					string text = subworldFile.name.Substring(subworldFile.name.LastIndexOf("/"));
					string text2 = subworldFile.name.Substring(0, subworldFile.name.Length - text.Length);
					text2 = text2.Substring(text2.LastIndexOf("/") + 1);
					if (!(text2 == "subworlds"))
					{
						if (!dictionary2.ContainsKey(text2))
						{
							dictionary2.Add(text2, new List<WeightedSubworldName> { subworldFile });
						}
						else
						{
							dictionary2[text2].Add(subworldFile);
						}
					}
				}
			}
		}
		foreach (KeyValuePair<string, List<WeightedSubworldName>> item6 in dictionary2)
		{
			string text3 = CodexCache.FormatLinkID(item6.Key);
			Tuple<Sprite, Color> tuple = null;
			string text4 = Strings.Get("STRINGS.SUBWORLDS." + text3.ToUpper() + ".NAME");
			if (text4.Contains("MISSING"))
			{
				text4 = text3 + " (missing string key)";
			}
			List<ContentContainer> list = new List<ContentContainer>();
			GenerateTitleContainers(text4, list);
			string text5 = "biomeIcon" + char.ToUpper(text3[0]) + text3.Substring(1).ToLower();
			Sprite sprite = Assets.GetSprite(text5);
			if (sprite != null)
			{
				tuple = new Tuple<Sprite, Color>(sprite, Color.white);
			}
			else
			{
				Debug.LogWarning("Missing codex biome icon: " + text5);
			}
			string text6 = Strings.Get("STRINGS.SUBWORLDS." + text3.ToUpper() + ".DESC");
			string text7 = Strings.Get("STRINGS.SUBWORLDS." + text3.ToUpper() + ".UTILITY");
			ContentContainer item = new ContentContainer(new List<ICodexWidget>
			{
				new CodexText(string.IsNullOrEmpty(text6) ? "Basic description of the biome." : text6),
				new CodexSpacer(),
				new CodexText(string.IsNullOrEmpty(text7) ? "Description of the biomes utility." : text7),
				new CodexSpacer()
			}, ContentContainer.ContentLayout.Vertical);
			list.Add(item);
			Dictionary<string, float> dictionary3 = new Dictionary<string, float>();
			ContentContainer item2 = new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexText(UI.CODEX.SUBWORLDS.ELEMENTS, CodexTextStyle.Subtitle),
				new CodexSpacer()
			}, ContentContainer.ContentLayout.Vertical);
			list.Add(item2);
			ContentContainer contentContainer = new ContentContainer();
			contentContainer.contentLayout = ContentContainer.ContentLayout.Vertical;
			contentContainer.content = new List<ICodexWidget>();
			list.Add(contentContainer);
			foreach (WeightedSubworldName item7 in item6.Value)
			{
				SubWorld subWorld = SettingsCache.subworlds[item7.name];
				foreach (WeightedBiome biome in SettingsCache.subworlds[item7.name].biomes)
				{
					foreach (ElementGradient item8 in SettingsCache.biomes.BiomeBackgroundElementBandConfigurations[biome.name])
					{
						if (dictionary3.ContainsKey(item8.content))
						{
							dictionary3[item8.content] = dictionary3[item8.content] + item8.bandSize;
							continue;
						}
						if (ElementLoader.FindElementByName(item8.content) == null)
						{
							Debug.LogError("Biome " + biome.name + " contains non-existent element " + item8.content);
						}
						dictionary3.Add(item8.content, item8.bandSize);
					}
				}
				foreach (Feature feature in subWorld.features)
				{
					foreach (KeyValuePair<string, ElementChoiceGroup<WeightedSimHash>> elementChoiceGroup in SettingsCache.GetCachedFeature(feature.type).ElementChoiceGroups)
					{
						foreach (WeightedSimHash choice in elementChoiceGroup.Value.choices)
						{
							if (dictionary3.ContainsKey(choice.element))
							{
								dictionary3[choice.element] = dictionary3[choice.element] + 1f;
							}
							else
							{
								dictionary3.Add(choice.element, 1f);
							}
						}
					}
				}
			}
			foreach (KeyValuePair<string, float> item9 in dictionary3.OrderBy(delegate(KeyValuePair<string, float> pair)
			{
				KeyValuePair<string, float> keyValuePair = pair;
				return keyValuePair.Value;
			}))
			{
				Element element = ElementLoader.FindElementByName(item9.Key);
				if (tuple == null)
				{
					tuple = Def.GetUISprite(element.substance);
				}
				contentContainer.content.Add(new CodexIndentedLabelWithIcon(element.name, CodexTextStyle.Body, Def.GetUISprite(element.substance)));
			}
			List<Tag> list2 = new List<Tag>();
			ContentContainer item3 = new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexText(UI.CODEX.SUBWORLDS.PLANTS, CodexTextStyle.Subtitle),
				new CodexSpacer()
			}, ContentContainer.ContentLayout.Vertical);
			list.Add(item3);
			ContentContainer contentContainer2 = new ContentContainer();
			contentContainer2.contentLayout = ContentContainer.ContentLayout.Vertical;
			contentContainer2.content = new List<ICodexWidget>();
			list.Add(contentContainer2);
			foreach (WeightedSubworldName item10 in item6.Value)
			{
				foreach (WeightedBiome biome2 in SettingsCache.subworlds[item10.name].biomes)
				{
					if (biome2.tags == null)
					{
						continue;
					}
					foreach (string tag3 in biome2.tags)
					{
						if (!list2.Contains(tag3))
						{
							GameObject gameObject = Assets.TryGetPrefab(tag3);
							if (gameObject != null && (gameObject.GetComponent<Harvestable>() != null || gameObject.GetComponent<SeedProducer>() != null))
							{
								list2.Add(tag3);
								contentContainer2.content.Add(new CodexIndentedLabelWithIcon(gameObject.GetProperName(), CodexTextStyle.Body, Def.GetUISprite(gameObject)));
							}
						}
					}
				}
				foreach (Feature feature2 in SettingsCache.subworlds[item10.name].features)
				{
					foreach (MobReference internalMob in SettingsCache.GetCachedFeature(feature2.type).internalMobs)
					{
						Tag tag = internalMob.type.ToTag();
						if (!list2.Contains(tag))
						{
							GameObject gameObject2 = Assets.TryGetPrefab(tag);
							if (gameObject2 != null && (gameObject2.GetComponent<Harvestable>() != null || gameObject2.GetComponent<SeedProducer>() != null))
							{
								list2.Add(tag);
								contentContainer2.content.Add(new CodexIndentedLabelWithIcon(gameObject2.GetProperName(), CodexTextStyle.Body, Def.GetUISprite(gameObject2)));
							}
						}
					}
				}
			}
			if (list2.Count == 0)
			{
				contentContainer2.content.Add(new CodexIndentedLabelWithIcon(UI.CODEX.SUBWORLDS.NONE, CodexTextStyle.Body, new Tuple<Sprite, Color>(Assets.GetSprite("inspectorUI_cannot_build"), Color.red)));
			}
			List<Tag> list3 = new List<Tag>();
			ContentContainer item4 = new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexText(UI.CODEX.SUBWORLDS.CRITTERS, CodexTextStyle.Subtitle),
				new CodexSpacer()
			}, ContentContainer.ContentLayout.Vertical);
			list.Add(item4);
			ContentContainer contentContainer3 = new ContentContainer();
			contentContainer3.contentLayout = ContentContainer.ContentLayout.Vertical;
			contentContainer3.content = new List<ICodexWidget>();
			list.Add(contentContainer3);
			foreach (WeightedSubworldName item11 in item6.Value)
			{
				foreach (WeightedBiome biome3 in SettingsCache.subworlds[item11.name].biomes)
				{
					if (biome3.tags == null)
					{
						continue;
					}
					foreach (string tag4 in biome3.tags)
					{
						if (!list3.Contains(tag4))
						{
							GameObject gameObject3 = Assets.TryGetPrefab(tag4);
							if (gameObject3 != null && gameObject3.HasTag(GameTags.Creature))
							{
								list3.Add(tag4);
								contentContainer3.content.Add(new CodexIndentedLabelWithIcon(gameObject3.GetProperName(), CodexTextStyle.Body, Def.GetUISprite(gameObject3)));
							}
						}
					}
				}
				foreach (Feature feature3 in SettingsCache.subworlds[item11.name].features)
				{
					foreach (MobReference internalMob2 in SettingsCache.GetCachedFeature(feature3.type).internalMobs)
					{
						Tag tag2 = internalMob2.type.ToTag();
						if (!list3.Contains(tag2))
						{
							GameObject gameObject4 = Assets.TryGetPrefab(tag2);
							if (gameObject4 != null && gameObject4.HasTag(GameTags.Creature))
							{
								list3.Add(tag2);
								contentContainer3.content.Add(new CodexIndentedLabelWithIcon(gameObject4.GetProperName(), CodexTextStyle.Body, Def.GetUISprite(gameObject4)));
							}
						}
					}
				}
			}
			if (list3.Count == 0)
			{
				contentContainer3.content.Add(new CodexIndentedLabelWithIcon(UI.CODEX.SUBWORLDS.NONE, CodexTextStyle.Body, new Tuple<Sprite, Color>(Assets.GetSprite("inspectorUI_cannot_build"), Color.red)));
			}
			string text8 = "BIOME" + text3;
			CodexEntry codexEntry = new CodexEntry("BIOMES", list, text8);
			codexEntry.name = text4;
			codexEntry.parentId = "BIOMES";
			codexEntry.icon = tuple.first;
			codexEntry.iconColor = tuple.second;
			CodexCache.AddEntry(text8, codexEntry);
			dictionary.Add(text8, codexEntry);
		}
		if (Application.isPlaying)
		{
			Global.Instance.modManager.HandleErrors(pooledList);
		}
		else
		{
			foreach (YamlIO.Error item12 in pooledList)
			{
				YamlIO.LogError(item12, force_log_as_warning: false);
			}
		}
		pooledList.Recycle();
		return dictionary;
	}

	public static Dictionary<string, CodexEntry> GenerateElementEntries()
	{
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		Dictionary<string, CodexEntry> dictionary2 = new Dictionary<string, CodexEntry>();
		Dictionary<string, CodexEntry> dictionary3 = new Dictionary<string, CodexEntry>();
		Dictionary<string, CodexEntry> dictionary4 = new Dictionary<string, CodexEntry>();
		Dictionary<string, CodexEntry> dictionary5 = new Dictionary<string, CodexEntry>();
		string text = CodexCache.FormatLinkID("ELEMENTS");
		string text2 = CodexCache.FormatLinkID("ELEMENTS_SOLID");
		string text3 = CodexCache.FormatLinkID("ELEMENTS_LIQUID");
		string text4 = CodexCache.FormatLinkID("ELEMENTS_GAS");
		string text5 = CodexCache.FormatLinkID("ELEMENTS_OTHER");
		string text6 = CodexCache.FormatLinkID("ELEMENTS_CLASSES");
		CodexElementMap usedMap2 = new CodexElementMap();
		CodexElementMap madeMap = new CodexElementMap();
		Tag waterTag = ElementLoader.FindElementByHash(SimHashes.Water).tag;
		Tag dirtyWaterTag = ElementLoader.FindElementByHash(SimHashes.DirtyWater).tag;
		Action<GameObject, CodexElementMap, CodexElementMap> action = delegate(GameObject prefab, CodexElementMap usedMap, CodexElementMap made)
		{
			HashSet<ElementUsage> hashSet2 = new HashSet<ElementUsage>();
			HashSet<ElementUsage> hashSet3 = new HashSet<ElementUsage>();
			EnergyGenerator component = prefab.GetComponent<EnergyGenerator>();
			if ((bool)component)
			{
				IEnumerable<EnergyGenerator.InputItem> inputs = component.formula.inputs;
				foreach (EnergyGenerator.InputItem item in inputs ?? Enumerable.Empty<EnergyGenerator.InputItem>())
				{
					hashSet2.Add(new ElementUsage(item.tag, item.consumptionRate, continuous: true));
				}
				IEnumerable<EnergyGenerator.OutputItem> outputs = component.formula.outputs;
				foreach (EnergyGenerator.OutputItem item2 in outputs ?? Enumerable.Empty<EnergyGenerator.OutputItem>())
				{
					Tag tag = ElementLoader.FindElementByHash(item2.element).tag;
					hashSet3.Add(new ElementUsage(tag, item2.creationRate, continuous: true));
				}
			}
			IEnumerable<ElementConverter> components = prefab.GetComponents<ElementConverter>();
			foreach (ElementConverter item3 in components ?? Enumerable.Empty<ElementConverter>())
			{
				IEnumerable<ElementConverter.ConsumedElement> consumedElements = item3.consumedElements;
				foreach (ElementConverter.ConsumedElement item4 in consumedElements ?? Enumerable.Empty<ElementConverter.ConsumedElement>())
				{
					hashSet2.Add(new ElementUsage(item4.tag, item4.massConsumptionRate, continuous: true));
				}
				IEnumerable<ElementConverter.OutputElement> outputElements = item3.outputElements;
				foreach (ElementConverter.OutputElement item5 in outputElements ?? Enumerable.Empty<ElementConverter.OutputElement>())
				{
					Tag tag2 = ElementLoader.FindElementByHash(item5.elementHash).tag;
					hashSet3.Add(new ElementUsage(tag2, item5.massGenerationRate, continuous: true));
				}
			}
			IEnumerable<ElementConsumer> components2 = prefab.GetComponents<ElementConsumer>();
			foreach (ElementConsumer item6 in components2 ?? Enumerable.Empty<ElementConsumer>())
			{
				if (!item6.storeOnConsume)
				{
					Tag tag3 = ElementLoader.FindElementByHash(item6.elementToConsume).tag;
					hashSet2.Add(new ElementUsage(tag3, item6.consumptionRate, continuous: true));
				}
			}
			IrrigationMonitor.Def def = prefab.GetDef<IrrigationMonitor.Def>();
			if (def != null)
			{
				PlantElementAbsorber.ConsumeInfo[] consumedElements2 = def.consumedElements;
				for (int l = 0; l < consumedElements2.Length; l++)
				{
					PlantElementAbsorber.ConsumeInfo consumeInfo = consumedElements2[l];
					hashSet2.Add(new ElementUsage(consumeInfo.tag, consumeInfo.massConsumptionRate, continuous: true));
				}
			}
			FertilizationMonitor.Def def2 = prefab.GetDef<FertilizationMonitor.Def>();
			if (def2 != null)
			{
				PlantElementAbsorber.ConsumeInfo[] consumedElements2 = def2.consumedElements;
				for (int l = 0; l < consumedElements2.Length; l++)
				{
					PlantElementAbsorber.ConsumeInfo consumeInfo2 = consumedElements2[l];
					hashSet2.Add(new ElementUsage(consumeInfo2.tag, consumeInfo2.massConsumptionRate, continuous: true));
				}
			}
			FlushToilet component2 = prefab.GetComponent<FlushToilet>();
			if ((bool)component2)
			{
				hashSet2.Add(new ElementUsage(waterTag, component2.massConsumedPerUse, continuous: false));
				hashSet3.Add(new ElementUsage(dirtyWaterTag, component2.massEmittedPerUse, continuous: false));
			}
			HandSanitizer component3 = prefab.GetComponent<HandSanitizer>();
			if ((bool)component3)
			{
				Tag tag4 = ElementLoader.FindElementByHash(component3.consumedElement).tag;
				hashSet2.Add(new ElementUsage(tag4, component3.massConsumedPerUse, continuous: false));
				if (component3.outputElement != SimHashes.Vacuum)
				{
					Tag tag5 = ElementLoader.FindElementByHash(component3.outputElement).tag;
					hashSet3.Add(new ElementUsage(tag5, component3.massConsumedPerUse, continuous: false));
				}
			}
			ConversionEntry ce = new ConversionEntry
			{
				title = prefab.GetProperName(),
				prefab = prefab,
				inSet = hashSet2,
				outSet = hashSet3
			};
			foreach (ElementUsage item7 in hashSet2)
			{
				usedMap.Add(item7.tag, ce);
			}
			foreach (ElementUsage item8 in hashSet3)
			{
				madeMap.Add(item8.tag, ce);
			}
		};
		foreach (PlanScreen.PlanInfo item9 in TUNING.BUILDINGS.PLANORDER)
		{
			foreach (string datum in item9.data)
			{
				BuildingDef buildingDef = Assets.GetBuildingDef(datum);
				if (!buildingDef.Deprecated)
				{
					action(buildingDef.BuildingComplete, usedMap2, madeMap);
				}
			}
		}
		HashSet<GameObject> hashSet = new HashSet<GameObject>(Assets.GetPrefabsWithComponent<Harvestable>());
		foreach (GameObject item10 in Assets.GetPrefabsWithComponent<WiltCondition>())
		{
			hashSet.Add(item10);
		}
		foreach (GameObject item11 in hashSet)
		{
			if (!(item11.GetComponent<BudUprootedMonitor>() != null))
			{
				action(item11, usedMap2, madeMap);
			}
		}
		foreach (GameObject item12 in Assets.GetPrefabsWithComponent<CreatureBrain>())
		{
			if (item12.GetDef<BabyMonitor.Def>() == null)
			{
				action(item12, usedMap2, madeMap);
			}
		}
		foreach (KeyValuePair<Tag, Diet> item13 in DietManager.CollectDiets(null))
		{
			GameObject gameObject = Assets.GetPrefab(item13.Key).gameObject;
			if (gameObject.GetDef<BabyMonitor.Def>() != null)
			{
				continue;
			}
			float num = 0f;
			foreach (AttributeModifier selfModifier in Db.Get().traits.Get(gameObject.GetComponent<Modifiers>().initialTraits[0]).SelfModifiers)
			{
				if (selfModifier.AttributeId == Db.Get().Amounts.Calories.deltaAttribute.Id)
				{
					num = selfModifier.Value;
				}
			}
			Diet.Info[] infos = item13.Value.infos;
			foreach (Diet.Info info in infos)
			{
				float num2 = (0f - num) / info.caloriesPerKg;
				float amount = num2 * info.producedConversionRate;
				foreach (Tag consumedTag in info.consumedTags)
				{
					ConversionEntry conversionEntry = new ConversionEntry();
					conversionEntry.title = gameObject.GetProperName();
					conversionEntry.prefab = gameObject;
					conversionEntry.inSet = new HashSet<ElementUsage>();
					conversionEntry.inSet.Add(new ElementUsage(consumedTag, num2, continuous: true));
					conversionEntry.outSet = new HashSet<ElementUsage>();
					conversionEntry.outSet.Add(new ElementUsage(info.producedElement, amount, continuous: true));
					usedMap2.Add(consumedTag, conversionEntry);
					madeMap.Add(info.producedElement, conversionEntry);
				}
			}
		}
		Action<Element, List<ContentContainer>> action2 = delegate(Element element, List<ContentContainer> containers)
		{
			if (element.highTempTransition != null || element.lowTempTransition != null)
			{
				containers.Add(new ContentContainer(new List<ICodexWidget>
				{
					new CodexText(CODEX.HEADERS.ELEMENTTRANSITIONS, CodexTextStyle.Subtitle),
					new CodexDividerLine()
				}, ContentContainer.ContentLayout.Vertical));
			}
			if (element.highTempTransition != null)
			{
				containers.Add(new ContentContainer(new List<ICodexWidget>
				{
					new CodexImage(32, 32, Def.GetUISprite(element.highTempTransition)),
					new CodexText((element.highTempTransition != null) ? (element.highTempTransition.name + " (" + element.highTempTransition.GetStateString() + ")  (" + GameUtil.GetFormattedTemperature(element.highTemp) + ")") : "")
				}, ContentContainer.ContentLayout.Horizontal));
			}
			if (element.lowTempTransition != null)
			{
				containers.Add(new ContentContainer(new List<ICodexWidget>
				{
					new CodexImage(32, 32, Def.GetUISprite(element.lowTempTransition)),
					new CodexText((element.lowTempTransition != null) ? (element.lowTempTransition.name + " (" + element.lowTempTransition.GetStateString() + ")  (" + GameUtil.GetFormattedTemperature(element.lowTemp) + ")") : "")
				}, ContentContainer.ContentLayout.Horizontal));
			}
			List<ICodexWidget> list4 = new List<ICodexWidget>();
			List<ICodexWidget> list5 = new List<ICodexWidget>();
			foreach (ComplexRecipe recipe in ComplexRecipeManager.Get().recipes)
			{
				if (recipe.ingredients.Any((ComplexRecipe.RecipeElement i) => i.material == element.tag))
				{
					list4.Add(new CodexRecipePanel(recipe));
				}
				if (recipe.results.Any((ComplexRecipe.RecipeElement i) => i.material == element.tag))
				{
					list5.Add(new CodexRecipePanel(recipe));
				}
			}
			if (usedMap2.map.TryGetValue(element.tag, out var value))
			{
				foreach (ConversionEntry item14 in value)
				{
					list4.Add(new CodexConversionPanel(item14.title, item14.inSet.ToArray(), item14.outSet.ToArray(), item14.prefab));
				}
			}
			if (madeMap.map.TryGetValue(element.tag, out var value2))
			{
				foreach (ConversionEntry item15 in value2)
				{
					list5.Add(new CodexConversionPanel(item15.title, item15.inSet.ToArray(), item15.outSet.ToArray(), item15.prefab));
				}
			}
			ContentContainer contentContainer = new ContentContainer(list4, ContentContainer.ContentLayout.Vertical);
			ContentContainer contentContainer2 = new ContentContainer(list5, ContentContainer.ContentLayout.Vertical);
			if (list4.Count > 0)
			{
				containers.Add(new ContentContainer(new List<ICodexWidget>
				{
					new CodexSpacer(),
					new CodexCollapsibleHeader(CODEX.HEADERS.ELEMENTCONSUMEDBY, contentContainer)
				}, ContentContainer.ContentLayout.Vertical));
				containers.Add(contentContainer);
			}
			if (list5.Count > 0)
			{
				containers.Add(new ContentContainer(new List<ICodexWidget>
				{
					new CodexSpacer(),
					new CodexCollapsibleHeader(CODEX.HEADERS.ELEMENTPRODUCEDBY, contentContainer2)
				}, ContentContainer.ContentLayout.Vertical));
				containers.Add(contentContainer2);
			}
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexText(element.FullDescription()),
				new CodexSpacer()
			}, ContentContainer.ContentLayout.Vertical));
		};
		string text7;
		foreach (Element element in ElementLoader.elements)
		{
			if (element.disabled)
			{
				continue;
			}
			List<ContentContainer> list = new List<ContentContainer>();
			string name = element.name + " (" + element.GetStateString() + ")";
			Tuple<Sprite, Color> tuple = Def.GetUISprite(element);
			if (tuple.first == null)
			{
				if (element.id == SimHashes.Void)
				{
					name = element.name;
					tuple = new Tuple<Sprite, Color>(Assets.GetSprite("ui_elements-void"), Color.white);
				}
				else if (element.id == SimHashes.Vacuum)
				{
					name = element.name;
					tuple = new Tuple<Sprite, Color>(Assets.GetSprite("ui_elements-vacuum"), Color.white);
				}
			}
			GenerateTitleContainers(name, list);
			GenerateImageContainers(new Tuple<Sprite, Color>[1] { tuple }, list, ContentContainer.ContentLayout.Horizontal);
			action2(element, list);
			text7 = element.id.ToString();
			string text8;
			Dictionary<string, CodexEntry> dictionary6;
			if (element.IsSolid)
			{
				text8 = text2;
				dictionary6 = dictionary2;
			}
			else if (element.IsLiquid)
			{
				text8 = text3;
				dictionary6 = dictionary3;
			}
			else if (element.IsGas)
			{
				text8 = text4;
				dictionary6 = dictionary4;
			}
			else
			{
				text8 = text5;
				dictionary6 = dictionary5;
			}
			CodexEntry codexEntry = new CodexEntry(text8, list, name);
			codexEntry.parentId = text8;
			codexEntry.icon = tuple.first;
			codexEntry.iconColor = tuple.second;
			CodexCache.AddEntry(text7, codexEntry);
			dictionary6.Add(text7, codexEntry);
		}
		text7 = text2;
		CodexEntry codexEntry2 = GenerateCategoryEntry(text7, UI.CODEX.CATEGORYNAMES.ELEMENTSSOLID, dictionary2, Assets.GetSprite("ui_elements-solid"));
		codexEntry2.parentId = text;
		codexEntry2.category = text;
		dictionary.Add(text7, codexEntry2);
		text7 = text3;
		codexEntry2 = GenerateCategoryEntry(text7, UI.CODEX.CATEGORYNAMES.ELEMENTSLIQUID, dictionary3, Assets.GetSprite("ui_elements-liquids"));
		codexEntry2.parentId = text;
		codexEntry2.category = text;
		dictionary.Add(text7, codexEntry2);
		text7 = text4;
		codexEntry2 = GenerateCategoryEntry(text7, UI.CODEX.CATEGORYNAMES.ELEMENTSGAS, dictionary4, Assets.GetSprite("ui_elements-gases"));
		codexEntry2.parentId = text;
		codexEntry2.category = text;
		dictionary.Add(text7, codexEntry2);
		text7 = text5;
		codexEntry2 = GenerateCategoryEntry(text7, UI.CODEX.CATEGORYNAMES.ELEMENTSOTHER, dictionary5, Assets.GetSprite("ui_elements-other"));
		codexEntry2.parentId = text;
		codexEntry2.category = text;
		dictionary.Add(text7, codexEntry2);
		Sprite sprite = Assets.GetSprite("ui_elements_classes");
		var obj = new[]
		{
			new
			{
				tag = GameTags.IceOre,
				checkPrefabs = false,
				solidOnly = false,
				spriteName = "ui_ice"
			},
			new
			{
				tag = GameTags.RefinedMetal,
				checkPrefabs = false,
				solidOnly = true,
				spriteName = "ui_refined_metal"
			},
			new
			{
				tag = GameTags.Filter,
				checkPrefabs = false,
				solidOnly = false,
				spriteName = "ui_filtration_medium"
			},
			new
			{
				tag = GameTags.Compostable,
				checkPrefabs = true,
				solidOnly = false,
				spriteName = "ui_compostable"
			},
			new
			{
				tag = GameTags.CombustibleLiquid,
				checkPrefabs = false,
				solidOnly = false,
				spriteName = "ui_combustible_liquids"
			}
		};
		Dictionary<string, CodexEntry> dictionary7 = new Dictionary<string, CodexEntry>();
		var array = obj;
		foreach (var anon in array)
		{
			string text9 = anon.tag.ToString();
			string name2 = Strings.Get("STRINGS.MISC.TAGS." + text9.ToUpper());
			List<ContentContainer> list2 = new List<ContentContainer>();
			GenerateTitleContainers(name2, list2);
			list2.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexText(Strings.Get("STRINGS.MISC.TAGS." + text9.ToUpper() + "_DESC")),
				new CodexSpacer()
			}, ContentContainer.ContentLayout.Vertical));
			List<ICodexWidget> list3 = new List<ICodexWidget>();
			if (anon.checkPrefabs)
			{
				foreach (GameObject item16 in Assets.GetPrefabsWithTag(anon.tag))
				{
					if (!item16.HasTag(GameTags.DeprecatedContent))
					{
						list3.Add(new CodexIndentedLabelWithIcon(item16.GetProperName(), CodexTextStyle.Body, Def.GetUISprite(item16)));
					}
				}
			}
			else
			{
				foreach (Element element2 in ElementLoader.elements)
				{
					if (element2.disabled || (anon.solidOnly && !element2.IsSolid))
					{
						continue;
					}
					bool flag = element2.materialCategory == anon.tag;
					Tag[] oreTags = element2.oreTags;
					for (int k = 0; k < oreTags.Length; k++)
					{
						if (oreTags[k] == anon.tag)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						list3.Add(new CodexIndentedLabelWithIcon(element2.name, CodexTextStyle.Body, Def.GetUISprite(element2.substance)));
					}
				}
			}
			list2.Add(new ContentContainer(list3, ContentContainer.ContentLayout.GridTwoColumn));
			CodexEntry codexEntry3 = new CodexEntry(text6, list2, name2);
			codexEntry3.parentId = text6;
			codexEntry3.icon = Assets.GetSprite(anon.spriteName);
			CodexCache.AddEntry(CodexCache.FormatLinkID(text9), codexEntry3);
			dictionary7.Add(text9, codexEntry3);
		}
		codexEntry2 = GenerateCategoryEntry(text6, UI.CODEX.CATEGORYNAMES.ELEMENTSCLASSES, dictionary7, sprite);
		codexEntry2.parentId = text;
		codexEntry2.category = text;
		dictionary.Add(text6, codexEntry2);
		PopulateCategoryEntries(dictionary);
		return dictionary;
	}

	public static Dictionary<string, CodexEntry> GenerateDiseaseEntries()
	{
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		foreach (Disease resource in Db.Get().Diseases.resources)
		{
			if (!resource.Disabled)
			{
				List<ContentContainer> list = new List<ContentContainer>();
				GenerateTitleContainers(resource.Name, list);
				GenerateDiseaseDescriptionContainers(resource, list);
				CodexEntry codexEntry = new CodexEntry("DISEASE", list, resource.Name);
				codexEntry.parentId = "DISEASE";
				dictionary.Add(resource.Id, codexEntry);
				codexEntry.icon = Assets.GetSprite("overlay_disease");
				CodexCache.AddEntry(resource.Id, codexEntry);
			}
		}
		return dictionary;
	}

	public static CategoryEntry GenerateCategoryEntry(string id, string name, Dictionary<string, CodexEntry> entries, Sprite icon = null, bool largeFormat = true, bool sort = true, string overrideHeader = null)
	{
		List<ContentContainer> list = new List<ContentContainer>();
		GenerateTitleContainers((overrideHeader == null) ? name : overrideHeader, list);
		List<CodexEntry> list2 = new List<CodexEntry>();
		foreach (KeyValuePair<string, CodexEntry> entry in entries)
		{
			list2.Add(entry.Value);
			if (icon == null)
			{
				icon = entry.Value.icon;
			}
		}
		CategoryEntry categoryEntry = new CategoryEntry("Root", list, name, list2, largeFormat, sort);
		categoryEntry.icon = icon;
		CodexCache.AddEntry(id, categoryEntry);
		return categoryEntry;
	}

	public static Dictionary<string, CodexEntry> GenerateTutorialNotificationEntries()
	{
		List<ContentContainer> list = new List<ContentContainer>();
		list.Add(new ContentContainer(new List<ICodexWidget>
		{
			new CodexSpacer()
		}, ContentContainer.ContentLayout.Vertical));
		CodexEntry codexEntry = new CodexEntry("MISCELLANEOUSTIPS", list, Strings.Get("STRINGS.UI.CODEX.CATEGORYNAMES.MISCELLANEOUSTIPS"));
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		for (int i = 0; i < 20; i++)
		{
			TutorialMessage tutorialMessage = (TutorialMessage)Tutorial.Instance.TutorialMessage((Tutorial.TutorialMessages)i, queueMessage: false);
			if (tutorialMessage != null && DlcManager.IsDlcListValidForCurrentContent(tutorialMessage.DLCIDs))
			{
				if (!string.IsNullOrEmpty(tutorialMessage.videoClipId))
				{
					List<ContentContainer> list2 = new List<ContentContainer>();
					GenerateTitleContainers(tutorialMessage.GetTitle(), list2);
					CodexVideo codexVideo = new CodexVideo();
					codexVideo.videoName = tutorialMessage.videoClipId;
					codexVideo.overlayName = tutorialMessage.videoOverlayName;
					codexVideo.overlayTexts = new List<string>
					{
						tutorialMessage.videoTitleText,
						VIDEOS.TUTORIAL_HEADER
					};
					list2.Add(new ContentContainer(new List<ICodexWidget> { codexVideo }, ContentContainer.ContentLayout.Vertical));
					list2.Add(new ContentContainer(new List<ICodexWidget>
					{
						new CodexText(tutorialMessage.GetMessageBody())
					}, ContentContainer.ContentLayout.Vertical));
					CodexEntry codexEntry2 = new CodexEntry("Videos", list2, UI.FormatAsLink(tutorialMessage.GetTitle(), "videos_" + i));
					codexEntry2.icon = Assets.GetSprite("codexVideo");
					CodexCache.AddEntry("videos_" + i, codexEntry2);
					dictionary.Add(codexEntry2.id, codexEntry2);
				}
				else
				{
					List<ContentContainer> list3 = new List<ContentContainer>();
					GenerateTitleContainers(tutorialMessage.GetTitle(), list3);
					list3.Add(new ContentContainer(new List<ICodexWidget>
					{
						new CodexText(tutorialMessage.GetMessageBody())
					}, ContentContainer.ContentLayout.Vertical));
					list3.Add(new ContentContainer(new List<ICodexWidget>
					{
						new CodexSpacer(),
						new CodexSpacer()
					}, ContentContainer.ContentLayout.Vertical));
					SubEntry item = new SubEntry("MISCELLANEOUSTIPS" + i, "MISCELLANEOUSTIPS", list3, tutorialMessage.GetTitle());
					codexEntry.subEntries.Add(item);
				}
			}
		}
		CodexCache.AddEntry("MISCELLANEOUSTIPS", codexEntry);
		return dictionary;
	}

	public static void PopulateCategoryEntries(Dictionary<string, CodexEntry> categoryEntries)
	{
		List<CategoryEntry> list = new List<CategoryEntry>();
		foreach (KeyValuePair<string, CodexEntry> categoryEntry in categoryEntries)
		{
			list.Add(categoryEntry.Value as CategoryEntry);
		}
		PopulateCategoryEntries(list);
	}

	public static void PopulateCategoryEntries(List<CategoryEntry> categoryEntries, Comparison<CodexEntry> comparison = null)
	{
		foreach (CategoryEntry categoryEntry in categoryEntries)
		{
			List<ContentContainer> contentContainers = categoryEntry.contentContainers;
			List<CodexEntry> list = new List<CodexEntry>();
			foreach (CodexEntry item3 in categoryEntry.entriesInCategory)
			{
				list.Add(item3);
			}
			if (categoryEntry.sort)
			{
				if (comparison == null)
				{
					list.Sort((CodexEntry a, CodexEntry b) => UI.StripLinkFormatting(a.name).CompareTo(UI.StripLinkFormatting(b.name)));
				}
				else
				{
					list.Sort(comparison);
				}
			}
			if (categoryEntry.largeFormat)
			{
				ContentContainer contentContainer = new ContentContainer(new List<ICodexWidget>(), ContentContainer.ContentLayout.Grid);
				foreach (CodexEntry item4 in list)
				{
					contentContainer.content.Add(new CodexLabelWithLargeIcon(item4.name, CodexTextStyle.BodyWhite, new Tuple<Sprite, Color>((item4.icon != null) ? item4.icon : Assets.GetSprite("unknown"), item4.iconColor), item4.id));
				}
				if (categoryEntry.showBeforeGeneratedCategoryLinks)
				{
					contentContainers.Add(contentContainer);
					continue;
				}
				ContentContainer item = contentContainers[contentContainers.Count - 1];
				contentContainers.RemoveAt(contentContainers.Count - 1);
				contentContainers.Insert(0, item);
				contentContainers.Insert(1, contentContainer);
				contentContainers.Insert(2, new ContentContainer(new List<ICodexWidget>
				{
					new CodexSpacer()
				}, ContentContainer.ContentLayout.Vertical));
				continue;
			}
			ContentContainer contentContainer2 = new ContentContainer(new List<ICodexWidget>(), ContentContainer.ContentLayout.Vertical);
			foreach (CodexEntry item5 in list)
			{
				if (item5.icon == null)
				{
					contentContainer2.content.Add(new CodexText(item5.name));
				}
				else
				{
					contentContainer2.content.Add(new CodexLabelWithIcon(item5.name, CodexTextStyle.Body, new Tuple<Sprite, Color>(item5.icon, item5.iconColor), 64, 48));
				}
			}
			if (categoryEntry.showBeforeGeneratedCategoryLinks)
			{
				contentContainers.Add(contentContainer2);
				continue;
			}
			ContentContainer item2 = contentContainers[contentContainers.Count - 1];
			contentContainers.RemoveAt(contentContainers.Count - 1);
			contentContainers.Insert(0, item2);
			contentContainers.Insert(1, contentContainer2);
		}
	}

	private static void GenerateTitleContainers(string name, List<ContentContainer> containers)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		list.Add(new CodexText(name, CodexTextStyle.Title));
		list.Add(new CodexDividerLine());
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	private static void GeneratePrerequisiteTechContainers(Tech tech, List<ContentContainer> containers)
	{
		if (tech.requiredTech == null || tech.requiredTech.Count == 0)
		{
			return;
		}
		List<ICodexWidget> list = new List<ICodexWidget>();
		list.Add(new CodexText(CODEX.HEADERS.PREREQUISITE_TECH, CodexTextStyle.Subtitle));
		list.Add(new CodexDividerLine());
		list.Add(new CodexSpacer());
		foreach (Tech item in tech.requiredTech)
		{
			list.Add(new CodexText(item.Name));
		}
		list.Add(new CodexSpacer());
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	private static void GenerateSkillRequirementsAndPerksContainers(Skill skill, List<ContentContainer> containers)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		CodexText item = new CodexText(CODEX.HEADERS.ROLE_PERKS, CodexTextStyle.Subtitle);
		CodexText item2 = new CodexText(CODEX.HEADERS.ROLE_PERKS_DESC);
		list.Add(item);
		list.Add(new CodexDividerLine());
		list.Add(item2);
		list.Add(new CodexSpacer());
		foreach (SkillPerk perk in skill.perks)
		{
			CodexText item3 = new CodexText(perk.Name);
			list.Add(item3);
		}
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
		list.Add(new CodexSpacer());
	}

	private static void GenerateRelatedSkillContainers(Skill skill, List<ContentContainer> containers)
	{
		bool flag = false;
		List<ICodexWidget> list = new List<ICodexWidget>();
		CodexText item = new CodexText(CODEX.HEADERS.PREREQUISITE_ROLES, CodexTextStyle.Subtitle);
		list.Add(item);
		list.Add(new CodexDividerLine());
		list.Add(new CodexSpacer());
		foreach (string priorSkill in skill.priorSkills)
		{
			CodexText item2 = new CodexText(Db.Get().Skills.Get(priorSkill).Name);
			list.Add(item2);
			flag = true;
		}
		if (flag)
		{
			list.Add(new CodexSpacer());
			containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
		}
		bool flag2 = false;
		List<ICodexWidget> list2 = new List<ICodexWidget>();
		CodexText item3 = new CodexText(CODEX.HEADERS.UNLOCK_ROLES, CodexTextStyle.Subtitle);
		CodexText item4 = new CodexText(CODEX.HEADERS.UNLOCK_ROLES_DESC);
		list2.Add(item3);
		list2.Add(new CodexDividerLine());
		list2.Add(item4);
		list2.Add(new CodexSpacer());
		foreach (Skill resource in Db.Get().Skills.resources)
		{
			if (resource.deprecated)
			{
				continue;
			}
			foreach (string priorSkill2 in resource.priorSkills)
			{
				if (priorSkill2 == skill.Id)
				{
					CodexText item5 = new CodexText(resource.Name);
					list2.Add(item5);
					flag2 = true;
				}
			}
		}
		if (flag2)
		{
			list2.Add(new CodexSpacer());
			containers.Add(new ContentContainer(list2, ContentContainer.ContentLayout.Vertical));
		}
	}

	private static void GenerateUnlockContainers(Tech tech, List<ContentContainer> containers)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		CodexText item = new CodexText(CODEX.HEADERS.TECH_UNLOCKS, CodexTextStyle.Subtitle);
		list.Add(item);
		list.Add(new CodexDividerLine());
		list.Add(new CodexSpacer());
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
		foreach (TechItem unlockedItem in tech.unlockedItems)
		{
			List<ICodexWidget> list2 = new List<ICodexWidget>();
			CodexImage item2 = new CodexImage(64, 64, unlockedItem.getUISprite("ui", arg2: false));
			list2.Add(item2);
			CodexText item3 = new CodexText(unlockedItem.Name);
			list2.Add(item3);
			containers.Add(new ContentContainer(list2, ContentContainer.ContentLayout.Horizontal));
		}
	}

	private static void GenerateRecipeContainers(Tag prefabID, List<ContentContainer> containers)
	{
		Recipe recipe = null;
		foreach (Recipe recipe2 in RecipeManager.Get().recipes)
		{
			if (recipe2.Result == prefabID)
			{
				recipe = recipe2;
				break;
			}
		}
		if (recipe == null)
		{
			return;
		}
		containers.Add(new ContentContainer(new List<ICodexWidget>
		{
			new CodexText(CODEX.HEADERS.RECIPE, CodexTextStyle.Subtitle),
			new CodexSpacer(),
			new CodexDividerLine()
		}, ContentContainer.ContentLayout.Vertical));
		Func<Recipe, List<ContentContainer>> func = delegate(Recipe rec)
		{
			List<ContentContainer> list = new List<ContentContainer>();
			foreach (Recipe.Ingredient ingredient in rec.Ingredients)
			{
				GameObject prefab = Assets.GetPrefab(ingredient.tag);
				if (prefab != null)
				{
					list.Add(new ContentContainer(new List<ICodexWidget>
					{
						new CodexImage(64, 64, Def.GetUISprite(prefab)),
						new CodexText(string.Format(UI.CODEX.RECIPE_ITEM, Assets.GetPrefab(ingredient.tag).GetProperName(), ingredient.amount, (ElementLoader.GetElement(ingredient.tag) == null) ? "" : UI.UNITSUFFIXES.MASS.KILOGRAM.text))
					}, ContentContainer.ContentLayout.Horizontal));
				}
			}
			return list;
		};
		containers.AddRange(func(recipe));
		GameObject gameObject = ((recipe.fabricators == null) ? null : Assets.GetPrefab(recipe.fabricators[0]));
		if (gameObject != null)
		{
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexText(UI.CODEX.RECIPE_FABRICATOR_HEADER, CodexTextStyle.Subtitle),
				new CodexDividerLine()
			}, ContentContainer.ContentLayout.Vertical));
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexImage(64, 64, Def.GetUISpriteFromMultiObjectAnim(gameObject.GetComponent<KBatchedAnimController>().AnimFiles[0])),
				new CodexText(string.Format(UI.CODEX.RECIPE_FABRICATOR, recipe.FabricationTime, gameObject.GetProperName()))
			}, ContentContainer.ContentLayout.Horizontal));
		}
	}

	private static void GenerateUsedInRecipeContainers(Tag prefabID, List<ContentContainer> containers)
	{
		List<Recipe> list = new List<Recipe>();
		foreach (Recipe recipe in RecipeManager.Get().recipes)
		{
			foreach (Recipe.Ingredient ingredient in recipe.Ingredients)
			{
				if (ingredient.tag == prefabID)
				{
					list.Add(recipe);
				}
			}
		}
		if (list.Count == 0)
		{
			return;
		}
		containers.Add(new ContentContainer(new List<ICodexWidget>
		{
			new CodexText(CODEX.HEADERS.USED_IN_RECIPES, CodexTextStyle.Subtitle),
			new CodexSpacer(),
			new CodexDividerLine()
		}, ContentContainer.ContentLayout.Vertical));
		foreach (Recipe item in list)
		{
			GameObject prefab = Assets.GetPrefab(item.Result);
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexImage(64, 64, Def.GetUISprite(prefab)),
				new CodexText(prefab.GetProperName())
			}, ContentContainer.ContentLayout.Horizontal));
		}
	}

	private static void GeneratePlantDescriptionContainers(GameObject plant, List<ContentContainer> containers)
	{
		SeedProducer component = plant.GetComponent<SeedProducer>();
		if (component != null)
		{
			GameObject prefab = Assets.GetPrefab(component.seedInfo.seedId);
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexText(CODEX.HEADERS.GROWNFROMSEED, CodexTextStyle.Subtitle),
				new CodexDividerLine()
			}, ContentContainer.ContentLayout.Vertical));
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexImage(48, 48, Def.GetUISprite(prefab)),
				new CodexText(prefab.GetProperName())
			}, ContentContainer.ContentLayout.Horizontal));
		}
		List<ICodexWidget> list = new List<ICodexWidget>();
		list.Add(new CodexSpacer());
		list.Add(new CodexText(UI.CODEX.DETAILS, CodexTextStyle.Subtitle));
		list.Add(new CodexDividerLine());
		InfoDescription component2 = Assets.GetPrefab(plant.PrefabID()).GetComponent<InfoDescription>();
		if (component2 != null)
		{
			list.Add(new CodexText(component2.description));
		}
		string text = "";
		List<Descriptor> plantRequirementDescriptors = GameUtil.GetPlantRequirementDescriptors(plant);
		if (plantRequirementDescriptors.Count > 0)
		{
			text += plantRequirementDescriptors[0].text;
			for (int i = 1; i < plantRequirementDescriptors.Count; i++)
			{
				text = text + "\n    • " + plantRequirementDescriptors[i].text;
			}
			list.Add(new CodexText(text));
			list.Add(new CodexSpacer());
		}
		text = "";
		List<Descriptor> plantEffectDescriptors = GameUtil.GetPlantEffectDescriptors(plant);
		if (plantEffectDescriptors.Count > 0)
		{
			text += plantEffectDescriptors[0].text;
			for (int j = 1; j < plantEffectDescriptors.Count; j++)
			{
				text = text + "\n    • " + plantEffectDescriptors[j].text;
			}
			CodexText item = new CodexText(text);
			list.Add(item);
			list.Add(new CodexSpacer());
		}
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	private static ICodexWidget GetIconWidget(object entity)
	{
		return new CodexImage(32, 32, Def.GetUISprite(entity));
	}

	private static void GenerateCreatureDescriptionContainers(GameObject creature, List<ContentContainer> containers)
	{
		RobotBatteryMonitor.Def def = creature.GetDef<RobotBatteryMonitor.Def>();
		if (def != null)
		{
			Amount batteryAmount = Db.Get().Amounts.Get(def.batteryAmountId);
			float value = Db.Get().traits.Get(creature.GetComponent<Modifiers>().initialTraits[0]).SelfModifiers.Find((AttributeModifier match) => match.AttributeId == batteryAmount.maxAttribute.Id).Value;
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexText(CODEX.HEADERS.INTERNALBATTERY, CodexTextStyle.Subtitle),
				new CodexText("    • " + string.Format(CODEX.ROBOT_DESCRIPTORS.BATTERY.CAPACITY, value))
			}, ContentContainer.ContentLayout.Vertical));
		}
		if (creature.GetDef<StorageUnloadMonitor.Def>() != null)
		{
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexText(CODEX.HEADERS.INTERNALSTORAGE, CodexTextStyle.Subtitle),
				new CodexText("    • " + string.Format(CODEX.ROBOT_DESCRIPTORS.STORAGE.CAPACITY, creature.GetComponents<Storage>()[1].Capacity()))
			}, ContentContainer.ContentLayout.Vertical));
		}
		List<GameObject> prefabsWithTag = Assets.GetPrefabsWithTag((creature.PrefabID().ToString() + "Egg").ToTag());
		if (prefabsWithTag != null && prefabsWithTag.Count > 0)
		{
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexText(CODEX.HEADERS.HATCHESFROMEGG, CodexTextStyle.Subtitle)
			}, ContentContainer.ContentLayout.Vertical));
			foreach (GameObject item in prefabsWithTag)
			{
				containers.Add(new ContentContainer(new List<ICodexWidget>
				{
					new CodexIndentedLabelWithIcon(item.GetProperName(), CodexTextStyle.Body, Def.GetUISprite(item))
				}, ContentContainer.ContentLayout.Horizontal));
			}
		}
		TemperatureVulnerable component = creature.GetComponent<TemperatureVulnerable>();
		if (component != null)
		{
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexText(CODEX.HEADERS.COMFORTRANGE, CodexTextStyle.Subtitle),
				new CodexText("    • " + string.Format(CODEX.CREATURE_DESCRIPTORS.TEMPERATURE.COMFORT_RANGE, GameUtil.GetFormattedTemperature(component.TemperatureWarningLow), GameUtil.GetFormattedTemperature(component.TemperatureWarningHigh))),
				new CodexText("    • " + string.Format(CODEX.CREATURE_DESCRIPTORS.TEMPERATURE.NON_LETHAL_RANGE, GameUtil.GetFormattedTemperature(component.TemperatureLethalLow), GameUtil.GetFormattedTemperature(component.TemperatureLethalHigh)))
			}, ContentContainer.ContentLayout.Vertical));
		}
		new List<Tag>();
		CreatureCalorieMonitor.Def def2 = creature.GetDef<CreatureCalorieMonitor.Def>();
		BeehiveCalorieMonitor.Def def3 = creature.GetDef<BeehiveCalorieMonitor.Def>();
		Diet.Info[] array = null;
		if (def2 != null)
		{
			array = def2.diet.infos;
		}
		else if (def3 != null)
		{
			array = def3.diet.infos;
		}
		if (array == null || array.Length == 0)
		{
			return;
		}
		float num = 0f;
		foreach (AttributeModifier selfModifier in Db.Get().traits.Get(creature.GetComponent<Modifiers>().initialTraits[0]).SelfModifiers)
		{
			if (selfModifier.AttributeId == Db.Get().Amounts.Calories.deltaAttribute.Id)
			{
				num = selfModifier.Value;
			}
		}
		List<ICodexWidget> list = new List<ICodexWidget>();
		Diet.Info[] array2 = array;
		foreach (Diet.Info info in array2)
		{
			if (info.consumedTags.Count == 0)
			{
				continue;
			}
			foreach (Tag consumedTag in info.consumedTags)
			{
				Element element = ElementLoader.FindElementByHash(ElementLoader.GetElementID(consumedTag));
				if ((element.id != SimHashes.Vacuum && element.id != SimHashes.Void) || !(Assets.GetPrefab(consumedTag) == null))
				{
					float num2 = (0f - num) / info.caloriesPerKg;
					float outputAmount = num2 * info.producedConversionRate;
					list.Add(new CodexConversionPanel(consumedTag.ProperName(), consumedTag, num2, inputContinuous: true, info.producedElement, outputAmount, outputContinuous: true, creature));
				}
			}
		}
		ContentContainer contentContainer = new ContentContainer(list, ContentContainer.ContentLayout.Vertical);
		containers.Add(new ContentContainer(new List<ICodexWidget>
		{
			new CodexSpacer(),
			new CodexCollapsibleHeader(CODEX.HEADERS.DIET, contentContainer)
		}, ContentContainer.ContentLayout.Vertical));
		containers.Add(contentContainer);
		containers.Add(new ContentContainer(new List<ICodexWidget>
		{
			new CodexSpacer(),
			new CodexSpacer()
		}, ContentContainer.ContentLayout.Vertical));
	}

	private static void GenerateDiseaseDescriptionContainers(Disease disease, List<ContentContainer> containers)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		list.Add(new CodexSpacer());
		foreach (Descriptor quantitativeDescriptor in disease.GetQuantitativeDescriptors())
		{
			list.Add(new CodexText(quantitativeDescriptor.text));
		}
		list.Add(new CodexSpacer());
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	private static void GenerateFoodDescriptionContainers(EdiblesManager.FoodInfo food, List<ContentContainer> containers)
	{
		containers.Add(new ContentContainer(new List<ICodexWidget>
		{
			new CodexText(food.Description),
			new CodexSpacer(),
			new CodexText(string.Format(UI.CODEX.FOOD.QUALITY, GameUtil.GetFormattedFoodQuality(food.Quality))),
			new CodexText(string.Format(UI.CODEX.FOOD.CALORIES, GameUtil.GetFormattedCalories(food.CaloriesPerUnit))),
			new CodexSpacer(),
			new CodexText(food.CanRot ? string.Format(UI.CODEX.FOOD.SPOILPROPERTIES, GameUtil.GetFormattedTemperature(food.RotTemperature), GameUtil.GetFormattedTemperature(food.PreserveTemperature), GameUtil.GetFormattedCycles(food.SpoilTime)) : UI.CODEX.FOOD.NON_PERISHABLE.ToString()),
			new CodexSpacer()
		}, ContentContainer.ContentLayout.Vertical));
	}

	private static void GenerateTechDescriptionContainers(Tech tech, List<ContentContainer> containers)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		CodexText item = new CodexText(Strings.Get("STRINGS.RESEARCH.TECHS." + tech.Id.ToUpper() + ".DESC"));
		list.Add(item);
		list.Add(new CodexSpacer());
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	private static void GenerateGenericDescriptionContainers(string description, List<ContentContainer> containers)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		CodexText item = new CodexText(description);
		list.Add(item);
		list.Add(new CodexSpacer());
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	private static void GenerateBuildingDescriptionContainers(BuildingDef def, List<ContentContainer> containers)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		list.Add(new CodexText(Strings.Get("STRINGS.BUILDINGS.PREFABS." + def.PrefabID.ToUpper() + ".EFFECT")));
		list.Add(new CodexSpacer());
		List<Descriptor> allDescriptors = GameUtil.GetAllDescriptors(def.BuildingComplete);
		List<Descriptor> requirementDescriptors = GameUtil.GetRequirementDescriptors(allDescriptors);
		if (requirementDescriptors.Count > 0)
		{
			list.Add(new CodexText(CODEX.HEADERS.BUILDINGREQUIREMENTS, CodexTextStyle.Subtitle));
			foreach (Descriptor item in requirementDescriptors)
			{
				list.Add(new CodexTextWithTooltip("    " + item.text, item.tooltipText));
			}
			list.Add(new CodexSpacer());
		}
		List<Descriptor> effectDescriptors = GameUtil.GetEffectDescriptors(allDescriptors);
		if (effectDescriptors.Count > 0)
		{
			list.Add(new CodexText(CODEX.HEADERS.BUILDINGEFFECTS, CodexTextStyle.Subtitle));
			foreach (Descriptor item2 in effectDescriptors)
			{
				list.Add(new CodexTextWithTooltip("    " + item2.text, item2.tooltipText));
			}
			list.Add(new CodexSpacer());
		}
		KPrefabID component = def.BuildingComplete.GetComponent<KPrefabID>();
		Pair<Tag, string>[] array = new Pair<Tag, string>[8]
		{
			new Pair<Tag, string>(RoomConstraints.ConstraintTags.IndustrialMachinery, CODEX.BUILDING_TYPE.INDUSTRIAL_MACHINERY),
			new Pair<Tag, string>(RoomConstraints.ConstraintTags.RecBuilding, ROOMS.CRITERIA.REC_BUILDING.NAME),
			new Pair<Tag, string>(RoomConstraints.ConstraintTags.Clinic, ROOMS.CRITERIA.CLINIC.NAME),
			new Pair<Tag, string>(RoomConstraints.ConstraintTags.WashStation, ROOMS.CRITERIA.WASH_STATION.NAME),
			new Pair<Tag, string>(RoomConstraints.ConstraintTags.AdvancedWashStation, ROOMS.CRITERIA.ADVANCED_WASH_STATION.NAME),
			new Pair<Tag, string>(RoomConstraints.ConstraintTags.Toilet, ROOMS.CRITERIA.TOILET.NAME),
			new Pair<Tag, string>(RoomConstraints.ConstraintTags.FlushToilet, ROOMS.CRITERIA.FLUSH_TOILET.NAME),
			new Pair<Tag, string>(GameTags.Decoration, ROOMS.CRITERIA.DECORATIVE_ITEM.NAME)
		};
		bool flag = false;
		Pair<Tag, string>[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Pair<Tag, string> pair = array2[i];
			if (component.HasTag(pair.first))
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			list.Add(new CodexText(CODEX.HEADERS.BUILDINGTYPE, CodexTextStyle.Subtitle));
			array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Pair<Tag, string> pair2 = array2[i];
				if (component.HasTag(pair2.first))
				{
					list.Add(new CodexText("    " + pair2.second));
				}
			}
			list.Add(new CodexSpacer());
		}
		list.Add(new CodexText(string.Concat("<i>", Strings.Get("STRINGS.BUILDINGS.PREFABS." + def.PrefabID.ToUpper() + ".DESC"), "</i>")));
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	private static void GenerateImageContainers(Sprite[] sprites, List<ContentContainer> containers, ContentContainer.ContentLayout layout)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		foreach (Sprite sprite in sprites)
		{
			if (!(sprite == null))
			{
				CodexImage item = new CodexImage(128, 128, sprite);
				list.Add(item);
			}
		}
		containers.Add(new ContentContainer(list, layout));
	}

	private static void GenerateImageContainers(Tuple<Sprite, Color>[] sprites, List<ContentContainer> containers, ContentContainer.ContentLayout layout)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		foreach (Tuple<Sprite, Color> tuple in sprites)
		{
			if (tuple != null)
			{
				CodexImage item = new CodexImage(128, 128, tuple);
				list.Add(item);
			}
		}
		containers.Add(new ContentContainer(list, layout));
	}

	private static void GenerateImageContainers(Sprite sprite, List<ContentContainer> containers)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		CodexImage item = new CodexImage(128, 128, sprite);
		list.Add(item);
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	public static void CreateUnlockablesContentContainer(SubEntry subentry)
	{
		ContentContainer contentContainer = new ContentContainer(new List<ICodexWidget>
		{
			new CodexText(CODEX.HEADERS.SECTION_UNLOCKABLES, CodexTextStyle.Subtitle),
			new CodexDividerLine()
		}, ContentContainer.ContentLayout.Vertical);
		contentContainer.showBeforeGeneratedContent = false;
		subentry.lockedContentContainer = contentContainer;
	}

	private static void GenerateFabricatorContainers(GameObject entity, List<ContentContainer> containers)
	{
		ComplexFabricator component = entity.GetComponent<ComplexFabricator>();
		if (!(component == null))
		{
			List<ICodexWidget> list = new List<ICodexWidget>();
			list.Add(new CodexSpacer());
			list.Add(new CodexText(Strings.Get("STRINGS.CODEX.HEADERS.FABRICATIONS"), CodexTextStyle.Subtitle));
			list.Add(new CodexDividerLine());
			containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
			List<ICodexWidget> list2 = new List<ICodexWidget>();
			ComplexRecipe[] recipes = component.GetRecipes();
			foreach (ComplexRecipe recipe in recipes)
			{
				list2.Add(new CodexRecipePanel(recipe));
			}
			containers.Add(new ContentContainer(list2, ContentContainer.ContentLayout.Vertical));
		}
	}

	private static void GenerateReceptacleContainers(GameObject entity, List<ContentContainer> containers)
	{
		SingleEntityReceptacle plot = entity.GetComponent<SingleEntityReceptacle>();
		if (plot == null)
		{
			return;
		}
		List<ICodexWidget> list = new List<ICodexWidget>();
		list.Add(new CodexText(Strings.Get("STRINGS.CODEX.HEADERS.RECEPTACLE"), CodexTextStyle.Subtitle));
		list.Add(new CodexDividerLine());
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
		Tag[] possibleDepositObjectTags = plot.possibleDepositObjectTags;
		for (int i = 0; i < possibleDepositObjectTags.Length; i++)
		{
			List<GameObject> prefabsWithTag = Assets.GetPrefabsWithTag(possibleDepositObjectTags[i]);
			if (plot.rotatable == null)
			{
				prefabsWithTag.RemoveAll(delegate(GameObject go)
				{
					IReceptacleDirection component = go.GetComponent<IReceptacleDirection>();
					return component != null && component.Direction != plot.Direction;
				});
			}
			foreach (GameObject item in prefabsWithTag)
			{
				List<ICodexWidget> list2 = new List<ICodexWidget>();
				list2.Add(new CodexImage(64, 64, Def.GetUISprite(item).first));
				list2.Add(new CodexText(item.GetProperName()));
				containers.Add(new ContentContainer(list2, ContentContainer.ContentLayout.Horizontal));
			}
		}
	}
}
