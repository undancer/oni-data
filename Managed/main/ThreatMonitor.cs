using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class ThreatMonitor : GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>
{
	public class Def : BaseDef
	{
		public Health.HealthState fleethresholdState = Health.HealthState.Injured;

		public Tag[] friendlyCreatureTags;
	}

	public class ThreatenedStates : State
	{
		public ThreatenedDuplicantStates duplicant;

		public State creature;
	}

	public class ThreatenedDuplicantStates : State
	{
		public State ShoudFlee;

		public State ShouldFight;
	}

	public struct Grudge
	{
		public FactionAlignment target;

		public float grudgeTime;

		public void Reset(FactionAlignment revengeTarget)
		{
			target = revengeTarget;
			float num = (grudgeTime = 10f);
		}

		public bool Calm(float dt, FactionAlignment self)
		{
			if (grudgeTime <= 0f)
			{
				return true;
			}
			grudgeTime = Mathf.Max(0f, grudgeTime - dt);
			if (grudgeTime == 0f)
			{
				if (FactionManager.Instance.GetDisposition(self.Alignment, target.Alignment) != FactionManager.Disposition.Attack)
				{
					PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, UI.GAMEOBJECTEFFECTS.FORGAVEATTACKER, self.transform, 2f, track_target: true);
				}
				Clear();
				return true;
			}
			return false;
		}

		public void Clear()
		{
			grudgeTime = 0f;
			target = null;
		}

		public bool IsValidRevengeTarget(bool isDuplicant)
		{
			if (target != null && target.IsAlignmentActive() && (target.health == null || !target.health.IsDefeated()))
			{
				if (isDuplicant)
				{
					return !target.IsPlayerTargeted();
				}
				return true;
			}
			return false;
		}
	}

	public new class Instance : GameInstance
	{
		public FactionAlignment alignment;

		private Navigator navigator;

		public ChoreDriver choreDriver;

		private Health health;

		private ChoreConsumer choreConsumer;

		public Grudge revengeThreat;

		private GameObject mainThreat;

		private List<FactionAlignment> threats = new List<FactionAlignment>();

		private Action<object> refreshThreatDelegate;

		public GameObject MainThreat => mainThreat;

		public bool IAmADuplicant => alignment.Alignment == FactionManager.FactionID.Duplicant;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			alignment = master.GetComponent<FactionAlignment>();
			navigator = master.GetComponent<Navigator>();
			choreDriver = master.GetComponent<ChoreDriver>();
			health = master.GetComponent<Health>();
			choreConsumer = master.GetComponent<ChoreConsumer>();
			refreshThreatDelegate = RefreshThreat;
		}

		public void ClearMainThreat()
		{
			SetMainThreat(null);
		}

		public void SetMainThreat(GameObject threat)
		{
			if (threat == mainThreat)
			{
				return;
			}
			if (mainThreat != null)
			{
				mainThreat.Unsubscribe(1623392196, refreshThreatDelegate);
				mainThreat.Unsubscribe(1969584890, refreshThreatDelegate);
				if (threat == null)
				{
					Trigger(2144432245);
				}
			}
			if (mainThreat != null)
			{
				mainThreat.Unsubscribe(1623392196, refreshThreatDelegate);
				mainThreat.Unsubscribe(1969584890, refreshThreatDelegate);
			}
			mainThreat = threat;
			if (mainThreat != null)
			{
				mainThreat.Subscribe(1623392196, refreshThreatDelegate);
				mainThreat.Subscribe(1969584890, refreshThreatDelegate);
			}
		}

		public void OnSafe(object data)
		{
			if (revengeThreat.target != null)
			{
				if (!revengeThreat.target.GetComponent<FactionAlignment>().IsAlignmentActive())
				{
					revengeThreat.Clear();
				}
				ClearMainThreat();
			}
		}

		public void OnAttacked(object data)
		{
			FactionAlignment factionAlignment = (FactionAlignment)data;
			revengeThreat.Reset(factionAlignment);
			if (mainThreat == null)
			{
				SetMainThreat(factionAlignment.gameObject);
				GoToThreatened();
			}
			else if (!WillFight())
			{
				GoToThreatened();
			}
			if ((bool)factionAlignment.GetComponent<Bee>())
			{
				Chore currentChore = choreDriver.GetCurrentChore();
				if (currentChore != null && currentChore.gameObject.GetComponent<HiveWorkableEmpty>() != null)
				{
					currentChore.gameObject.GetComponent<HiveWorkableEmpty>().wasStung = true;
				}
			}
		}

		public bool WillFight()
		{
			if (choreConsumer != null)
			{
				if (!choreConsumer.IsPermittedByUser(Db.Get().ChoreGroups.Combat))
				{
					return false;
				}
				if (!choreConsumer.IsPermittedByTraits(Db.Get().ChoreGroups.Combat))
				{
					return false;
				}
			}
			return health.State < base.smi.def.fleethresholdState;
		}

		private void GotoThreatResponse()
		{
			Chore currentChore = base.smi.master.GetComponent<ChoreDriver>().GetCurrentChore();
			if (WillFight() && mainThreat.GetComponent<FactionAlignment>().IsPlayerTargeted())
			{
				base.smi.GoTo(base.smi.sm.threatened.duplicant.ShouldFight);
			}
			else if (currentChore == null || currentChore.target == null || currentChore.target == base.master || !(currentChore.target.GetComponent<Pickupable>() != null))
			{
				base.smi.GoTo(base.smi.sm.threatened.duplicant.ShoudFlee);
			}
		}

		public void GoToThreatened()
		{
			if (IAmADuplicant)
			{
				GotoThreatResponse();
			}
			else
			{
				base.smi.GoTo(base.sm.threatened.creature);
			}
		}

		public void Cleanup(object data)
		{
			if ((bool)mainThreat)
			{
				mainThreat.Unsubscribe(1623392196, refreshThreatDelegate);
				mainThreat.Unsubscribe(1969584890, refreshThreatDelegate);
			}
		}

		public void RefreshThreat(object data)
		{
			if (IsRunning())
			{
				if (base.smi.CheckForThreats())
				{
					GoToThreatened();
				}
				else if (base.smi.GetCurrentState() != base.sm.safe)
				{
					Trigger(-21431934);
					base.smi.GoTo(base.sm.safe);
				}
			}
		}

		public bool CheckForThreats()
		{
			GameObject gameObject = ((!revengeThreat.IsValidRevengeTarget(IAmADuplicant)) ? FindThreat() : revengeThreat.target.gameObject);
			SetMainThreat(gameObject);
			return gameObject != null;
		}

		public GameObject FindThreat()
		{
			threats.Clear();
			if (base.isMasterNull)
			{
				return null;
			}
			bool flag = WillFight();
			ListPool<ScenePartitionerEntry, ThreatMonitor>.PooledList pooledList = ListPool<ScenePartitionerEntry, ThreatMonitor>.Allocate();
			int radius = 20;
			Extents extents = new Extents(Grid.PosToCell(base.gameObject), radius);
			GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.attackableEntitiesLayer, pooledList);
			for (int i = 0; i < pooledList.Count; i++)
			{
				FactionAlignment factionAlignment = pooledList[i].obj as FactionAlignment;
				if (factionAlignment.transform == null || factionAlignment == alignment || !factionAlignment.IsAlignmentActive() || FactionManager.Instance.GetDisposition(alignment.Alignment, factionAlignment.Alignment) != FactionManager.Disposition.Attack)
				{
					continue;
				}
				if (base.def.friendlyCreatureTags != null)
				{
					bool flag2 = false;
					Tag[] friendlyCreatureTags = base.def.friendlyCreatureTags;
					foreach (Tag tag in friendlyCreatureTags)
					{
						if (factionAlignment.HasTag(tag))
						{
							flag2 = true;
						}
					}
					if (flag2)
					{
						continue;
					}
				}
				if (navigator.CanReach(factionAlignment.attackable))
				{
					threats.Add(factionAlignment);
				}
			}
			pooledList.Recycle();
			if (IAmADuplicant && flag)
			{
				for (int k = 0; k < 6; k++)
				{
					if (k == 0)
					{
						continue;
					}
					foreach (FactionAlignment member in FactionManager.Instance.GetFaction((FactionManager.FactionID)k).Members)
					{
						if (member.IsPlayerTargeted() && !member.health.IsDefeated() && !threats.Contains(member) && navigator.CanReach(member.attackable))
						{
							threats.Add(member);
						}
					}
				}
			}
			if (threats.Count == 0)
			{
				return null;
			}
			return PickBestTarget(threats);
		}

		public GameObject PickBestTarget(List<FactionAlignment> threats)
		{
			float num = 1f;
			Vector2 a = base.gameObject.transform.GetPosition();
			GameObject result = null;
			float num2 = float.PositiveInfinity;
			for (int num3 = threats.Count - 1; num3 >= 0; num3--)
			{
				FactionAlignment factionAlignment = threats[num3];
				float num4 = Vector2.Distance(a, factionAlignment.transform.GetPosition()) / num;
				if (num4 < num2)
				{
					num2 = num4;
					result = factionAlignment.gameObject;
				}
			}
			return result;
		}
	}

	private FactionAlignment alignment;

	private Navigator navigator;

	public State safe;

	public ThreatenedStates threatened;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = safe;
		root.EventHandler(GameHashes.SafeFromThreats, delegate(Instance smi, object d)
		{
			smi.OnSafe(d);
		}).EventHandler(GameHashes.Attacked, delegate(Instance smi, object d)
		{
			smi.OnAttacked(d);
		}).EventHandler(GameHashes.ObjectDestroyed, delegate(Instance smi, object d)
		{
			smi.Cleanup(d);
		});
		safe.Enter(delegate(Instance smi)
		{
			smi.revengeThreat.Clear();
			smi.RefreshThreat(null);
		}).Update("safe", delegate(Instance smi, float dt)
		{
			smi.RefreshThreat(null);
		}, UpdateRate.SIM_1000ms, load_balance: true);
		threatened.duplicant.Transition(safe, (Instance smi) => !smi.CheckForThreats());
		threatened.duplicant.ShouldFight.ToggleChore(CreateAttackChore, safe).Update("DupeUpdateTarget", DupeUpdateTarget);
		threatened.duplicant.ShoudFlee.ToggleChore(CreateFleeChore, safe);
		threatened.creature.ToggleBehaviour(GameTags.Creatures.Flee, (Instance smi) => !smi.WillFight(), delegate(Instance smi)
		{
			smi.GoTo(safe);
		}).ToggleBehaviour(GameTags.Creatures.Attack, (Instance smi) => smi.WillFight(), delegate(Instance smi)
		{
			smi.GoTo(safe);
		}).Update("Threatened", CritterUpdateThreats);
	}

	private static void DupeUpdateTarget(Instance smi, float dt)
	{
		if (smi.MainThreat == null || !smi.MainThreat.GetComponent<FactionAlignment>().IsPlayerTargeted())
		{
			smi.Trigger(2144432245);
		}
	}

	private static void CritterUpdateThreats(Instance smi, float dt)
	{
		if (!smi.isMasterNull)
		{
			if (smi.revengeThreat.target != null && smi.revengeThreat.Calm(dt, smi.alignment))
			{
				smi.Trigger(-21431934);
			}
			else if (!smi.CheckForThreats())
			{
				smi.GoTo(smi.sm.safe);
			}
		}
	}

	private Chore CreateAttackChore(Instance smi)
	{
		return new AttackChore(smi.master, smi.MainThreat);
	}

	private Chore CreateFleeChore(Instance smi)
	{
		return new FleeChore(smi.master, smi.MainThreat);
	}
}
