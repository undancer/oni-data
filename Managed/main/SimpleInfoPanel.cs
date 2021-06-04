using UnityEngine;

public class SimpleInfoPanel
{
	protected SimpleInfoScreen simpleInfoRoot;

	public SimpleInfoPanel(SimpleInfoScreen simpleInfoRoot)
	{
		this.simpleInfoRoot = simpleInfoRoot;
	}

	public virtual void Refresh(CollapsibleDetailContentPanel panel, GameObject selectedTarget)
	{
	}
}
