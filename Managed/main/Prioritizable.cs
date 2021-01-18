using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Prioritizable")]
public class Prioritizable : KMonoBehaviour
{
	[SerializeField]
	[Serialize]
	private int masterPriority = int.MinValue;

	[SerializeField]
	[Serialize]
	private PrioritySetting masterPrioritySetting = new PrioritySetting(PriorityScreen.PriorityClass.basic, 5);

	public Action<PrioritySetting> onPriorityChanged;

	public bool showIcon = true;

	public Vector2 iconOffset;

	public float iconScale = 1f;

	[SerializeField]
	private int refCount;

	private static readonly EventSystem.IntraObjectHandler<Prioritizable> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<Prioritizable>(delegate(Prioritizable component, object data)
	{
		component.OnCopySettings(data);
	});

	private static Dictionary<PrioritySetting, PrioritySetting> conversions = new Dictionary<PrioritySetting, PrioritySetting>
	{
		{
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 1),
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 4)
		},
		{
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 2),
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 5)
		},
		{
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 3),
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 6)
		},
		{
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 4),
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 7)
		},
		{
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 5),
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 8)
		},
		{
			new PrioritySetting(PriorityScreen.PriorityClass.high, 1),
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 6)
		},
		{
			new PrioritySetting(PriorityScreen.PriorityClass.high, 2),
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 7)
		},
		{
			new PrioritySetting(PriorityScreen.PriorityClass.high, 3),
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 8)
		},
		{
			new PrioritySetting(PriorityScreen.PriorityClass.high, 4),
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 9)
		},
		{
			new PrioritySetting(PriorityScreen.PriorityClass.high, 5),
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 9)
		}
	};

	private HandleVector<int>.Handle scenePartitionerEntry;

	private Guid highPriorityStatusItem;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		Prioritizable component = gameObject.GetComponent<Prioritizable>();
		if (component != null)
		{
			SetMasterPriority(component.GetMasterPriority());
		}
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (masterPriority != int.MinValue)
		{
			masterPrioritySetting = new PrioritySetting(PriorityScreen.PriorityClass.basic, 5);
			masterPriority = int.MinValue;
		}
		if (SaveLoader.Instance.GameInfo.IsVersionExactly(7, 2) && conversions.TryGetValue(masterPrioritySetting, out var value))
		{
			masterPrioritySetting = value;
		}
	}

	protected override void OnSpawn()
	{
		if (onPriorityChanged != null)
		{
			onPriorityChanged(masterPrioritySetting);
		}
		RefreshHighPriorityNotification();
		RefreshTopPriorityOnWorld();
		Vector3 position = base.transform.GetPosition();
		Extents extents = new Extents((int)position.x, (int)position.y, 1, 1);
		scenePartitionerEntry = GameScenePartitioner.Instance.Add(base.name, this, extents, GameScenePartitioner.Instance.prioritizableObjects, null);
		Components.Prioritizables.Add(this);
	}

	public PrioritySetting GetMasterPriority()
	{
		return masterPrioritySetting;
	}

	public void SetMasterPriority(PrioritySetting priority)
	{
		if (!priority.Equals(masterPrioritySetting))
		{
			masterPrioritySetting = priority;
			if (onPriorityChanged != null)
			{
				onPriorityChanged(masterPrioritySetting);
			}
			RefreshTopPriorityOnWorld();
			RefreshHighPriorityNotification();
		}
	}

	private void RefreshTopPriorityOnWorld()
	{
		SetTopPriorityOnWorld(IsTopPriority());
	}

	private void SetTopPriorityOnWorld(bool state)
	{
		WorldContainer myWorld = base.gameObject.GetMyWorld();
		if (!(Game.Instance == null) && !(myWorld == null))
		{
			if (state)
			{
				myWorld.AddTopPriorityPrioritizable(this);
			}
			else
			{
				myWorld.RemoveTopPriorityPrioritizable(this);
			}
		}
	}

	public void AddRef()
	{
		refCount++;
		RefreshTopPriorityOnWorld();
		RefreshHighPriorityNotification();
	}

	public void RemoveRef()
	{
		refCount--;
		if (IsTopPriority() || refCount == 0)
		{
			SetTopPriorityOnWorld(state: false);
		}
		RefreshHighPriorityNotification();
	}

	public bool IsPrioritizable()
	{
		return refCount > 0;
	}

	public bool IsTopPriority()
	{
		return masterPrioritySetting.priority_class == PriorityScreen.PriorityClass.topPriority && IsPrioritizable();
	}

	protected override void OnCleanUp()
	{
		WorldContainer myWorld = base.gameObject.GetMyWorld();
		DebugUtil.DevAssert(myWorld != null, "World has been destroyed before this prioritizable");
		if (myWorld != null)
		{
			myWorld.RemoveTopPriorityPrioritizable(this);
		}
		base.OnCleanUp();
		GameScenePartitioner.Instance.Free(ref scenePartitionerEntry);
		Components.Prioritizables.Remove(this);
	}

	public static void AddRef(GameObject go)
	{
		Prioritizable component = go.GetComponent<Prioritizable>();
		if (component != null)
		{
			component.AddRef();
		}
	}

	public static void RemoveRef(GameObject go)
	{
		Prioritizable component = go.GetComponent<Prioritizable>();
		if (component != null)
		{
			component.RemoveRef();
		}
	}

	private void RefreshHighPriorityNotification()
	{
		bool flag = masterPrioritySetting.priority_class == PriorityScreen.PriorityClass.topPriority && IsPrioritizable();
		if (flag && highPriorityStatusItem == Guid.Empty)
		{
			highPriorityStatusItem = GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.EmergencyPriority);
		}
		else if (!flag && highPriorityStatusItem != Guid.Empty)
		{
			highPriorityStatusItem = GetComponent<KSelectable>().RemoveStatusItem(highPriorityStatusItem);
		}
	}
}
