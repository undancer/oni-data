using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class CropTendingStates : GameStateMachine<CropTendingStates, CropTendingStates.Instance, IStateMachineTarget, CropTendingStates.Def>
{
	public class AnimSet
	{
		public string crop_tending_pre;

		public string crop_tending;

		public string crop_tending_pst;

		public string[] hide_symbols_after_pre;
	}

	public class CropTendingEventData
	{
		public GameObject source;

		public Tag cropId;
	}

	public class Def : BaseDef
	{
		public string effectId;

		public string[] ignoreEffectGroup;

		public Dictionary<Tag, int> interests = new Dictionary<Tag, int>();

		public Dictionary<Tag, AnimSet> animSetOverrides = new Dictionary<Tag, AnimSet>();
	}

	public new class Instance : GameInstance
	{
		public Effect effect;

		public int moveCell;

		public AnimSet animSet;

		public bool tendedSucceeded;

		public bool[] symbolStates;

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToTendCrops);
			effect = Db.Get().effects.TryGet(base.smi.def.effectId);
		}
	}

	public class TendingStates : State
	{
		public State pre;

		public State tend;

		public State pst;
	}

	private const int MAX_NAVIGATE_DISTANCE = 100;

	private const int MAX_SQR_EUCLIDEAN_DISTANCE = 625;

	private static AnimSet defaultAnimSet = new AnimSet
	{
		crop_tending_pre = "crop_tending_pre",
		crop_tending = "crop_tending_loop",
		crop_tending_pst = "crop_tending_pst"
	};

	public TargetParameter targetCrop;

	private State findCrop;

	private State moveToCrop;

	private TendingStates tendCrop;

	private State behaviourcomplete;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = findCrop;
		root.Exit(delegate(Instance smi)
		{
			UnreserveCrop(smi);
			if (!smi.tendedSucceeded)
			{
				RestoreSymbolsVisibility(smi);
			}
		});
		findCrop.Enter(delegate(Instance smi)
		{
			FindCrop(smi);
			if (smi.sm.targetCrop.Get(smi) == null)
			{
				smi.GoTo(behaviourcomplete);
			}
			else
			{
				ReserverCrop(smi);
				smi.GoTo(moveToCrop);
			}
		});
		moveToCrop.ToggleStatusItem(CREATURES.STATUSITEMS.DIVERGENT_WILL_TEND.NAME, CREATURES.STATUSITEMS.DIVERGENT_WILL_TEND.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main).MoveTo((Instance smi) => smi.moveCell, tendCrop, behaviourcomplete).ParamTransition(targetCrop, behaviourcomplete, (Instance smi, GameObject p) => targetCrop.Get(smi) == null);
		tendCrop.DefaultState(tendCrop.pre).ToggleStatusItem(CREATURES.STATUSITEMS.DIVERGENT_TENDING.NAME, CREATURES.STATUSITEMS.DIVERGENT_TENDING.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main).ParamTransition(targetCrop, behaviourcomplete, (Instance smi, GameObject p) => targetCrop.Get(smi) == null)
			.Enter(delegate(Instance smi)
			{
				smi.animSet = GetCropTendingAnimSet(smi);
				StoreSymbolsVisibility(smi);
			});
		tendCrop.pre.Face(targetCrop).PlayAnim((Instance smi) => smi.animSet.crop_tending_pre).OnAnimQueueComplete(tendCrop.tend);
		tendCrop.tend.Enter(delegate(Instance smi)
		{
			SetSymbolsVisibility(smi, isVisible: false);
		}).QueueAnim((Instance smi) => smi.animSet.crop_tending).OnAnimQueueComplete(tendCrop.pst);
		tendCrop.pst.QueueAnim((Instance smi) => smi.animSet.crop_tending_pst).OnAnimQueueComplete(behaviourcomplete).Exit(delegate(Instance smi)
		{
			GameObject gameObject = smi.sm.targetCrop.Get(smi);
			if (gameObject != null)
			{
				if (smi.effect != null)
				{
					gameObject.GetComponent<Effects>().Add(smi.effect, should_save: true);
				}
				smi.tendedSucceeded = true;
				CropTendingEventData data = new CropTendingEventData
				{
					source = smi.gameObject,
					cropId = smi.sm.targetCrop.Get(smi).PrefabID()
				};
				smi.sm.targetCrop.Get(smi).Trigger(90606262, data);
				smi.Trigger(90606262, data);
			}
		});
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToTendCrops);
	}

	private AnimSet GetCropTendingAnimSet(Instance smi)
	{
		if (smi.def.animSetOverrides.TryGetValue(targetCrop.Get(smi).PrefabID(), out var value))
		{
			return value;
		}
		return defaultAnimSet;
	}

	private void FindCrop(Instance smi)
	{
		Navigator component = smi.GetComponent<Navigator>();
		Crop crop = null;
		int moveCell = Grid.InvalidCell;
		int num = 100;
		int num2 = -1;
		foreach (Crop worldItem in Components.Crops.GetWorldItems(smi.gameObject.GetMyWorldId()))
		{
			if (smi.effect != null)
			{
				Effects component2 = worldItem.GetComponent<Effects>();
				if (component2 != null)
				{
					bool flag = false;
					string[] ignoreEffectGroup = smi.def.ignoreEffectGroup;
					foreach (string effect_id in ignoreEffectGroup)
					{
						if (component2.HasEffect(effect_id))
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						continue;
					}
				}
			}
			Growing component3 = worldItem.GetComponent<Growing>();
			if ((component3 != null && component3.IsGrown()) || worldItem.HasTag(GameTags.Creatures.ReservedByCreature) || Vector2.SqrMagnitude(worldItem.transform.position - smi.transform.position) > 625f)
			{
				continue;
			}
			smi.def.interests.TryGetValue(worldItem.PrefabID(), out var value);
			if (value < num2)
			{
				continue;
			}
			bool flag2 = value > num2;
			int cell = Grid.PosToCell(worldItem);
			int[] array = new int[2]
			{
				Grid.CellLeft(cell),
				Grid.CellRight(cell)
			};
			int num3 = 100;
			int num4 = Grid.InvalidCell;
			for (int j = 0; j < array.Length; j++)
			{
				if (Grid.IsValidCell(array[j]))
				{
					int navigationCost = component.GetNavigationCost(array[j]);
					if (navigationCost != -1 && navigationCost < num3)
					{
						num3 = navigationCost;
						num4 = array[j];
					}
				}
			}
			if (num3 != -1 && num4 != Grid.InvalidCell && (flag2 || num3 < num))
			{
				moveCell = num4;
				num = num3;
				num2 = value;
				crop = worldItem;
			}
		}
		GameObject value2 = ((crop != null) ? crop.gameObject : null);
		smi.sm.targetCrop.Set(value2, smi);
		smi.moveCell = moveCell;
	}

	private void ReserverCrop(Instance smi)
	{
		GameObject gameObject = smi.sm.targetCrop.Get(smi);
		if (gameObject != null)
		{
			DebugUtil.Assert(!gameObject.HasTag(GameTags.Creatures.ReservedByCreature));
			gameObject.AddTag(GameTags.Creatures.ReservedByCreature);
		}
	}

	private void UnreserveCrop(Instance smi)
	{
		GameObject gameObject = smi.sm.targetCrop.Get(smi);
		if (gameObject != null)
		{
			gameObject.RemoveTag(GameTags.Creatures.ReservedByCreature);
		}
	}

	private void SetSymbolsVisibility(Instance smi, bool isVisible)
	{
		if (!(targetCrop.Get(smi) != null))
		{
			return;
		}
		string[] hide_symbols_after_pre = smi.animSet.hide_symbols_after_pre;
		if (hide_symbols_after_pre == null)
		{
			return;
		}
		KAnimControllerBase component = targetCrop.Get(smi).GetComponent<KAnimControllerBase>();
		if (component != null)
		{
			string[] array = hide_symbols_after_pre;
			foreach (string text in array)
			{
				component.SetSymbolVisiblity(text, isVisible);
			}
		}
	}

	private void StoreSymbolsVisibility(Instance smi)
	{
		if (!(targetCrop.Get(smi) != null))
		{
			return;
		}
		string[] hide_symbols_after_pre = smi.animSet.hide_symbols_after_pre;
		if (hide_symbols_after_pre == null)
		{
			return;
		}
		KAnimControllerBase component = targetCrop.Get(smi).GetComponent<KAnimControllerBase>();
		if (component != null)
		{
			smi.symbolStates = new bool[hide_symbols_after_pre.Length];
			for (int i = 0; i < hide_symbols_after_pre.Length; i++)
			{
				smi.symbolStates[i] = component.GetSymbolVisiblity(hide_symbols_after_pre[i]);
			}
		}
	}

	private void RestoreSymbolsVisibility(Instance smi)
	{
		if (!(targetCrop.Get(smi) != null) || smi.symbolStates == null)
		{
			return;
		}
		string[] hide_symbols_after_pre = smi.animSet.hide_symbols_after_pre;
		if (hide_symbols_after_pre == null)
		{
			return;
		}
		KAnimControllerBase component = targetCrop.Get(smi).GetComponent<KAnimControllerBase>();
		if (component != null)
		{
			for (int i = 0; i < hide_symbols_after_pre.Length; i++)
			{
				component.SetSymbolVisiblity(hide_symbols_after_pre[i], smi.symbolStates[i]);
			}
		}
	}
}
