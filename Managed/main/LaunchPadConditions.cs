using System.Collections.Generic;

public class LaunchPadConditions : KMonoBehaviour, IProcessConditionSet
{
	private List<ProcessCondition> conditions;

	public List<ProcessCondition> GetConditionSet(ProcessCondition.ProcessConditionType conditionType)
	{
		if (conditionType != ProcessCondition.ProcessConditionType.RocketStorage)
		{
			return null;
		}
		return conditions;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		conditions = new List<ProcessCondition>();
		conditions.Add(new TransferCargoCompleteCondition(base.gameObject));
	}
}
