using System.IO;
using UnityEngine;

public class BundledAssetsLoader : KMonoBehaviour
{
	public static BundledAssetsLoader instance;

	public BundledAssets Expansion1Assets
	{
		get;
		private set;
	}

	protected override void OnPrefabInit()
	{
		instance = this;
		Debug.Log("Expansion1: " + DlcManager.IsExpansion1Active());
		if (DlcManager.IsExpansion1Active())
		{
			Debug.Log("Loading Expansion1 assets from bundle");
			string path = Path.Combine(Application.streamingAssetsPath, DlcManager.GetContentBundleName("EXPANSION1_ID"));
			AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
			Debug.Assert(assetBundle != null, "Expansion1 is Active but its asset bundle failed to load");
			GameObject gameObject = assetBundle.LoadAsset<GameObject>("Expansion1Assets");
			Debug.Assert(gameObject != null, "Could not load the Expansion1Assets prefab");
			Expansion1Assets = Util.KInstantiate(gameObject, base.gameObject).GetComponent<BundledAssets>();
		}
	}
}
