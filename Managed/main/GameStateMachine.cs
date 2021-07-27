using System;
using System.Collections.Generic;
using Klei.AI;
using UnityEngine;

public abstract class GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType> : StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType> where StateMachineType : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType> where StateMachineInstanceType : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.GameInstance where MasterType : IStateMachineTarget
{
	public class PreLoopPostState : State
	{
		public State pre;

		public State loop;

		public State pst;
	}

	public class WorkingState : State
	{
		public State waiting;

		public State working_pre;

		public State working_loop;

		public State working_pst;
	}

	public class GameInstance : GenericInstance
	{
		public void Queue(string anim, KAnim.PlayMode mode = KAnim.PlayMode.Once)
		{
			base.smi.GetComponent<KBatchedAnimController>().Queue(anim, mode);
		}

		public void Play(string anim, KAnim.PlayMode mode = KAnim.PlayMode.Once)
		{
			base.smi.GetComponent<KBatchedAnimController>().Play(anim, mode);
		}

		public GameInstance(MasterType master, DefType def)
			: base(master)
		{
			base.def = def;
		}

		public GameInstance(MasterType master)
			: base(master)
		{
		}
	}

	public class TagTransitionData : Transition
	{
		private Tag[] tags;

		private bool onRemove;

		private TargetParameter target;

		private bool is_executing;

		public TagTransitionData(State source_state, State target_state, int idx, Tag[] tags, bool on_remove, TargetParameter target)
			: base(tags.ToString(), (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State)source_state, (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State)target_state, idx, (ConditionCallback)null)
		{
			this.tags = tags;
			onRemove = on_remove;
			this.target = target;
		}

		public override void Evaluate(StateMachineInstanceType smi)
		{
			if (!onRemove)
			{
				if (!HasAllTags(smi))
				{
					return;
				}
			}
			else if (HasAnyTags(smi))
			{
				return;
			}
			ExecuteTransition(smi);
		}

		private bool HasAllTags(StateMachineInstanceType smi)
		{
			KPrefabID component = target.Get(smi).GetComponent<KPrefabID>();
			for (int i = 0; i < tags.Length; i++)
			{
				if (!component.HasTag(tags[i]))
				{
					return false;
				}
			}
			return true;
		}

		private bool HasAnyTags(StateMachineInstanceType smi)
		{
			KPrefabID component = target.Get(smi).GetComponent<KPrefabID>();
			for (int i = 0; i < tags.Length; i++)
			{
				if (component.HasTag(tags[i]))
				{
					return true;
				}
			}
			return false;
		}

		private void ExecuteTransition(StateMachineInstanceType smi)
		{
			if (!is_executing)
			{
				is_executing = true;
				smi.GoTo(targetState);
				is_executing = false;
			}
		}

		private void OnCallback(StateMachineInstanceType smi)
		{
			if (target.Get(smi) == null)
			{
				return;
			}
			if (!onRemove)
			{
				if (!HasAllTags(smi))
				{
					return;
				}
			}
			else if (HasAnyTags(smi))
			{
				return;
			}
			ExecuteTransition(smi);
		}

		public override Context Register(StateMachineInstanceType smi)
		{
			Context result = base.Register(smi);
			result.handlerId = target.Get(smi).Subscribe(-1582839653, delegate
			{
				OnCallback(smi);
			});
			return result;
		}

		public override void Unregister(StateMachineInstanceType smi, Context context)
		{
			base.Unregister(smi, context);
			if (target.Get(smi) != null)
			{
				target.Get(smi).Unsubscribe(context.handlerId);
			}
		}
	}

	public class EventTransitionData : Transition
	{
		private GameHashes evtId;

		private TargetParameter target;

		private Func<StateMachineInstanceType, KMonoBehaviour> globalEventSystemCallback;

		public EventTransitionData(State source_state, State target_state, int idx, GameHashes evt, Func<StateMachineInstanceType, KMonoBehaviour> global_event_system_callback, ConditionCallback condition, TargetParameter target)
			: base(evt.ToString(), (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State)source_state, (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State)target_state, idx, condition)
		{
			evtId = evt;
			this.target = target;
			globalEventSystemCallback = global_event_system_callback;
		}

		public override void Evaluate(StateMachineInstanceType smi)
		{
			if (condition != null && condition(smi))
			{
				ExecuteTransition(smi);
			}
		}

		private void ExecuteTransition(StateMachineInstanceType smi)
		{
			smi.GoTo(targetState);
		}

		private void OnCallback(StateMachineInstanceType smi)
		{
			if (condition == null || condition(smi))
			{
				ExecuteTransition(smi);
			}
		}

		public override Context Register(StateMachineInstanceType smi)
		{
			Context result = base.Register(smi);
			Action<object> handler = delegate
			{
				OnCallback(smi);
			};
			GameObject gameObject = null;
			if (globalEventSystemCallback != null)
			{
				gameObject = globalEventSystemCallback(smi).gameObject;
			}
			else
			{
				gameObject = target.Get(smi);
				if (gameObject == null)
				{
					throw new InvalidOperationException("TargetParameter: " + target.name + " is null");
				}
			}
			result.handlerId = gameObject.Subscribe((int)evtId, handler);
			return result;
		}

		public override void Unregister(StateMachineInstanceType smi, Context context)
		{
			base.Unregister(smi, context);
			GameObject gameObject = null;
			if (globalEventSystemCallback != null)
			{
				KMonoBehaviour kMonoBehaviour = globalEventSystemCallback(smi);
				if (kMonoBehaviour != null)
				{
					gameObject = kMonoBehaviour.gameObject;
				}
			}
			else
			{
				gameObject = target.Get(smi);
			}
			if (gameObject != null)
			{
				gameObject.Unsubscribe(context.handlerId);
			}
		}
	}

	public new class State : StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State
	{
		private class TransitionUpdater : UpdateBucketWithUpdater<StateMachineInstanceType>.IUpdater
		{
			private Transition.ConditionCallback condition;

			private State state;

			public TransitionUpdater(Transition.ConditionCallback condition, State state)
			{
				this.condition = condition;
				this.state = state;
			}

			public void Update(StateMachineInstanceType smi, float dt)
			{
				if (condition(smi))
				{
					smi.GoTo(state);
				}
			}
		}

		[DoNotAutoCreate]
		private TargetParameter stateTarget;

		public State root => this;

		public State master
		{
			get
			{
				stateTarget = sm.masterTarget;
				return this;
			}
		}

		private TargetParameter GetStateTarget()
		{
			if (stateTarget == null)
			{
				if (parent != null)
				{
					return ((State)parent).GetStateTarget();
				}
				TargetParameter targetParameter = sm.stateTarget;
				if (targetParameter == null)
				{
					return sm.masterTarget;
				}
				return targetParameter;
			}
			return stateTarget;
		}

		public int CreateDataTableEntry()
		{
			return sm.dataTableSize++;
		}

		public int CreateUpdateTableEntry()
		{
			return sm.updateTableSize++;
		}

		public State DoNothing()
		{
			return this;
		}

		private static List<Action> AddAction(string name, Callback callback, List<Action> actions, bool add_to_end)
		{
			if (actions == null)
			{
				actions = new List<Action>();
			}
			Action item = new Action(name, callback);
			if (add_to_end)
			{
				actions.Add(item);
			}
			else
			{
				actions.Insert(0, item);
			}
			return actions;
		}

		public State Target(TargetParameter target)
		{
			stateTarget = target;
			return this;
		}

		public State Update(Action<StateMachineInstanceType, float> callback, UpdateRate update_rate = UpdateRate.SIM_200ms, bool load_balance = false)
		{
			return Update(sm.name + "." + name, callback, update_rate, load_balance);
		}

		public State BatchUpdate(UpdateBucketWithUpdater<StateMachineInstanceType>.BatchUpdateDelegate batch_update, UpdateRate update_rate = UpdateRate.SIM_200ms)
		{
			return BatchUpdate(sm.name + "." + name, batch_update, update_rate);
		}

		public State Enter(Callback callback)
		{
			return Enter("Enter", callback);
		}

		public State Exit(Callback callback)
		{
			return Exit("Exit", callback);
		}

		private State InternalUpdate(string name, UpdateBucketWithUpdater<StateMachineInstanceType>.IUpdater bucket_updater, UpdateRate update_rate, bool load_balance, UpdateBucketWithUpdater<StateMachineInstanceType>.BatchUpdateDelegate batch_update = null)
		{
			int updateTableIdx = CreateUpdateTableEntry();
			if (updateActions == null)
			{
				updateActions = new List<UpdateAction>();
			}
			UpdateAction item = default(UpdateAction);
			item.updateTableIdx = updateTableIdx;
			item.updateRate = update_rate;
			item.updater = bucket_updater;
			int num = 1;
			if (load_balance)
			{
				num = Singleton<StateMachineUpdater>.Instance.GetFrameCount(update_rate);
			}
			item.buckets = new StateMachineUpdater.BaseUpdateBucket[num];
			for (int i = 0; i < num; i++)
			{
				UpdateBucketWithUpdater<StateMachineInstanceType> updateBucketWithUpdater = new UpdateBucketWithUpdater<StateMachineInstanceType>(name);
				updateBucketWithUpdater.batch_update_delegate = batch_update;
				Singleton<StateMachineUpdater>.Instance.AddBucket(update_rate, updateBucketWithUpdater);
				item.buckets[i] = updateBucketWithUpdater;
			}
			updateActions.Add(item);
			return this;
		}

		public State UpdateTransition(State destination_state, Func<StateMachineInstanceType, float, bool> callback, UpdateRate update_rate = UpdateRate.SIM_200ms, bool load_balance = false)
		{
			Action<StateMachineInstanceType, float> checkCallback = delegate(StateMachineInstanceType smi, float dt)
			{
				if (callback(smi, dt))
				{
					smi.GoTo(destination_state);
				}
			};
			Enter(delegate(StateMachineInstanceType smi)
			{
				checkCallback(smi, 0f);
			});
			Update(checkCallback, update_rate, load_balance);
			return this;
		}

		public State Update(string name, Action<StateMachineInstanceType, float> callback, UpdateRate update_rate = UpdateRate.SIM_200ms, bool load_balance = false)
		{
			return InternalUpdate(name, new BucketUpdater<StateMachineInstanceType>(callback), update_rate, load_balance);
		}

		public State BatchUpdate(string name, UpdateBucketWithUpdater<StateMachineInstanceType>.BatchUpdateDelegate batch_update, UpdateRate update_rate = UpdateRate.SIM_200ms)
		{
			return InternalUpdate(name, null, update_rate, load_balance: false, batch_update);
		}

		public State FastUpdate(string name, UpdateBucketWithUpdater<StateMachineInstanceType>.IUpdater updater, UpdateRate update_rate = UpdateRate.SIM_200ms, bool load_balance = false)
		{
			return InternalUpdate(name, updater, update_rate, load_balance);
		}

		public State Enter(string name, Callback callback)
		{
			enterActions = AddAction(name, callback, enterActions, add_to_end: true);
			return this;
		}

		public State Exit(string name, Callback callback)
		{
			exitActions = AddAction(name, callback, exitActions, add_to_end: false);
			return this;
		}

		public State Toggle(string name, Callback enter_callback, Callback exit_callback)
		{
			int data_idx = CreateDataTableEntry();
			Enter("ToggleEnter(" + name + ")", delegate(StateMachineInstanceType smi)
			{
				smi.dataTable[data_idx] = GameStateMachineHelper.HasToggleEnteredFlag;
				enter_callback(smi);
			});
			Exit("ToggleExit(" + name + ")", delegate(StateMachineInstanceType smi)
			{
				if (smi.dataTable[data_idx] != null)
				{
					smi.dataTable[data_idx] = null;
					exit_callback(smi);
				}
			});
			return this;
		}

		private void Break(StateMachineInstanceType smi)
		{
		}

		public State BreakOnEnter()
		{
			return Enter(delegate(StateMachineInstanceType smi)
			{
				Break(smi);
			});
		}

		public State BreakOnExit()
		{
			return Exit(delegate(StateMachineInstanceType smi)
			{
				Break(smi);
			});
		}

		public State AddEffect(string effect_name)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("AddEffect(" + effect_name + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Effects>(smi).Add(effect_name, should_save: true);
			});
			return this;
		}

		public State ToggleAnims(Func<StateMachineInstanceType, HashedString> chooser_callback)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("EnableAnims()", delegate(StateMachineInstanceType smi)
			{
				HashedString hashedString2 = chooser_callback(smi);
				if (!(hashedString2 == null) && hashedString2.IsValid)
				{
					KAnimFile anim2 = Assets.GetAnim(hashedString2);
					if (anim2 == null)
					{
						HashedString hashedString3 = hashedString2;
						Debug.LogWarning("Missing anims: " + hashedString3.ToString());
					}
					else
					{
						state_target.Get<KAnimControllerBase>(smi).AddAnimOverrides(anim2);
					}
				}
			});
			Exit("Disableanims()", delegate(StateMachineInstanceType smi)
			{
				HashedString hashedString = chooser_callback(smi);
				if (!(hashedString == null) && hashedString.IsValid)
				{
					KAnimFile anim = Assets.GetAnim(hashedString);
					if (anim != null)
					{
						state_target.Get<KAnimControllerBase>(smi).RemoveAnimOverrides(anim);
					}
				}
			});
			return this;
		}

		public State ToggleAnims(string anim_file, float priority = 0f, string RequiredDlc = "")
		{
			TargetParameter state_target = GetStateTarget();
			Toggle("ToggleAnims(" + anim_file + ")", delegate(StateMachineInstanceType smi)
			{
				if (DlcManager.IsContentActive(RequiredDlc))
				{
					KAnimFile anim2 = Assets.GetAnim(anim_file);
					if (anim2 == null)
					{
						Debug.LogError("Trying to add missing override anims:" + anim_file);
					}
					state_target.Get<KAnimControllerBase>(smi).AddAnimOverrides(anim2, priority);
				}
			}, delegate(StateMachineInstanceType smi)
			{
				if (DlcManager.IsContentActive(RequiredDlc))
				{
					KAnimFile anim = Assets.GetAnim(anim_file);
					state_target.Get<KAnimControllerBase>(smi).RemoveAnimOverrides(anim);
				}
			});
			return this;
		}

		public State ToggleAttributeModifier(string modifier_name, Func<StateMachineInstanceType, AttributeModifier> callback, Func<StateMachineInstanceType, bool> condition = null)
		{
			TargetParameter state_target = GetStateTarget();
			int data_idx = CreateDataTableEntry();
			Enter("AddAttributeModifier( " + modifier_name + " )", delegate(StateMachineInstanceType smi)
			{
				if (condition == null || condition(smi))
				{
					AttributeModifier attributeModifier = callback(smi);
					DebugUtil.Assert(smi.dataTable[data_idx] == null);
					smi.dataTable[data_idx] = attributeModifier;
					state_target.Get(smi).GetAttributes().Add(attributeModifier);
				}
			});
			Exit("RemoveAttributeModifier( " + modifier_name + " )", delegate(StateMachineInstanceType smi)
			{
				if (smi.dataTable[data_idx] != null)
				{
					AttributeModifier modifier = (AttributeModifier)smi.dataTable[data_idx];
					smi.dataTable[data_idx] = null;
					GameObject gameObject = state_target.Get(smi);
					if (gameObject != null)
					{
						gameObject.GetAttributes().Remove(modifier);
					}
				}
			});
			return this;
		}

		public State ToggleLoopingSound(string event_name, Func<StateMachineInstanceType, bool> condition = null, bool pause_on_game_pause = true, bool enable_culling = true, bool enable_camera_scaled_position = true)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("StartLoopingSound( " + event_name + " )", delegate(StateMachineInstanceType smi)
			{
				if (condition == null || condition(smi))
				{
					state_target.Get(smi).GetComponent<LoopingSounds>().StartSound(event_name, pause_on_game_pause, enable_culling, enable_camera_scaled_position);
				}
			});
			Exit("StopLoopingSound( " + event_name + " )", delegate(StateMachineInstanceType smi)
			{
				state_target.Get(smi).GetComponent<LoopingSounds>().StopSound(event_name);
			});
			return this;
		}

		public State ToggleLoopingSound(string state_label, Func<StateMachineInstanceType, string> event_name_callback, Func<StateMachineInstanceType, bool> condition = null)
		{
			TargetParameter state_target = GetStateTarget();
			int data_idx = CreateDataTableEntry();
			Enter("StartLoopingSound( " + state_label + " )", delegate(StateMachineInstanceType smi)
			{
				if (condition == null || condition(smi))
				{
					string text = event_name_callback(smi);
					smi.dataTable[data_idx] = text;
					state_target.Get(smi).GetComponent<LoopingSounds>().StartSound(text);
				}
			});
			Exit("StopLoopingSound( " + state_label + " )", delegate(StateMachineInstanceType smi)
			{
				if (smi.dataTable[data_idx] != null)
				{
					state_target.Get(smi).GetComponent<LoopingSounds>().StopSound((string)smi.dataTable[data_idx]);
					smi.dataTable[data_idx] = null;
				}
			});
			return this;
		}

		public State RefreshUserMenuOnEnter()
		{
			Enter("RefreshUserMenuOnEnter()", delegate(StateMachineInstanceType smi)
			{
				Game.Instance.userMenu.Refresh(smi.master.gameObject);
			});
			return this;
		}

		public State WorkableStartTransition(Func<StateMachineInstanceType, Workable> get_workable_callback, State target_state)
		{
			int data_idx = CreateDataTableEntry();
			Enter("Enter WorkableStartTransition(" + target_state.longName + ")", delegate(StateMachineInstanceType smi)
			{
				Workable workable2 = get_workable_callback(smi);
				if (workable2 != null)
				{
					Action<Workable.WorkableEvent> action = delegate(Workable.WorkableEvent evt)
					{
						if (evt == Workable.WorkableEvent.WorkStarted)
						{
							smi.GoTo(target_state);
						}
					};
					smi.dataTable[data_idx] = action;
					workable2.OnWorkableEventCB = (Action<Workable.WorkableEvent>)Delegate.Combine(workable2.OnWorkableEventCB, action);
				}
			});
			Exit("Exit WorkableStartTransition(" + target_state.longName + ")", delegate(StateMachineInstanceType smi)
			{
				Workable workable = get_workable_callback(smi);
				if (workable != null)
				{
					Action<Workable.WorkableEvent> value = (Action<Workable.WorkableEvent>)smi.dataTable[data_idx];
					smi.dataTable[data_idx] = null;
					workable.OnWorkableEventCB = (Action<Workable.WorkableEvent>)Delegate.Remove(workable.OnWorkableEventCB, value);
				}
			});
			return this;
		}

		public State WorkableStopTransition(Func<StateMachineInstanceType, Workable> get_workable_callback, State target_state)
		{
			int data_idx = CreateDataTableEntry();
			Enter("Enter WorkableStopTransition(" + target_state.longName + ")", delegate(StateMachineInstanceType smi)
			{
				Workable workable2 = get_workable_callback(smi);
				if (workable2 != null)
				{
					Action<Workable.WorkableEvent> action = delegate(Workable.WorkableEvent evt)
					{
						if (evt == Workable.WorkableEvent.WorkStopped)
						{
							smi.GoTo(target_state);
						}
					};
					smi.dataTable[data_idx] = action;
					workable2.OnWorkableEventCB = (Action<Workable.WorkableEvent>)Delegate.Combine(workable2.OnWorkableEventCB, action);
				}
			});
			Exit("Exit WorkableStopTransition(" + target_state.longName + ")", delegate(StateMachineInstanceType smi)
			{
				Workable workable = get_workable_callback(smi);
				if (workable != null)
				{
					Action<Workable.WorkableEvent> value = (Action<Workable.WorkableEvent>)smi.dataTable[data_idx];
					smi.dataTable[data_idx] = null;
					workable.OnWorkableEventCB = (Action<Workable.WorkableEvent>)Delegate.Remove(workable.OnWorkableEventCB, value);
				}
			});
			return this;
		}

		public State WorkableCompleteTransition(Func<StateMachineInstanceType, Workable> get_workable_callback, State target_state)
		{
			int data_idx = CreateDataTableEntry();
			Enter("Enter WorkableCompleteTransition(" + target_state.longName + ")", delegate(StateMachineInstanceType smi)
			{
				Workable workable2 = get_workable_callback(smi);
				if (workable2 != null)
				{
					Action<Workable.WorkableEvent> action = delegate(Workable.WorkableEvent evt)
					{
						if (evt == Workable.WorkableEvent.WorkCompleted)
						{
							smi.GoTo(target_state);
						}
					};
					smi.dataTable[data_idx] = action;
					workable2.OnWorkableEventCB = (Action<Workable.WorkableEvent>)Delegate.Combine(workable2.OnWorkableEventCB, action);
				}
			});
			Exit("Exit WorkableCompleteTransition(" + target_state.longName + ")", delegate(StateMachineInstanceType smi)
			{
				Workable workable = get_workable_callback(smi);
				if (workable != null)
				{
					Action<Workable.WorkableEvent> value = (Action<Workable.WorkableEvent>)smi.dataTable[data_idx];
					smi.dataTable[data_idx] = null;
					workable.OnWorkableEventCB = (Action<Workable.WorkableEvent>)Delegate.Remove(workable.OnWorkableEventCB, value);
				}
			});
			return this;
		}

		public State ToggleGravity()
		{
			TargetParameter state_target = GetStateTarget();
			int data_idx = CreateDataTableEntry();
			Enter("AddComponent<Gravity>()", delegate(StateMachineInstanceType smi)
			{
				GameObject gameObject = state_target.Get(smi);
				smi.dataTable[data_idx] = gameObject;
				GameComps.Gravities.Add(gameObject, Vector2.zero);
			});
			Exit("RemoveComponent<Gravity>()", delegate(StateMachineInstanceType smi)
			{
				GameObject go = (GameObject)smi.dataTable[data_idx];
				smi.dataTable[data_idx] = null;
				GameComps.Gravities.Remove(go);
			});
			return this;
		}

		public State ToggleGravity(State landed_state)
		{
			TargetParameter state_target = GetStateTarget();
			EventTransition(GameHashes.Landed, landed_state);
			Toggle("GravityComponent", delegate(StateMachineInstanceType smi)
			{
				GameComps.Gravities.Add(state_target.Get(smi), Vector2.zero);
			}, delegate(StateMachineInstanceType smi)
			{
				GameComps.Gravities.Remove(state_target.Get(smi));
			});
			return this;
		}

		public State ToggleThought(Func<StateMachineInstanceType, Thought> chooser_callback)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("EnableThought()", delegate(StateMachineInstanceType smi)
			{
				Thought thought2 = chooser_callback(smi);
				state_target.Get(smi).GetSMI<ThoughtGraph.Instance>().AddThought(thought2);
			});
			Exit("DisableThought()", delegate(StateMachineInstanceType smi)
			{
				Thought thought = chooser_callback(smi);
				state_target.Get(smi).GetSMI<ThoughtGraph.Instance>().RemoveThought(thought);
			});
			return this;
		}

		public State ToggleThought(Thought thought, Func<StateMachineInstanceType, bool> condition_callback = null)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("AddThought(" + thought.Id + ")", delegate(StateMachineInstanceType smi)
			{
				if (condition_callback == null || condition_callback(smi))
				{
					state_target.Get(smi).GetSMI<ThoughtGraph.Instance>().AddThought(thought);
				}
			});
			if (condition_callback != null)
			{
				Update("ValidateThought(" + thought.Id + ")", delegate(StateMachineInstanceType smi, float dt)
				{
					if (condition_callback(smi))
					{
						state_target.Get(smi).GetSMI<ThoughtGraph.Instance>().AddThought(thought);
					}
					else
					{
						state_target.Get(smi).GetSMI<ThoughtGraph.Instance>().RemoveThought(thought);
					}
				});
			}
			Exit("RemoveThought(" + thought.Id + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get(smi).GetSMI<ThoughtGraph.Instance>().RemoveThought(thought);
			});
			return this;
		}

		public State ToggleExpression(Func<StateMachineInstanceType, Expression> chooser_callback)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("AddExpression", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<FaceGraph>(smi).AddExpression(chooser_callback(smi));
			});
			Exit("RemoveExpression", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<FaceGraph>(smi).RemoveExpression(chooser_callback(smi));
			});
			return this;
		}

		public State ToggleExpression(Expression expression, Func<StateMachineInstanceType, bool> condition = null)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("AddExpression(" + expression.Id + ")", delegate(StateMachineInstanceType smi)
			{
				if (condition == null || condition(smi))
				{
					state_target.Get<FaceGraph>(smi).AddExpression(expression);
				}
			});
			if (condition != null)
			{
				Update("ValidateExpression(" + expression.Id + ")", delegate(StateMachineInstanceType smi, float dt)
				{
					if (condition(smi))
					{
						state_target.Get<FaceGraph>(smi).AddExpression(expression);
					}
					else
					{
						state_target.Get<FaceGraph>(smi).RemoveExpression(expression);
					}
				});
			}
			Exit("RemoveExpression(" + expression.Id + ")", delegate(StateMachineInstanceType smi)
			{
				FaceGraph faceGraph = state_target.Get<FaceGraph>(smi);
				if (faceGraph != null)
				{
					faceGraph.RemoveExpression(expression);
				}
			});
			return this;
		}

		public State ToggleMainStatusItem(StatusItem status_item, Func<StateMachineInstanceType, object> callback = null)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("AddMainStatusItem(" + status_item.Id + ")", delegate(StateMachineInstanceType smi)
			{
				object data = ((callback != null) ? callback(smi) : smi);
				state_target.Get<KSelectable>(smi).SetStatusItem(Db.Get().StatusItemCategories.Main, status_item, data);
			});
			Exit("RemoveMainStatusItem(" + status_item.Id + ")", delegate(StateMachineInstanceType smi)
			{
				KSelectable kSelectable = state_target.Get<KSelectable>(smi);
				if (kSelectable != null)
				{
					kSelectable.SetStatusItem(Db.Get().StatusItemCategories.Main, null);
				}
			});
			return this;
		}

		public State ToggleCategoryStatusItem(StatusItemCategory category, StatusItem status_item, object data = null)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("AddCategoryStatusItem(" + category.Id + ", " + status_item.Id + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<KSelectable>(smi).SetStatusItem(category, status_item, (data != null) ? data : smi);
			});
			Exit("RemoveCategoryStatusItem(" + category.Id + ", " + status_item.Id + ")", delegate(StateMachineInstanceType smi)
			{
				KSelectable kSelectable = state_target.Get<KSelectable>(smi);
				if (kSelectable != null)
				{
					kSelectable.SetStatusItem(category, null);
				}
			});
			return this;
		}

		public State ToggleStatusItem(StatusItem status_item, object data = null)
		{
			TargetParameter state_target = GetStateTarget();
			int data_idx = CreateDataTableEntry();
			Enter("AddStatusItem(" + status_item.Id + ")", delegate(StateMachineInstanceType smi)
			{
				object obj = data;
				if (obj == null)
				{
					obj = smi;
				}
				Guid guid2 = state_target.Get<KSelectable>(smi).AddStatusItem(status_item, obj);
				smi.dataTable[data_idx] = guid2;
			});
			Exit("RemoveStatusItem(" + status_item.Id + ")", delegate(StateMachineInstanceType smi)
			{
				KSelectable kSelectable = state_target.Get<KSelectable>(smi);
				if (kSelectable != null && smi.dataTable[data_idx] != null)
				{
					Guid guid = (Guid)smi.dataTable[data_idx];
					kSelectable.RemoveStatusItem(guid);
				}
				smi.dataTable[data_idx] = null;
			});
			return this;
		}

		public State ToggleSnapOn(string snap_on)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("SnapOn(" + snap_on + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<SnapOn>(smi).AttachSnapOnByName(snap_on);
			});
			Exit("SnapOff(" + snap_on + ")", delegate(StateMachineInstanceType smi)
			{
				SnapOn snapOn = state_target.Get<SnapOn>(smi);
				if (snapOn != null)
				{
					snapOn.DetachSnapOnByName(snap_on);
				}
			});
			return this;
		}

		public State ToggleTag(Tag tag)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("AddTag(" + tag.Name + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<KPrefabID>(smi).AddTag(tag);
			});
			Exit("RemoveTag(" + tag.Name + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<KPrefabID>(smi).RemoveTag(tag);
			});
			return this;
		}

		public State ToggleStatusItem(StatusItem status_item, Func<StateMachineInstanceType, object> callback)
		{
			return ToggleStatusItem(status_item, callback, null);
		}

		public State ToggleStatusItem(StatusItem status_item, Func<StateMachineInstanceType, object> callback, StatusItemCategory category)
		{
			TargetParameter state_target = GetStateTarget();
			int data_idx = CreateDataTableEntry();
			Enter("AddStatusItem(" + status_item.Id + ")", delegate(StateMachineInstanceType smi)
			{
				if (category == null)
				{
					object data = ((callback != null) ? callback(smi) : null);
					Guid guid2 = state_target.Get<KSelectable>(smi).AddStatusItem(status_item, data);
					smi.dataTable[data_idx] = guid2;
				}
				else
				{
					object data2 = ((callback != null) ? callback(smi) : null);
					Guid guid3 = state_target.Get<KSelectable>(smi).SetStatusItem(category, status_item, data2);
					smi.dataTable[data_idx] = guid3;
				}
			});
			Exit("RemoveStatusItem(" + status_item.Id + ")", delegate(StateMachineInstanceType smi)
			{
				KSelectable kSelectable = state_target.Get<KSelectable>(smi);
				if (kSelectable != null && smi.dataTable[data_idx] != null)
				{
					if (category == null)
					{
						Guid guid = (Guid)smi.dataTable[data_idx];
						kSelectable.RemoveStatusItem(guid);
					}
					else
					{
						kSelectable.SetStatusItem(category, null);
					}
				}
				smi.dataTable[data_idx] = null;
			});
			return this;
		}

		public State ToggleStatusItem(Func<StateMachineInstanceType, StatusItem> status_item_cb, Func<StateMachineInstanceType, object> data_callback = null)
		{
			TargetParameter state_target = GetStateTarget();
			int data_idx = CreateDataTableEntry();
			Enter("AddStatusItem(DynamicallyConstructed)", delegate(StateMachineInstanceType smi)
			{
				StatusItem statusItem = status_item_cb(smi);
				if (statusItem != null)
				{
					object data = ((data_callback != null) ? data_callback(smi) : null);
					Guid guid2 = state_target.Get<KSelectable>(smi).AddStatusItem(statusItem, data);
					smi.dataTable[data_idx] = guid2;
				}
			});
			Exit("RemoveStatusItem(DynamicallyConstructed)", delegate(StateMachineInstanceType smi)
			{
				KSelectable kSelectable = state_target.Get<KSelectable>(smi);
				if (kSelectable != null && smi.dataTable[data_idx] != null)
				{
					Guid guid = (Guid)smi.dataTable[data_idx];
					kSelectable.RemoveStatusItem(guid);
				}
				smi.dataTable[data_idx] = null;
			});
			return this;
		}

		public State ToggleFX(Func<StateMachineInstanceType, Instance> callback)
		{
			int data_idx = CreateDataTableEntry();
			Enter("EnableFX()", delegate(StateMachineInstanceType smi)
			{
				Instance instance2 = callback(smi);
				if (instance2 != null)
				{
					instance2.StartSM();
					smi.dataTable[data_idx] = instance2;
				}
			});
			Exit("DisableFX()", delegate(StateMachineInstanceType smi)
			{
				Instance instance = (Instance)smi.dataTable[data_idx];
				smi.dataTable[data_idx] = null;
				instance?.StopSM("ToggleFX.Exit");
			});
			return this;
		}

		public State BehaviourComplete(Func<StateMachineInstanceType, Tag> tag_cb, bool on_exit = false)
		{
			if (on_exit)
			{
				Exit("BehaviourComplete()", delegate(StateMachineInstanceType smi)
				{
					smi.Trigger(-739654666, tag_cb(smi));
					smi.GoTo((BaseState)null);
				});
			}
			else
			{
				Enter("BehaviourComplete()", delegate(StateMachineInstanceType smi)
				{
					smi.Trigger(-739654666, tag_cb(smi));
					smi.GoTo((BaseState)null);
				});
			}
			return this;
		}

		public State BehaviourComplete(Tag tag, bool on_exit = false)
		{
			if (on_exit)
			{
				Exit("BehaviourComplete(" + tag.ToString() + ")", delegate(StateMachineInstanceType smi)
				{
					smi.Trigger(-739654666, tag);
					smi.GoTo((BaseState)null);
				});
			}
			else
			{
				Enter("BehaviourComplete(" + tag.ToString() + ")", delegate(StateMachineInstanceType smi)
				{
					smi.Trigger(-739654666, tag);
					smi.GoTo((BaseState)null);
				});
			}
			return this;
		}

		public State ToggleBehaviour(Tag behaviour_tag, Transition.ConditionCallback precondition, Action<StateMachineInstanceType> on_complete = null)
		{
			Func<object, bool> precondition_cb = (object obj) => precondition(obj as StateMachineInstanceType);
			Enter("AddPrecondition", delegate(StateMachineInstanceType smi)
			{
				if (smi.GetComponent<ChoreConsumer>() != null)
				{
					smi.GetComponent<ChoreConsumer>().AddBehaviourPrecondition(behaviour_tag, precondition_cb, smi);
				}
			});
			Exit("RemovePrecondition", delegate(StateMachineInstanceType smi)
			{
				if (smi.GetComponent<ChoreConsumer>() != null)
				{
					smi.GetComponent<ChoreConsumer>().RemoveBehaviourPrecondition(behaviour_tag, precondition_cb, smi);
				}
			});
			ToggleTag(behaviour_tag);
			if (on_complete != null)
			{
				EventHandler(GameHashes.BehaviourTagComplete, delegate(StateMachineInstanceType smi, object data)
				{
					if ((Tag)data == behaviour_tag)
					{
						on_complete(smi);
					}
				});
			}
			return this;
		}

		public void ClearFetch(StateMachineInstanceType smi, int fetch_data_idx, int callback_data_idx)
		{
			FetchList2 fetchList = (FetchList2)smi.dataTable[fetch_data_idx];
			if (fetchList != null)
			{
				smi.dataTable[fetch_data_idx] = null;
				smi.dataTable[callback_data_idx] = null;
				fetchList.Cancel("ClearFetchListFromSM");
			}
		}

		public void SetupFetch(Func<StateMachineInstanceType, FetchList2> create_fetchlist_callback, State target_state, StateMachineInstanceType smi, int fetch_data_idx, int callback_data_idx)
		{
			FetchList2 fetchList = create_fetchlist_callback(smi);
			System.Action action = delegate
			{
				ClearFetch(smi, fetch_data_idx, callback_data_idx);
				smi.GoTo(target_state);
			};
			fetchList.Submit(action, check_storage_contents: true);
			smi.dataTable[fetch_data_idx] = fetchList;
			smi.dataTable[callback_data_idx] = action;
		}

		public State ToggleFetch(Func<StateMachineInstanceType, FetchList2> create_fetchlist_callback, State target_state)
		{
			int data_idx = CreateDataTableEntry();
			int callback_data_idx = CreateDataTableEntry();
			Enter("ToggleFetchEnter()", delegate(StateMachineInstanceType smi)
			{
				SetupFetch(create_fetchlist_callback, target_state, smi, data_idx, callback_data_idx);
			});
			Exit("ToggleFetchExit()", delegate(StateMachineInstanceType smi)
			{
				ClearFetch(smi, data_idx, callback_data_idx);
			});
			return this;
		}

		private void ClearChore(StateMachineInstanceType smi, int chore_data_idx, int callback_data_idx)
		{
			Chore chore = (Chore)smi.dataTable[chore_data_idx];
			if (chore != null)
			{
				Action<Chore> value = (Action<Chore>)smi.dataTable[callback_data_idx];
				smi.dataTable[chore_data_idx] = null;
				smi.dataTable[callback_data_idx] = null;
				chore.onExit = (Action<Chore>)Delegate.Remove(chore.onExit, value);
				chore.Cancel("ClearGlobalChore");
			}
		}

		private Chore SetupChore(Func<StateMachineInstanceType, Chore> create_chore_callback, State success_state, State failure_state, StateMachineInstanceType smi, int chore_data_idx, int callback_data_idx, bool is_success_state_reentrant, bool is_failure_state_reentrant)
		{
			Chore chore = create_chore_callback(smi);
			DebugUtil.DevAssert(!chore.IsPreemptable, "ToggleChore can't be used with preemptable chores! :( (but it should...)");
			chore.runUntilComplete = false;
			Action<Chore> action = delegate
			{
				bool isComplete = chore.isComplete;
				if ((isComplete && is_success_state_reentrant) || (is_failure_state_reentrant && !isComplete))
				{
					SetupChore(create_chore_callback, success_state, failure_state, smi, chore_data_idx, callback_data_idx, is_success_state_reentrant, is_failure_state_reentrant);
				}
				else
				{
					State state = success_state;
					if (!isComplete)
					{
						state = failure_state;
					}
					ClearChore(smi, chore_data_idx, callback_data_idx);
					smi.GoTo(state);
				}
			};
			Chore chore2 = chore;
			chore2.onExit = (Action<Chore>)Delegate.Combine(chore2.onExit, action);
			smi.dataTable[chore_data_idx] = chore;
			smi.dataTable[callback_data_idx] = action;
			return chore;
		}

		public State ToggleRecurringChore(Func<StateMachineInstanceType, Chore> callback, Func<StateMachineInstanceType, bool> condition = null)
		{
			int data_idx = CreateDataTableEntry();
			int callback_data_idx = CreateDataTableEntry();
			Enter("ToggleRecurringChoreEnter()", delegate(StateMachineInstanceType smi)
			{
				if (condition == null || condition(smi))
				{
					SetupChore(callback, this, this, smi, data_idx, callback_data_idx, is_success_state_reentrant: true, is_failure_state_reentrant: true);
				}
			});
			Exit("ToggleRecurringChoreExit()", delegate(StateMachineInstanceType smi)
			{
				ClearChore(smi, data_idx, callback_data_idx);
			});
			return this;
		}

		public State ToggleChore(Func<StateMachineInstanceType, Chore> callback, State target_state)
		{
			int data_idx = CreateDataTableEntry();
			int callback_data_idx = CreateDataTableEntry();
			Enter("ToggleChoreEnter()", delegate(StateMachineInstanceType smi)
			{
				SetupChore(callback, target_state, target_state, smi, data_idx, callback_data_idx, is_success_state_reentrant: false, is_failure_state_reentrant: false);
			});
			Exit("ToggleChoreExit()", delegate(StateMachineInstanceType smi)
			{
				ClearChore(smi, data_idx, callback_data_idx);
			});
			return this;
		}

		public State ToggleChore(Func<StateMachineInstanceType, Chore> callback, State success_state, State failure_state)
		{
			int data_idx = CreateDataTableEntry();
			int callback_data_idx = CreateDataTableEntry();
			bool is_success_state_reentrant = success_state == this;
			bool is_failure_state_reentrant = failure_state == this;
			Enter("ToggleChoreEnter()", delegate(StateMachineInstanceType smi)
			{
				SetupChore(callback, success_state, failure_state, smi, data_idx, callback_data_idx, is_success_state_reentrant, is_failure_state_reentrant);
			});
			Exit("ToggleChoreExit()", delegate(StateMachineInstanceType smi)
			{
				ClearChore(smi, data_idx, callback_data_idx);
			});
			return this;
		}

		public State ToggleReactable(Func<StateMachineInstanceType, Reactable> callback)
		{
			int data_idx = CreateDataTableEntry();
			Enter(delegate(StateMachineInstanceType smi)
			{
				smi.dataTable[data_idx] = callback(smi);
			});
			Exit(delegate(StateMachineInstanceType smi)
			{
				Reactable reactable = (Reactable)smi.dataTable[data_idx];
				smi.dataTable[data_idx] = null;
				reactable?.Cleanup();
			});
			return this;
		}

		public State RemoveEffect(string effect_name)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("RemoveEffect(" + effect_name + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Effects>(smi).Remove(effect_name);
			});
			return this;
		}

		public State ToggleEffect(string effect_name)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("AddEffect(" + effect_name + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Effects>(smi).Add(effect_name, should_save: false);
			});
			Exit("RemoveEffect(" + effect_name + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Effects>(smi).Remove(effect_name);
			});
			return this;
		}

		public State ToggleEffect(Func<StateMachineInstanceType, Effect> callback)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("AddEffect()", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Effects>(smi).Add(callback(smi), should_save: false);
			});
			Exit("RemoveEffect()", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Effects>(smi).Remove(callback(smi));
			});
			return this;
		}

		public State ToggleEffect(Func<StateMachineInstanceType, string> callback)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("AddEffect()", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Effects>(smi).Add(callback(smi), should_save: false);
			});
			Exit("RemoveEffect()", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Effects>(smi).Remove(callback(smi));
			});
			return this;
		}

		public State LogOnExit(Func<StateMachineInstanceType, string> callback)
		{
			Enter("Log()", delegate
			{
			});
			return this;
		}

		public State LogOnEnter(Func<StateMachineInstanceType, string> callback)
		{
			Exit("Log()", delegate
			{
			});
			return this;
		}

		public State ToggleUrge(Urge urge)
		{
			return ToggleUrge((StateMachineInstanceType smi) => urge);
		}

		public State ToggleUrge(Func<StateMachineInstanceType, Urge> urge_callback)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("AddUrge()", delegate(StateMachineInstanceType smi)
			{
				Urge urge2 = urge_callback(smi);
				state_target.Get<ChoreConsumer>(smi).AddUrge(urge2);
			});
			Exit("RemoveUrge()", delegate(StateMachineInstanceType smi)
			{
				Urge urge = urge_callback(smi);
				ChoreConsumer choreConsumer = state_target.Get<ChoreConsumer>(smi);
				if (choreConsumer != null)
				{
					choreConsumer.RemoveUrge(urge);
				}
			});
			return this;
		}

		public State OnTargetLost(TargetParameter parameter, State target_state)
		{
			ParamTransition(parameter, target_state, (StateMachineInstanceType smi, GameObject p) => p == null);
			return this;
		}

		public State ToggleBrain(string reason)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("StopBrain(" + reason + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Brain>(smi).Stop(reason);
			});
			Exit("ResetBrain(" + reason + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Brain>(smi).Reset(reason);
			});
			return this;
		}

		public State TriggerOnEnter(GameHashes evt, Func<StateMachineInstanceType, object> callback = null)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("Trigger(" + evt.ToString() + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get(smi).Trigger(data: (callback != null) ? callback(smi) : null, hash: (int)evt);
			});
			return this;
		}

		public State TriggerOnExit(GameHashes evt, Func<StateMachineInstanceType, object> callback = null)
		{
			TargetParameter state_target = GetStateTarget();
			Exit("Trigger(" + evt.ToString() + ")", delegate(StateMachineInstanceType smi)
			{
				GameObject gameObject = state_target.Get(smi);
				if (gameObject != null)
				{
					object data = ((callback != null) ? callback(smi) : null);
					gameObject.Trigger((int)evt, data);
				}
			});
			return this;
		}

		public State ToggleStateMachine(Func<StateMachineInstanceType, Instance> callback)
		{
			int data_idx = CreateDataTableEntry();
			Enter("EnableStateMachine()", delegate(StateMachineInstanceType smi)
			{
				Instance instance2 = callback(smi);
				smi.dataTable[data_idx] = instance2;
				instance2.StartSM();
			});
			Exit("DisableStateMachine()", delegate(StateMachineInstanceType smi)
			{
				Instance instance = (Instance)smi.dataTable[data_idx];
				smi.dataTable[data_idx] = null;
				instance?.StopSM("ToggleStateMachine.Exit");
			});
			return this;
		}

		public State ToggleComponent<ComponentType>(bool disable = false) where ComponentType : MonoBehaviour
		{
			TargetParameter state_target = GetStateTarget();
			Enter("EnableComponent(" + typeof(ComponentType).Name + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<ComponentType>(smi).enabled = !disable;
			});
			Exit("DisableComponent(" + typeof(ComponentType).Name + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<ComponentType>(smi).enabled = disable;
			});
			return this;
		}

		public State InitializeOperationalFlag(Operational.Flag flag, bool init_val = false)
		{
			Enter("InitOperationalFlag (" + flag.Name + ", " + init_val + ")", delegate(StateMachineInstanceType smi)
			{
				smi.GetComponent<Operational>().SetFlag(flag, init_val);
			});
			return this;
		}

		public State ToggleOperationalFlag(Operational.Flag flag)
		{
			Enter("ToggleOperationalFlag True (" + flag.Name + ")", delegate(StateMachineInstanceType smi)
			{
				smi.GetComponent<Operational>().SetFlag(flag, value: true);
			});
			Exit("ToggleOperationalFlag False (" + flag.Name + ")", delegate(StateMachineInstanceType smi)
			{
				smi.GetComponent<Operational>().SetFlag(flag, value: false);
			});
			return this;
		}

		public State ToggleReserve(TargetParameter reserver, TargetParameter pickup_target, FloatParameter requested_amount, FloatParameter actual_amount)
		{
			int data_idx = CreateDataTableEntry();
			Enter("Reserve(" + pickup_target.name + ", " + requested_amount.name + ")", delegate(StateMachineInstanceType smi)
			{
				Pickupable pickupable2 = pickup_target.Get<Pickupable>(smi);
				GameObject gameObject = reserver.Get(smi);
				float val = requested_amount.Get(smi);
				float val2 = Mathf.Max(1f, Db.Get().Attributes.CarryAmount.Lookup(gameObject).GetTotalValue());
				float val3 = Math.Min(val, val2);
				val3 = Math.Min(val3, pickupable2.UnreservedAmount);
				if (val3 <= 0f)
				{
					pickupable2.PrintReservations();
					Debug.LogError(val2 + ", " + val + ", " + pickupable2.UnreservedAmount + ", " + val3);
				}
				actual_amount.Set(val3, smi);
				int num = pickupable2.Reserve("ToggleReserve", gameObject, val3);
				smi.dataTable[data_idx] = num;
			});
			Exit("Unreserve(" + pickup_target.name + ", " + requested_amount.name + ")", delegate(StateMachineInstanceType smi)
			{
				int ticket = (int)smi.dataTable[data_idx];
				smi.dataTable[data_idx] = null;
				Pickupable pickupable = pickup_target.Get<Pickupable>(smi);
				if (pickupable != null)
				{
					pickupable.Unreserve("ToggleReserve", ticket);
				}
			});
			return this;
		}

		public State ToggleWork(string work_type, Action<StateMachineInstanceType> callback, Func<StateMachineInstanceType, bool> validate_callback, State success_state, State failure_state)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("StartWork(" + work_type + ")", delegate(StateMachineInstanceType smi)
			{
				if (validate_callback(smi))
				{
					callback(smi);
				}
				else
				{
					smi.GoTo(failure_state);
				}
			});
			Update("Work(" + work_type + ")", delegate(StateMachineInstanceType smi, float dt)
			{
				if (validate_callback(smi))
				{
					switch (state_target.Get<Worker>(smi).Work(dt))
					{
					case Worker.WorkResult.Success:
						smi.GoTo(success_state);
						break;
					case Worker.WorkResult.Failed:
						smi.GoTo(failure_state);
						break;
					}
				}
				else
				{
					smi.GoTo(failure_state);
				}
			}, UpdateRate.SIM_33ms);
			Exit("StopWork()", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Worker>(smi).StopWork();
			});
			return this;
		}

		public State ToggleWork<WorkableType>(TargetParameter source_target, State success_state, State failure_state, Func<StateMachineInstanceType, bool> is_valid_cb) where WorkableType : Workable
		{
			TargetParameter state_target = GetStateTarget();
			ToggleWork(typeof(WorkableType).Name, delegate(StateMachineInstanceType smi)
			{
				Workable workable = source_target.Get<WorkableType>(smi);
				state_target.Get<Worker>(smi).StartWork(new Worker.StartWorkInfo(workable));
			}, (StateMachineInstanceType smi) => (UnityEngine.Object)source_target.Get<WorkableType>(smi) != (UnityEngine.Object)null && (is_valid_cb == null || is_valid_cb(smi)), success_state, failure_state);
			return this;
		}

		public State DoEat(TargetParameter source_target, FloatParameter amount, State success_state, State failure_state)
		{
			TargetParameter state_target = GetStateTarget();
			ToggleWork("Eat", delegate(StateMachineInstanceType smi)
			{
				Edible workable = source_target.Get<Edible>(smi);
				Worker worker = state_target.Get<Worker>(smi);
				float amount2 = amount.Get(smi);
				worker.StartWork(new Edible.EdibleStartWorkInfo(workable, amount2));
			}, (StateMachineInstanceType smi) => source_target.Get<Edible>(smi) != null, success_state, failure_state);
			return this;
		}

		public State DoSleep(TargetParameter sleeper, TargetParameter bed, State success_state, State failure_state)
		{
			TargetParameter state_target = GetStateTarget();
			ToggleWork("Sleep", delegate(StateMachineInstanceType smi)
			{
				Worker worker = state_target.Get<Worker>(smi);
				Sleepable workable = bed.Get<Sleepable>(smi);
				worker.StartWork(new Worker.StartWorkInfo(workable));
			}, (StateMachineInstanceType smi) => bed.Get<Sleepable>(smi) != null, success_state, failure_state);
			return this;
		}

		public State DoDelivery(TargetParameter worker_param, TargetParameter storage_param, State success_state, State failure_state)
		{
			ToggleWork("Pickup", delegate(StateMachineInstanceType smi)
			{
				Worker worker = worker_param.Get<Worker>(smi);
				Storage workable = storage_param.Get<Storage>(smi);
				worker.StartWork(new Worker.StartWorkInfo(workable));
			}, (StateMachineInstanceType smi) => storage_param.Get<Storage>(smi) != null, success_state, failure_state);
			return this;
		}

		public State DoPickup(TargetParameter source_target, TargetParameter result_target, FloatParameter amount, State success_state, State failure_state)
		{
			TargetParameter state_target = GetStateTarget();
			ToggleWork("Pickup", delegate(StateMachineInstanceType smi)
			{
				Pickupable pickupable = source_target.Get<Pickupable>(smi);
				Worker worker = state_target.Get<Worker>(smi);
				float amount2 = amount.Get(smi);
				worker.StartWork(new Pickupable.PickupableStartWorkInfo(pickupable, amount2, delegate(GameObject result)
				{
					result_target.Set(result, smi);
				}));
			}, (StateMachineInstanceType smi) => source_target.Get<Pickupable>(smi) != null || result_target.Get<Pickupable>(smi) != null, success_state, failure_state);
			return this;
		}

		public State ToggleNotification(Func<StateMachineInstanceType, Notification> callback)
		{
			int data_idx = CreateDataTableEntry();
			TargetParameter state_target = GetStateTarget();
			Enter("EnableNotification()", delegate(StateMachineInstanceType smi)
			{
				Notification notification2 = callback(smi);
				smi.dataTable[data_idx] = notification2;
				state_target.AddOrGet<Notifier>(smi).Add(notification2);
			});
			Exit("DisableNotification()", delegate(StateMachineInstanceType smi)
			{
				Notification notification = (Notification)smi.dataTable[data_idx];
				if (notification != null)
				{
					if (state_target != null)
					{
						Notifier notifier = state_target.Get<Notifier>(smi);
						if (notifier != null)
						{
							notifier.Remove(notification);
						}
					}
					smi.dataTable[data_idx] = null;
				}
			});
			return this;
		}

		public State DoReport(ReportManager.ReportType reportType, Func<StateMachineInstanceType, float> callback, Func<StateMachineInstanceType, string> context_callback = null)
		{
			Enter("DoReport()", delegate(StateMachineInstanceType smi)
			{
				float value = callback(smi);
				string note = ((context_callback != null) ? context_callback(smi) : null);
				ReportManager.Instance.ReportValue(reportType, value, note);
			});
			return this;
		}

		public State DoNotification(Func<StateMachineInstanceType, Notification> callback)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("DoNotification()", delegate(StateMachineInstanceType smi)
			{
				Notification notification = callback(smi);
				state_target.AddOrGet<Notifier>(smi).Add(notification);
			});
			return this;
		}

		public State DoTutorial(Tutorial.TutorialMessages msg)
		{
			Enter("DoTutorial()", delegate
			{
				Tutorial.Instance.TutorialMessage(msg);
			});
			return this;
		}

		public State ToggleScheduleCallback(string name, Func<StateMachineInstanceType, float> time_cb, Action<StateMachineInstanceType> callback)
		{
			int data_idx = CreateDataTableEntry();
			Enter("AddScheduledCallback(" + name + ")", delegate(StateMachineInstanceType smi)
			{
				SchedulerHandle schedulerHandle2 = GameScheduler.Instance.Schedule(name, time_cb(smi), delegate(object smi_data)
				{
					callback((StateMachineInstanceType)smi_data);
				}, smi);
				DebugUtil.Assert(smi.dataTable[data_idx] == null);
				smi.dataTable[data_idx] = schedulerHandle2;
			});
			Exit("RemoveScheduledCallback(" + name + ")", delegate(StateMachineInstanceType smi)
			{
				if (smi.dataTable[data_idx] != null)
				{
					SchedulerHandle schedulerHandle = (SchedulerHandle)smi.dataTable[data_idx];
					smi.dataTable[data_idx] = null;
					schedulerHandle.ClearScheduler();
				}
			});
			return this;
		}

		public State ScheduleGoTo(Func<StateMachineInstanceType, float> time_cb, BaseState state)
		{
			Enter("ScheduleGoTo(" + state.name + ")", delegate(StateMachineInstanceType smi)
			{
				smi.ScheduleGoTo(time_cb(smi), state);
			});
			return this;
		}

		public State ScheduleGoTo(float time, BaseState state)
		{
			Enter("ScheduleGoTo(" + time + ", " + state.name + ")", delegate(StateMachineInstanceType smi)
			{
				smi.ScheduleGoTo(time, state);
			});
			return this;
		}

		public State EventHandler(GameHashes evt, Func<StateMachineInstanceType, KMonoBehaviour> global_event_system_callback, Callback callback)
		{
			return EventHandler(evt, global_event_system_callback, delegate(StateMachineInstanceType smi, object d)
			{
				callback(smi);
			});
		}

		public State EventHandler(GameHashes evt, Func<StateMachineInstanceType, KMonoBehaviour> global_event_system_callback, GameEvent.Callback callback)
		{
			if (events == null)
			{
				events = new List<StateEvent>();
			}
			TargetParameter target = GetStateTarget();
			GameEvent item = new GameEvent(evt, callback, target, global_event_system_callback);
			events.Add(item);
			return this;
		}

		public State EventHandler(GameHashes evt, Callback callback)
		{
			return EventHandler(evt, delegate(StateMachineInstanceType smi, object d)
			{
				callback(smi);
			});
		}

		public State EventHandler(GameHashes evt, GameEvent.Callback callback)
		{
			EventHandler(evt, null, callback);
			return this;
		}

		public State EventHandlerTransition(GameHashes evt, State state, Func<StateMachineInstanceType, object, bool> callback)
		{
			return EventHandler(evt, delegate(StateMachineInstanceType smi, object d)
			{
				if (callback(smi, d))
				{
					smi.GoTo(state);
				}
			});
		}

		public State EventHandlerTransition(GameHashes evt, Func<StateMachineInstanceType, KMonoBehaviour> global_event_system_callback, State state, Func<StateMachineInstanceType, object, bool> callback)
		{
			return EventHandler(evt, global_event_system_callback, delegate(StateMachineInstanceType smi, object d)
			{
				if (callback(smi, d))
				{
					smi.GoTo(state);
				}
			});
		}

		public State ParamTransition<ParameterType>(Parameter<ParameterType> parameter, State state, Parameter<ParameterType>.Callback callback)
		{
			if (parameterTransitions == null)
			{
				parameterTransitions = new List<StateMachine.ParameterTransition>();
			}
			parameterTransitions.Add(new Parameter<ParameterType>.Transition(parameter, state, callback));
			return this;
		}

		public State OnSignal(Signal signal, State state, Func<StateMachineInstanceType, bool> callback)
		{
			ParamTransition(signal, state, (StateMachineInstanceType smi, SignalParameter p) => callback(smi));
			return this;
		}

		public State OnSignal(Signal signal, State state)
		{
			ParamTransition(signal, state, null);
			return this;
		}

		public State EnterTransition(State state, Transition.ConditionCallback condition)
		{
			string text = "(Stop)";
			if (state != null)
			{
				text = state.name;
			}
			Enter("Transition(" + text + ")", delegate(StateMachineInstanceType smi)
			{
				if (condition(smi))
				{
					smi.GoTo(state);
				}
			});
			return this;
		}

		public State Transition(State state, Transition.ConditionCallback condition, UpdateRate update_rate = UpdateRate.SIM_200ms)
		{
			string text = "(Stop)";
			if (state != null)
			{
				text = state.name;
			}
			Enter("Transition(" + text + ")", delegate(StateMachineInstanceType smi)
			{
				if (condition(smi))
				{
					smi.GoTo(state);
				}
			});
			FastUpdate("Transition(" + text + ")", new TransitionUpdater(condition, state), update_rate);
			return this;
		}

		public State DefaultState(State default_state)
		{
			defaultState = default_state;
			return this;
		}

		public State GoTo(State state)
		{
			string text = "(null)";
			if (state != null)
			{
				text = state.name;
			}
			Update("GoTo(" + text + ")", delegate(StateMachineInstanceType smi, float dt)
			{
				smi.GoTo(state);
			});
			return this;
		}

		public State StopMoving()
		{
			TargetParameter target = GetStateTarget();
			Exit("StopMoving()", delegate(StateMachineInstanceType smi)
			{
				target.Get<Navigator>(smi).Stop();
			});
			return this;
		}

		public State OnBehaviourComplete(Tag behaviour, Action<StateMachineInstanceType> cb)
		{
			EventHandler(GameHashes.BehaviourTagComplete, delegate(StateMachineInstanceType smi, object d)
			{
				if ((Tag)d == behaviour)
				{
					cb(smi);
				}
			});
			return this;
		}

		public State MoveTo(Func<StateMachineInstanceType, int> cell_callback, State success_state = null, State fail_state = null, bool update_cell = false)
		{
			return MoveTo(cell_callback, null, success_state, fail_state, update_cell);
		}

		public State MoveTo(Func<StateMachineInstanceType, int> cell_callback, Func<StateMachineInstanceType, CellOffset[]> cell_offsets_callback, State success_state = null, State fail_state = null, bool update_cell = false)
		{
			EventTransition(GameHashes.DestinationReached, success_state);
			EventTransition(GameHashes.NavigationFailed, fail_state);
			CellOffset[] default_offset = new CellOffset[1];
			TargetParameter state_target = GetStateTarget();
			Enter("MoveTo()", delegate(StateMachineInstanceType smi)
			{
				int cell2 = cell_callback(smi);
				Navigator navigator = state_target.Get<Navigator>(smi);
				CellOffset[] offsets = default_offset;
				if (cell_offsets_callback != null)
				{
					offsets = cell_offsets_callback(smi);
				}
				navigator.GoTo(cell2, offsets);
			});
			if (update_cell)
			{
				Update("MoveTo()", delegate(StateMachineInstanceType smi, float dt)
				{
					int cell = cell_callback(smi);
					state_target.Get<Navigator>(smi).UpdateTarget(cell);
				});
			}
			Exit("StopMoving()", delegate(StateMachineInstanceType smi)
			{
				state_target.Get(smi).GetComponent<Navigator>().Stop();
			});
			return this;
		}

		public State MoveTo<ApproachableType>(TargetParameter move_parameter, State success_state, State fail_state = null, CellOffset[] override_offsets = null, NavTactic tactic = null) where ApproachableType : IApproachable
		{
			EventTransition(GameHashes.DestinationReached, success_state);
			EventTransition(GameHashes.NavigationFailed, fail_state);
			TargetParameter state_target = GetStateTarget();
			CellOffset[] offsets;
			Enter("MoveTo(" + move_parameter.name + ")", delegate(StateMachineInstanceType smi)
			{
				offsets = override_offsets;
				IApproachable approachable = move_parameter.Get<ApproachableType>(smi);
				KMonoBehaviour kMonoBehaviour = move_parameter.Get<KMonoBehaviour>(smi);
				if (kMonoBehaviour == null)
				{
					smi.GoTo(fail_state);
				}
				else
				{
					Navigator component = state_target.Get(smi).GetComponent<Navigator>();
					if (offsets == null)
					{
						offsets = approachable.GetOffsets();
					}
					component.GoTo(kMonoBehaviour, offsets, tactic);
				}
			});
			Exit("StopMoving()", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Navigator>(smi).Stop();
			});
			return this;
		}

		public State Face(TargetParameter face_target, float x_offset = 0f)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("Face", delegate(StateMachineInstanceType smi)
			{
				if (face_target != null)
				{
					IApproachable approachable = face_target.Get<IApproachable>(smi);
					if (approachable != null)
					{
						float target_x = approachable.transform.GetPosition().x + x_offset;
						state_target.Get<Facing>(smi).Face(target_x);
					}
				}
			});
			return this;
		}

		public State TagTransition(Tag[] tags, State state, bool on_remove = false)
		{
			if (transitions == null)
			{
				transitions = new List<BaseTransition>();
			}
			TagTransitionData item = new TagTransitionData(this, state, transitions.Count, tags, on_remove, GetStateTarget());
			transitions.Add(item);
			return this;
		}

		public State TagTransition(Tag tag, State state, bool on_remove = false)
		{
			return TagTransition(new Tag[1] { tag }, state, on_remove);
		}

		public State EventTransition(GameHashes evt, Func<StateMachineInstanceType, KMonoBehaviour> global_event_system_callback, State state, Transition.ConditionCallback condition = null)
		{
			if (transitions == null)
			{
				transitions = new List<BaseTransition>();
			}
			TargetParameter target = GetStateTarget();
			EventTransitionData item = new EventTransitionData(this, state, transitions.Count, evt, global_event_system_callback, condition, target);
			transitions.Add(item);
			return this;
		}

		public State EventTransition(GameHashes evt, State state, Transition.ConditionCallback condition = null)
		{
			return EventTransition(evt, null, state, condition);
		}

		public State ReturnSuccess()
		{
			Enter("ReturnSuccess()", delegate(StateMachineInstanceType smi)
			{
				smi.SetStatus(Status.Success);
				smi.StopSM("GameStateMachine.ReturnSuccess()");
			});
			return this;
		}

		public State ReturnFailure()
		{
			Enter("ReturnFailure()", delegate(StateMachineInstanceType smi)
			{
				smi.SetStatus(Status.Failed);
				smi.StopSM("GameStateMachine.ReturnFailure()");
			});
			return this;
		}

		public State ToggleStatusItem(string name, string tooltip, string icon = "", StatusItem.IconType icon_type = StatusItem.IconType.Info, NotificationType notification_type = NotificationType.Neutral, bool allow_multiples = false, HashedString render_overlay = default(HashedString), int status_overlays = 129022, Func<string, StateMachineInstanceType, string> resolve_string_callback = null, Func<string, StateMachineInstanceType, string> resolve_tooltip_callback = null, StatusItemCategory category = null)
		{
			StatusItem statusItem = new StatusItem(longName, name, tooltip, icon, icon_type, notification_type, allow_multiples, render_overlay, status_overlays);
			if (resolve_string_callback != null)
			{
				statusItem.resolveStringCallback = (string str, object obj) => resolve_string_callback(str, (StateMachineInstanceType)obj);
			}
			if (resolve_tooltip_callback != null)
			{
				statusItem.resolveTooltipCallback = (string str, object obj) => resolve_tooltip_callback(str, (StateMachineInstanceType)obj);
			}
			ToggleStatusItem(statusItem, (StateMachineInstanceType smi) => smi, category);
			return this;
		}

		public State PlayAnim(string anim)
		{
			TargetParameter state_target = GetStateTarget();
			KAnim.PlayMode mode = KAnim.PlayMode.Once;
			Enter("PlayAnim(" + anim + ", " + mode.ToString() + ")", delegate(StateMachineInstanceType smi)
			{
				KAnimControllerBase kAnimControllerBase = state_target.Get<KAnimControllerBase>(smi);
				if (kAnimControllerBase != null)
				{
					kAnimControllerBase.Play(anim, mode);
				}
			});
			return this;
		}

		public State PlayAnim(Func<StateMachineInstanceType, string> anim_cb, KAnim.PlayMode mode = KAnim.PlayMode.Once)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("PlayAnim(" + mode.ToString() + ")", delegate(StateMachineInstanceType smi)
			{
				KAnimControllerBase kAnimControllerBase = state_target.Get<KAnimControllerBase>(smi);
				if (kAnimControllerBase != null)
				{
					kAnimControllerBase.Play(anim_cb(smi), mode);
				}
			});
			return this;
		}

		public State PlayAnim(string anim, KAnim.PlayMode mode)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("PlayAnim(" + anim + ", " + mode.ToString() + ")", delegate(StateMachineInstanceType smi)
			{
				KAnimControllerBase kAnimControllerBase = state_target.Get<KAnimControllerBase>(smi);
				if (kAnimControllerBase != null)
				{
					kAnimControllerBase.Play(anim, mode);
				}
			});
			return this;
		}

		public State PlayAnim(string anim, KAnim.PlayMode mode, Func<StateMachineInstanceType, string> suffix_callback)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("PlayAnim(" + anim + ", " + mode.ToString() + ")", delegate(StateMachineInstanceType smi)
			{
				string text = "";
				if (suffix_callback != null)
				{
					text = suffix_callback(smi);
				}
				KAnimControllerBase kAnimControllerBase = state_target.Get<KAnimControllerBase>(smi);
				if (kAnimControllerBase != null)
				{
					kAnimControllerBase.Play(anim + text, mode);
				}
			});
			return this;
		}

		public State QueueAnim(Func<StateMachineInstanceType, string> anim_cb, bool loop = false, Func<StateMachineInstanceType, string> suffix_callback = null)
		{
			TargetParameter state_target = GetStateTarget();
			KAnim.PlayMode mode = KAnim.PlayMode.Once;
			if (loop)
			{
				mode = KAnim.PlayMode.Loop;
			}
			Enter("QueueAnim(" + mode.ToString() + ")", delegate(StateMachineInstanceType smi)
			{
				string text = "";
				if (suffix_callback != null)
				{
					text = suffix_callback(smi);
				}
				KAnimControllerBase kAnimControllerBase = state_target.Get<KAnimControllerBase>(smi);
				if (kAnimControllerBase != null)
				{
					kAnimControllerBase.Queue(anim_cb(smi) + text, mode);
				}
			});
			return this;
		}

		public State QueueAnim(string anim, bool loop = false, Func<StateMachineInstanceType, string> suffix_callback = null)
		{
			TargetParameter state_target = GetStateTarget();
			KAnim.PlayMode mode = KAnim.PlayMode.Once;
			if (loop)
			{
				mode = KAnim.PlayMode.Loop;
			}
			Enter("QueueAnim(" + anim + ", " + mode.ToString() + ")", delegate(StateMachineInstanceType smi)
			{
				string text = "";
				if (suffix_callback != null)
				{
					text = suffix_callback(smi);
				}
				KAnimControllerBase kAnimControllerBase = state_target.Get<KAnimControllerBase>(smi);
				if (kAnimControllerBase != null)
				{
					kAnimControllerBase.Queue(anim + text, mode);
				}
			});
			return this;
		}

		public State PlayAnims(Func<StateMachineInstanceType, HashedString[]> anims_callback, KAnim.PlayMode mode = KAnim.PlayMode.Once)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("PlayAnims", delegate(StateMachineInstanceType smi)
			{
				KAnimControllerBase kAnimControllerBase = state_target.Get<KAnimControllerBase>(smi);
				if (kAnimControllerBase != null)
				{
					HashedString[] anim_names = anims_callback(smi);
					kAnimControllerBase.Play(anim_names, mode);
				}
			});
			return this;
		}

		public State PlayAnims(Func<StateMachineInstanceType, HashedString[]> anims_callback, Func<StateMachineInstanceType, KAnim.PlayMode> mode_cb)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("PlayAnims", delegate(StateMachineInstanceType smi)
			{
				KAnimControllerBase kAnimControllerBase = state_target.Get<KAnimControllerBase>(smi);
				if (kAnimControllerBase != null)
				{
					HashedString[] anim_names = anims_callback(smi);
					KAnim.PlayMode mode = mode_cb(smi);
					kAnimControllerBase.Play(anim_names, mode);
				}
			});
			return this;
		}

		public State OnAnimQueueComplete(State state)
		{
			TargetParameter state_target = GetStateTarget();
			Enter("CheckIfAnimQueueIsEmpty", delegate(StateMachineInstanceType smi)
			{
				if (state_target.Get<KBatchedAnimController>(smi).IsStopped())
				{
					smi.GoTo(state);
				}
			});
			return EventTransition(GameHashes.AnimQueueComplete, state);
		}
	}

	public class GameEvent : StateEvent
	{
		public delegate void Callback(StateMachineInstanceType smi, object callback_data);

		private GameHashes id;

		private TargetParameter target;

		private Callback callback;

		private Func<StateMachineInstanceType, KMonoBehaviour> globalEventSystemCallback;

		public GameEvent(GameHashes id, Callback callback, TargetParameter target, Func<StateMachineInstanceType, KMonoBehaviour> global_event_system_callback)
			: base(id.ToString())
		{
			this.id = id;
			this.target = target;
			this.callback = callback;
			globalEventSystemCallback = global_event_system_callback;
		}

		public override Context Subscribe(Instance smi)
		{
			Context result = base.Subscribe(smi);
			StateMachineInstanceType cast_smi = (StateMachineInstanceType)smi;
			Action<object> handler = delegate(object d)
			{
				if (!Instance.error)
				{
					callback(cast_smi, d);
				}
			};
			if (globalEventSystemCallback != null)
			{
				KMonoBehaviour kMonoBehaviour = globalEventSystemCallback(cast_smi);
				result.data = kMonoBehaviour.Subscribe((int)id, handler);
			}
			else
			{
				result.data = target.Get(cast_smi).Subscribe((int)id, handler);
			}
			return result;
		}

		public override void Unsubscribe(Instance smi, Context context)
		{
			StateMachineInstanceType val = (StateMachineInstanceType)smi;
			if (globalEventSystemCallback != null)
			{
				KMonoBehaviour kMonoBehaviour = globalEventSystemCallback(val);
				if (kMonoBehaviour != null)
				{
					kMonoBehaviour.Unsubscribe(context.data);
				}
			}
			else
			{
				GameObject gameObject = target.Get(val);
				if (gameObject != null)
				{
					gameObject.Unsubscribe(context.data);
				}
			}
		}
	}

	public class ApproachSubState<ApproachableType> : State where ApproachableType : IApproachable
	{
		public State InitializeStates(TargetParameter mover, TargetParameter move_target, State success_state, State failure_state = null, CellOffset[] override_offsets = null, NavTactic tactic = null)
		{
			base.root.Target(mover).OnTargetLost(move_target, failure_state).MoveTo<ApproachableType>(move_target, success_state, failure_state, override_offsets, (tactic == null) ? NavigationTactics.ReduceTravelDistance : tactic);
			return this;
		}
	}

	public class DebugGoToSubState : State
	{
		public State InitializeStates(State exit_state)
		{
			base.root.Enter("GoToCursor", delegate(StateMachineInstanceType smi)
			{
				GoToCursor(smi);
			}).EventHandler(GameHashes.DebugGoTo, (StateMachineInstanceType smi) => Game.Instance, delegate(StateMachineInstanceType smi)
			{
				GoToCursor(smi);
			}).EventTransition(GameHashes.DestinationReached, exit_state)
				.EventTransition(GameHashes.NavigationFailed, exit_state);
			return this;
		}

		public void GoToCursor(StateMachineInstanceType smi)
		{
			smi.GetComponent<Navigator>().GoTo(Grid.PosToCell(DebugHandler.GetMousePos()), new CellOffset[1]);
		}
	}

	public class DropSubState : State
	{
		public State InitializeStates(TargetParameter carrier, TargetParameter item, TargetParameter drop_target, State success_state, State failure_state = null)
		{
			base.root.Target(carrier).Enter("Drop", delegate(StateMachineInstanceType smi)
			{
				Storage storage = carrier.Get<Storage>(smi);
				GameObject gameObject = item.Get(smi);
				storage.Drop(gameObject);
				int cell = Grid.CellAbove(Grid.PosToCell(drop_target.Get<Transform>(smi).GetPosition()));
				gameObject.transform.SetPosition(Grid.CellToPosCCC(cell, Grid.SceneLayer.Move));
				smi.GoTo(success_state);
			});
			return this;
		}
	}

	public class FetchSubState : State
	{
		public ApproachSubState<Pickupable> approach;

		public State pickup;

		public State success;

		public State InitializeStates(TargetParameter fetcher, TargetParameter pickup_source, TargetParameter pickup_chunk, FloatParameter requested_amount, FloatParameter actual_amount, State success_state, State failure_state = null)
		{
			Target(fetcher);
			base.root.DefaultState(approach).ToggleReserve(fetcher, pickup_source, requested_amount, actual_amount);
			approach.InitializeStates(fetcher, pickup_source, pickup, null, null, NavigationTactics.ReduceTravelDistance).OnTargetLost(pickup_source, failure_state);
			pickup.DoPickup(pickup_source, pickup_chunk, actual_amount, success_state, failure_state);
			return this;
		}
	}

	public class HungrySubState : State
	{
		public State satisfied;

		public State hungry;

		public State InitializeStates(TargetParameter target, StatusItem status_item)
		{
			Target(target);
			base.root.DefaultState(satisfied);
			satisfied.EventTransition(GameHashes.AddUrge, hungry, (StateMachineInstanceType smi) => IsHungry(smi));
			hungry.EventTransition(GameHashes.RemoveUrge, satisfied, (StateMachineInstanceType smi) => !IsHungry(smi)).ToggleStatusItem(status_item);
			return this;
		}

		private static bool IsHungry(StateMachineInstanceType smi)
		{
			return smi.GetComponent<ChoreConsumer>().HasUrge(Db.Get().Urges.Eat);
		}
	}

	public class PlantAliveSubState : State
	{
		public State InitializeStates(TargetParameter plant, State death_state = null)
		{
			base.root.Target(plant).TagTransition(GameTags.Uprooted, death_state).EventTransition(GameHashes.TooColdFatal, death_state, (StateMachineInstanceType smi) => isLethalTemperature(plant.Get(smi)))
				.EventTransition(GameHashes.TooHotFatal, death_state, (StateMachineInstanceType smi) => isLethalTemperature(plant.Get(smi)))
				.EventTransition(GameHashes.Drowned, death_state);
			return this;
		}

		public bool ForceUpdateStatus(GameObject plant)
		{
			TemperatureVulnerable component = plant.GetComponent<TemperatureVulnerable>();
			EntombVulnerable component2 = plant.GetComponent<EntombVulnerable>();
			PressureVulnerable component3 = plant.GetComponent<PressureVulnerable>();
			if ((component == null || !component.IsLethal) && (component2 == null || !component2.GetEntombed))
			{
				if (!(component3 == null))
				{
					return !component3.IsLethal;
				}
				return true;
			}
			return false;
		}

		private static bool isLethalTemperature(GameObject plant)
		{
			TemperatureVulnerable component = plant.GetComponent<TemperatureVulnerable>();
			if (component == null)
			{
				return false;
			}
			if (component.GetInternalTemperatureState == TemperatureVulnerable.TemperatureState.LethalCold)
			{
				return true;
			}
			if (component.GetInternalTemperatureState == TemperatureVulnerable.TemperatureState.LethalHot)
			{
				return true;
			}
			return false;
		}
	}

	public State root = new State();

	protected static Parameter<bool>.Callback IsFalse = (StateMachineInstanceType smi, bool p) => !p;

	protected static Parameter<bool>.Callback IsTrue = (StateMachineInstanceType smi, bool p) => p;

	protected static Parameter<float>.Callback IsZero = (StateMachineInstanceType smi, float p) => p == 0f;

	protected static Parameter<float>.Callback IsLTZero = (StateMachineInstanceType smi, float p) => p < 0f;

	protected static Parameter<float>.Callback IsLTEZero = (StateMachineInstanceType smi, float p) => p <= 0f;

	protected static Parameter<float>.Callback IsGTZero = (StateMachineInstanceType smi, float p) => p > 0f;

	protected static Parameter<float>.Callback IsGTEZero = (StateMachineInstanceType smi, float p) => p >= 0f;

	protected static Parameter<float>.Callback IsOne = (StateMachineInstanceType smi, float p) => p == 1f;

	protected static Parameter<float>.Callback IsLTOne = (StateMachineInstanceType smi, float p) => p < 1f;

	protected static Parameter<float>.Callback IsLTEOne = (StateMachineInstanceType smi, float p) => p <= 1f;

	protected static Parameter<float>.Callback IsGTOne = (StateMachineInstanceType smi, float p) => p > 1f;

	protected static Parameter<float>.Callback IsGTEOne = (StateMachineInstanceType smi, float p) => p >= 1f;

	protected static Parameter<GameObject>.Callback IsNotNull = (StateMachineInstanceType smi, GameObject p) => p != null;

	protected static Parameter<GameObject>.Callback IsNull = (StateMachineInstanceType smi, GameObject p) => p == null;

	public override void InitializeStates(out BaseState default_state)
	{
		base.InitializeStates(out default_state);
	}

	public static Transition.ConditionCallback Not(Transition.ConditionCallback transition_cb)
	{
		return (StateMachineInstanceType smi) => !transition_cb(smi);
	}

	public override void BindStates()
	{
		BindState(null, root, "root");
		BindStates(root, this);
	}
}
public abstract class GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType> : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, object> where StateMachineType : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, object> where StateMachineInstanceType : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, object>.GameInstance where MasterType : IStateMachineTarget
{
}
public abstract class GameStateMachine<StateMachineType, StateMachineInstanceType> : GameStateMachine<StateMachineType, StateMachineInstanceType, IStateMachineTarget, object> where StateMachineType : GameStateMachine<StateMachineType, StateMachineInstanceType, IStateMachineTarget, object> where StateMachineInstanceType : GameStateMachine<StateMachineType, StateMachineInstanceType, IStateMachineTarget, object>.GameInstance
{
}
