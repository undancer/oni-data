using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Def : ScriptableObject
{
	public string PrefabID;

	public Tag Tag;

	private static Dictionary<Tuple<KAnimFile, string, bool>, Sprite> knownUISprites = new Dictionary<Tuple<KAnimFile, string, bool>, Sprite>();

	private const string DEFAULT_SPRITE = "unknown";

	public virtual string Name => null;

	public virtual void InitDef()
	{
		Tag = TagManager.Create(PrefabID);
	}

	public static Tuple<Sprite, Color> GetUISprite(object item, string animName = "ui", bool centered = false)
	{
		if (item is Substance)
		{
			return GetUISprite(ElementLoader.FindElementByHash((item as Substance).elementID), animName, centered);
		}
		if (item is Element)
		{
			if ((item as Element).IsSolid)
			{
				return new Tuple<Sprite, Color>(GetUISpriteFromMultiObjectAnim((item as Element).substance.anim, animName, centered), Color.white);
			}
			if ((item as Element).IsLiquid)
			{
				return new Tuple<Sprite, Color>(Assets.GetSprite("element_liquid"), (item as Element).substance.uiColour);
			}
			if ((item as Element).IsGas)
			{
				return new Tuple<Sprite, Color>(Assets.GetSprite("element_gas"), (item as Element).substance.uiColour);
			}
			return new Tuple<Sprite, Color>(null, Color.clear);
		}
		if (item is AsteroidGridEntity)
		{
			return new Tuple<Sprite, Color>(((AsteroidGridEntity)item).GetUISprite(), Color.white);
		}
		if (item is GameObject)
		{
			GameObject gameObject = item as GameObject;
			if (ElementLoader.GetElement(gameObject.PrefabID()) != null)
			{
				return GetUISprite(ElementLoader.GetElement(gameObject.PrefabID()), animName, centered);
			}
			CreatureBrain component = gameObject.GetComponent<CreatureBrain>();
			if (component != null)
			{
				animName = component.symbolPrefix + "ui";
			}
			SpaceArtifact component2 = gameObject.GetComponent<SpaceArtifact>();
			if (component2 != null)
			{
				animName = component2.GetUIAnim();
			}
			if (gameObject.HasTag(GameTags.Egg))
			{
				IncubationMonitor.Def def = gameObject.GetDef<IncubationMonitor.Def>();
				if (def != null)
				{
					GameObject prefab = Assets.GetPrefab(def.spawnedCreature);
					if ((bool)prefab)
					{
						component = prefab.GetComponent<CreatureBrain>();
						if ((bool)component && !string.IsNullOrEmpty(component.symbolPrefix))
						{
							animName = component.symbolPrefix + animName;
						}
					}
				}
			}
			KBatchedAnimController component3 = gameObject.GetComponent<KBatchedAnimController>();
			if ((bool)component3)
			{
				Sprite uISpriteFromMultiObjectAnim = GetUISpriteFromMultiObjectAnim(component3.AnimFiles[0], animName, centered);
				return new Tuple<Sprite, Color>(uISpriteFromMultiObjectAnim, (uISpriteFromMultiObjectAnim != null) ? Color.white : Color.clear);
			}
			if (gameObject.GetComponent<Building>() != null)
			{
				Sprite uISprite = gameObject.GetComponent<Building>().Def.GetUISprite(animName, centered);
				return new Tuple<Sprite, Color>(uISprite, (uISprite != null) ? Color.white : Color.clear);
			}
			Debug.LogWarningFormat("Can't get sprite for type {0} (no KBatchedAnimController)", item.ToString());
			return new Tuple<Sprite, Color>(Assets.GetSprite("unknown"), Color.grey);
		}
		if (item is string)
		{
			if (Db.Get().Amounts.Exists(item as string))
			{
				return new Tuple<Sprite, Color>(Assets.GetSprite(Db.Get().Amounts.Get(item as string).uiSprite), Color.white);
			}
			if (Db.Get().Attributes.Exists(item as string))
			{
				return new Tuple<Sprite, Color>(Assets.GetSprite(Db.Get().Attributes.Get(item as string).uiSprite), Color.white);
			}
			return GetUISprite((item as string).ToTag(), animName, centered);
		}
		if (item is Tag)
		{
			if (ElementLoader.GetElement((Tag)item) != null)
			{
				return GetUISprite(ElementLoader.GetElement((Tag)item), animName, centered);
			}
			if (Assets.GetPrefab((Tag)item) != null)
			{
				return GetUISprite(Assets.GetPrefab((Tag)item), animName, centered);
			}
			if (Assets.GetSprite(((Tag)item).Name) != null)
			{
				return new Tuple<Sprite, Color>(Assets.GetSprite(((Tag)item).Name), Color.white);
			}
		}
		DebugUtil.DevAssertArgs(false, "Can't get sprite for type ", item.ToString());
		return new Tuple<Sprite, Color>(Assets.GetSprite("unknown"), Color.grey);
	}

	public static Sprite GetUISpriteFromMultiObjectAnim(KAnimFile animFile, string animName = "ui", bool centered = false, string symbolName = "")
	{
		Tuple<KAnimFile, string, bool> key = new Tuple<KAnimFile, string, bool>(animFile, animName, centered);
		if (knownUISprites.ContainsKey(key))
		{
			return knownUISprites[key];
		}
		if (animFile == null)
		{
			DebugUtil.LogWarningArgs(animName, "missing Anim File");
			return Assets.GetSprite("unknown");
		}
		KAnimFileData data = animFile.GetData();
		if (data == null)
		{
			DebugUtil.LogWarningArgs(animName, "KAnimFileData is null");
			return Assets.GetSprite("unknown");
		}
		if (data.build == null)
		{
			return Assets.GetSprite("unknown");
		}
		KAnim.Anim.Frame frame = KAnim.Anim.Frame.InvalidFrame;
		for (int i = 0; i < data.animCount; i++)
		{
			KAnim.Anim anim = data.GetAnim(i);
			if (anim.name == animName)
			{
				frame = anim.GetFrame(data.batchTag, 0);
			}
		}
		if (!frame.IsValid())
		{
			DebugUtil.LogWarningArgs($"missing '{animName}' anim in '{animFile}'");
			return Assets.GetSprite("unknown");
		}
		if (data.elementCount == 0)
		{
			return Assets.GetSprite("unknown");
		}
		KAnim.Anim.FrameElement frameElement = default(KAnim.Anim.FrameElement);
		if (string.IsNullOrEmpty(symbolName))
		{
			symbolName = animName;
		}
		KAnimHashedString symbol_name = new KAnimHashedString(symbolName);
		KAnim.Build.Symbol symbol = data.build.GetSymbol(symbol_name);
		if (symbol == null)
		{
			DebugUtil.LogWarningArgs(animFile.name, animName, "placeSymbol [", frameElement.symbol, "] is missing");
			return Assets.GetSprite("unknown");
		}
		int frame2 = frameElement.frame;
		KAnim.Build.SymbolFrame symbolFrame = symbol.GetFrame(frame2).symbolFrame;
		if (symbolFrame == null)
		{
			DebugUtil.LogWarningArgs(animName, "SymbolFrame [", frameElement.frame, "] is missing");
			return Assets.GetSprite("unknown");
		}
		Texture2D texture = data.build.GetTexture(0);
		Debug.Assert(texture != null, "Invalid texture on " + animFile.name);
		float x = symbolFrame.uvMin.x;
		float x2 = symbolFrame.uvMax.x;
		float y = symbolFrame.uvMax.y;
		float y2 = symbolFrame.uvMin.y;
		int num = (int)((float)texture.width * Mathf.Abs(x2 - x));
		int num2 = (int)((float)texture.height * Mathf.Abs(y2 - y));
		float num3 = Mathf.Abs(symbolFrame.bboxMax.x - symbolFrame.bboxMin.x);
		Rect rect = default(Rect);
		rect.width = num;
		rect.height = num2;
		rect.x = (int)((float)texture.width * x);
		rect.y = (int)((float)texture.height * y);
		float pixelsPerUnit = 100f;
		if (num != 0)
		{
			pixelsPerUnit = 100f / (num3 / (float)num);
		}
		Sprite sprite = Sprite.Create(texture, rect, centered ? new Vector2(0.5f, 0.5f) : Vector2.zero, pixelsPerUnit, 0u, SpriteMeshType.FullRect);
		sprite.name = $"{texture.name}:{animName}:{frameElement.frame.ToString()}:{centered}";
		knownUISprites[key] = sprite;
		return sprite;
	}
}
