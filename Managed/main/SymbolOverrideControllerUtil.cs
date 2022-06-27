using UnityEngine;

public static class SymbolOverrideControllerUtil
{
	public static SymbolOverrideController AddToPrefab(GameObject prefab)
	{
		SymbolOverrideController result = prefab.AddComponent<SymbolOverrideController>();
		KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
		DebugUtil.Assert(component != null, "SymbolOverrideController must be added after a KBatchedAnimController component.");
		component.usingNewSymbolOverrideSystem = true;
		return result;
	}

	public static void AddBuildOverride(this SymbolOverrideController symbol_override_controller, KAnimFileData anim_file_data, int priority = 0)
	{
		for (int i = 0; i < anim_file_data.build.symbols.Length; i++)
		{
			KAnim.Build.Symbol symbol = anim_file_data.build.symbols[i];
			symbol_override_controller.AddSymbolOverride(new HashedString(symbol.hash.HashValue), symbol, priority);
		}
	}

	public static void RemoveBuildOverride(this SymbolOverrideController symbol_override_controller, KAnimFileData anim_file_data, int priority = 0)
	{
		for (int i = 0; i < anim_file_data.build.symbols.Length; i++)
		{
			KAnim.Build.Symbol symbol = anim_file_data.build.symbols[i];
			symbol_override_controller.RemoveSymbolOverride(new HashedString(symbol.hash.HashValue), priority);
		}
	}

	public static void TryRemoveBuildOverride(this SymbolOverrideController symbol_override_controller, KAnimFileData anim_file_data, int priority = 0)
	{
		for (int i = 0; i < anim_file_data.build.symbols.Length; i++)
		{
			KAnim.Build.Symbol symbol = anim_file_data.build.symbols[i];
			symbol_override_controller.TryRemoveSymbolOverride(new HashedString(symbol.hash.HashValue), priority);
		}
	}

	public static void TryRemoveSymbolOverride(this SymbolOverrideController symbol_override_controller, HashedString target_symbol, int priority = 0)
	{
		if (symbol_override_controller.GetSymbolOverrideIdx(target_symbol, priority) >= 0)
		{
			symbol_override_controller.RemoveSymbolOverride(target_symbol, priority);
		}
	}

	public static void ApplySymbolOverridesByAffix(this SymbolOverrideController symbol_override_controller, KAnimFile anim_file, string prefix = null, string postfix = null, int priority = 0)
	{
		for (int i = 0; i < anim_file.GetData().build.symbols.Length; i++)
		{
			KAnim.Build.Symbol symbol = anim_file.GetData().build.symbols[i];
			string text = HashCache.Get().Get(symbol.hash);
			if (prefix != null && postfix != null)
			{
				if (text.StartsWith(prefix) && text.EndsWith(postfix))
				{
					string text2 = text.Substring(prefix.Length, text.Length - prefix.Length);
					text2 = text2.Substring(0, text2.Length - postfix.Length);
					symbol_override_controller.AddSymbolOverride(text2, symbol, priority);
				}
			}
			else if (prefix != null && text.StartsWith(prefix))
			{
				symbol_override_controller.AddSymbolOverride(text.Substring(prefix.Length, text.Length - prefix.Length), symbol, priority);
			}
			else if (postfix != null && text.EndsWith(postfix))
			{
				symbol_override_controller.AddSymbolOverride(text.Substring(0, text.Length - postfix.Length), symbol, priority);
			}
		}
	}
}
