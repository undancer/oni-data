internal abstract class DivisibleTask<SharedData> : IWorkItem<SharedData>
{
	public string name;

	public int start;

	public int end;

	public void Run(SharedData sharedData)
	{
		RunDivision(sharedData);
	}

	protected DivisibleTask(string name)
	{
		this.name = name;
	}

	protected abstract void RunDivision(SharedData sharedData);
}
