using Klei.AI;
using UnityEngine;

public class ChoreConsumerState
{
	public KPrefabID prefabid;

	public GameObject gameObject;

	public ChoreConsumer consumer;

	public ChoreProvider choreProvider;

	public Navigator navigator;

	public Ownable ownable;

	public Assignables assignables;

	public MinionResume resume;

	public ChoreDriver choreDriver;

	public Schedulable schedulable;

	public Traits traits;

	public Equipment equipment;

	public Storage storage;

	public ConsumableConsumer consumableConsumer;

	public KSelectable selectable;

	public Worker worker;

	public SolidTransferArm solidTransferArm;

	public bool hasSolidTransferArm;

	public ScheduleBlock scheduleBlock;

	public ChoreConsumerState(ChoreConsumer consumer)
	{
		this.consumer = consumer;
		navigator = consumer.GetComponent<Navigator>();
		prefabid = consumer.GetComponent<KPrefabID>();
		ownable = consumer.GetComponent<Ownable>();
		gameObject = consumer.gameObject;
		solidTransferArm = consumer.GetComponent<SolidTransferArm>();
		hasSolidTransferArm = solidTransferArm != null;
		resume = consumer.GetComponent<MinionResume>();
		choreDriver = consumer.GetComponent<ChoreDriver>();
		schedulable = consumer.GetComponent<Schedulable>();
		traits = consumer.GetComponent<Traits>();
		choreProvider = consumer.GetComponent<ChoreProvider>();
		MinionIdentity component = consumer.GetComponent<MinionIdentity>();
		if (component != null)
		{
			if (component.assignableProxy == null)
			{
				component.assignableProxy = MinionAssignablesProxy.InitAssignableProxy(component.assignableProxy, component);
			}
			assignables = component.GetSoleOwner();
			equipment = component.GetEquipment();
		}
		else
		{
			assignables = consumer.GetComponent<Assignables>();
			equipment = consumer.GetComponent<Equipment>();
		}
		storage = consumer.GetComponent<Storage>();
		consumableConsumer = consumer.GetComponent<ConsumableConsumer>();
		worker = consumer.GetComponent<Worker>();
		selectable = consumer.GetComponent<KSelectable>();
		if (schedulable != null)
		{
			int blockIdx = Schedule.GetBlockIdx();
			scheduleBlock = schedulable.GetSchedule().GetBlock(blockIdx);
		}
	}

	public void Refresh()
	{
		if (schedulable != null)
		{
			int blockIdx = Schedule.GetBlockIdx();
			Schedule schedule = schedulable.GetSchedule();
			if (schedule != null)
			{
				scheduleBlock = schedule.GetBlock(blockIdx);
			}
		}
	}
}
