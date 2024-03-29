using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/CO2Manager")]
public class CO2Manager : KMonoBehaviour, ISim33ms
{
	private const float CO2Lifetime = 3f;

	[SerializeField]
	private Vector3 acceleration;

	[SerializeField]
	private CO2 prefab;

	[SerializeField]
	private GameObject breathPrefab;

	[SerializeField]
	private Color tintColour;

	private List<CO2> co2Items = new List<CO2>();

	private ObjectPool breathPool;

	private ObjectPool co2Pool;

	public static CO2Manager instance;

	public static void DestroyInstance()
	{
		instance = null;
	}

	protected override void OnPrefabInit()
	{
		instance = this;
		prefab.gameObject.SetActive(value: false);
		breathPrefab.SetActive(value: false);
		co2Pool = new ObjectPool(InstantiateCO2, 16);
		breathPool = new ObjectPool(InstantiateBreath, 16);
	}

	private GameObject InstantiateCO2()
	{
		GameObject obj = GameUtil.KInstantiate(prefab, Grid.SceneLayer.Front);
		obj.SetActive(value: false);
		return obj;
	}

	private GameObject InstantiateBreath()
	{
		GameObject obj = GameUtil.KInstantiate(breathPrefab, Grid.SceneLayer.Front);
		obj.SetActive(value: false);
		return obj;
	}

	public void Sim33ms(float dt)
	{
		Vector2I xy = default(Vector2I);
		Vector2I xy2 = default(Vector2I);
		Vector3 vector = acceleration * dt;
		int num = co2Items.Count;
		for (int i = 0; i < num; i++)
		{
			CO2 cO = co2Items[i];
			cO.velocity += vector;
			cO.lifetimeRemaining -= dt;
			Grid.PosToXY(cO.transform.GetPosition(), out xy);
			cO.transform.SetPosition(cO.transform.GetPosition() + cO.velocity * dt);
			Grid.PosToXY(cO.transform.GetPosition(), out xy2);
			int num2 = Grid.XYToCell(xy.x, xy.y);
			for (int num3 = xy.y; num3 >= xy2.y; num3--)
			{
				int num4 = Grid.XYToCell(xy.x, num3);
				bool flag = !Grid.IsValidCell(num4) || cO.lifetimeRemaining <= 0f;
				if (!flag)
				{
					Element element = Grid.Element[num4];
					flag = element.IsLiquid || element.IsSolid || (Grid.Properties[num4] & 1) != 0;
				}
				if (flag)
				{
					int gameCell = num4;
					bool flag2 = false;
					if (num2 != num4)
					{
						gameCell = num2;
						flag2 = true;
					}
					else
					{
						bool flag3 = false;
						int num5 = -1;
						int num6 = -1;
						CellOffset[] dEFAULT_BREATHABLE_OFFSETS = OxygenBreather.DEFAULT_BREATHABLE_OFFSETS;
						foreach (CellOffset offset in dEFAULT_BREATHABLE_OFFSETS)
						{
							int num7 = Grid.OffsetCell(num4, offset);
							if (Grid.IsValidCell(num7))
							{
								Element element2 = Grid.Element[num7];
								if (element2.id == SimHashes.CarbonDioxide || element2.HasTag(GameTags.Breathable))
								{
									num5 = num7;
									flag3 = true;
									flag2 = true;
									break;
								}
								if (element2.IsGas)
								{
									num6 = num7;
									flag2 = true;
								}
							}
						}
						if (flag2)
						{
							gameCell = ((!flag3) ? num6 : num5);
						}
					}
					cO.TriggerDestroy();
					if (flag2)
					{
						SimMessages.ModifyMass(gameCell, cO.mass, byte.MaxValue, 0, CellEventLogger.Instance.CO2ManagerFixedUpdate, cO.temperature, SimHashes.CarbonDioxide);
						num--;
						co2Items[i] = co2Items[num];
						co2Items.RemoveAt(num);
					}
					break;
				}
				num2 = num4;
			}
		}
	}

	public void SpawnCO2(Vector3 position, float mass, float temperature, bool flip)
	{
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Front);
		GameObject obj = co2Pool.GetInstance();
		obj.transform.SetPosition(position);
		obj.SetActive(value: true);
		CO2 component = obj.GetComponent<CO2>();
		component.mass = mass;
		component.temperature = temperature;
		component.velocity = Vector3.zero;
		component.lifetimeRemaining = 3f;
		KBatchedAnimController component2 = component.GetComponent<KBatchedAnimController>();
		component2.TintColour = tintColour;
		component2.onDestroySelf = OnDestroyCO2;
		component2.FlipX = flip;
		component.StartLoop();
		co2Items.Add(component);
	}

	public void SpawnBreath(Vector3 position, float mass, float temperature, bool flip)
	{
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Front);
		SpawnCO2(position, mass, temperature, flip);
		GameObject obj = breathPool.GetInstance();
		obj.transform.SetPosition(position);
		obj.SetActive(value: true);
		KBatchedAnimController component = obj.GetComponent<KBatchedAnimController>();
		component.TintColour = tintColour;
		component.onDestroySelf = OnDestroyBreath;
		component.FlipX = flip;
		component.Play("breath");
	}

	private void OnDestroyCO2(GameObject co2_go)
	{
		co2_go.SetActive(value: false);
		co2Pool.ReleaseInstance(co2_go);
	}

	private void OnDestroyBreath(GameObject breath_go)
	{
		breath_go.SetActive(value: false);
		breathPool.ReleaseInstance(breath_go);
	}
}
