using System.Collections.Generic;

public interface IGroupProber
{
	void Occupy(object prober, short serial_no, IEnumerable<int> cells);

	void SetValidSerialNos(object prober, short previous_serial_no, short serial_no);

	bool ReleaseProber(object prober);
}
