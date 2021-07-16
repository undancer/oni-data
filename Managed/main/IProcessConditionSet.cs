using System.Collections.Generic;

public interface IProcessConditionSet
{
	List<ProcessCondition> GetConditionSet(ProcessCondition.ProcessConditionType conditionType);
}
