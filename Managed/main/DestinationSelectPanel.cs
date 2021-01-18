using System;
using System.Collections.Generic;
using Database;
using Klei.CustomSettings;
using ProcGen;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/DestinationSelectPanel")]
public class DestinationSelectPanel : KMonoBehaviour
{
	[SerializeField]
	private GameObject asteroidPrefab;

	[SerializeField]
	private KButtonDrag dragTarget;

	[SerializeField]
	private MultiToggle leftArrowButton;

	[SerializeField]
	private MultiToggle rightArrowButton;

	[SerializeField]
	private RectTransform asteroidContainer;

	[SerializeField]
	private float asteroidFocusScale = 2f;

	[SerializeField]
	private float asteroidXSeparation = 240f;

	[SerializeField]
	private float focusScaleSpeed = 0.5f;

	[SerializeField]
	private float centeringSpeed = 0.5f;

	[SerializeField]
	private GameObject moonContainer;

	[SerializeField]
	private GameObject moonPrefab;

	private float offset = 0f;

	private int selectedIndex = -1;

	private List<DestinationAsteroid2> asteroids = new List<DestinationAsteroid2>();

	private int numAsteroids;

	private List<string> clusterKeys;

	private Dictionary<string, string> clusterStartWorlds;

	private Dictionary<string, ColonyDestinationAsteroidBeltData> asteroidData = new Dictionary<string, ColonyDestinationAsteroidBeltData>();

	private Vector2 dragStartPos;

	private Vector2 dragLastPos;

	private bool isDragging = false;

	private const string debugFmt = "{world}: {seed} [{traits}] {{settings}}";

	private float min => asteroidContainer.rect.x + offset;

	private float max => min + asteroidContainer.rect.width;

	public event Action<ColonyDestinationAsteroidBeltData> OnAsteroidClicked;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		dragTarget.onBeginDrag += BeginDrag;
		dragTarget.onDrag += Drag;
		dragTarget.onEndDrag += EndDrag;
		MultiToggle multiToggle = leftArrowButton;
		multiToggle.onClick = (System.Action)Delegate.Combine(multiToggle.onClick, new System.Action(ClickLeft));
		MultiToggle multiToggle2 = rightArrowButton;
		multiToggle2.onClick = (System.Action)Delegate.Combine(multiToggle2.onClick, new System.Action(ClickRight));
	}

	private void BeginDrag()
	{
		dragStartPos = Input.mousePosition;
		dragLastPos = dragStartPos;
		isDragging = true;
		KFMOD.PlayUISound(GlobalAssets.GetSound("DestinationSelect_Scroll_Start"));
	}

	private void Drag()
	{
		Vector2 vector = Input.mousePosition;
		float num = vector.x - dragLastPos.x;
		dragLastPos = vector;
		offset += num;
		int num2 = selectedIndex;
		selectedIndex = Mathf.RoundToInt((0f - offset) / asteroidXSeparation);
		selectedIndex = Mathf.Clamp(selectedIndex, 0, clusterStartWorlds.Count - 1);
		if (num2 != selectedIndex)
		{
			this.OnAsteroidClicked(asteroidData[clusterKeys[selectedIndex]]);
			KFMOD.PlayUISound(GlobalAssets.GetSound("DestinationSelect_Scroll"));
		}
	}

	private void EndDrag()
	{
		Drag();
		isDragging = false;
		KFMOD.PlayUISound(GlobalAssets.GetSound("DestinationSelect_Scroll_Stop"));
	}

	private void ClickLeft()
	{
		selectedIndex = Mathf.Clamp(selectedIndex - 1, 0, clusterKeys.Count - 1);
		this.OnAsteroidClicked(asteroidData[clusterKeys[selectedIndex]]);
	}

	private void ClickRight()
	{
		selectedIndex = Mathf.Clamp(selectedIndex + 1, 0, clusterKeys.Count - 1);
		this.OnAsteroidClicked(asteroidData[clusterKeys[selectedIndex]]);
	}

	public void Init()
	{
		clusterKeys = new List<string>();
		clusterStartWorlds = new Dictionary<string, string>();
		UpdateDisplayedWorlds();
	}

	private void Update()
	{
		if (!isDragging)
		{
			float num = offset + (float)selectedIndex * asteroidXSeparation;
			float value = 0f;
			if (num != 0f)
			{
				value = 0f - num;
			}
			value = Mathf.Clamp(value, (0f - asteroidXSeparation) * 2f, asteroidXSeparation * 2f);
			if (value != 0f)
			{
				float num2 = centeringSpeed * Time.unscaledDeltaTime;
				float num3 = value * centeringSpeed * Time.unscaledDeltaTime;
				if (num3 > 0f && num3 < num2)
				{
					num3 = Mathf.Min(num2, value);
				}
				else if (num3 < 0f && num3 > 0f - num2)
				{
					num3 = Mathf.Max(0f - num2, value);
				}
				offset += num3;
			}
		}
		float x = asteroidContainer.rect.min.x;
		float x2 = asteroidContainer.rect.max.x;
		offset = Mathf.Clamp(offset, (float)(-(clusterStartWorlds.Count - 1)) * asteroidXSeparation + x, x2);
		RePlaceAsteroids();
		for (int i = 0; i < moonContainer.transform.childCount; i++)
		{
			moonContainer.transform.GetChild(i).GetChild(0).SetLocalPosition(new Vector3(0f, 1.5f + 3f * Mathf.Sin(((float)i + Time.realtimeSinceStartup) * 1.25f), 0f));
		}
	}

	public void UpdateDisplayedWorlds()
	{
		clusterKeys.Clear();
		clusterStartWorlds.Clear();
		asteroidData.Clear();
		moonContainer.SetActive(!DlcManager.IsExpansion1Active());
		foreach (KeyValuePair<string, ClusterLayout> item in SettingsCache.clusterLayouts.clusterCache)
		{
			if (!(item.Key == "clusters/SandstoneDefault"))
			{
				clusterKeys.Add(item.Key);
				ColonyDestinationAsteroidBeltData value = new ColonyDestinationAsteroidBeltData(item.Value.GetStartWorld(), 0, item.Key);
				asteroidData[item.Key] = value;
				clusterStartWorlds.Add(item.Key, item.Value.GetStartWorld());
			}
		}
		clusterKeys.Sort((string a, string b) => SettingsCache.clusterLayouts.clusterCache[a].menuOrder.CompareTo(SettingsCache.clusterLayouts.clusterCache[b].menuOrder));
	}

	[ContextMenu("RePlaceAsteroids")]
	public void RePlaceAsteroids()
	{
		BeginAsteroidDrawing();
		for (int i = 0; i < clusterKeys.Count; i++)
		{
			float num = offset + (float)i * asteroidXSeparation;
			if (!(num + offset + asteroidXSeparation < min) && !(num + offset - asteroidXSeparation > max))
			{
				DestinationAsteroid2 asteroid = GetAsteroid(clusterKeys[i], (i == selectedIndex) ? asteroidFocusScale : 1f);
				asteroid.transform.SetLocalPosition(new Vector3(num, (i == selectedIndex) ? (5f + 10f * Mathf.Sin(Time.realtimeSinceStartup * 1f)) : 0f, 0f));
			}
		}
		EndAsteroidDrawing();
	}

	private void BeginAsteroidDrawing()
	{
		numAsteroids = 0;
	}

	private void ShowMoons(ColonyDestinationAsteroidBeltData asteroid)
	{
		if (asteroid.worlds.Count > 0)
		{
			while (moonContainer.transform.childCount < asteroid.worlds.Count)
			{
				UnityEngine.Object.Instantiate(moonPrefab, moonContainer.transform);
			}
			for (int i = 0; i < asteroid.worlds.Count; i++)
			{
				KBatchedAnimController componentInChildren = moonContainer.transform.GetChild(i).GetComponentInChildren<KBatchedAnimController>();
				int index = (-1 + i + asteroid.worlds.Count / 2) % asteroid.worlds.Count;
				ProcGen.World world = null;
				world = asteroid.worlds[index];
				AsteroidType typeOrDefault = Db.Get().AsteroidTypes.GetTypeOrDefault(world.asteroidType);
				KAnimFile anim = Assets.GetAnim(typeOrDefault.animName);
				componentInChildren.SwapAnims(new KAnimFile[1]
				{
					anim
				});
				componentInChildren.initialMode = KAnim.PlayMode.Loop;
				componentInChildren.initialAnim = "idle_loop";
				componentInChildren.gameObject.SetActive(value: true);
				if (componentInChildren.HasAnimation(componentInChildren.initialAnim))
				{
					componentInChildren.Play(componentInChildren.initialAnim, KAnim.PlayMode.Loop);
				}
				componentInChildren.transform.parent.gameObject.SetActive(value: true);
			}
			for (int j = asteroid.worlds.Count; j < moonContainer.transform.childCount; j++)
			{
				moonContainer.transform.GetChild(j).gameObject.SetActive(value: false);
			}
		}
		moonContainer.SetActive(asteroid.worlds.Count > 0);
	}

	private DestinationAsteroid2 GetAsteroid(string name, float scale)
	{
		DestinationAsteroid2 destinationAsteroid;
		if (numAsteroids < asteroids.Count)
		{
			destinationAsteroid = asteroids[numAsteroids];
		}
		else
		{
			destinationAsteroid = Util.KInstantiateUI<DestinationAsteroid2>(asteroidPrefab, asteroidContainer.gameObject);
			destinationAsteroid.OnClicked += this.OnAsteroidClicked;
			asteroids.Add(destinationAsteroid);
		}
		destinationAsteroid.SetAsteroid(asteroidData[name]);
		asteroidData[name].TargetScale = scale;
		asteroidData[name].Scale += (asteroidData[name].TargetScale - asteroidData[name].Scale) * focusScaleSpeed * Time.unscaledDeltaTime;
		destinationAsteroid.transform.localScale = Vector3.one * asteroidData[name].Scale;
		numAsteroids++;
		return destinationAsteroid;
	}

	private void EndAsteroidDrawing()
	{
		for (int i = 0; i < asteroids.Count; i++)
		{
			asteroids[i].gameObject.SetActive(i < numAsteroids);
		}
	}

	public ColonyDestinationAsteroidBeltData SelectAsteroid(string name, int seed)
	{
		selectedIndex = clusterKeys.IndexOf(name);
		asteroidData[name].ReInitialize(seed);
		ShowMoons(asteroidData[name]);
		return asteroidData[name];
	}

	public void ScrollLeft()
	{
		int index = Mathf.Max(selectedIndex - 1, 0);
		this.OnAsteroidClicked(asteroidData[clusterKeys[index]]);
	}

	public void ScrollRight()
	{
		int index = Mathf.Min(selectedIndex + 1, clusterStartWorlds.Count - 1);
		this.OnAsteroidClicked(asteroidData[clusterKeys[index]]);
	}

	private void DebugCurrentSetting()
	{
		ColonyDestinationAsteroidBeltData colonyDestinationAsteroidBeltData = asteroidData[clusterKeys[selectedIndex]];
		string text = "{world}: {seed} [{traits}] {{settings}}";
		string startWorldName = colonyDestinationAsteroidBeltData.startWorldName;
		string newValue = colonyDestinationAsteroidBeltData.seed.ToString();
		text = text.Replace("{world}", startWorldName);
		text = text.Replace("{seed}", newValue);
		List<AsteroidDescriptor> traitDescriptors = colonyDestinationAsteroidBeltData.GetTraitDescriptors();
		string[] array = new string[traitDescriptors.Count];
		for (int i = 0; i < traitDescriptors.Count; i++)
		{
			array[i] = traitDescriptors[i].text;
		}
		string newValue2 = string.Join(", ", array);
		text = text.Replace("{traits}", newValue2);
		switch (CustomGameSettings.Instance.customGameMode)
		{
		case CustomGameSettings.CustomGameMode.Survival:
			text = text.Replace("{settings}", "Survival");
			break;
		case CustomGameSettings.CustomGameMode.Nosweat:
			text = text.Replace("{settings}", "Nosweat");
			break;
		case CustomGameSettings.CustomGameMode.Custom:
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, SettingConfig> qualitySetting in CustomGameSettings.Instance.QualitySettings)
			{
				if (qualitySetting.Value.coordinate_dimension >= 0 && qualitySetting.Value.coordinate_dimension_width >= 0)
				{
					SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(qualitySetting.Key);
					if (currentQualitySetting.id != qualitySetting.Value.GetDefaultLevelId())
					{
						list.Add($"{qualitySetting.Value.label}={currentQualitySetting.label}");
					}
				}
			}
			text = text.Replace("{settings}", string.Join(", ", list.ToArray()));
			break;
		}
		}
		Debug.Log(text);
	}
}
