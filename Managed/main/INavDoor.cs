public interface INavDoor
{
	bool isSpawned { get; }

	bool IsOpen();

	void Open();

	void Close();
}
