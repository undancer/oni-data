using System.Collections.Generic;
using ProcGen;
using STRINGS;
using UnityEngine;

public class ColonyDestinationAsteroidBeltData
{
	private ProcGen.World startWorld;

	private ClusterLayout cluster;

	private List<AsteroidDescriptor> paramDescriptors = new List<AsteroidDescriptor>();

	private List<AsteroidDescriptor> traitDescriptors = new List<AsteroidDescriptor>();

	private static List<Tuple<string, string, string>> survivalOptions = new List<Tuple<string, string, string>>
	{
		new Tuple<string, string, string>(WORLDS.SURVIVAL_CHANCE.MOSTHOSPITABLE, "", "D2F40C"),
		new Tuple<string, string, string>(WORLDS.SURVIVAL_CHANCE.VERYHIGH, "", "7DE419"),
		new Tuple<string, string, string>(WORLDS.SURVIVAL_CHANCE.HIGH, "", "36D246"),
		new Tuple<string, string, string>(WORLDS.SURVIVAL_CHANCE.NEUTRAL, "", "63C2B7"),
		new Tuple<string, string, string>(WORLDS.SURVIVAL_CHANCE.LOW, "", "6A8EB1"),
		new Tuple<string, string, string>(WORLDS.SURVIVAL_CHANCE.VERYLOW, "", "937890"),
		new Tuple<string, string, string>(WORLDS.SURVIVAL_CHANCE.LEASTHOSPITABLE, "", "9636DF")
	};

	public float TargetScale
	{
		get;
		set;
	}

	public float Scale
	{
		get;
		set;
	}

	public int seed
	{
		get;
		private set;
	}

	public string startWorldPath => startWorld.filePath;

	public Sprite sprite
	{
		get;
		private set;
	}

	public int difficulty
	{
		get;
		private set;
	}

	public string startWorldName => Strings.Get(startWorld.name);

	public string properName => (cluster != null) ? cluster.name : "";

	public string beltPath => (cluster != null) ? cluster.filePath : WorldGenSettings.ClusterDefaultName;

	public List<ProcGen.World> worlds
	{
		get;
		private set;
	}

	public ProcGen.World GetStartWorld => startWorld;

	public ColonyDestinationAsteroidBeltData(string staringWorldName, int seed, string clusterPath)
	{
		Scale = 1f;
		TargetScale = 1f;
		startWorld = SettingsCache.worlds.GetWorldData(staringWorldName);
		worlds = new List<ProcGen.World>();
		if (clusterPath != null)
		{
			cluster = SettingsCache.clusterLayouts.GetClusterData(clusterPath);
			for (int i = 0; i < cluster.worldPlacements.Count; i++)
			{
				if (i != cluster.startWorldIndex)
				{
					worlds.Add(SettingsCache.worlds.GetWorldData(cluster.worldPlacements[i].world));
				}
			}
		}
		ReInitialize(seed);
	}

	private Sprite GetUISprite(string filename)
	{
		if (filename.IsNullOrWhiteSpace())
		{
			filename = (DlcManager.IsExpansion1Active() ? "asteroid_sandstone_start_kanim" : "Asteroid_sandstone");
		}
		Assets.TryGetAnim(filename, out var anim);
		if (anim != null)
		{
			return Def.GetUISpriteFromMultiObjectAnim(anim);
		}
		return Assets.GetSprite(filename);
	}

	public void ReInitialize(int seed)
	{
		this.seed = seed;
		paramDescriptors.Clear();
		traitDescriptors.Clear();
		sprite = GetUISprite(startWorld.asteroidIcon);
		difficulty = cluster.difficulty;
	}

	public List<AsteroidDescriptor> GetParamDescriptors()
	{
		if (paramDescriptors.Count == 0)
		{
			paramDescriptors = GenerateParamDescriptors();
		}
		return paramDescriptors;
	}

	public List<AsteroidDescriptor> GetTraitDescriptors()
	{
		if (traitDescriptors.Count == 0)
		{
			traitDescriptors = GenerateTraitDescriptors();
		}
		return traitDescriptors;
	}

	private List<AsteroidDescriptor> GenerateParamDescriptors()
	{
		List<AsteroidDescriptor> list = new List<AsteroidDescriptor>();
		if (cluster != null && DlcManager.IsExpansion1Active())
		{
			list.Add(new AsteroidDescriptor(string.Format(WORLDS.SURVIVAL_CHANCE.CLUSTERNAME, Strings.Get(cluster.name)), Strings.Get(cluster.description)));
		}
		list.Add(new AsteroidDescriptor(string.Format(WORLDS.SURVIVAL_CHANCE.PLANETNAME, startWorldName), null));
		list.Add(new AsteroidDescriptor(Strings.Get(startWorld.description), null));
		if (DlcManager.IsExpansion1Active())
		{
			list.Add(new AsteroidDescriptor(string.Format(WORLDS.SURVIVAL_CHANCE.MOONNAMES), null));
			foreach (ProcGen.World world in worlds)
			{
				list.Add(new AsteroidDescriptor($"{Strings.Get(world.name)}", Strings.Get(world.description)));
			}
		}
		int index = Mathf.Clamp(difficulty, 0, survivalOptions.Count - 1);
		Tuple<string, string, string> tuple = survivalOptions[index];
		list.Add(new AsteroidDescriptor(string.Format(WORLDS.SURVIVAL_CHANCE.TITLE, tuple.first, tuple.third), null));
		return list;
	}

	private List<AsteroidDescriptor> GenerateTraitDescriptors()
	{
		List<AsteroidDescriptor> list = new List<AsteroidDescriptor>();
		if (startWorld.disableWorldTraits)
		{
			list.Add(new AsteroidDescriptor(WORLD_TRAITS.NO_TRAITS.NAME, WORLD_TRAITS.NO_TRAITS.DESCRIPTION));
		}
		else
		{
			List<string> randomTraits = SettingsCache.GetRandomTraits(seed);
			foreach (string item in randomTraits)
			{
				WorldTrait cachedTrait = SettingsCache.GetCachedTrait(item, assertMissingTrait: true);
				list.Add(new AsteroidDescriptor(string.Format("<color=#{1}>{0}</color>", Strings.Get(cachedTrait.name), cachedTrait.colorHex), Strings.Get(cachedTrait.description)));
			}
		}
		return list;
	}
}
