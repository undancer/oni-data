using System.Collections.Generic;
using Klei.AI;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/DupeGreetingManager")]
public class DupeGreetingManager : KMonoBehaviour, ISim200ms
{
	public class Tuning : TuningData<Tuning>
	{
		public float cyclesBeforeFirstGreeting;

		public float greetingDelayMultiplier;
	}

	private class GreetingUnit
	{
		public MinionIdentity minion;

		public Reactable reactable;

		public GreetingUnit(MinionIdentity minion, Reactable reactable)
		{
			this.minion = minion;
			this.reactable = reactable;
		}
	}

	private class GreetingSetup
	{
		public int cell;

		public GreetingUnit A;

		public GreetingUnit B;
	}

	private const float COOLDOWN_TIME = 720f;

	private Dictionary<int, MinionIdentity> candidateCells;

	private List<GreetingSetup> activeSetups;

	private Dictionary<MinionIdentity, float> cooldowns;

	private static readonly List<string> waveAnims = new List<string>
	{
		"anim_react_wave_kanim",
		"anim_react_wave_shy_kanim",
		"anim_react_fingerguns_kanim"
	};

	protected override void OnPrefabInit()
	{
		candidateCells = new Dictionary<int, MinionIdentity>();
		activeSetups = new List<GreetingSetup>();
		cooldowns = new Dictionary<MinionIdentity, float>();
	}

	public void Sim200ms(float dt)
	{
		if (GameClock.Instance.GetTime() / 600f < TuningData<Tuning>.Get().cyclesBeforeFirstGreeting)
		{
			return;
		}
		for (int num = activeSetups.Count - 1; num >= 0; num--)
		{
			GreetingSetup greetingSetup = activeSetups[num];
			if (!ValidNavigatingMinion(greetingSetup.A.minion) || !ValidOppositionalMinion(greetingSetup.A.minion, greetingSetup.B.minion))
			{
				greetingSetup.A.reactable.Cleanup();
				greetingSetup.B.reactable.Cleanup();
				activeSetups.RemoveAt(num);
			}
		}
		candidateCells.Clear();
		foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
		{
			if ((cooldowns.ContainsKey(item) && GameClock.Instance.GetTime() - cooldowns[item] < 720f * TuningData<Tuning>.Get().greetingDelayMultiplier) || !ValidNavigatingMinion(item))
			{
				continue;
			}
			for (int i = 0; i <= 2; i++)
			{
				int offsetCell = GetOffsetCell(item, i);
				if (candidateCells.ContainsKey(offsetCell) && ValidOppositionalMinion(item, candidateCells[offsetCell]))
				{
					BeginNewGreeting(item, candidateCells[offsetCell], offsetCell);
					break;
				}
				candidateCells[offsetCell] = item;
			}
		}
	}

	private int GetOffsetCell(MinionIdentity minion, int offset)
	{
		Facing component = minion.GetComponent<Facing>();
		return component.GetFacing() ? Grid.OffsetCell(Grid.PosToCell(minion), -offset, 0) : Grid.OffsetCell(Grid.PosToCell(minion), offset, 0);
	}

	private bool ValidNavigatingMinion(MinionIdentity minion)
	{
		if (minion == null)
		{
			return false;
		}
		Navigator component = minion.GetComponent<Navigator>();
		return component != null && component.IsMoving() && component.CurrentNavType == NavType.Floor;
	}

	private bool ValidOppositionalMinion(MinionIdentity reference_minion, MinionIdentity minion)
	{
		if (reference_minion == null)
		{
			return false;
		}
		if (minion == null)
		{
			return false;
		}
		Facing component = minion.GetComponent<Facing>();
		Facing component2 = reference_minion.GetComponent<Facing>();
		return ValidNavigatingMinion(minion) && component != null && component2 != null && component.GetFacing() != component2.GetFacing();
	}

	private void BeginNewGreeting(MinionIdentity minion_a, MinionIdentity minion_b, int cell)
	{
		GreetingSetup greetingSetup = new GreetingSetup();
		greetingSetup.cell = cell;
		greetingSetup.A = new GreetingUnit(minion_a, GetReactable(minion_a));
		greetingSetup.B = new GreetingUnit(minion_b, GetReactable(minion_b));
		activeSetups.Add(greetingSetup);
	}

	private Reactable GetReactable(MinionIdentity minion)
	{
		return new SelfEmoteReactable(minion.gameObject, "NavigatorPassingGreeting", Db.Get().ChoreTypes.Emote, waveAnims[Random.Range(0, waveAnims.Count)], 1000f).AddStep(new EmoteReactable.EmoteStep
		{
			anim = "react",
			startcb = BeginReacting
		}).AddThought(Db.Get().Thoughts.Chatty);
	}

	private void BeginReacting(GameObject minionGO)
	{
		if (minionGO == null)
		{
			return;
		}
		MinionIdentity component = minionGO.GetComponent<MinionIdentity>();
		Vector3 vector = Vector3.zero;
		foreach (GreetingSetup activeSetup in activeSetups)
		{
			if (activeSetup.A.minion == component)
			{
				if (activeSetup.B.minion != null)
				{
					vector = activeSetup.B.minion.transform.GetPosition();
				}
				break;
			}
			if (activeSetup.B.minion == component)
			{
				if (activeSetup.A.minion != null)
				{
					vector = activeSetup.A.minion.transform.GetPosition();
				}
				break;
			}
		}
		Facing component2 = minionGO.GetComponent<Facing>();
		component2.SetFacing(vector.x < minionGO.transform.GetPosition().x);
		Effects component3 = minionGO.GetComponent<Effects>();
		component3.Add("Greeting", should_save: true);
		cooldowns[component] = GameClock.Instance.GetTime();
	}
}
