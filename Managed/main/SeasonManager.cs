using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/SeasonManager")]
public class SeasonManager : KMonoBehaviour, ISim200ms
{
	private struct BombardmentInfo
	{
		public string prefab;

		public float weight;
	}

	private struct Season
	{
		public string name;

		public int durationInCycles;

		public MathUtil.MinMax secondsBombardmentOff;

		public MathUtil.MinMax secondsBombardmentOn;

		public MathUtil.MinMax secondsBetweenBombardments;

		public bool meteorBackground;

		public BombardmentInfo[] bombardmentInfo;
	}

	[Serialize]
	private int currentSeasonIndex = int.MaxValue;

	[Serialize]
	private int currentSeasonsCyclesElapsed = int.MaxValue;

	[Serialize]
	private float bombardmentPeriodRemaining;

	[Serialize]
	private bool bombardmentOn;

	[Serialize]
	private float secondsUntilNextBombardment;

	private GameObject activeMeteorBackground;

	private const string SEASONNAME_DEFAULT = "Default";

	private const string SEASONNAME_METEORSHOWER_IRON = "MeteorShowerIron";

	private const string SEASONNAME_METEORSHOWER_GOLD = "MeteorShowerGold";

	private const string SEASONNAME_METEORSHOWER_COPPER = "MeteorShowerCopper";

	private Dictionary<string, Season> seasons;

	private string[] SeasonLoop;

	private static readonly EventSystem.IntraObjectHandler<SeasonManager> OnNewDayDelegate = new EventSystem.IntraObjectHandler<SeasonManager>(delegate(SeasonManager component, object data)
	{
		component.OnNewDay(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(631075836, OnNewDayDelegate);
		if (currentSeasonIndex >= SeasonLoop.Length)
		{
			currentSeasonIndex = SeasonLoop.Length - 1;
			currentSeasonsCyclesElapsed = int.MaxValue;
		}
		UpdateState();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Unsubscribe(631075836, OnNewDayDelegate);
	}

	private void OnNewDay(object data)
	{
		currentSeasonsCyclesElapsed++;
		UpdateState();
	}

	private void UpdateState()
	{
		Season season = seasons[SeasonLoop[currentSeasonIndex]];
		if (currentSeasonsCyclesElapsed >= season.durationInCycles)
		{
			currentSeasonIndex = (currentSeasonIndex + 1) % SeasonLoop.Length;
			ResetSeasonProgress();
		}
	}

	private void ResetSeasonProgress()
	{
		Season season = seasons[SeasonLoop[currentSeasonIndex]];
		currentSeasonsCyclesElapsed = 0;
		bombardmentOn = false;
		bombardmentPeriodRemaining = season.secondsBombardmentOff.Get();
		secondsUntilNextBombardment = season.secondsBetweenBombardments.Get();
	}

	public void Sim200ms(float dt)
	{
		Season season = seasons[SeasonLoop[currentSeasonIndex]];
		bombardmentPeriodRemaining -= dt;
		if (bombardmentPeriodRemaining <= 0f)
		{
			float num = bombardmentPeriodRemaining;
			bombardmentOn = !bombardmentOn;
			bombardmentPeriodRemaining = (bombardmentOn ? season.secondsBombardmentOn.Get() : season.secondsBombardmentOff.Get());
			if (bombardmentPeriodRemaining != 0f)
			{
				bombardmentPeriodRemaining += num;
			}
		}
		if (bombardmentOn && season.bombardmentInfo != null && season.bombardmentInfo.Length != 0)
		{
			if (activeMeteorBackground == null)
			{
				activeMeteorBackground = Util.KInstantiate(EffectPrefabs.Instance.MeteorBackground);
				activeMeteorBackground.transform.SetPosition(new Vector3(125f, 435f, 25f));
				activeMeteorBackground.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
			}
			secondsUntilNextBombardment -= dt;
			if (secondsUntilNextBombardment <= 0f)
			{
				float num2 = secondsUntilNextBombardment;
				DoBombardment(season.bombardmentInfo);
				secondsUntilNextBombardment = season.secondsBetweenBombardments.Get();
				if (secondsUntilNextBombardment != 0f)
				{
					secondsUntilNextBombardment += num2;
				}
			}
		}
		else if (activeMeteorBackground != null)
		{
			ParticleSystem component = activeMeteorBackground.GetComponent<ParticleSystem>();
			component.Stop();
			if (!component.IsAlive())
			{
				UnityEngine.Object.Destroy(activeMeteorBackground);
				activeMeteorBackground = null;
			}
		}
	}

	private void DoBombardment(BombardmentInfo[] bombardment_info)
	{
		float num = 0f;
		for (int i = 0; i < bombardment_info.Length; i++)
		{
			BombardmentInfo bombardmentInfo = bombardment_info[i];
			num += bombardmentInfo.weight;
		}
		num = UnityEngine.Random.Range(0f, num);
		BombardmentInfo bombardmentInfo2 = bombardment_info[0];
		int num2 = 0;
		while (num - bombardmentInfo2.weight > 0f)
		{
			num -= bombardmentInfo2.weight;
			bombardmentInfo2 = bombardment_info[++num2];
		}
		Game.Instance.Trigger(-84771526);
		SpawnBombard(bombardmentInfo2.prefab);
	}

	private GameObject SpawnBombard(string prefab)
	{
		GameObject obj = Util.KInstantiate(position: new Vector3(UnityEngine.Random.value * (float)Grid.WidthInCells, 1.2f * (float)Grid.HeightInCells, Grid.GetLayerZ(Grid.SceneLayer.FXFront)), original: Assets.GetPrefab(prefab), rotation: Quaternion.identity);
		obj.SetActive(value: true);
		return obj;
	}

	public bool CurrentSeasonHasBombardment()
	{
		Season season = seasons[SeasonLoop[currentSeasonIndex]];
		if (season.bombardmentInfo != null)
		{
			return season.bombardmentInfo.Length != 0;
		}
		return false;
	}

	public float TimeUntilNextBombardment()
	{
		if (!CurrentSeasonHasBombardment())
		{
			return float.MaxValue;
		}
		if (!bombardmentOn)
		{
			return bombardmentPeriodRemaining;
		}
		return 0f;
	}

	public float GetBombardmentDuration()
	{
		if (CurrentSeasonHasBombardment())
		{
			Season season = seasons[SeasonLoop[currentSeasonIndex]];
			if (!bombardmentOn)
			{
				return season.secondsBombardmentOn.Get();
			}
			return 0f;
		}
		return 0f;
	}

	public void ForceBeginMeteorSeasonWithShower()
	{
		for (int i = 0; i < SeasonLoop.Length; i++)
		{
			if (SeasonLoop[i] == "MeteorShowerIron")
			{
				currentSeasonIndex = i;
			}
		}
		ResetSeasonProgress();
		Season season = seasons[SeasonLoop[currentSeasonIndex]];
		bombardmentOn = true;
		bombardmentPeriodRemaining = season.secondsBombardmentOn.Get();
	}

	[ContextMenu("Bombard")]
	public void Debug_Bombardment()
	{
		BombardmentInfo[] bombardmentInfo = seasons[SeasonLoop[currentSeasonIndex]].bombardmentInfo;
		DoBombardment(bombardmentInfo);
	}

	[ContextMenu("Force Shower")]
	public void Debug_ForceShower()
	{
		currentSeasonIndex = Array.IndexOf(SeasonLoop, "MeteorShowerIron");
		ResetSeasonProgress();
		bombardmentOn = true;
		bombardmentPeriodRemaining = float.MaxValue;
		secondsUntilNextBombardment = 0f;
	}

	public void DrawDebugger()
	{
	}

	public SeasonManager()
	{
		Dictionary<string, Season> dictionary = new Dictionary<string, Season>();
		Season value = new Season
		{
			durationInCycles = 4
		};
		dictionary.Add("Default", value);
		value = new Season
		{
			durationInCycles = 10,
			secondsBombardmentOff = new MathUtil.MinMax(300f, 1200f),
			secondsBombardmentOn = new MathUtil.MinMax(100f, 400f),
			secondsBetweenBombardments = new MathUtil.MinMax(1f, 1.5f),
			meteorBackground = true
		};
		BombardmentInfo[] array = new BombardmentInfo[3];
		BombardmentInfo bombardmentInfo = new BombardmentInfo
		{
			prefab = IronCometConfig.ID,
			weight = 1f
		};
		array[0] = bombardmentInfo;
		bombardmentInfo = new BombardmentInfo
		{
			prefab = RockCometConfig.ID,
			weight = 2f
		};
		array[1] = bombardmentInfo;
		bombardmentInfo = new BombardmentInfo
		{
			prefab = DustCometConfig.ID,
			weight = 5f
		};
		array[2] = bombardmentInfo;
		value.bombardmentInfo = array;
		dictionary.Add("MeteorShowerIron", value);
		value = new Season
		{
			durationInCycles = 5,
			secondsBombardmentOff = new MathUtil.MinMax(800f, 1200f),
			secondsBombardmentOn = new MathUtil.MinMax(50f, 100f),
			secondsBetweenBombardments = new MathUtil.MinMax(0.3f, 0.5f),
			meteorBackground = true
		};
		BombardmentInfo[] array2 = new BombardmentInfo[3];
		bombardmentInfo = new BombardmentInfo
		{
			prefab = GoldCometConfig.ID,
			weight = 2f
		};
		array2[0] = bombardmentInfo;
		bombardmentInfo = new BombardmentInfo
		{
			prefab = RockCometConfig.ID,
			weight = 0.5f
		};
		array2[1] = bombardmentInfo;
		bombardmentInfo = new BombardmentInfo
		{
			prefab = DustCometConfig.ID,
			weight = 5f
		};
		array2[2] = bombardmentInfo;
		value.bombardmentInfo = array2;
		dictionary.Add("MeteorShowerGold", value);
		value = new Season
		{
			durationInCycles = 7,
			secondsBombardmentOff = new MathUtil.MinMax(300f, 1200f),
			secondsBombardmentOn = new MathUtil.MinMax(100f, 400f),
			secondsBetweenBombardments = new MathUtil.MinMax(4f, 6.5f),
			meteorBackground = true
		};
		BombardmentInfo[] array3 = new BombardmentInfo[2];
		bombardmentInfo = new BombardmentInfo
		{
			prefab = CopperCometConfig.ID,
			weight = 1f
		};
		array3[0] = bombardmentInfo;
		bombardmentInfo = new BombardmentInfo
		{
			prefab = RockCometConfig.ID,
			weight = 1f
		};
		array3[1] = bombardmentInfo;
		value.bombardmentInfo = array3;
		dictionary.Add("MeteorShowerCopper", value);
		seasons = dictionary;
		SeasonLoop = new string[6]
		{
			"Default",
			"MeteorShowerIron",
			"Default",
			"MeteorShowerCopper",
			"Default",
			"MeteorShowerGold"
		};
		base._002Ector();
	}
}
