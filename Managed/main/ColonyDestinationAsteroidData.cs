using System.Collections.Generic;
using ProcGen;
using STRINGS;
using UnityEngine;

public class ColonyDestinationAsteroidData
{
	private ProcGen.World world;

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

	public string worldPath => world.filePath;

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

	public string properName => Strings.Get(world.name);

	public ColonyDestinationAsteroidData(string worldName, int seed)
	{
		Scale = 1f;
		TargetScale = 1f;
		world = SettingsCache.worlds.GetWorldData(worldName);
		ReInitialize(seed);
	}

	public void ReInitialize(int seed)
	{
		this.seed = seed;
		paramDescriptors.Clear();
		traitDescriptors.Clear();
		sprite = Db.Get().AsteroidTypes.GetTypeOrDefault(world.asteroidType).GetUISprite();
		difficulty = world.difficulty;
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
		list.Add(new AsteroidDescriptor(string.Format(WORLDS.SURVIVAL_CHANCE.PLANETNAME, properName), null));
		list.Add(new AsteroidDescriptor(Strings.Get(world.description), null));
		int index = Mathf.Clamp(difficulty, 0, survivalOptions.Count - 1);
		Tuple<string, string, string> tuple = survivalOptions[index];
		list.Add(new AsteroidDescriptor(string.Format(WORLDS.SURVIVAL_CHANCE.TITLE, tuple.first, tuple.third), null));
		return list;
	}

	private List<AsteroidDescriptor> GenerateTraitDescriptors()
	{
		List<AsteroidDescriptor> list = new List<AsteroidDescriptor>();
		if (world.disableWorldTraits)
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
