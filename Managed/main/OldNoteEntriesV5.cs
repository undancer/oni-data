using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

public class OldNoteEntriesV5
{
	[StructLayout(LayoutKind.Explicit)]
	public struct NoteEntry
	{
		[FieldOffset(0)]
		public int reportEntryId;

		[FieldOffset(4)]
		public int noteHash;

		[FieldOffset(8)]
		public float value;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct NoteEntryArray
	{
		[FieldOffset(0)]
		public byte[] bytes;

		[FieldOffset(0)]
		public NoteEntry[] structs;

		public int StructSizeInBytes => Marshal.SizeOf(typeof(NoteEntry));
	}

	public struct NoteStorageBlock
	{
		public int entryCount;

		public NoteEntryArray entries;

		public void Deserialize(BinaryReader reader)
		{
			entryCount = reader.ReadInt32();
			entries.bytes = reader.ReadBytes(entries.StructSizeInBytes * entryCount);
		}
	}

	public List<NoteStorageBlock> storageBlocks = new List<NoteStorageBlock>();

	public void Deserialize(BinaryReader reader)
	{
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			NoteStorageBlock item = default(NoteStorageBlock);
			item.Deserialize(reader);
			storageBlocks.Add(item);
		}
	}
}
