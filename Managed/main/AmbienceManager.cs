using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/AmbienceManager")]
public class AmbienceManager : KMonoBehaviour
{
	public class Tuning : TuningData<Tuning>
	{
		public int backwallTileValue = 1;

		public int foundationTileValue = 2;

		public int buildingTileValue = 3;
	}

	public class Layer : IComparable<Layer>
	{
		private const string TILE_PERCENTAGE_ID = "tilePercentage";

		private const string AVERAGE_TEMPERATURE_ID = "averageTemperature";

		private const string AVERAGE_RADIATION_ID = "averageRadiation";

		public string sound;

		public string oneShotSound;

		public int tileCount;

		public float tilePercentage;

		public float volume;

		public bool isRunning;

		private EventInstance soundEvent;

		public float averageTemperature;

		public float averageRadiation;

		public Layer(string sound, string one_shot_sound)
		{
			this.sound = sound;
			oneShotSound = one_shot_sound;
		}

		public void Reset()
		{
			tileCount = 0;
			averageTemperature = 0f;
			averageRadiation = 0f;
		}

		public void UpdatePercentage(int cell_count)
		{
			tilePercentage = (float)tileCount / (float)cell_count;
		}

		public void UpdateAverageTemperature()
		{
			averageTemperature /= tileCount;
			soundEvent.setParameterByName("averageTemperature", averageTemperature);
		}

		public void UpdateAverageRadiation()
		{
			averageRadiation = ((tileCount > 0) ? (averageRadiation / (float)tileCount) : 0f);
			soundEvent.setParameterByName("averageRadiation", averageRadiation);
		}

		public void UpdateParameters(Vector3 emitter_position)
		{
			if (soundEvent.isValid())
			{
				Vector3 pos = new Vector3(emitter_position.x, emitter_position.y, 0f);
				soundEvent.set3DAttributes(pos.To3DAttributes());
				soundEvent.setParameterByName("tilePercentage", tilePercentage);
			}
		}

		public int CompareTo(Layer layer)
		{
			return layer.tileCount - tileCount;
		}

		public void Stop()
		{
			if (soundEvent.isValid())
			{
				soundEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
				soundEvent.release();
			}
			isRunning = false;
		}

		public void Start(Vector3 emitter_position)
		{
			if (isRunning)
			{
				return;
			}
			if (oneShotSound != null)
			{
				EventInstance eventInstance = KFMOD.CreateInstance(oneShotSound);
				if (!eventInstance.isValid())
				{
					Debug.LogWarning("Could not find event: " + oneShotSound);
					return;
				}
				ATTRIBUTES_3D attributes = new Vector3(emitter_position.x, emitter_position.y, 0f).To3DAttributes();
				eventInstance.set3DAttributes(attributes);
				eventInstance.setVolume(tilePercentage * 2f);
				eventInstance.start();
				eventInstance.release();
			}
			else
			{
				soundEvent = KFMOD.CreateInstance(sound);
				if (soundEvent.isValid())
				{
					soundEvent.start();
				}
				isRunning = true;
			}
		}
	}

	[Serializable]
	public class QuadrantDef
	{
		public string name;

		[EventRef]
		public string[] liquidSounds;

		[EventRef]
		public string[] gasSounds;

		[EventRef]
		public string[] solidSounds;

		[EventRef]
		public string fogSound;

		[EventRef]
		public string spaceSound;

		[EventRef]
		public string facilitySound;

		[EventRef]
		public string radiationSound;
	}

	public class Quadrant
	{
		public class SolidTimer
		{
			public static float solidMinTime = 9f;

			public static float solidMaxTime = 15f;

			public float solidTargetTime;

			public SolidTimer()
			{
				solidTargetTime = Time.unscaledTime + UnityEngine.Random.value * solidMinTime;
			}

			public bool ShouldPlay()
			{
				if (Time.unscaledTime > solidTargetTime)
				{
					solidTargetTime = Time.unscaledTime + solidMinTime + UnityEngine.Random.value * (solidMaxTime - solidMinTime);
					return true;
				}
				return false;
			}
		}

		public string name;

		public Vector3 emitterPosition;

		public Layer[] gasLayers = new Layer[4];

		public Layer[] liquidLayers = new Layer[4];

		public Layer fogLayer;

		public Layer spaceLayer;

		public Layer facilityLayer;

		public Layer radiationLayer;

		public Layer[] solidLayers = new Layer[16];

		private List<Layer> allLayers = new List<Layer>();

		private List<Layer> loopingLayers = new List<Layer>();

		private List<Layer> oneShotLayers = new List<Layer>();

		private List<Layer> topLayers = new List<Layer>();

		public static int activeSolidLayerCount = 2;

		public int totalTileCount;

		private bool m_isRadiationEnabled;

		private SolidTimer[] solidTimers;

		public Quadrant(QuadrantDef def)
		{
			name = def.name;
			fogLayer = new Layer(def.fogSound, null);
			allLayers.Add(fogLayer);
			loopingLayers.Add(fogLayer);
			spaceLayer = new Layer(def.spaceSound, null);
			allLayers.Add(spaceLayer);
			loopingLayers.Add(spaceLayer);
			facilityLayer = new Layer(def.facilitySound, null);
			allLayers.Add(facilityLayer);
			loopingLayers.Add(facilityLayer);
			m_isRadiationEnabled = Sim.IsRadiationEnabled();
			if (m_isRadiationEnabled)
			{
				radiationLayer = new Layer(def.radiationSound, null);
				allLayers.Add(radiationLayer);
			}
			for (int i = 0; i < 4; i++)
			{
				gasLayers[i] = new Layer(def.gasSounds[i], null);
				liquidLayers[i] = new Layer(def.liquidSounds[i], null);
				allLayers.Add(gasLayers[i]);
				allLayers.Add(liquidLayers[i]);
				loopingLayers.Add(gasLayers[i]);
				loopingLayers.Add(liquidLayers[i]);
			}
			for (int j = 0; j < solidLayers.Length; j++)
			{
				if (j >= def.solidSounds.Length)
				{
					SolidAmbienceType solidAmbienceType = (SolidAmbienceType)j;
					Debug.LogError("Missing solid layer: " + solidAmbienceType);
				}
				solidLayers[j] = new Layer(null, def.solidSounds[j]);
				allLayers.Add(solidLayers[j]);
				oneShotLayers.Add(solidLayers[j]);
			}
			solidTimers = new SolidTimer[activeSolidLayerCount];
			for (int k = 0; k < activeSolidLayerCount; k++)
			{
				solidTimers[k] = new SolidTimer();
			}
		}

		public void Update(Vector2I min, Vector2I max, Vector3 emitter_position)
		{
			emitterPosition = emitter_position;
			totalTileCount = 0;
			for (int i = 0; i < allLayers.Count; i++)
			{
				allLayers[i].Reset();
			}
			for (int j = min.y; j < max.y; j++)
			{
				if (j % 2 == 1)
				{
					continue;
				}
				for (int k = min.x; k < max.x; k++)
				{
					if (k % 2 == 0)
					{
						continue;
					}
					int num = Grid.XYToCell(k, j);
					if (!Grid.IsValidCell(num))
					{
						continue;
					}
					totalTileCount++;
					if (Grid.IsVisible(num))
					{
						if (Grid.GravitasFacility[num])
						{
							facilityLayer.tileCount += 8;
						}
						else
						{
							Element element = Grid.Element[num];
							if (element != null)
							{
								if (element.IsLiquid && Grid.IsSubstantialLiquid(num))
								{
									AmbienceType ambience = element.substance.GetAmbience();
									if (ambience != AmbienceType.None)
									{
										liquidLayers[(int)ambience].tileCount++;
										liquidLayers[(int)ambience].averageTemperature += Grid.Temperature[num];
									}
								}
								else if (element.IsGas)
								{
									AmbienceType ambience2 = element.substance.GetAmbience();
									if (ambience2 != AmbienceType.None)
									{
										gasLayers[(int)ambience2].tileCount++;
										gasLayers[(int)ambience2].averageTemperature += Grid.Temperature[num];
									}
								}
								else if (element.IsSolid)
								{
									SolidAmbienceType solidAmbience = element.substance.GetSolidAmbience();
									if (Grid.Foundation[num])
									{
										solidAmbience = SolidAmbienceType.Tile;
										solidLayers[(int)solidAmbience].tileCount += TuningData<Tuning>.Get().foundationTileValue;
										spaceLayer.tileCount -= TuningData<Tuning>.Get().foundationTileValue;
									}
									else if (Grid.Objects[num, 2] != null)
									{
										solidAmbience = SolidAmbienceType.Tile;
										solidLayers[(int)solidAmbience].tileCount += TuningData<Tuning>.Get().backwallTileValue;
										spaceLayer.tileCount -= TuningData<Tuning>.Get().backwallTileValue;
									}
									else if (solidAmbience != SolidAmbienceType.None)
									{
										solidLayers[(int)solidAmbience].tileCount++;
									}
									else if (element.id == SimHashes.Regolith || element.id == SimHashes.MaficRock)
									{
										spaceLayer.tileCount++;
									}
								}
								else if (element.id == SimHashes.Vacuum && CellSelectionObject.IsExposedToSpace(num))
								{
									if (Grid.Objects[num, 1] != null)
									{
										spaceLayer.tileCount -= TuningData<Tuning>.Get().buildingTileValue;
									}
									spaceLayer.tileCount++;
								}
							}
						}
						if (Grid.Radiation[num] > 0f)
						{
							radiationLayer.averageRadiation += Grid.Radiation[num];
							radiationLayer.tileCount++;
						}
					}
					else
					{
						fogLayer.tileCount++;
					}
				}
			}
			Vector2I vector2I = max - min;
			int cell_count = vector2I.x * vector2I.y;
			for (int l = 0; l < allLayers.Count; l++)
			{
				allLayers[l].UpdatePercentage(cell_count);
			}
			loopingLayers.Sort();
			topLayers.Clear();
			for (int m = 0; m < loopingLayers.Count; m++)
			{
				Layer layer = loopingLayers[m];
				if (m < 3 && layer.tilePercentage > 0f)
				{
					layer.Start(emitter_position);
					layer.UpdateAverageTemperature();
					layer.UpdateParameters(emitter_position);
					topLayers.Add(layer);
				}
				else
				{
					layer.Stop();
				}
			}
			if (m_isRadiationEnabled)
			{
				radiationLayer.Start(emitter_position);
				radiationLayer.UpdateAverageRadiation();
				radiationLayer.UpdateParameters(emitter_position);
			}
			oneShotLayers.Sort();
			for (int n = 0; n < activeSolidLayerCount; n++)
			{
				if (solidTimers[n].ShouldPlay() && oneShotLayers[n].tilePercentage > 0f)
				{
					oneShotLayers[n].Start(emitter_position);
				}
			}
		}

		public List<Layer> GetAllLayers()
		{
			return allLayers;
		}
	}

	private float emitterZPosition;

	public QuadrantDef[] quadrantDefs;

	public Quadrant[] quadrants = new Quadrant[4];

	protected override void OnSpawn()
	{
		if (!RuntimeManager.IsInitialized)
		{
			base.enabled = false;
			return;
		}
		for (int i = 0; i < quadrants.Length; i++)
		{
			quadrants[i] = new Quadrant(quadrantDefs[i]);
		}
	}

	protected override void OnForcedCleanUp()
	{
		Quadrant[] array = quadrants;
		for (int i = 0; i < array.Length; i++)
		{
			foreach (Layer allLayer in array[i].GetAllLayers())
			{
				allLayer.Stop();
			}
		}
	}

	private void LateUpdate()
	{
		GridArea visibleArea = GridVisibleArea.GetVisibleArea();
		Vector2I min = visibleArea.Min;
		Vector2I max = visibleArea.Max;
		Vector2I vector2I = min + (max - min) / 2;
		Vector3 vector = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, Camera.main.transform.GetPosition().z));
		Vector3 vector2 = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, Camera.main.transform.GetPosition().z));
		Vector3 vector3 = vector2 + (vector - vector2) / 2f;
		Vector3 vector4 = vector - vector2;
		if (vector4.x > vector4.y)
		{
			vector4.y = vector4.x;
		}
		else
		{
			vector4.x = vector4.y;
		}
		vector = vector3 + vector4 / 2f;
		vector2 = vector3 - vector4 / 2f;
		Vector3 vector5 = vector4 / 2f / 2f;
		quadrants[0].Update(new Vector2I(min.x, min.y), new Vector2I(vector2I.x, vector2I.y), new Vector3(vector2.x + vector5.x, vector2.y + vector5.y, emitterZPosition));
		quadrants[1].Update(new Vector2I(vector2I.x, min.y), new Vector2I(max.x, vector2I.y), new Vector3(vector3.x + vector5.x, vector2.y + vector5.y, emitterZPosition));
		quadrants[2].Update(new Vector2I(min.x, vector2I.y), new Vector2I(vector2I.x, max.y), new Vector3(vector2.x + vector5.x, vector3.y + vector5.y, emitterZPosition));
		quadrants[3].Update(new Vector2I(vector2I.x, vector2I.y), new Vector2I(max.x, max.y), new Vector3(vector3.x + vector5.x, vector3.y + vector5.y, emitterZPosition));
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		for (int i = 0; i < quadrants.Length; i++)
		{
			num += (float)quadrants[i].spaceLayer.tileCount;
			num2 += (float)quadrants[i].facilityLayer.tileCount;
			num3 += (float)quadrants[i].totalTileCount;
		}
		AudioMixer.instance.UpdateSpaceVisibleSnapshot(num / num3);
		AudioMixer.instance.UpdateFacilityVisibleSnapshot(num2 / num3);
	}
}
