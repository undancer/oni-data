using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class SymbolOverrideInfoGpuData
{
	[StructLayout(LayoutKind.Explicit)]
	public struct SymbolOverrideInfo
	{
		[FieldOffset(0)]
		public float atlas;

		[FieldOffset(4)]
		public float isoverriden;

		[FieldOffset(8)]
		public float unused1;

		[FieldOffset(12)]
		public float unused2;

		[FieldOffset(16)]
		public Vector2 bboxMin;

		[FieldOffset(24)]
		public Vector2 bboxMax;

		[FieldOffset(32)]
		public Vector2 uvMin;

		[FieldOffset(40)]
		public Vector2 uvMax;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct SymbolOverrideInfoToByteConverter
	{
		[FieldOffset(0)]
		public byte[] bytes;

		[FieldOffset(0)]
		public SymbolOverrideInfo[] symbolOverrideInfos;
	}

	public const int FLOATS_PER_SYMBOL_OVERRIDE_INFO = 12;

	private SymbolOverrideInfoToByteConverter symbolOverrideInfoConverter;

	private int symbolCount;

	private SymbolOverrideInfo[] symbolOverrideInfos => symbolOverrideInfoConverter.symbolOverrideInfos;

	public int version { get; private set; }

	public SymbolOverrideInfoGpuData(int symbol_count)
	{
		symbolCount = symbol_count;
		symbolOverrideInfoConverter = new SymbolOverrideInfoToByteConverter
		{
			bytes = new byte[12 * symbol_count * 4]
		};
		for (int i = 0; i < symbol_count; i++)
		{
			symbolOverrideInfos[i].atlas = 0f;
		}
		MarkDirty();
	}

	private void MarkDirty()
	{
		version++;
	}

	public void SetSymbolOverrideInfo(int symbol_start_idx, int symbol_num_frames, int atlas_idx, KBatchGroupData source_data, int source_start_idx, int source_num_frames)
	{
		for (int i = 0; i < symbol_num_frames; i++)
		{
			int num = symbol_start_idx + i;
			int index = source_start_idx + Math.Min(source_num_frames - 1, i);
			KAnim.Build.SymbolFrameInstance symbolFrameInstance = source_data.symbolFrameInstances[index];
			ref SymbolOverrideInfo reference = ref symbolOverrideInfos[num];
			reference.atlas = atlas_idx;
			reference.isoverriden = 1f;
			reference.bboxMin = symbolFrameInstance.symbolFrame.bboxMin;
			reference.bboxMax = symbolFrameInstance.symbolFrame.bboxMax;
			reference.uvMin = symbolFrameInstance.symbolFrame.uvMin;
			reference.uvMax = symbolFrameInstance.symbolFrame.uvMax;
		}
		MarkDirty();
	}

	public void SetSymbolOverrideInfo(int symbol_idx, ref KAnim.Build.SymbolFrameInstance symbol_frame_instance)
	{
		ref SymbolOverrideInfo reference = ref symbolOverrideInfos[symbol_idx];
		reference.atlas = symbol_frame_instance.buildImageIdx;
		reference.isoverriden = 1f;
		reference.bboxMin = symbol_frame_instance.symbolFrame.bboxMin;
		reference.bboxMax = symbol_frame_instance.symbolFrame.bboxMax;
		reference.uvMin = symbol_frame_instance.symbolFrame.uvMin;
		reference.uvMax = symbol_frame_instance.symbolFrame.uvMax;
		MarkDirty();
	}

	public void WriteToTexture(byte[] data, int data_idx, int instance_idx)
	{
		DebugUtil.Assert(instance_idx * symbolCount * 12 * 4 == data_idx);
		Buffer.BlockCopy(symbolOverrideInfoConverter.bytes, 0, data, data_idx, symbolCount * 12 * 4);
	}
}
