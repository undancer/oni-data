using System.Collections.Generic;
using UnityEngine;

public class KBatchGroupData
{
	public const int SIZE_OF_SYMBOL_FRAME_ELEMENT = 16;

	public const int SIZE_OF_ANIM_FRAME = 4;

	public const int SIZE_OF_ANIM_FRAME_ELEMENT = 16;

	private const int MAX_VISIBLE_SYMBOLS = 120;

	public const int MAX_GROUP_SIZE = 30;

	private const int NULL_DATA_FRAME_ID = -1010;

	public HashedString groupID
	{
		get;
		private set;
	}

	public bool isSwap
	{
		get;
		private set;
	}

	public int maxVisibleSymbols
	{
		get;
		private set;
	}

	public int maxSymbolsPerBuild => frameElementSymbols.Count;

	public int maxSymbolFrameInstancesPerbuild => symbolFrameInstances.Count;

	public int animDataStartOffset => symbolFrameInstances.Count * 16;

	public List<KAnim.Anim> anims
	{
		get;
		private set;
	}

	public Dictionary<KAnimHashedString, int> animIndex
	{
		get;
		private set;
	}

	public Dictionary<KAnimHashedString, int> animCount
	{
		get;
		private set;
	}

	public List<KAnim.Anim.Frame> animFrames
	{
		get;
		private set;
	}

	public List<KAnim.Anim.FrameElement> frameElements
	{
		get;
		private set;
	}

	public List<KAnim.Build> builds
	{
		get;
		private set;
	}

	public List<KAnim.Build.Symbol> frameElementSymbols
	{
		get;
		private set;
	}

	public Dictionary<KAnimHashedString, int> frameElementSymbolIndices
	{
		get;
		private set;
	}

	public List<KAnim.Build.SymbolFrameInstance> symbolFrameInstances
	{
		get;
		private set;
	}

	public Dictionary<KAnimHashedString, int> textureStartIndex
	{
		get;
		private set;
	}

	public Dictionary<KAnimHashedString, int> firstSymbolIndex
	{
		get;
		private set;
	}

	public List<Texture2D> textures
	{
		get;
		private set;
	}

	public KBatchGroupData(HashedString id)
	{
		groupID = id;
		maxVisibleSymbols = 1;
		Init();
	}

	private void Init()
	{
		anims = new List<KAnim.Anim>();
		animIndex = new Dictionary<KAnimHashedString, int>();
		animCount = new Dictionary<KAnimHashedString, int>();
		animFrames = new List<KAnim.Anim.Frame>();
		frameElements = new List<KAnim.Anim.FrameElement>();
		builds = new List<KAnim.Build>();
		frameElementSymbols = new List<KAnim.Build.Symbol>();
		frameElementSymbolIndices = new Dictionary<KAnimHashedString, int>();
		symbolFrameInstances = new List<KAnim.Build.SymbolFrameInstance>();
		textures = new List<Texture2D>();
		textureStartIndex = new Dictionary<KAnimHashedString, int>();
		firstSymbolIndex = new Dictionary<KAnimHashedString, int>();
	}

	public void FreeResources()
	{
		if (anims != null)
		{
			anims.Clear();
			anims = null;
		}
		if (animIndex != null)
		{
			animIndex.Clear();
			animIndex = null;
		}
		if (animCount != null)
		{
			animCount.Clear();
			animCount = null;
		}
		if (animFrames != null)
		{
			animFrames.Clear();
			animFrames = null;
		}
		if (frameElements != null)
		{
			frameElements.Clear();
			frameElements = null;
		}
		if (builds != null)
		{
			builds.Clear();
			builds = null;
		}
		if (frameElementSymbols != null)
		{
			frameElementSymbols.Clear();
			frameElementSymbols = null;
		}
		if (symbolFrameInstances != null)
		{
			symbolFrameInstances.Clear();
			symbolFrameInstances = null;
		}
		if (textures != null)
		{
			textures.Clear();
			textures = null;
		}
		if (textureStartIndex != null)
		{
			textureStartIndex.Clear();
			textureStartIndex = null;
		}
		if (firstSymbolIndex != null)
		{
			firstSymbolIndex.Clear();
			firstSymbolIndex = null;
		}
	}

	public KAnim.Build AddNewBuildFile(KAnimHashedString fileHash)
	{
		textureStartIndex.Add(fileHash, textures.Count);
		firstSymbolIndex.Add(fileHash, GetSymbolCount());
		KAnim.Build build = new KAnim.Build();
		build.textureStartIdx = textures.Count;
		build.fileHash = fileHash;
		build.index = builds.Count;
		builds.Add(build);
		return build;
	}

	public void AddTextures(List<Texture2D> buildtextures)
	{
		textures.AddRange(buildtextures);
	}

	public void AddAnim(KAnim.Anim anim)
	{
		Debug.Assert(anim.index == anims.Count);
		anims.Add(anim);
	}

	public KAnim.Anim GetAnim(int anim)
	{
		if (anim < 0 || anim >= anims.Count)
		{
			Debug.LogError($"Anim [{anim}] out of range [{anims.Count}] in batch [{groupID}]");
		}
		return anims[anim];
	}

	public KAnim.Build GetBuild(int index)
	{
		return builds[index];
	}

	public void UpdateMaxVisibleSymbols(int newCount)
	{
		maxVisibleSymbols = Mathf.Min(120, Mathf.Max(maxVisibleSymbols, newCount));
	}

	public KAnim.Build.Symbol GetSymbol(KAnimHashedString symbol_name)
	{
		int value = 0;
		if (!frameElementSymbolIndices.TryGetValue(symbol_name, out value))
		{
			return null;
		}
		return frameElementSymbols[value];
	}

	public KAnim.Build.Symbol GetSymbol(int index)
	{
		if (index >= 0 && index < frameElementSymbols.Count)
		{
			return frameElementSymbols[index];
		}
		return null;
	}

	public void AddBuildSymbol(KAnim.Build.Symbol symbol)
	{
		if (!frameElementSymbolIndices.ContainsKey(symbol.hash))
		{
			frameElementSymbolIndices.Add(symbol.hash, frameElementSymbols.Count);
		}
		frameElementSymbols.Add(symbol);
	}

	public int GetSymbolCount()
	{
		return frameElementSymbols.Count;
	}

	public KAnim.Build.SymbolFrameInstance GetSymbolFrameInstance(int index)
	{
		if (index >= 0 && index < symbolFrameInstances.Count)
		{
			return symbolFrameInstances[index];
		}
		KAnim.Build.SymbolFrameInstance result = default(KAnim.Build.SymbolFrameInstance);
		result.symbolIdx = -1;
		return result;
	}

	public Texture2D GetTexure(int index)
	{
		if (index < 0 || textures == null || index >= textures.Count)
		{
			return null;
		}
		return textures[index];
	}

	public KAnim.Build.Symbol GetBuildSymbol(int idx)
	{
		if (frameElementSymbols == null || idx < 0 || idx >= frameElementSymbols.Count)
		{
			return null;
		}
		return frameElementSymbols[idx];
	}

	public KAnim.Anim.Frame GetFrame(int index)
	{
		if (index < 0 || index >= animFrames.Count)
		{
			return KAnim.Anim.Frame.InvalidFrame;
		}
		return animFrames[index];
	}

	public KAnim.Anim.FrameElement GetFrameElement(int index)
	{
		return frameElements[index];
	}

	public List<KAnim.Anim.Frame> GetAnimFrames()
	{
		return animFrames;
	}

	public List<KAnim.Anim.FrameElement> GetAnimFrameElements()
	{
		return frameElements;
	}

	public int GetBuildSymbolFrameCount()
	{
		return symbolFrameInstances.Count;
	}

	public void WriteAnimData(int start_index, float[] data)
	{
		List<KAnim.Anim.Frame> animFrames = GetAnimFrames();
		List<KAnim.Anim.FrameElement> animFrameElements = GetAnimFrameElements();
		int num = 1 + ((animFrames.Count == 0) ? symbolFrameInstances.Count : animFrames.Count);
		if (animFrames.Count == 0 && symbolFrameInstances.Count == 0 && animFrameElements.Count == 0)
		{
			Debug.LogError("Eh, no data " + animFrames.Count + " " + symbolFrameInstances.Count + " " + animFrameElements.Count);
		}
		data[start_index++] = num;
		data[start_index++] = animFrames.Count;
		data[start_index++] = animFrameElements.Count;
		data[start_index++] = symbolFrameInstances.Count;
		if (animFrames.Count == 0)
		{
			for (int i = 0; i < symbolFrameInstances.Count; i++)
			{
				WriteAnimFrame(data, start_index, i, i, 1, i);
				start_index += 4;
			}
			for (int j = 0; j < symbolFrameInstances.Count; j++)
			{
				WriteAnimFrameElement(data, start_index, j, j, Matrix2x3.identity, Color.white, 0);
				start_index += 16;
			}
			return;
		}
		for (int k = 0; k < animFrames.Count; k++)
		{
			Write(data, start_index, k, animFrames[k]);
			start_index += 4;
		}
		for (int l = 0; l < animFrameElements.Count; l++)
		{
			KAnim.Anim.FrameElement element = animFrameElements[l];
			if (element.symbol == KGlobalAnimParser.MISSING_SYMBOL)
			{
				WriteAnimFrameElement(data, start_index, -1, l, Matrix2x3.identity, Color.white, 0);
			}
			else
			{
				KAnim.Build.Symbol buildSymbol = GetBuildSymbol(element.symbolIdx);
				if (buildSymbol == null)
				{
					Debug.LogError(string.Concat("Missing symbol for Anim Frame Element: [", HashCache.Get().Get(element.symbol), ": ", element.symbol, "]"));
				}
				int frameIdx = buildSymbol.GetFrameIdx(element.frame);
				Write(data, start_index, frameIdx, l, element);
			}
			start_index += 16;
		}
	}

	public int GetFirstIndex(KAnimHashedString symbol)
	{
		return frameElementSymbols.FindIndex((KAnim.Build.Symbol fes) => fes.hash == symbol);
	}

	public int GetSymbolIndex(KAnimHashedString symbol)
	{
		int value = 0;
		if (!frameElementSymbolIndices.TryGetValue(symbol, out value))
		{
			return -1;
		}
		return value;
	}

	public int WriteBuildData(List<KAnim.Build.SymbolFrameInstance> symbol_frame_instances, float[] data)
	{
		int num = 0;
		for (num = 0; num < symbol_frame_instances.Count; num++)
		{
			Write(data, num * 16, num, symbolFrameInstances[num].buildImageIdx, symbol_frame_instances[num]);
		}
		return num * 16;
	}

	private void Write(float[] data, int startIndex, int thisFrameIndex, int atlasIndex, KAnim.Build.SymbolFrameInstance symbol_frame_instance)
	{
		data[startIndex++] = atlasIndex;
		data[startIndex++] = thisFrameIndex;
		data[startIndex++] = symbol_frame_instance.symbolIdx;
		KAnim.Build.SymbolFrame symbolFrame = symbol_frame_instance.symbolFrame;
		KAnim.Build.Symbol buildSymbol = GetBuildSymbol(symbol_frame_instance.symbolIdx);
		if (buildSymbol == null || symbolFrame == null)
		{
			data[startIndex++] = 0f;
			data[startIndex++] = 0f;
			data[startIndex++] = 0f;
			data[startIndex++] = 0f;
		}
		else
		{
			data[startIndex++] = buildSymbol.numFrames;
			data[startIndex++] = buildSymbol.flags;
			if (firstSymbolIndex.ContainsKey(buildSymbol.build.fileHash))
			{
				data[startIndex++] = firstSymbolIndex[buildSymbol.build.fileHash];
			}
			else
			{
				data[startIndex++] = 0f;
			}
			data[startIndex++] = buildSymbol.symbolIndexInSourceBuild;
		}
		data[startIndex++] = 3.452817E+09f;
		if (symbolFrame != null)
		{
			data[startIndex++] = symbolFrame.bboxMin.x;
			data[startIndex++] = symbolFrame.bboxMin.y;
			data[startIndex++] = symbolFrame.bboxMax.x;
			data[startIndex++] = symbolFrame.bboxMax.y;
			data[startIndex++] = symbolFrame.uvMin.x;
			data[startIndex++] = symbolFrame.uvMin.y;
			data[startIndex++] = symbolFrame.uvMax.x;
			data[startIndex++] = symbolFrame.uvMax.y;
		}
	}

	private void WriteAnimFrame(float[] data, int startIndex, int firstElementIdx, int idx, int numElements, int thisFrameIndex)
	{
		data[startIndex++] = firstElementIdx;
		data[startIndex++] = numElements;
		data[startIndex++] = thisFrameIndex;
		data[startIndex++] = idx;
	}

	private void Write(float[] data, int startIndex, int thisFrameIndex, KAnim.Anim.Frame frame)
	{
		WriteAnimFrame(data, startIndex, frame.firstElementIdx, frame.idx, frame.numElements, thisFrameIndex);
	}

	private void WriteAnimFrameElement(float[] data, int startIndex, int symbolFrameIdx, int thisFrameIndex, Matrix2x3 transform, Color colour, int flags)
	{
		if (symbolFrameIdx != -1010)
		{
			data[startIndex++] = symbolFrameIdx;
			data[startIndex++] = thisFrameIndex;
			data[startIndex++] = flags;
			data[startIndex++] = 0f;
			data[startIndex++] = colour.r;
			data[startIndex++] = colour.g;
			data[startIndex++] = colour.b;
			data[startIndex++] = colour.a;
			data[startIndex++] = transform.m00;
			data[startIndex++] = transform.m01;
			data[startIndex++] = transform.m02;
			data[startIndex++] = 2.8801546E+09f;
			data[startIndex++] = transform.m10;
			data[startIndex++] = transform.m11;
			data[startIndex++] = transform.m12;
			data[startIndex++] = 3.1664858E+09f;
		}
		else
		{
			data[startIndex++] = symbolFrameIdx;
			data[startIndex++] = thisFrameIndex;
			data[startIndex++] = flags;
			data[startIndex++] = -1f;
			data[startIndex++] = colour.r;
			data[startIndex++] = colour.g;
			data[startIndex++] = colour.b;
			data[startIndex++] = colour.a;
			data[startIndex++] = 0f;
			data[startIndex++] = 0f;
			data[startIndex++] = 0f;
			data[startIndex++] = 2.8801546E+09f;
			data[startIndex++] = 0f;
			data[startIndex++] = 0f;
			data[startIndex++] = 0f;
			data[startIndex++] = 3.1664858E+09f;
		}
	}

	private void WriteNullFrameElement(float[] data, int startIndex, int thisFrameIndex)
	{
		WriteAnimFrameElement(data, startIndex, -1010, thisFrameIndex, Matrix2x3.identity, Color.black, 0);
	}

	private void Write(float[] data, int startIndex, int symbolFrameIdx, int thisFrameIndex, KAnim.Anim.FrameElement element)
	{
		WriteAnimFrameElement(data, startIndex, symbolFrameIdx, thisFrameIndex, element.transform, element.multColour, element.flags);
	}
}
