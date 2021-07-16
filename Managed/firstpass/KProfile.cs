using System;

public class KProfile : IDisposable
{
	private string name;

	public KProfile(string name, string group = "Game")
	{
		this.name = name;
	}

	public void Dispose()
	{
	}
}
