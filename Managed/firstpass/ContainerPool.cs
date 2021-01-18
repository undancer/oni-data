using System.Collections.Generic;

public abstract class ContainerPool
{
	public abstract string GetName();
}
public class ContainerPool<ContainerType, PoolIdentifier> : ContainerPool where ContainerType : new()
{
	private Stack<ContainerType> freeContainers = new Stack<ContainerType>();

	public ContainerType Allocate()
	{
		lock (freeContainers)
		{
			if (freeContainers.Count == 0)
			{
				return new ContainerType();
			}
			return freeContainers.Pop();
		}
	}

	public void Free(ContainerType container)
	{
		lock (freeContainers)
		{
			freeContainers.Push(container);
		}
	}

	public override string GetName()
	{
		return typeof(PoolIdentifier).Name + "." + typeof(ContainerType).Name;
	}
}
