using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SymbolOverrideController")]
public class SymbolOverrideController : KMonoBehaviour
{
	[Serializable]
	public struct SymbolEntry
	{
		public HashedString targetSymbol;

		[NonSerialized]
		public KAnim.Build.Symbol sourceSymbol;

		public HashedString sourceSymbolId;

		public HashedString sourceSymbolBatchTag;

		public int priority;
	}

	private struct SymbolToOverride
	{
		public KAnim.Build.Symbol sourceSymbol;

		public HashedString targetSymbol;

		public KBatchGroupData data;

		public int atlasIdx;
	}

	private class BatchGroupInfo
	{
		public KAnim.Build build;

		public int atlasIdx;

		public KBatchGroupData data;
	}

	public bool applySymbolOverridesEveryFrame;

	[SerializeField]
	private List<SymbolEntry> symbolOverrides = new List<SymbolEntry>();

	private KAnimBatch.AtlasList atlases;

	private KBatchedAnimController animController;

	private FaceGraph faceGraph;

	private bool requiresSorting;

	public SymbolEntry[] GetSymbolOverrides => symbolOverrides.ToArray();

	public int version
	{
		get;
		private set;
	}

	protected override void OnPrefabInit()
	{
		animController = GetComponent<KBatchedAnimController>();
		DebugUtil.Assert(GetComponent<KBatchedAnimController>() != null, "SymbolOverrideController requires KBatchedAnimController");
		DebugUtil.Assert(GetComponent<KBatchedAnimController>().usingNewSymbolOverrideSystem, "SymbolOverrideController requires usingNewSymbolOverrideSystem to be set to true. Try adding the component by calling: SymbolOverrideControllerUtil.AddToPrefab");
		for (int i = 0; i < symbolOverrides.Count; i++)
		{
			SymbolEntry value = symbolOverrides[i];
			value.sourceSymbol = KAnimBatchManager.Instance().GetBatchGroupData(value.sourceSymbolBatchTag).GetSymbol(value.sourceSymbolId);
			symbolOverrides[i] = value;
		}
		atlases = new KAnimBatch.AtlasList(0);
		faceGraph = GetComponent<FaceGraph>();
	}

	public void AddSymbolOverride(HashedString target_symbol, KAnim.Build.Symbol source_symbol, int priority = 0)
	{
		if (source_symbol == null)
		{
			throw new Exception("NULL source symbol when overriding: " + target_symbol.ToString());
		}
		SymbolEntry symbolEntry = default(SymbolEntry);
		symbolEntry.targetSymbol = target_symbol;
		symbolEntry.sourceSymbol = source_symbol;
		symbolEntry.sourceSymbolId = new HashedString(source_symbol.hash.HashValue);
		symbolEntry.sourceSymbolBatchTag = source_symbol.build.batchTag;
		symbolEntry.priority = priority;
		SymbolEntry symbolEntry2 = symbolEntry;
		int symbolOverrideIdx = GetSymbolOverrideIdx(target_symbol, priority);
		if (symbolOverrideIdx >= 0)
		{
			symbolOverrides[symbolOverrideIdx] = symbolEntry2;
		}
		else
		{
			symbolOverrides.Add(symbolEntry2);
		}
		MarkDirty();
	}

	public void RemoveSymbolOverride(HashedString target_symbol, int priority = 0)
	{
		for (int i = 0; i < symbolOverrides.Count; i++)
		{
			SymbolEntry symbolEntry = symbolOverrides[i];
			if (symbolEntry.targetSymbol == target_symbol && symbolEntry.priority == priority)
			{
				symbolOverrides.RemoveAt(i);
				break;
			}
		}
		MarkDirty();
	}

	public void RemoveAllSymbolOverrides(int priority = 0)
	{
		symbolOverrides.RemoveAll((SymbolEntry x) => x.priority >= priority);
		MarkDirty();
	}

	public int GetSymbolOverrideIdx(HashedString target_symbol, int priority = 0)
	{
		for (int i = 0; i < symbolOverrides.Count; i++)
		{
			SymbolEntry symbolEntry = symbolOverrides[i];
			if (symbolEntry.targetSymbol == target_symbol && symbolEntry.priority == priority)
			{
				return i;
			}
		}
		return -1;
	}

	public int GetAtlasIdx(Texture2D atlas)
	{
		return atlases.GetAtlasIdx(atlas);
	}

	public void ApplyOverrides()
	{
		if (requiresSorting)
		{
			symbolOverrides.Sort((SymbolEntry x, SymbolEntry y) => x.priority - y.priority);
			requiresSorting = false;
		}
		KAnimBatch batch = animController.GetBatch();
		DebugUtil.Assert(batch != null);
		KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(animController.batchGroupID);
		int count = batch.atlases.Count;
		atlases.Clear(count);
		DictionaryPool<KAnim.Build, BatchGroupInfo, SymbolOverrideController>.PooledDictionary pooledDictionary = DictionaryPool<KAnim.Build, BatchGroupInfo, SymbolOverrideController>.Allocate();
		foreach (SymbolEntry symbolOverride in symbolOverrides)
		{
			if (!pooledDictionary.TryGetValue(symbolOverride.sourceSymbol.build, out var value))
			{
				value = new BatchGroupInfo
				{
					build = symbolOverride.sourceSymbol.build,
					data = KAnimBatchManager.Instance().GetBatchGroupData(symbolOverride.sourceSymbol.build.batchTag)
				};
				Texture2D texture = symbolOverride.sourceSymbol.build.GetTexture(0);
				int num = (value.atlasIdx = atlases.Add(texture));
				pooledDictionary[value.build] = value;
			}
			KAnim.Build.Symbol symbol = batchGroupData.GetSymbol(symbolOverride.targetSymbol);
			if (symbol != null)
			{
				animController.SetSymbolOverrides(symbol.firstFrameIdx, symbol.numFrames, value.atlasIdx, value.data, symbolOverride.sourceSymbol.firstFrameIdx, symbolOverride.sourceSymbol.numFrames);
			}
		}
		pooledDictionary.Recycle();
		if (faceGraph != null)
		{
			faceGraph.ApplyShape();
		}
	}

	public void ApplyAtlases()
	{
		KAnimBatch batch = animController.GetBatch();
		atlases.Apply(batch.matProperties);
	}

	public void MarkDirty()
	{
		if (animController != null)
		{
			animController.SetDirty();
		}
		version++;
		requiresSorting = true;
	}
}
