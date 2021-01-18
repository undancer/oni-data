using System;
using System.Collections.Generic;

public class ChoreTable
{
	public class Builder
	{
		private struct Info
		{
			public int interruptGroupId;

			public StateMachine.BaseDef def;
		}

		private int interruptGroupId;

		private List<Info> infos = new List<Info>();

		public Builder PushInterruptGroup()
		{
			interruptGroupId++;
			return this;
		}

		public Builder PopInterruptGroup()
		{
			DebugUtil.Assert(interruptGroupId > 0);
			interruptGroupId--;
			return this;
		}

		public Builder Add(StateMachine.BaseDef def, bool condition = true)
		{
			if (condition)
			{
				Info info = default(Info);
				info.interruptGroupId = interruptGroupId;
				info.def = def;
				Info item = info;
				infos.Add(item);
			}
			return this;
		}

		public ChoreTable CreateTable()
		{
			DebugUtil.Assert(interruptGroupId == 0);
			Entry[] array = new Entry[infos.Count];
			Stack<int> stack = new Stack<int>();
			for (int i = 0; i < infos.Count; i++)
			{
				int priority = 10000 - i * 100;
				int num = 10000 - i * 100;
				int num2 = infos[i].interruptGroupId;
				if (num2 != 0)
				{
					if (stack.Count != num2)
					{
						stack.Push(num);
					}
					else
					{
						num = stack.Peek();
					}
				}
				else if (stack.Count > 0)
				{
					stack.Pop();
				}
				array[i] = new Entry(infos[i].def, priority, num);
			}
			return new ChoreTable(array);
		}
	}

	public class ChoreTableChore<StateMachineType, StateMachineInstanceType> : Chore<StateMachineInstanceType> where StateMachineInstanceType : StateMachine.Instance
	{
		public ChoreTableChore(StateMachine.BaseDef state_machine_def, ChoreType chore_type, KPrefabID prefab_id)
			: base(chore_type, (IStateMachineTarget)prefab_id, prefab_id.GetComponent<ChoreProvider>(), run_until_complete: true, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.basic, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.WorkTime)
		{
			showAvailabilityInHoverText = false;
			base.smi = state_machine_def.CreateSMI(this) as StateMachineInstanceType;
		}
	}

	public struct Entry
	{
		public Type choreClassType;

		public ChoreType choreType;

		public StateMachine.BaseDef stateMachineDef;

		public Entry(StateMachine.BaseDef state_machine_def, int priority, int interrupt_priority)
		{
			Type stateMachineInstanceType = Singleton<StateMachineManager>.Instance.CreateStateMachine(state_machine_def.GetStateMachineType()).GetStateMachineInstanceType();
			Type[] typeArguments = new Type[2]
			{
				state_machine_def.GetStateMachineType(),
				stateMachineInstanceType
			};
			choreClassType = typeof(ChoreTableChore<, >).MakeGenericType(typeArguments);
			choreType = new ChoreType(state_machine_def.ToString(), null, new string[0], "", "", "", "", new Tag[0], priority, priority);
			choreType.interruptPriority = interrupt_priority;
			stateMachineDef = state_machine_def;
		}
	}

	public class Instance
	{
		private struct Entry
		{
			public Chore chore;

			public Entry(ChoreTable.Entry chore_table_entry, KPrefabID prefab_id)
			{
				parameters[0] = chore_table_entry.stateMachineDef;
				parameters[1] = chore_table_entry.choreType;
				parameters[2] = prefab_id;
				chore = (Chore)Activator.CreateInstance(chore_table_entry.choreClassType, parameters);
			}

			public void OnCleanUp(KPrefabID prefab_id)
			{
				if (chore != null)
				{
					chore.Cancel("ChoreTable.Instance.OnCleanUp");
					chore = null;
				}
			}
		}

		private static object[] parameters = new object[3];

		private ListPool<Entry, Instance>.PooledList entries;

		public static void ResetParameters()
		{
			for (int i = 0; i < parameters.Length; i++)
			{
				parameters[i] = null;
			}
		}

		public Instance(ChoreTable chore_table, KPrefabID prefab_id)
		{
			entries = ListPool<Entry, Instance>.Allocate();
			for (int i = 0; i < chore_table.entries.Length; i++)
			{
				entries.Add(new Entry(chore_table.entries[i], prefab_id));
			}
		}

		public void OnCleanUp(KPrefabID prefab_id)
		{
			for (int i = 0; i < entries.Count; i++)
			{
				entries[i].OnCleanUp(prefab_id);
			}
			entries.Recycle();
			entries = null;
		}
	}

	private Entry[] entries;

	public ChoreTable(Entry[] entries)
	{
		this.entries = entries;
	}

	public int GetChorePriority<StateMachineType>(ChoreConsumer chore_consumer)
	{
		Entry[] array = entries;
		for (int i = 0; i < array.Length; i++)
		{
			Entry entry = array[i];
			if (entry.stateMachineDef.GetStateMachineType() == typeof(StateMachineType))
			{
				return entry.choreType.priority;
			}
		}
		Debug.LogError(chore_consumer.name + "'s chore table does not have an entry for: " + typeof(StateMachineType).Name);
		return -1;
	}
}
