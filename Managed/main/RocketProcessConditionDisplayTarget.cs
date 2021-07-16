using System;
using System.Collections.Generic;

public class RocketProcessConditionDisplayTarget : KMonoBehaviour, IProcessConditionSet, ISim1000ms
{
	private CraftModuleInterface craftModuleInterface;

	private Guid statusHandle = Guid.Empty;

	public List<ProcessCondition> GetConditionSet(ProcessCondition.ProcessConditionType conditionType)
	{
		if (craftModuleInterface == null)
		{
			craftModuleInterface = GetComponent<RocketModuleCluster>().CraftInterface;
		}
		return craftModuleInterface.GetConditionSet(conditionType);
	}

	public void Sim1000ms(float dt)
	{
		bool flag = false;
		foreach (ProcessCondition item in GetConditionSet(ProcessCondition.ProcessConditionType.All))
		{
			if (item.EvaluateCondition() == ProcessCondition.Status.Failure)
			{
				flag = true;
				if (statusHandle == Guid.Empty)
				{
					statusHandle = GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.RocketChecklistIncomplete);
				}
				break;
			}
		}
		if (!flag && statusHandle != Guid.Empty)
		{
			GetComponent<KSelectable>().RemoveStatusItem(statusHandle);
		}
	}
}
