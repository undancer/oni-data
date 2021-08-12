using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/TreeBud")]
public class TreeBud : KMonoBehaviour, IWiltCause
{
	[MyCmpReq]
	private Growing growing;

	[MyCmpReq]
	private StandardCropPlant crop;

	[Serialize]
	public Ref<BuddingTrunk> buddingTrunk;

	[Serialize]
	private int trunkPosition;

	[Serialize]
	public int growingPos;

	private int trunkWiltHandle = -1;

	private int trunkWiltRecoverHandle = -1;

	private static StandardCropPlant.AnimSet[] animSets = new StandardCropPlant.AnimSet[7]
	{
		new StandardCropPlant.AnimSet
		{
			grow = "branch_a_grow",
			grow_pst = "branch_a_grow_pst",
			idle_full = "branch_a_idle_full",
			wilt_base = "branch_a_wilt",
			harvest = "branch_a_harvest"
		},
		new StandardCropPlant.AnimSet
		{
			grow = "branch_b_grow",
			grow_pst = "branch_b_grow_pst",
			idle_full = "branch_b_idle_full",
			wilt_base = "branch_b_wilt",
			harvest = "branch_b_harvest"
		},
		new StandardCropPlant.AnimSet
		{
			grow = "branch_c_grow",
			grow_pst = "branch_c_grow_pst",
			idle_full = "branch_c_idle_full",
			wilt_base = "branch_c_wilt",
			harvest = "branch_c_harvest"
		},
		new StandardCropPlant.AnimSet
		{
			grow = "branch_d_grow",
			grow_pst = "branch_d_grow_pst",
			idle_full = "branch_d_idle_full",
			wilt_base = "branch_d_wilt",
			harvest = "branch_d_harvest"
		},
		new StandardCropPlant.AnimSet
		{
			grow = "branch_e_grow",
			grow_pst = "branch_e_grow_pst",
			idle_full = "branch_e_idle_full",
			wilt_base = "branch_e_wilt",
			harvest = "branch_e_harvest"
		},
		new StandardCropPlant.AnimSet
		{
			grow = "branch_f_grow",
			grow_pst = "branch_f_grow_pst",
			idle_full = "branch_f_idle_full",
			wilt_base = "branch_f_wilt",
			harvest = "branch_f_harvest"
		},
		new StandardCropPlant.AnimSet
		{
			grow = "branch_g_grow",
			grow_pst = "branch_g_grow_pst",
			idle_full = "branch_g_idle_full",
			wilt_base = "branch_g_wilt",
			harvest = "branch_g_harvest"
		}
	};

	private static Vector3[] animOffset = new Vector3[7]
	{
		new Vector3(1f, 0f, 0f),
		new Vector3(1f, -1f, 0f),
		new Vector3(1f, -2f, 0f),
		new Vector3(0f, -2f, 0f),
		new Vector3(-1f, -2f, 0f),
		new Vector3(-1f, -1f, 0f),
		new Vector3(-1f, 0f, 0f)
	};

	private static readonly EventSystem.IntraObjectHandler<TreeBud> OnHarvestDelegate = new EventSystem.IntraObjectHandler<TreeBud>(delegate(TreeBud component, object data)
	{
		component.OnHarvest(data);
	});

	public string WiltStateString => "    • " + DUPLICANTS.STATS.TRUNKHEALTH.NAME;

	public WiltCondition.Condition[] Conditions => new WiltCondition.Condition[1] { WiltCondition.Condition.UnhealthyRoot };

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		simRenderLoadBalance = true;
		int cell = Grid.PosToCell(base.gameObject);
		GameObject gameObject = Grid.Objects[cell, 5];
		if (gameObject != null && gameObject != base.gameObject)
		{
			Util.KDestroyGameObject(base.gameObject);
		}
		else
		{
			SetOccupyGridSpace(active: true);
		}
		Subscribe(1272413801, OnHarvestDelegate);
	}

	private void OnHarvest(object data)
	{
		if (buddingTrunk.Get() != null)
		{
			buddingTrunk.Get().TryRollNewSeed();
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (buddingTrunk != null && buddingTrunk.Get() != null)
		{
			SubscribeToTrunk();
			UpdateAnimationSet();
		}
		else
		{
			Debug.LogWarning("TreeBud loaded with missing trunk reference. Destroying...");
			Util.KDestroyGameObject(base.gameObject);
		}
	}

	protected override void OnCleanUp()
	{
		UnsubscribeToTrunk();
		SetOccupyGridSpace(active: false);
		base.OnCleanUp();
	}

	private void SetOccupyGridSpace(bool active)
	{
		int cell = Grid.PosToCell(base.gameObject);
		if (active)
		{
			GameObject gameObject = Grid.Objects[cell, 5];
			if (gameObject != null && gameObject != base.gameObject)
			{
				Debug.LogWarningFormat(base.gameObject, "TreeBud.SetOccupyGridSpace already occupied by {0}", gameObject);
			}
			Grid.Objects[cell, 5] = base.gameObject;
		}
		else if (Grid.Objects[cell, 5] == base.gameObject)
		{
			Grid.Objects[cell, 5] = null;
		}
	}

	private void SubscribeToTrunk()
	{
		if (trunkWiltHandle == -1 && trunkWiltRecoverHandle == -1)
		{
			Debug.Assert(this.buddingTrunk != null, "buddingTrunk null");
			BuddingTrunk buddingTrunk = this.buddingTrunk.Get();
			Debug.Assert(buddingTrunk != null, "tree_trunk null");
			trunkWiltHandle = buddingTrunk.Subscribe(-724860998, OnTrunkWilt);
			trunkWiltRecoverHandle = buddingTrunk.Subscribe(712767498, OnTrunkRecover);
			Trigger(912965142, !buddingTrunk.GetComponent<WiltCondition>().IsWilting());
			ReceptacleMonitor component = GetComponent<ReceptacleMonitor>();
			PlantablePlot receptacle = buddingTrunk.GetComponent<ReceptacleMonitor>().GetReceptacle();
			component.SetReceptacle(receptacle);
			Vector3 position = base.gameObject.transform.position;
			position.z = Grid.GetLayerZ(Grid.SceneLayer.BuildingFront) - 0.1f * (float)trunkPosition;
			base.gameObject.transform.SetPosition(position);
			GetComponent<BudUprootedMonitor>().SetParentObject(buddingTrunk.GetComponent<KPrefabID>());
		}
	}

	private void UnsubscribeToTrunk()
	{
		if (this.buddingTrunk == null)
		{
			Debug.LogWarning("buddingTrunk null", base.gameObject);
			return;
		}
		BuddingTrunk buddingTrunk = this.buddingTrunk.Get();
		if (buddingTrunk == null)
		{
			Debug.LogWarning("tree_trunk null", base.gameObject);
			return;
		}
		buddingTrunk.Unsubscribe(trunkWiltHandle);
		buddingTrunk.Unsubscribe(trunkWiltRecoverHandle);
		buddingTrunk.OnBranchRemoved(trunkPosition, this);
	}

	public void SetTrunkPosition(BuddingTrunk budding_trunk, int idx)
	{
		buddingTrunk = new Ref<BuddingTrunk>(budding_trunk);
		trunkPosition = idx;
		SubscribeToTrunk();
		UpdateAnimationSet();
	}

	private void OnTrunkWilt(object data = null)
	{
		Trigger(912965142, false);
	}

	private void OnTrunkRecover(object data = null)
	{
		Trigger(912965142, true);
	}

	private void UpdateAnimationSet()
	{
		crop.anims = animSets[trunkPosition];
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		component.Offset = animOffset[trunkPosition];
		component.Play(crop.anims.grow, KAnim.PlayMode.Paused);
		crop.RefreshPositionPercent();
	}
}
